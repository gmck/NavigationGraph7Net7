using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Navigation;
using AndroidX.Navigation.Fragment;
using AndroidX.Navigation.UI;
using Google.Android.Material.AppBar;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.companyname.navigationgraph7net7
{

    //adb tcpip 5555 
    //adb connect 192.168.1.102:5555 connected to 192.168.1.102:5555 - Pixel6
    //adb tcpip 5555
    //adb connect 192.168.1.116:5555 for the S20
    //Must have the usb connection when typing adb tcpip 5555. once connected by wifi we can then remove the usb cable

    //adb uninstall Mono.Android.DebugRuntime
    //adb uninstall com.companyname.whatever
    //adb shell pm uninstall -k --user 0 <package name>
    //adb shell cmd com.companyname.whatever uninstall -k 

    //https://proandroiddev.com/handling-back-press-in-android-13-the-correct-way-be43e0ad877a

    [Activity(Label = "@string/app_name", MainLauncher = true)]  //Theme = "@style/Theme.NavigationGraph.RedBmw", no required here - handled by postSplashScreenTheme, see Styles.xml
    public class MainActivity : BaseActivity, IOnApplyWindowInsetsListener,
                                NavController.IOnDestinationChangedListener,
                                NavigationView.IOnNavigationItemSelectedListener
    {

        private readonly string logTag = "navigationGraph7";

        private AppBarConfiguration? appBarConfiguration;
        private NavigationView? navigationView;
        private DrawerLayout? drawerLayout;
        private BottomNavigationView? bottomNavigationView;
        private NavController? navController;
        private MaterialToolbar? toolbar;

        // Preference variables - see OnDestinationChanged where they are checked
        private bool devicesWithNotchesAllowFullScreen;             // allow full screen for devices with notches
        private bool animateFragments;                              // animate fragments 
        private bool resetHelperExplanationDialogs;
        
        // The following fragments are immersive fragments - see SetShortEdgesIfRequired
        private List<int>? immersiveFragmentsDestinationIds;

        #region OnCreate
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);
            base.OnCreate(savedInstanceState);

            // System.Threading.Thread.Sleep(500). Only use for demonstration purposes in that during the SplashScrren you can easily observe the background color of the launch icon. Remove for production build.
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Require a toolbar
            toolbar = FindViewById<MaterialToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // navigationView, bottomNavigationView for NavigationUI and drawerLayout for the AppBarConfiguration and NavigationUI
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.bottom_nav);

            // NavHostFragment so we can get a NavController 
            NavHostFragment? navHostFragment = SupportFragmentManager.FindFragmentById(Resource.Id.nav_host) as NavHostFragment;
            navController = navHostFragment!.NavController;

            // These are the fragments that you don't wont the back button of the toolbar to display on e.g. topLevel fragments. They correspond to the items of the NavigationView.
            int[] topLevelDestinationIds = new int[] { Resource.Id.home_fragment, Resource.Id.gallery_fragment, Resource.Id.slideshow_fragment, Resource.Id.widgets_fragment, Resource.Id.purchase_fragment };
            appBarConfiguration = new AppBarConfiguration.Builder(topLevelDestinationIds).SetOpenableLayout(drawerLayout).Build();  // SetDrawerLayout replaced with SetOpenableLayout

            // The following fragments are immersive fragments - see SetShortEdgesIfRequired
            immersiveFragmentsDestinationIds = new List<int> { Resource.Id.race_result_fragment };

            NavigationUI.SetupActionBarWithNavController(this, navController, appBarConfiguration);

            navigationView!.SetNavigationItemSelectedListener(this);
            bottomNavigationView!.ItemSelected += BottomNavigationView_ItemSelected!;

            ViewCompat.SetOnApplyWindowInsetsListener(toolbar!, this);
            ViewCompat.SetOnApplyWindowInsetsListener(drawerLayout!, this);

            // Add the DestinationChanged listener
            navController.AddOnDestinationChangedListener(this);
        }
        #endregion

        #region OnApplyWindowInsets
        public WindowInsetsCompat OnApplyWindowInsets(View v, WindowInsetsCompat insets)
        {
            AndroidX.Core.Graphics.Insets statusBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.StatusBars());
            AndroidX.Core.Graphics.Insets navigationBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.NavigationBars());

            if (v is MaterialToolbar)
            {
                SetTopMargin(v, statusBarsInsets);

                // Appear never to need displayCutout because it is always null
                if (OperatingSystem.IsAndroidVersionAtLeast(28)) 
                {
                    if (insets.DisplayCutout != null)
                        Window!.Attributes!.LayoutInDisplayCutoutMode = devicesWithNotchesAllowFullScreen ? LayoutInDisplayCutoutMode.ShortEdges : LayoutInDisplayCutoutMode.Default;
                }
            }
            else if (v is DrawerLayout)
                SetLeftMargin(v, navigationBarsInsets);

            return insets;
        }
        #endregion

        #region SetMargins
        private static void SetTopMargin(View v, AndroidX.Core.Graphics.Insets insets)
        {
            ViewGroup.MarginLayoutParams marginLayoutParams = (ViewGroup.MarginLayoutParams)v!.LayoutParameters!;
            //Log.Debug(logTag, "MainActivity - marginLayoutParams.LeftMargin " + marginLayoutParams.LeftMargin.ToString());
            marginLayoutParams.LeftMargin = marginLayoutParams.LeftMargin;
            marginLayoutParams.TopMargin = insets.Top;          // top is all we are concerned with - this will position the toolbar insets.Top from the top of the screen
            marginLayoutParams.RightMargin = marginLayoutParams.RightMargin;
            marginLayoutParams.BottomMargin = marginLayoutParams.BottomMargin;
            v.LayoutParameters = marginLayoutParams;
            v.RequestLayout();
        }

        private static void SetLeftMargin(View v, AndroidX.Core.Graphics.Insets insets)
        {
            ViewGroup.MarginLayoutParams marginLayoutParams = (ViewGroup.MarginLayoutParams)v!.LayoutParameters!;
            marginLayoutParams.LeftMargin = insets.Left;
            marginLayoutParams.TopMargin = marginLayoutParams.TopMargin;
            marginLayoutParams.RightMargin = insets.Right;// marginLayoutParams.RightMargin;
            marginLayoutParams.BottomMargin = marginLayoutParams.BottomMargin;
            v.LayoutParameters = marginLayoutParams;
            v.RequestLayout();
        }
        // Make a single function for setting Margins
        #endregion

        #region OnSupportNavigationUp
        public override bool OnSupportNavigateUp()
        {
            return NavigationUI.NavigateUp(navController!, appBarConfiguration!) || base.OnSupportNavigateUp();
        }
        #endregion

        #region OnNavigationItemSelected
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            // Using Fader2 as the default as animateFragment is false by default - check AnimationResource.cs for different animations
            if (!animateFragments)
                AnimationResource.Fader2();
            else
                AnimationResource.Slider();

            NavOptions navOptions = new NavOptions.Builder()
                    .SetLaunchSingleTop(true)
                    .SetEnterAnim(AnimationResource.EnterAnimation)
                    .SetExitAnim(AnimationResource.ExitAnimation)
                    .SetPopEnterAnim(AnimationResource.PopEnterAnimation)
                    .SetPopExitAnim(AnimationResource.PopExitAnimation)
                    .Build();

            bool proceed = false;

            switch (menuItem.ItemId)
            {
                // These are all topLevel fragments
                // Add fragment classes and fragment layouts as we add to the codebase as per the NavigationView items. 
                // If any classes and layouts are missing, then the NavigationView will not update the item selected.
                // The menuitem highlight will stay on the current item and the current fragment will remain displayed, nor will the app crash.
                case Resource.Id.home_fragment:
                case Resource.Id.gallery_fragment:
                case Resource.Id.slideshow_fragment:
                case Resource.Id.widgets_fragment:
                case Resource.Id.purchase_fragment:
                    proceed = true;
                    break;

                default:
                    break;
            }
            // We have the option here of animating our toplevel destinations. If we don't want animation comment out the NavOptions. 
            bool handled = false;
            if (proceed)
            {
                navController!.Navigate(menuItem.ItemId, null, navOptions);
                handled = true;
            }

            if (drawerLayout!.IsDrawerOpen(GravityCompat.Start))
                drawerLayout.CloseDrawer(GravityCompat.Start);

            return handled;

        }
        #endregion

        #region BottomNavigationViewItemSelected
        private void BottomNavigationView_ItemSelected(object sender, NavigationBarView.ItemSelectedEventArgs e)
        {
            // Note NavigationBarView - not BottomNavigationView probably not what is expected. Note that BottomNavigationView inherits from NavigationBarView. See notes in NavigationGraph.docx
            if (!animateFragments)
                AnimationResource.Fader2();
            else
                AnimationResource.Slider();

            NavOptions navOptions = new NavOptions.Builder()
                    .SetLaunchSingleTop(true)
                    .SetEnterAnim(AnimationResource.EnterAnimation)
                    .SetExitAnim(AnimationResource.ExitAnimation)
                    .SetPopEnterAnim(AnimationResource.PopEnterAnimation)
                    .SetPopExitAnim(AnimationResource.PopExitAnimation)
                    .Build();
            
            Log.Debug(logTag, "Navigate to - Enter Animation " + navOptions.EnterAnim.ToString());
            Log.Debug(logTag, "Navigate to - Exit Animation " + navOptions.ExitAnim.ToString());
            Log.Debug(logTag, "Navigate to - Pop Enter Animation " + navOptions.PopEnterAnim.ToString());
            Log.Debug(logTag, "Navigate to - Pop Exit Animation " + navOptions.PopExitAnim.ToString());

            bool proceed = false;

            switch (e.Item.ItemId)
            {
                //case Resource.Id.holding_fragment:
                case Resource.Id.leaderboardpager_fragment:
                case Resource.Id.register_fragment:
                case Resource.Id.race_result_fragment:
                    proceed = true;
                    break;

                default:
                    break;
            }
            if (proceed)
                navController!.Navigate(e.Item.ItemId, null, navOptions);

        }
        #endregion

        #region OnDestinationChanged
        public void OnDestinationChanged(NavController navController, NavDestination navDestination, Bundle? bundle)
        {
            CheckForPreferenceChanges();

            // The first menu item is not checked by default, so we need to check it to show it is selected on the startDestination fragment, i.e. the home_fragment
            navigationView!.Menu!.FindItem(Resource.Id.home_fragment)!.SetChecked(navDestination.Id == Resource.Id.home_fragment);

            // The slideshowFragment contains a BottomNavigationView. We only want to show the BottomNavigationView when the SlideshowFragment is displayed.
            bottomNavigationView!.Visibility = navDestination.Id == Resource.Id.slideshow_fragment ? ViewStates.Visible : ViewStates.Gone;

            // By default because the LeaderboardPagerFragment and the RegisterFragment are not top level fragments, they will default to showing an up button (left arrow) plus the title.
            // If you don't want the up button, remove it here.  
            if (navDestination.Id == Resource.Id.leaderboardpager_fragment || navDestination.Id == Resource.Id.register_fragment || navDestination.Id == Resource.Id.race_result_fragment)
            {
                toolbar!.Title = navDestination.Label;   
                toolbar.NavigationIcon = null;
            }

            // Is it an immersive fragment or is the preference set to allow full screen on devices with notches
            SetShortEdgesIfRequired(navDestination);
        }
        #endregion

        #region CheckForPreferenceChanges
        private void CheckForPreferenceChanges()
        {
            // Check if anything has been changed in the Settings Fragment before re-reading and updating the preference variables
            resetHelperExplanationDialogs = sharedPreferences!.GetBoolean("helper_screens", false);
            
            if (resetHelperExplanationDialogs) 
            {
                ISharedPreferencesEditor? editor = sharedPreferences.Edit();
                editor!.PutBoolean("showSubscriptionExplanationDialog", true);
                editor.PutBoolean("helper_screens", false);
                editor.Commit();
            }
            
            // Re read again.
            resetHelperExplanationDialogs = sharedPreferences.GetBoolean("helper_screens", false);
            devicesWithNotchesAllowFullScreen = sharedPreferences.GetBoolean("devicesWithNotchesAllowFullScreen", false);
            animateFragments = sharedPreferences.GetBoolean("use_animations", false);
        }
        #endregion

        #region SetShortEdgesIfRequired
        private void SetShortEdgesIfRequired(NavDestination navDestination)
        {
            // Note: LayoutInDisplayCutoutMode.ShortEdges could be set in HideSystemUi in the ImmersiveFragment if you didn't have this requirement. 

            // For when we have more than one immersive fragment. Are they all going to displayed shortEdges or are only some screens (non immersive) going to be displayed ShortEdges.
            // Still to be done - change wording of the deviceWithNotchesAllowFullScreen, which will mean all fragments other than the ImmersiveFragments - see immersiveFragmentsDestinationIds - only one in the project.
            // Effectively it will be all fragments - or none, except all immersiveFragments will always be full screen because they will be in the List<int> immersiveFragmentDestinationIds. 
            if (OperatingSystem.IsAndroidVersionAtLeast(28))
                Window!.Attributes!.LayoutInDisplayCutoutMode = immersiveFragmentsDestinationIds!.Contains<int>(navDestination.Id) | devicesWithNotchesAllowFullScreen ? LayoutInDisplayCutoutMode.ShortEdges : LayoutInDisplayCutoutMode.Default;
            
            
        }
        #endregion

        #region OnDestroy
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (IsFinishing)
                StopService();

            Log.Debug(logTag, "OnDestroy IsFinishing is " + IsFinishing.ToString());
        }
        #endregion

        #region Methods only called by ImmersiveFragment - if using
        public void DisableDrawerLayout() => drawerLayout!.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
        public void EnableDrawerLayout() => drawerLayout!.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
        #endregion

        #region StopService - Dummy service test
        public void StopService()
        {
            // Called from OnDestroy when IsFinishing is true
            Log.Debug(logTag, "StopService - called from OnDestroy");
            Toast.MakeText(this, "Service has been stopped.", ToastLength.Long)!.Show();
        }
        #endregion

    }
}

