using Kurir.Models;
using Kurir.Persistance;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.CourierPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UncofirmedDeliveriesListPage : ContentPage
    {
       private HttpClient client = App.client;
       
        private SQLiteAsyncConnection _connection;
        private DeliveryService deliveryService;
       
        public List<DeliveryModel> listOfDeliveries;


        public UncofirmedDeliveriesListPage()
        {
            try
            {
                
                deliveryService = new DeliveryService();
                InitializeComponent();

                _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            }
            catch (Exception ex)
            {

                  DisplayAlert("Eror",ex.Message+ex.InnerException,"ok");
            }
            
            
        }

        


        protected async override void OnAppearing()
        {

            try
            {

                await GetDeliveriesFromServer();
            base.OnAppearing();
            }
            catch (Exception ex)
            {

                await DisplayAlert("Eror", ex.Message + ex.InnerException, "ok");
            }
        }

        private async Task<bool> GetDeliveriesFromServer()
        {
            var list = await deliveryService.GetUncofirmedForCourier();
            if (list != null)
            {
                listOfDeliveries = new List<DeliveryModel>(list);
                await _connection.CreateTableAsync<DeliveryModel>();

                foreach (var item in listOfDeliveries)
                {


                    int x = await _connection.UpdateAsync(item);
                    if (x == 0)
                    {
                        await _connection.InsertAsync(item);
                    }


                }

                DeliveryList.ItemsSource = listOfDeliveries;

                return true;
            }
            else
            {
                Message.Text = "No deliveries to show.";
                return false;
            }


        }
        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (DeliveryModel)e.SelectedItem;
            var selectedDelivery = listOfDeliveries.Where(d => d.DeliveryID == item.DeliveryID).First();
            await Navigation.PushAsync(new DeliveryDetailPage(selectedDelivery));
        }

        private async void DeliveryList_Refreshing(object sender, EventArgs e)
        {
            await GetDeliveriesFromServer();
            DeliveryList.EndRefresh();
        }

        public async void ConfirmAction(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);

            try
            {
                if (int.TryParse(mi.CommandParameter.ToString(), out int IDint))
                {
                    DeliveryModel selectedDelivery = listOfDeliveries.Where(x => x.DeliveryID == IDint).First();
                    if (selectedDelivery != null)
                    {
                        selectedDelivery.DeliveryStatus = 2;
                        var response = await deliveryService.EditDelivery(selectedDelivery);
                        if (response != null)
                        {
                            await DisplayAlert("Succses", "Delivery confirmed.", "ok");
                        }
                        else
                        {
                            await DisplayAlert("Delivery confimation failed.", " Try again. Check internet connection.", "ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Delivery confimation failed.", " Try again. Check internet connection.", "ok");
                    }
                }
                else
                {
                    await DisplayAlert("Delivery confimation failed.", " Try again. Check internet connection.", "ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Delivery confimation failed.", " Try again. Check internet connection. Error : " + ex.Message + ex.InnerException, "ok");
            }

        }


        public async void DeclineAction(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);

            try
            {
                if (int.TryParse(mi.CommandParameter.ToString(), out int IDint))
                {
                    DeliveryModel selectedDelivery = listOfDeliveries.Where(x => x.DeliveryID == IDint).First();
                    if (selectedDelivery != null)
                    {
                        selectedDelivery.DeliveryStatus = 1;
                        selectedDelivery.CourierID = 0;
                        var response = await deliveryService.EditDelivery(selectedDelivery);
                        if (response != null)
                        {
                            await DisplayAlert("Succses", "Delivery confirmed.", "ok");
                        }
                        else
                        {
                            await DisplayAlert("Delivery confimation failed.", " Try again. Check internet connection.", "ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Delivery confimation failed.", " Try again. Check internet connection.", "ok");
                    }
                }
                else
                {
                    await DisplayAlert("Delivery confimation failed.", " Try again. Check internet connection.", "ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Delivery confimation failed.", " Try again. Check internet connection. Error : " + ex.Message + ex.InnerException, "ok");
            }

        }
    }
}