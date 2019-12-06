using Xamarin.Forms;
using System.Net.Http;
using System.Threading;
using Xamarin.Essentials;

namespace Kurir
{
    public partial class App : Application
    {
        public static HttpClient client = new HttpClient();
        public static string LocalServerLink = "https://kurirserver.conveyor.cloud/";//localhost:44355/";
        public static string InternetServerLink = "https://kurirserver.conveyor.cloud/";
        public static bool ServerActive = false;
        

       
        public App()
        {
           
            InitializeComponent();
           
           
        }
       
        protected override async void OnStart()
        {


                string currentServerLink = "";
                HttpResponseMessage response;
                try
                {
                    response = await client.GetAsync(InternetServerLink + "api/BaraBara");
                    if (response.IsSuccessStatusCode)
                    {
                        currentServerLink = InternetServerLink;
                        ServerActive = true;
                    }
                    else
                    {
                        ServerActive = false;
                    }
                }
                catch
                {
                    ServerActive = false;
                }
                if (!ServerActive)
                {
                    try
                    {
                        response = await client.GetAsync(LocalServerLink + "api/BaraBara");
                        if (response.IsSuccessStatusCode)
                        {
                            currentServerLink = LocalServerLink;
                            ServerActive = true;
                        }
                        else
                        {
                            ServerActive = false;
                        }
                    }
                    catch
                    {
                        ServerActive = false;
                    }
                }
            if (ServerActive)
            {
                if (Current.Properties.ContainsKey("ServerLink"))
                    Current.Properties["ServerLink"] = currentServerLink;
                else
                    Current.Properties.Add("ServerLink", currentServerLink);

                Current.MainPage = new NavigationPage(new MainPage());

            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new NoServicePage());

            }





        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        
    }
}
