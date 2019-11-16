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

namespace Kurir.CourierPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryCreateEditPage : ContentPage
    {
        private SQLiteHelper liteHelper;
        private readonly HttpClient _client = App.client;
        private readonly SQLiteAsyncConnection _connection;
        private List<PaymentTypeModel> paymentTypes;
        private List<DeliveryTypeModel> deliveryTypes;
        private List<ActiveCourierModel> activeCouriers;
        private DeliveryModel delivery;
        private DeliveryModel PotencialDelivery;
        private DeliveryService deliveryService;
        private AddressService addressService;
        private UserService userService;
        private List<FullAddressModel> Addresses;
        private FullAddressModel startAddress;
        private FullAddressModel endAddress;
        private readonly double step = 30;
        public DeliveryCreateEditPage()
        {
            userService = new UserService();
            addressService = new AddressService();
            liteHelper = new SQLiteHelper();
            deliveryService = new DeliveryService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            PotencialDelivery = new DeliveryModel() { DeliveryPrice = 160, DeliveryStatus = 1, UserID = Convert.ToInt32(Application.Current.Properties["UserID"]), DispatcherID = Convert.ToInt32(Application.Current.Properties["UserID"]), StartAddress = new FullAddressModel() { UserID = Convert.ToInt32(Application.Current.Properties["UserID"]), Address = "", Phone = "+3816", Name = "" }, EndAddress = new FullAddressModel() { UserID = Convert.ToInt32(Application.Current.Properties["UserID"]), Address = "", Phone = "+3816", Name = "" } };

            InitializeComponent();
        }
        public DeliveryCreateEditPage(DeliveryModel delivery)
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
            this.startAddress = await addressService.GetAddressByIDAsync(delivery.StartAddressID);
            this.endAddress = await addressService.GetAddressByIDAsync(delivery.EndAddressID);
            this.Titlelbl.Text = "Ažuriranje dostave";
            DeliveryGrid.BindingContext = delivery;
            // StartName.Text = delivery.StartAddress.Name;
            // StartAddress.Text = delivery.StartAddress.Address;
            //  StartPhone.Text = delivery.StartAddress.Phone;
            if (delivery.StartAddress.Zone > 0)
                ZoneStartSteplbl.Text = "4. Zona adrese pošiljaoca : " + delivery.StartAddress.Zone.ToString();
            else
                ZoneStartSteplbl.Text = "4. Zona adrese pošiljaoca : 1";
            // NameEnd.Text = delivery.EndAddress.Name;
            // EndAddress.Text = delivery.EndAddress.Address;
            //  PhoneEnd.Text = delivery.EndAddress.Phone;
            if (delivery.EndAddress.Zone > 0)
                ZoneEndSteplbl.Text = " 8. Zona adrese primaoca : " + delivery.EndAddress.Zone.ToString();
            else
                ZoneEndSteplbl.Text = " 8. Zona adrese primaoca : 1";
            //  Price.Text =delivery.DeliveryPrice.ToString();
            //  DeliveryDetails.Text = delivery.Description;
            PaymentTypePicker.SelectedIndex = delivery.PaymentTypeID - 1;
            DeliverTypePicker.SelectedIndex = delivery.DeliveryTypeID - 1;
            //var unc = activeCouriers.Where(c => c.CourierID == delivery.CourierID).FirstOrDefault();
            //if (unc == null)
            //{
            //    var courier = await userService.GetCourierModelByID(delivery.CourierID);
            //    if (courier.CourierID != 0)
            //    {
            //        activeCouriers.Add(courier);
            //        CourierPicker.ItemsSource = activeCouriers;
            //        CourierPicker.SelectedItem = courier;
            //    }
            //}

        }
        private async Task<bool> CheckDelivery(DeliveryModel delivery)
        {
            if (!string.IsNullOrWhiteSpace(delivery.StartAddress.Phone) && !string.IsNullOrWhiteSpace(delivery.EndAddress.Phone) && !string.IsNullOrWhiteSpace(delivery.StartAddress.Name) && !string.IsNullOrWhiteSpace(delivery.StartAddress.Address) && !string.IsNullOrWhiteSpace(delivery.EndAddress.Name) && !string.IsNullOrWhiteSpace(delivery.EndAddress.Address) && !(delivery.DeliveryPrice < 100))
            { return true; }
            else
            {
                await DisplayAlert("Nisu sva polja popunjena", "Popuni sva polja.", "Potvrdi");
                return false;
            }
        }
        private async void DeliveryButton_Clicked(object sender, EventArgs e)
        {
            //ActiveCourierModel courier = CourierPicker.SelectedItem as ActiveCourierModel;
            //FullAddressModel startAddress = new FullAddressModel
            //{
            //    UserID = Convert.ToInt32(App.Current.Properties["UserID"].ToString()),
            //    Phone = StartPhone.Text.Trim(),
            //    Name = StartName.Text.Trim(),
            //    Address = StartAddress.Text.Trim(),
            //    LocationID = 1,
            //    Zone = Convert.ToInt32(ZoneStart.Value)

            //};
            //FullAddressModel endAddress = new FullAddressModel
            //{
            //    UserID = Convert.ToInt32(App.Current.Properties["UserID"].ToString()),
            //    Phone = PhoneEnd.Text.Trim(),
            //    Name = NameEnd.Text.Trim(),
            //    Address = EndAddress.Text.Trim(),
            //    LocationID = 1,
            //    Zone = Convert.ToInt32(ZoneEnd.Value)
            //};
            //DeliveryModel newDelivery = new DeliveryModel()
            //{

            //    UserID = Convert.ToInt32(Application.Current.Properties["UserID"].ToString()),
            //    DispatcherID = Convert.ToInt32(Application.Current.Properties["UserID"].ToString()),
            //    CreateTime = DateTime.Now,
            //    DeliveryPrice = Convert.ToDecimal(Price.Text),

            //    PaymentTypeID = PaymentTypePicker.SelectedIndex + 1,
            //    DeliveryTypeID = DeliverTypePicker.SelectedIndex + 1,
            //    CourierID = courier.CourierID,
            //    Description = DeliveryDetails.Text,
            //    DeliveryStatus = 1,
            //    StartAddressID = 2,
            //    EndAddressID = 2


            //};
            if (DeliveryButton.Text.ToLower() == "završi" && PotencialDelivery != null)
            {
                if (await CheckDelivery(PotencialDelivery))
                {

                    try
                    {
                        PotencialDelivery.PaymentTypeID = PaymentTypePicker.SelectedIndex + 1;
                        PotencialDelivery.DeliveryTypeID = DeliverTypePicker.SelectedIndex + 1;
                        PotencialDelivery.CourierID =Convert.ToInt32(Application.Current.Properties["UserID"]);
                        PotencialDelivery.StartAddress.Zone = Convert.ToInt32(ZoneStart.Value);
                        PotencialDelivery.EndAddress.Zone = Convert.ToInt32(ZoneEnd.Value);
                        PotencialDelivery.CreateTime = DateTime.Now;

                        ///Nova Start adressa
                        // var postStartAddress = await addressService.PostFullAddress(PotencialDelivery.StartAddress);
                        // PotencialDelivery.StartAddressID = await TypeHelp(postStartAddress);
                        PotencialDelivery = await StartAddressCheck(PotencialDelivery);
                        if (PotencialDelivery == null)
                            await DisplayAlert("Greska", "", "Potvrdi");
                        ///Nova End Addressa
                        // var postEndAddress = await addressService.PostFullAddress(startAddress);
                        //PotencialDelivery.EndAddressID = await TypeHelp(postEndAddress);
                        PotencialDelivery = await EndAddressCheck(PotencialDelivery);
                        if (PotencialDelivery == null)
                            await DisplayAlert("Greska", "", "Potvrdi");
                        DeliveryModel dtodel = PotencialDelivery;
                        dtodel.StartAddress = null;
                        dtodel.EndAddress = null;

                        var PostDelivery = await deliveryService.CreateDelivery(dtodel);

                        if (PostDelivery != null)
                        {
                            await DisplayAlert("BaraBara", "Dostava je poslata kuriru", "Potvrdi.");
                            await Navigation.PopAsync();
                        }
                        else
                            await DisplayAlert("Došlo je do greške.", "Pokušaj ponovo.", "Potvrdi");

                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Došlo je do greške.", "Pokušaj ponovo. " + Environment.NewLine + "Tekst greške : " + ex.Message, "Potvrdi");
                    }
                }
            }
            else
            {
                if (delivery != null)
                {
                    if (await CheckDelivery(delivery))
                    {
                        switch (delivery.DeliveryStatus)
                        {
                            case 0:
                                delivery.DeliveryStatus = 1;
                                break;
                            case 1:
                                delivery.DeliveryStatus = 1;
                                break;
                            case 2:
                                delivery.DeliveryStatus = 2;
                                break;
                            case 3:
                                delivery.DeliveryStatus = 3;
                                break;
                            case 4:
                                delivery.DeliveryStatus = 4;
                                break;

                        }
                        delivery.PaymentTypeID = PaymentTypePicker.SelectedIndex + 1;
                        delivery.DeliveryTypeID = DeliverTypePicker.SelectedIndex + 1;
                        delivery.CourierID = Convert.ToInt32(Application.Current.Properties["UserID"]);
                        delivery.StartAddress.Zone = Convert.ToInt32(ZoneStart.Value);
                        delivery.EndAddress.Zone = Convert.ToInt32(ZoneEnd.Value);
                        delivery = await StartAddressCheck(delivery);
                        if (delivery == null)
                            await DisplayAlert("Greska", "", "Potvrdi");
                        ///Nova End Addressa
                        // var postEndAddress = await addressService.PostFullAddress(startAddress);
                        //PotencialDelivery.EndAddressID = await TypeHelp(postEndAddress);
                        delivery = await EndAddressCheck(delivery);
                        if (delivery == null)
                            await DisplayAlert("Greska", "", "Potvrdi");

                        //if (!checkStartAddress(startAddress))
                        //    {
                        //    // delivery.StartAddressID = delivery.StartAddressID;
                        //    object sAddress = await addressService.PostFullAddress(startAddress);

                        //    delivery.StartAddressID = await TypeHelp(sAddress);

                        //}

                        //    if (!checkEndAddress(endAddress))
                        //    {
                        //       // newDelivery.EndAddressID = delivery.EndAddressID;
                        //    object eAddress = await addressService.PostFullAddress(endAddress);
                        //    delivery.EndAddressID = await TypeHelp(eAddress);
                        //}

                        try
                        {
                            var PostDelivery = await deliveryService.EditDelivery(delivery);
                            if (PostDelivery != null)
                            {
                                await DisplayAlert("BaraBara", "Dostava je ažurirana.", "Potvrdi.");
                                await Navigation.PopAsync();
                            }
                            else await DisplayAlert("Došlo je do greške.", "Pokušaj ponovo.", "Potvrdi");

                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Došlo je do greške.", "Pokušaj ponovo. " + Environment.NewLine + "Tekst greške : " + ex.Message, "Potvrdi");
                        }
                    }

                }
            }
        }
        private async Task<bool> DelAndPayTypes()
        {
            try
            {

                var response = await liteHelper.UpdateSQLiteDbWithPayAndDelTypes();
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
                await DisplayAlert("Greška na serveru.", ex.Message, "Potvrdi");
                return false;
            }

        }

        //private async Task<bool> ActiveCourierListUpdate()
        //{

        //    var response = await userService.GetActiveCouriers();
        //    if (response != null)
        //    {
        //        activeCouriers = new List<ActiveCourierModel>(response);
        //        CourierPicker.ItemsSource = activeCouriers;
        //        //CourierPicker.;
        //        CourierPicker.SelectedIndex = 0;
        //        return true;


        //    }
        //    else return false;
        //}
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //if (await ActiveCourierListUpdate())
            //{
                if (!await DelAndPayTypes())
                    await DisplayAlert("Greška na serveru", "Proveri konekciju na internet. Ako je povezan javi Matiji.", "Ok");
                if (delivery != null)
                {
                    DeliveryGrid.BindingContext = delivery;
                    Titlelbl.Text = "Ažuriranje dostave";
                    DeliveryButton.Text = "Ažuriraj";
                    await FillFormAsync();
                }
                else
                {
                    Titlelbl.Text = "Kreiranje dostave";
                    DeliveryButton.Text = "Završi";
                    DeliveryGrid.BindingContext = PotencialDelivery;
                }
            //}
            //else
           // {
              //  await DisplayAlert("Greška na serveru.", "Nema aktivnih kurira. Javi im da se uloguju na sistem i aktiviraju lociranje.", "Potvrdi");

          //  }



        }

        private async void NotifyNameStart(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            listStartAddres.ItemsSource = new List<FullAddressModel>(await GetAddressesByName(property.Text));
            if (string.IsNullOrWhiteSpace(property.Text))
            {
                property.TextColor = Color.Red;
            }
            else
                property.TextColor = Color.Black;
        }
        private async void NotifyNameEnd(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            listEndAddres.ItemsSource = new List<FullAddressModel>(await GetAddressesByName(property.Text));
            if (string.IsNullOrWhiteSpace(property.Text))
            {
                property.TextColor = Color.Red;
            }
            else
                property.TextColor = Color.Black;
        }
        private async void NotifyAddressStart(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            listStartAddres.ItemsSource = new List<FullAddressModel>(await GetAddressesByAddress(property.Text));
            if (!string.IsNullOrWhiteSpace(property.Text))
            {
                if (property.Text.ToCharArray().Any(char.IsDigit))
                {
                    //if (property.Text.ToCharArray().Any(x => x.ToString() == "/"))
                    property.TextColor = Color.Black;
                    // else { property.TextColor = Color.Black;
                    //     await DisplayAlert("Sprat  ","  ", "ok"); }
                }
            }
            else
                property.TextColor = Color.Red;

        }
        private async void NotifyAddressEnd(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            listEndAddres.ItemsSource = new List<FullAddressModel>(await GetAddressesByAddress(property.Text));
            if (!string.IsNullOrWhiteSpace(property.Text))
            {
                if (property.Text.ToCharArray().Any(char.IsDigit))
                    property.TextColor = Color.Black;

            }
            else
                property.TextColor = Color.Red;

        }
        async void NotifyPhoneStart(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            listStartAddres.ItemsSource = await GetAddressesByPhone(property.Text);
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
        async void NotifyPhoneEnd(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            listEndAddres.ItemsSource = await GetAddressesByPhone(property.Text);
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
            se[1] = ZoneStart.Value.ToString();
            ZoneStartSteplbl.Text = se[0] + ":" + se[1];
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
            else if (Price.Text.ToCharArray().All(char.IsDigit))
            {
                property.TextColor = Color.Black;
            }
            else
            {
                try
                {
                    Convert.ToInt32(property.Text);
                    property.TextColor = Color.Black;
                }
                catch { property.TextColor = Color.Red; }
            }
        }
        private async Task<IEnumerable<FullAddressModel>> GetAddressesByAddress(string searchText = null)
        {
            Addresses = new List<FullAddressModel>(await addressService.GetAllAddressesAsync());
            if (String.IsNullOrEmpty(searchText))
                return Addresses;
            else return Addresses.Where(c => (c.Address.ToLower().StartsWith(searchText.ToLower().Trim())));

        }
        private async Task<IEnumerable<FullAddressModel>> GetAddressesByPhone(string searchText = null)
        {
            Addresses = new List<FullAddressModel>(await addressService.GetAllAddressesAsync());
            if (String.IsNullOrEmpty(searchText))
                return Addresses;
            else return Addresses.Where(c => (c.Phone.ToString().ToLower().StartsWith(searchText.ToLower().Trim())));

        }
        private async Task<IEnumerable<FullAddressModel>> GetAddressesByName(string searchText = null)
        {
            Addresses = new List<FullAddressModel>(await addressService.GetAllAddressesAsync());
            if (String.IsNullOrEmpty(searchText))
                return Addresses;
            else return Addresses.Where(c => (c.Name.ToLower().StartsWith(searchText.ToLower().Trim())));

        }

        private void ListStartAddres_SelectedIndexChanged(object sender, EventArgs e)
        {
            var x = sender as ListView;
            if (delivery == null)
            {
                PotencialDelivery.StartAddress = (FullAddressModel)x.SelectedItem;
                DeliveryGrid.BindingContext = null;
                DeliveryGrid.BindingContext = PotencialDelivery;
            }
            else
            {
                delivery.StartAddress = (FullAddressModel)x.SelectedItem;
                DeliveryGrid.BindingContext = null;
                DeliveryGrid.BindingContext = delivery;
            }
        }
        private void ListEndAddres_SelectedIndexChanged(object sender, EventArgs e)
        {

            var x = sender as ListView;
            if (delivery == null)
            {
                PotencialDelivery.EndAddress = (FullAddressModel)x.SelectedItem;
                DeliveryGrid.BindingContext = null;
                DeliveryGrid.BindingContext = PotencialDelivery;
            }
            else
            {
                delivery.EndAddress = (FullAddressModel)x.SelectedItem;
                DeliveryGrid.BindingContext = null;
                DeliveryGrid.BindingContext = delivery;
            }
        }
        //private bool checkStartAddress(FullAddressModel startAddress)
        //{
        //    if (delivery.StartAddress.Name == this.startAddress.Name)
        //    {
        //        if (delivery.StartAddress.Address == this.startAddress.Address)
        //        {
        //            if (delivery.StartAddress.Phone == this.startAddress.Phone)
        //            {
        //                return true;
        //            }
        //            else return false;
        //        }
        //        else return false;
        //    }
        //    else return false;
        //}
        //private bool checkEndAddress(FullAddressModel endAddress)
        //{
        //    if (delivery.EndAddress.Name == this.endAddress.Name)
        //    {
        //        if (delivery.EndAddress.Address == this.endAddress.Address)
        //        {
        //            if (delivery.EndAddress.Phone == this.endAddress.Phone)
        //            {
        //                return true;
        //            }
        //            else return false;
        //        }
        //        else return false;
        //    }
        //    else return false;
        //}
        private async Task<int> TypeHelp(object address)
        {
            switch (address)
            {
                case FullAddressModel model:
                    return model.FullAddressID;

                case string stringMsg:
                    await DisplayAlert("Greška", stringMsg, "ok");
                    return 0;
                default:
                    await DisplayAlert("Greška", "debug errror.", "ok");

                    return 0;
            }
        }
        private async Task<DeliveryModel> PostContextOfStartAddress(DeliveryModel delivery)
        {
            try
            {
                delivery.StartAddressID = 0;
                delivery.StartAddress.FullAddressID = 0;
                delivery.StartAddress.FullAddressID = await TypeHelp(await addressService.PostFullAddress(delivery.StartAddress));
                delivery.StartAddressID = delivery.StartAddress.FullAddressID;
                return delivery;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Server Error" + ex.Message + ex.InnerException, "Ok");
                return null;
            }

        }

        private async Task<DeliveryModel> StartAddressCheck(DeliveryModel delivery)
        {

            if (delivery.StartAddressID > 0 && delivery.StartAddress != null)
            {
                FullAddressModel startAddressFromDB = await addressService.GetAddressByIDAsync(delivery.StartAddressID);
                if (startAddressFromDB != null)
                {
                    if (delivery.StartAddress.CompareTo(startAddressFromDB).ContainsValue(false))
                    {
                        string str = await DisplayActionSheet("Ažuriraj postojeći ili kreraj novi šablon adrese?", null, null, "Ažuriraj", "Kreiraj");
                        if (str == "Ažuriraj")
                        {   //soft delete to save all addresses as they where
                            bool put = await Delete(startAddressFromDB);
                            var post = await PostContextOfStartAddress(delivery);
                            if (put && post != null)
                            {
                                await DisplayAlert("BaraBara", "Uspešno ažuriran šablon adrese.", "Ok");
                                return post;
                            }
                            else
                            {
                                return null;
                            }

                        }
                        else //if (str == "Kreiraj")
                        {
                            var post = await PostContextOfStartAddress(delivery);
                            if (post != null)
                            {

                                await DisplayAlert("BaraBara", "Uspešno kreiran šablon adrese.", "Ok");
                                return post;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("BaraBara", "Uspešno izabran postojeći šablon adrese.", "Ok");
                        return delivery;
                    }
                }
                else
                {

                    var post = await PostContextOfStartAddress(delivery);
                    if (post != null)
                    {

                        await DisplayAlert("BaraBara", "Uspešno kreiran nov šablon adrese.", "Ok");
                        return post;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            else
            {

                var post = await PostContextOfStartAddress(delivery);
                if (post != null)
                {

                    await DisplayAlert("BaraBara", "Uspešno kreiran nov šablon adrese.", "Ok");
                    return post;
                }
                else
                {
                    return null;
                }

            }
        }
        private async Task<DeliveryModel> PostContextOfEndAddress(DeliveryModel delivery)
        {
            try
            {
                delivery.EndAddressID = 0;
                delivery.EndAddress.FullAddressID = 0;
                delivery.EndAddress.FullAddressID = await TypeHelp(await addressService.PostFullAddress(delivery.EndAddress));
                delivery.EndAddressID = delivery.EndAddress.FullAddressID;
                return delivery;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Server Error" + ex.Message + ex.InnerException, "Ok");
                return null;
            }

        }

        private async Task<DeliveryModel> EndAddressCheck(DeliveryModel delivery)
        {

            if (delivery.EndAddressID > 0 && delivery.EndAddress != null)
            {
                FullAddressModel endAddressFromDB = await addressService.GetAddressByIDAsync(delivery.EndAddressID);
                if (endAddressFromDB != null)
                {
                    if (delivery.EndAddress.CompareTo(endAddressFromDB).ContainsValue(false))
                    {
                        string str = await DisplayActionSheet("Ažuriraj postojeći ili kreraj novi šablon adrese?", null, null, "Ažuriraj", "Kreiraj");
                        if (str == "Ažuriraj")
                        {   //soft delete to save all addresses as they where
                            bool put = await Delete(endAddressFromDB);
                            var post = await PostContextOfEndAddress(delivery);
                            if (put && post != null)
                            {
                                await DisplayAlert("BaraBara", "Uspešno ažuriran šablon adrese.", "Ok");
                                return post;
                            }
                            else
                            {
                                return null;
                            }

                        }
                        else //if (str == "Kreiraj")
                        {
                            var post = await PostContextOfEndAddress(delivery);
                            if (post != null)
                            {

                                await DisplayAlert("BaraBara", "Uspešno kreiran šablon adrese.", "Ok");
                                return post;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("BaraBara", "Uspešno izabran postojeći šablon adrese.", "Ok");
                        return delivery;
                    }
                }
                else
                {

                    var post = await PostContextOfEndAddress(delivery);
                    if (post != null)
                    {

                        await DisplayAlert("BaraBara", "Uspešno kreiran nov šablon adrese.", "Ok");
                        return post;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            else
            {

                var post = await PostContextOfEndAddress(delivery);
                if (post != null)
                {

                    await DisplayAlert("BaraBara", "Uspešno kreiran nov šablon adrese.", "Ok");
                    return post;
                }
                else
                {
                    return null;
                }

            }
        }
        private async Task<bool> Delete(FullAddressModel endAddressFromDB)
        {
            try
            {
                string m = await addressService.DeleteAddress(endAddressFromDB.FullAddressID);

                switch (m)
                {
                    case "Address with given parametters not found.":
                        return false;
                    case "address successesfully deleted.":
                        return true;
                    default:
                        return false;
                }
                //await DisplayAlert("Server Message", m, "Ok");

            }
            catch
            {
                //await DisplayAlert("Error", "Server Error" + ex.Message + ex.InnerException, "Ok");
                return false;
            }
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Slider s = (Slider)sender;
            int value =Convert.ToInt32( Math.Round(s.Value/step));
            s.Value = value*step;

           //Slider value round on 30 mins
            if (delivery != null)
            {
                delivery.WaitingInMinutes = value;
            }
            else
            {
                PotencialDelivery.WaitingInMinutes = value;
            }
        }
    }
}