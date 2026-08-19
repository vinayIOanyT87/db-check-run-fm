using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
    public abstract class FMCollectionBaseBindingList<ConsolidatedObjectType> : CollectionBase, IBindingList, IEnumerable<ConsolidatedObjectType>
    {
        // Provides support for LINQ
        public new IEnumerator<ConsolidatedObjectType> GetEnumerator()
        {
            foreach (ConsolidatedObjectType X in this.List)
            {
                yield return X;
            }
        }

        // Supported Features
        bool IBindingList.SupportsSorting { get { return true; } }

        // Unsupported Features
        #region Unsupported Features
        bool IBindingList.AllowEdit { get { return false; } }
        bool IBindingList.AllowNew { get { return false; } }
        bool IBindingList.AllowRemove { get { return false; } }
        bool IBindingList.SupportsChangeNotification { get { return false; } }
        bool IBindingList.SupportsSearching { get { return false; } }
        object IBindingList.AddNew() { throw new NotSupportedException(); }
        void IBindingList.AddIndex( PropertyDescriptor property ) { throw new NotSupportedException(); }
        int IBindingList.Find( PropertyDescriptor property, object key ) { throw new NotSupportedException(); }
        void IBindingList.RemoveIndex( PropertyDescriptor property ) { throw new NotSupportedException(); }
        PropertyDescriptor IBindingList.SortProperty { get { return null; } }
        #endregion

        private bool isSorted = false;
        bool IBindingList.IsSorted { get { return isSorted; } }

        private ListChangedEventHandler onListChanged;
        public event ListChangedEventHandler ListChanged
        {
            add { onListChanged += value; }
            remove { onListChanged -= value; }
        }

        private ListSortDirection sortDirection;
        ListSortDirection IBindingList.SortDirection { get { return sortDirection; } }

        void IBindingList.ApplySort( PropertyDescriptor property, ListSortDirection direction )
        {
            isSorted = true;
            sortDirection = direction;

            ArrayList newArray = new ArrayList();

            // This is a bit silly having to do it this way but I ran out of time trying
            // to find how to specify the direction programatically.  
            if (direction == ListSortDirection.Ascending)
            {
                var sortedList = from E in this
                                 orderby property.GetValue( E )
                                 select E;

                foreach (var E in sortedList)
                {
                    newArray.Add( E );
                }
            }
            else
            {
                var sortedList = from E in this
                                 orderby property.GetValue( E ) descending
                                 select E;

                foreach (var E in sortedList)
                {
                    newArray.Add( E );
                }
            }

            this.Clear();
            foreach (var E in newArray)
            {
                //Add( E );
                InnerList.Add( E );
            }


        }

        void IBindingList.RemoveSort()
        {
            isSorted = false;

            // Revert to default sort
            Sort();
        }

        public virtual void Sort()
        {
            this.InnerList.Sort();
        }

    }

}