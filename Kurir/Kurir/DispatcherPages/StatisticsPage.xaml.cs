using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Kurir.Models;
using SQLite;
using System.Net.Http;
using Kurir.Persistance;
using System.Diagnostics;

namespace Kurir.DispatcherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatisticsPage : ContentPage
    {
        #region excess
        //private readonly HttpClient _client = App.client;
        //private readonly SQLiteAsyncConnection _connection;
        ////private SQLiteHelper liteHelper;
        //private List<PaymentTypeModel> paymentTypes;
        //private List<DeliveryTypeModel> deliveryTypes;
        //private List<DeliveryModel> list;
        //private decimal Revenue;
        //private int NumberOfDeliveries;
        /////////////////////////////
        //_connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        //liteHelper = new SQLiteHelper();
        #endregion
        private List<DeliveryModel> sveDostave;
        private readonly DeliveryService deliveryService;
        public StatisticsPage()
        {
            deliveryService = new DeliveryService();
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            #region pickerRegion
            //< StackLayout Orientation = "Horizontal"
            //         Margin = "0, 0, 0, 30" >
            //    < Label Text = "Include both days in total: "
            //       VerticalOptions = "Center" />
            //    < Switch x: Name = "includeSwitch"
            //        Toggled = "OnSwitchToggled" />
            //</ StackLayout >
            //Picker PaymentTypePicker = new Picker()
            //{
            //   ItemsSource = paymentTypes,
            //   SelectedIndex = 0
            //};
            ////PaymentTypePicker.SetBinding(Picker.ItemsSourceProperty, "paymentTypes",);
            //PaymentTypePicker.ItemDisplayBinding = new Binding("PaymentTypeName");
            //Picker DeliveryTypePicker = new Picker()
            //{
            //    ItemsSource = deliveryTypes,
            //    SelectedIndex = 0
            //};
            //PaymentTypePicker.SetBinding(Picker.ItemsSourceProperty, "paymentTypes",);
            //DeliveryTypePicker.ItemDisplayBinding = new Binding("DeliveryTypeName");
            #endregion
            

            startDatePicker.Date = new DateTime(2019, 8, 1);
            endDatePicker.Date = DateTime.Now;
            base.OnAppearing();
            try
            {
                if (await GetStatistics())
                { }
                else throw new Exception("No data to show.");

            }
            catch (Exception ex) { await DisplayAlert("Error", ex.Message, "ok"); }

        }
        //private async Task<bool> DelAndPayTypes()//private async Task<bool> DelAndPayTypes()
        //{
        //    try
        //    {

        //        var response = await liteHelper.UpdateSQLiteDb();
        //        if (response)
        //        {

        //            paymentTypes = await _connection.Table<PaymentTypeModel>().ToListAsync();
        //            deliveryTypes = await _connection.Table<DeliveryTypeModel>().ToListAsync();

        //            return true;
        //        }
        //        else
        //        {

        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Problem occurred", ex.Message, "ok");
        //        return false;
        //    }

        //}
        private async Task<bool> GetStatistics()
        {
            try
            {
                List<StatisticUserCandDModel> listC = new List<StatisticUserCandDModel>();
                List<StatisticUserCandDModel> ListD = new List<StatisticUserCandDModel>();
                var lc = await deliveryService.GetStatisticsOfCouriers(startDatePicker.Date, endDatePicker.Date);
                var ld = await deliveryService.GetStatisticsOfDispatchers(startDatePicker.Date, endDatePicker.Date);
                if (lc != null)
                { listC = new List<StatisticUserCandDModel>(lc); }
                if (ld != null)
                { ListD = new List<StatisticUserCandDModel>(ld); }
                if (listC.Count() > 0)
                {
                    if (ListD.Count() > 0)
                    {
                        foreach (var item in listC)
                        {
                        
                            var d = ListD.Where(i => i.UserID == item.UserID).FirstOrDefault();
                            if (d != null)
                            {
                                item.PrometCash += d.PrometCash;
                                item.PrometCupon += d.PrometCupon;
                                item.PrometFaktura += d.PrometFaktura;
                                item.Promet += d.Promet;
                                item.PrihodOdPrometa += d.PrihodOdPrometa;
                                item.BrojDostava += d.BrojDostava;
                                ListD.Remove(d);
                            }
                        }
                    }
                }
                if (ListD.Count > 0)
                {
                    foreach (var item in ListD)
                    {
                        listC.Add(item);
                    }
                }
                await GetDeliveries();
                StatisticUserCandDModel fullStat = new StatisticUserCandDModel
                { ImePrezime = "Total",
                    BrojDostava = sveDostave.Count(),
                    Promet = Convert.ToDouble(sveDostave.Sum(x => x.DeliveryPrice)),
                    PrometCash = 0,
                    PrometFaktura = 0,
                    PrometCupon = 0
                };
                foreach (var item in sveDostave)
                {
                    switch (item.PaymentTypeID)
                    {
                        case 1:
                            fullStat.PrometCash += Convert.ToDouble(item.DeliveryPrice);
                            break;
                        case 2:
                            fullStat.PrometFaktura += Convert.ToDouble(item.DeliveryPrice);
                            break;
                        case 3:
                            fullStat.PrometCupon += Convert.ToDouble(item.DeliveryPrice);
                            break;
                    }
                }
                if (listC.Count > 0)
                {
                    fullStat.PrihodOdPrometa = fullStat.Promet-listC.Sum(x=>x.PrihodOdPrometa);
                    
                    listC.Add(fullStat);
                    Statistics.ItemsSource = listC;
                    return true;
                }
                else return false;
            }
           catch(Exception ex)
            {
                Debug.WriteLine(ex.InnerException + "_\r\n" + ex.Message);
                return false;
            }
        }

        private async Task GetDeliveries()
        {
            sveDostave = new List<DeliveryModel>(await deliveryService.GetAllFinishedDeliveries(startDatePicker.Date, endDatePicker.Date));
            foreach (var item in sveDostave)
            {
                switch (item.DeliveryStatus)
                {
                    case 4:
                        item.DeliveryStatusImageSource = "delivered.png";
                        break;
                    case 3:
                        item.DeliveryStatusImageSource = "zeleni50.png";
                        break;
                    case 2:
                        item.DeliveryStatusImageSource = "zuti50.png";
                        break;
                    case 1:
                        item.DeliveryStatusImageSource = "crveni50.png";
                        break;
                    default:
                        item.DeliveryStatusImageSource = "crveni50.png";
                        break;
                }
            }
        }
        async void OnDateSelectedStart(object sender, DateChangedEventArgs args)
        {
            var startDate =  (DatePicker) sender;
            if (startDate.Date > endDatePicker.Date)
            { endDatePicker.Date = startDate.Date;}
            endDatePicker.MinimumDate = startDate.Date;
            await GetStatistics();
        }
        async void OnDateSelectedEnd(object sender, DateChangedEventArgs args)
        {
            var endDate = (DatePicker)sender;
            if (endDate.Date < startDatePicker.Date)
            { startDatePicker.Date = endDate.Date; }
            startDatePicker.MaximumDate = endDate.Date;
            await GetStatistics();
        }
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await GetStatistics();
        }

        private void Statistics_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var obj = sender as ListView;
            var objCD = obj.SelectedItem as StatisticUserCandDModel;
           var list =  sveDostave.Where(x => ((x.CourierID == objCD.UserID) || (x.DispatcherID == objCD.UserID)));
        if(list!=null)
        DeliveryList.ItemsSource = list;
        }

        private async void DeliveryList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var obj = sender as ListView;
            var objMD = obj.SelectedItem as DeliveryModel;
            string answer = await DisplayActionSheet("Pogledaj detalje dostave ili ažuriraj.","Odustani",null,"Ažuriraj","Detalji");
            if (answer == "Detalji")
            { await Navigation.PushModalAsync(new DeliveryDetailPage(objMD)); }
            else if (answer == "Ažuriraj")
            {
                await Navigation.PushModalAsync(new EditDeliveryDispatcherPage(objMD.ConvertToExtended()));
            }
        }
    }
}