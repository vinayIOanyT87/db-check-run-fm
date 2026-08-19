namespace Dispatch
{
	using System;
	using System.Collections;

	using FMBusinessObjects.DataObjects;

	[Serializable()]
	public class TransactionDOCollectionClass : CollectionBase
	{
		public void Add(TransactionDO transActionDOObject)
		{
			this.List.Add(transActionDOObject);
		}

		public void Remove(int index)
		{
			if (index > this.Count - 1 || index < 0)
			{
				throw new Exception("Invalid Index");
			}
			
			this.List.RemoveAt(index);
		}

		public void Remove(TransactionDO transActionDOObject)
		{
			int index = 0;

			foreach (TransactionDO item in this.List)
			{
				if (item.TransID == transActionDOObject.TransID)
				{
					this.List.RemoveAt(index);
					return;
				}

				index++;
			}
		}

		public TransactionDO this[int index]
		{
			get
			{
				return (TransactionDO) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}
	}
}
