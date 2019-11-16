using Kurir.Models;
using Kurir.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.CourierPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveriesHistoryListPage : ContentPage
    {
        public DeliveriesHistoryListPage()
        {
            deliveryService = new DeliveryService();
            //_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }
        private HttpClient client = App.client;
        private List<DeliveryModel> listOfDeliveries;
        // private SQLiteAsyncConnection _connection;
        private DeliveryService deliveryService;

       
        protected async override void OnAppearing()
        {
            await GetDeliveriesFromServer();
            base.OnAppearing();
        }
        private async Task<bool> GetDeliveriesFromServer()
        {
            if (Application.Current.Properties.ContainsKey("UserID"))
            {


                var list = await deliveryService.GetDeliveriesForCourierDesc();
                if (list != null)
                {
                    listOfDeliveries = new List<DeliveryModel>(list);
                    //await _connection.DropTableAsync<DeliveryModel>();
                    // await _connection.CreateTableAsync<DeliveryModel>();

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
                    return false;
                }

            }
            else return false;
        }
        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (DeliveryModel)e.SelectedItem;
            var selectedDelivery = listOfDeliveries.Where(d => d.DeliveryID == item.DeliveryID).First();
            string answer = await DisplayActionSheet("Would you like to Edit or See Details of delivery?","Edit","Details");
            switch (answer)
            {
                case "Edit":
                    await Navigation.PushAsync(new DeliveryCreateEditPage(item));
                    break;
                case "Details":
                    await Navigation.PushAsync(new DeliveryDetailCourierPage(item));
                    break;
            }
           // await DisplayAlert("Error","Napraviti stranicu sa editovanjem dostave gde kurir menja cenu, cekanje, opis, specijalno adrese ili kreira novu dostavu. Takodje padajuci meni sa statusom dostave. Gde kurir moze da.","OK"); //Navigation.PushAsync(new NewDelivery(selectedDelivery));
        }
        private async void DeliveryList_Refreshing(object sender, EventArgs e)
        {
            await GetDeliveriesFromServer();
            DeliveryList.EndRefresh();
        }

    }
}