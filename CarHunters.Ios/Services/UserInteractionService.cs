using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models.UserInteraction;
using CarHunters.Core.PlatformAbstractions;
using UIKit;

namespace CarHunters.Ios.Services
{
    public class UserInteractionService : IUserInteractionService
    {
		public void Confirm(string message, Action okClicked, string title = "", string okButton = "OK", string cancelButton = "Cancel", bool cancellable = true)
		{
			Confirm(message, confirmed =>
			{
				if (confirmed)
					okClicked();
			},
			title, okButton, cancelButton, cancellable);
		}

		public void Confirm(string message, Action<bool> answer, string title = "", string okButton = "OK", string cancelButton = "Cancel", bool cancellable = true)
		{
			UIApplication.SharedApplication.InvokeOnMainThread(() =>
			{
                var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create(okButton, UIAlertActionStyle.Default, okAction => answer(true)));
                alertController.AddAction(UIAlertAction.Create(cancelButton, UIAlertActionStyle.Cancel, okAction => answer(false)));

                PresentAlertController(alertController);
			});
		}

		public Task<bool> ConfirmAsync(string message, string title = "", string okButton = "OK", string cancelButton = "Cancel", bool cancellable = true)
		{
			var tcs = new TaskCompletionSource<bool>();
			Confirm(message, (r) => tcs.TrySetResult(r), title, okButton, cancelButton, cancellable);
			return tcs.Task;
		}

		public void Alert(string message, Action done = null, string title = "", string okButton = "OK")
		{
			UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create(okButton, UIAlertActionStyle.Default, (obj) => { done?.Invoke(); }));

                PresentAlertController(alertController);
			});
		}

		public Task AlertAsync(string message, string title = "", string okButton = "OK")
		{
			var tcs = new TaskCompletionSource<object>();
			Alert(message, () => tcs.TrySetResult(null), title, okButton);
			return tcs.Task;
		}

		public void Input(string message, Action<string> okClicked, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
		{
			Input(message, (ok, text) =>
			{
				if (ok)
					okClicked(text);
			},
				placeholder, title, okButton, cancelButton, initialText);
		}

		public void Input(string message, Action<bool, string> answer, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
		{
			UIApplication.SharedApplication.InvokeOnMainThread(() =>
			{
                var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

                UITextField field = null;
                alertController.AddTextField((textField) => {
                    field = textField;
                    field.Placeholder = placeholder;
                    field.Text = initialText;
                    field.AutocorrectionType = UITextAutocorrectionType.No;
                    field.KeyboardType = UIKeyboardType.Default;
                    field.ReturnKeyType = UIReturnKeyType.Done;
                    field.ClearButtonMode = UITextFieldViewMode.WhileEditing;

                });

                alertController.AddAction(UIAlertAction.Create(okButton, UIAlertActionStyle.Default, actionOk => answer(true, field.Text)));
                alertController.AddAction(UIAlertAction.Create(okButton, UIAlertActionStyle.Cancel, actionCancel => answer(false, null)));
                PresentAlertController(alertController);
			});
		}

		public Task<InputResponse> InputAsync(string message, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", string initialText = null)
		{
			var tcs = new TaskCompletionSource<InputResponse>();
			Input(message, (ok, text) => tcs.TrySetResult(new InputResponse { Ok = ok, Text = text }), placeholder, title, okButton, cancelButton, initialText);
			return tcs.Task;
		}

		public void Selector(List<SelectorItem> items, Action<SelectorItem> selector, string title = null, string cancelButton = "Cancel")
		{
			UIAlertController alertController = UIAlertController.Create(title, null, UIAlertControllerStyle.ActionSheet);

			foreach (var item in items)
			{
                alertController.AddAction(UIAlertAction.Create(item.Text, UIAlertActionStyle.Default, (obj) => selector(items.ElementAt(item.Id))));
			}
			alertController.AddAction(UIAlertAction.Create(cancelButton, UIAlertActionStyle.Cancel, (obj) => { }));

            PresentAlertController(alertController);
		}

        public void Selector(List<SelectorItem> items, Action<SelectorItem> selector, SelectorItem cancel, string title = null, string cancelButton = "Cancel")
        {
            UIAlertController alertController = UIAlertController.Create(title, null, UIAlertControllerStyle.ActionSheet);

            foreach (var item in items)
            {
                alertController.AddAction(UIAlertAction.Create(item.Text, UIAlertActionStyle.Default, (obj) => selector(items.ElementAt(item.Id))));
            }
            alertController.AddAction(UIAlertAction.Create(cancelButton, UIAlertActionStyle.Cancel, (obj) => selector(cancel)));

            PresentAlertController(alertController);
        }

        void PresentAlertController(UIAlertController alertController)
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var controller = window.RootViewController;
            while (controller.PresentedViewController != null)
            {
                controller = controller.PresentedViewController;
            }

            UIPopoverPresentationController presentationPopover = alertController.PopoverPresentationController;
            controller = controller.PresentedViewController ?? controller;
            if (presentationPopover != null)
            {
                presentationPopover.SourceView = controller.View;
                presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
            }

            controller.PresentViewController(alertController, true, null);
        }
    }
}
