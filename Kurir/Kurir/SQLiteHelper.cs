using Kurir.Models;
using Kurir.Persistance;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kurir
{
    public class SQLiteHelper: ISQLiteHelper
    {
        private HttpClient _client = new HttpClient();
        private readonly SQLiteAsyncConnection _connection;

        public SQLiteHelper()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        private async Task<bool> TryUpdateOrInsert(IEnumerable<Object> list)
        {
            int x=0;
            int y = 0;
            foreach (var item in list)
            {
                y++;
                if (await _connection.UpdateAsync(item) == 0)
                    if (await _connection.InsertAsync(item) == 1)
                    { x++; }
                else x++;
                

            }
            if (x == y)
                return true;
            else return false;


        }
        public async Task<bool> UpdateSQLiteDb()
        {
            string uriPaymentTypes = Application.Current.Properties["ServerLink"].ToString() + "api/PaymentTypes/GetPaymentTypes";
            string uriDeliveryTypes = Application.Current.Properties["ServerLink"].ToString() + "api/DeliveryTypes/GetDeliveryTypes";
            var responsePaymentTypes = await _client.GetAsync(uriPaymentTypes);
            var responseDeliveryTypes = await _client.GetAsync(uriDeliveryTypes);
            if (responsePaymentTypes.StatusCode == System.Net.HttpStatusCode.OK && responseDeliveryTypes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseContentPaymentTypes = await responsePaymentTypes.Content.ReadAsStringAsync();
                var responseContentDeliveryTypes = await responseDeliveryTypes.Content.ReadAsStringAsync();
               var paymentTypes = JsonConvert.DeserializeObject<List<PaymentTypeModel>>(responseContentPaymentTypes);
               var deliveryTypes = JsonConvert.DeserializeObject<List<DeliveryTypeModel>>(responseContentDeliveryTypes);
               await _connection.CreateTableAsync<PaymentTypeModel>();
               await _connection.CreateTableAsync<DeliveryTypeModel>();
               await TryUpdateOrInsert(paymentTypes);
               await TryUpdateOrInsert(deliveryTypes);



                return true;
            }
            else
            {
                return false;
                //await DisplayAlert("Internet", "Check Your connection settings", "Ok");
            }
        }
    }
}
