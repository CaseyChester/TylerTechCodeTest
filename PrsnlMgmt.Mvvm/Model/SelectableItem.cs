namespace PrsnlMgmt.Mvvm.Model
{
    public class SelectableItem<TItem> : BindableBase
    {
        private bool _isSelected;
        private TItem _item;
        private object _sourceObject;

        public SelectableItem(TItem item)
        {
            _item = item;
            _sourceObject = null;
        }

        public SelectableItem(TItem item, object sourceObject)
        {
            _item = item;
            _sourceObject = sourceObject;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public TItem Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        public object SourceObject
        {
            get => _sourceObject;
            set => SetProperty(ref _sourceObject, value);
        }
    }
}