using Android.Support.V4.Content;
using Kurir.Models;
using Kurir.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Timers;

namespace Kurir.CourierPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultCourierPage : ContentPage
    {
        Location LastKnownlocation = new Location();
        private Timer LocationUpdateTimer;
        private LocationService locationService;
        public DefaultCourierPage()
        {
            //location;
            locationService = new LocationService();
            InitializeComponent();
        }
        private async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Location Newlocation = await locationService.GetCurrentLocation();
            if (Newlocation != LastKnownlocation)
                LastKnownlocation = Newlocation;
            if (await locationService.AddLocation(ParseLocation()) != null)
                try { await locationService.AddLocation(ParseLocation()); }
                catch (Exception ex) { throw ex; }
        }
        private LocationModel ParseLocation()
        {
            LocationModel l = new LocationModel()
            {
                 UserID = Convert.ToInt32(Application.Current.Properties["UserID"].ToString()) ,
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
        protected async override void OnAppearing()
        {
            LocationUpdateTimer = new Timer();
            LocationUpdateTimer.Interval = 50000;

            // Hook up the Elapsed event for the timer. 
            LocationUpdateTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            LocationUpdateTimer.AutoReset = true;

            // Start the timer
            LocationUpdateTimer.Enabled = true;

            LastKnownlocation = await  locationService.GetCurrentLocation();

            if (LastKnownlocation != null)
            {
               
                try
                {

                    if (await locationService.AddLocation(ParseLocation()) != null)
                        await DisplayAlert("Succses", "your location is sent to server.", "ok");
                    else
                    {
                        await DisplayAlert("Error", "500Error", "ok");
                    }
                }
                catch (Exception ex) { await DisplayAlert("Exception",ex.Message,"ok"); }
                    
                //}

            }
            else
            {
               await DisplayAlert("Location unknown!","Please allow EkoKurir App to access location.","ok");
            }
            base.OnAppearing();
        }
    }
}