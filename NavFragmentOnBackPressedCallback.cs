﻿using Android.Content;
using Android.Util;
using AndroidX.Activity;
using AndroidX.Fragment.App;
using AndroidX.Navigation;
using AndroidX.Preference;
using com.companyname.navigationgraph7net7.Fragments;

namespace com.companyname.navigationgraph7net7
{
    public class NavFragmentOnBackPressedCallback : OnBackPressedCallback
    {
        // Notes: OnBackPressedCallback was failing to work if instantiated in OnStart it would work in most instances, but fail on some Fragments OnDestroy where the callback is removed
        // onBackPressedCallback?.Remove();
        // base.OnDestroy();
        // onBackPressedCallback could be null and therefore the callback was not removed which subsequently stuffed up other fragments.
        // Moving  the instantiation from OnStart to OnResume appears to have fixed the problem.

        private readonly Fragment fragment;
        private readonly bool animateFragments;
        private NavOptions? navOptions;

        private readonly string logTag = "navigationGraph7";

        public NavFragmentOnBackPressedCallback(Fragment fragment, bool enabled) : base(enabled)
        {
            this.fragment = fragment;
            // For animations only
            ISharedPreferences? sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this.fragment.Activity!);
            animateFragments = sharedPreferences!.GetBoolean("use_animations", false);
        }

        public override void HandleOnBackPressed()
        {
            if (!animateFragments)
                AnimationResource.Fader3();
            else
                AnimationResource.Slider();

            navOptions = new NavOptions.Builder()
                    .SetLaunchSingleTop(true) // 22/05/2021 We do need this
                    .SetEnterAnim(AnimationResource.EnterAnimation)
                    .SetExitAnim(AnimationResource.ExitAnimation)
                    .SetPopEnterAnim(AnimationResource.PopEnterAnimation)
                    .SetPopExitAnim(AnimationResource.PopExitAnimation)
                    //.SetPopUpTo(Resource.Id.home_fragment, false, true)             // Forgot to add this earlier, which caused incorrect popping of the fragments of the NavigationView 
                    .Build();

            Log.Debug(logTag, "Navigate back - Enter Animation " + navOptions.EnterAnim.ToString());
            Log.Debug(logTag, "Navigate back - Exit Animation " + navOptions.ExitAnim.ToString());
            Log.Debug(logTag, "Navigate back - Pop Enter Animation " + navOptions.PopEnterAnim.ToString());
            Log.Debug(logTag, "Navigate back - Pop Exit Animation " + navOptions.PopExitAnim.ToString());

            //Mixture of top level and non top level fragments
            //if (fragment is HomeFragment homeFragment)
            //    homeFragment.HandleOnBackPressed();
            //if (fragment is GalleryFragment galleryFragment)
            //    galleryFragment.HandleOnBackPressed(navOptions);
            //else if (fragment is SlideshowFragment slideshowFragment)
            //    slideshowFragment.HandleOnBackPressed(navOptions);
            //else if (fragment is LeaderboardPagerFragment leaderboardPagerFragment)
            //    leaderboardPagerFragment.HandleOnBackPressed(navOptions);
            //else if (fragment is RegisterFragment registerFragment)
            //    registerFragment.HandleOnBackPressed(navOptions);
            //else if (fragment is RaceResultFragment raceResultFragment)
            //    raceResultFragment.HandleOnBackPressed(navOptions);
            //else if (fragment is MaterialWidgetsFragment materialWidgetsFragment)
            //    materialWidgetsFragment.HandleOnBackPressed(navOptions);
            //else if (fragment is PurchaseFragment purchaseFragment)
            //    purchaseFragment.HandleOnBackPressed(navOptions);
            //else if (fragment is MaintenanceFileSelectionFragment maintenanceFileSelectionFragment)
            //    maintenanceFileSelectionFragment.HandleOnBackPressed(navOptions);
            //else if (fragment is SettingsFragment settingsFragment)
            //    settingsFragment.HandleOnBackPressed(navOptions);

            //if (fragment is LeaderboardPagerFragment leaderboardPagerFragment)
            //    leaderboardPagerFragment.HandleOnBackPressed(navOptions);
            //else if (fragment is RegisterFragment registerFragment)
            //    registerFragment.HandleOnBackPressed(navOptions);
            //else if (fragment is RaceResultFragment raceResultFragment)
            //    raceResultFragment.HandleOnBackPressed(navOptions);
        }
    } 
}

