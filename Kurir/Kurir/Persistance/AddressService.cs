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
    public class AddressService
    {
        private HttpClient _client = App.client;
        private SQLiteAsyncConnection _connection;
        private string ServerLink;
        public AddressService()
        {
            ServerLink = Application.Current.Properties["ServerLink"].ToString() + "api/Addresses";
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }
        public async Task<FullAddressModel> GetAddressByIDAsync(int ID)
        {
            var uri = ServerLink + "/" + ID;
            var res = await _client.GetAsync(uri);
            if (res.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var resString = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<FullAddressModel>(resString);

            }
            else return null;
        }
        public async Task<IEnumerable<FullAddressModel>> GetAddressByUserIDAsync(int UserID)
        {
            var uri = ServerLink + "/GetByUserID/" + UserID;
            var res = await _client.GetAsync(uri);
            switch (res.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var s = await res.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<FullAddressModel>>(s);
                
                default:
                    return null;

            }
        }
        public async Task<IEnumerable<FullAddressModel>> GetAllAddressesAsync()
        {
            try
            {
                var uri = ServerLink;
                var res = await _client.GetAsync(uri);
                var s = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<FullAddressModel>>(s);
            }
            catch { return null; }
        }
        public async Task<object> PostFullAddress( FullAddressModel address)
        {
            var uri = ServerLink;
            var jsonObject = JsonConvert.SerializeObject(address);
            HttpContent httpContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            var res = await _client.PostAsync(uri, httpContent);
            switch (res.StatusCode)
            {
                
                case System.Net.HttpStatusCode.Created:
                    
                     var s=await res.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<FullAddressModel>(s);
                case System.Net.HttpStatusCode.BadRequest:
                    return "Address editing ecountered server error.";
                case System.Net.HttpStatusCode.NoContent:
                    return "Address editing ecountered server error.";
                case System.Net.HttpStatusCode.InternalServerError:
                    return "Address editing ecountered server error.";
                default:
                    return res.StatusCode.ToString();

            }
        }
        public async Task<string> PutFullAddress(int ID,FullAddressModel address)
        {
            var uri = ServerLink + "/" + ID;
            var jsonObject = JsonConvert.SerializeObject(address);
            HttpContent httpContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
      
            var res = await _client.PutAsync(uri, httpContent);
            switch (res.StatusCode)
            {
                case System.Net.HttpStatusCode.NotFound:
                    return "Address you are trying to edit does not exists.";
                case System.Net.HttpStatusCode.OK:
                    return "Address edited successfully.";
                case System.Net.HttpStatusCode.BadRequest:
                    return "Address editing ecountered server error.";
                case System.Net.HttpStatusCode.NoContent:
                    return "Address editing ecountered server error.";
                case System.Net.HttpStatusCode.InternalServerError:
                    return "Address editing ecountered server error.";
                default:
                    return res.StatusCode.ToString();
                    
            }
            
        }
        public async Task<string> DeleteAddress(int ID)
        {
            var uri = ServerLink + "/" + ID;
            var res = await _client.DeleteAsync(uri);

            return  await res.Content.ReadAsStringAsync();

        }
    }
}
