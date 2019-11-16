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

namespace Kurir.UserPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryHistory : ContentPage
    {
        private HttpClient client = App.client;
        private List<DeliveryModel> listOfDeliveries;
       // private SQLiteAsyncConnection _connection;
        private DeliveryService deliveryService;

        public DeliveryHistory()
        {
            deliveryService = new DeliveryService();
            //_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
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


                var list = await deliveryService.GetDeliveriesForUser(Convert.ToInt32(Application.Current.Properties["UserID"].ToString()));
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
                                item.DeliveryStatusImageSource = "zutieleni50.png";
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
            var item =(DeliveryModel) e.SelectedItem;
           var selectedDelivery = listOfDeliveries.Where(d => d.DeliveryID == item.DeliveryID).First();
            //provera da li je kurir vec preuzeo dostavu ili je proslo 5 minuta od slanja dostave, tada korisniku nije dozvoljeno menjanje dostave
            if (selectedDelivery.DeliveryStatus>1)
            {
                await Navigation.PushAsync(new DeliveryDetailPage(selectedDelivery));
            }
            else
            {
                await Navigation.PushAsync(new StartAddressPage(selectedDelivery));
               
            }
        }
        private async void DeliveryList_Refreshing(object sender, EventArgs e)
        {
            await GetDeliveriesFromServer();
            DeliveryList.EndRefresh();
        }
        public async void DeleteCommand(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);

            try
            {
                if (int.TryParse(mi.CommandParameter.ToString(), out int IDint))
                {
                    DeliveryModel selectedDelivery = listOfDeliveries.Where(x => x.DeliveryID == IDint).First();
                    if (selectedDelivery != null)
                    {

                        if (selectedDelivery.DeliveryStatus==0)
                        {
                            selectedDelivery.UserID = 0;
                            var x = await deliveryService.EditDelivery(selectedDelivery);
                           if(x.UserID == 0)
                            { listOfDeliveries.Remove(selectedDelivery);
                                DeliveryList.ItemsSource = null;
                                DeliveryList.ItemsSource = listOfDeliveries;
                                await DisplayAlert("Succses", "Delivery removed.", "ok");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Delivery remuval failed.", " Delivery is allready confirmed by dispatcher.", "ok");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Delivery confimation failed.", " Try again. Check internet connection. Error : " + ex.Message + ex.InnerException, "ok");
            }

        }

    }
    }