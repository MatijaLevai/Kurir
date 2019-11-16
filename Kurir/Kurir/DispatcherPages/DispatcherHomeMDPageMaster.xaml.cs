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

namespace Kurir.DispatcherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DispatcherHomeMDPageMaster : ContentPage
    {
        public ListView ListView;
       
        public DispatcherHomeMDPageMaster()
        {
            InitializeComponent();

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