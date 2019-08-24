using Kurir.AdminPages;
using Kurir.CourierPages;
using Kurir.DispatcherPages;
using Kurir.Models;
using Kurir.SuperAdminPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserRolePage : ContentPage
    {
        private HttpClient _client = new HttpClient();
        private RegisterUserModel _user;
        private string uri;
       
        public  UserRolePage(RegisterUserModel user)
        {
            _user = user;
            InitializeComponent();
            
        }

        private async void MenuItem_ClickedAsync(object sender, EventArgs e)
        {
            UserRoleModel selected =(UserRoleModel) RoleList.SelectedItem;
            await DisplayAlert("Chosen",selected.Name, "ok");
            string url =uri+"api/Users/ChangeCurrentUserRole/" + Application.Current.Properties["UserID"].ToString() + "/" + selected.UserRoleID;
            var response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                await DisplayAlert("succsses", response.StatusCode.ToString(), "ok");
                switch (selected.RoleID)
                {
                    case 1:
                        await Navigation.PushAsync(new DefaultSuperAdminPage());
                        break;
                    case 2:
                        await Navigation.PushAsync(new DefultAdminPage());
                        break;
                    case 3:
                        await Navigation.PushAsync(new UserHomePage());
                        break;
                    case 4:
                        await Navigation.PushAsync(new DefaultCourierPage());
                        break;
                    case 5:
                        await Navigation.PushAsync(new DefaultDispatcherPage());
                        break;
                }
            }
            else { await DisplayAlert("error","please check your internet connection","Ok"); }

            
            
        }
        protected override async void OnAppearing()
        {
            uri = Application.Current.Properties["ServerLink"].ToString();
            //switch (Device.RuntimePlatform)
            //{ 
            ////{ case Device.UWP:
            ////        uri = Application.Current.Properties["ServerLinkLocal"].ToString();
            ////        break;

            //    default:
            //        uri = Application.Current.Properties["ServerLink"].ToString();
            //        break;
            //}
            try
            {

               string  url =uri+"api/UserRoles/GetUserRoles/";
                var response = await _client.GetAsync(url+_user.UserID);
                //Debug.WriteLine("poslat upit serveru");
                var responseContent = await response.Content.ReadAsStringAsync();
                //await DisplayAlert("Content of json",responseContent.ToString(),"ok");
               // Debug.WriteLine("primljen odgovor od servera");
                var userroles = JsonConvert.DeserializeObject<List<UserRoleModel>>(responseContent);
               // Debug.WriteLine("priprema ubjekta u listu");
                foreach (var item in userroles)
                {
                    switch (item.RoleID)
                    {
                        case 1:
                            item.Name = "SuperAdmin";
                            break;
                        case 2:
                            item.Name = "Admin";
                            break;
                        case 3:
                            item.Name = "User";
                            break;
                        case 4:
                            item.Name = "Couirier";
                            break;
                        case 5:
                            item.Name = "Dispatcher";
                            break;
                    }
                }
                RoleList.ItemsSource = new ObservableCollection<UserRoleModel>(userroles);
            }
            catch (Exception ex)
            {await DisplayAlert("Erro or not?",ex.Message,"ok?"); }
            
            base.OnAppearing();
        }
    }
}