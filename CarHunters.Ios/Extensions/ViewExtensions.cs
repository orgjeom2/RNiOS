using System;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using UIKit;
using CoreAnimation;

namespace CarHunters.Ios.Extensions
{
    public static class ViewExtensions
    {
        /// <summary>
        /// Find the first responder in the <paramref name="view"/>'s subview hierarchy
        /// </summary>
        /// <param name="view">
        /// A <see cref="UIView"/>
        /// </param>
        /// <returns>
        /// A <see cref="UIView"/> that is the first responder or null if there is no first responder
        /// </returns>
        public static UIView FindFirstResponder(this UIView view)
        {
            if (view.IsFirstResponder)
            {
                return view;
            }
            foreach (UIView subView in view.Subviews)
            {
                var firstResponder = subView.FindFirstResponder();
                if (firstResponder != null)
                    return firstResponder;
            }
            return null;
        }

        /// <summary>
        /// Find the first Superview of the specified type (or descendant of)
        /// </summary>
        /// <param name="view">
        /// A <see cref="UIView"/>
        /// </param>
        /// <param name="stopAt">
        /// A <see cref="UIView"/> that indicates where to stop looking up the superview hierarchy
        /// </param>
        /// <param name="type">
        /// A <see cref="Type"/> to look for, this should be a UIView or descendant type
        /// </param>
        /// <returns>
        /// A <see cref="UIView"/> if it is found, otherwise null
        /// </returns>
        public static UIView FindSuperviewOfType(this UIView view, UIView stopAt, Type type)
        {
            if (view.Superview != null)
            {
                if (type.IsInstanceOfType(view.Superview))
                {
                    return view.Superview;
                }

                if (view.Superview != stopAt)
                    return view.Superview.FindSuperviewOfType(stopAt, type);
            }

            return null;
        }

        public static UILabel ChangeLabelStyle(this UILabel label, UIFont font, nfloat minimumSize, UIColor textColor, bool translateResize, UITextAlignment aligment)
        {
            label.TextAlignment = aligment;
            label.Font = font;
            label.MinimumFontSize = minimumSize;
            label.TextColor = textColor;
            label.TranslatesAutoresizingMaskIntoConstraints = translateResize;
            return label;
        }

        public static UILabel ChangeLabelStyle(this UILabel label, UIFont font, nfloat minimumSize, UIColor textColor, bool translateResize, UITextAlignment aligment, bool shrinkText)
        {
            //TODO: DEPRECATED
            label.AdjustsFontSizeToFitWidth = shrinkText;
            label.TextAlignment = aligment;
            label.Font = font;
            label.MinimumFontSize = minimumSize;
            //label.MinimumScaleFactor = 1f;
            label.TextColor = textColor;
            label.TranslatesAutoresizingMaskIntoConstraints = translateResize;
            return label;
        }

        public static UITextField ChangeTextFieldStyle(this UITextField textField, UIFont font, nfloat minimumSize, UIColor textColor, bool translateResize, UITextAlignment aligment, bool shrinkText = false)
        {
            textField.TextAlignment = aligment;
            textField.Font = font;
            textField.MinimumFontSize = minimumSize;
            textField.TextColor = textColor;
            textField.TranslatesAutoresizingMaskIntoConstraints = translateResize;
            textField.AdjustsFontSizeToFitWidth = shrinkText;
            return textField;
        }

        public static void AddFullWidthShadow(this UIView view)
        {
            view.Layer.MasksToBounds = false;
            //view.Layer.ShadowColor = ThemeManager.ShadowColor.CGColor;
            view.Layer.ShadowOffset = new CGSize(0, 1f);
            view.Layer.ShadowOpacity = 0.4f;
        }

        public static void AddBottomLine(this UIViewController controller, UIColor color, params UITextField[] fields)
        {
            foreach (var field in fields)
            {
                UIView bottomBorder = new UIView();
                bottomBorder.BackgroundColor = color;
                bottomBorder.TranslatesAutoresizingMaskIntoConstraints = false;
                field.AddSubview(bottomBorder);
                field.AddConstraints(
                    bottomBorder.WithRelativeWidth(field),
                    bottomBorder.WithSameLeft(field),
                    bottomBorder.WithSameRight(field),
                    bottomBorder.Height().EqualTo(1),
                    bottomBorder.AtBottomOf(field)
                );
            }
        }

