using Kurir.Persistance;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.UserPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserHomePageMaster : ContentPage
    {
        public ListView ListView;
        private readonly UserService userService;
        
        public UserHomePageMaster()
        {
            InitializeComponent();
            userService = new UserService();
            BindingContext = new UserHomePageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class UserHomePageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<UserHomePageMenuItem> MenuItems { get; set; }

            public UserHomePageMasterViewModel()
            {
                MenuItems = new ObservableCollection<UserHomePageMenuItem>(new[]
                {
                    new UserHomePageMenuItem { Id = 0, Title = "New Delivery", TargetType=typeof(StartAddressPage)},
                    new UserHomePageMenuItem { Id = 1, Title = "Delivery History", TargetType=typeof(DeliveryHistory)},
                    new UserHomePageMenuItem { Id = 2, Title = "Edit Account", TargetType=typeof(EditAccount)}

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
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                int usrID = Int32.Parse(Application.Current.Properties["UserID"].ToString());

                if (await userService.LogOut(usrID))
                {

                    Application.Current.Properties.Remove("Mail");
                    Application.Current.Properties.Remove("UserID");
                    Application.Current.Properties.Remove("Pass");
                    Application.Current.Properties.Remove("Name");
                    await Navigation.PushAsync(new WelcomeTabbedPage());

                }
                else await DisplayAlert("error", "Server Error", "ok.");
            }
            catch (Exception ex)
            { await DisplayAlert("error", ex.Message, "ok."); }
        }

    }
}