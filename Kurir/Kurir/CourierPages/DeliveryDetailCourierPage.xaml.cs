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

namespace Kurir.CourierPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryDetailCourierPage : ContentPage
    {
       
        private HttpClient _client = App.client;
        private DeliveryModel delivery;
        private List<PaymentTypeModel> paymentTypes;
        private List<DeliveryTypeModel> deliveryTypes;
        private readonly SQLiteAsyncConnection _connection;
        private SQLiteHelper helper;
        private LocationService locationService;
        public DeliveryDetailCourierPage(DeliveryModel delivery)
        {
            locationService = new LocationService();
            helper = new SQLiteHelper();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            this.delivery = delivery;
            BindingContext = this.delivery;
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            if (await helper.UpdateSQLiteDbWithPayAndDelTypes())
            {
                try
                {
                    paymentTypes = await _connection.Table<PaymentTypeModel>().ToListAsync();
                    deliveryTypes = await _connection.Table<DeliveryTypeModel>().ToListAsync();
                    PaymentTypeModel p = paymentTypes.Where(d => d.PaymentTypeID == delivery.PaymentTypeID).FirstOrDefault();
                    if (p != null)
                        PaymentType.Text = p.PaymentTypeName;
                    DeliveryTypeModel D = deliveryTypes.Where(d => d.DeliveryTypeID == delivery.DeliveryTypeID).FirstOrDefault();
                    if (D != null)
                        DeliveryType.Text = D.DeliveryTypeName;
                    if(delivery.DeliveryStatus==4)
                    directionsBtn.IsVisible = false;
                }
                catch (Exception ex) { await DisplayAlert("error", ex.Message + ">||<" + ex.InnerException, "ok"); }

            }
            else throw new Exception("No internet or bad request to server");

            base.OnAppearing();
        }

        private async void ButtonGetDirections_Clicked(object sender, EventArgs e)
        {
            if (delivery.DeliveryStatus == 2)
            {
                if (delivery.StartAddress.LocationID > 1)
                {
                    LocationModel l = await locationService.GetByID(Convert.ToInt32(delivery.StartAddress.LocationID));
                    await Map.OpenAsync(l.Latitude, l.Longitude, new MapLaunchOptions() { NavigationMode = NavigationMode.Bicycling });
                }
            }
            else if (delivery.DeliveryStatus == 3)
            {
                if (delivery.EndAddress.LocationID > 1)
                {
                    LocationModel l = await locationService.GetByID(Convert.ToInt32(delivery.StartAddress.LocationID));
                    await Map.OpenAsync(l.Latitude, l.Longitude, new MapLaunchOptions() { NavigationMode = NavigationMode.Bicycling });
                }
            }
        }
    }
}