#region OnCreate - Replaced - left here because of notes etc
//protected override void OnCreate(Bundle? savedInstanceState)
//{
//    AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);
//    base.OnCreate(savedInstanceState);

//    // Only for demonstration purposes in that you can easily see the background color and the launch icon. Remove for production build.
//    //System.Threading.Thread.Sleep(500);

//    // Set our view from the "main" layout resource
//    SetContentView(Resource.Layout.activity_main);

//    // Require a toolbar
//    toolbar = FindViewById<MaterialToolbar>(Resource.Id.toolbar);
//    SetSupportActionBar(toolbar);

//    // navigationView, bottomNavigationView for NavigationUI and drawerLayout for the AppBarConfiguration and NavigationUI
//    drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
//    navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
//    bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.bottom_nav);

//    // NavHostFragment so we can get a NavController 
//    NavHostFragment? navHostFragment = SupportFragmentManager.FindFragmentById(Resource.Id.nav_host) as NavHostFragment;
//    navController = navHostFragment!.NavController;


//    // These are the fragments that you don't wont the back button of the toolbar to display on e.g. topLevel fragments. They correspond to the items of the NavigationView.
//    int[] topLevelDestinationIds = new int[] { Resource.Id.home_fragment, Resource.Id.gallery_fragment, Resource.Id.slideshow_fragment, Resource.Id.widgets_fragment, Resource.Id.purchase_fragment };
//    appBarConfiguration = new AppBarConfiguration.Builder(topLevelDestinationIds).SetOpenableLayout(drawerLayout).Build();  // SetDrawerLayout replaced with SetOpenableLayout

