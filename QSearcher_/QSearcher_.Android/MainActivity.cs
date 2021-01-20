
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Widget;
using Android.OS;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Auth.Api;
using Firebase.Auth;
using Firebase;
using Android.Content;
using Android.Gms.Tasks;
using Android.Gms.Location;
using Com.Karumi.Dexter;
using Android;
using Com.Karumi.Dexter.Listener.Single;
using Com.Karumi.Dexter.Listener;
using Android.Support.V4.App;
using Xamarin.Essentials;
using System.Threading.Tasks;
using QSearcher_.Droid;
using QSearcher_.Data;
using System.Linq;

[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]
namespace QSearcher_.Droid
{
    [Activity(Label = "QSearcher", Icon = "@drawable/icon2", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IOnSuccessListener, IOnFailureListener, IPermissionListener
    {
        GoogleSignInOptions gso;
        GoogleApiClient googleApiClient;
        FirebaseAuth firebaseAuth;
        public static PersonLogin login;
        LocationRequest locationRequest;
        FusedLocationProviderClient fusedLocationProviderClient;
        public static readonly int PickImageId = 1000;
        public TaskCompletionSource<System.IO.Stream> PickImageTaskCompletionSource { set; get; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Instance = this;
            LoadApplication(new App());
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                return;
            }
            //SetContentView(Resource.Layout.main)
            //  Intent intent = new Intent();
            // intent.SetClass(this,PrevActivity);
            gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
              .RequestIdToken("вставьте свой токен :)")
              .RequestEmail()
              .Build();
            googleApiClient = new GoogleApiClient.Builder(this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, gso).Build();
            googleApiClient.Connect();
            firebaseAuth = GetFirebaseAuth();
            string loginCheck = Preferences.Get("userName", "default");
            //string locationCheck = Preferences.Get("location", "epty");
            //if (!Preferences.ContainsKey("location"))
            //    Preferences.Set("location", "msk");
            if(Preferences.ContainsKey("location"))
            {
                string[] str = Preferences.Get("location", "empty").Split(',');
                ContentManager.Locations = str.ToList();
            }
            if (loginCheck == "empty" ||loginCheck=="default")
            {
                SigninButton();
            }

            Dexter.WithActivity(this)
                .WithPermission(Manifest.Permission.AccessFineLocation)
                .WithListener(this)
                .Check();
        }

       
        private FirebaseAuth GetFirebaseAuth()
        {
            var app = FirebaseApp.InitializeApp(this);
            FirebaseAuth mAuth;
            if (app == null)
            {
                var options = new FirebaseOptions.Builder()
                    .SetProjectId("ж")
                    .SetApplicationId("ж")
                    .SetApiKey("вствьте свой апи :)")
                    .SetDatabaseUrl("ж")
                    .SetStorageBucket("")
                    .Build();
                app = FirebaseApp.InitializeApp(this, options);
                mAuth = FirebaseAuth.Instance;
            }
            else
            {
                mAuth = FirebaseAuth.Instance;
            }
            return mAuth;
        }

        private void SigninButton()
        {
            var intent = Auth.GoogleSignInApi.GetSignInIntent(googleApiClient);
            StartActivityForResult(intent, 1);
        }
        
        internal static MainActivity Instance { get; private set; }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (data != null))
                {
                    Android.Net.Uri uri = data.Data;
                    System.IO.Stream stream = ContentResolver.OpenInputStream(uri);
                    PickImageTaskCompletionSource.SetResult(stream);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
            if (requestCode == 1)
            {
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                if (result.IsSuccess)
                {
                    GoogleSignInAccount account = result.SignInAccount;

                    LoginWithFirebase(account);
                }
            }
        }

        private void LoginWithFirebase(GoogleSignInAccount account)
        {
            var credentials = GoogleAuthProvider.GetCredential(account.IdToken, null);
            firebaseAuth.SignInWithCredential(credentials).AddOnSuccessListener(this)
                .AddOnFailureListener(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            login = new PersonLogin(firebaseAuth.CurrentUser.DisplayName, firebaseAuth.CurrentUser.Email, firebaseAuth.CurrentUser.PhotoUrl.Path);
            Toast.MakeText(this, "Добро пожаловать, " + login.Name, ToastLength.Short).Show();
            Preferences.Set("userName", PersonLogin.NameStatic);
            Preferences.Set("userEmail", PersonLogin.EmailStatic);
            Preferences.Set("userPhoto", PersonLogin.PhotoUrlStatic);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(this, "Произошла ошибка авторизации", ToastLength.Short).Show();
        }
        public static void GetOut()
        {
            FirebaseAuth.Instance.SignOut();
        }

        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            UpdateLocation();
            Toast.MakeText(this, "You should accept permission", ToastLength.Short).Show();
        }

        private void UpdateLocation()
        {
            BuildLocationRequest();

            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
                return;
            fusedLocationProviderClient.RequestLocationUpdates(locationRequest, GetPendingIntent());

        }

        private PendingIntent GetPendingIntent()
        {
            Intent intent = new Intent(this, typeof(MyLocationService));
            intent.SetAction(MyLocationService.ACTION_PROCESS_LOCATION);
            return PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
        }

        private void BuildLocationRequest()
        {
            locationRequest = new LocationRequest();
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetInterval(5600);
            locationRequest.SetFastestInterval(3000);
            locationRequest.SetSmallestDisplacement(10f);
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {
            Toast.MakeText(this, "Check Permission", ToastLength.Short).Show();
        }
    }
}