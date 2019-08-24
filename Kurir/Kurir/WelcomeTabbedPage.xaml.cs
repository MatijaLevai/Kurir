using Kurir.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Kurir.Persistance;
using SQLite;

namespace Kurir
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomeTabbedPage : TabbedPage
    {
        private HttpClient _client = new HttpClient();
        private SQLiteAsyncConnection _connection;
        private string link;

        public WelcomeTabbedPage()
        {
            InitializeComponent();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
           

        }



        private async void  Register_Clicked(object sender, EventArgs e)
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
                        PassConfirm = PassConfirm.Text.Trim()

                    };

                    string uri = link+"api/users/GetUserByEmail/" + userNew.Mail;

                    var response = await _client.GetAsync(uri);

                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseBool = JsonConvert.DeserializeObject<Boolean>(responseString);
                    if (responseBool)
                    {
                        if (userNew.Pass != userNew.PassConfirm)
                        {
                            userNew.Valid = false;
                            userNew.Message = "Password does not match confirmPassword";
                        }

                        else
                        {
                            userNew.Valid = true;
                            userNew.Message = "Registration successful";
                        }

                    }
                    else
                    {
                        userNew.Valid = false;
                        userNew.Message = "Email address already in use.Please login to continue.";
                    }

                    if (userNew.Valid)
                    {
                       
                        uri = link + "api/users/Register";
                        string jsonUser = JsonConvert.SerializeObject(userNew);
                        HttpContent httpContent = new StringContent(jsonUser, Encoding.UTF8, "application/json");
                        HttpResponseMessage msg = await _client.PostAsync(uri, httpContent);
                        if (msg.IsSuccessStatusCode)
                        {
                            await DisplayAlert("Registration successful", "Welcome to Eko Kurir App", "Ok.");
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
                            Application.Current.Properties["UserID"] = userNew.UserID;
                            Application.Current.Properties["Pass"] = userNew.Pass;
                            await Navigation.PushAsync(new UserHomePage());

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

        private async void Login_Clicked(object sender, EventArgs e)
        {
            try
            {
                var user = new LoginUserModel { Pass = LoginPass.Text, Mail = LoginMail.Text };
                string uri = link+"api/users/login";
              
                string jsonUser = JsonConvert.SerializeObject(user);
                //Debug.WriteLine("user Serlijalizovan u json");
                HttpContent httpContent = new StringContent(jsonUser, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {//Debug.WriteLine("poslat upit serveru");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //Debug.WriteLine("primljen odgovor od servera");
                    var userResponse = JsonConvert.DeserializeObject<RegisterUserModel>(responseContent);
                    // Debug.WriteLine("deserijalizovvan objekat od servera");
                    // await DisplayAlert("Login","Uspesno ste se ulogovali!","Ok");
                    // Debug.WriteLine("dodeljena textualna vrednost labeli");
                    var userSQLite = await _connection.Table<RegisterUserModel>().Where(u => u.UserID == userResponse.UserID).FirstOrDefaultAsync();
                    if (userSQLite == null)
                    {
                        int rowsAdded = await _connection.InsertAsync(userResponse);
                        await DisplayAlert("SQLITE", "Insert into table register model done", "OK");
                    }
                    else
                    {
                        var responseSQLite = await _connection.UpdateAsync(userResponse);
                        await DisplayAlert("SQLITE", "Updated table register model done", "OK");
                    }
                    Application.Current.Properties["Mail"] = userResponse.Mail;
                    Application.Current.Properties["UserID"] = userResponse.UserID;
                    Application.Current.Properties["Pass"] = userResponse.Pass;

                    //await DisplayAlert("app current", "added mail in  Application.Current.Properties[Mail]", "OK");
                    await Navigation.PushAsync(new UserRolePage(userResponse));
                    return;
                }
                else await DisplayAlert("Error", "Check your internet connection and try again","Ok");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await DisplayAlert("Error",ex.Message,"ok");
                return;
            }
        }

        protected override async void OnAppearing()
        {
            switch (Device.RuntimePlatform)
            {
                //case Device.iOS:
                //    link = Application.Current.Properties["ServerLink"].ToString();
                //    break;
                //case Device.Android:
                //    link = Application.Current.Properties["ServerLink"].ToString();
                //    break;
                //case Device.UWP:
                default:
                    link = Application.Current.Properties["ServerLink"].ToString();
                    break;
            }
            //await DisplayAlert("start", Application.Current.Properties["Mail"]+"|_____|"+link,"ok");
            await _connection.CreateTableAsync<LocationModel>();
            await _connection.CreateTableAsync<RegisterUserModel>();

            if (Application.Current.Properties.ContainsKey("Mail") && Application.Current.Properties.ContainsKey("Pass"))
            {

                //    var loginResponse = await DisplayActionSheet("One-Click Login",null, "different user", "Click here to Login");
                //    if (loginResponse == "login")
                // 
                if (Application.Current.Properties["Mail"] != null&& Application.Current.Properties["Pass"] != null)
                    try
                {
                    var user = new LoginUserModel { Pass = Application.Current.Properties["Pass"].ToString(), Mail = Application.Current.Properties["Mail"].ToString() };
                    string uri = link + "api/users/login";
                    string jsonUser = JsonConvert.SerializeObject(user);
                    HttpContent httpContent = new StringContent(jsonUser, Encoding.UTF8, "application/json");
                    var response = await _client.PostAsync(uri, httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        //Debug.WriteLine("poslat upit serveru");
                        var responseContent = await response.Content.ReadAsStringAsync();
                        //Debug.WriteLine("primljen odgovor od servera");
                        var userResponse = JsonConvert.DeserializeObject<RegisterUserModel>(responseContent);
                        Application.Current.Properties["UserID"] = userResponse.UserID;
                        await DisplayAlert("Welcome back", "login successful", "ok");
                        await Navigation.PushAsync(new UserRolePage(userResponse));
                    }
                    //}
                    //else if (loginResponse == "different user")
                    //{
                    //    await DisplayAlert("Different user?", "login or register please", "ok");
                    //    Application.Current.Properties["Mail"] = null;
                    //    Application.Current.Properties["UserID"] = null;
                    //    Application.Current.Properties["Pass"] = null;
                    //}
                }
                catch (Exception ex)
                {
                    await DisplayAlert("error",ex.Message+ex.Source+ex.StackTrace,"ok");
                }

                }
            base.OnAppearing();
        }
    }
           
}