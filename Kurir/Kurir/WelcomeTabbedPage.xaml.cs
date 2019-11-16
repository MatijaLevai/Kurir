using Kurir.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Mail;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Kurir.Persistance;
using SQLite;
using Kurir.UserPages;

namespace Kurir
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomeTabbedPage : TabbedPage
    {
        private HttpClient _client = App.client;
        private SQLiteAsyncConnection _connection;
        private string link;
        private UserService userService;
        private UserRoleService userRoleService;
        public WelcomeTabbedPage()
        {
            NavigationPage.SetHasBackButton(this, false);
            userRoleService = new UserRoleService();
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
                            Application.Current.Properties["Mail"] = userNew.Mail;
                            Application.Current.Properties["UserID"] = userNewID;
                            Application.Current.Properties["Pass"] = userNew.Pass;
                            Application.Current.Properties["Name"] = userNew.FirstName;
                            if (!Application.Current.Properties.ContainsKey("User"))
                                Application.Current.Properties.Add("User", JsonConvert.SerializeObject(userNew));
                            else Application.Current.Properties["User"] = JsonConvert.SerializeObject(userNew);
                           //await DisplayAlert("UserDetailsStored:", "App.Properties[Mail] = "+Application.Current.Properties["Mail"]+ "\r\n" + "Application.Current.Properties[UserID] =" + Application.Current.Properties["UserID"]+ "\r\n" + "Application.Current.Properties[Pass] = " +Application.Current.Properties["Pass"]+ "\r\n" + "Application.Current.Properties[Name] = " + Application.Current.Properties["Name"], "proceed");



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
                    if (!Application.Current.Properties.ContainsKey("User"))
                        Application.Current.Properties.Add("User", JsonConvert.SerializeObject(userResponse));
                    else Application.Current.Properties["User"] = JsonConvert.SerializeObject(userResponse);
                    await Application.Current.SavePropertiesAsync();
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
            if (Application.Current.Properties.ContainsKey("ServerLink")) Application.Current.Properties["ServerLink"] = "https://kurirserver.conveyor.cloud/";
            else Application.Current.Properties.Add("ServerLink", "https://kurirserver.conveyor.cloud/");

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                BackgroundColor = Color.FromHex("#f5f5f5");
                BarTextColor = Color.FromHex("#f5f5f5");
                break;
            }
            await _connection.CreateTableAsync<LocationModel>();
            await _connection.CreateTableAsync<RegisterUserModel>();
            base.OnAppearing();
            #region Vi[ak
            //if (Application.Current.Properties.ContainsKey("User"))
            //try
            //{
            //    RegisterUserModel user = JsonConvert.DeserializeObject<RegisterUserModel>(Application.Current.Properties["User"].ToString());
            //    UserRoleModel ur = await userRoleService.Get(user.ActiveUserRoleID);
            //    switch (ur.RoleID)
            //    {
            //        case 1:
            //            Application.Current.MainPage = new NavigationPage(new DefaultSuperAdminPage());
            //            break;
            //        case 2:
            //            Application.Current.MainPage = new NavigationPage(new UsersListPage());
            //            break;
            //        case 3:
            //            Application.Current.MainPage = new NavigationPage(new UserHomePage());
            //            break;
            //        case 4:
            //            Application.Current.MainPage = new NavigationPage(new DefaultCouriersPage());
            //            break;
            //        case 5:
            //            Application.Current.MainPage = new NavigationPage(new DispatcherHomePage());
            //            break;
            //    }



            //}
            //catch (Exception ex) { await DisplayAlert("Greska",ex.Message+ex.InnerException,"ok"); }
            //if (Application.Current.Properties.ContainsKey("Mail") && Application.Current.Properties.ContainsKey("Pass"))
            //{
            //    if (Application.Current.Properties["Mail"] != null && Application.Current.Properties["Pass"] != null)
            //    {
            //        String[] parts = Application.Current.Properties["Mail"].ToString().Split(new[] { '@' });
            //        String username = parts[0];
            //        var loginResponse = await DisplayActionSheet("Welcome back " + username, null, "different user", "Click here to Login");
            //        if (loginResponse == "Click here to Login")
            //        {

            //            try
            //            {
            //                var user = new LoginUserModel { Pass = Application.Current.Properties["Pass"].ToString(), Mail = Application.Current.Properties["Mail"].ToString() };

            //                var userResponse = await userService.Login(user);
            //                if (string.IsNullOrWhiteSpace(userResponse.Message))
            //                {
            //                    Application.Current.Properties["Mail"] = userResponse.Mail;
            //                    Application.Current.Properties["UserID"] = userResponse.UserID;
            //                    Application.Current.Properties["Pass"] = userResponse.Pass;
            //                    Application.Current.Properties["Name"] = userResponse.FirstName;
            //                    if (!Application.Current.Properties.ContainsKey("User"))
            //                    Application.Current.Properties.Add("User", userResponse);
            //                    else Application.Current.Properties["User"] = JsonConvert.SerializeObject(userResponse);
            //                    await Application.Current.SavePropertiesAsync();
            //                    Application.Current.MainPage = new UserRolePage(userResponse);
            //                   // await Navigation.PushAsync();
            //                }
            //                else
            //                {
            //                    await DisplayAlert("Eror",userResponse.Message+". Please try again.", "ok.");
            //                }

            //            }
            //            catch (Exception ex)
            //            {

            //                Debug.WriteLine(ex.Message);
            //                await DisplayAlert("Error", ex.Message, "ok");
            //                return;
            //            }

            //        }
            //        else if (loginResponse == "different user")
            //        {
            //            Application.Current.Properties["Mail"] = null;
            //            Application.Current.Properties["UserID"] = null;
            //            Application.Current.Properties["Pass"] = null;
            //            Application.Current.Properties["Name"] = null;
            //            Application.Current.Properties.Clear();
            //            await Application.Current.SavePropertiesAsync();
            //        }

            //    }
            //}
            //string settings = "";
            //foreach (var item in Application.Current.Properties)
            //{
            //    //switch (item.Key)

            //    settings += item.Key + item.Value.ToString()+Environment.NewLine;
            //}
            //await DisplayAlert("Sacuvani parametri: ",settings,"Ok");
            //
            #endregion
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
            //return base.OnBackButtonPressed();
        }
    }
           
}