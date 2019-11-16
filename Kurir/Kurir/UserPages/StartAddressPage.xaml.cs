using Kurir.Models;
using Kurir.Persistance;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.UserPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartAddressPage : ContentPage
    {
        private readonly AddressService addressService;
        private List<FullAddressModel> userAddresses;
        private List<FullAddressModel> userSearchAddresses;
        LocationService locationService;
        private DeliveryModel delivery;
        private bool EditEnabled;
        private bool Name;
        private bool Address;
        private bool Phone;
        private LocationModel StartAddressLocation;
       // private readonly SQLiteAsyncConnection _connection;

        public StartAddressPage()
        {
            // _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            locationService = new LocationService();
            addressService = new AddressService();
            GetUserAddresses();
            EditEnabled = false;
            delivery = new DeliveryModel()
            {
                DeliveryPrice = 160,
                DeliveryID = 0,
                StartAddressID = 0,
                EndAddress = null,
                EndAddressID = 0,
                CreateTime = DateTime.Now,
                StartTime=Convert.ToDateTime("0001 - 01 - 01 00:00:00.0000000"),
                EndTime = Convert.ToDateTime("0001 - 01 - 01 00:00:00.0000000"),
                PaymentTypeID=1,
                DeliveryTypeID=1,
                Description="",
                WaitingInMinutes=0,
                DeliveryStatusImageSource="zuti50.png",
                CourierID=0,
                DispatcherID=0,
                UserID = Convert.ToInt32(Application.Current.Properties["UserID"]),
                StartAddress = new FullAddressModel() { UserID = Convert.ToInt32(Application.Current.Properties["UserID"]),Address="",Phone="+3816",Name="" },
                DeliveryStatus = 0
            };
            InitializeComponent();
            
        }
        
        protected override async void OnAppearing()
        {
            try
            {
                if (delivery.StartAddressID > 0&& delivery.StartAddress==null)
                {
                    delivery.StartAddress = await addressService.GetAddressByIDAsync(delivery.StartAddressID);
                }

               CheckLocation();
               CheckLocationKey();
            
               if( StartAddressGrid.BindingContext != delivery.StartAddress)
                    StartAddressGrid.BindingContext = delivery.StartAddress;
                base.OnAppearing();
            }
            catch (Exception ex)
            {
                await DisplayAlert("exception type " + ex.GetType().Name, ex.Message + "||" + ex.InnerException, "ok");
            }
        }
        private async void CheckLocation()
        {
            if (delivery.StartAddress.LocationID > 0)
            {
                try
                {
                    StartAddressLocation = await locationService.GetByID(Convert.ToInt32(delivery.StartAddress.LocationID));
                    StartAddressLocationIDButton.Text = "Edit Location";
                }
                catch { StartAddressLocationIDButton.Text = "Add Location"; }

            }
            else { StartAddressLocationIDButton.Text = "Add Location"; }
        }
        private async void CheckLocationKey()
        {
            if (Application.Current.Properties.ContainsKey("StartLocationID"))
            {
                LocationModel result = await locationService.GetByID(Convert.ToInt32(Application.Current.Properties["StartLocationID"]));
                if (result != null)
                {
                    if (delivery.StartAddress.LocationID != result.LocationID)
                    {
                        string answer = await DisplayActionSheet("Change location of senders address?", null, null, "Yes", "No");
                        if (answer == "Yes")
                        {
                            StartAddressLocation = result;
                            delivery.StartAddress.LocationID = result.LocationID;
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

        public StartAddressPage(DeliveryModel delivery)
        {
            locationService = new LocationService(); 
            //_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            if (delivery != null)
            {
                this.delivery = delivery;
                EditEnabled = true;

            }
            else
            {
                EditEnabled = false;
                delivery = new DeliveryModel()
                {
                    DeliveryPrice = 160,
                    DeliveryID = 0,
                    StartAddressID = 0,
                    EndAddress = null,
                    EndAddressID = 0,
                    CreateTime = DateTime.Now,
                    StartTime = Convert.ToDateTime("0001 - 01 - 01 00:00:00.0000000"),
                    EndTime = Convert.ToDateTime("0001 - 01 - 01 00:00:00.0000000"),
                    PaymentTypeID = 1,
                    DeliveryTypeID = 1,
                    Description = "",
                    WaitingInMinutes = 0,
                    DeliveryStatusImageSource = "zuti50.png",
                    CourierID = 0,
                    DispatcherID = 0,
                    UserID = Convert.ToInt32(Application.Current.Properties["UserID"]),
                    DeliveryStatus = 0,
                    StartAddress = new FullAddressModel() { UserID = Convert.ToInt32(Application.Current.Properties["UserID"]) }
                    
                };
            }
            addressService = new AddressService();
            GetUserAddresses();
            InitializeComponent();

        }
        public async void GetUserAddresses()
        {
            try
            {
                var response = await addressService.GetAddressesByUserIDAsync(Convert.ToInt32(Application.Current.Properties["UserID"]));
                if(response!=null)
                userAddresses = new List<FullAddressModel>(response);
                AddressListView.ItemsSource = userAddresses;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error updating list of addresses",ex.Message+ex.InnerException,"ok");
                Debug.WriteLine(ex.Message + ex.InnerException);
            }
        }

        async void NotifyAddress(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try {

                var property = sender as Entry;
                if (!string.IsNullOrWhiteSpace(property.Text))
                {

                    if ((property.Text.ToCharArray().Any(char.IsDigit)))
                    {
                        property.TextColor = Color.Black;
                        Address = true;
                    }
                    else
                    {
                        property.TextColor = Color.Red;
                        Address=false;
                    }
                }
                else
                {
                    property.TextColor = Color.Red;
                    Address = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error on " + e.PropertyName, ex.Message + "||" + ex.InnerException, "ok");
                Address = false;
            }

            CheckEnteries();
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
                       Phone = true;
                    }
                    else
                    {
                        property.TextColor = Color.Red;
                        Phone = false;
                    }

                }
                catch (Exception ex)
                {
                    Phone = false;
                    await DisplayAlert("Error on " + e.PropertyName, ex.Message + "||" + ex.InnerException, "ok");
                }


            }
            else
            {
                property.TextColor = Color.Red;
                Phone = false;
            }

            CheckEnteries();
        }

        private async Task<bool> PostContextOfAddress()
        {
            try {
                delivery.StartAddressID = 0;
                delivery.StartAddress.FullAddressID = 0;
                delivery.StartAddress.FullAddressID = await TypeHelp(await addressService.PostFullAddress(delivery.StartAddress));
                delivery.StartAddressID = delivery.StartAddress.FullAddressID;
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error","Server Error"+ex.Message+ex.InnerException,"Ok");
                return false;
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
            catch (Exception ex)
            {
                //await DisplayAlert("Error", "Server Error" + ex.Message + ex.InnerException, "Ok");
                return false;
            }

        }

        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            bool done = false;
            try
            {
                if (delivery.StartAddressID > 0 && delivery.StartAddress != null)
                {
                    FullAddressModel startAddressFromDB = await addressService.GetAddressByIDAsync(delivery.StartAddressID);
                    if (startAddressFromDB != null)
                    {
                        if (delivery.StartAddress.CompareTo(startAddressFromDB).ContainsValue(false))
                        {
                            string str = await DisplayActionSheet("Update or Create new address template?","Cancel",null,"Update", "Create");
                            if (str == "Update")
                            {   //soft delete to save all addresses as they where
                                bool put = await Delete(startAddressFromDB);
                                bool post = await PostContextOfAddress();
                                if (put && post)
                                {
                                    await DisplayAlert("Success", "Updated existing template for address.", "Ok");
                                    done = true;
                                }
                                else
                                {
                                    done = false;
                                }

                            }
                            else if (str == "Create")
                            {
                                if (await PostContextOfAddress())
                                {
                                    
                                    await DisplayAlert("Success", "Created new template for address.", "Ok");
                                    done = true;
                                }
                                else
                                {
                                    done = false;
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Done", "Selected existing template for senders address of delivery.", "Ok");
                            done = true;
                        }
                    }
                    else
                    {
                        if (await PostContextOfAddress())
                        {
                            done = true;
                            await DisplayAlert("Success", "Created new template for address.", "Ok");
                        }
                        else { done = false; }
                        
                    }
                }
                else
                {
                    if (await PostContextOfAddress())
                    {
                        done = true;
                        await DisplayAlert("Success", "Created new template for address.", "Ok");
                    }
                    else { done = false; }
                    
                }

                if (done)
                { await Navigation.PushAsync(new EndAddressPage(delivery, EditEnabled)); }


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message+ex.InnerException);
               await DisplayAlert("Error",ex.Message+ex.InnerException,"ok");
            }
        }

        async void NotifyName(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            if (!string.IsNullOrWhiteSpace(property.Text))
           {
                try {
                    
                    if (property.Text.ToArray().Count() < 3)
                    {
                        property.TextColor = Color.Red;
                        Name = false;
                    }
                    else
                    {
                        property.TextColor = Color.Black;
                        Name = true;
                    }
                }
                catch (Exception ex)
                {
                    Name = false;
                    Debug.WriteLine(ex.Message + ex.InnerException);
                    await DisplayAlert("Error on " + e.PropertyName, ex.Message + ex.InnerException, "ok");
                }
            }else
            {
                property.TextColor = Color.Red;
                Name = false;
            }

            CheckEnteries();
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

        private void SearchBar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                SearchBar bar = (SearchBar)sender;
            if (bar.Text != null)
            {

                string searchText = bar.Text.Trim().ToLower();
                if (String.IsNullOrEmpty(searchText))
                {
                    AddressListView.ItemsSource = userAddresses;

                }

                else
                {
                    
                        userSearchAddresses = new List<FullAddressModel>(userAddresses.Where(c => ((c.Address.ToLower().StartsWith(searchText)) || (c.Name.ToLower().StartsWith(searchText)) || (c.Phone.StartsWith(searchText)))));
                        if (userSearchAddresses.Count > 0)
                        {
                            AddressListView.ItemsSource = userSearchAddresses;
                        }
                        else
                        {
                            AddressListView.ItemsSource = userAddresses;
                            bar.Placeholder = "Search by name, address or phone";

                        }
                    
                }
            }
            }
            catch
            {
                AddressListView.ItemsSource = userAddresses;

            }
        }
        //private async Task<bool> CheckStartAddress(FullAddressModel startAddress)
        //{
        //    if (startAddress.FullAddressID > 0)
        //    {
        //      var address=  await addressService.GetAddressByIDAsync(startAddress.FullAddressID);
        //        if (await TypeHelp(address) > 0)
        //        {
        //            if ((address.Name == startAddress.Name)&& (address.Address == startAddress.Address)&&(address.Phone == startAddress.Phone))
        //            {
        //              return true;
        //            }
        //            else return false; 
        //        }
        //        else return false;
        //    }
        //    else return false;
        //}

        readonly Func<string, bool> selector = delegate (string c)
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
        private bool CheckEnteries()
        {
            if (Name && Address && Phone)
            {
                IsEnabledNextButton(true);
                return true;
            }
            else {
                IsEnabledNextButton(false);
                return false;
            }
        }
        private void IsEnabledNextButton(bool isEnabled)
        {
            if (!isEnabled)
            {
                NextBtn.IsVisible = false;
                NextBtn.IsEnabled = false;
            }
            else
            {
                NextBtn.IsVisible = true;
                NextBtn.IsEnabled = true;
            }
        }

        private async void AddressListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            delivery.StartAddress = (FullAddressModel) e.SelectedItem;
            await DisplayAlert("Selected template","Name : "+delivery.StartAddress.Name+Environment.NewLine+"Address : "+delivery.StartAddress.Address + Environment.NewLine+" Phone : " +delivery.StartAddress.Phone,"ok");
            delivery.StartAddressID = delivery.StartAddress.FullAddressID;
            StartAddressGrid.BindingContext = delivery.StartAddress;
            CheckLocation();

        }

        private void AddressListView_Refreshing(object sender, EventArgs e)
        {
            GetUserAddresses();
            AddressListView.EndRefresh();
        }

        private async void StartAddressLocationIDButton_Clicked(object sender, EventArgs e)
        {
           await Navigation.PushModalAsync(new AddLocationToAddressPage(delivery,true));
        }

        public async void DeleteCommand(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);

            try
            {
                if (int.TryParse(mi.CommandParameter.ToString(), out int IDint))
                {
                    FullAddressModel selectedAddress = userAddresses.Where(x => x.FullAddressID == IDint).First();
                    if (selectedAddress != null)
                    {
                       
                        if (await Delete(selectedAddress))
                        {
                            userAddresses.Remove(selectedAddress);
                            AddressListView.ItemsSource = null;
                            AddressListView.ItemsSource = userAddresses;
                            await DisplayAlert("Succses", "Address removed.", "ok");
                        }
                        else
                        {
                            await DisplayAlert("Failed.", " Try again. Check internet connection.", "ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Failed.", " Try again. Check internet connection.", "ok");
                    }
                }
                else
                {
                    await DisplayAlert("Failed.", " Try again. Check internet connection.", "ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Failed.", " Try again. Check internet connection. Error : " + ex.Message + ex.InnerException, "ok");
            }

        }
    }
}