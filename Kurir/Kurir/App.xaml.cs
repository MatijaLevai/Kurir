using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.Net.Http;

namespace Kurir
{
    public partial class App : Application
    {
        public static HttpClient client = new HttpClient();

        public App()
        {
            
            
            MainPage = new NavigationPage(new WelcomeTabbedPage());
            //Current.Properties["ServerLink"] = "https://192.168.1.2:45456/";
            Current.Properties["ServerLink"]="https://kurirserver.conveyor.cloud/";
            //Current.Properties["ServerLink"] = "https://localhost:44367";
            //Current.Properties["ServerLink"] = "http://localhost:59794";
            InitializeComponent();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
