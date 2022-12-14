using AndroidX.Activity;
using AndroidX.AppCompat.App;

namespace com.companyname.navigationgraph7net7
{

    public class NavigationGraphOnBackPressedCallback : OnBackPressedCallback
    {
        private readonly AppCompatActivity? activity;
        public NavigationGraphOnBackPressedCallback(AppCompatActivity activity, bool enabled) : base(enabled)
        {
            this.activity = activity;
        }


        public override void HandleOnBackPressed()
        {
            if (activity is MainActivity mainActivity)
                mainActivity.HandleOnBackPressed();
        }
    }
}
