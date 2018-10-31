using System;
using MvvmCross.Plugin.Messenger;
namespace CarHunters.Core.Common.Models.Messages
{
    public class ChangeStateAppMessage : MvxMessage
    {
        public bool InBackground { get; }

        public ChangeStateAppMessage(object sender, bool inBackground) : base(sender)
        {
            InBackground = inBackground;
        }
    }
}
