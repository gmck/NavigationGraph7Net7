# NavigationGraph7Net7 net7.0-android33
**06/04/2023**

Removed all references to the OnBackPressedCallback in the StartDestination Fragment (HomeFragment) as it can't be used if supporting Predictive Back Gesture. 

Reported failure of Predictive Back Gesture problem to Google on Pixel Android 13 devices (Pixel6 and Pixel3a) https://issuetracker.google.com/issues/275597731

Also had a response from Microsoft about the failure of the Xamarin Android Designer to open any xml layout file if the app uses a nav_graph. See https://developercommunity.visualstudio.com/t/XamarinAndroid-Designer-fails-on-openin/1617011.

It was first reported 25 Feb 2020, and now over three years later they claim they wont fix it. What a useless software company!!. Compare their response with the one from Google. Reading their response, you'd think they were conversing with a 9 year old who was disappointed about his latest school project. Must be from their latest Bing authored woke response group. I conclude from that using their Send Feedback from within Visual Studio 2022 as a complete waste of your time.



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
