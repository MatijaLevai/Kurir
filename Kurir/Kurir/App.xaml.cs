using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;

namespace Kurir
{
    public partial class App : Application
    {
       

        public App()
        {
            
            InitializeComponent();
            MainPage = new NavigationPage(new WelcomeTabbedPage());
            Current.Properties["ServerLink"] = "https://kurirserver.conveyor.cloud/";
           // Current.Properties["ServerLinkLocal"] = "https://localhost:44367/";
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
