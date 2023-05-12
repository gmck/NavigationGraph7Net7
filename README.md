# NavigationGraph7Net7 net7.0-android33
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
