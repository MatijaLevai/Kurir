using Kurir.AdminPages;
using Kurir.CourierPages;
using Kurir.DispatcherPages;
using Kurir.Models;
using Kurir.Persistance;
using Kurir.SuperAdminPages;
using Kurir.UserPages;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http;
using Xamarin.Forms;

namespace Kurir
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        //private HttpClient _client = new HttpClient();
        public static HttpClient client = App.client;
        private UserRoleService userRoleService;
        
        public MainPage()
        {
            userRoleService = new UserRoleService();
            InitializeComponent();

        }
        protected override async void OnAppearing()
        {

            // try
            // {

            if (App.ServerActive)
            {

                if (!Application.Current.Properties.ContainsKey("User"))
                {

                    Application.Current.MainPage = new NavigationPage(new WelcomeTabbedPage());

                }
                else
                {
                    RegisterUserModel user = JsonConvert.DeserializeObject<RegisterUserModel>(Application.Current.Properties["User"].ToString());
                    UserRoleModel ur = await userRoleService.Get(user.ActiveUserRoleID);
                    if (ur != null)
                    {
                        switch (ur.RoleID)
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
                                DefaultCouriersPage main = new DefaultCouriersPage();
                                Application.Current.MainPage = new NavigationPage(main);
                                break;
                            case 5:
                                Application.Current.MainPage = new NavigationPage(new DispatcherHomeMDPage());
                                break;
                        }
                    }
                    else
                    {
                        Application.Current.Properties.Remove("User");
                        Application.Current.MainPage = new NavigationPage(new WelcomeTabbedPage());
                    }
                }
            }
            else {
                Application.Current.MainPage = new NavigationPage(new NoServicePage());
            }
                //catch (Exception)
                //{
                //    //await DisplayAlert("Greska", ex.Message + ex.InnerException, "ok"); 
                //}
            base.OnAppearing();
        }



    }
}
