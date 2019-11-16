using Xamarin.Forms;
using System.Net.Http;

namespace Kurir
{
    public partial class App : Application
    {
        public static HttpClient client = new HttpClient();


        public App()
        {
           
            if (Current.Properties.ContainsKey("ServerLink")) Current.Properties["ServerLink"] = "https://kurirserver.conveyor.cloud/";
            else Current.Properties.Add("ServerLink", "https://kurirserver.conveyor.cloud/");

            
            InitializeComponent();


            //Current.Properties["GoogleApiKey"] = "AIzaSyDch3NOgOI5gnYpfoc9lwiTU0Z-coMgnK4";
            //Current.Properties["ServerLink"] = "https://192.168.1.2:45456/";
            //Current.Properties["ServerLink"] = "https://localhost:44367"; 
            //Current.Properties["ServerLink"] = "http://localhost:59794";
            //DependencyService.Get<INotificationManager>().Initialize();
        }
       
        protected override void OnStart()
        {
            
            if (!Current.Properties.ContainsKey("User"))
            {
                Current.MainPage = new NavigationPage(new WelcomeTabbedPage());
                
            }
            else
            {
                Current.MainPage = new NavigationPage(new MainPage());
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
