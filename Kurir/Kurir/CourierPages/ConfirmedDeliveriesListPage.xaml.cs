using Kurir.Models;
using Kurir.Persistance;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Android.Locations;
using Android.Content;

namespace Kurir.CourierPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmedDeliveriesListPage : ContentPage
    {
        LocationService locationService;
        AddressService addressService;
        public ConfirmedDeliveriesListPage()
        {
            try
            {
                addressService = new AddressService();
                locationService = new LocationService();
                deliveryService = new DeliveryService();
                InitializeComponent();

                _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            }
            catch (Exception ex)
            {

                DisplayAlert("Eror", ex.Message + ex.InnerException, "ok");
            }
        }
        private HttpClient client = App.client;

        private SQLiteAsyncConnection _connection;
        private DeliveryService deliveryService;

        public List<DeliveryModel> listOfDeliveries;

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
            var list = await deliveryService.GetCofirmedForCourier();
            if (list != null)
            {
                listOfDeliveries = new List<DeliveryModel>(list);
                //await _connection.CreateTableAsync<DeliveryModel>();

                //foreach (var item in listOfDeliveries)
                //{


                //    int x = await _connection.UpdateAsync(item);
                //    if (x == 0)
                //    {
                //        await _connection.InsertAsync(item);
                //    }


                //}

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
        public async void DirectionAction(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            if (int.TryParse(mi.CommandParameter.ToString(), out int IDint))
                {
                    DeliveryModel selectedDelivery = listOfDeliveries.Where(x => x.DeliveryID == IDint).First();
                    if (selectedDelivery != null)
                    {
                        if (selectedDelivery.StartAddress.LocationID > 1)
                        {
                            LocationModel l = await locationService.GetByID(Convert.ToInt32(selectedDelivery.StartAddress.LocationID));
                            await Map.OpenAsync(l.Latitude, l.Longitude, new MapLaunchOptions() { NavigationMode = NavigationMode.Walking });
                        }
                    }
                }
            
            }
        public async void StartAction(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);

            try
            {
                if (int.TryParse(mi.CommandParameter.ToString(), out int IDint))
                {
                    DeliveryModel selectedDelivery = listOfDeliveries.Where(x => x.DeliveryID == IDint).First();
                    if (selectedDelivery != null)
                    {
                        selectedDelivery.DeliveryStatus = 3;
                        Xamarin.Essentials.Location l = await locationService.GetCurrentLocation();
                        if (l != null)
                        {
                            LocationModel lm = new LocationModel() { Latitude = l.Latitude, Longitude = l.Longitude, Altitude = Convert.ToDouble(l.Altitude.ToString()), UserID = Convert.ToInt32(Application.Current.Properties["UserID"].ToString()), DToffSet = l.Timestamp };

                            LocationModel lmReturned = await locationService.AddLocation(lm);
                            if (lmReturned.LocationID > 1)
                            {
                                FullAddressModel startAddress = await addressService.GetAddressByIDAsync(selectedDelivery.StartAddressID);
                                startAddress.LocationID = lmReturned.LocationID;
                            }
                            selectedDelivery.StartTime = DateTime.Now;
                            var response = await deliveryService.EditDelivery(selectedDelivery);
                            if (response != null)
                            {
                                await DisplayAlert("Succses", "Delivery started.", "ok");
                                await GetDeliveriesFromServer();
                            }
                            else
                            {
                                await DisplayAlert("Delivery confimation failed.", " Try again. Check internet connection.", "ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Location Error.", "Please enable location service on device.", "ok");
                        }
                        
                    }
                    else
                    {
                        await DisplayAlert("Delivery confimation failed.", " Try again.", "ok");
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