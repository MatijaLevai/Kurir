using Kurir.Models;
using Kurir.Persistance;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewDelivery : ContentPage
    {
        #region Properties of Class New Delivery
        private SQLiteHelper liteHelper;
        private readonly HttpClient _client ;
        private readonly SQLiteAsyncConnection _connection;
        private  List<PaymentTypeModel> paymentTypes;
        private  List<DeliveryTypeModel> deliveryTypes;
        private DeliveryModel delivery;
        private DeliveryService deliveryService;
        #endregion

        Func<string, bool> selector = delegate (string c)
        {
            var s = c.ToCharArray();
            if (s.All(char.IsNumber) && s.Count() > 6)
                return true;
            else if (s[0] == '+' &&s.Count()>11)
            {
                
                bool res = false;
                for (int i = 1; i < s.Count()-1; i++)
                {
                    if (char.IsNumber(s[i]))
                        res = true;
                    else
                        return false;
                }
                return res;
            }
            else return false;
        };
        #region Constructors
        public NewDelivery()
        {
            _client = App.client;
            liteHelper = new SQLiteHelper();
            deliveryService = new DeliveryService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }
        public NewDelivery(DeliveryModel delivery)
        {
            _client = App.client;
            if (delivery != null)
            {
                this.delivery = delivery;
                BindingContext = this.delivery;
            }
            liteHelper = new SQLiteHelper();
            deliveryService = new DeliveryService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }
        #endregion

        protected override async void OnAppearing()
        {
            if (!await DelAndPayTypes())
                await DisplayAlert("Internet", "Check Your connection settings", "Ok");
            if (delivery == null)
            {
                Titlelbl.Text = "Create delivery";
                OrderDelivery.Text = "Order";
            }
            else
            {
                Titlelbl.Text = "Edit Delivery";
                OrderDelivery.Text = "Edit";
            }
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    NewDeliveryStack.BackgroundColor = Color.Default;
                    break;
                case Device.Android:
                    NewDeliveryStack.BackgroundColor = Color.White;
                    break;
                default:
                    break;
            }
            
            base.OnAppearing();


        }

        private async void OrderDelivery_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(StartPhone.Text) && !string.IsNullOrWhiteSpace(PhoneEnd.Text) &&  !string.IsNullOrWhiteSpace(StartName.Text) && !string.IsNullOrWhiteSpace(StartAddress.Text) && !string.IsNullOrWhiteSpace(NameEnd.Text) && !string.IsNullOrWhiteSpace(EndAddress.Text))
            {

                DeliveryModel newDelivery = new DeliveryModel()
                {
                    PhoneOfStart = StartPhone.Text.Trim(),
                    NameStart = StartName.Text.Trim(),
                    StartAddress = StartAddress.Text.Trim(),
                    NameEnd = NameEnd.Text.Trim(),
                    PhoneOfEnd = PhoneEnd.Text.Trim(),
                    EndAddress = EndAddress.Text.Trim(),
                    UserID = Convert.ToInt32(Application.Current.Properties["UserID"].ToString()),
                    CreateTime = DateTime.Now,
                    PaymentTypeID = PaymentTypePicker.SelectedIndex+1,
                    DeliveryTypeID = DeliverTypePicker.SelectedIndex+1,
                    Description = DeliveryDetails.Text,
                    StartLocationID=1,
                    EndLocationID=1,
                    ZoneEnd = 0,
                    ZoneStart = 0,
                    WaitingInMinutes=0,
                    DeliveryStatusImageSource="",
                };
                if (delivery!=null)
                { try
                    {
                        newDelivery.DeliveryPrice = delivery.DeliveryPrice;
                        newDelivery.CourierID = delivery.CourierID;
                        newDelivery.ZoneStart = delivery.ZoneStart;
                        newDelivery.ZoneEnd = delivery.ZoneEnd;
                        newDelivery.DispatcherID = delivery.DispatcherID;
                        newDelivery.DeliveryID = delivery.DeliveryID;

                        var deliveryUpdate = await deliveryService.EditDelivery(newDelivery);
                        if (deliveryUpdate != null)
                        {
                            await DisplayAlert("All Done.", "Your order is placed.", "Confirm.");
                            await Navigation.PopAsync();
                        }
                        else await DisplayAlert("Something went wrong.", "Please try again.", "ok");
                    }
                    catch (Exception ex) { await DisplayAlert("error", "msg: " + ex.Message + "IE : " + ex.InnerException, "ok"); }
                }
                else {
                    try
                    {
                        var delivery = await deliveryService.CreateDelivery(newDelivery);
                        if (delivery !=null)
                        {
                            await DisplayAlert("All Done.", "Your order is placed.", "Confirm.");
                            await Navigation.PopAsync();
                        }
                        else await DisplayAlert("Something went wrong.", "Please try again.", "ok");

                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Something went wrong.", "Please try again."+ ex.Message, "ok");
                    }
                }
                


            }
            else await DisplayAlert("Atention!", "Please fill Entrys correctly.", "ok?");
        }
        private async Task<bool> DelAndPayTypes() 
        {
            try {

                var response = await liteHelper.UpdateSQLiteDb();
                if (response)
                {
                    
                    paymentTypes =  await _connection.Table<PaymentTypeModel>().ToListAsync();
                    deliveryTypes = await _connection.Table<DeliveryTypeModel>().ToListAsync();
                    PaymentTypePicker.ItemsSource = paymentTypes;
                    DeliverTypePicker.ItemsSource = deliveryTypes;
                    PaymentTypePicker.SelectedIndex = 0;
                    DeliverTypePicker.SelectedIndex = 0;
                    return true;
                }
                else
                {

                    OrderDelivery.IsEnabled = false;
                    
                    return false;
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Server or Internet problem.", ex.Message, "ok");
                return false;
            }
            
        }

        void Notify(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            if (string.IsNullOrWhiteSpace(property.Text))
            {
                property.TextColor = Color.Red;
            }
            else
                property.TextColor = Color.Black;
        }
        void NotifyAddress(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            if (!string.IsNullOrWhiteSpace(property.Text))
            {
                if (property.Text.ToCharArray().Any(char.IsDigit))
                property.TextColor = Color.Black;
            }
            else
                property.TextColor = Color.Red;

        }
        async void NotifyPhone(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            if (!string.IsNullOrWhiteSpace(property.Text))
            {
                try
                {
                    if (selector(property.Text))
                    {
                      property.TextColor = Color.Black;
                    }
                    else
                    {
                        property.TextColor = Color.Red;
                    }
                        
                }
                catch (Exception ex)
                {
                    await DisplayAlert("error ", ex.Message + "||" + ex.InnerException, "ok");
                 }
                
            
            }
            else
                property.TextColor = Color.Red;
        }
        
        void NotifyPicker(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Picker;
            if (property.SelectedIndex>=0)
            {
                property.BackgroundColor = Color.FromHex("#02ce0a");
               
            }
            else
                property.BackgroundColor = Color.Red;
        }


    }
}