//    // The following fragments are immersive fragments - see SetShortEdgesIfRequired
//    immersiveFragmentsDestinationIds = new List<int> { Resource.Id.race_result_fragment };

//    NavigationUI.SetupActionBarWithNavController(this, navController, appBarConfiguration);

//    // Notes using both Navigation.Fragment and Navigation.UI version 2.3.5.3. Navigation.UI therefore includes Android.Material 1.4.0.4
//    // These two are working, but no animation, other than when the two fragments opened from slideshowFragment close, made possible because of HandleBackPressed(NavOptions navOptions)
//    // Could fix by adding animation to the graph, but that limits the app to only one type of animation. Therefore replacing with SetNavigationItemSelectedListener 
//    // That solves the problem of animating the top level fragments, but opening both fragments via the BottomNavigationView still have no animation.
//    // So will replace NavigationUI.SetupWithNavController(bottomNavigationView, navController) with BottomNavigationView_ItemSelected

//    //NavigationUI.SetupWithNavController(navigationView, navController);
//    //NavigationUI.SetupWithNavController(bottomNavigationView, navController);

//    // Upgrading to Navigation.Fragment and Navigation.UI version 2.4.2. Navigation.UI includes now Android.Material 1.5.0.2 - also tested 1.6.0
//    navigationView!.SetNavigationItemSelectedListener(this);
//    bottomNavigationView!.ItemSelected += BottomNavigationView_ItemSelected!;

