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
        private readonly HttpClient _client = new HttpClient();
        private readonly SQLiteAsyncConnection _connection;
        private  List<PaymentTypeModel> paymentTypes;
        private  List<DeliveryTypeModel> deliveryTypes;
    
        public NewDelivery()
        {
            InitializeComponent();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        private async void OrderDelivery_Clicked(object sender, EventArgs e)
        {
            if (PaymentTypePicker.SelectedItem != null|| string.IsNullOrWhiteSpace(StartPhone.Text) || string.IsNullOrWhiteSpace(PhoneEnd.Text)|| string.IsNullOrWhiteSpace(StartName.Text) || string.IsNullOrWhiteSpace(StartAddress.Text) || string.IsNullOrWhiteSpace(NameEnd.Text) || string.IsNullOrWhiteSpace(EndAddress.Text))
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
                    Description = DeliveryDetails.Text
                };
                //await DisplayAlert("delivery detail", "PhoneOfStart :"+ newDelivery.PhoneOfStart + "NameStart :"+newDelivery.NameStart+ "StartAddress :"+newDelivery.StartAddress+ "NameEnd :"+newDelivery.NameEnd+ "PhoneOfEnd :"+newDelivery.PhoneOfEnd+ "EndAddress :"+newDelivery.EndAddress+ "UserID :"+newDelivery.UserID+ "CreateTime :"+newDelivery.CreateTime+ "PaymentTypeID :"+newDelivery.PaymentTypeID, "ok");
                try
                {
                    var jsonObject = JsonConvert.SerializeObject(newDelivery);
                    HttpContent httpContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                    var uri = Application.Current.Properties["ServerLink"].ToString() + "api/Deliveries/NewDelivery";
                   var response = _client.PostAsync(uri,httpContent);
                    if (response.Result.IsSuccessStatusCode)
                    {

                        var responseContent = await response.Result.Content.ReadAsStringAsync();
                        //Debug.WriteLine("primljen odgovor od servera");
                        var userResponse = JsonConvert.DeserializeObject<RegisterUserModel>(responseContent);
                        await DisplayAlert("All Done.", "Your order is placed.", "Confirm.");
                    }
                    else await DisplayAlert("No good",response.Result.StatusCode.ToString() +"InnerExc"+response.Result.ReasonPhrase,"ok");
                }
                catch (Exception ex)
                { 
                await DisplayAlert("No good", ex.Message, "ok");

                }


            }
            else await DisplayAlert("Atention!", "Please fill Entrys correctly.", "ok?");
        }
        protected override async void OnAppearing()
        {

            string uriPaymentTypes = Application.Current.Properties["ServerLink"].ToString()+"api/PaymentTypes/GetPaymentTypes";
            string uriDeliveryTypes = Application.Current.Properties["ServerLink"].ToString() + "api/DeliveryTypes/GetDeliveryTypes";
            var responsePaymentTypes = await _client.GetAsync(uriPaymentTypes);
            var responseDeliveryTypes = await _client.GetAsync(uriDeliveryTypes);
            if (responsePaymentTypes.StatusCode == System.Net.HttpStatusCode.OK && responseDeliveryTypes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseContentPaymentTypes = await responsePaymentTypes.Content.ReadAsStringAsync();
                var responseContentDeliveryTypes = await responseDeliveryTypes.Content.ReadAsStringAsync();
                paymentTypes = JsonConvert.DeserializeObject<List<PaymentTypeModel>>(responseContentPaymentTypes);
                deliveryTypes = JsonConvert.DeserializeObject<List<DeliveryTypeModel>>(responseContentDeliveryTypes);
                PaymentTypePicker.ItemsSource = paymentTypes;
                DeliverTypePicker.ItemsSource = deliveryTypes;
                PaymentTypePicker.SelectedIndex = 1;
                DeliverTypePicker.SelectedIndex = 1;

            }
            else {
                OrderDelivery.IsEnabled = false;
                await DisplayAlert("Internet", "Check Your connection settings", "Ok"); }
           

        }

        void Notify(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            if (string.IsNullOrWhiteSpace(property.Text))
            {
                property.BackgroundColor = Color.Red;
            }
            else
                property.BackgroundColor = Color.LightSteelBlue;
        }
        void NotifyAddress(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            if (string.IsNullOrWhiteSpace(property.Text) || !property.Text.ToCharArray().Any(char.IsDigit))
            {
                property.BackgroundColor = Color.Red;
            }
            else
                property.BackgroundColor = Color.LightSteelBlue;
        }
        async void NotifyPhone(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            if (property.Text!=null)
            {
                try
                {
                    if (property.Text.ToCharArray().All(char.IsDigit) || Int64.Parse(property.Text).ToString().Count() >= 6)
                    {//(property.Text.IndexOf('+') <= 1)
                        await DisplayAlert("broj prosao proveru", "property.Text.ToCharArray().All(char.IsDigit) || Int64.Parse(property.Text).ToString().Count() >= 6", "ok");
                        property.BackgroundColor = Color.LightSteelBlue;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("error ", ex.Message + "||" + ex.InnerException, "");
                 }
                
            
            }
            else
                property.BackgroundColor = Color.Red;
        }
        
        void NotifyPicker(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Picker;
            if (property.SelectedIndex==0)
            {
                property.BackgroundColor = Color.Red;
            }
            else
                property.BackgroundColor = Color.LightSteelBlue;
        }


    }
}