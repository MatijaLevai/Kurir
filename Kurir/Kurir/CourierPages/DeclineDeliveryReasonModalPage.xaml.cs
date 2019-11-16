using Kurir.Models;
using Kurir.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.CourierPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeclineDeliveryReasonModalPage : ContentPage
    {
        private DeliveryService deliveryService;
        DeliveryModel delivery;
        public DeclineDeliveryReasonModalPage(DeliveryModel delivery)
        {
           
            if (delivery != null)
            {
                deliveryService = new DeliveryService();
                this.delivery = delivery;
                BindingContext = delivery;

            }
            else { Pop(); }

            InitializeComponent();
        }
        private async void Pop()
        { await Navigation.PopModalAsync(); }
        private async void Decline()
        {
            delivery.DeliveryStatus = 0;
            delivery.CourierID = 0;
            var response = await deliveryService.EditDelivery(delivery);
            if (response != null)
            {
                await DisplayAlert("BaraBara", "Dostava odbijena.", "ok");
                Pop();
            }
            else
            {
                await DisplayAlert("Greška", " Odbijanje neuspešno", "ok");
                Pop();
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Decline();
        }
    }
}