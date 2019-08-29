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
        private HttpClient client = new HttpClient();
        private List<DeliveryModel> listOfDeliveries;
        private Frame ekoFrame;
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
                     DateTime dt = new DateTime(1,1,1,0,0,0,0);
                    foreach (var item in listOfDeliveries)
                    {
                        //Delivery detail image 
                        if (item.EndTime > dt)
                        {
                            item.DeliveryStatusImageSource ="delivered.png";
                        }
                        else if (item.StartTime > dt)
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
           
           var selecterdDelivery = listOfDeliveries.Where(d => d.DeliveryID == item.DeliveryID).First();
            if (selecterdDelivery.StartTime > new DateTime(1, 1, 1, 0, 0, 0, 0))
            {
                await Navigation.PushAsync(new DeliveryDetailPage(selecterdDelivery));
            }
            else
            {
                await Navigation.PushAsync(new NewDelivery(selecterdDelivery));
               
            }
        }

        private async void DeliveryList_Refreshing(object sender, EventArgs e)
        {
            await GetDeliveriesFromServer();
            DeliveryList.EndRefresh();
        }
        //private string SwitchImageSource(string s)
        //{
        //    switch (Device.RuntimePlatform)
        //    {
        //        case Device.UWP:
        //            s= "Kurir." + s;
        //            break;
        //        case Device.Android:
        //            s = "Kurir." + s;
        //            break;
                
        //    }
        //    return s;

        //}
    }
    }