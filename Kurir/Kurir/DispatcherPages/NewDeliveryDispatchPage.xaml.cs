using Kurir.Models;
using Kurir.Persistance;
using Kurir.UserPages;
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
        private List<CourierModel> listOfCouriers;
        private DeliveryModel PotencialDelivery;
        private DeliveryService deliveryService;
        private AddressService addressService;
        private UserService userService;
        private LocationService locationService;
        private List<FullAddressModel> Addresses;
        private LocationModel startAddressLocation;
        private LocationModel endAddressLocation;

        public NewDeliveryDispatchPage()
        {
            locationService = new LocationService();
            userService = new UserService();
            addressService = new AddressService();
            liteHelper = new SQLiteHelper();
            deliveryService = new DeliveryService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            PotencialDelivery = new DeliveryModel() { DeliveryPrice = 160, DeliveryStatus = 1, UserID = Convert.ToInt32(Application.Current.Properties["UserID"]), DispatcherID = Convert.ToInt32(Application.Current.Properties["UserID"]), StartAddress = new FullAddressModel() { UserID = Convert.ToInt32(Application.Current.Properties["UserID"]), Address = "", Phone = "+3816", Name = "",LocationID=1 }, EndAddress = new FullAddressModel() { UserID = Convert.ToInt32(Application.Current.Properties["UserID"]), Address = "", Phone = "+3816", Name = "" ,LocationID=1} };

            InitializeComponent();
        }
        
        protected override async void OnAppearing()
        {

            if (!await DelAndPayTypes())
            {
                await DisplayAlert("Greška na serveru", "Proveri konekciju na internet. Ako je povezan javi Matiji.", "Ok");
            }
            if (!await CourierListUpdate())
            {
                await DisplayAlert("Greška na serveru.", "Nema aktivnih kurira. Javi im da se uloguju na sistem i aktiviraju lociranje. ", "Potvrdi");
            }
            if (!await RefreshAddressList())
            {
                await DisplayAlert("Greška na serveru", "Proveri konekciju na internet. Ako je povezan javi Matiji.", "Ok");
            }
            if(PotencialDelivery!=null)
            {
                
               await CheckStartLocationKey();
               await CheckEndLocationKey();
                  await CheckStartLocation();
                await CheckEndLocation();
                DeliveryGrid.BindingContext = PotencialDelivery;
                Titlelbl.Text = "Kreiranje dostave";
                DeliveryButton.Text = "Završi";

            }

            base.OnAppearing();


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
        private async Task CheckStartLocation()
        {
            if (PotencialDelivery.StartAddress != null)
            {
                if (PotencialDelivery.StartAddress.LocationID > 0)
                {
                    try
                    {
                        startAddressLocation = await locationService.GetByID(Convert.ToInt32(PotencialDelivery.StartAddress.LocationID));
                        BtnAddStartAddressLocation.Text = "Ažuriraj lokaciju";
                    }
                    catch { BtnAddStartAddressLocation.Text = "Dodaj lokaciju"; }

                }
                else { BtnAddStartAddressLocation.Text = "Dodaj lokaciju"; }
            }
            else { BtnAddStartAddressLocation.Text = "Dodaj lokaciju"; }
}
        private async Task CheckEndLocation()
        {
            if (PotencialDelivery.EndAddress != null)
            {

                if (PotencialDelivery.EndAddress.LocationID > 0)
            {
                try
                {
                    endAddressLocation = await locationService.GetByID(Convert.ToInt32(PotencialDelivery.EndAddress.LocationID));
                    BtnAddEndAddressLocation.Text = "Ažuriraj lokaciju";
                }
                catch { BtnAddEndAddressLocation.Text = "Dodaj lokaciju"; }

            }
            else { BtnAddEndAddressLocation.Text = "Dodaj lokaciju"; }
            }
            else { BtnAddEndAddressLocation.Text = "Dodaj lokaciju"; }
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
            CourierModel courier = CourierPicker.SelectedItem as CourierModel;
            
            if (DeliveryButton.Text.ToLower() == "završi" && PotencialDelivery != null)
            {
                if (await CheckDelivery(PotencialDelivery))
                {

                    try
                    {
                        PotencialDelivery.PaymentTypeID = PaymentTypePicker.SelectedIndex + 1;
                        PotencialDelivery.DeliveryTypeID = DeliverTypePicker.SelectedIndex + 1;
                        PotencialDelivery.CourierID = courier.CourierID;
                        PotencialDelivery.StartAddress.Zone = Convert.ToInt32(ZoneStart.Value);
                        PotencialDelivery.EndAddress.Zone = Convert.ToInt32(ZoneEnd.Value);
                        PotencialDelivery.CreateTime = DateTime.Now;

                       
                        if (await StartAddressCheck())
                            await DisplayAlert("Greska", "", "Potvrdi");

                        if (await EndAddressCheck())
                            await DisplayAlert("Greska", "", "Potvrdi");
                        DeliveryModel dtodel = PotencialDelivery;
                        dtodel.StartAddress = null;
                        dtodel.EndAddress = null;
                        
                        var PostDelivery = await deliveryService.CreateDelivery(dtodel);

                        if (PostDelivery != null)
                        {
                            await DisplayAlert("BaraBara", "Dostava je poslata kuriru", "Potvrdi.");
                            await Navigation.PopToRootAsync();

                        }
                        else
                            await DisplayAlert("Došlo je do greške.", "Pokušaj ponovo.", "Potvrdi");

                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Došlo je do greške.", "Pokušaj ponovo. " + Environment.NewLine + "Tekst greške : " + ex.Message, "Potvrdi");
                    }
                }
                else
                    await DisplayAlert("Došlo je do greške.", "Pokušaj ponovo.", "Potvrdi");
            }
            else
                await DisplayAlert("Došlo je do greške.", "Pokušaj ponovo.", "Potvrdi");


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
        private async Task<bool> CourierListUpdate()
        {

            var response = await userService.GetCouriers();
            if (response != null)
            {
                listOfCouriers = new List<CourierModel>(response);
                CourierPicker.ItemsSource = listOfCouriers;
                //CourierPicker.;
                CourierPicker.SelectedIndex = 0;
                return true;


            }
            else return false;
        }
        private async Task<bool> RefreshAddressList()
        {
            Addresses = new List<FullAddressModel>(await addressService.GetAllAddressesAsync());
            listStartAddres.ItemsSource = Addresses;
            listEndAddres.ItemsSource = Addresses;
            if (Addresses != null)
                return true;
            else return false;
        }
        private void NotifyNameStart(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;

            if (string.IsNullOrWhiteSpace(property.Text))
            {
                property.TextColor = Color.Red;
            }
            else
            {
                property.TextColor = Color.Black;
                if (Addresses != null)
                    listStartAddres.ItemsSource = new List<FullAddressModel>(GetAddressesByName(property.Text));
            }
        }
        private void NotifyNameEnd(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            if (string.IsNullOrWhiteSpace(property.Text))
            {
                property.TextColor = Color.Red;
            }
            else
            {
                if (Addresses != null)
                    listEndAddres.ItemsSource = new List<FullAddressModel>(GetAddressesByName(property.Text));
            }
        }
        private void NotifyAddressStart(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            
            if (!string.IsNullOrWhiteSpace(property.Text))
            {
                if (property.Text.ToCharArray().Any(char.IsDigit))
                {
                    //if (property.Text.ToCharArray().Any(x => x.ToString() == "/"))
                    property.TextColor = Color.Black;
                    if (Addresses != null)
                    {
                        listStartAddres.ItemsSource = new List<FullAddressModel>(GetAddressesByAddress(property.Text));
                    }
                    // else { property.TextColor = Color.Black;
                    //     await DisplayAlert("Sprat  ","  ", "ok"); }
                }
            }
            else
                property.TextColor = Color.Red;

        }
        private void NotifyAddressEnd(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
           if (!string.IsNullOrWhiteSpace(property.Text))
            {
                if (property.Text.ToCharArray().Any(char.IsDigit))
                {
                    property.TextColor = Color.Black;
                    if (Addresses != null)
                    {
                        listEndAddres.ItemsSource = new List<FullAddressModel>(GetAddressesByAddress(property.Text));
                    }
                }

            }
            else
                property.TextColor = Color.Red;

        }
        async void NotifyPhoneStart(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            
            if (!string.IsNullOrWhiteSpace(property.Text))
            {
                try
                {
                    if (selector(property.Text))
                    {
                        property.TextColor = Color.Black;
                        if (Addresses != null)
                        {
                            listStartAddres.ItemsSource = GetAddressesByPhone(property.Text);
                        }
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
           
            if (!string.IsNullOrWhiteSpace(property.Text))
            {
                try
                {
                    if (selector(property.Text))
                    {
                        property.TextColor = Color.Black;
                        if (Addresses != null)
                        {
                            listEndAddres.ItemsSource = GetAddressesByPhone(property.Text);

                        }
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
        private IEnumerable<FullAddressModel> GetAddressesByAddress(string searchText = null)
        {
           // Addresses = new List<FullAddressModel>(await addressService.GetAllAddressesAsync());
            if (String.IsNullOrEmpty(searchText))
                return Addresses;
            else return Addresses.Where(c => (c.Address.ToLower().StartsWith(searchText.ToLower().Trim())));

        }
        private IEnumerable<FullAddressModel> GetAddressesByPhone(string searchText = null)
        {
            //Addresses = new List<FullAddressModel>(await addressService.GetAllAddressesAsync());
            if (String.IsNullOrEmpty(searchText))
                return Addresses;
            else return Addresses.Where(c => (c.Phone.ToString().ToLower().StartsWith(searchText.ToLower().Trim())));

        }
        private IEnumerable<FullAddressModel> GetAddressesByName(string searchText = null)
        {
            //Addresses = new List<FullAddressModel>(await addressService.GetAllAddressesAsync());
            if (String.IsNullOrEmpty(searchText))
                return Addresses;
            else return Addresses.Where(c => (c.Name.ToLower().StartsWith(searchText.ToLower().Trim())));

        }
        private async Task CheckEndLocationKey()
        {
            if (Application.Current.Properties.ContainsKey("EndLocationID"))
            {
                LocationModel result = await locationService.GetByID(Convert.ToInt32(Application.Current.Properties["EndLocationID"]));
                if (result != null)
                {
                    if (PotencialDelivery.EndAddress.LocationID != result.LocationID)
                    {
                        string answer = await DisplayActionSheet("Ažuriraj lokaciju adrese primaoca", null, null, "Da", "Ne");
                        if (answer == "Da")
                        {
                            endAddressLocation = result;
                            PotencialDelivery.EndAddress.LocationID = result.LocationID;
                            Application.Current.Properties.Remove("EndLocationID");

                        }
                        else
                        {
                            Application.Current.Properties.Remove("EndLocationID");
                        }
                    }
                    else
                    {
                        Application.Current.Properties.Remove("EndLocationID");
                    }
                }
                else
                {
                    Application.Current.Properties.Remove("EndLocationID");

                }
            }

        }
        private async Task CheckStartLocationKey()
        {
            if (Application.Current.Properties.ContainsKey("StartLocationID"))
            {
                LocationModel result = await locationService.GetByID(Convert.ToInt32(Application.Current.Properties["StartLocationID"]));
                if (result != null)
                {
                    if (PotencialDelivery.StartAddress.LocationID != result.LocationID)
                    {
                        string answer = await DisplayActionSheet("Ažuriraj lokaciju adrese pošiljaoca", null, null, "Da", "Ne");
                        if (answer == "Da")
                        {
                            startAddressLocation = result;
                            PotencialDelivery.StartAddress.LocationID = result.LocationID;
                            Application.Current.Properties.Remove("StartLocationID");
                        }
                        else
                        {
                            Application.Current.Properties.Remove("StartLocationID");
                        }
                    }
                    else
                    {
                        Application.Current.Properties.Remove("StartLocationID");

                    }

                }
                else
                {
                    Application.Current.Properties.Remove("StartLocationID");

                }
            }

        }

        private void ListStartAddres_SelectedIndexChanged(object sender, EventArgs e)
        {
            var x = sender as ListView;
            
            FullAddressModel selected = (FullAddressModel)x.SelectedItem;
            if (selected != null)
            {

                    PotencialDelivery.StartAddress = selected;
                    PotencialDelivery.StartAddressID = selected.FullAddressID;
                    DeliveryGrid.BindingContext = null;
                    DeliveryGrid.BindingContext = PotencialDelivery;
                
            }
        }
        private void ListEndAddres_SelectedIndexChanged(object sender, EventArgs e)
        {

            var x = sender as ListView;
            FullAddressModel selected = (FullAddressModel)x.SelectedItem;
            if (selected != null)
            {

                    PotencialDelivery.EndAddress = selected;
                    PotencialDelivery.EndAddressID = selected.FullAddressID;
                    DeliveryGrid.BindingContext = null;
                    DeliveryGrid.BindingContext = PotencialDelivery;
               
            }
        }
       
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
        private async Task<bool> PostContextOfStartAddress()
        {
            try
            {
                PotencialDelivery.StartAddressID = 0;
                PotencialDelivery.StartAddress.FullAddressID = 0;
                PotencialDelivery.StartAddress.FullAddressID = await TypeHelp(await addressService.PostFullAddress(PotencialDelivery.StartAddress));
                PotencialDelivery.StartAddressID = PotencialDelivery.StartAddress.FullAddressID;
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Server Error" + ex.Message + ex.InnerException, "Ok");
                return false;
            }

        }

        private async Task<bool> StartAddressCheck()
        {

            if (PotencialDelivery.StartAddressID > 0 && PotencialDelivery.StartAddress != null)
            {
                FullAddressModel startAddressFromDB = await addressService.GetAddressByIDAsync(PotencialDelivery.StartAddressID);
                if (startAddressFromDB != null)
                {
                    if (PotencialDelivery.StartAddress.CompareTo(startAddressFromDB).ContainsValue(false))
                    {
                        string str = await DisplayActionSheet("Ažuriraj postojeći ili kreraj novi šablon adrese?", null, null, "Ažuriraj", "Kreiraj");
                        if (str == "Ažuriraj")
                        {   //soft delete to save all addresses as they where
                            bool put = await Delete(startAddressFromDB);

                            if (put && await PostContextOfStartAddress())
                            {
                                await DisplayAlert("BaraBara", "Uspešno ažuriran šablon adrese.", "Ok");
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }
                        else //if (str == "Kreiraj")
                        {

                            if (await PostContextOfStartAddress())
                            {

                                await DisplayAlert("BaraBara", "Uspešno kreiran šablon adrese.", "Ok");
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("BaraBara", "Uspešno izabran postojeći šablon adrese.", "Ok");
                        return true;
                    }
                }
                else
                {

                    if (await PostContextOfStartAddress())
                    {

                        await DisplayAlert("BaraBara", "Uspešno kreiran nov šablon adrese.", "Ok");
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            else
            {
                if (await PostContextOfStartAddress())
                {

                    await DisplayAlert("BaraBara", "Uspešno kreiran nov šablon adrese.", "Ok");
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
        private async Task<bool> PostContextOfEndAddress()
        {
            try
            {
                PotencialDelivery.EndAddressID = 0;
                PotencialDelivery.EndAddress.FullAddressID = 0;
                PotencialDelivery.EndAddress.FullAddressID = await TypeHelp(await addressService.PostFullAddress(PotencialDelivery.EndAddress));
                PotencialDelivery.EndAddressID = PotencialDelivery.EndAddress.FullAddressID;
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Server Error" + ex.Message + ex.InnerException, "Ok");
                return false;
            }

        }

        private async Task<bool> EndAddressCheck()
        {

            if (PotencialDelivery.EndAddressID > 0 && PotencialDelivery.EndAddress != null)
            {
                FullAddressModel endAddressFromDB = await addressService.GetAddressByIDAsync(PotencialDelivery.EndAddressID);
                if (endAddressFromDB != null)
                {
                    if (PotencialDelivery.EndAddress.CompareTo(endAddressFromDB).ContainsValue(false))
                    {
                        string str = await DisplayActionSheet("Ažuriraj postojeći ili kreraj novi šablon adrese?", null, null, "Ažuriraj", "Kreiraj");
                        if (str == "Ažuriraj")
                        {   //soft delete to save all addresses as they where
                            bool put = await Delete(endAddressFromDB);

                            if (put && await PostContextOfEndAddress())
                            {
                                await DisplayAlert("BaraBara", "Uspešno ažuriran šablon adrese.", "Ok");
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }
                        else //if (str == "Kreiraj")
                        {
                            if (await PostContextOfEndAddress())
                            {

                                await DisplayAlert("BaraBara", "Uspešno kreiran šablon adrese.", "Ok");
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("BaraBara", "Uspešno izabran postojeći šablon adrese.", "Ok");
                        return true;
                    }
                }
                else
                {
                    if (await PostContextOfEndAddress())
                    {

                        await DisplayAlert("BaraBara", "Uspešno kreiran nov šablon adrese.", "Ok");
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            else
            {
                if (await PostContextOfEndAddress())
                {

                    await DisplayAlert("BaraBara", "Uspešno kreiran nov šablon adrese.", "Ok");
                    return true;
                }
                else
                {
                    return false;
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

        private async void BtnAddEndAddressLocation_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddLocationToAddressPage(PotencialDelivery, false));
        }

        private async void BtnAddStartAddressLocation_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AddLocationToAddressPage(PotencialDelivery, true));
        }
    }
}