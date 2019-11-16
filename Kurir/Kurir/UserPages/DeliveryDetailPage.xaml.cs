using Kurir.Models;
using Kurir.Persistance;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir
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
            if (await helper.UpdateSQLiteDbWithPayAndDelTypes())
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

            }
            else throw new Exception("No internet or bad request to server");
            
            base.OnAppearing();
        }

        
       


    }
}