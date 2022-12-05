using Android.OS;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.View;
using AndroidX.Fragment.App;
using AndroidX.Navigation;
using Google.Android.Material.CheckBox;
using Google.Android.Material.MaterialSwitch;

namespace com.companyname.navigationgraph7net7.Fragments
{
    public class WidgetsFragment : Fragment, IOnApplyWindowInsetsListener
    {
        private NavFragmentOnBackPressedCallback? onBackPressedCallback;
        private ConstraintLayout? widgetsConstraintLayout;
        private int initialPaddingBottom;

        public WidgetsFragment( ) { }

        #region OnCreateView
        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            View? view =inflater.Inflate(Resource.Layout.fragment_widgets, container, false);

            widgetsConstraintLayout = view!.FindViewById<ConstraintLayout>(Resource.Id.widgets_constraint);
            MaterialCheckBox? materialCheckBox = view!.FindViewById<MaterialCheckBox>(Resource.Id.checkBox1);
            MaterialSwitch? materialSwitch = view.FindViewById<MaterialSwitch>(Resource.Id.switch1);

            ViewCompat.SetOnApplyWindowInsetsListener(widgetsConstraintLayout!, this);
            initialPaddingBottom = widgetsConstraintLayout!.PaddingBottom;

            materialCheckBox!.Checked = true;
            materialSwitch!.Checked = true;
            return view;
        }
        #endregion

        #region OnResume
        public override void OnResume()
        {
            base.OnResume();
           
            onBackPressedCallback = new NavFragmentOnBackPressedCallback(this, true);
            RequireActivity().OnBackPressedDispatcher.AddCallback(ViewLifecycleOwner, onBackPressedCallback);
        }
        #endregion

        #region OnDestroy
        public override void OnDestroy()
        {
            onBackPressedCallback?.Remove();
            base.OnDestroy();
        }
        #endregion

        #region HandleBackPressed
        public void HandleBackPressed(NavOptions navOptions)
        {
            onBackPressedCallback!.Enabled = false;

            NavController navController = Navigation.FindNavController(Activity!, Resource.Id.nav_host);

            // Navigate back to the HomeFragment
            navController.PopBackStack(Resource.Id.home_fragment, false);
            navController.Navigate(Resource.Id.home_fragment, null, navOptions);

        }
        #endregion

        #region OnApplyWindowInsets
        public WindowInsetsCompat OnApplyWindowInsets(View v, WindowInsetsCompat insets)
        {
            // Need to keep the FloatingActionButton above the NavigationBar in Landscape Mode so we pad the view
            if (v is ConstraintLayout)
            {
                AndroidX.Core.Graphics.Insets navigationBarsInsets = insets.GetInsets(WindowInsetsCompat.Type.NavigationBars());
                v.SetPadding(v.PaddingLeft, v.PaddingTop, v.PaddingRight, initialPaddingBottom + navigationBarsInsets.Bottom);
            }
            return insets;
        }
        #endregion
    }
}