//    ViewCompat.SetOnApplyWindowInsetsListener(toolbar!, this);
//    ViewCompat.SetOnApplyWindowInsetsListener(drawerLayout!, this);

//    // Add the DestinationChanged listener
//    navController.AddOnDestinationChangedListener(this);

//    #region Notes
//    // Demonstrates the problem if using 2.3.5.3 versions of Navigation and 1.4.0.4 of Material respectively
//    // Already using both overloads of SetupWithNavController() and there is no provision to pass a NavOptions. Since there is no animation contained in nav_graph therefore no animation when
//    // opening any fragment. 
//    // The only animation is in closing the fragments as each fragment has a HandleBackPressed(NavOptions navOptions)
//    // Therefore to get animation we had to drop NavigationUI.SetupWithNavController(navigationView, navController) and use navigationView.SetNavigationItemSelectedListener(this) which allows creating our
//    // own NavOptions. At this point each fragment can be potentially animated both opening and closing.
//    // Test: comment out NavigationUI.SetupWithNavController(navigationView, navController) and uncomment navigationView.SetNavigationItemSelectedListener(this);
//    // Works as per the requirement with a choice of animations, controlled via a preference. The ugly slider animation makes checking the animation of each fragment animation easier to view.

//    // Upgrade Navigation packages to latest available. 2.4.2 will include material 1.5.0.2
//    // Clean, Rebuild Deploy. All seems ok until you try and open either of the BottomNavigationView Fragments after it has already been opened once. Will not open again without first closing
//    // the SlideshowFragment and then opening it again and either fragment will open, but only the one time.
//    // Attempts to fix.
//    // Comment out NavigationUI.SetupWithNavController(bottomNavigationView, navController) and uncomment bottomNavigationView.ItemSelected, which works
//    // It would appear that there is a problem with Xamarin.Google.Android.Material 1.5.0.2 
//    // Next step was to add Xamarin.Google.Android.Material 1.6.0 
//    // No change from 1.5.0.2 behavior - work arounds work as before.
//    #endregion
//}
#endregion

