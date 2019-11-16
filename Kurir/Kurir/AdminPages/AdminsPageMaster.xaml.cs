using Kurir.DispatcherPages;
using Kurir.Models;
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

namespace Kurir.AdminPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminsPageMaster : ContentPage
    {
        public ListView ListView;
        private string AdmisName;
        public AdminsPageMaster()
        {
            InitializeComponent();
            AdminNameGet();
            BindingContext = new AdminsPageMasterMasterViewModel();
            ListView = MenuItemsListView;
        }
        private void AdminNameGet()
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
            else
            {
                Application.Current.MainPage = new MainPage();
            }
        }
        class AdminsPageMasterMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<AdminsPageMenuItem> MenuItems { get; set; }

            public AdminsPageMasterMasterViewModel()
            {
                MenuItems = new ObservableCollection<AdminsPageMenuItem>(new[]
                {
                    new AdminsPageMenuItem { Id = 0,TargetType= typeof(UsersListPage), Title = "Korisnici" },
                    new AdminsPageMenuItem { Id = 1,TargetType= typeof(ListOfAllDeliveries), Title = "Dostave" },
                    new AdminsPageMenuItem { Id = 2,TargetType= typeof(StatisticsPage), Title = "Statistika" },
                    new AdminsPageMenuItem { Id = 3, Title = "" },
                    new AdminsPageMenuItem { Id = 4, Title = "" },
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