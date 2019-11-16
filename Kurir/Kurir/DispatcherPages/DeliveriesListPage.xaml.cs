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
    public partial class DeliveriesListPage : ContentPage
    {

        private HttpClient client = App.client;
        private List<ExtendedDeliveryModel> listOfDeliveries;
        private List<CourierModel> listOfCouriers;
        // private SQLiteAsyncConnection _connection;
        private DeliveryService deliveryService;
        private UserService userService;
        private bool decending
        {get;set;
        }
        public DeliveriesListPage()
        { 

            decending = true;
            userService = new UserService();
            deliveryService = new DeliveryService();
            // _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            if(!await GetCouriers())
            {
                await DisplayAlert("Greska kod liste kurira","","ok");
            }
            await GetDeliveriesFromServer();

            SwitchDecending.IsToggled = decending;
            base.OnAppearing();
        }

        private async Task<bool> GetDeliveriesFromServer()
        {
            CourierModel cmNull = new CourierModel { CourierFullName = "???" };
            var list = await deliveryService.GetDeliveriesForDispatcher();
            if (list != null)
            {
                listOfDeliveries = new List<ExtendedDeliveryModel>(list);
                #region sqlLite
                //await _connection.DropTableAsync<DeliveryModel>();
                // await _connection.CreateTableAsync<DeliveryModel>();

                //int x = await _connection.UpdateAsync(item);
                //if (x == 0)
                //{
                //    await _connection.InsertAsync(item);
                //}
                #endregion
                if (listOfCouriers != null)
                {
                    foreach (var item in listOfDeliveries)
                    {
                        var cm = listOfCouriers.Where(u => u.CourierID == item.CourierID).FirstOrDefault();
                        if (cm != null)
                            item.Courier = cm;
                        else
                            item.Courier = cmNull;
                        //Delivery detail image 
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
                    }
                }

                DeliveryList.ItemsSource = listOfDeliveries;

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
        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (ExtendedDeliveryModel)e.SelectedItem;
            if (item != null)
            {
                var selectedDelivery = listOfDeliveries.Where(d => d.DeliveryID == item.DeliveryID).First();
                await Navigation.PushAsync(new EditDeliveryDispatcherPage(selectedDelivery));
                //await Navigation.PushAsync(new NewDeliveryDispatchPage());
            }


        }
        private void DeliveryListOrderBy()
        {if (listOfDeliveries != null)
            {
                if (decending)
                {

                    Sorter.Text = "Sortirano po opadajućem redosledu";
                    DeliveryList.ItemsSource = listOfDeliveries.OrderByDescending(d => d.DeliveryID).ToList();
                }
                else
                {
                    DeliveryList.ItemsSource = listOfDeliveries.OrderBy(d => d.DeliveryID);
                    Sorter.Text = "Sortirano rastućem redosledu";
                }
            }
        }
        private async void DeliveryList_Refreshing(object sender, EventArgs e)
        {
            await GetDeliveriesFromServer();
            DeliveryList.EndRefresh();
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

        private void Switch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var X = (Switch)sender;
            decending =X.IsToggled;
            DeliveryListOrderBy();
        }
    }
}