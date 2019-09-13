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

namespace Kurir.DispatcherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveCourierListPage : ContentPage
    {
        private HttpClient client = App.client;
        private List<ActiveCourierModel> listOfCouriers;
        private SQLiteAsyncConnection _connection;
        private UserService userService;
        public ActiveCourierListPage()
        {
            userService = new UserService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            await GetActiveCouriersFromServer();
            base.OnAppearing();
        }

        private async Task<bool> GetActiveCouriersFromServer()
        {
            var list = await userService.GetActiveCouriers();
            if (list != null)
            {
                listOfCouriers = new List<ActiveCourierModel>(list);
                //await _connection.DropTableAsync<DeliveryModel>();
               // await _connection.CreateTableAsync<ActiveCourierModel>();

                foreach (var item in listOfCouriers)
                {
                    if(item.Lat>0&&item.Long>0)
                    {
                        item.StatusImageSource = "zeleni50.png";
                    }
                    else
                    {
                        item.StatusImageSource = "zuti50.png";
                    }

                    //int x = await _connection.UpdateAsync(item);
                    //if (x == 0)
                    //{
                    //    await _connection.InsertAsync(item);
                    //}


                }

                ActiveCourierList.ItemsSource = listOfCouriers;
                Message.Text = "";
                return true;
            }
            else
            {
                Message.Text = "No active couriers to show.";
                return false;
            }


        }
        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (ActiveCourierModel)e.SelectedItem;
            if (item.Lat > 0 && item.Long > 0)
            {
                Location l = new Location(item.Lat, item.Long, item.DToffSet);
                l.Altitude = item.Alt;
                try {
                    await Navigation.PushAsync(new MapPage(l,item.CourierFullName));
                    }
                catch (Exception ex) { await DisplayAlert("", ex.Message + ex.InnerException, "ok"); }
                //await Map.OpenAsync(l);
            }
            else
                await DisplayAlert("Unknown location","Courier can not be located at this time. Please try again in few moments.","ok.");
                    

        }

        private async void List_Refreshing(object sender, EventArgs e)
        {
            await GetActiveCouriersFromServer();
            ActiveCourierList.EndRefresh();
        }
    }
}