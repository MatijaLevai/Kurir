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
        private HttpClient _client = App.client;
        private SQLiteAsyncConnection _connection;
        private string link;
        private UserService userService;

        public WelcomeTabbedPage()
        {
            NavigationPage.SetHasBackButton(this, false);
           
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            userService = new UserService();
             InitializeComponent();
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
                    var responseBool =await userService.GetUserByEmail(userNew.Mail);
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

                       var userNewID = await userService.Register(userNew);
                        if (userNewID != null)
                        {
                            Application.Current.Properties["Mail"] =   userNew.Mail;
                            Application.Current.Properties["UserID"] = userNewID;
                            Application.Current.Properties["Pass"] =   userNew.Pass;
                            Application.Current.Properties["Name"] =   userNew.FirstName;
                            await DisplayAlert("UserDetailsStored:", "App.Properties[Mail] = "+Application.Current.Properties["Mail"]+ "\r\n" + "Application.Current.Properties[UserID] =" + Application.Current.Properties["UserID"]+ "\r\n" + "Application.Current.Properties[Pass] = " +Application.Current.Properties["Pass"]+ "\r\n" + "Application.Current.Properties[Name] = " + Application.Current.Properties["Name"], "proceed");
                                                                     


                            await Navigation.PushAsync(new UserHomePage());
                        }
                        else
                            await DisplayAlert("Atention!", "Server Connection problem. Try again please.", "ok?");

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
                var userResponse =await userService.Login(user);
                if (string.IsNullOrWhiteSpace(userResponse.Message))
                {
                    Application.Current.Properties["Mail"] = userResponse.Mail;
                    Application.Current.Properties["UserID"] = userResponse.UserID;
                    Application.Current.Properties["Pass"] = userResponse.Pass;
                    Application.Current.Properties["Name"] = userResponse.FirstName;

                    await Navigation.PushAsync(new UserRolePage(userResponse));
                }
                else
                {
                    await DisplayAlert("Eror", "response contains" + userResponse.Message, "try again.");
                }
                
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
                case Device.Android:
                    BackgroundColor = Color.FromHex("#f5f5f5");
                    BarTextColor = Color.FromHex("#f5f5f5");
                    link = Application.Current.Properties["ServerLink"].ToString();
                    break;
                default:
                    link = Application.Current.Properties["ServerLink"].ToString();
                    break;
            }
            await _connection.CreateTableAsync<LocationModel>();
            await _connection.CreateTableAsync<RegisterUserModel>();

            if (Application.Current.Properties.ContainsKey("Mail") && Application.Current.Properties.ContainsKey("Pass"))
            {
                if (Application.Current.Properties["Mail"] != null && Application.Current.Properties["Pass"] != null)
                {
                    String[] parts = Application.Current.Properties["Mail"].ToString().Split(new[] { '@' });
                    String username = parts[0];
                    var loginResponse = await DisplayActionSheet("Welcome back " + username, null, "different user", "Click here to Login");
                    if (loginResponse == "Click here to Login")
                    {

                        try
                        {
                            var user = new LoginUserModel { Pass = Application.Current.Properties["Pass"].ToString(), Mail = Application.Current.Properties["Mail"].ToString() };

                            var userResponse = await userService.Login(user);
                            if (string.IsNullOrWhiteSpace(userResponse.Message))
                            {
                                Application.Current.Properties["Mail"] = userResponse.Mail;
                                Application.Current.Properties["UserID"] = userResponse.UserID;
                                Application.Current.Properties["Pass"] = userResponse.Pass;
                                Application.Current.Properties["Name"] = userResponse.FirstName;

                                await Navigation.PushAsync(new UserRolePage(userResponse));
                            }
                            else
                            {
                                await DisplayAlert("Eror",userResponse.Message+". Please try again.", "ok.");
                            }

                        }
                        catch (Exception ex)
                        {

                            Debug.WriteLine(ex.Message);
                            await DisplayAlert("Error", ex.Message, "ok");
                            return;
                        }

                    }
                    else if (loginResponse == "different user")
                    {
                        Application.Current.Properties["Mail"] = null;
                        Application.Current.Properties["UserID"] = null;
                        Application.Current.Properties["Pass"] = null;

                        Application.Current.Properties["Name"] = null;
                    }

                }
            }
            base.OnAppearing();
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
            //return base.OnBackButtonPressed();
        }
    }
           
}