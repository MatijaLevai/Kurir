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
        private HttpClient _client = App.client;
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
        public async Task<bool> UpdateSQLiteDbWithPayAndDelTypes()
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
        public IEnumerable<string> StreetsOfNs()
        {
            List<string> streets=new List<string>();
            streets.Add("Bulevar oslobođenja");
            streets.Add( "Bulevar Jaše Tomića" );
            streets.Add( "Bulevar kralja Petra I" );
            streets.Add( "Bulevar cara Lazara" );
            streets.Add( "Bulevar despota Stefana" );
            streets.Add( "Bulevar Mihajla Pupina");
            streets.Add( "Bulevar Evrope");
            streets.Add( "Subotički bulevar");
            streets.Add( "Bulevar patrijarha Pavla (Somborski bulevar)");
            streets.Add( "Bulevar vojvode Stepe" );
            streets.Add( "Bulevar kneza Miloša" );
            streets.Add( "Bulevar Jovana Dučića" );
            streets.Add( "Bulevar Slobodana Jovanović" );
            streets.Add( "Pozorišni trg" );
            streets.Add( "Trg 1. maja" );
            streets.Add( "Trg 23. oktobra" );
            streets.Add("Trg 27. marta" );
            streets.Add("Trg carice Milice" );
            streets.Add("Trg Dositeja Obradovića");
            streets.Add("Trg Feher Ferenca" );
            streets.Add("Trg galerija" );
            streets.Add("Trg Komenskog" );
            streets.Add("Trg majke Jevrosime" );
            streets.Add("Trg Marije Trandafil" );
            streets.Add("Trg mira" );
            streets.Add("Trg mladenaca" );
            streets.Add("Trg neznanog junaka" );
            streets.Add("Trg republike" );
            streets.Add("Trg slobode" );
            streets.Add("Trifkovićev trg" );
            streets.Add("Žitni trg" );
            streets.Add("Zmaj Jovina" );
            streets.Add("Dunavska" );
            streets.Add("Ilije Ognjanovića" );
            streets.Add("Laze Telečkog" );
            streets.Add("Miletićeva" );
            streets.Add("Pašićeva");
            streets.Add("Zlatne grede" );
            streets.Add("Njegoševa" );

            streets.Add("Futoška" );
            streets.Add("Jevrejska" );
            streets.Add("Šafarikova" );
            streets.Add("Masarikova" );
            streets.Add("Jovana Subotića" );
            streets.Add("Vojvode Bojovića" );
            streets.Add("Lukijana Mušickog" );
            streets.Add("Vojvode Šupljikca" );
            streets.Add("Vuka Karadžića" );
            streets.Add("Pap Pavla" );
            streets.Add("Narodnih heroja" );
            streets.Add("Železnička" );
            streets.Add("Vase Stajića" );
            streets.Add("Maksima Gorkog" );
            streets.Add("Radnička" );
            streets.Add("Stražilovska" );
            streets.Add("Stevana Musića" );
            streets.Add("Vojvode Mišića" );
            streets.Add("Jovana Đorđevića" );
            streets.Add("Vladike Platona" );
            streets.Add("Sutjeska" );
            streets.Add("Zarka Zrenjanina" );
            streets.Add("Kej žrtava racije" );
            streets.Add("Beogradski kej" );
            streets.Add("Sunčani kej" );
            streets.Add("Temerinska" );
            streets.Add("Gundulićeva" );
            streets.Add("Almaška" );
            streets.Add("Kosovska" );
            streets.Add("Tekelijina" );
            streets.Add("Marka Miljanova" );
            streets.Add("Save Vukovica" );
            streets.Add("Bele Njive" );
            streets.Add("Djordja Rajkovica" );
            streets.Add("Filipa Visnjica" );
            streets.Add("Svetosavska" );
            streets.Add("Matice Srpske" );
            streets.Add("Sterijina" );
            streets.Add("Baranjska" );
            streets.Add("Pecka" );
            streets.Add(  "Petra Kocica" );
            streets.Add(  "Milosa Obilica" );
            streets.Add(  "Pavla Stamatovica" );
            streets.Add(  "Koce Kolarova" );
            streets.Add(  "Jase Ignjatovica" );
            streets.Add(  "Patrijarha Carnojevica" );
            streets.Add(  "Hadzi Cvetica" );
            streets.Add(  "Jug Bogdana" );
            streets.Add(  "Skerliceva" );
            streets.Add(  "Daniciceva" );
            streets.Add(  "Sumadijska" );
            streets.Add(  "Episkopa Visariona" );
            streets.Add(  "Zarka Vasiljevica" );
            streets.Add(  "Stevana Milovanova" );
            streets.Add(  "Milosa Bajica" );
            streets.Add(  "Dusana Vasiljeva" );
            streets.Add(  "Milana Rakica" );
            streets.Add(  "Venizelosova" );
            streets.Add(  "Koste Hadzi mladjeg" );
            streets.Add(  "Zemljane Cuprije" );
            streets.Add(  "Marka Nesica" );
            
            
            streets.Add(  "Kisačka" );
            streets.Add(  "Jovana Cvijića" );
            streets.Add(  "Dositejeva");
            streets.Add(  "Karađorđeva" );
            streets.Add(  "Branka Radičevića" );
            streets.Add(  "Radoja Domanovića" );
            streets.Add(  "Partizanska" );
            
            
            streets.Add(  "Braće Ribnikar" );
            streets.Add( "Miše Dimitrijevića" );
            streets.Add( "Danila Kiša" );
            streets.Add( "Vojvođanska" );
            streets.Add( "Lasla Gala" );
            streets.Add( "Puškinova" );
            streets.Add( "Gogoljeva" );
            streets.Add( "Tolstojeva" );
            streets.Add( "Vojvode Knićanina" );
            
            
            
            streets.Add( "Fruškogorska" );
            streets.Add( "Narodnog fronta" );
             streets.Add("Dr Sime Miloševića" );
            streets.Add( "Drage Spasić" );
             streets.Add("Dragiše Brašovana" );
             streets.Add("Dr Ivana Ribara" );
             streets.Add("Resavska" );
             streets.Add("Ravanička" );
            
             streets.Add("Balzakova" );
             streets.Add("Podgorička" );
             streets.Add("Šekspirova" );
             streets.Add("Iva Andrića" );
             streets.Add("1300 kaplara" );
             streets.Add("Banović Strahinje" );
            
            
             streets.Add("Cara Dušana" );
             streets.Add("Hajduk Veljkova" );
             streets.Add("Novosadskog sajma" );
             streets.Add("Rumenačka" );
             streets.Add("Braće Popović" );
             streets.Add("Hadži Ruvimova" );
             streets.Add("Stevana Mokranjca" );
            
            
             streets.Add("Ilariona Ruvarca" );
            streets.Add("Heroja Pinkija" );
             streets.Add("Ćirila i Metodija" );
             streets.Add("Petefi Šandora" );
             streets.Add("Subotička" );
            streets.Add( "Vršačka" );
             streets.Add("Adi Endrea" );
             streets.Add("Bogdana Šuputa" );
             streets.Add("Feješ Klare" );
             streets.Add("Šarplaninska" );
            
            
             streets.Add("Kornelija Stankovića" );
            streets.Add( "Milenka Grčića" );
            streets.Add( "Janka Veselinovića" );
             streets.Add("Janka Čmelika" );
             streets.Add("Dr Svetislava Kasapinovića" );
             streets.Add("Ilije Birčanina" );
          
            
             streets.Add("Radivoja Raše Radujkova" );
             streets.Add("Stojana Novakovića" );
             streets.Add("Đorđa Nikšića Johana" );
             streets.Add("Todora Jovanovića Toze" );
            streets.Add("Mileve Marić" );
            streets.Add( "Kaće Dejanović" );
             streets.Add("Seljačkih buna" );
            streets.Add( "Partizanskih baza" );
             streets.Add("Braće Dronjak" );
             streets.Add("Bate Brkića" );
             streets.Add("Dušana Danilovića" );
             streets.Add("Stevana Hladnog" );
             streets.Add("Antuna Urbana" );
            
            
             streets.Add("Branka Ćopića" );
             streets.Add("Slavujeva" );
             streets.Add("Sime Šolaje" );
             streets.Add("Podunavska" );
            
            
             streets.Add("Temerinski put" );
             streets.Add("Sentandrejski put" );
             streets.Add("Primorska" );
             streets.Add("Otokara Keršovanija" );
             streets.Add("Čenejska" );
             streets.Add("Paje Radosavljevića" );
             streets.Add("Velebitska" );
            streets.Add("Savska");
            return streets;
        }
    }
}
