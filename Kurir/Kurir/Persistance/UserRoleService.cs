using Kurir.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kurir.Persistance
{
   public class UserRoleService
    {
        private HttpClient _client = App.client;
        private string ServerLink;

        public UserRoleService()
        {
            ServerLink = Application.Current.Properties["ServerLink"].ToString() + "api/UserRoles";
        }
        public async  Task<bool> AddNewUserRole(UserRoleModel userRole)
        {
            var uri = ServerLink + "/Add";
            var jsonObject = JsonConvert.SerializeObject(userRole);
            HttpContent httpContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            var res = await _client.PostAsync(uri, httpContent);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else return false;
        }
        public async Task<bool> Delete(int id)
        {
            var uri = ServerLink + "/Delete/"+id;
            var res = await _client.DeleteAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else return false;
        }
        //Get
        public async Task<UserRoleModel> Get(int id)
        {
            var uri = ServerLink + "/Get/" + id;
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string resString =await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserRoleModel>(resString);
            }
            else return null;
        }
    }
}
