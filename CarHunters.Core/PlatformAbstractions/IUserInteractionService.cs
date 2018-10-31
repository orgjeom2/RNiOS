using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models.UserInteraction;

namespace CarHunters.Core.PlatformAbstractions
{
    public interface IUserInteractionService
    {
		void Confirm(string message, Action okClicked, string title = null, string okButton = "OK", string cancelButton = "Cancel", bool cancellable = true);
		void Confirm(string message, Action<bool> answer, string title = null, string okButton = "OK", string cancelButton = "Cancel", bool cancellable = true);
		Task<bool> ConfirmAsync(string message, string title = "", string okButton = "OK", string cancelButton = "Cancel", bool cancellable = true);

		void Alert(string message, Action done = null, string title = "", string okButton = "OK");
		Task AlertAsync(string message, string title = "", string okButton = "OK");

		void Selector(List<SelectorItem> items, Action<SelectorItem> selector, string title = null, string cancelButton = "Cancel");
        void Selector(List<SelectorItem> items, Action<SelectorItem> selector, SelectorItem cancel, string title = null,
            string cancelButton = "Cancel");
    }
}
