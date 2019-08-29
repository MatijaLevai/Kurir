using Kurir.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Logout : ContentPage
    {
        private UserService userService;
        public Logout()
        {
            userService = new UserService();
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
           
        }
    }
}