        public static void AddBottomLine<T>(this T view, UIColor color, float height = 1) where T : UIView 
        {
            UIView bottomBorder = new UIView();
            bottomBorder.BackgroundColor = color;
            bottomBorder.TranslatesAutoresizingMaskIntoConstraints = false;
            view.AddSubview(bottomBorder);
            view.AddConstraints(
                bottomBorder.WithRelativeWidth(view),
                bottomBorder.WithSameLeft(view),
                bottomBorder.WithSameRight(view),
                bottomBorder.Height().EqualTo(height),
                bottomBorder.AtBottomOf(view)
            );
        }

        public static void AddTopLine<T>(this T view, UIColor color, float height = 1) where T : UIView
        {
            UIView bottomBorder = new UIView();
            bottomBorder.BackgroundColor = color;
            bottomBorder.TranslatesAutoresizingMaskIntoConstraints = false;
            view.AddSubview(bottomBorder);
            view.AddConstraints(
                bottomBorder.WithRelativeWidth(view),
                bottomBorder.WithSameLeft(view),
                bottomBorder.WithSameRight(view),
                bottomBorder.Height().EqualTo(height),
                bottomBorder.AtTopOf(view)
            );
        }

        public static void AddMarginedTopLine<T>(this T view, UIColor color, float height = 1, float marginLeft = 0f) where T : UIView
        {
            UIView bottomBorder = new UIView();
            bottomBorder.BackgroundColor = color;
            bottomBorder.TranslatesAutoresizingMaskIntoConstraints = false;
            view.AddSubview(bottomBorder);
            view.AddConstraints(
                bottomBorder.WithRelativeWidth(view),
                bottomBorder.Leading().EqualTo(marginLeft).LeadingOf(view),
                bottomBorder.WithSameRight(view),
                bottomBorder.Height().EqualTo(height),
                bottomBorder.AtTopOf(view)
            );
        }

        public static void AddMarginedBottomLine<T>(this T view, UIColor color, float height = 1, float marginLeft = 0f) where T : UIView
        {
            UIView bottomBorder = new UIView();
            bottomBorder.BackgroundColor = color;
            bottomBorder.TranslatesAutoresizingMaskIntoConstraints = false;
            view.AddSubview(bottomBorder);
            view.AddConstraints(
                bottomBorder.WithRelativeWidth(view),
                bottomBorder.Leading().EqualTo(marginLeft).LeadingOf(view),
                bottomBorder.WithSameRight(view),
                bottomBorder.Height().EqualTo(height),
                bottomBorder.AtBottomOf(view)
            );
        }

        public static void AddArrangedSubviews(this UIStackView stackView, params UIView[] views)
        {
            foreach(var view in views)
            {
                if (view != null)
                    stackView.AddArrangedSubview(view);
            }
        }

        public static void RemoverArrangedSubviews(this UIStackView stackView, params UIView[] views)
        {
            foreach (var view in views)
            {
                if (view != null)
                {
                    stackView.RemoveArrangedSubview(view);
                    view.RemoveFromSuperview();
                }
            }
        }

        public static void InsertArrangedSubviews(this UIStackView stackView, int startPosition = 0, params UIView[] views)
        {
            if (startPosition < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startPosition), "Index cannot be negative!");
            }

            if(startPosition == 0)
            {
                stackView.AddArrangedSubviews(views);
                return;
            }

            for (var i = 0; i < views.Length; i++)
            {
                stackView.InsertArrangedSubview(views[i], (nuint)(startPosition + i));
            }
        }

        public static void AddDashedBorder(this UIView view, UIColor color)
        {
            var shapeLayer = new CAShapeLayer();
            shapeLayer.StrokeColor = color.CGColor;
            shapeLayer.LineWidth = 3f;
            shapeLayer.FillColor = UIColor.Clear.CGColor;
            shapeLayer.Frame = new CGRect(0, 0, view.Frame.Width, view.Frame.Height);
            shapeLayer.LineDashPattern = new Foundation.NSNumber[] { 6, 6 };
            shapeLayer.Path = UIBezierPath.FromRoundedRect(new CGRect(0, 0, view.Frame.Width, view.Frame.Height), 6f).CGPath;
            view.ClipsToBounds = true;
            view.Layer.AddSublayer(shapeLayer);
        }
    }
}

