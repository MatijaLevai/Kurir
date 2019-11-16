using Kurir.Models;
using Kurir.Persistance;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.DispatcherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListOfAllDeliveries : ContentPage
    {
       
        private HttpClient client = App.client;
        private List<ExtendedDeliveryModel> listOfDeliveries;
        private List<CourierModel> listOfCouriers;
        private SQLiteAsyncConnection _connection;
        private DeliveryService deliveryService;
        private UserService userService;
        private bool decending;
           
        public ListOfAllDeliveries()
        {
            decending = true;
            userService = new UserService();
            deliveryService = new DeliveryService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            try
            {
                if (!(await GetCouriers() && !(await GetDeliveriesFromServer(DateTime.Now,DateTime.Now))))
                {
                    Message.IsVisible = true;
                    Message.IsEnabled = true;
                }
                else
                {
                    Message.IsVisible = false;
                    Message.IsEnabled = false;
                }
            }
            catch (Exception ex) { await DisplayAlert("Connection Error!",ex.Message+ex.InnerException,"ok"); };

            SwitchDecending.IsToggled = decending;
            base.OnAppearing();
        }

        private async Task<bool> GetDeliveriesFromServer(DateTime OD, DateTime DO)
        {
            CourierModel cmNull = new CourierModel { CourierFullName = "???" };
            ////erro casting list of objects
            ///
            var list = await deliveryService.GetDeliveriesByDate(OD,DO);
            if (list != null)
            {
                listOfDeliveries = new List<ExtendedDeliveryModel>();
                
                    foreach (var item in list)
                    {
                    switch (item.DeliveryStatus)
                    {
                        case 4:
                            item.DeliveryStatusImageSource = "delivered.png";
                            break;
                        case 3:
                            item.DeliveryStatusImageSource = "zeleni50.png";
                            break;
                        case 2:
                            item.DeliveryStatusImageSource = "zuti50.png";
                            break;
                        case 1:
                            item.DeliveryStatusImageSource = "crveni50.png";
                            break;
                        default:
                            item.DeliveryStatusImageSource = "crveni50.png";
                            break;
                    }
                    ExtendedDeliveryModel extD = new ExtendedDeliveryModel();
                        extD = item.ConvertToExtended();
                    if (listOfCouriers != null)
                    {
                        var cm = listOfCouriers.Where(u => u.CourierID == item.CourierID).FirstOrDefault();
                        if (cm != null)
                            extD.Courier = cm;
                        else
                            extD.Courier = cmNull;
                    }
                        //Delivery detail image 
                        
                        listOfDeliveries.Add(extD);

                    
                    }
                DeliveryList.ItemsSource = listOfDeliveries;
                DeliveryListOrderBy();
                return true;
            }
            else
            {
                Message.Text = "No deliveries to show.";
                Message.IsVisible = true;
                Message.IsEnabled = true;
                return false;
            }



        }
        private async Task<bool> GetCouriers()
        {
            var list = await userService.GetCouriers();
            if (list != null)
            {
                listOfCouriers = new List<CourierModel>(list);
                return true;
            }
            else return false;

        }
        public async void DeclineAction(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);

            try
            {
                if (int.TryParse(mi.CommandParameter.ToString(), out int IDint))
                {
                    ExtendedDeliveryModel selectedDelivery = listOfDeliveries.Where(x => x.DeliveryID == IDint).First();
                    if (selectedDelivery != null)
                    {
                        ResponseModel response = await deliveryService.DeleteDelivery(selectedDelivery.DeliveryID);
                        if (response.Success)
                        {
                            listOfDeliveries.Remove(selectedDelivery);
                            DeliveryList.ItemsSource = null;
                            DeliveryList.ItemsSource = listOfDeliveries;
                            
                           
                        }
                            await DisplayAlert("", response.Message, "ok");
                    }
                    else
                    {
                        await DisplayAlert("Greška", " Brisanje neuspešno", "ok");
                    }
                }
                else
                {
                    await DisplayAlert("Greška", " Brisanje neuspešno", "ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Brisanje nije uspelo", " Pokušaj ponovo. Proveri internet konekciju. Tekst greške : " + ex.Message + ex.InnerException, "ok");
            }

        }
        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {/////
            if (e.SelectedItem != null)
            {
                var item = (DeliveryModel)e.SelectedItem;
                var selectedDelivery = listOfDeliveries.Where(d => d.DeliveryID == item.DeliveryID).First();
                await Navigation.PushAsync(new EditDeliveryDispatcherPage(selectedDelivery));
            }
            //await Navigation.PushAsync(new NewDeliveryDispatchPage());


        }
        private void DeliveryListOrderBy()
        {
            if (listOfDeliveries != null)
            {
                if (decending)
                {

                    Sorter.Text = "Opadajući redosled";
                    DeliveryList.ItemsSource = listOfDeliveries.OrderByDescending(d => d.DeliveryID).ToList();
                }
                else
                {
                    DeliveryList.ItemsSource = listOfDeliveries.OrderBy(d => d.DeliveryID);
                    Sorter.Text = "Rastući redosled";
                }
            }
        }
        private async void DeliveryList_Refreshing(object sender, EventArgs e)
        {
            await GetDeliveriesFromServer(startDatePicker.Date, endDatePicker.Date);
            DeliveryList.EndRefresh();
        }
       
        async void OnDateSelectedStart(object sender, DateChangedEventArgs args)
        {
            var startDate = (DatePicker)sender;
            if (startDate.Date > endDatePicker.Date)
            { endDatePicker.Date = startDate.Date; }
            endDatePicker.MinimumDate = startDate.Date;
            await GetDeliveriesFromServer(startDate.Date,endDatePicker.Date);
        }
        async void OnDateSelectedEnd(object sender, DateChangedEventArgs args)
        {
            var endDate = (DatePicker)sender;
            if (endDate.Date < startDatePicker.Date)
            { startDatePicker.Date = endDate.Date; }
            startDatePicker.MaximumDate = endDate.Date;
            await GetDeliveriesFromServer(startDatePicker.Date, endDatePicker.Date);
        }
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await GetDeliveriesFromServer(startDatePicker.Date, endDatePicker.Date);
            DeliveryListOrderBy();
        }

        private void SwitchDecending_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var X = (Switch)sender;
            decending = X.IsToggled;
            DeliveryListOrderBy();
        }
    }
}