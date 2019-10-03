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
    public class DeliveryService
    {
        private HttpClient _client = App.client;
        private SQLiteAsyncConnection _connection;
        private string ServerLink;
        public DeliveryService()
        {
            ServerLink = Application.Current.Properties["ServerLink"].ToString() + "api/Deliveries";
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }
        public async Task<DeliveryModel> CreateDelivery(DeliveryModel newDelivery)
        {
            var jsonObject = JsonConvert.SerializeObject(newDelivery);
            HttpContent httpContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            var uri = ServerLink+ "/NewDelivery";
            var response = _client.PostAsync(uri, httpContent);
            if (response.Result.IsSuccessStatusCode)
            {

                var responseContent = await response.Result.Content.ReadAsStringAsync();
               
                var delivery = JsonConvert.DeserializeObject<DeliveryModel>(responseContent);
                return delivery;
            }
            else return null;
        }
        public async Task<DeliveryModel> EditDelivery(DeliveryModel newDelivery)
        {
            string uri = ServerLink + "/EditDelivery";
            string jsonD = JsonConvert.SerializeObject(newDelivery);
            HttpContent httpContent = new StringContent(jsonD, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await _client.PutAsync(uri, httpContent);
            if (msg.IsSuccessStatusCode)
            {
                var stringD = await msg.Content.ReadAsStringAsync();
                var d = JsonConvert.DeserializeObject<DeliveryModel>(stringD);
                return d;
            }
            else return null;

        }

        public async Task<IEnumerable<DeliveryModel>> GetCofirmedForCourier()
        {
            var uri = ServerLink + "/odataget?$filter=courierid eq " + Application.Current.Properties["UserID"] + " and deliverystatus eq 2";

            var res = await _client.GetAsync(uri);

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (res.Content != null)
                {
                    var resString = await res.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);
                }
                else return null;

            }
            else return null;
        }

        public async Task<IEnumerable<DeliveryModel>> GetDeliveriesForUser()
        {
            var uri = ServerLink + "/GetDeliveriesForUser/" + Application.Current.Properties["UserID"].ToString();
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resString = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);

            }
            else return null;
        }
        public async Task<IEnumerable<DeliveryModel>> GetDeliveriesForDispatcher()
        {
            var uri = ServerLink + "/GetDeliveriesForDispatcher/" + Application.Current.Properties["UserID"].ToString();
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resString = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);

            }
            else return null;
        }
        public async Task<IEnumerable<DeliveryModel>> GetAllDeliveries()
        {
            var uri = ServerLink + "/ODataGet/";
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK && res.Content!=null)
            {
                var resString = await res.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);

            }
            else return null;
        }
        public async Task<IEnumerable<DeliveryModel>> GetUncofirmedDeliveriesForDispatcher()
        {
            var uri = ServerLink + "/GetUncofirmedDeliveriesForDispatcher/";
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resString = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);

            }
            else return null;
        }
        public async Task<IEnumerable<DeliveryModel>> GetUncofirmedForCourier()
        {
            var uri = ServerLink + "/odataget?$filter=courierid eq " + Application.Current.Properties["UserID"] + " and deliverystatus eq 1";
                
            var res = await _client.GetAsync(uri);
           
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (res.Content != null)
                {
                    var resString = await res.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);
                }
                else return null;
                
            }
            else return null;
        }
        public async Task<IEnumerable<StatisticUserCandDModel>> GetStatisticsOfCouriers(DateTime d1, DateTime d2)
        {
            string d11 = d1.Ticks.ToString();
            string d22 = d2.Ticks.ToString();
            var uri = ServerLink + "/StatisticsCouriers/" + d11+"/"+d22;
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resString = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<StatisticUserCandDModel>>(resString);

            }
            else return null;
        }
        public async Task<IEnumerable<StatisticUserCandDModel>> GetStatisticsOfDispatchers(DateTime d1, DateTime d2)
        {
            string d11 = d1.Ticks.ToString();
            string d22 = d2.Ticks.ToString();
            var uri = ServerLink + "/StatisticsDispatchers/" + d11 + "/" + d22;
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resString = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<StatisticUserCandDModel>>(resString);

            }
            else return null;
        }
    }
}
