using Android.OS;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using AndroidX.Fragment.App;
using AndroidX.Navigation;
using System;

namespace com.companyname.navigationgraph7net7.Fragments
{
    public class PermissionsTestFragment : Fragment
    {
        public PermissionsTestFragment() { } 

        private ActivityResultLauncher? bluetoothPermissionLauncher;        

        public override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            bluetoothPermissionLauncher = RegisterForActivityResult(new ActivityResultContracts.RequestMultiplePermissions(), permissionsGranted =>
            {
                if ((bool)permissionsGranted)
                    Navigation.FindNavController(Activity!, Resource.Id.nav_host).Navigate(Resource.Id.settingsFragment);
                //else
                
                    //Permission denied - show DeniedDialog                }
            });

        }

        public override void OnViewCreated(Android.Views.View view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            

            bluetoothPermissionLauncher!.Launch(new string[] { Android.Manifest.Permission.BluetoothConnect, Android.Manifest.Permission.BluetoothScan });
        }
        private ActivityResultLauncher? RegisterForActivityResult(ActivityResultContracts.RequestMultiplePermissions requestMultiplePermissions, Action<object> value)
        {
            throw new NotImplementedException();
        }

        
    }
}
