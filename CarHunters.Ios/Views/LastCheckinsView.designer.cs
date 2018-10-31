using UIKit;
using CoreGraphics;
using Cirrious.FluentLayouts.Touch;
using CarHunters.Ios.Helpers;
using CarHunters.Ios.CustomViews;
namespace CarHunters.Ios.Views
{
    partial class LastCheckinsView
    {
        UITableView _tableView;
        MainBottomBiew _bottomView;
        UIButton _huntButton;

        protected override void InitView()
        {
            base.InitView();

            _tableView = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.White,
                TableFooterView = new UIView(CGRect.Empty),
                TableHeaderView = new UIView(CGRect.Empty),
                RowHeight = UITableView.AutomaticDimension,
                EstimatedRowHeight = 100f
            };

            _bottomView = new MainBottomBiew
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _huntButton = new UIButton()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Red
            };
        }

        protected override void ConstraintView()
        {
            base.ConstraintView();

            View.AddSubviews(_tableView, _huntButton, _bottomView);
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            View.AddConstraints(
                _tableView.Top().EqualTo().TopOf(View),
                _tableView.Leading().EqualTo().LeadingOf(View),
                _tableView.Trailing().EqualTo().TrailingOf(View),
                _tableView.Bottom().EqualTo().BottomOf(View),

                _huntButton.WithSameCenterX(View),
                _huntButton.Width().EqualTo(100),
                _huntButton.Height().EqualTo(100),
                _huntButton.WithSameCenterY(View),

                _bottomView.Bottom().EqualTo().BottomOf(View),
                _bottomView.Leading().EqualTo().LeadingOf(View),
                _bottomView.Trailing().EqualTo().TrailingOf(View)
            );
        }

        protected override void DoViewDidLoad()
        {
            base.DoViewDidLoad();
            NavigationController.SetInnerNavigationControllerStyle();
        }
    }
}
