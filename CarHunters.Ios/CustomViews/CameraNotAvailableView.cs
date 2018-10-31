using System;
using UIKit;
using CarHunters.Ios.Extensions;
using CarHunters.Core.Resources;
using Cirrious.FluentLayouts.Touch;
namespace CarHunters.Ios.CustomViews
{
    public class CameraNotAvailableView : UIView
    {
        UIImageView imageView;
        UILabel label;
        public UIButton OpenSettingsButton;
        public UIButton GoBackButton;

        public CameraNotAvailableView()
        {
            InitView();
            ConstrainView();
        }

        void InitView()
        {
            BackgroundColor = UIColor.Black;

            imageView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ClipsToBounds = true,
                BackgroundColor = UIColor.LightGray
            };

            label = new UILabel
            {
                Lines = 0,
                Text = Translator.GetText("CameraPermissionRestrictedPlsGrantItThroughSettingsString")
            }.ChangeLabelStyle(UIFont.SystemFontOfSize(17f, UIFontWeight.Regular),
                               17f, UIColor.White, false, UITextAlignment.Center);

            OpenSettingsButton = new UIButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear
            };
            OpenSettingsButton.SetTitle(Translator.GetText("OpenSettingsString"), UIControlState.Normal);
            OpenSettingsButton.SetTitleColor(UIColor.White, UIControlState.Normal);

            GoBackButton = new UIButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Red
            };
        }

        void ConstrainView()
        {
            AddSubviews(imageView, label, OpenSettingsButton, GoBackButton);
            this.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            this.AddConstraints(
                imageView.Width().EqualTo(100),
                imageView.Height().EqualTo(100),
                imageView.WithSameCenterX(this),
                imageView.WithSameCenterY(this).Minus(50),

                label.Top().EqualTo(10).BottomOf(imageView),
                label.Leading().EqualTo(20).LeadingOf(this),
                label.Trailing().EqualTo(-20).TrailingOf(this),

                OpenSettingsButton.Top().EqualTo(10).BottomOf(label),
                OpenSettingsButton.WithSameCenterX(this),
                OpenSettingsButton.Height().EqualTo(44),

                GoBackButton.WithSameCenterX(this),
                GoBackButton.Width().EqualTo(50),
                GoBackButton.Height().EqualTo(50),
                GoBackButton.Top().EqualTo(10).BottomOf(OpenSettingsButton)
            );
        }
    }
}
