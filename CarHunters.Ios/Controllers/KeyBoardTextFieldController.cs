﻿using System;
using CoreGraphics;
using Foundation;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using UIKit;
using ViewExtensions = CarHunters.Ios.Extensions.ViewExtensions;

namespace CarHunters.Ios.Controllers
{
    public abstract class KeyBoardTextFieldController<TViewModel> : MvxViewController<TViewModel> where TViewModel : class, IMvxViewModel
    {
        private NSObject _keyBoardWillShow;
		private NSObject _keyBoardWillHide;
		protected UIView ViewToCenterOnKeyboardShown;

		protected KeyBoardTextFieldController()
		{

		}

		protected KeyBoardTextFieldController(string nibName, NSBundle bundle) : base(nibName, bundle)
		{
		}

		public virtual bool HandlesKeyboardNotifications
		{
			get { return false; }
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			if (HandlesKeyboardNotifications)
			{
				RegisterForKeyboardNotifications();
			}
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			if (HandlesKeyboardNotifications)
			{
				UnregisterForKeyboardNotifications();
			}
		}

		protected virtual void RegisterForKeyboardNotifications()
		{
			_keyBoardWillHide = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
			_keyBoardWillShow = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
		}

		protected virtual void UnregisterForKeyboardNotifications()
		{
			if (!IsViewLoaded)
			{
				return;
			}

			View.EndEditing(false);
			
			NSNotificationCenter.DefaultCenter.RemoveObserver(_keyBoardWillShow);
			_keyBoardWillShow.Dispose();
			_keyBoardWillShow = null;
			NSNotificationCenter.DefaultCenter.RemoveObserver(_keyBoardWillHide);
			_keyBoardWillHide.Dispose();
			_keyBoardWillHide = null;
		}

		/// <summary>
		/// Gets the UIView that represents the "active" user input control (e.g. textfield, or button under a text field)
		/// </summary>
		/// <returns>
		/// A <see cref="UIView"/>
		/// </returns>
		protected virtual UIView KeyboardGetActiveView()
		{
			return View.FindFirstResponder();
		}

		private void OnKeyboardNotification(NSNotification notification)
		{
			if (!IsViewLoaded) return;

			//Check if the keyboard is becoming visible
            var visible = notification.Name == UIKeyboard.WillShowNotification;

			//Start an animation, using values from the keyboard
			UIView.BeginAnimations("AnimateForKeyboard");
			UIView.SetAnimationBeginsFromCurrentState(true);
			UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
			UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

			//Pass the notification, calculating keyboard height, etc.
			bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
			var keyboardFrame = visible
				? UIKeyboard.FrameEndFromNotification(notification)
				: UIKeyboard.FrameBeginFromNotification(notification);

			OnKeyboardChanged(visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);

			//Commit the animation
			UIView.CommitAnimations();
		}

		/// <summary>
		/// Override this method to apply custom logic when the keyboard is shown/hidden
		/// </summary>
		/// <param name='visible'>
		/// If the keyboard is visible
		/// </param>
		/// <param name='keyboardHeight'>
		/// Calculated height of the keyboard (width not generally needed here)
		/// </param>
		protected virtual void OnKeyboardChanged(bool visible, nfloat keyboardHeight)
		{
			var activeView = ViewToCenterOnKeyboardShown = KeyboardGetActiveView();
			if (activeView == null)
				return;

			var scrollView = ViewExtensions.FindSuperviewOfType(activeView, View, KeyboardElementType) as UIScrollView;
			if (scrollView == null)
				return;

			if (!visible)
				RestoreScrollPosition(scrollView);
			else
				CenterViewInScroll(activeView, scrollView, keyboardHeight);
		}

		public virtual Type KeyboardElementType { get { return typeof(UIScrollView); } }
		public nint VerticalSpacingFromKeyboard { get; set; } = 10;

		protected virtual void CenterViewInScroll(UIView viewToCenter, UIScrollView scrollView, nfloat keyboardHeight)
		{
			var contentInsets = new UIEdgeInsets(0.0f, 0.0f, keyboardHeight, 0.0f);
			scrollView.ContentInset = contentInsets;
			scrollView.ScrollIndicatorInsets = contentInsets;

			// Position of the active field relative isnside the scroll view
			// If activeField is hidden by keyboard, scroll it so it's visible
			CGRect viewRectAboveKeyboard = new CGRect(View.Frame.Location, new CGSize(View.Frame.Width, View.Frame.Size.Height - keyboardHeight));

			CGRect activeFieldAbsoluteFrame = ViewToCenterOnKeyboardShown.Superview.ConvertRectToView(ViewToCenterOnKeyboardShown.Frame, View);
			// activeFieldAbsoluteFrame is relative to this.View so does not include any scrollView.ContentOffset
			//fix y position
			if (activeFieldAbsoluteFrame.Y < viewRectAboveKeyboard.Y)
				activeFieldAbsoluteFrame.Y += viewRectAboveKeyboard.Y;
			else
				activeFieldAbsoluteFrame.Y += 5f;

			// Check if the activeField will be partially or entirely covered by the keyboard
			if (!viewRectAboveKeyboard.Contains(activeFieldAbsoluteFrame))
			{
				ScrollToPointKeyboardAction(scrollView, activeFieldAbsoluteFrame, viewRectAboveKeyboard, keyboardHeight);
			}
		}

		protected virtual void ScrollToPointKeyboardAction(UIScrollView scrollView, CGRect activeFieldAbsoluteFrame, CGRect viewRectAboveKeyboard, nfloat keyboardHeight)
		{
			// Scroll to the activeField Y position + activeField.Height + current scrollView.ContentOffset.Y - the keyboard Height
			CGPoint scrollPoint = new CGPoint(0.0f, activeFieldAbsoluteFrame.Location.Y + activeFieldAbsoluteFrame.Height + scrollView.ContentOffset.Y - viewRectAboveKeyboard.Height + VerticalSpacingFromKeyboard);
			scrollView.SetContentOffset(scrollPoint, true);
		}

		protected virtual void RestoreScrollPosition(UIScrollView scrollView)
		{
			scrollView.ContentInset = UIEdgeInsets.Zero;
			//          scrollView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
		}

		/// <summary>
		/// Call it to force dismiss keyboard when background is tapped
		/// </summary>
		protected virtual void DismissKeyboardOnBackgroundTap(bool cancelsTouchesInView = true)
		{
			// Add gesture recognizer to hide keyboard
			var tap = new UITapGestureRecognizer { CancelsTouchesInView = cancelsTouchesInView };
			tap.AddTarget(() => View.EndEditing(true));
			View.AddGestureRecognizer(tap);
		}
	}
}