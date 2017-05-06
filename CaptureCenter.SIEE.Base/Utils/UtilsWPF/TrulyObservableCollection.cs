using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ExportExtensionCommon
{
    [Serializable]
    public sealed class TrulyObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
#pragma warning disable 0114
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0114

        public TrulyObservableCollection() { init(); }

        public TrulyObservableCollection(List<T> list)
        {
            if (list != null)
                foreach (T element in list) this.Add(element);
            init();
        }

        public TrulyObservableCollection(ObservableCollection<T> oc)
        {
            if (oc != null)
                foreach (T element in oc) this.Add(element);
            init();
        }

        public TrulyObservableCollection<T> Clone()
        {
            return new TrulyObservableCollection<T>(this);
        }

        private void init()
        {
            CollectionChanged += TrulyObservableCollection_CollectionChanged;
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            init();
            foreach (T item in this)
                (item as INotifyPropertyChanged).PropertyChanged += item_PropertyChanged;
        }

        void TrulyObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Object item in e.NewItems)
                    (item as INotifyPropertyChanged).PropertyChanged += item_PropertyChanged;

            if (e.OldItems != null)
                foreach (Object item in e.OldItems)
                    (item as INotifyPropertyChanged).PropertyChanged -= item_PropertyChanged;
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(e.PropertyName));
        }
    }
}
