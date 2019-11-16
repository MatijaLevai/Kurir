using Kurir.Models;
using Kurir.Persistance;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.UserPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewDelivery : ContentPage
    {
        #region Properties of partial Class New Delivery
        private SQLiteHelper liteHelper;
        private readonly HttpClient _client ;
        private readonly SQLiteAsyncConnection _connection;
        private  List<PaymentTypeModel> paymentTypes;
        private  List<DeliveryTypeModel> deliveryTypes;
        private DeliveryModel delivery;
        private DeliveryService deliveryService;
        private AddressService addressService;
        private bool EditEnabled;
       
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
        #region Constructor
        public NewDelivery(DeliveryModel delivery,bool editEnabled)
        {
            EditEnabled = editEnabled;
            _client = App.client;
            liteHelper = new SQLiteHelper();
            deliveryService = new DeliveryService();
            addressService = new AddressService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            if (delivery != null)
            {
                this.delivery = delivery;
                this.BindingContext = this.delivery;
            }
            InitializeComponent();
        }
        #endregion
        
        private async Task<bool> DelAndPayTypes()
        {
            try
            {

                var response = await liteHelper.UpdateSQLiteDbWithPayAndDelTypes();
                if (response)
                {
                    #region PaymentType
                    //< Label HorizontalTextAlignment = "Center" FontSize = "Medium" Text = "7. Chose payment type:" />
                    //< Picker HorizontalOptions = "Center" FontSize = "Medium" x: Name = "PaymentTypePicker" PropertyChanged = "NotifyPicker" TextColor = "Black"  ItemDisplayBinding = "{Binding PaymentTypeName}" ></ Picker >
                    //paymentTypes = await _connection.Table<PaymentTypeModel>().ToListAsync();
                    // PaymentTypePicker.ItemsSource = paymentTypes;
                    // PaymentTypePicker.SelectedIndex = 0;
                    #endregion
                    deliveryTypes = await _connection.Table<DeliveryTypeModel>().ToListAsync();
                  
                    DeliverTypePicker.ItemsSource = deliveryTypes;
                  
                    DeliverTypePicker.SelectedIndex = 0;
                    return true;
                }
                else
                {

                    OrderDelivery.IsEnabled = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Problem occurred", ex.Message, "ok");
                return false;
            }

        }
        protected override async void OnAppearing()
        {
            if (delivery == null)
            {
                await DisplayAlert("error", "Create new delivery", "ok").ConfigureAwait(false);
                Navigation.RemovePage(this);
                await Navigation.PushAsync(new StartAddressPage());
            }
            if (await DelAndPayTypes())
            {
                if (EditEnabled)
                {
                    Titlelbl.Text = "Edit Delivery";
                    OrderDelivery.Text = "Edit";
                }
                else
                {
                    Titlelbl.Text = "Create delivery";
                    OrderDelivery.Text = "Order";
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
            else { await DisplayAlert("Internet", "Check Your connection settings", "Ok"); }
        }

        private async void OrderDelivery_Clicked(object sender, EventArgs e)
        {
            bool done = false;
            try
            {
                #region Check start address
                
                #endregion
                #region Check end address
                
                #endregion
                #region Check Delivery
                if (delivery.DeliveryID > 0&&EditEnabled)
                {
                    DeliveryModel deliveryFromDB = await deliveryService.GetDeliveryByID(delivery.DeliveryID);
                    if (deliveryFromDB != null)
                    {
                        if (delivery.CompareTo(deliveryFromDB).ContainsValue(false))
                        {
                            await deliveryService.EditDelivery(delivery);
                            await DisplayAlert("Success", "Updated existing delivery", "Ok");
                            done = true;
                            #region UpdateOrCreateDelivery
                            //string str = await DisplayActionSheet("Update or Create new delivery?", "cancel", null, "Update", "Create");
                            //if (str == "Update")
                            //{
                            //    

                            //}
                            //else if (str == "Create")
                            //{
                            //    delivery.CreateTime = DateTime.Now;
                            //    delivery.StartTime = Convert.ToDateTime("0001 - 01 - 01 00:00:00.0000000");
                            //    delivery.EndTime = Convert.ToDateTime("0001 - 01 - 01 00:00:00.0000000");
                            //    delivery.WaitingInMinutes = 0;
                            //    delivery.DeliveryStatusImageSource = "zuti50.png";
                            //    delivery.CourierID = 0;
                            //    delivery.DispatcherID = 0;
                            //    delivery.UserID = Convert.ToInt32(Application.Current.Properties["UserID"]);
                            //    delivery.DeliveryStatus = 0;
                            //    delivery = await deliveryService.CreateDelivery(delivery);
                            //    await DisplayAlert("Success", "New delivery created .", "Ok");

                            //}
                            #endregion
                        }
                        else
                        {
                            done = true;
                            await DisplayAlert("Done", "No changes are made to delivery.", "Ok");
                        }
                    }
                    else
                    {
                        delivery = await deliveryService.CreateDelivery(delivery);
                        done = true;
                        await DisplayAlert("Success", "New delivery created .", "Ok");
                    }
                }
                else
                {
                    //workoround for inserting object of delivery with objects of addresses as propeties
                    var del = delivery;
                    del.StartAddress = null;
                    del.EndAddress = null;
                    await deliveryService.CreateDelivery(del);
                    await DisplayAlert("Success", "Created new delivery.", "Ok");
                    done = true;
                   
                }
                if (done)
                {
                    await Navigation.PopToRootAsync();
                }
            }
            #endregion
            catch (Exception ex)
            {
                await DisplayAlert("error", "msg: " + ex.Message + "IE : " + ex.InnerException, "ok");
            }
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
           
        void Notify(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            if (string.IsNullOrWhiteSpace(property.Text)&&property.Text.ToArray().Count()<3)
            {
                property.TextColor = Color.Red;
            }
            else
                property.TextColor = Color.Black;
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

#region Notify Error Entering Address Details
//void NotifyAddress(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//{
//    var property = sender as Entry;
//    if (!string.IsNullOrWhiteSpace(property.Text))
//    {
//        if (property.Text.ToCharArray().Any(char.IsDigit))
//        property.TextColor = Color.Black;
//    }
//    else
//        property.TextColor = Color.Red;

//}
//async void NotifyPhone(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//{
//    var property = sender as Entry;
//    if (!string.IsNullOrWhiteSpace(property.Text))
//    {
//        try
//        {
//            if (selector(property.Text))
//            {
//              property.TextColor = Color.Black;
//            }
//            else
//            {
//                property.TextColor = Color.Red;
//            }

//        }
//        catch (Exception ex)
//        {
//            await DisplayAlert("error ", ex.Message + "||" + ex.InnerException, "ok");
//         }


//    }
//    else
//        property.TextColor = Color.Red;
//}  
//private bool CheckStartAddress(FullAddressModel startAddress)
//{
//    if (startAddress.Name == this.delivery.StartAddress.Name)
//    {
//        if (startAddress.Address == this.delivery.StartAddress.Address)
//        {
//            if (startAddress.Phone == this.delivery.StartAddress.Phone)
//            {
//                return true;
//            }
//            else return false;
//        }
//        else return false;
//    }
//    else return false;
//}
//private bool CheckEndAddress(FullAddressModel endAddress)
//{
//    if (endAddress.Name == this.delivery.EndAddress.Name)
//    {
//        if (endAddress.Address == this.delivery.EndAddress.Address)
//        {
//            if (endAddress.Phone == this.delivery.EndAddress.Phone)
//            {
//                return true;
//            }
//            else return false;
//        }
//        else return false;
//    }
//    else return false;
//}
#endregion