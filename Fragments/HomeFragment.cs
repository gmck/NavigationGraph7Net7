﻿using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.Fragment.App;
using AndroidX.Navigation;
using AndroidX.Navigation.UI;
using AndroidX.Preference;

namespace com.companyname.navigationgraph7net7.Fragments
{
    // OnCreateOptionsMenu, SetHasOptionsMenu (or when using C# HasOptionsMenu) and OnOptionsItemSelected have been deprecated with the release of Xamarin.AndroidX.Navigation.Fragment 2.5.1
    // New with this release are the new IMenuProvider and IMenuHost and replacement methods OnCreateMenu and OnMenuItemSelected
    // Therefore this requires the removal of OnCreateOptionsMenu and OnOptionsItemSelected from the MainActivity in your MainActivity if your fragments require different menus.
    // If retained, then every fragment will have the same menu.
    // You can no longer remove a menu from a fragment which doesn't require a menu by setting HasOptionsMenu = true and then doing a menu.Clear in OnCreateOptionsMenu.
    // In other words if you do have OnCreateOptionsMenu and OnOptionsItemSelected then you should move those menuitems to
    // the StartDestinationFragment = e.g. as in this example the HomeFragment.
    // AddMenuProvider is based on LifeCycle therefore it is only applicable while this fragment is visible. The other ctor requires you to use RemoveMenuProvider.
    // Any fragment that doesn't require a menu then doesn't implement the IMenuProvider

    // OnPrepareMenu and onMenuClosed are missing from IMenuProvider interface
    // See Menu Deprecations when upgrading AndroidX.Navigation.Fragment to 2.5.1 #611 Sept 2nd 2022
    public class HomeFragment : Fragment, IMenuProvider
    {
        private IMenuHost? menuHost;
        private bool animateFragments;
        
        // Just a test to see if I could replicate behaviour of the deprecated OnPrepareOptionsMenu. I doubt that this is the correct way to disable a menuItem - but it appears to work
        private bool enableSubscriptionInfoMenuItem;

        public HomeFragment() { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View? view = inflater.Inflate(Resource.Layout.fragment_home, container, false);
            TextView? textView = view!.FindViewById<TextView>(Resource.Id.text_home);
            textView!.Text = "This is home fragment";

            return view;
        }
        #endregion

        #region OnViewCreated
        public override void OnViewCreated(View view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Check for any changed values
            ISharedPreferences? sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity!);
            animateFragments = sharedPreferences!.GetBoolean("use_animations", false);
            enableSubscriptionInfoMenuItem = sharedPreferences.GetBoolean("showSubscriptionInfo", false);

            // New with release of Xamarin.AndroidX.Navigation.Fragment 2.5.1
            // see https://medium.com/tech-takeaways/how-to-migrate-the-deprecated-oncreateoptionsmenu-b59635d9fe10
            menuHost = RequireActivity();
            menuHost.AddMenuProvider(this, ViewLifecycleOwner, AndroidX.Lifecycle.Lifecycle.State.Resumed!);
            
            if (enableSubscriptionInfoMenuItem)
                menuHost.InvalidateMenu();

            // More concise than the above 
            //(RequireActivity() as IMenuHost).AddMenuProvider(this, ViewLifecycleOwner, AndroidX.Lifecycle.Lifecycle.State.Resumed);

        }
        #endregion

        #region OnCreateMenu
        public void OnCreateMenu(IMenu menu, MenuInflater menuInflater)
        {
            menuInflater.Inflate(Resource.Menu.menu_home_fragment, menu);
            OnPrepareMenu(menu);
        }
        #endregion

        #region OnPrepareMenu
        public void OnPrepareMenu(IMenu menu)
        {
            // OnPrepareMenu is missing from  IMenuProvider interface - this is a workaround.
            // I doubt that this is the correct way to disable a menuItem - but it appears to work. Trying to implement behaviour of the deprecated OnPrepareOptionsMenu,
            // think it should be using MenuHostHelper but not sure how to implement it - still trying to figure it out.

            IMenuItem? menuItemSettings = menu.FindItem(Resource.Id.action_subscription_info);
            menuItemSettings!.SetEnabled(enableSubscriptionInfoMenuItem);
        }
        #endregion

        #region OnMenuItemSelected
        public bool OnMenuItemSelected(IMenuItem menuItem)
        {
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
                    .SetPopUpTo(Resource.Id.home_fragment, false, true)     // Inclusive false, saveState true.
                    .SetRestoreState(true)
                    .Build();

            switch (menuItem.ItemId)
            {
                case Resource.Id.action_settings:
                    Navigation.FindNavController(Activity!, Resource.Id.nav_host).Navigate(Resource.Id.settingsFragment, null, navOptions);
                    return true;

                case Resource.Id.action_subscription_info:
                    ShowSubscriptionInfoDialog(GetString(Resource.String.subscription_explanation_title), GetString(Resource.String.subscription_explanation_text));
                    return true;

                // Must have this default condition - otherwise we lose the ability to open the NavigationMenu in the MainActivity via the hamburger icon - tapping on it doesn't display the NavigationView
                default:
                    return NavigationUI.OnNavDestinationSelected(menuItem, Navigation.FindNavController(Activity!, Resource.Id.nav_host));
            }
        }
        #endregion

        #region ShowSubscriptionInfoDialog - Moved from the MainActivity
        private void ShowSubscriptionInfoDialog(string title, string explanation)
        {
            string tag = "SubscriptionInfoDialogFragment";
            FragmentManager fm = Activity!.SupportFragmentManager;
            if (fm != null && !fm.IsDestroyed)
            {
                Fragment? fragment = fm.FindFragmentByTag(tag);
                if (fragment == null)
                    BasicDialogFragment.NewInstance(title, explanation).Show(fm, tag);
            }
        }
        #endregion

        #region not used - but keep as reference
        //public int BackStackCount()
        //{
        //    NavHostFragment? navHostFragment = SupportFragmentManager.FindFragmentById(Resource.Id.nav_host) as NavHostFragment;
        //    int backStackEntryCount = navHostFragment!.ChildFragmentManager.BackStackEntryCount;
        //    return backStackEntryCount;
        //}
        #endregion
    }
}