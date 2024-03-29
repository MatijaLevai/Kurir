﻿using Kurir.AdminPages;
using Kurir.CourierPages;
using Kurir.DispatcherPages;
using Kurir.Models;
using Kurir.SuperAdminPages;
using Kurir.UserPages;
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
        private HttpClient _client = App.client;
        private RegisterUserModel _user;
        private string uri;
       
        public  UserRolePage(RegisterUserModel user)
        {
            NavigationPage.SetHasBackButton(this, false);
            _user = user;
            InitializeComponent();
            
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
            //return base.OnBackButtonPressed();
        }
        private async void MenuItem_ClickedAsync(object sender, EventArgs e)
        {
            UserRoleModel selected =(UserRoleModel) RoleList.SelectedItem;
           //await DisplayAlert("Chosen",selected.Name, "ok");
            string url =uri+"api/Users/ChangeCurrentUserRole/" + Application.Current.Properties["UserID"].ToString() + "/" + selected.UserRoleID;
            var response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                //await DisplayAlert("succsses", response.StatusCode.ToString(), "ok");
                switch (selected.RoleID)
                {
                    case 1:
                        Application.Current.MainPage = new NavigationPage(new DefaultSuperAdminPage());
                        break;
                    case 2:
                        Application.Current.MainPage = new NavigationPage(new AdminsPage());
                        break;
                    case 3:
                        Application.Current.MainPage = new NavigationPage(new UserHomePage());
                        break;
                    case 4:
                        Application.Current.MainPage = new NavigationPage(new DefaultCouriersPage());
                        break;
                    case 5:
                        Application.Current.MainPage = new NavigationPage(new DispatcherHomeMDPage());
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
                if (userroles.Count() == 1)
                {
                    var ur = userroles.First();
                    if(ur.RoleID==3)
                    await Navigation.PushAsync(new UserHomePage());
                }
                RoleList.ItemsSource = new ObservableCollection<UserRoleModel>(userroles);
             
            }
            catch (Exception ex)
            {await DisplayAlert("Error",ex.Message,"ok?"); }
            
        
            base.OnAppearing();
        }

        
    }
}