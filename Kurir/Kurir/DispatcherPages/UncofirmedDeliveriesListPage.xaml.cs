using Kurir.Models;
using Kurir.Persistance;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.DispatcherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UncofirmedDeliveriesListPage : ContentPage
    {
        private HttpClient client = App.client;
        private List<DeliveryModel> listOfDeliveries;
        private SQLiteAsyncConnection _connection;
        private DeliveryService deliveryService;
        public UncofirmedDeliveriesListPage()
        {
            deliveryService = new DeliveryService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            await GetUncofirmedDeliveriesFromServer();
            base.OnAppearing();
        }

        private async Task<bool> GetUncofirmedDeliveriesFromServer()
        {
            var list = await deliveryService.GetUncofirmedDeliveriesForDispatcher();
            if (list != null)
            {
                listOfDeliveries = new List<DeliveryModel>(list);
                //await _connection.DropTableAsync<DeliveryModel>();
                await _connection.CreateTableAsync<DeliveryModel>();

                foreach (var item in listOfDeliveries)
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
                    //int x = await _connection.UpdateAsync(item);
                    //if (x == 0)
                    //{
                    //    await _connection.InsertAsync(item);
                    //}


                }

                DeliveryList.ItemsSource = listOfDeliveries;

                return true;
            }
            else
            {
                Message.Text = "No deliveries to show.";
                Message.IsVisible = true ;
                Message.IsEnabled = true;
                return false;
            }


        }
        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (DeliveryModel)e.SelectedItem;
            var selectedDelivery = listOfDeliveries.Where(d => d.DeliveryID == item.DeliveryID).First();
            await Navigation.PushAsync(new EditDeliveryDispatcherPage(selectedDelivery.ConvertToExtended()));
            //await Navigation.PushAsync(new NewDeliveryDispatchPage());


        }

        private async void DeliveryList_Refreshing(object sender, EventArgs e)
        {
            await GetUncofirmedDeliveriesFromServer();
            DeliveryList.EndRefresh();
        }

        
    }
}