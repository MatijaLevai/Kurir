using Kurir.Persistance;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kurir.DispatcherPages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Timers;
using Kurir.Models;

namespace Kurir.CourierPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultCouriersPageMaster : ContentPage
    {
        public ListView ListView;
        private UserService userService;
        
       
        public DefaultCouriersPageMaster()
        {
            InitializeComponent();
            userService = new UserService();
            BindingContext = new DefaultCouriersPageMasterMasterViewModel();
            ListView = MenuItemsListView;
           
        }

        class DefaultCouriersPageMasterMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<DefaultCouriersPageMenuItem> MenuItems { get; set; }


            public DefaultCouriersPageMasterMasterViewModel()
            {
                MenuItems = new ObservableCollection<DefaultCouriersPageMenuItem>(new[]
                {
                    new DefaultCouriersPageMenuItem { Id = 0, Title = "Nepotvrđene", TargetType=typeof(UncofirmedDeliveriesListPage) },
                    new DefaultCouriersPageMenuItem { Id = 1, Title = "Potvrđene", TargetType=typeof(ConfirmedDeliveriesListPage) },
                    new DefaultCouriersPageMenuItem { Id = 2, Title = "Preuzete", TargetType=typeof(StartedDeliveriesListPage) },
                    new DefaultCouriersPageMenuItem { Id = 3, Title = "Sve dostave" , TargetType=typeof(DeliveriesHistoryListPage)},
                    new DefaultCouriersPageMenuItem { Id = 4, Title = "Profil" , TargetType=typeof(EditAccount)},
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
        }
           

            #endregion
        
    }
}