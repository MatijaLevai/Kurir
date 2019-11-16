using Kurir.Persistance;
using Kurir.UserPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.UserPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserHomePage : MasterDetailPage
    {
       
        public UserHomePage()
        {
            NavigationPage.SetHasBackButton(this, false);
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            UserHomePageMenuItem item = e.SelectedItem as UserHomePageMenuItem;
            if (item == null)
                return;
            
            if (item.TargetType == typeof(StartAddressPage))
            {

                var page =new NavigationPage(new StartAddressPage());
                page.Title = item.Title;

                Detail = page;
            }
            else
            {
                var page = (Page)Activator.CreateInstance(item.TargetType);
                page.Title = item.Title;

                Detail = new NavigationPage(page);
            }
            
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
            //return base.OnBackButtonPressed();
        }

        
    }
}