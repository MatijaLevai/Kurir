using Kurir.Models;
using Kurir.Persistance;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.DispatcherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewDeliveryDispatchPage : ContentPage
    {
        private SQLiteHelper liteHelper;
        private readonly HttpClient _client = App.client;
        private readonly SQLiteAsyncConnection _connection;
        private List<PaymentTypeModel> paymentTypes;
        private List<DeliveryTypeModel> deliveryTypes;
        private List<ActiveCourierModel> activeCouriers;
        private DeliveryModel delivery;
        private DeliveryService deliveryService;
        private AddressService addressService;
        private UserService userService;
        private List<string> streets;
        private FullAddressModel startAddress;
        private FullAddressModel endAddress;
        public NewDeliveryDispatchPage()
        {
            userService = new UserService();
            addressService = new AddressService();
            liteHelper = new SQLiteHelper();
            deliveryService = new DeliveryService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
        }
        public NewDeliveryDispatchPage(DeliveryModel delivery)
        {
            
            addressService = new AddressService();
            userService = new UserService();
            liteHelper = new SQLiteHelper();
            deliveryService = new DeliveryService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            if (delivery != null)
            {
                this.delivery = delivery;
               
            }
            InitializeComponent();
        }
        Func<string, bool> selector = delegate (string c)
        {
            var s = c.ToCharArray();
            if (s.All(char.IsNumber) && s.Count() > 6)
                return true;
            else if (s[0] == '+' && s.Count() > 11)
            {

                bool res = false;
                for (int i = 1; i < s.Count() - 1; i++)
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

        private async Task FillFormAsync()
        {
           // this.startAddress = await addressService.GetAddressByIDAsync(delivery.StartAddressID);
           // this.endAddress = await addressService.GetAddressByIDAsync(delivery.EndAddressID);
            this.Titlelbl.Text = "Update Delivery ";
           StartName.Text = delivery.StartAddress.Name;
           StartAddress.Text = delivery.StartAddress.Address;
           StartPhone.Text = delivery.StartAddress.Phone;
            if(startAddress.Zone>0)
                ZoneStartSteplbl.Text = "4. Zone Start : "+ delivery.StartAddress.Zone.ToString();
            else
                ZoneStartSteplbl.Text = "4. Zone Start : 1";
            NameEnd.Text = delivery.EndAddress.Name;
           EndAddress.Text = delivery.EndAddress.Address;
           PhoneEnd.Text = delivery.EndAddress.Phone;
            if(endAddress.Zone>0)
                ZoneEndSteplbl.Text =" 8.Zone End: " + delivery.EndAddress.Zone.ToString();
            else
                ZoneEndSteplbl.Text = "8.Zone End: 1";
            Price.Text =delivery.DeliveryPrice.ToString();
           DeliveryDetails.Text = delivery.Description;
            PaymentTypePicker.SelectedIndex = delivery.PaymentTypeID-1;
            DeliverTypePicker.SelectedIndex = delivery.DeliveryTypeID-1;
            var unc = activeCouriers.Where(c => c.CourierID == delivery.CourierID).FirstOrDefault();
            if (unc == null)
            {
                var courier = await userService.GetCourierModelByID(delivery.CourierID);
                if (courier.CourierID!= 0)
                {
                    activeCouriers.Add(courier);
                    CourierPicker.ItemsSource = activeCouriers;
                    CourierPicker.SelectedItem = courier;
                }
            }

        }
        private async void DeliveryButton_Clicked(object sender, EventArgs e)
        {
            
            if (!string.IsNullOrWhiteSpace(StartPhone.Text) && !string.IsNullOrWhiteSpace(PhoneEnd.Text) && !string.IsNullOrWhiteSpace(StartName.Text) && !string.IsNullOrWhiteSpace(StartAddress.Text) && !string.IsNullOrWhiteSpace(NameEnd.Text) && !string.IsNullOrWhiteSpace(EndAddress.Text) && !string.IsNullOrWhiteSpace(Price.Text))
            {
                ActiveCourierModel courier = CourierPicker.SelectedItem as ActiveCourierModel;
                FullAddressModel startAddress = new FullAddressModel
                {
                    UserID = Convert.ToInt32(App.Current.Properties["UserID"].ToString()),
                    Phone = StartPhone.Text.Trim(),
                    Name = StartName.Text.Trim(),
                    Address = StartAddress.Text.Trim(),
                    LocationID = 1,
                    Zone = Convert.ToInt32(ZoneStart.Value)

                };
                FullAddressModel endAddress = new FullAddressModel
                {
                    UserID = Convert.ToInt32(App.Current.Properties["UserID"].ToString()),
                    Phone = PhoneEnd.Text.Trim(),
                    Name = NameEnd.Text.Trim(),
                    Address = EndAddress.Text.Trim(),
                    LocationID = 1,
                    Zone = Convert.ToInt32(ZoneEnd.Value)
                };
                DeliveryModel newDelivery = new DeliveryModel()
                {
                    
                    UserID = Convert.ToInt32(Application.Current.Properties["UserID"].ToString()),
                    DispatcherID = Convert.ToInt32(Application.Current.Properties["UserID"].ToString()),
                    CreateTime = DateTime.Now,
                    DeliveryPrice = Convert.ToDecimal(Price.Text),
                  
                    PaymentTypeID = PaymentTypePicker.SelectedIndex + 1,
                    DeliveryTypeID = DeliverTypePicker.SelectedIndex + 1,
                    CourierID = courier.CourierID,
                    Description = DeliveryDetails.Text,
                    DeliveryStatus = 1,
                    StartAddressID = 2,
                    EndAddressID = 2


            };
                if (DeliveryButton.Text.ToLower() == "order")
                {
                    try
                    {
                        var delivery = await deliveryService.CreateDelivery(newDelivery);
                        if (delivery != null)
                        {
                            await DisplayAlert("All Done.", "Your order is placed.", "Confirm.");
                            await Navigation.PopAsync();
                            await Navigation.PushAsync(new DispatcherHomePage());
                        }
                        else
                            await DisplayAlert("Something went wrong.", "Please try again.", "ok");

                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Something went wrong.", "Please try again." + ex.Message, "ok");
                    }
                }
                else
                {
                    if (delivery != null)
                    {
                        
                        newDelivery.DeliveryID = delivery.DeliveryID;
                        newDelivery.UserID = delivery.UserID;
                       
                        newDelivery.CreateTime = delivery.CreateTime;
                        if (checkStartAddress(startAddress))
                        {
                            newDelivery.StartAddressID = delivery.StartAddressID;
                        }
                        else
                        {
                            object sAddress = await addressService.PostFullAddress(startAddress);

                            newDelivery.StartAddressID = await TypeHelp(sAddress);

                        }
                        if (checkEndAddress(endAddress))
                        {
                            newDelivery.EndAddressID = delivery.EndAddressID;
                        }
                        else
                        {
                            object eAddress = await addressService.PostFullAddress(endAddress);
                            newDelivery.EndAddressID = await TypeHelp(eAddress);
                        }
                        try
                        {
                            var delivery = await deliveryService.EditDelivery(newDelivery);
                            if (delivery != null)
                            {
                                await DisplayAlert("All Done.", "Your order is placed.", "Confirm.");
                                await Navigation.PopAsync();
                                await Navigation.PushAsync(new DispatcherHomePage());
                            }
                            else await DisplayAlert("Something went wrong.", "Please try again.", "ok");
                        }
                        catch (Exception ex) { await DisplayAlert("error", "msg: " + ex.Message + "IE : " + ex.InnerException, "jbg"); }
                    }
                 }
            }
            else
            {
                await DisplayAlert("Atention!", "Please fill Entrys correctly.", "ok?");
            }
        }
        private async Task<bool> DelAndPayTypes()
        {
            try
            {

                var response = await liteHelper.UpdateSQLiteDb();
                if (response)
                {

                    paymentTypes = await _connection.Table<PaymentTypeModel>().ToListAsync();
                    deliveryTypes = await _connection.Table<DeliveryTypeModel>().ToListAsync();
                    PaymentTypePicker.ItemsSource = paymentTypes;
                    DeliverTypePicker.ItemsSource = deliveryTypes;
                    PaymentTypePicker.SelectedIndex = 0;
                    DeliverTypePicker.SelectedIndex = 0;
                    return true;
                }
                else
                {

                    DeliveryButton.IsEnabled = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Problem occurred", ex.Message, "ok");
                return false;
            }

        }

        private async Task<bool> ActiveCourierListUpdate()
        {
            
            var response = await userService.GetActiveCouriers();
            if (response != null)
            {
                activeCouriers = new List<ActiveCourierModel>(response);
                CourierPicker.ItemsSource = activeCouriers ;
                //CourierPicker.;
                CourierPicker.SelectedIndex = 0;
                return true;


            }
            else return false;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (await ActiveCourierListUpdate())
            {
                if (!await DelAndPayTypes())
                    await DisplayAlert("Internet", "Check Your connection settings", "Ok");
                if (delivery != null)
                {
                    Titlelbl.Text = "Edit delivery";
                    DeliveryButton.Text = "Finish";
                    await FillFormAsync();
                }
                else
                {
                    Titlelbl.Text = "Create delivery";
                    DeliveryButton.Text = "Order";
                }
            }
            else { await DisplayAlert("Error","None of couriers are active right now.","ok");

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
                property.TextColor = Color.White;
        }
        void NotifyAddressStart(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            listStartAddres.ItemsSource = new List<string>(GetAddresses(property.Text));
            if (!string.IsNullOrWhiteSpace(property.Text))
            {
                if (property.Text.ToCharArray().Any(char.IsDigit))
                    property.TextColor = Color.White;
            }
            else
                property.TextColor = Color.Red;

        }
        void NotifyAddressEnd(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            listEndAddres.ItemsSource = new List<string>(GetAddresses(property.Text));
            if (!string.IsNullOrWhiteSpace(property.Text))
            {
                if (property.Text.ToCharArray().Any(char.IsDigit))
                    property.TextColor = Color.White;
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
                        property.TextColor = Color.White;
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
            if (property.SelectedIndex >= 0)
            {
                property.BackgroundColor = Color.FromHex("#02ce0a");

            }
            else
                property.BackgroundColor = Color.Red;
        }

        private void ZoneStart_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string s = ZoneStartSteplbl.Text;
            string[] se = s.Split(':');
            se[1]= ZoneStart.Value.ToString();
            ZoneStartSteplbl.Text = se[0] +":"+ se[1];
        }

        private void ZoneEnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string s = ZoneEndSteplbl.Text;
            string[] se = s.Split(':');
            se[1] = ZoneEnd.Value.ToString();
            ZoneEndSteplbl.Text = se[0] + ":" + se[1];
        }

        private void Price_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            if (string.IsNullOrWhiteSpace(property.Text))
            {

                property.TextColor = Color.Red;
            }
            else if (!Price.Text.ToCharArray().All(char.IsDigit))
            {
                property.TextColor = Color.Red;
            }
            else
            {
                property.TextColor = Color.White;
            }

        }

        private async void Address()
        {
            await DisplayAlert("Address should look like :", " Name of street building number / apartment / floor / interphone", "ok");
        }

        IEnumerable<string> GetAddresses(string searchText = null)
        {
            streets = new List<string>(liteHelper.StreetsOfNs());
            if (String.IsNullOrEmpty(searchText))
                return streets;
            else return streets.Where(c => c.ToLower().StartsWith(searchText));

        }

        private void ListStartAddres_SelectedIndexChanged(object sender, EventArgs e)
        {
            var x = sender as ListView;

            StartAddress.Text = x.SelectedItem.ToString();
            StartAddress.Focus();
        }
        private void ListEndAddres_SelectedIndexChanged(object sender, EventArgs e)
        {
            var x = sender as ListView;

            EndAddress.Text = x.SelectedItem.ToString();
            EndAddress.Focus();
        }
        private bool checkStartAddress(FullAddressModel startAddress)
        {
            if (startAddress.Name == this.startAddress.Name)
            {
                if (startAddress.Address == this.startAddress.Address)
                {
                    if (startAddress.Phone == this.startAddress.Phone)
                    {
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }
        private bool checkEndAddress(FullAddressModel endAddress)
        {
            if (endAddress.Name == this.endAddress.Name)
            {
                if (endAddress.Address == this.endAddress.Address)
                {
                    if (endAddress.Phone == this.endAddress.Phone)
                    {
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }
        private async Task<int> TypeHelp(object address)
        {
            switch (address)
            {
                case FullAddressModel model:
                    return model.FullAddressID;

                case string stringMsg:
                    await DisplayAlert("Error", stringMsg, "ok");
                    return 0;
                default:
                    await DisplayAlert("Error", "debug errror.", "ok");
                    //await this.Navigation.PopAsync();
                    return 0;
            }
        }
    }
}