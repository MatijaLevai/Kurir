using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Kurir.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new Kurir.App());
            Xamarin.FormsMaps.Init("Asji15gO1XMlGuAY-CtztLimsiCN4kdtVD-K9uAxKyFN5grWwT4n5jbC_zSNYFE9");
          
            //try { Xamarin.FormsMaps.Init("AlWR1u - 4e7Y720S3XsQXFSJTKvJDjQmrZB6nTcFdJite01Y8TJofCZ1LjRcfQzUR"); }
            //catch { Xamarin.FormsMaps.Init("Asji15gO1XMlGuAY-CtztLimsiCN4kdtVD-K9uAxKyFN5grWwT4n5jbC_zSNYFE9"); }
        }
    }
}
