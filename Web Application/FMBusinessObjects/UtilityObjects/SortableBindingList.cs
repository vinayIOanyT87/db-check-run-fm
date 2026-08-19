using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.UtilityObjects
{
	public class SortableBindingList<T> : System.ComponentModel.BindingList<T>
	{
		// constructor
		public SortableBindingList( System.Collections.Generic.List<T> list )
			: base( list ) { }

		// fields
		private bool m_IsSorted;
		private System.ComponentModel.ListSortDirection m_SortDirection;
		private System.ComponentModel.PropertyDescriptor m_SortProperty;

		// properties
		protected override System.ComponentModel.ListSortDirection SortDirectionCore { get { return m_SortDirection; } }
		protected override System.ComponentModel.PropertyDescriptor SortPropertyCore { get { return m_SortProperty; } }
		protected override bool IsSortedCore { get { return m_IsSorted; } }
		protected override bool SupportsSortingCore { get { return true; } }

		// methods
		protected override void RemoveSortCore() { m_IsSorted = false; }
		protected override void ApplySortCore( System.ComponentModel.PropertyDescriptor prop, System.ComponentModel.ListSortDirection direction )
		{
			if (prop.PropertyType.GetInterface( "IComparable" ) == null)
				return;
			var _List = this.Items as System.Collections.Generic.List<T>;
			if (_List == null)
			{
				m_IsSorted = false;
			}
			else
			{
				var _Comparer = new PropertyComparer( prop.Name, direction );
				_List.Sort( _Comparer );
				m_IsSorted = true;
				m_SortDirection = direction;
				m_SortProperty = prop;
			}
			OnListChanged( new System.ComponentModel.ListChangedEventArgs( System.ComponentModel.ListChangedType.Reset, -1 ) );
		}

		// sub class
		public class PropertyComparer : System.Collections.Generic.IComparer<T>
		{
			// properties
			private System.Reflection.PropertyInfo PropInfo { get; set; }
			private System.ComponentModel.ListSortDirection Direction { get; set; }

			// methods
			public PropertyComparer( string propName, System.ComponentModel.ListSortDirection direction )
			{
				this.PropInfo = typeof( T ).GetProperty( propName );
				this.Direction = direction;
			}
			public int Compare( T x, T y )
			{
				var _X = PropInfo.GetValue( x, null );
				var _Y = PropInfo.GetValue( y, null );
				if (Direction == System.ComponentModel.ListSortDirection.Ascending)
					return System.Collections.Comparer.Default.Compare( _X, _Y );
				else
					return System.Collections.Comparer.Default.Compare( _Y, _X );
			}
		}
	}

}
