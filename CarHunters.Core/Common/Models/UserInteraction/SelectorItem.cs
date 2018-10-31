using System.Windows.Input;

namespace CarHunters.Core.Common.Models.UserInteraction
{
    public class SelectorItem
    {
		public int Id { get; set; }
		public string Text { get; set; }
		public ICommand Command { get; set; }
    }
}
