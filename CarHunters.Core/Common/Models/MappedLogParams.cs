using System;
using MvvmCross.ViewModels;
using CarHunters.Core.Common.Extensions;
namespace CarHunters.Core.Common.Models
{
    public class MappedLogParams : MvxNotifyPropertyChanged
    {
        public string Text { get; set; }
        public CustomColor Color { get; set; }

        public MappedLogParams(){}
        public MappedLogParams(LogParams logParams)
        {
            Text = logParams.Text;
            Color = logParams.Color.ToOurColor();
        }
    }
}
