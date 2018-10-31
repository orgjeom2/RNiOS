using UIKit;
using CoreAnimation;
using CarHunters.Ios.Helpers;
using CarHunters.Ios.Extensions;
using CoreGraphics;

namespace CarHunters.Ios.CustomViews
{
    public class HuntRoundedButton : UIButton
    {
        public HuntRoundedButton(){}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var gradientLayer = new CAGradientLayer
            {
                Frame = Bounds,
                Colors = new[]
                {
                    Theme.HuntButtonGradientStartColor.ToUIColor().CGColor,
                    Theme.HuntButtonGradientEndColor.ToUIColor().CGColor
                },
                StartPoint = new CGPoint(0.0, 0.0),
                EndPoint = new CGPoint(1.0, 1.0f),
                CornerRadius = 30f
            };
            UIBezierPath bezierPath = UIBezierPath.FromRoundedRect(Bounds, 30f);
            Layer.CornerRadius = 30f;
            Layer.MasksToBounds = false;
            Layer.ShadowColor = UIColor.Black.CGColor;
            Layer.ShadowOffset = new CGSize(1,0);
            Layer.ShadowOpacity = 0.5f;
            Layer.ShadowPath = bezierPath.CGPath;
            Layer.InsertSublayer(gradientLayer, 0);
        }
    }
}
