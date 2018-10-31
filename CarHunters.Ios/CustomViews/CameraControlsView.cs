using UIKit;
using Cirrious.FluentLayouts.Touch;

namespace CarHunters.Ios.CustomViews
{
    public class CameraControlsView : UIView
    {
        public UIButton GoBackButton;

        public CameraControlsView()
        {
            InitView();
            ConstrainView();
        }

        void InitView()
        {
            GoBackButton = new UIButton
            {
                BackgroundColor = UIColor.Red,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
        }

        void ConstrainView()
        {
            AddSubview(GoBackButton);
            this.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            this.AddConstraints(
                GoBackButton.Height().EqualTo(40),
                GoBackButton.Width().EqualTo(40),
                GoBackButton.Bottom().EqualTo(-20).BottomOf(this),
                GoBackButton.WithSameCenterX(this).WithMultiplier(0.25f)
            );
        }
    }
}