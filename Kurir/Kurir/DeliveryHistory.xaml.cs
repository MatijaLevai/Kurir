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

        public DeliveryHistory()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {

            var res = await client.GetAsync(Application.Current.Properties["ServerLink"].ToString() + "api/deliveries/GetDeliveriesForUser/" + Application.Current.Properties["UserID"].ToString());
            if (res.StatusCode==System.Net.HttpStatusCode.OK)
            {
                var resString = await res.Content.ReadAsStringAsync();

                listOfDeliveries = JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);
                if (listOfDeliveries.Count() > 0)
                {
                    //await _connection.DropTableAsync<DeliveryModel>();
                    await _connection.CreateTableAsync<DeliveryModel>();

                    foreach (var item in listOfDeliveries)
                    {
                       int x = await _connection.UpdateAsync(item);
                        if (x==0)
                        { await _connection.InsertAsync(item);
                        }
                    }
                    DeliveryList.ItemsSource = listOfDeliveries;
                }
                else
                {
                    Message.Text = "No deliveries to show.";
                }

            }


            base.OnAppearing();
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
            DeliveryList.ItemsSource = await _connection.Table<DeliveryModel>().ToListAsync();
            DeliveryList.EndRefresh();
        }
    }
    }