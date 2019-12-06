
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
                try
                {
                    var userID = Convert.ToInt32(jsonContent);
                    userNew.UserID = userID;

                    //var userSQLite = await _connection.Table<RegisterUserModel>().Where(u => u.UserID == userNew.UserID).FirstOrDefaultAsync();
                    //if (userSQLite == null)
                    //{
                    //    int rowsAdded = await _connection.InsertAsync(userNew);
                    //}
                    //else
                    //{
                    //    var responseSQLite = await _connection.UpdateAsync(userNew);
                    //}
                    return userNew.UserID.ToString();
                }
                catch { return "Registration failed, try again."; }

            }
            else return "Registration failed, try again.";
            }
        public async Task<RegisterUserModel> EditUser(RegisterUserModel userNew)
        {
            string uri = ServerLink + "EditUser/" + userNew.UserID;
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
             response = await _client.PostAsync(uri, httpContent);
            
           if (response.IsSuccessStatusCode)
            {

                var responseContent = await response.Content.ReadAsStringAsync();
                var userResponse = JsonConvert.DeserializeObject<RegisterUserModel>(responseContent);
                //var userSQLite = await _connection.Table<RegisterUserModel>().Where(u => u.UserID == userResponse.UserID).FirstOrDefaultAsync();
                //if (userSQLite == null)
                //{
                //    int rowsAdded = await _connection.InsertAsync(userResponse);
                //}
                //else
                //{
                //    var responseSQLite = await _connection.DeleteAsync(userSQLite);
                //    responseSQLite = await _connection.InsertAsync(userResponse);
                //}
                return userResponse;
            } 
            else { if (response.StatusCode==System.Net.HttpStatusCode.BadGateway) return new RegisterUserModel { Message = "Server ne radi." };
                else { return new RegisterUserModel { Message = response.Content.ToString() }; }
            }
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
        public async Task<List<RegisterUserModel>> GetAllUsers()
        {
            string uri = ServerLink + "GetUsers";

            var response = await _client.GetAsync(uri);

            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<RegisterUserModel>>(responseString);
            }
            else return null;

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
        public async Task<int> GetCountOfCouriers()
        {
            string uri = ServerLink + "GetCountByRole/4";
            var x = await _client.GetAsync(uri);
            try
            {
                if (x.IsSuccessStatusCode)
                {
                    string y = await x.Content.ReadAsStringAsync();
                    return Convert.ToInt32(y);
                }
                else return 0;
                
            }
            catch //(Exception ex)
            { return 0; }
        }
            public async Task<IEnumerable<CourierModel>> GetCouriers()
        {
            await _connection.CreateTableAsync<CourierModel>();
            //await _connection.DropTableAsync<CourierModel>();
            int x = await this.GetCountOfCouriers();
            if (await _connection.Table<CourierModel>().CountAsync() == x)
                return await _connection.Table<CourierModel>().ToListAsync();
            else
            {

                string uri = ServerLink + "GetCouriers";
                var response = await _client.GetAsync(uri);
                var responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var list = JsonConvert.DeserializeObject<IEnumerable<CourierModel>>(responseString);

                    foreach (var item in list)
                    {

                        await _connection.InsertOrReplaceAsync(item);
                        //await _connection.InsertAsync(item);
                    }
                    return list;
                }
                else return null;
            }
        }
        public async Task<bool> CheckIfCurrentUserRoleIsCourier(int userID)
        {
            string link = ServerLink + "GetCurrentUserRole/" + userID;
            var ur = await _client.GetAsync(link);
           
            if (ur.IsSuccessStatusCode)
            {
                var responseString = await ur.Content.ReadAsStringAsync();
                UserRoleModel userRole =  JsonConvert.DeserializeObject<UserRoleModel>(responseString);
                if (userRole.RoleID == 4)
                    return true;
                else return false;
            }

            else { return false; }
        }
    }
}
