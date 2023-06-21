using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.Navigation;
using AndroidX.Preference;
using com.companyname.navigationgraph7net7.Dialogs;

namespace com.companyname.navigationgraph7net7.Fragments
{
    public class PurchaseFragment : Fragment
    {
        private NavFragmentOnBackPressedCallback? onBackPressedCallback;
        private bool showSubscriptionExplanationDialog;
        
        public PurchaseFragment() { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            View? view = inflater.Inflate(Resource.Layout.fragment_purchase, container, false);
            TextView? textView = view!.FindViewById<TextView>(Resource.Id.text_purchase);
            textView!.Text = "This is purchase fragment";

            ISharedPreferences? sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity!);
            showSubscriptionExplanationDialog = sharedPreferences!.GetBoolean("showSubscriptionExplanationDialog", true);

            return view;
        }
        #endregion

        #region OnResume
        public override void OnResume()
        {
            base.OnResume();

            onBackPressedCallback = new NavFragmentOnBackPressedCallback(this, true);
            //// Android docs:  Strongly recommended to use the ViewLifecycleOwner.This ensures that the OnBackPressedCallback is only added when the LifecycleOwner is Lifecycle.State.STARTED.
            //// The activity also removes registered callbacks when their associated LifecycleOwner is destroyed, which prevents memory leaks and makes it suitable for use in fragments or other lifecycle owners
            //// that have a shorter lifetime than the activity.
            //// Note: this rule out using OnAttach(Context context) as the view hasn't been created yet.
            //RequireActivity().OnBackPressedDispatcher.AddCallback(ViewLifecycleOwner, onBackPressedCallback);

            if (showSubscriptionExplanationDialog)
                ShowSubscriptionExplanationDialog();

        }
        #endregion

        #region OnDestroy
        public override void OnDestroy()
        {
            onBackPressedCallback?.Remove();
            base.OnDestroy();
        }
        #endregion

        #region HandleOnBackPressed
        public void HandleOnBackPressed(NavOptions navOptions)
        {
            onBackPressedCallback!.Enabled = false;

            NavController navController = Navigation.FindNavController(Activity!, Resource.Id.nav_host);

            // Navigate back to the HomeFragment
            //navController.PopBackStack(Resource.Id.home_fragment, false);

            // You can always change to this, so that it doesn't animate for the immersive version
            navController.Navigate(Resource.Id.home_fragment, null, navOptions);
        }
        #endregion

        #region ShowSubscriptionExplanationDialog
        internal void ShowSubscriptionExplanationDialog()
        {
            // Make sure the the MaterialSwitch is checked. We don't want this to pop up again after the transaction completes. Optional fourth parameter.
            string tag = "SubscriptionExplanationDialogFragment";
            string preferenceName = "showSubscriptionExplanationDialog";

            FragmentManager fm = Activity!.SupportFragmentManager;
            if (fm != null && !fm.IsDestroyed)
            {
                Fragment? fragment = fm.FindFragmentByTag(tag);
                if (fragment == null)
                    HelperExplanationDialogFragment.NewInstance(GetString(Resource.String.subscription_explanation_title),
                                                                GetString(Resource.String.subscription_explanation_text_short), preferenceName).Show(fm, tag);  
            }
        }
        #endregion
    }
}