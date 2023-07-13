# NavigationGraph7Net7 : net7.0-android33
**July 13, 2023**

No real changes in this version, but I just wanted to add a comment about the package Xamarin.Google.Android.Material. 

As I was converting my main app (Material 2) to use Material 3, I found that I didn't need to include the above package, when I was expecting that I would. 

This might not be news to you, but when checking out what seemed to be an anomaly, I found that the package was already a dependency of the Xamarin.AndroidX.NavigationUI package. 

I can't remember if that has always been the case, but I thought I'd mention it just in case you were wondering why I have deleted it in this update.

**June 29, 2023**

Commented out return NavigationUI.OnNavDestinationSelected(menuItem, Navigation.FindNavController(Activity!, Resource.Id.nav_host)); in OnMenuItemSelected(IMenuItem menuItem) of the HomeFragment and replaced with just return false. At one stage the commented code was required for the Hamburger menu to work, but it now works without it.  

**June 26, 2023**

Just added a comment in MaintenanceFileSelectionFragment.cs to explain how to invalidate a menu with the following ```(RequireActivity() as IMenuHost).InvalidateMenu()``` since there is no code example in that fragment.


**June 21, 2023**

Renamed WidgetsFragment to MaterialWidgetsFragment
Added Fader3() to AnimationResource.cs.
Also Added ActivityResultCallback.cs - not using though

Commented out RequireActivity().OnBackPressedDispatcher.AddCallback(ViewLifecycleOwner, onBackPressedCallback) in all the fragments.

***SetPopUp***

Added .SetPopUpTo(Resource.Id.home_fragment, false, true) to NavOptions in OnNavigationItemSelected of the MainActivity.  Also did the same to NavOptions in the OnMenuItemSelected of the HomeFragment.

Additionally added SetPopUpTo(Resource.Id.slideshow_fragment, false, true) to NavOptions in BottomNavigationViewItemSelected.

Effectively this now means we don't have a use for the NavFragmentOnBackPressedCallback. These changes appear to allow the same functionality as was provided by the OnBackPressedCallback. The reason for the changes is with Android 14 approaching, I'm looking for the same functionality the OnBackPressedCallback provided. However, we now know from previous experiments with the Predictive Back Gesture that the callback had to be removed from the HomeFragment in order for the Predictive Back Gesture to work. So I'm hoping that the above changes will allow the Predictive Back Gesture to work as it does in this version at least on Samsung devices. 

As mentioned previously PBG stopped working on my Pixel 6 with the release of March update. Exactly the same happened with a new Pixel 7. It also displayed the PBG until it also had the March update. Google is warning developers that back navigation will be broken in their apps if they don’t support this feature when it’s enforced.

  Evidently Android14 has an OnBackAnimationCallback, but I've not seen any documentation.

  ***The old code is still there in each fragments and the NavFragmentOnBackPressedCallback, but the code is now commented out.***

  One of my apps required Bluetooth Permissions, so as a test I've added a Bluetooth Permissions check in this version. In that app in the SettingsFragment the user can choose from different paired Bluetooth devices, hence the permission requirement for devices running Android 12+. In the real app a HelperExplanationDialog is used to explain to the user that the permission request is about to requested. Here I haven't bothered with the dialog, so now when opening the Settings fragment from the 3-dot menu you will see the Permissions request. There is no effect on this app as it doesn't use Bluetooth. 
  
  Unfortunately even though the code works it is not the solution I'm looking for.  OnRequestPermissionsResult has been deprecated in AndroidX.Fragment.App.Fragment.
  
  The new way is to use ActivityResultCallback and an ActivityResultLauncher.  However, I haven't as yet worked out how to get it to wotk.

**June 5, 2023**

***Menu changes in Android Apps*** 

I have added an extra fragment in this version of NavigationGraph7Net7 called MaintenanceFileSelectionFragment which further demonstrates how to use the IMenuProvider and the IMenuHost interfaces for when you need a menu on a fragment that is selected via the 3 dot menu normally associated with the MainActivity.

There are significant changes for apps with the use of the new IMenuProvider interface and the  IMenuHost interface with the release of AndroidX.Core.View in Xamarin.AndroidX.Core.

