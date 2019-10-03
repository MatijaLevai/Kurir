using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kurir.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.DispatcherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryDetailPage : ContentPage
    {
        public DeliveryDetailPage(DeliveryModel delivery)
        {
            InitializeComponent();
        }
    }
}