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

namespace AndroidBeacon.Droid.helpers
{
    public static class Extensions
    {
        public static bool IsIn<T>(this T @this, params T[] types)
        {
            return types.Contains(@this);
        }
    }
}