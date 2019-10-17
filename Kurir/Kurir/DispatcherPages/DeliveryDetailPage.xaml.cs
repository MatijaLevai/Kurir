using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kurir.Models;
using Kurir.Persistance;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.DispatcherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryDetailPage : ContentPage
    {
        private HttpClient _client = App.client;
        private DeliveryModel delivery;
        private List<PaymentTypeModel> paymentTypes;
        private List<DeliveryTypeModel> deliveryTypes;
        private readonly SQLiteAsyncConnection _connection;
        private SQLiteHelper helper;
        public DeliveryDetailPage(DeliveryModel delivery)
        {
            helper = new SQLiteHelper();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            this.delivery = delivery;
            BindingContext = this.delivery;
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
           
                try
                {
                    paymentTypes = await _connection.Table<PaymentTypeModel>().ToListAsync();
                    deliveryTypes = await _connection.Table<DeliveryTypeModel>().ToListAsync();
                    PaymentTypeModel p = paymentTypes.Where(d => d.PaymentTypeID == delivery.PaymentTypeID).FirstOrDefault();
                    if (p != null)
                        PaymentType.Text = p.PaymentTypeName;
                    DeliveryTypeModel D = deliveryTypes.Where(d => d.DeliveryTypeID == delivery.DeliveryTypeID).FirstOrDefault();
                    if (D != null)
                        DeliveryType.Text = D.DeliveryTypeName;
                }
                catch (Exception ex) { await DisplayAlert("error", ex.Message + ">||<" + ex.InnerException, "ok"); }

            

            base.OnAppearing();
        }


    }
}