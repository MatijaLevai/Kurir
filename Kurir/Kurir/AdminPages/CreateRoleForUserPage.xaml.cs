using Kurir.Models;
using Kurir.Persistance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.AdminPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateRoleForUserPage : ContentPage
    {
        private HttpClient _client = App.client;
        private RegisterUserModel user;
        private List<RoleModel> allRoles;
        private List<UserRoleModel> userRoles;
        private string uri;
        private UserRoleService userRoleService;
        public CreateRoleForUserPage(RegisterUserModel user)
        {
            uri = Application.Current.Properties["ServerLink"].ToString();
            this.user = user;
            userRoleService = new UserRoleService();
            InitializeComponent();
        }
        private async void GetAllUsersRoles()
        {
            try
            {

                string url = uri + "api/UserRoles/GetUserRoles/";
                var response = await _client.GetAsync(url + user.UserID);
                //Debug.WriteLine("poslat upit serveru");
                var responseContent = await response.Content.ReadAsStringAsync();
                //await DisplayAlert("Content of json",responseContent.ToString(),"ok");
                // Debug.WriteLine("primljen odgovor od servera");
                userRoles = JsonConvert.DeserializeObject<List<UserRoleModel>>(responseContent);
                // Debug.WriteLine("priprema ubjekta u listu");
                foreach (var item in userRoles)
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
                
                UserRoleList.ItemsSource = new ObservableCollection<UserRoleModel>(userRoles);

            }
            catch (Exception ex)
            { await DisplayAlert("Error", ex.Message, "ok?"); }
        }
        private async void GetAllRoles()
        {
            try
            {

                string url = uri + "api/UserRoles/GetAllRoles";
                var response = await _client.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                allRoles = JsonConvert.DeserializeObject<List<RoleModel>>(responseContent);

            }
            catch (Exception ex)
            { await DisplayAlert("Error", ex.Message, "ok?"); }
        }
        protected override void OnAppearing()
        {
            GetAllRoles();
            GetAllUsersRoles();
            base.OnAppearing();
        }

        private async void AddRole_Clicked(object sender, EventArgs e)
        {
            
            bool admin = true ;
            bool dispatch = true ;
            bool courier = true;
            int roleId = 0;
            foreach (var item in userRoles)
            {
                switch (item.Name)
                {
                    case "Admin":
                        admin = false;
                        break;
                    case "Dispatcher":
                        dispatch = false;
                        break;
                    case "Courier":
                        courier = false;
                        break;
                }
            }
            string answer = null;
            if(admin&&dispatch&&courier)
                answer = await DisplayActionSheet("Select new role","cancel",null,"Courier","Dispatcher","Admin");
            else if(admin&&dispatch)
                answer = await DisplayActionSheet("Select new role", "cancel", null, "Dispatcher", "Admin");
            else if(admin&&courier)
                answer = await DisplayActionSheet("Select new role", "cancel", null, "Courier", "Admin");
            else if (courier && dispatch)
                answer =  await DisplayActionSheet("Select new role", "cancel", null, "Dispatcher", "Courier");
            else if(admin)
                answer =  await DisplayActionSheet("Select new role", "cancel", null, "Admin");
            else if(courier)
                answer = await DisplayActionSheet("Select new role", "cancel", null, "Courier");
            else if(dispatch)
                answer = await DisplayActionSheet("Select new role", "cancel", null, "Dispatcher");
            switch (answer)
            {
                case "Admin":
                    roleId = 2;
                    break;
                case "Dispatcher":
                    roleId = 5;
                    break;
                case "Courier":
                    roleId = 4;
                    break;
                default:
                    await DisplayAlert("Erorr","Please try again","ok");
                    break;
            }
            if (roleId > 0)
            {
                UserRoleModel ur = new UserRoleModel()
                {
                    UserID = user.UserID,
                    RoleID = roleId
                };
                if (await userRoleService.AddNewUserRole(ur))
                {
                   
                    await DisplayAlert("Succsses", "Created new role for " + user.ToString(), "ok");
                    GetAllUsersRoles();
                }
                else await DisplayAlert("Erorr", "Please try again", "ok");
            }

        }

        public async void DeleteRole(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);

            try
            {
                if (int.TryParse(mi.CommandParameter.ToString(), out int IDint))
                {
                    UserRoleModel selectedUserRole = userRoles.Where(x => x.UserRoleID == IDint).First();
                    userRoles.Remove(selectedUserRole);
                    if (selectedUserRole != null)
                    {

                        if (await userRoleService.Delete(IDint))
                        {
                           
                            await DisplayAlert("Succses", "Role removed.", "ok");
                            GetAllUsersRoles();
                        }
                        else
                        {
                            await DisplayAlert("Remuval failed.", " Try again.", "ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Remuval failed.", " Try again.", "ok");
                    }
                }
                else
                {
                    await DisplayAlert("Remuval failed.", " Try again.", "ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Remuval failed.", " Try again." + ex.Message + ex.InnerException, "ok");
            }

        }
    }
}