I first started experimenting with IMenuProvider and IMenuHost in September 2022 and realised pretty much straight away that using them was going to cause a restructuring of the menus in my apps. My apps had always been based on Xamarin’s NavigationDrawer template. So effectively you had two ways of navigating either via the Navigation Drawer or via the 3 dot menu of the MainActivity.

I used the NavigationView of the Navigation Drawer for all the fragments that related to the actual operation of the app and the 3 dot menu of the MainActivity for maintenance-type features, e.g. SettingsFragment, GoogleSignInFragment,  Purchase a SubscriptionFragment,  Privacy Policy Dialog, About Dialog, Help Display, Revision History dialog etc.

The quirky part of that was that these fragments then automatically inherited the same 3-dot menu as every fragment of the 3-dot menu inherited that menu unless you created a new menu for a particular fragment. For instance, most of these fragments did not require a 3 dot menu, so the way around that was to then remove the MainActivity’s menu using HasOptionsMenu = true in the OnCreate of those fragments and then in the OnCreateOptionsMenu immediately call menu.Clear(). 

Therefore you repeated the same sort of code for every fragment that did not require a menu. If a fragment called from the MainActivity’s 3 dot menu did require a menu, then you simply created a menu for it, but first, **you still had to call menu.Clear()** to clear the MainActivity’s menu before inflating the new menu for that fragment. Hence my term “quirky”. 

All was fine, if not quirky, until a new version of AndroidX.Core.View arrived in September 2022, introduced via upgrading Xamarin.AndroidX.Navigation.Fragment from 2.4.2 to 2.5.1. The following methods were deprecated  OnCreateOptionsMenu, OnPrepareOptionsMenu, OnOptionsItemSelected and HasMenuOptions or more technically Java’s SetHasMenuOptions(bool). 

These methods were replaced with the IMenuProvider interface methods OnCreateMenu, OnPrepareMenu, OnMenuItemSelected and OnMenuClosed. Unfortunately, two methods were missing from the IMenuProvider interface in the AndroidX.Core.View package, onPrepareMenu and OnMenuClosed. Fortunately, those two missing methods have now (May 2023) been added.

Therefore instead of the menu belonging to the MainActivity, the equivalent menu now belongs to the StartDestination fragment as in this example the HomeFragment. There is now no 3 dot menu associated with the MainActivity.

So what do you do about a fragment such as the SettingsFragment that doesn’t have a menu? Answer absolutely nothing (other than remove the old menu code) and so say goodbye to the quirkiness of before.

The following is a quote from Ian Lake of Google from an answer on StackOverflow.

***AndroidX ComponentActivity [and its subclasses of FragmentActivity and AppCompatActivity] now implements the MenuHost interface. This allows any component to add menu items to the ActionBar by adding a MenuProvider instance to the activity. Each MenuProvider can optionally be added with a Lifecycle that will automatically control the visibility of those menu items based on the Lifecycle state and handle the removal of the MenuProvider when the Lifecycle is destroyed.***

He then goes on to make the following three points.


1.	A single MenuProvider should only be touching its Menu Items. You should never, ever, ever be "clearing all menu items" or anything that affects another component's menu items.

2.	By calling addMenuProvider with a Lifecycle (in this case, the Fragment view's Lifecycle - i.e., the one that only exists when the Fragment's view is on screen), then you automatically hide the menu items when your Fragment's view is destroyed (when your replace call happens) and automatically reshown when your fragment's view re-appears (i.e., when the back stack is popped).
3.	That the fragment itself that is controlling the Lifecycle and visibility of the menu items should be the one creating and handling its own menu items. Your activity (which can add its own MenuProvider as seen in the other example) should only be adding menu items that exist for the entire Lifecycle of the activity (items that are visible on all fragments).

The replacement methods are coded much the same as the original methods. The only difference is in the setup of the menu and the slight change in the method names by removing “options”. We now use the AddMenuProvider method of the IMenuHost interface. There are 3 AddMenuProvider methods. The third method is recommended for use with fragments.
```
void AddMenuProvider(IMenuProvider p0, ILifecycleOwner p1, AndroidX.Lifecycle.Lifecycle.State p2);
```

