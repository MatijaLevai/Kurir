using Kurir.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using Kurir.Models;
using SQLite;

namespace Kurir.UserPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddLocationToAddressPage : ContentPage
    {
        LocationService locationService;
        Location UserLocatation;
        Location SelectedLocation;
        IDictionary<string, Pin> Pins;
        Pin SelectedPin;
        Pin StartPin;
        Pin EndPin;
        DeliveryModel delivery;
        bool SenderReciver;//true=Sender  false=Reciver
       // private readonly SQLiteAsyncConnection _connection;
      
        public AddLocationToAddressPage(DeliveryModel delivery ,bool sr)
        {
            //_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            //_connection.CreateTableAsync<LocationModel>();
            Pins = new Dictionary<string, Pin>();
            SenderReciver = sr;
            if (delivery != null)
            {
                this.delivery = delivery;
            }
            locationService = new LocationService();
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            UserLocatation = await locationService.GetCurrentLocation();
            Pin userPin = new Pin() { Position = new Position(UserLocatation.Latitude, UserLocatation.Longitude), Type = PinType.Generic, Label = Application.Current.Properties["Name"] + "'s  location" };
            #region Foreach Type Of Pin
            //foreach (var item in MyMap.Pins)
            //{
            //    switch (item.Type)
            //    {
            //        case PinType.Generic:
            //            item.Address = "You are here.";
            //            break;
            //        case PinType.Place:
            //            item.Address = delivery.EndAddress.Address;
            //            break;
            //        case PinType.SavedPin:
            //            item.Address = delivery.StartAddress.Address;
            //            break;
            //        default:
            //            break;
            //    }
            //}
            #endregion
            userPin.Clicked += (sender, e) => {
                DisplayAlert("Your Location", "Longitude : " + userPin.Position.Longitude + Environment.NewLine + "Latitude : " + userPin.Position.Latitude, "ok");
            };
            MyMap.Pins.Add(userPin);
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(MyMap.Pins.First().Position, Distance.FromMiles(0.9)));
            MyMap.SetVisibleRegion(MapSpan.FromCenterAndRadius(new Position(45.250505, 19.791081), Distance.FromMiles(7)));

            if (SenderReciver)
            {
                if (delivery.StartAddress.LocationID > 0)
                {
                    var l = await locationService.GetByID(Convert.ToInt32(delivery.StartAddress.LocationID));
                    StartPin = new Pin() { Position = new Position(l.Latitude, l.Longitude), Type = PinType.SavedPin, Label = "Sender's  location" };
                   
                    StartPin.Clicked += (sender, e) => {
                        DisplayAlert("Senders Location", "Longitude : " + StartPin.Position.Longitude + Environment.NewLine + "Latitude : " + StartPin.Position.Latitude, "ok");
                    };
                    MyMap.Pins.Add(StartPin);

                }
            }
            else
            {
                if (delivery.StartAddress.LocationID > 0)
                {
                    var l = await locationService.GetByID(Convert.ToInt32(delivery.StartAddress.LocationID));
                    StartPin = new Pin() { Position = new Position(l.Latitude, l.Longitude), Type = PinType.SavedPin, Label = "Sender's  location" };
                    
                    StartPin.Clicked += (sender, e) => {
                        DisplayAlert("Senders Location", "Longitude : " + StartPin.Position.Longitude + Environment.NewLine + "Latitude : " + StartPin.Position.Latitude, "ok");
                    };
                    MyMap.Pins.Add(StartPin);
                }
                if (delivery.EndAddress.LocationID > 0)
                {
                    var l = await locationService.GetByID(Convert.ToInt32(delivery.EndAddress.LocationID));
                    EndPin = new Pin() { Position = new Position(l.Latitude, l.Longitude), Type = PinType.Place, Label = "Reciver's  location" };
                   
                    EndPin.Clicked += (sender, e) => {
                        DisplayAlert("Recivers Location", "Longitude : " + EndPin.Position.Longitude + Environment.NewLine + "Latitude : " + EndPin.Position.Latitude, "ok");
                    };
                    MyMap.Pins.Add(EndPin);
                }
            }
           


            base.OnAppearing();
        }
        private void CheckPin(Pin pin)
        {
            if (pin != null)
            {
                if (MyMap.Pins.Contains(pin))
                {
                    MyMap.Pins.Remove(pin);

                }

            }
        }
        private void MyMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            Position p = e.Position;
            //if (MyMap.Pins.Count >= 2)
            //{
            //    MyMap.Pins.Remove(SelectedPin);
            //}

            SelectedLocation = new Location()
            {
                Latitude = p.Latitude,
                Longitude = p.Longitude
            };
            if (SenderReciver)
            {
                CheckPin(StartPin);
                StartPin = new Pin
                {
                    Position =p,
                    Label = "Senders location",
                    Type = PinType.SavedPin
                };
                MyMap.Pins.Add(StartPin);
            }
            else
            {
                CheckPin(EndPin);
                EndPin = new Pin
                {
                    Position = p,
                    Label = "Recivers Location",
                    Type = PinType.Place
                };
                MyMap.Pins.Add(EndPin);
            }
        }
        private async void PopToAddress_Clicked(object sender, EventArgs e)
        {
            if (await AddLocationToAddress())
            {
                
                await Navigation.PopModalAsync();
            }
        }

        private async void PopToAddressWithUsersLocation_Clicked(object sender, EventArgs e)
        {
            SelectedLocation = UserLocatation;
            if (await AddLocationToAddress())
            {
                await Navigation.PopModalAsync();
            }
        }
        //private async Task<bool> AddLocationToSqlite(LocationModel lm)
        //{
        //    try
        //    {
        //        int x = await _connection.InsertAsync(lm);
        //        if (x > 0)
        //            return true;
        //        else return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Error",ex.Message+ex.InnerException,"ok");
        //        return false;
        //    }
            
        //}
        private async Task<bool> AddLocationToAddress()
        {
            if (SelectedLocation != null)
            {
                try
                {
                    LocationModel l = new LocationModel()
                    {
                        Latitude = SelectedLocation.Latitude,
                        Longitude = SelectedLocation.Longitude,
                        DToffSet = DateTimeOffset.Now,
                        UserID = Convert.ToInt32(Application.Current.Properties["UserID"])
                    };

                    try { l.Altitude = Convert.ToDouble(SelectedLocation.Altitude); }
                    catch { l.Altitude = 0; }
                    var locationFromDb = await locationService.AddLocation(l);
                    if (SenderReciver)
                    {
                        Application.Current.Properties.Add("StartLocationID", locationFromDb.LocationID);
                    }
                    else
                    {
                        Application.Current.Properties.Add("EndLocationID", locationFromDb.LocationID);
                    }
                    //if (await AddLocationToSqlite(locationFromDb))
                    return true;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("error", "msg: " + ex.Message + "IE : " + ex.InnerException, "ok");
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}