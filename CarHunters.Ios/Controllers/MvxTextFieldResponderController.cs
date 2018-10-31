using System;
using MvvmCross.ViewModels;
using UIKit;

namespace CarHunters.Ios.Controllers
{
    public abstract class MvxTextFieldResponderController<TViewModel> : KeyBoardTextFieldController<TViewModel> where TViewModel : class, IMvxViewModel
    {
        protected MvxTextFieldResponderController()
        {

        }

        protected MvxTextFieldResponderController(string nibName, Foundation.NSBundle bundle) : base(nibName, bundle)
        {
        }

        private UITextField[] formTextfields;

        protected void EnableNextKeyForTextFields(params UITextField[] fields)
        {
            formTextfields = fields;

            foreach (var field in fields)
            {
                field.ShouldReturn += ShouldReturn;
            }
        }

        private bool ShouldReturn(UITextField textField)
        {
            int index = Array.IndexOf(formTextfields, textField);

            if (index > -1 && index < formTextfields.Length - 1)
            {
                formTextfields[index + 1].BecomeFirstResponder();
                return true;
            }
            else if (index == formTextfields.Length - 1)
            {
                formTextfields[index].ResignFirstResponder();
                FormFinished();
            }

            return false;
        }

        protected virtual void FormFinished()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && formTextfields != null)
            {
                foreach (var item in formTextfields)
                {
                    item.ShouldReturn -= ShouldReturn;
                }
                formTextfields = null;
            }
            base.Dispose(disposing);
        }
    }
}