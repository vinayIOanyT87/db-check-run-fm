using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{

	[Serializable]
	public class AlarmTestEquationEditorModel
	{
		public Guid AlarmTestGuid;

		public Guid TagGuid;

		public string TagName;

		public string TagType;

		public bool UseBitmask;

		public bool CanUseBitmask;

		public int BitMaskDigits;

		public int TagAttribute;

		public string LimitName;

		public int BitwiseOperator;

		public string Bitmask;

		public int ComparisonOperator;
	}
}
