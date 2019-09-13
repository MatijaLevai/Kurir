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
                GetStatistics();
                TitleLabel.Text = "Statistics of the entire business";
                CalculateResult();
            }
            catch (Exception ex) { await DisplayAlert("Error",ex.Message,"ok"); }
            ////
            ///ODRADI PREPRAVKU LABELE ZA NEMA KURIRA I POPRAVI OBJECT REFERENCE NULL
            ////


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

        private async void GetStatistics()
        {
            var list = await deliveryService.GetAllDeliveries();
            if (list!=null)
           this.list = new List<DeliveryModel>(list);
            

        }
        private void CalculateResult()
        {
            foreach (var item in list)
            {
                Revenue += item.DeliveryPrice;
                NumberOfDeliveries++;
            }
            RevenueLabel.Text = Revenue.ToString();
            DeliveredLabel.Text = NumberOfDeliveries.ToString();
        }
        
        void OnDateSelectedStart(object sender, DateChangedEventArgs args)
        {
            var startDate =  (DatePicker) sender;
            if (startDate.Date > endDatePicker.Date)
            { endDatePicker.Date = startDate.Date; }
            endDatePicker.MinimumDate = startDate.Date;
            Recalculate();
        }
        void OnDateSelectedEnd(object sender, DateChangedEventArgs args)
        {
            var endDate = (DatePicker)sender;
            if (endDate.Date < startDatePicker.Date)
            { startDatePicker.Date = endDate.Date; }
            startDatePicker.MaximumDate = endDate.Date;
            Recalculate();
        }

        void OnSwitchToggled(object sender, ToggledEventArgs args)
        {
            Recalculate();
        }

        void Recalculate()
        {
            TimeSpan timeSpan = endDatePicker.Date - startDatePicker.Date +
                (includeSwitch.IsToggled ? TimeSpan.FromDays(1) : TimeSpan.Zero);

            resultLabel.Text = String.Format("{0} day{1} between dates",
                                             timeSpan.Days, timeSpan.Days == 1 ? "" : "s");
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            //endDatePicker.Date
            //startDatePicker.Date
            IEnumerable<DeliveryModel> result;
            if(includeSwitch.IsToggled)
                result = list.Where(d=>(d.EndTime.Date>=startDatePicker.Date)&&(d.EndTime.Date<=endDatePicker.Date));
            else       
                result = list.Where(d => (d.EndTime.Date >= startDatePicker.Date) && (d.EndTime.Date < endDatePicker.Date));
            list = new List<DeliveryModel>(result);
            CalculateResult();
        }
    }
}