using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;

namespace DispatchPrototype
{
	public class AddInItemsCollectionClass : CollectionBase
	{

		public void Add(AddInItemClass AddInItems)
		{
			List.Add(AddInItems);
		}

		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				throw new Exception("Invalid Index");
			}
			else
			{
				List.RemoveAt(index);
			}

		}

		public void Remove(AddInItemClass AddInItems)
		{
			int index = 0;

			foreach (AddInItemClass Item in List)
			{
				if (Item.Index == AddInItems.Index)
				{
					List.RemoveAt(index);
					return;
				}

				index++;

			}

		}

		public AddInItemClass Item(int Index)
		{
			return (AddInItemClass)List[Index];
		}

	}

	public class AddInItemClass
	{
		public int Index;
		public string MenuItem;
		public string Application;
		public AddInItemClass()
		{
			MenuItem = "";
			Application = "";
		}
	}
}
