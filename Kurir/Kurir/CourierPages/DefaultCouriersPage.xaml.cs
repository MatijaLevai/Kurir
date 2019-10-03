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
        private LocationService locationService;
        Location LastKnownlocation = new Location();
        private Timer LocationUpdateTimer;

        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            LocationUpdateTimer.Stop();
            try
            {
                Location Newlocation = await locationService.GetCurrentLocation();
            if (Newlocation != LastKnownlocation)
                LastKnownlocation = Newlocation;
                if (await locationService.AddLocation(ParseLastKnownLocation()) != null)
                {}
                    //await locationService.AddLocation(ParseLastKnownLocation());
                  
                    //await DisplayAlert("location", "Location sent to server", "ok");
                }
                catch (Exception ex) {
                // await DisplayAlert("ex", ex.Message + ex.InnerException, "ok"); 
                }
            LocationUpdateTimer.Start();
        }
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
        protected async override void OnAppearing()
        {
           

            LastKnownlocation = await locationService.GetCurrentLocation();

            if (LastKnownlocation != null)
            {

                try
                {

                    if (await locationService.AddLocation(ParseLastKnownLocation()) != null)
                        await DisplayAlert("Succses", "your location is sent to server.", "ok");
                    else
                    {
                        await DisplayAlert("Error", "500Error", "ok");
                    }
                }
                catch (Exception ex) { await DisplayAlert("Exception", ex.Message, "ok"); }

                //}

            }
            else
            {
                await DisplayAlert("Location unknown!", "Please allow EkoKurir App to access location.", "ok");
            }
            base.OnAppearing();
        }
        public DefaultCouriersPage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            NavigationPage.SetHasBackButton(this,false);
            locationService = new LocationService();
            LocationUpdateTimer = new Timer();
            LocationUpdateTimer.Interval = 50000;

            // Hook up the Elapsed event for the timer. 
            LocationUpdateTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            LocationUpdateTimer.AutoReset = false;

            // Start the timer
            LocationUpdateTimer.Enabled = true;
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
            return true;
            //return base.OnBackButtonPressed();
        }
    }
}