using Kurir.Models;
using Kurir.Persistance;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryHistory : ContentPage
    {
        private HttpClient client = App.client;
        private List<DeliveryModel> listOfDeliveries;
        private SQLiteAsyncConnection _connection;
        private DeliveryService deliveryService;

        public DeliveryHistory()
        {
            deliveryService = new DeliveryService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            await GetDeliveriesFromServer();
            base.OnAppearing();
        }
        private async Task<bool> GetDeliveriesFromServer()
        {
            if (Application.Current.Properties.ContainsKey("UserID"))
            {


                var list = await deliveryService.GetDeliveriesForUser();
                if (list != null)
                {
                    listOfDeliveries = new List<DeliveryModel>(list);
                    //await _connection.DropTableAsync<DeliveryModel>();
                    await _connection.CreateTableAsync<DeliveryModel>();
                   
                    foreach (var item in listOfDeliveries)
                    {
                        //Delivery detail image 
                        if (item.EndTime > item.StartTime)
                        {
                            item.DeliveryStatusImageSource ="delivered.png";
                        }
                        else if (item.StartTime > item.CreateTime)
                        {
                            item.DeliveryStatusImageSource ="zeleni50.png";
                        }
                        else
                        {
                            item.DeliveryStatusImageSource ="zuti50.png";
                        }

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
            else return false;
        }
        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item =(DeliveryModel) e.SelectedItem;
           var selectedDelivery = listOfDeliveries.Where(d => d.DeliveryID == item.DeliveryID).First();
            //provera da li je kurir vec preuzeo dostavu ili je proslo 5 minuta od slanja dostave, tada korisniku nije dozvoljeno menjanje dostave
            if (selectedDelivery.StartTime > selectedDelivery.CreateTime || selectedDelivery.CreateTime.AddMinutes(5) < DateTime.Now)
            {
                await Navigation.PushAsync(new DeliveryDetailPage(selectedDelivery));
            }
            else
            {
                await Navigation.PushAsync(new NewDelivery(selectedDelivery));
               
            }
        }
        private async void DeliveryList_Refreshing(object sender, EventArgs e)
        {
            await GetDeliveriesFromServer();
            DeliveryList.EndRefresh();
        }
        
    }
    }