and the easiest way to use it is as follows in the OnViewCreated of the fragment
```
(RequireActivity() as IMenuHost).AddMenuProvider(this, ViewLifecycleOwner, AndroidX.Lifecycle.Lifecycle.State.Resumed!);
```

A fragment with a menu shouldn’t have an up button on the ActionBar. Whereas the SettingsFragment which doesn’t have a menu, does have an up button. I’ve found a couple of ways of removing the up button – see one way the code in OnDestinationChange in the MainActivity, however, I would have thought the more appropriate choice is to remove the back button within the fragment code in OnViewCreated as in this new fragment example.

Please note that the MaintenanceFileSelectionFragment is not meant to be complete as far as functionality as it was borrowed from one of my apps for testing purposes for the new menu code.

**May 25 2023**

Updated NuGet packages were just released. These include the long awaited fixes for the missing methods OnPrepareMenu and OnMenuClosed. The work arounds in the HomeFragment have now been removed and replaced with the new methods. 

Please note that I've deliberately set android:enableOnBackInvokedCallback="false" in the androidmanifest.xml. Google keep asking me for more information, even though I've supplied them with a project, so I'm not sure when it will get fixed. I'd be very interested in any feedback that others may have as it is possible this only effects Pixel devices in Australia.

**May 12, 2023**

Better documentation for Material 3 components at  https://github.com/material-components/material-components-android/tree/master/docs/components.

So I've modified the style of AlertDialogRoundedCornersTheme to match the above documentation.
```
Container: backgroundTint ?attr/colorSurface
Title color: ?attr/colorOnSurface
Text color: ?attr/colorOnSurfaceVariant
```
**May 3, 2023**

Rationalization of overriding the Theme Builder colours. Further to the changes made a couple of days ago, I've now removed all overridden colours except for colorPrimary and colorSecondary from the three themes. The text color of the dialogs and bottomSheetDialogs is now colorOnSurfaceVariant and the background colour of both is now colorSurfaceVariant. Unfortunately, the documentation for BottomSheetDialogs doesn't specify a colour for the text, so I've just gone with the same colour as used by standard dialogs.

All the packages have also been bumped to the latest versions.

**May 1, 2003**

