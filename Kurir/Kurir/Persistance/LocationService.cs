using Kurir.Models;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace Kurir.Persistance
{
    public class LocationService
    {

        private HttpClient _client = App.client;
        private SQLiteAsyncConnection _connection;
        private string ServerLink;
        public LocationService()
        {
            if (!Application.Current.Properties.ContainsKey("ServerLink"))
            {
                Application.Current.Properties.Add("ServerLink", "https://kurirserver.conveyor.cloud/");
            }
            ServerLink = Application.Current.Properties["ServerLink"].ToString() + "api/Locations/";
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }
        public async Task<LocationModel> GetByID(int id)
        {
            // GetLocationByID /
                try
            {
                    string uri = ServerLink + "GetLocationByID/"+id;
                    var response = await _client.GetAsync(uri);

                    var responseString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<LocationModel>(responseString);
                    }
                    else return null;
               
            }
            catch (Exception ex) { Console.WriteLine(ex.Message + ex.InnerException); return null; }

        }
        public async Task<LocationModel> AddLocation(LocationModel l)
        {
            try
            {
                if (l != null)
                {
                    string uri = ServerLink + "AddLocation/";
                    string jsonLocation = JsonConvert.SerializeObject(l);
                    HttpContent httpContent = new StringContent(jsonLocation, Encoding.UTF8, "application/json");
                    var response = await _client.PostAsync(uri, httpContent);

                    var responseString = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<LocationModel>(responseString);
                    }
                    else return null;
                }

                else return null;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message+ex.InnerException); return null; }
        }

        public async Task<Location> GetCurrentLocation()
        {
            try
            {
                //safe:
                //var request = new GeolocationRequest(GeolocationAccuracy.High);
                //var location = await Geolocation.GetLocationAsync(request);
                //simple :
                //Location accuretSenior = null;
                //Location location=null; 
                //accuretSenior = await Geolocation.GetLocationAsync();
                //location = await Geolocation.GetLastKnownLocationAsync();
                Location accurate = null;
               

                try
                {
                    
                    DateTime dateTime = DateTime.Now;
                    accurate = await Geolocation.GetLocationAsync();
                        //await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High, new TimeSpan(0, 0, 10)));
                    
                }
                catch(Exception ex) { Debug.WriteLine(ex.Message); }
                if (accurate != null)
                {
                    accurate.Timestamp = DateTime.Now;
                }

                    return accurate;
                
                //else if (accuretSenior != null && accuretSenior.Altitude.HasValue)
                //{
                //    return accuretSenior;
                //}
               // else if (location != null)
                //{
                    //Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                   // return location;
                //}
                
                    //request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    //location = await Geolocation.GetLocationAsync(request);
                    //if (location != null)
                    //{
                    //    //Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    //    return location;
                    //}
                    
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Debug.WriteLine(fnsEx.Message+fnsEx.InnerException);
                return null;
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Debug.WriteLine(fneEx.Message + fneEx.InnerException);
                return null;
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                Debug.WriteLine(pEx.Message + pEx.InnerException);
                return null;
                // Handle permission exception
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.InnerException);
                return null;
                // Unable to get location
            }
        }

    }
}
