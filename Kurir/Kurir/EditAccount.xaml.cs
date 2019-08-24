using Kurir.Models;
using Kurir.Persistance;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAccount : ContentPage
    {
        private HttpClient _client = new HttpClient();
        private SQLiteAsyncConnection _connection;
        public EditAccount()
        {
            InitializeComponent();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
           
        }
        protected override async void OnAppearing()
        {
            string mail = Application.Current.Properties["Mail"].ToString();
            RegisterUserModel user =await  _connection.Table<RegisterUserModel>().Where(u=>u.Mail == mail).FirstAsync();
            if (user != null)
            {
                this.BindingContext = user;
                FirstName.SetBinding(Entry.TextProperty, new Binding() { Source = BindingContext, Path = "FirstName" });
                LastName.SetBinding(Entry.TextProperty, new Binding() { Source = BindingContext, Path = "LastName" });
                Phone.SetBinding(Entry.TextProperty, new Binding() { Source = BindingContext, Path = "Phone" });
                Mail.SetBinding(Entry.TextProperty, new Binding() { Source = BindingContext, Path = "Mail" });
                Pass.SetBinding(Entry.TextProperty, new Binding() { Source = BindingContext, Path = "Pass" });
                PassConfirm.SetBinding(Entry.TextProperty, new Binding() { Source = BindingContext, Path = "PassConfirm" });

            }
            base.OnAppearing();
        }

        private async void  Edit_Clicked(object sender, EventArgs e)
        {
            var colorRed = new Color();
            var colorGreen = new Color();
            colorRed = Color.FromRgb(255, 69, 0);
            colorGreen = Color.FromRgb(50, 205, 50);
            int numberOfError = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(FirstName.Text.ToLower().Trim()))
                {
                    FirstName.BackgroundColor = colorRed;
                    numberOfError++;
                }
                else { FirstName.BackgroundColor = colorGreen; }
                if (string.IsNullOrWhiteSpace(LastName.Text.ToLower().Trim()))
                {
                    LastName.BackgroundColor = colorRed;
                    numberOfError++;
                }
                else { LastName.BackgroundColor = colorGreen; }
                if (string.IsNullOrWhiteSpace(Phone.Text.ToLower().Trim()))
                {
                    Phone.BackgroundColor = colorRed;
                    numberOfError++;
                }
                else { Phone.BackgroundColor = colorGreen; }
                if (string.IsNullOrWhiteSpace(Mail.Text.ToLower().Trim()))
                {
                    Mail.BackgroundColor = colorRed;
                    numberOfError++;
                }
                else
                {
                    Mail.BackgroundColor = colorGreen;

                }
                if (string.IsNullOrWhiteSpace(Pass.Text.ToLower().Trim()))
                {
                    Pass.BackgroundColor = colorRed;
                    numberOfError++;
                }
                else
                {
                    Pass.BackgroundColor = colorGreen;
                }
                if (string.IsNullOrWhiteSpace(PassConfirm.Text.ToLower().Trim()))
                {
                    PassConfirm.BackgroundColor = colorRed;
                    numberOfError++;
                }
                else
                {
                    PassConfirm.BackgroundColor = colorGreen;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await DisplayAlert("Error", "Please fill all fields.", "ok");
                numberOfError++;
            }


            if (numberOfError == 0)
            {
                try
                {
                    MailAddress email = new MailAddress(Mail.Text);

                    var userNew = new RegisterUserModel()
                    {
                        FirstName = FirstName.Text.ToLower().Trim(),
                        LastName = LastName.Text.ToLower().Trim(),
                        Phone = Phone.Text,
                        Mail = email.Address,
                        Pass = Pass.Text.Trim(),
                        PassConfirm = PassConfirm.Text.Trim(),
                        Valid = true

                    };
                    if (userNew.Mail != Application.Current.Properties["Mail"].ToString())
                    {
                        string uri = Application.Current.Properties["ServerLink"].ToString()+"api/users/GetUserByEmail/" + userNew.Mail;

                        var response = await _client.GetAsync(uri);

                        var responseString = await response.Content.ReadAsStringAsync();
                        var responseBool = JsonConvert.DeserializeObject<Boolean>(responseString);
                        userNew.Valid = responseBool;

                    }
                    if (userNew.Valid)
                    {
                        if (userNew.Pass != userNew.PassConfirm)
                        {
                            userNew.Valid = false;
                            userNew.Message = "Password does not match confirmPassword";
                        }

                        else
                        {
                            userNew.Valid = true;
                            userNew.Message = "Edit is successful";
                        }

                    }
                    else
                    {
                        userNew.Valid = false;
                        userNew.Message += "Email address already in use.Please login to continue.";
                    }

                    if (userNew.Valid)
                    {

                        string uri = Application.Current.Properties["ServerLink"].ToString()+"api/users/EditUser/"+ Application.Current.Properties["UserID"].ToString();
                        string jsonUser = JsonConvert.SerializeObject(userNew);
                        HttpContent httpContent = new StringContent(jsonUser, Encoding.UTF8, "application/json");
                        HttpResponseMessage msg = await _client.PutAsync(uri, httpContent);
                        if (msg.IsSuccessStatusCode)
                        {
                            await DisplayAlert("Succses", "Acccount edit is successful", "Ok.");
                            var userSQLite = await _connection.Table<RegisterUserModel>().Where(u => u.UserID == userNew.UserID).FirstOrDefaultAsync();
                            if (userSQLite == null)
                            {
                                int rowsAdded = await _connection.InsertAsync(userNew);
                                await DisplayAlert("SQLITE", "Insert into table register model done", "OK");
                            }
                            else
                            {
                                var responseSQLite = await _connection.UpdateAsync(userNew);
                                await DisplayAlert("SQLITE", "Updated table register model done", "OK");
                            }
                            Application.Current.Properties["Mail"] = userNew.Mail;

                        }
                    }
                    else await DisplayAlert("Atention!", userNew.Message.ToString(), "ok?");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("ERROR", "MESSAGE : " + ex.Message + "INNEREXCEPTION : " + ex.InnerException, "Ok.");
                }
            }
            else { await DisplayAlert("Error", "Please fill all fields.", "ok"); }

        }
    }
}