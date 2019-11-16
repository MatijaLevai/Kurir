using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;

namespace Kurir
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPageActiveCourier : ContentPage
    {
        public MapPageActiveCourier()
        {
            InitializeComponent();

            MyMap.MoveToRegion( MapSpan.FromCenterAndRadius(new Position(37, -122), Distance.FromMiles(1)));
        }
        public MapPageActiveCourier(Location location,string Name)
        {
            InitializeComponent();
            Pin pinKurir = new Pin();
            pinKurir.Position = new Position(location.Latitude,location.Longitude);
            pinKurir.Label = Name;
            pinKurir.Type = PinType.Place;
            MyMap.Pins.Add(pinKurir);
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), new Distance(500)));
        }
    }
}