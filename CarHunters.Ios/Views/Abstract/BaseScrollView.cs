using Cirrious.FluentLayouts.Touch;
using CarHunters.Core.ViewModels.Abstract;
using UIKit;

namespace CarHunters.Ios.Views.Abstract
{
    public abstract class BaseScrollView<TViewModel> : BaseView<TViewModel> where TViewModel : BaseViewModel
    {
        protected UIView ContentView { get; private set; }
        protected UIScrollView ScrollView { get; private set; }

        protected override void DoViewDidLoad()
        {
            base.DoViewDidLoad();
            InitScrollView();
            InitContentViewConstraints();
        }

        void InitScrollView()
        {
            ScrollView = new UIScrollView() { TranslatesAutoresizingMaskIntoConstraints = false, Bounces = false };
            ContentView = new UIView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
            };

            View.AddSubview(ScrollView);
            View.AddConstraints(
                ScrollView.Top().EqualTo().TopOf(View),
                ScrollView.Leading().EqualTo().LeadingOf(View),
                ScrollView.Trailing().EqualTo().TrailingOf(View),
                ScrollView.Bottom().EqualTo().BottomOf(View)
                );
            ScrollView.AddSubview(ContentView);
        }

        public virtual void InitContentViewConstraints()
        {
            ScrollView.AddConstraints(
                ContentView.AtTopOf(ScrollView),
                ContentView.AtBottomOf(ScrollView),
                ContentView.WithSameWidth(ScrollView),
                ContentView.WithSameCenterX(ScrollView)
                );
        }
    }

}
