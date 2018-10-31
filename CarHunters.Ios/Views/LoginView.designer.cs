using Foundation;
using UIKit;
using CarHunters.Core.Resources;
using CarHunters.Ios.Extensions;
using Cirrious.FluentLayouts.Touch;
using Facebook.LoginKit;
using CarHunters.Ios.Helpers;
using CarHunters.Ios.CustomViews;

namespace CarHunters.Ios.Views
{
    partial class LoginView
    {
        public override bool ShowActivityIndicator => true;

        UILabel _anonymousLogin;
        FacebookLoginButton _facebookLogin;

        protected override void InitView()
        {
            base.InitView();

            _anonymousLogin = new UILabel
            {
                Lines = 0,
                Text = Translator.GetText("AnonymousLoginString")
            }.ChangeLabelStyle(UIFont.SystemFontOfSize(17f, UIFontWeight.Regular),
                               17f, UIColor.Black, false, UITextAlignment.Center);

            _facebookLogin = new FacebookLoginButton() { TranslatesAutoresizingMaskIntoConstraints = false };
        }

        protected override void ConstraintView()
        {
            base.ConstraintView();

            View.AddSubviews(_anonymousLogin, _facebookLogin);
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            View.AddConstraints(
                _facebookLogin.Bottom().EqualTo(-10f).TopOf(_anonymousLogin),
                _facebookLogin.WithSameCenterX(View),

                _anonymousLogin.Leading().EqualTo(15f).LeadingOf(View),
                _anonymousLogin.Trailing().EqualTo(-15f).TrailingOf(View),
                _anonymousLogin.WithSameCenterX(View),
                _anonymousLogin.WithSameCenterY(View)
            );
        }

        protected override void DoViewDidLoad()
        {
            base.DoViewDidLoad();
        }
    }
}