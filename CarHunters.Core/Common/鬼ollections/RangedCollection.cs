using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarHunters.Core.Common.Extensions;
using CarHunters.Core.Common.Models;
using MvvmCross.ViewModels;

namespace CarHunters.Core.Common.Сollections
{
    public class RangedCollection<TPOCO, TView> : MvxObservableCollection<TPOCO> where TPOCO : RangedCollectionPOCO<TView>, new()
    {
        readonly IComparer<TPOCO> _comparare;
        readonly object _locker = new object();

        public RangedCollection(IComparer<TPOCO> comparable) : this(new List<TPOCO>(), comparable)
        {
        }

        public RangedCollection(IEnumerable<TPOCO> collection, IComparer<TPOCO> comparable)
        {
            _comparare = comparable;
            SortAndInsertPOCO(collection.ToList());
        }

        public RangedCollection(IEnumerable<TView> collection, IComparer<TPOCO> comparable)
        {
            _comparare = comparable;
            HandleRange(collection);
        }

        public Task HandleRange(IEnumerable<TView> list)
        {
           return InvokeOnMainThread(() =>
            {
                lock(_locker)
                {
                    var items = list as IList<TView> ?? list.ToList();
                    if (TryToInsertAll(items))
                        return;

                    foreach (var item in items)
                    {
                        var foundItem = this.FirstOrDefault(x => x.Equals(item));
                        if (foundItem == null)
                        {
                            var certainObj = new TPOCO();
                            certainObj.Init(item);

                            int index = this.BinarySearch(certainObj, _comparare);
                            if (index < 0 && ~index <= Count)
                            {
                                Insert(~index, certainObj);
                            }
                        }
                        else if (foundItem.NeedUpdate(item))
                            FindAndInsertItem(foundItem, item);
                    }
                }
            });
        }

        public IEnumerable<TPOCO> Search(Func<TPOCO, bool> predicate)
        { 
            lock (_locker)
            {
                return this.Where(predicate).ToList();
            }
        }

        public void RefreshItem(TPOCO obj)
        {
            InvokeOnMainThread(() =>
            {
                lock (_locker)
                {
                    var item = IndexOf(obj);
                    if (item == -1)
                        return;

                    Remove(obj);
                    var newIndex = this.BinarySearch(obj, _comparare);
                    if (newIndex < 0)
                        Insert(~newIndex, obj);
                }
            });
        }

        public Task Remove(IEnumerable<string> ids)
        {
          return InvokeOnMainThread(() =>
            {
                lock (_locker)
                {
                    foreach (var id in ids)
                    {
                        var foundItem = this.FirstOrDefault(x => x.Id == id);
                        if (foundItem != null)
                            Remove(foundItem);
                    }
                }
            });
        }

        public new void Clear()
        {
            InvokeOnMainThread(() => 
            {
                lock (_locker)
                    base.Clear();
            });
        }

        bool TryToInsertAll(IEnumerable<TView> list)
        {
            var items = list as IList<TView> ?? list.ToList();
            if (Count == 0 && items.Count > 1)
            {
                SortAndInsertPOCO(items.Select(x =>
                {
                    var item = new TPOCO();
                    item.Init(x);
                    return item;
                }).ToList());
                return true;
            }

            return false;
        }

        void SortAndInsertPOCO(List<TPOCO> list)
        { 
            list.Sort(_comparare);
            AddRange(list);
        }

        void FindAndInsertItem(TPOCO foundItem, TView newItem)
        {
            var oldIndex = IndexOf(foundItem);

            RemoveAt(oldIndex);

            var item = new TPOCO();
            item.Init(newItem);

            var newIndex = this.BinarySearch(item, _comparare);
            if (newIndex < 0)
                Insert(~newIndex, item);
        }
    }
}

