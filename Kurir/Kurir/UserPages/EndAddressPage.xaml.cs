using Kurir.Models;
using Kurir.Persistance;
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
    public partial class EndAddressPage : ContentPage
    {
        private LocationService locationService;
        private AddressService addressService;
        private List<FullAddressModel> userAddresses;
        private List<FullAddressModel> userSearchAddresses;
        private DeliveryModel delivery;
        private bool EditEnabled;
        private bool Name;
        private bool Address;
        private bool Phone;
        private LocationModel EndAddressLocation;
        public EndAddressPage(DeliveryModel d,bool EditEnabled)
        {
            locationService = new LocationService();
            if (d != null)
            {
                this.EditEnabled = EditEnabled;
                delivery = d;
                //delivery.StartAddress = d.StartAddress;//?
                if (delivery.EndAddress == null)
                {
                    delivery.EndAddress = new FullAddressModel() { UserID = Convert.ToInt32(Application.Current.Properties["UserID"]), Name = "", Phone = "+3816", Address = "" };
                }
                
                InitializeComponent();
            }
            
        }
        protected override async void OnAppearing()
        {
           
            addressService = new AddressService();
            GetUserAddresses();
            if(delivery.EndAddressID>0)
            delivery.EndAddress = await addressService.GetAddressByIDAsync(delivery.EndAddressID);
            CheckLocation();
            CheckLocationKey();
            this.EndAddressGrid.BindingContext = delivery.EndAddress;
            base.OnAppearing();
        }
        private async void CheckLocation()
        {
            if (delivery.EndAddress.LocationID > 0)
            {
                try
                {
                    EndAddressLocation = await locationService.GetByID(Convert.ToInt32(delivery.EndAddress.LocationID));
                    EndAddressLocationIDButton.Text = "Edit Location";
                }
                catch { EndAddressLocationIDButton.Text = "Add Location"; }

            }
            else { EndAddressLocationIDButton.Text = "Add Location"; }
           
        }
        private async void CheckLocationKey()
        {
            if (Application.Current.Properties.ContainsKey("EndLocationID"))
            {
                LocationModel result = await locationService.GetByID(Convert.ToInt32(Application.Current.Properties["EndLocationID"]));
                if (result != null)
                {
                    if (delivery.EndAddress.LocationID != result.LocationID)
                    {
                        string answer = await DisplayActionSheet("Update location of recivers address?", null, null, "Yes", "No");
                        if (answer == "Yes")
                        {
                            EndAddressLocation = result;
                            delivery.EndAddress.LocationID = result.LocationID;
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
        public async void GetUserAddresses()
        {
            try
            {
                var response = await addressService.GetAddressesByUserIDAsync(Convert.ToInt32(Application.Current.Properties["UserID"]));
                if (response != null)
                    userAddresses = new List<FullAddressModel>(response);
                var c = userAddresses.Where(a => a.FullAddressID == delivery.StartAddressID).FirstOrDefault();
                if (c != null)
                {
                    userAddresses.Remove(c);
                }
                AddressListView.ItemsSource = userAddresses;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error updating list of addresses", ex.Message + ex.InnerException, "ok");
                Debug.WriteLine(ex.Message + ex.InnerException);
            }
        }
        async void NotifyAddress(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {

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
                        Address = false;
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
       
        async void NotifyName(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var property = sender as Entry;
            if (!string.IsNullOrWhiteSpace(property.Text))
            {
                try
                {

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
            }
            else
            {
                property.TextColor = Color.Red;
                Name = false;
            }

            CheckEnteries();
        }
        private async Task<bool> PostContextOfAddress()
        {
            try
            {
                delivery.EndAddressID = 0;
                delivery.EndAddress.FullAddressID = 0;
                delivery.EndAddress.FullAddressID = await TypeHelp(await addressService.PostFullAddress(delivery.EndAddress));
                delivery.EndAddressID = delivery.EndAddress.FullAddressID;
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Server Error" + ex.Message + ex.InnerException, "Ok");
                return false;
            }

        }
        private async Task<bool> Delete(FullAddressModel endAddressFromDB)
        {
            try
            {
                string m = await addressService.DeleteAddress(endAddressFromDB.FullAddressID);
                //await DisplayAlert("Server Message", m, "Ok");
                switch (m)
                {
                    case "Address with given parametters not found.":
                        return false;
                    case "address successesfully deleted.":
                        return true;
                    default:
                        return false;
                }
                

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Server Error" + ex.Message + ex.InnerException, "Ok");
                return false;
            }

        }
        private async void NextButton_Clicked(object sender, EventArgs e)
        {
            bool done = false;
            try
            {
                if (delivery.EndAddressID > 0 && delivery.EndAddress != null)
                {
                    FullAddressModel endAddressFromDB = await addressService.GetAddressByIDAsync(delivery.EndAddressID);
                    if (endAddressFromDB != null)
                    {
                        if (delivery.EndAddress.CompareTo(endAddressFromDB).ContainsValue(false))
                        {


                            string str = await DisplayActionSheet("Update or Create new address template?", "Cancel", null, "Update", "Create");

                            if (str == "Update")
                            {
                                //soft delete to save all addresses as they where
                                bool put = await Delete(endAddressFromDB);
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
                                    done = true;
                                    await DisplayAlert("Success", "Created new template for address.", "Ok");
                                }
                                else { done = false; }

                            }
                        }
                        else
                        {
                            done = true;
                            await DisplayAlert("Done", "Selected existing template for recivers address of delivery.", "Ok");
                        }


                    }
                    else if (delivery.EndAddress != null)
                    {
                        if (await PostContextOfAddress())
                        {
                            done = true;
                            await DisplayAlert("Success", "Created new template for address.", "Ok");
                        }
                        else { done = false; }
                      
                    }
                    else
                    {
                        done = false;
                        await DisplayAlert("Error","Recivers address is not in correct format",":(");
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
                { await Navigation.PushAsync(new NewDelivery(delivery, EditEnabled)); }

                

                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message + ex.InnerException,"ok");
                Debug.WriteLine(ex.Message + ex.InnerException);
            }
        }
        private bool CheckEnteries()
        {
            if (Name && Address && Phone)
            {
                IsEnabledNextButton(true);
                return true;
            }
            else
            {
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
       
        private void AddressListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (delivery.EndAddress == null)
            { delivery.EndAddress = new FullAddressModel(); }
            delivery.EndAddress = (FullAddressModel) e.SelectedItem;
            delivery.EndAddressID = delivery.EndAddress.FullAddressID;
            EndAddressGrid.BindingContext = delivery.EndAddress;
            CheckLocation();
        }

        private void AddressListView_Refreshing(object sender, EventArgs e)
        {
          GetUserAddresses();
          AddressListView.EndRefresh();
        }

        private async void EndAddressLocationIDButton_Clicked(object sender, EventArgs e)
        {
          await  Navigation.PushModalAsync(new AddLocationToAddressPage(delivery, false));
        }

        private void SearchBar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SearchBar bar = (SearchBar)sender;
            if (String.IsNullOrEmpty(bar.Text))
            {
                
            
                AddressListView.ItemsSource = userAddresses;
               
            }
            else {
                try
                {
                    string searchText = bar.Text.Trim().ToLower();
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
                catch {
                    AddressListView.ItemsSource = userAddresses;
                   
                }
            }


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
                await DisplayAlert("FSailed.", " Try again. Check internet connection. Error : " + ex.Message + ex.InnerException, "ok");
            }

        }
    }
}