#region OnApplyWindowInsets - Replaced with cleaned up version - left here because of notes etc
//public WindowInsetsCompat OnApplyWindowInsets(View v, WindowInsetsCompat insets)
//{
//    AndroidX.Core.Graphics.Insets statusBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.StatusBars());
//    // This is the only one we need the rest were for Log.Debug purposes to prove that we weren't getting insets until after the HideSystemUi had executed. 
//    //AndroidX.Core.Graphics.Insets systemBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());            
//    AndroidX.Core.Graphics.Insets navigationBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.NavigationBars());

//    if (v is MaterialToolbar)
//    {
//        SetTopMargin(v, statusBarsInsets);

//        // Appear never to need displayCutout because it is always null
//        if (OperatingSystem.IsAndroidVersionAtLeast(28)) //if (OperatingSystem.IsAndroidVersionAtLeast(28))
//        {
//            if (insets.DisplayCutout != null)
//            {
//                Window!.Attributes!.LayoutInDisplayCutoutMode = devicesWithNotchesAllowFullScreen ? LayoutInDisplayCutoutMode.ShortEdges : LayoutInDisplayCutoutMode.Default;
//                //Log.Debug(logTag, "MainActivity - LayoutInDisplayCutoutMode is " + Window.Attributes.LayoutInDisplayCutoutMode.ToString());
//                //Log.Debug(logTag, "MainActivity - statusBarsInsets are " + statusBarsInsets.ToString());
//            }
//        }
//        //Log.Debug(logTag, "MainActivity - StatusBarsInsets are " + statusBarsInsets.ToString());
//        //Log.Debug(logTag, "MainActivity - NavigationBarsInsets are " + navigationBarsInsets.ToString());
//        //Log.Debug(logTag, "MainActivity - SystemBarsInsets are " + systemBarsInsets.ToString());
//    }
//    else if (v is DrawerLayout)
//        SetLeftMargin(v, navigationBarsInsets);

//    return insets;
//}
#endregion
