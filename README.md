# NavigationGraph7Net7 net7.0-android33
**21/04/2023**

Had the wrong setting for 3 dot menus when in dark Mode.
The 3 dot menu was dark on light, should have been light on dark. Corrected to the following

```
<style name="Theme.NavigationGraph.PopupOverlay" parent="ThemeOverlay.Material3.Dark" />
```
**18/04/2023**

Another small change to eliminate an unwanted visible line at the bottom of the tab layout containing the page indictors of the LeaderboardFragment. Added the following to the TabLayout of the fragment_leaderboard_viewpager.xml.

```
android:background="?android:attr/colorBackground"
```

**11/04/2023**

While working on NavigationGraph8Net7 which will use the color naming scheme that jetpack compose devs use. That is name colours via their luminence values, which I find much easier to understand than the complex system that the Material 3 design developers came up with. I came across a problem with the two generic dialog type layouts generic_dialog and generic_dialog_switch_scrollable in that they still had references to AppCompat attributes. I corrected them and so have also corrected them in this project. Since they are customised dialogs they now follow the Material 3 spec and use the attribute colorOnSurface in the textView's android:textColor of those two dialog layouts. 

The changes make very little difference to NavigationGraph7Net7, but will be evident in NavigationGraph8Net7.

**06/04/2023**

Removed all references to the OnBackPressedCallback in the StartDestination Fragment (HomeFragment) as it can't be used if supporting Predictive Back Gesture. 

Reported failure of Predictive Back Gesture problem to Google on Pixel Android 13 devices (Pixel6 and Pixel3a) https://issuetracker.google.com/issues/275597731

Also had a response from Microsoft about the failure of the Xamarin Android Designer to open any xml layout file if the app uses a nav_graph. See https://developercommunity.visualstudio.com/t/XamarinAndroid-Designer-fails-on-openin/1617011.

It was first reported 25 Feb 2020, and now over three years later they claim they wont fix it. What a totally useless software company!!. Compare their response with the one from Google. Reading their response, you'd think they were conversing with a 9 year old who was disappointed about his latest school project. Must be from their latest Bing authored woke response group. I conclude from that using their Send Feedback from within Visual Studio 2022 as a complete waste of your time.

Minor change - Updated Xamarin.AndroidX.AppCompat to 1.6.1

**05/04/2023**

Had to revert to previous version of OnActivityDestroyed in NavigationGraph7Net7.cs because the change below was not tested against the SettingsFragment. With that change any setting in the fragment that caused the activity to be recreated would call activity.Finish() and cause the app to exit. Not what we wanted!!

**04/04/2023**
Changed OnActivityDestroyed in NavigationGraph7Net7.cs to

```
public void OnActivityDestroyed(Activity activity)
{
    if (activity is MainActivity && !activity.IsFinishing)
        activity.Finish();
}
```

IsFinishing is true in MainActivity so StopService is being called when debugging, but do not see it in the logs. Added a Toast in StopService which displays that the service method has stopped

**03/04/2023**
The PredictiveBackGesture stopped working reliably after upgrading my Pixel 6 to the March 2023 Pixel 6 Security/Feature Update. Three other Android 13 Samsung devices still display the PBG.



**14/03/2023** 

Removed the NavActivityOnBackPressedCallback class as it was serving no purpose and therefore not required.

Also commented out code in the HomeFragment re NavFragmentOnBackPressedCallback as it is no longer used. Therefore also commented out the reference to the home fragment in NavFragmentOnBackPressedCallback's HandleBackPressed.

NavigationGraph7Net also supports the Predictive Back Gesture by default. For more detailed information re the Predictive Back Gesture, please see the project PredictiveBackGestureNet7.

Now that VS 2022 17.6.0 Prev 1.0 has a half decent markdown editor, the NavigationGraph.docx file used by previous projects has been discontinued and replaced with the github READMe.md.


**04/03/2023** 

Updated the Toast for nullable in the RegisterFragment, left the previous efforts commented as it may help someone - way simple when you know how!!!

Remove the #pragma warning disable CS8602 from the Toast.

Updated some of the packages

**25/01/2023**

Removed references to IOnBackInvokedCallback see PredictiveBackGesture project.

Jan 25 2023

Added Application, Application.IActivityLifecycleCallbacks to NavigationGraph7ApplicationNet7.cs 
