using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;

namespace FMBusinessServices.InternalClasses.EntityImportExport
{
	[CollectionDataContract]
	public class XMLExportDocumentCollectionClass : CollectionBase
	{
		public void Add ( WSExportObject XMLDocumentObject )
		{
			List.Add ( XMLDocumentObject );
		}

		public void Remove ( int index )
		{
			if (index > Count - 1 || index < 0)
			{
				throw new Exception ( "Invalid Index" );
			}
			else
			{
				List.RemoveAt ( index );
			}

		}

		public void Remove ( WSExportObject XMLDocumentObject )
		{
			int index = 0;

			foreach (WSExportObject Item in List)
			{
				if (Item.WorkSheetName == XMLDocumentObject.WorkSheetName)
				{
					List.RemoveAt ( index );
					return;
				}
				index++;
			}
		}

		public void RemoveAll ( )
		{
			int index = 0;

			foreach (WSExportObject Item in List)
			{
				List.RemoveAt ( index );
				index++;
			}
		}

		public WSExportObject find ( string worksheetname )
		{
			foreach (WSExportObject Item in List)
			{
				if (Item.WorkSheetName == worksheetname)
				{
					return Item;
				}
			}
			return null;
		}

		public WSExportObject this[int Index]
		{
			get { return (WSExportObject) List[Index]; }
			set { List[Index] = value; }
		}
	}
	[CollectionDataContract]
	public class XMLImportDocumentCollectionClass : CollectionBase
	{
		public void Add ( WSImportObject XMLDocumentObject )
		{
			List.Add ( XMLDocumentObject );
		}

		public void Remove ( int index )
		{
			if (index > Count - 1 || index < 0)
			{
				throw new Exception ( "Invalid Index" );
			}
			else
			{
				List.RemoveAt ( index );
			}

		}

		public void Remove ( WSImportObject XMLDocumentObject )
		{
			int index = 0;

			foreach (WSImportObject Item in List)
			{
				if (Item.WorkSheetName == XMLDocumentObject.WorkSheetName)
				{
					List.RemoveAt ( index );
					return;
				}
				index++;
			}
		}

		public void RemoveAll ( )
		{
			int index = 0;

			foreach (WSImportObject Item in List)
			{
				List.RemoveAt ( index );
				index++;
			}
		}

		public WSImportObject find ( string worksheetname )
		{
			foreach (WSImportObject Item in List)
			{
				if (Item.WorkSheetName == worksheetname)
				{
					return Item;
				}
			}
			return null;
		}

		public WSImportObject this[int Index]
		{
			get { return (WSImportObject) List[Index]; }
			set { List[Index] = value; }
		}

	}
}