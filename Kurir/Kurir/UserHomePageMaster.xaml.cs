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

namespace Kurir
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserHomePageMaster : ContentPage
    {
        public ListView ListView;

        public UserHomePageMaster()
        {
            InitializeComponent();

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
                    new UserHomePageMenuItem { Id = 0, Title = "New Delivery", TargetType=typeof(NewDelivery)},
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
    }
}