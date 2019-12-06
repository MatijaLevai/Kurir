using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Kurir.Droid
{
    public static class HideService
    {
        public static void BackPress()
        {
            Intent main = new Intent(Intent.ActionMain);
            main.AddCategory(Intent.CategoryHome);
            MainActivity.Instance.StartActivity(main);
        }
    }
}