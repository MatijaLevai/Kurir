using Kurir.Models;
using Kurir.Persistance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.AdminPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserDetailPage : ContentPage
    {
        private readonly Color colorRed = new Color(255, 69, 0);
        private Color colorGreen = new Color(50, 205, 50);
        private RegisterUserModel user;
        private readonly UserService userService;
        private readonly DeliveryService deliveryService;
        private List<DeliveryModel> listOfDeliveries;
        public UserDetailPage(RegisterUserModel user)
        {
            this.user = user;
            userService = new UserService();
            deliveryService = new DeliveryService();
            InitializeComponent();
        }
        private async Task<bool> GetDeliveries()
        {

            var list = await deliveryService.GetDeliveriesWithCourierDispatchUserID(user.UserID);
            if (list != null)
            {
                listOfDeliveries = new List<DeliveryModel>(list);
                //await _connection.DropTableAsync<DeliveryModel>();
                // await _connection.CreateTableAsync<DeliveryModel>();

                foreach (var item in listOfDeliveries)
                {
                    //Delivery detail image 
                    switch (item.DeliveryStatus)
                    {
                        case 4:
                            item.DeliveryStatusImageSource = "delivered.png";
                            break;
                        case 3:
                            item.DeliveryStatusImageSource = "zeleni50.png";
                            break;
                        case 2:
                            item.DeliveryStatusImageSource = "zuti50.png";
                            break;
                        case 1:
                            item.DeliveryStatusImageSource = "crveni50.png";
                            break;
                        default:
                            item.DeliveryStatusImageSource = "crveni50.png";
                            break;
                    }
                }

                DeliveryList.ItemsSource = listOfDeliveries;
                return true;
            }
            else
            {
                Message.Text = "Kurir nema dostava.";
                return false;
            }
        }
        protected override async void OnAppearing()
        {
            if (user != null)
            {
                UserDetailGrid.BindingContext = user;
                await GetDeliveries();
# region Sample Binding in Code////
                //FirstName.SetBinding(Entry.TextProperty, new Binding() { Source = BindingContext, Path = "FirstName" });
#endregion
            }
            base.OnAppearing();
        }

        private async void Edit_Clicked(object sender, EventArgs e)
        {
            
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
                //POSEBNO cemo menjati sifru
                //< Entry Grid.Row = "4" Grid.Column = "1" x: Name = "Pass" IsPassword = "true" MaxLength = "30" Text = "{Binding Pass}" ></ Entry >
            
                //                < Label Grid.Row = "5" Grid.Column = "0" Text = "Confirm password:" />
                 
                //                     < Entry Grid.Row = "5" Grid.Column = "1" x: Name = "PassConfirm" IsPassword = "true" MaxLength = "30" Text = "{Binding PassConfirm}" ></ Entry >

                ////if (string.IsNullOrWhiteSpace(Pass.Text.ToLower().Trim()))
                ////{
                //    Pass.BackgroundColor = colorRed;
                //    numberOfError++;
                //}
                //else
                //{
                //    Pass.BackgroundColor = colorGreen;
                //}
                //if (string.IsNullOrWhiteSpace(PassConfirm.Text.ToLower().Trim()))
                //{
                //    PassConfirm.BackgroundColor = colorRed;
                //    numberOfError++;
                //}
                //else
                //{
                //    PassConfirm.BackgroundColor = colorGreen;
                //}
                if (string.IsNullOrWhiteSpace(Percent.Text.ToLower().Trim()))
                {
                    Percent.BackgroundColor = colorRed;
                    numberOfError++;
                }
                else
                {
                    Percent.BackgroundColor = colorGreen;
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
                        Pass = user.Pass,
                        PassConfirm = user.Pass,
                        Procenat = Convert.ToInt32(Percent.Text),
                        Valid = true,
                        UserID = user.UserID,
                        RegistrationDate = user.RegistrationDate
                    };
                    if (userNew.Mail != user.Mail)
                    {
                        userNew.Valid = (await userService.GetUserByEmail(userNew.Mail));

                    }
                    if (userNew.Valid)
                    {
                        
                        //{
                        //    userNew.Valid = false;
                        //    userNew.Message = "Password does not match confirmPassword";
                        //}

                        //else
                        //{
                        //    userNew.Valid = true;

                        //}

                    }
                    else
                    {
                        userNew.Valid = false;
                        userNew.Message += "Email address already in use.Please login to continue.";
                    }

                    if (userNew.Valid)
                    {

                        userNew = await userService.EditUser(userNew);
                        if (string.IsNullOrWhiteSpace(userNew.Message))
                        {
                            userNew.Message = "Edit is successful";
                            user = userNew;
                            UserDetailGrid.BindingContext = null;
                            UserDetailGrid.BindingContext = user;
                            UserDetailGrid.BackgroundColor = Color.DarkGray;
                            FirstName.BackgroundColor = Color.DarkGray;
                            LastName.BackgroundColor = Color.DarkGray;
                            Phone.BackgroundColor = Color.DarkGray;
                            Mail.BackgroundColor = Color.DarkGray;
                            Percent.BackgroundColor = Color.DarkGray;
                           
                        }
                        await DisplayAlert(" ", userNew.Message.ToString(), "ok");


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

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new CreateRoleForUserPage(user));
        }
    }
}