using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Kurir
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoServicePage : ContentPage
    {

        HttpResponseMessage result;
        string message;
        private async Task<bool> TryGetServer()
        {
            try
            {
                result =await App.client.GetAsync(App.InternetServerLink + "api/BaraBara");
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                message = ex.Message + ex.InnerException + result.StatusCode + result.Headers;
                return false;
            }
        }
        public NoServicePage()
        {
            InitializeComponent();
            result = new HttpResponseMessage();
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            
        }
        

        async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            App.ServerActive = await TryGetServer();
            if (App.ServerActive)
            {
                if (!Application.Current.Properties.ContainsKey("User"))
                {
                    Application.Current.MainPage = new NavigationPage(new WelcomeTabbedPage());

                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(new MainPage());
                }
            }
            else { await DisplayAlert("",message,"ok"); }
        }
        public async Task PlacePhoneCall()
        {
            try
            {
                PhoneDialer.Open("+381603060696");
               // PhoneDialer.Open("+381603060699");
            }
            catch (Exception ex)
            {
                await DisplayAlert("",ex.Message,"ok");
                // Other error has occurred.
            }
        }

        private async void Nazovi_Clicked(object sender, EventArgs e)
        {
                await PlacePhoneCall();
            
        }
    }
}