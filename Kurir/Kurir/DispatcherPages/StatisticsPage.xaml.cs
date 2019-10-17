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

namespace Kurir.DispatcherPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatisticsPage : ContentPage
    {
        private readonly HttpClient _client = App.client;
        private readonly SQLiteAsyncConnection _connection;
        private SQLiteHelper liteHelper;
        private List<PaymentTypeModel> paymentTypes;
        private List<DeliveryTypeModel> deliveryTypes;
        private List<DeliveryModel> list;
        private DeliveryService deliveryService;
        private decimal Revenue;
        private int NumberOfDeliveries;
        public StatisticsPage()
        {
            deliveryService = new DeliveryService();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            liteHelper = new SQLiteHelper();
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
            try {
                if (await GetStatistics())
                { }
                else throw new Exception("No data to show.");
              
            }
            catch (Exception ex) { await DisplayAlert("Error",ex.Message,"ok"); }
           


            base.OnAppearing();
        }
        private async Task<bool> DelAndPayTypes()
        {
            try
            {

                var response = await liteHelper.UpdateSQLiteDb();
                if (response)
                {

                    paymentTypes = await _connection.Table<PaymentTypeModel>().ToListAsync();
                    deliveryTypes = await _connection.Table<DeliveryTypeModel>().ToListAsync();
                    
                    return true;
                }
                else
                {

                    return false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Problem occurred", ex.Message, "ok");
                return false;
            }

        }

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
                var sveDostave = await deliveryService.GetAllFinishedDeliveries();

                StatisticUserCandDModel fullStat = new StatisticUserCandDModel
                {   ImePrezime= "Total",
                    BrojDostava = sveDostave.Count(),
                    Promet = Convert.ToDouble(sveDostave.Sum(x => x.DeliveryPrice)),
                };
                foreach (var item in sveDostave)
                {


                    switch (item.DeliveryTypeID)
                    {
                        default:
                            break;
                    }
                    fullStat.PrometCash = Convert.ToDouble(sveDostave.Where(y => y.DeliveryTypeID == 1).Sum(x => x.DeliveryPrice));
                    fullStat.PrometFaktura = Convert.ToDouble(sveDostave.Where(y => y.DeliveryTypeID == 2).Sum(x => x.DeliveryPrice));
                    fullStat.PrometCupon = Convert.ToDouble(sveDostave.Where(y => y.DeliveryTypeID == 3).Sum(x => x.DeliveryPrice));
                }
                fullStat.PrihodOdPrometa = fullStat.Promet;
                foreach (var item in listC)
                {
                    fullStat.PrihodOdPrometa -= item.PrihodOdPrometa;
                }
                listC.Add(fullStat);
                Statistics.ItemsSource = listC;
                return true;
            }
           catch(Exception ex) {
                return false; }
        }
        
        
        void OnDateSelectedStart(object sender, DateChangedEventArgs args)
        {
            var startDate =  (DatePicker) sender;
            if (startDate.Date > endDatePicker.Date)
            { endDatePicker.Date = startDate.Date; }
            endDatePicker.MinimumDate = startDate.Date;
        }
        void OnDateSelectedEnd(object sender, DateChangedEventArgs args)
        {
            var endDate = (DatePicker)sender;
            if (endDate.Date < startDatePicker.Date)
            { startDatePicker.Date = endDate.Date; }
            startDatePicker.MaximumDate = endDate.Date;
        }

       

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await GetStatistics();
        }
    }
}