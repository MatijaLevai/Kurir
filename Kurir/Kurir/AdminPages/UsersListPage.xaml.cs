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

namespace Kurir.AdminPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersListPage : ContentPage
    {
        private HttpClient client = App.client;
        private List<RegisterUserModel> listOfUsers;
        private UserService userService;
        public UsersListPage()
        {
            userService = new UserService();
            //_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }
       
        
        protected async override void OnAppearing()
        {
            await GetUsersFromServer();
            base.OnAppearing();
        }

        private async Task<bool> GetUsersFromServer()
        {
            var list = await userService.GetAllUsers();
            if (list != null)
            {
                listOfUsers = new List<RegisterUserModel>(list);

                UsersList.ItemsSource = listOfUsers;
                Message.Text = "List of all users : ";
                return true;
            }
            else
            {
                Message.Text = "No users to show.";
                return false;
            }


        }
        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (RegisterUserModel)e.SelectedItem;
            

                try
                {
                    await Navigation.PushAsync(new UserDetailPage(item)); //nova stranica za detalje usera gde treba userroles da se dodaje oduzima i da se dodaje procenat i oduzima
                }
                catch (Exception ex) { await DisplayAlert("", ex.Message + ex.InnerException, "ok"); }
            
        }

        private async void List_Refreshing(object sender, EventArgs e)
        {
            await GetUsersFromServer();
            UsersList.EndRefresh();
        }
    }
}