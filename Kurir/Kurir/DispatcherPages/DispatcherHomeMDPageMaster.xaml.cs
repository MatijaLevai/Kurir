using Kurir.Models;
using Kurir.Persistance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.DispatcherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DispatcherHomeMDPageMaster : ContentPage
    {
        public ListView ListView;
        private UserService userService;


        public DispatcherHomeMDPageMaster()
        {
            InitializeComponent();
            userService = new UserService();
            DispatchNameGet();
            BindingContext = new DispatcherHomeMDPageMasterViewModel();
            ListView = MenuItemsListView;
        }
        private void DispatchNameGet()
        {
            //Title = "Eko dispečer " + Application.Current.Properties["Name"].ToString();
            if (Application.Current.Properties.ContainsKey("Name"))
            {
                LabelName.Text = Application.Current.Properties["Name"].ToString();
            }
            else if (Application.Current.Properties.ContainsKey("User"))
            {
                RegisterUserModel user = JsonConvert.DeserializeObject<RegisterUserModel>(Application.Current.Properties["User"].ToString());
                LabelName.Text = user.FirstName.ToString();
            }
            else { Application.Current.MainPage = new MainPage();
                }
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
                    Application.Current.MainPage = new NavigationPage(new WelcomeTabbedPage());

                }
                else await DisplayAlert("error", "Server Error", "ok.");
            }
            catch (Exception ex)
            { await DisplayAlert("error", ex.Message, "ok."); }
        }
        class DispatcherHomeMDPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<DispatcherHomeMDPageMenuItem> MenuItems { get; set; }

            public DispatcherHomeMDPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<DispatcherHomeMDPageMenuItem>(new[]
                {
                     new DispatcherHomeMDPageMenuItem { Id = 0, Title = "Nova dostava",TargetType=typeof(NewDeliveryDispatchPage) },
                    new DispatcherHomeMDPageMenuItem { Id = 1, Title = "Aktivni kuriri",TargetType=typeof(ActiveCourierListMapPage) },
                    new DispatcherHomeMDPageMenuItem { Id = 2, Title = "Nepotvrđene dostave",TargetType=typeof(UncofirmedDeliveriesListPage)},
                    new DispatcherHomeMDPageMenuItem { Id = 3, Title = "Sve dostave",TargetType=typeof(ListOfAllDeliveries) },
                    new DispatcherHomeMDPageMenuItem { Id = 4, Title = "Dostave aktivnog dispečera",TargetType=typeof(DeliveriesListPage) },
                     new DispatcherHomeMDPageMenuItem { Id = 5, Title = "Statistika",TargetType=typeof(StatisticsPage) },
                    new DispatcherHomeMDPageMenuItem { Id = 6, Title = "Ažuriraj nalog",TargetType=typeof(EditDispatcherPage) }
                   

                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
            
        }
    }
}