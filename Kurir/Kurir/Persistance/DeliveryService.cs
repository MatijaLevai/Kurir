using Kurir.Models;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        //public class Response
        //{
        //    public bool Success { get; set; }
        //    public string Message { get; set; }
        //}
        public async Task<ResponseModel> DeleteDelivery(int i)
        {
            string uri = ServerLink + "/Delete/"+i;
            HttpResponseMessage msg = await _client.DeleteAsync(uri);
            ResponseModel response = new ResponseModel()
            {
                Success = msg.IsSuccessStatusCode,
                Message = await msg.Content.ReadAsStringAsync()
            };
                return response;
        }
        public async Task<DeliveryModel> GetDeliveryByID(int id)

        {
            try
            {
                var uri = ServerLink + "/GetDelivery/"+id;
                var res = await _client.GetAsync(uri);
                if (res.StatusCode == System.Net.HttpStatusCode.OK && res.Content != null)
                {
                    var resStringD = await res.Content.ReadAsStringAsync();
                    DeliveryModel delivery = JsonConvert.DeserializeObject<DeliveryModel>(resStringD);
                    
                    return delivery;
                }
                else return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
                return null;
            }
        }
        public async Task<IEnumerable<DeliveryModel>> GetCofirmedForCourier()
        {
            var uri = ServerLink + "/odataget?$expand=StartAddress,EndAddress&$filter=courierid eq " + Application.Current.Properties["UserID"] + " and deliverystatus eq 2";

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
        
             public async Task<IEnumerable<DeliveryModel>> GetGetTodaysDeliveriesForCourier(int id=0)
        {
            string uri;
            if (id == 0)
            {
                uri = ServerLink + "/GetTodaysDeliveriesForCourier/" + Application.Current.Properties["UserID"]+ "&$orderby = Createtime desc"; 
                //uri = ServerLink + "/odataget?$expand=StartAddress,EndAddress&$filter=courierid eq " + Application.Current.Properties["UserID"] + " and deliverystatus eq 2&$orderby = Createtime desc";
            }
            else
            {
                // DateTime dtNow = DateTime.Now;
                // DateTime dt = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day,1,0,0);
                uri = ServerLink + "/GetTodaysDeliveriesForCourier/" + id;
                    //"odataget?$filter= courierid eq" +id+ "and CreateTime ge" + dt;     
                //uri = ServerLink + "/odataget?$expand=StartAddress,EndAddress&$filter=courierid eq " + id + " and deliverystatus eq 2";

            }
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
        public async Task<IEnumerable<DeliveryModel>> GetGetLastWeekDeliveriesForCourier(int id = 0)
        {
            string uri;
            if (id == 0)
            {
                uri = ServerLink + "/odataget?$expand=StartAddress,EndAddress&$filter=courierid eq " + Application.Current.Properties["UserID"] + " and deliverystatus eq 2&$orderby = Createtime desc";
            }
            else
            {
                // DateTime dtNow = DateTime.Now;
                // DateTime dt = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day,1,0,0);
                uri = ServerLink + "/GetTodaysDeliveriesForCourier/" + id;
                //"odataget?$filter= courierid eq" +id+ "and CreateTime ge" + dt;     
                //uri = ServerLink + "/odataget?$expand=StartAddress,EndAddress&$filter=courierid eq " + id + " and deliverystatus eq 2";

            }
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
        public async Task<IEnumerable<DeliveryModel>> GetDeliveriesForCourierDesc(int CourierID=0)
        {
            string uri = ServerLink ;
            if (CourierID == 0)
            {  uri += "/odataget?$expand=StartAddress,EndAddress&$filter=courierid eq " + Application.Current.Properties["UserID"] + " &$orderby=Createtime desc"; }

            else {  uri += "/odataget?$expand=StartAddress,EndAddress&$filter=CourierID eq " + CourierID+ " &$orderby=Createtime desc"; }
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
        public async Task<IEnumerable<DeliveryModel>> GetStartedDeliveriesForCourier()
        {
            var uri = ServerLink + "/odataget?$expand=StartAddress,EndAddress&$filter=courierid eq " + Application.Current.Properties["UserID"]+ " and deliverystatus eq 3";

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
        public async Task<IEnumerable<DeliveryModel>> GetDeliveriesForUser(int userID)
        {
            var uri = ServerLink + "/GetDeliveriesForUser/" + userID;
           // var uri = ServerLink + "/odataget?$expand=filter=userid eq " +userID;

            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resString = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);

            }
            else return null;
        }
        public async Task<IEnumerable<ExtendedDeliveryModel>> GetDeliveriesForDispatcher()
        {
            var uri = ServerLink + "/GetDeliveriesForDispatcher/" + Application.Current.Properties["UserID"].ToString();

            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resString = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<ExtendedDeliveryModel>>(resString);

            }
            else return null;
        }
        public async Task<IEnumerable<DeliveryModel>> GetAllDeliveries()
        {
            var uri = ServerLink + "/odataget?$expand=StartAddress,EndAddress";
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK && res.Content!=null)
            {
                var resString = await res.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);

            }
            else return null;
        }
        public async Task<IEnumerable<DeliveryModel>> GetAllFinishedDeliveries(DateTime d1,DateTime d2)
        {
            var uri = ServerLink + "/odataget?$expand=StartAddress,EndAddress&$filter=DeliveryStatus eq 4 and createtime ge " + d1.Date.ToString("yyyy-MM-dd") + " and createtime le " + d2.Date.ToString("yyyy-MM-dd");
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK && res.Content != null)
            {
                var resString = await res.Content.ReadAsStringAsync();

                var list = JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);
                return list;
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
            var uri = ServerLink + "/odataget?$expand=StartAddress,EndAddress&$filter=courierid eq " + Application.Current.Properties["UserID"] + " and deliverystatus eq 1";
                
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
        
        public async Task<IEnumerable<DeliveryModel>> GetDeliveriesWithCourierDispatchUserID(int UserID)
        {
            var uri = ServerLink + "/odataget?$expand=StartAddress,EndAddress&$filter=courierid eq " + UserID + "or dispatcherid eq " + UserID+ "or userid eq " + UserID;

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
        public async Task<IEnumerable<DeliveryModel>> GetDeliveriesWithCourierOrDispatchByDATE(int UserID,DateTime d1, DateTime d2)
        {
            string d11 = d1.Ticks.ToString();
            string d22 = d2.Ticks.ToString();
            var uri = ServerLink + "/GetDeliveriesByDateCourierDispatch/"+UserID+"/"+d11+"/"+d22;

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
        public async Task<IEnumerable<DeliveryModel>> GetDeliveriesByDate(DateTime d1, DateTime d2)
        {
            string d11 = d1.Ticks.ToString();
            string d22 = d2.Ticks.ToString();
            var uri = ServerLink + "/GetByDate/" + d11 + "/" + d22;
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resString = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);

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
        public async Task<IEnumerable<DeliveryModel>> GetDeliveriesByDateAndUser(DateTime d1, DateTime d2,int id)
        {
            string d11 = d1.Ticks.ToString();
            string d22 = d2.Ticks.ToString();
            var uri = ServerLink + "/GetByDateAndUser/"+ id +"/"+ d11 + "/" + d22;
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resString = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);

            }
            else return null;
        }
        public async Task<IEnumerable<DeliveryModel>> GetDeliveriesByDateAndCourtier(DateTime d1, DateTime d2, int id)
        {
            string d11 = d1.Ticks.ToString();
            string d22 = d2.Ticks.ToString();
            var uri = ServerLink + "/GetByDateAndCourier/" + id + "/" + d11 + "/" + d22;
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resString = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);

            }
            else return null;
        }
        public async Task<IEnumerable<DeliveryModel>> GetDeliveriesByDateAndDispatcher(DateTime d1, DateTime d2, int id)
        {
            string d11 = d1.Ticks.ToString();
            string d22 = d2.Ticks.ToString();
            var uri = ServerLink + "/GetByDateAndDispatcher/" + id + "/" + d11 + "/" + d22;
            var res = await _client.GetAsync(uri);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resString = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<DeliveryModel>>(resString);

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
