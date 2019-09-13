
using Kurir.Models;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kurir.Persistance
{
    public class UserService
    {
        private HttpClient _client = App.client;
        private SQLiteAsyncConnection _connection;
        private string ServerLink;
        public UserService()
        {
            if (!Application.Current.Properties.ContainsKey("ServerLink"))
            {
                Application.Current.Properties.Add("ServerLink", "https://kurirserver.conveyor.cloud/");
            }
            ServerLink = Application.Current.Properties["ServerLink"].ToString()+"api/Users/";
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }
        public async Task<string> Register(RegisterUserModel userNew)
        {
            string uri = ServerLink + "Register";
            string jsonUser = JsonConvert.SerializeObject(userNew);
            HttpContent httpContent = new StringContent(jsonUser, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await _client.PostAsync(uri, httpContent);
            if (msg.IsSuccessStatusCode)
            {
                string jsonContent = await msg.Content.ReadAsStringAsync();
                var userID =Convert.ToInt32(jsonContent);
                if (userID > 0)
                    userNew.UserID = userID;
                var userSQLite = await _connection.Table<RegisterUserModel>().Where(u => u.UserID == userNew.UserID).FirstOrDefaultAsync();
                if (userSQLite == null)
                {
                    int rowsAdded = await _connection.InsertAsync(userNew);
                }
                else
                {
                    var responseSQLite = await _connection.UpdateAsync(userNew);
                }
                return userNew.UserID.ToString();

            }
            else return "Registration failed, try again.";
            }
        public async Task<RegisterUserModel> EditUser(RegisterUserModel userNew)
        {
            string uri = ServerLink + "EditUser/" + Application.Current.Properties["UserID"].ToString();
            string jsonUser = JsonConvert.SerializeObject(userNew);
            HttpContent httpContent = new StringContent(jsonUser, Encoding.UTF8, "application/json");
            HttpResponseMessage msg;
            try { msg = await _client.PutAsync(uri, httpContent); }
            catch (Exception ex)
            {
                userNew.Message = ex.Message;
                return userNew;
            }
            if (msg.IsSuccessStatusCode)
            {

                var userSQLite = await _connection.Table<RegisterUserModel>().Where(u => u.UserID == userNew.UserID).FirstOrDefaultAsync();
                if (userSQLite == null)
                {
                    int rowsAdded = await _connection.InsertAsync(userNew);

                }
                else
                {
                    var responseSQLite = await _connection.UpdateAsync(userNew);

                }
                userNew.Message = "";
                return userNew;
            }
            else
            {
                userNew.Message = "Acccount edit failed. Try again.";
                return userNew;
            }
        }
        public async Task<RegisterUserModel> Login(LoginUserModel newUser)
        {
            string uri = ServerLink + "login/";

            string jsonUser = JsonConvert.SerializeObject(newUser);
            HttpContent httpContent = new StringContent(jsonUser, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            try { response = await _client.PostAsync(uri, httpContent); }
            catch (Exception ex) { return new RegisterUserModel { Message = "Login failed, try again." + ex.Message }; }
            if (response != null && response.IsSuccessStatusCode)
            {

                var responseContent = await response.Content.ReadAsStringAsync();
                var userResponse = JsonConvert.DeserializeObject<RegisterUserModel>(responseContent);
                var userSQLite = await _connection.Table<RegisterUserModel>().Where(u => u.UserID == userResponse.UserID).FirstOrDefaultAsync();
                if (userSQLite == null)
                {
                    int rowsAdded = await _connection.InsertAsync(userResponse);
                }
                else
                {
                    var responseSQLite = await _connection.UpdateAsync(userResponse);
                }
                return userResponse;
            }
            else {return new RegisterUserModel { Message = "Login failed, try again."}; }
         }
        public async Task<bool> LogOut(int userID)
        {
            string uri = ServerLink + "Logout/" + userID;
            var response = await _client.GetAsync(uri);

            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Boolean>(responseString);
            }
            else return false;
        }
        public async Task<bool> GetUserByEmail(string mail)
        {
            string uri = ServerLink + "GetUserByEmail/" + mail;

            var response = await _client.GetAsync(uri);

            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Boolean>(responseString);
            }
            else return false;

        }
        //GetUser full name by ID
        public async Task<string> GetUserByID(int id)
        {
            string uri = ServerLink + "GetUser/" + id;

            var response = await _client.GetAsync(uri);

            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<string>(responseString);
            }
            else return "Unknown User";

        }
        public async Task<ActiveCourierModel> GetCourierModelByID(int id)
        {
            string uri = ServerLink + "GetCourierModel/" + id;

            var response = await _client.GetAsync(uri);

            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<ActiveCourierModel>(responseString);
            }
            else return new ActiveCourierModel() { CourierFullName = "Unknown User", CourierID = 0, Lat = 0, Long = 0 };

        }
        public async Task<IEnumerable<ActiveCourierModel>> GetActiveCouriers()
        {
            string uri = ServerLink + "GetActiveCouriers/";

            var response = await _client.GetAsync(uri);

            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ActiveCourierModel>>(responseString);
            }
            else return null;
        }
        public static string PopUp()
        {
            return "Prosao poziv metode :)";
        }
    }
}
