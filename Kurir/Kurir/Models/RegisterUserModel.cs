using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Kurir.Models
{
    public class RegisterUserModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        [PrimaryKey]
        public int UserID { get; set; }
        private string firstName { get; set; }
        private string lastName { get; set; }
        private string phone { get; set; }
        private string mail { get; set; }
        private string pass { get; set; }
        private string passConfirm { get; set; }
        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                if (firstName == value || string.IsNullOrWhiteSpace(value))
                    return;
                else
                {
                    firstName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string LastName
        {
            get
            {
                return lastName;
            }
            set {
                if (lastName == value || string.IsNullOrWhiteSpace(value))
                    return;
                else
                {
                    lastName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Phone
        {
            get
            {
                return phone;
            }
            set
            {
                if (phone == value || string.IsNullOrWhiteSpace(value))
                    return;
                else
                {
                    phone = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Mail {
            get
            {
                return mail;
            }
            set
            {
                if (mail == value || string.IsNullOrWhiteSpace(value))
                    return;
                else
                {
                    mail = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Pass
        {
            get
            {
                return pass;
            }
            set
            {
                if (pass == value || string.IsNullOrWhiteSpace(value))
                    return;
                else
                {
                    pass = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PassConfirm
        {
            get
            {
                return passConfirm;
            }
            set
            {
                if (passConfirm == value || string.IsNullOrWhiteSpace(value))
                    return;
                else
                {
                    passConfirm = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsActive { get; set; }

        public DateTime RegistrationDate { get; set; }
       
        public bool Valid { get; set; }
        public string Message { get; set; }

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));//if null do nothing, else invoke method
        }
       
    }
}
