using AtomAPI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtomRoomList.Models
{
    public class AtomItemBase : BindableBase
    {
        protected bool? _isSelected = new bool?(false);

        public AtomItemBase ParentItem { get; set; }
        public IList<AtomItemBase> Items { get; set; }

        public virtual bool? IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;

                if (value.HasValue)
                {
                    foreach (var item in Items)
                    {
                        item.IsSelected = new bool?(value.Value);
                    }

                    ParentItem?.TrySetSelectedNullValue();
                }

                if (Items.Count == 0)
                {
                    SelectedChangedEvent?.Invoke(this, EventArgs.Empty);
                }

                OnPropertyChanged(nameof(IsSelected));
            }
        }


        public event EventHandler SelectedChangedEvent;

        public AtomItemBase()
        {
            Items = new List<AtomItemBase>();
        }

        public virtual void TrySetSelectedNullValue()
        {
            _isSelected = !Items.All(item =>
            {
                bool? nullable = item.IsSelected;
                bool flag = true;
                return nullable.GetValueOrDefault() == flag & nullable.HasValue;
            }) ? (!Items.All(item =>
            {
                bool? nullable = item.IsSelected;
                bool flag = false;
                return nullable.GetValueOrDefault() == flag & nullable.HasValue;
            }) ? new bool?() : new bool?(false)) : new bool?(true);

            OnPropertyChanged(nameof(IsSelected));

            ParentItem?.TrySetSelectedNullValue();
        }
    }
}
