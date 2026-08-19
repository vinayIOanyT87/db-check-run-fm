

namespace FMBusinessObjects.DataObjects
{
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	public class ExStarsIrsErrorCodeClassList : Dictionary<ExStarsIrsErrorCodeClass, ExStarsIrsErrorCodeClass>
	{
		public string GetDescription(ExStarsIrsErrorCodeClass.CodeGroupEnum codeGroup, string code)
		{
			ExStarsIrsErrorCodeClass irsErrorCode = this.LookUp(codeGroup, code);
			return irsErrorCode == null ? "" : irsErrorCode.Description;
		}

		public ExStarsIrsErrorCodeClass LookUp(ExStarsIrsErrorCodeClass.CodeGroupEnum codeGroup, string code)
		{
			try
			{
				ExStarsIrsErrorCodeClass key = new ExStarsIrsErrorCodeClass(codeGroup, code);
				return this[key];
			}
			catch
			{
				return null;
			}
		}		
	}

	/// <summary>
	/// Error codes defined by the IRS Motor Fuel Excise Tax EDI Guide p 162
	/// </summary>
	public class ExStarsIrsErrorCodeClass
	{
		public enum CodeGroupEnum { PBI01_Primary, PBI01_Secondary, PBI03_Primary, PBI03_Secondary, PBI04};

		[DataMember]
		public CodeGroupEnum CodeGroup { get; set;}
		[DataMember]
		public string Code { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public string ElementId { get; set; }


		public ExStarsIrsErrorCodeClass(){}

		/// <summary>
		/// Use this contructor to build a key to be used when looking up in ExStarsIrsErrorCodeClassList
		/// </summary>
		/// <param name="codeGroup"></param>
		/// <param name="code"></param>
		public ExStarsIrsErrorCodeClass(CodeGroupEnum codeGroup, string code)
		{
			this.CodeGroup = codeGroup;
			this.Code = code;
		}

		public override int GetHashCode()
		{
			return Code.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			ExStarsIrsErrorCodeClass compareTo = obj as ExStarsIrsErrorCodeClass;

			return compareTo != null 
				&& Code.Equals(compareTo.Code) 
				&& CodeGroup.Equals(compareTo.CodeGroup);
		}
	}
}
