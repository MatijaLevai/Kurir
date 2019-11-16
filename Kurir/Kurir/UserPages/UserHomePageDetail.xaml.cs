using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kurir.UserPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserHomePageDetail : ContentPage
    {
        public UserHomePageDetail()
        {
            InitializeComponent();
           logo.Source = ImageSource.FromResource("Kurir.Images.eko-kurir-logo.png");
           
        }
    }
}