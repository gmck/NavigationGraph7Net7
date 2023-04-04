# NavigationGraph7Net7 net7.0-android33
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
