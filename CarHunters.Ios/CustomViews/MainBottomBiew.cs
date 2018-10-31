using System;
using CarHunters.Ios.Extensions;
using CarHunters.Ios.Helpers;
using CoreAnimation;
using CoreGraphics;
using UIKit;
using Cirrious.FluentLayouts.Touch;
using CarHunters.Core.Resources;
namespace CarHunters.Ios.CustomViews
{
    public class MainBottomBiew : UIView
    {
        public UILabel LeftMenuTitle;
        public HuntRoundedButton HuntButton;
        public UILabel RightMenuTitle;

        public MainBottomBiew()
        {
            InitView();
            ConstrainView();
        }

        void InitView()
        {
            LeftMenuTitle = new UILabel
            {
                Lines = 1,
                Text = Translator.GetText("LastCheckinsString"),
            }.ChangeLabelStyle(UIFont.SystemFontOfSize(15f, UIFontWeight.Regular),
                               15f, UIColor.White, false, UITextAlignment.Center);

            HuntButton = new HuntRoundedButton() 
            { 
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            RightMenuTitle = new UILabel
            {
                Lines = 1,
                Text = Translator.GetText("MyCarsString"),
            }.ChangeLabelStyle(UIFont.SystemFontOfSize(15f, UIFontWeight.Regular),
                               15f, UIColor.White, false, UITextAlignment.Center);
        }

        void ConstrainView()
        {
            this.AddSubviews(LeftMenuTitle, HuntButton, RightMenuTitle);
            this.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            this.AddConstraints(
                this.Height().LessThanOrEqualTo(100),

                LeftMenuTitle.WithSameCenterY(HuntButton),
                LeftMenuTitle.Trailing().EqualTo(-15).LeadingOf(HuntButton),
                LeftMenuTitle.Leading().EqualTo(15).LeadingOf(this),

                HuntButton.WithSameCenterX(this),
                HuntButton.Width().EqualTo(60),
                HuntButton.Height().EqualTo(60),
                HuntButton.WithSameCenterY(this).Minus(10),

                RightMenuTitle.WithSameCenterY(HuntButton),
                RightMenuTitle.Trailing().EqualTo(-15).TrailingOf(this),
                RightMenuTitle.Leading().EqualTo(15).TrailingOf(HuntButton)
            );
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var gradient = new CAGradientLayer
            {
                Frame = Bounds,
                Colors = new[]
                {
                    Theme.GradientEndGreenColor.ToUIColor().CGColor,
                    Theme.GradientStartBlueColor.ToUIColor().CGColor
                },
                StartPoint = new CGPoint(0.0, 0.5),
                EndPoint = new CGPoint(1.0, 0.5f)
            };
            Layer.InsertSublayer(gradient, 0);
        }
    }
}
