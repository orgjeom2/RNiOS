using System;
using MvvmCross.Platforms.Ios.Binding.Views;
using Foundation;
using UIKit;
using CarHunters.Ios.Extensions;
using Cirrious.FluentLayouts.Touch;
using MvvmCross.Binding.BindingContext;
using CarHunters.Core.Common.Models;
using CarHunters.Ios.Converters;

namespace CarHunters.Ios.Cells
{
    public class LogItemTableViewCell : MvxTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(LogItemTableViewCell));
        UILabel _logInfo;

        public LogItemTableViewCell(IntPtr handle) : base(handle)
        {
            InitCell();
            ConstrainCell();
            DoBind();
        }

        void DoBind()
        {
            var set = this.CreateBindingSet<LogItemTableViewCell, MappedLogParams>();
            set.Bind(_logInfo).For(v => v.TextColor).To(vm => vm.Color).WithConversion(nameof(CustomColorToUiColorConverter));
            set.Bind(_logInfo).To(vm => vm.Text);
            set.Apply();
        }

        void InitCell()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            ContentView.BackgroundColor = UIColor.Clear;
            BackgroundColor = UIColor.Clear;
            _logInfo = new UILabel
            {
                Lines = 0,
                BackgroundColor = UIColor.Clear,
            }.ChangeLabelStyle(UIFont.SystemFontOfSize(12f, UIFontWeight.Regular),
                               12f, UIColor.Black, false, UITextAlignment.Left);
        }

        void ConstrainCell()
        {
            ContentView.AddSubview(_logInfo);
            ContentView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            ContentView.AddConstraints(
                _logInfo.Leading().EqualTo(5).LeadingOf(ContentView),
                _logInfo.Trailing().EqualTo(-5).TrailingOf(ContentView),
                _logInfo.Top().EqualTo().TopOf(ContentView),
                _logInfo.Bottom().EqualTo().BottomOf(ContentView)
            );
        }
    }
}
