using Kurir.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.AdminPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminsPage : MasterDetailPage
    {
        private UserService userService;
        public AdminsPage()
        {
            userService = new UserService();
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as AdminsPageMenuItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
        private async void LoogOutButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                int usrID = Int32.Parse(Application.Current.Properties["UserID"].ToString());

                if (await userService.LogOut(usrID))
                {

                    //Application.Current.Properties.Remove("Mail");
                    // Application.Current.Properties.Remove("UserID");
                    // Application.Current.Properties.Remove("Pass");
                    // Application.Current.Properties.Remove("Name");
                    var link = Application.Current.Properties["ServerLink"].ToString();
                    Application.Current.Properties.Clear();
                    Application.Current.Properties.Add("ServerLink", link);
                    await Application.Current.SavePropertiesAsync();
                    await Navigation.PushAsync(new WelcomeTabbedPage());

                }
                else await DisplayAlert("error", "Server Error", "ok.");
            }
            catch (Exception ex)
            { await DisplayAlert("error", ex.Message, "ok."); }
        }
    }
}