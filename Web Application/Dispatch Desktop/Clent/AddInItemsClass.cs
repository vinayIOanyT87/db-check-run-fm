using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;

namespace DispatchPrototype
{
	public class AddInItemsCollectionClass : CollectionBase
	{

		public void Add(AddInItemClass addInItems)
		{
			this.List.Add(addInItems);
		}

		public void Remove(int index)
		{
			if (index > this.Count - 1 || index < 0)
			{
				throw new Exception("Invalid Index");
			}
			
			this.List.RemoveAt(index);
		}

		public void Remove(AddInItemClass addInItems)
		{
			int index = 0;

			foreach (AddInItemClass item in this.List)
			{
				if (item.Index == addInItems.Index)
				{
					this.List.RemoveAt(index);
					return;
				}

				index++;
			}
		}

		public AddInItemClass Item(int index)
		{
			return (AddInItemClass)this.List[index];
		}
	}

	public class AddInItemClass
	{
		public int Index;
		public string MenuItem;
		public string Application;

		public AddInItemClass()
		{
			this.MenuItem = string.Empty;
			this.Application = string.Empty;
		}
	}
}
