using MvvmCross.ViewModels;

namespace CarHunters.Core.Common.Models
{
    public abstract class RangedCollectionPOCO<T> : MvxNotifyPropertyChanged
    {
        protected RangedCollectionPOCO()
        {
            
        }

        public abstract string Id { get; set; }

        public abstract void Init(T t);

        public abstract bool NeedUpdate(T t);
        public abstract bool Equals(T t);
    }
}