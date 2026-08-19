using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	public class BaseCollections : ICollection, IComparer
	{
		#region Attributes
		protected ArrayList items;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the Base Collections class.
		/// </summary>
		public BaseCollections()
		{
			this.items = new ArrayList();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Returns the count of the number of items in the list.
		/// </summary>
		public int Count
		{
			get { return items.Count; }
		}

		/// <summary>
		/// Returns true of the items in the list are synchronized.
		/// </summary>
		public bool IsSynchronized
		{
			get { return items.IsSynchronized; }
		}

		/// <summary>
		/// Returns the synchronized root of the list.
		/// </summary>
		public object SyncRoot
		{
			get { return items.SyncRoot; }
		}

		/// <summary>
		/// Returns true if the size is fixed.
		/// </summary>
		public bool IsFixedSize
		{
			get { return false; }
		}

		/// <summary>
		/// Returns true if the list is read only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Returns the object in the list for a given index.  If the
		/// index given is greater than the list count a null object is
		/// returned.  Adds a new item to the list.
		/// </summary>
		public object this[int index]
		{
			get
			{
				if (index >= this.items.Count)
				{
					return null;
				}
				return items[index];
			}

			set { items.Add( value ); }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Copies items to a destination array given an index.
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="index"></param>
		public void CopyTo( Array dest, int index )
		{
			items.CopyTo( dest, index );
		}

		/// <summary>
		/// Compares one item with another item.  Uses the string compare
		/// method.
		/// </summary>
		/// <param name="item1"></param>
		/// <param name="item2"></param>
		/// <returns></returns>
		public int Compare( object item1, object item2 )
		{
			return String.Compare( item1.ToString(), item2.ToString() );
		}

		/// <summary>
		/// Returns an enumerator for the current list.  The enumerator
		/// is an interal class.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		/// <summary>
		/// Adds a new item to the list and returns the list count.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int Add( object item )
		{
			return items.Add( item );
		}

		/// <summary>
		/// Removes all items in the list.
		/// </summary>
		public void Clear()
		{
			this.items.Clear();
		}

		/// <summary>
		/// Returns true if the list contains the given search item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains( object item )
		{
			return this.items.Contains( item );
		}

		/// <summary>
		/// Returns the index of the given object if it is contained
		/// in the list.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int IndexOf( object item )
		{
			return this.items.IndexOf( item );
		}

		/// <summary>
		/// Inserts a new item before into the list for the requested index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		public void Insert( int index, object item )
		{
			this.items.Insert( index, item );
		}

		/// <summary>
		/// Removes the requested object from the list.
		/// </summary>
		/// <param name="item"></param>
		public void Remove( object item )
		{
			this.items.Remove( item );
		}

		/// <summary>
		/// Removes an object from the list using the given index.
		/// </summary>
		/// <param name="element"></param>
		public void RemoveAt( int element )
		{
			if (element < this.items.Count)
			{
				this.items.RemoveAt( element );
			}
		}
		#endregion

	}
}
