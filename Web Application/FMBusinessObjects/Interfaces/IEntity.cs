using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.Interfaces
{
	/// <summary>
	/// Summary description for IEntity.
	/// </summary>
	public interface IEntity
	{
		string ID
		{
			get;
		}

		string TypeID
		{
			get;
		}

		ENTITY_TYPE Type
		{
			get;
		}
	}
}
