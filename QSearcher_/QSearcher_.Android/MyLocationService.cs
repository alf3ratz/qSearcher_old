using System;
using Android.Content;
using Android.Gms.Location;
using Android.Widget;
using QSearcher_.Pages;

namespace QSearcher_.Droid
{
    [BroadcastReceiver]
    public class MyLocationService : BroadcastReceiver
    {
        public static string ACTION_PROCESS_LOCATION = "QSearcher.UPDATE_LOCATION";
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent != null)
            {
                string action = intent.Action;
                if (action.Equals(ACTION_PROCESS_LOCATION))
                {
                    LocationResult result = LocationResult.ExtractResult(intent);
                    if (result != null)
                    {
                        var location = result.LastLocation;
                        try
                        {
                            MapPage.MyLat = location.Latitude;
                            MapPage.MyLon = location.Longitude;
                        }
                        catch (Exception)
                        {
                            Toast.MakeText(context, "Something went wrong!", ToastLength.Short).Show();
                        }
                    }
                }
            }
        }
    }
}