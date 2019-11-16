using Kurir.Models;
using Kurir.Persistance;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using System.Timers;
namespace Kurir.DispatcherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveCourierListMapPage : ContentPage
    {
        private HttpClient client = App.client;
        private List<ActiveCourierModel> listOfCouriers;
        private SQLiteAsyncConnection _connection;
        private UserService userService;
        private DeliveryService deliveryService;
        private List<DeliveryModel> listOfCouriersDeliveries;
       // private System.Timers.Timer CourierUpdateTimer;
        //private Label Message;
       
        public ActiveCourierListMapPage()
        {
            deliveryService = new DeliveryService();
            
            userService = new UserService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        //    CourierUpdateTimer = new Timer()
        //    {
        //        Interval = 27000,
        //        AutoReset = false,
        //        Enabled = true
        //};

        //    // Hook up the Elapsed event for the timer. 
        //    CourierUpdateTimer.Elapsed += OnTimeEvent;

            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            if (await GetActiveCouriersFromServer())
            { }
                base.OnAppearing();

        }
        private void AddPinToMap(Location location,string name)
        {
            MyMap.Pins.Clear();
            Pin pinKurir = new Pin();
            pinKurir.Position = new Position(location.Latitude, location.Longitude);
            pinKurir.Label = name;
            pinKurir.Type = PinType.Place;
            MyMap.Pins.Add(pinKurir);
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), new Distance(500)));

        }
        private async Task<bool> GetDeliveriesForCourierFromServer(int CourierID)
        {
            listOfCouriersDeliveries = new List<DeliveryModel>( await deliveryService.GetGetTodaysDeliveriesForCourier(CourierID));
            if (listOfCouriersDeliveries.Count() > 0)
            {
                foreach (var item in listOfCouriersDeliveries)
                {
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

                    DeliveryList.ItemsSource = listOfCouriersDeliveries;
                return true;
            }
            else return false;
        }
        private async Task<bool> GetActiveCouriersFromServer()
        {
            try
            {
                var list = await userService.GetActiveCouriers();
                if (list != null)
                {
                    listOfCouriers = new List<ActiveCourierModel>(list);
                    //await _connection.DropTableAsync<DeliveryModel>();
                    // await _connection.CreateTableAsync<ActiveCourierModel>();

                    foreach (var item in listOfCouriers)
                    {
                        if (item.Lat > 0 && item.Long > 0)
                        {
                            item.StatusImageSource = "zeleni50.png";
                        }
                        else
                        {
                            item.StatusImageSource = "zuti50.png";
                        }

                        //int x = await _connection.UpdateAsync(item);
                        //if (x == 0)
                        //{
                        //    await _connection.InsertAsync(item);
                        //}


                    }

                    ActiveCourierList.ItemsSource = listOfCouriers;
                    // Message.Text = "";
                    return true;
                }
                else
                {
                    // Message.Text = "Trenutno nema aktivnih kurira.";
                    return false;
                }

            }
            catch (Exception ex) {
                await DisplayAlert("",ex.Message+ex.InnerException, "ok.");
                return false;
            }
        }
        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (ActiveCourierModel)e.SelectedItem;
            try
            {
                if (!await GetDeliveriesForCourierFromServer(item.CourierID))
                {
                    //Message.Text = "Kurir " + item.CourierFullName +" trenutno nema dostava.";
                }
            }
            catch {
                await DisplayAlert("BaraBara", "Kurir"+item.CourierFullName+" nema dostava.", "ok.");
            }

            if (item.Lat > 0 && item.Long > 0)
            {
                Location l = new Location(item.Lat, item.Long, item.DToffSet);
                l.Altitude = item.Alt;
                try
                {
                    AddPinToMap(l,item.CourierFullName);
                   //await Navigation.PushAsync(new MapPageActiveCourier(l, item.CourierFullName));
                }
                catch (Exception ex) { await DisplayAlert("", ex.Message + ex.InnerException, "ok"); }
                //await Map.OpenAsync(l);
            }
            else
                await DisplayAlert("Unknown location", "Courier can not be located at this time. Please try again in few moments.", "ok.");


        }
        private async void OnDeliveerySelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (DeliveryModel)e.SelectedItem;
            var selectedDelivery = listOfCouriersDeliveries.Where(d => d.DeliveryID == item.DeliveryID).First();
            await Navigation.PushAsync(new EditDeliveryDispatcherPage(selectedDelivery.ConvertToExtended()));
            //await Navigation.PushAsync(new NewDeliveryDispatchPage());


        }
        //private async void OnTimeEvent(Object source, ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        CourierUpdateTimer.Stop();
        //        CourierUpdateTimer.Enabled = false;
        //        if (await GetActiveCouriersFromServer())
        //        { }
        //        CourierUpdateTimer.Enabled = true;
        //        CourierUpdateTimer.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("",ex.Message+ex.InnerException,"ok");
        //    }
        //}

    }
}