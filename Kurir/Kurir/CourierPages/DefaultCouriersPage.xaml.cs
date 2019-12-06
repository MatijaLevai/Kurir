using Kurir.Helpers;
using Kurir.Models;
using Kurir.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.CourierPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultCouriersPage : MasterDetailPage
    {
        private UserService userService;
        private DeliveryService deliveryService;
        private LocationService locationService;
        Location LastKnownlocation = new Location();
        List<DeliveryModel> couriersDeliveryList;
       // private Timer LocationUpdateTimer;
        INotificationManager notificationManager;
        //int notificationNumber = 0;


        //private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        //{
        //    LocationUpdateTimer.Stop();
        //    try
        //    {
        //        Location Newlocation = await locationService.GetCurrentLocation();
        //        if (Newlocation != LastKnownlocation)
        //            LastKnownlocation = Newlocation;
        //        if (await locationService.AddLocation(ParseLastKnownLocation()) != null)
        //        { }
        //    }
        //    catch (Exception ex) {
        //        await DisplayAlert("ex location service", ex.Message + ex.InnerException, "ok");
        //    }
        //    if (Device.RuntimePlatform == Device.Android)
        //    {
        //        try
        //        {
        //            if (Application.Current.Properties.ContainsKey("UserID"))
        //            {
        //                if (await userService.CheckIfCurrentUserRoleIsCourier(Convert.ToInt32(Application.Current.Properties["UserID"])))
        //                {
        //                    var list = await deliveryService.GetUncofirmedForCourier();
        //                    if (list.Count() > 0)
        //                    {
        //                        notificationNumber++;
        //                        string title = $"Local Notification #{notificationNumber} BaraBara";
        //                        string message = $"You have now received {notificationNumber} notifications!" + Environment.NewLine + "Stigla nova dostava.";
        //                        notificationManager.ScheduleNotification(title, message);
        //                    }
        //                }

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            await DisplayAlert("ex notification service", ex.Message + ex.InnerException, "ok");
        //        }
        //    }
        //    LocationUpdateTimer.Start();
        //}
        private LocationModel ParseLastKnownLocation()
        {
            try
            {
                LocationModel l = new LocationModel()
                {
                    UserID = Convert.ToInt32(Application.Current.Properties["UserID"].ToString()),
                    DToffSet = LastKnownlocation.Timestamp,
                    Longitude = LastKnownlocation.Longitude,
                    Latitude = LastKnownlocation.Latitude,
                };
                double alt = 0;
                try { alt = Convert.ToDouble(LastKnownlocation.Altitude); }
                catch { alt = 0; }
                if (alt >= 0)
                    l.Altitude = alt;// Convert.ToDouble(location.Altitude);
                                     //l.Altitude =location.Altitude;

                l.DToffSet = LastKnownlocation.Timestamp;
                return l;
            }
            catch (Exception ex) { Console.WriteLine(ex.InnerException + ex.Message); return new LocationModel(); };
        }
        protected override async void OnAppearing()
        {
            

            base.OnAppearing();
            if (await InitialLocationService())
            { }


        }
       
        private async Task<bool> InitialLocationService()
        {
            // try
        //    {

                LastKnownlocation = await locationService.GetCurrentLocation();

                if (LastKnownlocation != null)
                {
                    var location = await locationService.AddLocation(ParseLastKnownLocation());
                    if (location == null)
                    {
                        await DisplayAlert("Error", "500Error", "ok");
                        return false;
                    }

                    else
                    {
                        return true;
                    }
                }
                else
                {
                    await DisplayAlert("Location unknown!", "Please allow EkoKurir App to access location.", "ok");
                    return false;
                }
            //}
            //catch (Exception ex) { await DisplayAlert("Exception", ex.Message, "ok");return false; }
        }
        public DefaultCouriersPage()
        {

            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            NavigationPage.SetHasBackButton(this,false);
            locationService = new LocationService();
            //LocationUpdateTimer = new Timer();
            Device.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                // Do something
                TimerUpdate();
                return true;

                // True = Repeat again, False = Stop the timer
            });
            couriersDeliveryList = new List<DeliveryModel>();
            //LocationUpdateTimer.Interval = 25000;


            //// Hook up the Elapsed event for the timer. 
            //LocationUpdateTimer.Elapsed += OnTimedEvent;

            //// Have the timer fire repeated events (true is the default)
            //LocationUpdateTimer.AutoReset = false;

            //// Start the timer
            //LocationUpdateTimer.Enabled = true;
            deliveryService = new DeliveryService();
            userService = new UserService();
            if (Device.RuntimePlatform == Device.Android)
            {
                

                notificationManager = DependencyService.Get<INotificationManager>();
                notificationManager.NotificationReceived += (sender, eventArgs) =>
                {
                    var evtData = (NotificationEventArgs)eventArgs;
                    ShowNotification(evtData.Title, evtData.Message);
                };
            }
            
        }
        private async void TimerUpdate()
        {
            try
            {
                Location Newlocation = await locationService.GetCurrentLocation();
                if (Newlocation != LastKnownlocation)
                    LastKnownlocation = Newlocation;
                if (await locationService.AddLocation(ParseLastKnownLocation()) != null)
                { }
            }
            catch (Exception ex)
            {
                await DisplayAlert("ex location service", ex.Message + ex.InnerException, "ok");
            }
            if (Device.RuntimePlatform == Device.Android)
            {
                try
                {
                    if (Application.Current.Properties.ContainsKey("UserID"))
                    {
                        if (await userService.CheckIfCurrentUserRoleIsCourier(Convert.ToInt32(Application.Current.Properties["UserID"])))
                        {
                            var list = await deliveryService.GetUncofirmedForCourier();
                            if (list.Count() > 0)
                            {
                                foreach (var item in list)
                                {
                                    if (!Contains(item))
                                    {
                                        couriersDeliveryList.Add(item);
                                        string title = $"Od : " + item.StartAddress.Address;
                                        string message = $"Do : " + item.EndAddress.Address;
                                        notificationManager.ScheduleNotification(title, message);
                                    }
                                }

                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("ex notification service", ex.Message + ex.InnerException, "ok");

                }
            }

        }
        private bool Contains(DeliveryModel item)
        {
            foreach (var itm in couriersDeliveryList)
            {
                if (itm.DeliveryID == item.DeliveryID)
                    return true;
            }
             return false;
        }
        private void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert( title, message, "ok");
            });
        }
        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as DefaultCouriersPageMenuItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
        protected override bool OnBackButtonPressed()
        {
            //if (Device.RuntimePlatform == Device.Android)
            //{ HideService.BackPress(); }
            return true;
            //return base.OnBackButtonPressed();
        }
    }
}