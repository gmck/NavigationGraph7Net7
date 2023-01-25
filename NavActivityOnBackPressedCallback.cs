using AndroidX.Activity;
using AndroidX.AppCompat.App;

namespace com.companyname.navigationgraph7net7
{
    public class NavActivityOnBackPressedCallback : OnBackPressedCallback
    {
        private AppCompatActivity activity;
        private bool shouldCallFinish;

        public bool ShouldCallFinish { get => shouldCallFinish; set => shouldCallFinish = value; }

        public NavActivityOnBackPressedCallback(AppCompatActivity activity, bool enabled) : base(enabled)
        {
            this.activity = activity;
            shouldCallFinish = false;
        }

        public override void HandleOnBackPressed()
        {
            if (activity is MainActivity mainActivity)
                mainActivity.HandleOnBackPressed();
        }
    }
}
