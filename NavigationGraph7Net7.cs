using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System;

namespace com.companyname.navigationgraph7net7
{
    [Application]
    public class NavigationGraph7ApplicationNet7 : Application, Application.IActivityLifecycleCallbacks
    {

        protected NavigationGraph7ApplicationNet7(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }
        public NavigationGraph7ApplicationNet7() { }

        public override void OnCreate()
        {
            base.OnCreate();
            // This screws up when using the SplashScreen Api - looks like an Android bug. So we use DynamicColors.ApplyToActivityIfAvailable in BaseActivity since we only have one activity. - note the singular, rather than the plural as is here  
            //DynamicColors.ApplyToActivitiesIfAvailable(this);
            RegisterActivityLifecycleCallbacks(this);
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        // This fires before OnActivityDestroyed
        //public override void OnTrimMemory([GeneratedEnum] TrimMemory level)
        //{
        //    base.OnTrimMemory(level);
        //    if (level == TrimMemory.UiHidden)
        //    {
        //        // stop your service here
        //        //StopService(new Intent(this, typeof(MyService)));
        //        // ...
        //    }
        //}

        public void OnActivityCreated(Activity activity, Bundle? savedInstanceState) { }


        public void OnActivityDestroyed(Activity activity)
        {
            if (activity is MainActivity &!activity.IsFinishing)
                activity.Finish();
        }


        public void OnActivityPaused(Activity activity) { }


        public void OnActivityResumed(Activity activity) { }


        public void OnActivitySaveInstanceState(Activity activity, Bundle outState) { }


        public void OnActivityStarted(Activity activity) { }


        public void OnActivityStopped(Activity activity) { }

    }
}