I decided to revert to using the Material3 generated (https://m3.material.io/theme-builder#/custom) colours for each of the 3 themes, instead of what I indicated earlier for NavigationGraph8Net7. While I'm attracted to the jetpack compose convention of naming colours by their luminance value it is a lot of work to manually come up with a complete scheme.

However, if using the Theme Builder, I'm still stuck with having to override colours like colorPrimary and colorSecondary if I still want to maintain the colours of my app which was based on the Material2 design colour system. The missing Material2 colorPrimaryVariant in Material3 can only be replaced with Material3's colorSecondary (as suggested in the compose migration docs). However, because the actual colours input into the theme builder are not used, (they use your input colours as a seed, you then need to override colorPrimary and colorSecondary to at least use the same colours of the Toolbar and Statusbar of your app. I've also had to override colorOnSurface for use in dialogs.

I've left the original values in the themes, so you can comment out my changes and uncomment the original values to see the differences.

Other changes include a clean-up of the MainActivity, where various methods included notes and explanations about what the code was doing as the NavigationComponent evolved. I've removed those notes and explanations and moved them to the bottom of the MainActivity, as the notes are still valuable to understand how the Navigation Component has evolved over its development.

As the Predictive Back Gesture is still not working on Pixel devices running Android 13 (as mentioned here 06/04/2023) I've set ```android:enableOnBackInvokedCallback="false"``` in the AndroidManifest.xml. The update didn't only screw up Predictive Back Gesture, it also caused other problems. Note: This will disable the Predictive Back Gesture for all devices. If you want to enable it for devices that support it correctly (e.g. Samsung 13 devices), then set it back to true.


**April 21, 2023**

Had the wrong setting for 3 dot menus when in dark Mode.
The 3 dot menu was dark on light, should have been light on dark. Corrected to the following

```
<style name="Theme.NavigationGraph.PopupOverlay" parent="ThemeOverlay.Material3.Dark" />
```
**April 18, 2023**

Another small change to eliminate an unwanted visible line at the bottom of the tab layout containing the page indictors of the LeaderboardFragment. Added the following to the TabLayout of the fragment_leaderboard_viewpager.xml.

```
android:background="?android:attr/colorBackground"
```

**April 11, 2023**

While working on NavigationGraph8Net7 which will use the color naming scheme that jetpack compose devs use. That is name colours via their luminence values, which I find much easier to understand than the complex system that the Material 3 design developers came up with. I came across a problem with the two generic dialog type layouts generic_dialog and generic_dialog_switch_scrollable in that they still had references to AppCompat attributes. I corrected them and so have also corrected them in this project. Since they are customised dialogs they now follow the Material 3 spec and use the attribute colorOnSurface in the textView's android:textColor of those two dialog layouts. 

The changes make very little difference to NavigationGraph7Net7, but will be evident in NavigationGraph8Net7.

**April 6, 2023**

Removed all references to the OnBackPressedCallback in the StartDestination Fragment (HomeFragment) as it can't be used if supporting Predictive Back Gesture. 

Reported failure of Predictive Back Gesture problem to Google on Pixel Android 13 devices (Pixel6 and Pixel3a) https://issuetracker.google.com/issues/275597731

Also had a response from Microsoft about the failure of the Xamarin Android Designer to open any xml layout file if the app uses a nav_graph. See https://developercommunity.visualstudio.com/t/XamarinAndroid-Designer-fails-on-openin/1617011.

It was first reported 25 Feb 2020, and now over three years later they claim they wont fix it. What a totally useless software company!!. Compare their response with the one from Google. Reading their response, you'd think they were conversing with a 9 year old who was disappointed about his latest school project. Must be from their latest Bing authored woke response group. I conclude from that using their Send Feedback from within Visual Studio 2022 as a complete waste of your time.

Minor change - Updated Xamarin.AndroidX.AppCompat to 1.6.1

**April 5, 2023**

Had to revert to previous version of OnActivityDestroyed in NavigationGraph7Net7.cs because the change below was not tested against the SettingsFragment. With that change any setting in the fragment that caused the activity to be recreated would call activity.Finish() and cause the app to exit. Not what we wanted!!

**April 4, 2023**

Changed OnActivityDestroyed in NavigationGraph7Net7.cs to

```
public void OnActivityDestroyed(Activity activity)
{
    if (activity is MainActivity && !activity.IsFinishing)
        activity.Finish();
}
```

IsFinishing is true in MainActivity so StopService is being called when debugging, but do not see it in the logs. Added a Toast in StopService which displays that the service method has stopped

**April 3, 2023**

The PredictiveBackGesture stopped working reliably after upgrading my Pixel 6 to the March 2023 Pixel 6 Security/Feature Update. Three other Android 13 Samsung devices still display the PBG.



**March 14, 2023** 

Removed the NavActivityOnBackPressedCallback class as it was serving no purpose and therefore not required.

Also commented out code in the HomeFragment re NavFragmentOnBackPressedCallback as it is no longer used. Therefore also commented out the reference to the home fragment in NavFragmentOnBackPressedCallback's HandleBackPressed.

NavigationGraph7Net also supports the Predictive Back Gesture by default. For more detailed information re the Predictive Back Gesture, please see the project PredictiveBackGestureNet7.

Now that VS 2022 17.6.0 Prev 1.0 has a half decent markdown editor, the NavigationGraph.docx file used by previous projects has been discontinued and replaced with the github READMe.md.


**March 4,  2023** 

Updated the Toast for nullable in the RegisterFragment, left the previous efforts commented as it may help someone - way simple when you know how!!!

Remove the #pragma warning disable CS8602 from the Toast.

Updated some of the packages

**January 25, 2023**

Removed references to IOnBackInvokedCallback see PredictiveBackGesture project.

Added Application, Application.IActivityLifecycleCallbacks to NavigationGraph7ApplicationNet7.cs 
