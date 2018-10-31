using System;
using UIKit;
using CarHunters.Ios.Extensions;
using CarHunters.Core.Resources;
using Cirrious.FluentLayouts.Touch;
using CarHunters.Ios.Helpers;
namespace CarHunters.Ios.CustomViews
{
    public class FacebookLoginButton : UIView
    {
        UIImageView _facebookImage;
        UILabel _title;

        public FacebookLoginButton()
        {
            InitView();
            ConstrainView();
        }

        void InitView()
        {
            this.BackgroundColor = Theme.LoginButtonBlueColor.ToUIColor();

            _facebookImage = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.White,
                ClipsToBounds = true,
                Image = UIImage.FromBundle("FacebookIcon")
            };

            _title = new UILabel
            {
                Lines = 0,
                Text = Translator.GetText("LoginWithFacebook")
            }.ChangeLabelStyle(UIFont.SystemFontOfSize(17f, UIFontWeight.Regular),
                               17f, UIColor.White, false, UITextAlignment.Left);
        }

        void ConstrainView()
        {
            this.AddSubviews(_facebookImage, _title);
            this.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            this.AddConstraints(
                _facebookImage.WithSameCenterY(this),
                _facebookImage.Leading().EqualTo(10).LeadingOf(this),
                _facebookImage.Height().EqualTo(24),
                _facebookImage.Width().EqualTo(24),

                _title.Leading().EqualTo(5).TrailingOf(_facebookImage),
                _title.Trailing().EqualTo(-10).TrailingOf(this),
                _title.WithSameCenterY(this),

                this.Height().EqualTo(44)
            );
        }
    }
}
