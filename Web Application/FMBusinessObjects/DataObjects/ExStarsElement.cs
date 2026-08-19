
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Text.RegularExpressions;
	using FMBusinessObjects.Exceptions;

	[Serializable]
	public class ExStarsElementList : List<ExStarsElement>
	{
		public  int MaxIndex { get; protected set; }

		public ExStarsElementList()
			: base()
		{
			this.MaxIndex = -1;
		}

		public new void Add(ExStarsElement newElement)
		{
			this.MaxIndex = Math.Max(this.MaxIndex, newElement.Index);
			base.Add(newElement);
		}

		public override int GetHashCode()
		{
			int hash = this.Count;
			for (int i = 0; i < this.Count; i++)
			{
				hash ^= this[i].GetHashCode();
			}
			return hash;
		}

		public override bool Equals(object obj)
		{
			ExStarsElementList compareTo = obj as ExStarsElementList;
			bool isEqual = (compareTo != null) && (this.Count == compareTo.Count);
			System.Diagnostics.Debug.WriteIf(!isEqual, "ExStarsElementList is not equal\n");
			for (int i = 0; isEqual && i < this.Count; i++)
			{
				isEqual = this[i].Equals(compareTo[i]);
				System.Diagnostics.Debug.WriteIf(!isEqual, string.Format("ExStarsElementList[{0}] is not equal: {1}\n", i, compareTo[i].ToString()));
			}
			return isEqual;
		}

		/// <summary>
		/// Not all indexes are used so accessing as an array may not give the same result as acces ByIndex()
		/// </summary>
		/// <param name="index">Match the Index property to this value</param>
		/// <returns></returns>
		public ExStarsElement ByIndex(int index)
		{
			for( int arrayPos = 0;arrayPos < this.Count; arrayPos++)
			{
				if (this[arrayPos].Index == index)
				{
					return this[arrayPos];
				}
			}

			throw  new ExStarsElementsException("Element with index {0}, does not exist", index);
		}
	}

	// The elements (i.e fields) within an IRS-defined ExSTARS segment
	[Serializable]
	public class ExStarsElement
	{
		/// <summary>
		/// 1..n based
		/// </summary>
		public int Index { get; protected set; }
		public string Name { get; protected set; }
		public string Description { get; protected set; }
		public EnumExStarsRequired Required { get; protected set; }
		public EnumExStarsElementTypes ElementType { get; protected set; }
		public int MinLen { get; protected set; }
		public int MaxLen { get; protected set; }
		public int DecimalPlaces { get; protected set; }
		public string Value { get; set; }
		public static int EstimatedTextLength { get {  return 100;} }
		public ExStarsElementList SubElements { get; protected set; }

		public string Errors{ get; set; }

		public override int GetHashCode()
		{
			return this.Name.GetHashCode() ^ this.Value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			ExStarsElement compareTo = obj as ExStarsElement;
			bool isEqual = compareTo != null
				   && this.Index == compareTo.Index
			       && this.Name.Equals(compareTo.Name)
			       && this.Description.Equals(compareTo.Description)
			       && this.Value.Equals(compareTo.Value)
			       && this.SubElements.Equals(compareTo.SubElements);
			if (!isEqual)
			{
				System.Diagnostics.Debug.WriteLine("Element Not Equal:" + this.ToString());
			}
			return isEqual;
		}

		/// <summary>
		/// Needed to serialize
		/// </summary>
		public ExStarsElement()
			: this(-1, "", "", EnumExStarsRequired.unknown, EnumExStarsElementTypes.dontValidate, -1, -1, -1, "")
		{
		}

		public ExStarsElement(int index, string name, string description, EnumExStarsRequired required, EnumExStarsElementTypes elementType, int minLen, int maxLen, string value)
			: this(index, name, description, required, elementType, minLen, maxLen, -1, value)
		{
		}
		public ExStarsElement(int index, string name, string description, EnumExStarsElementTypes elementType, int minLen, int maxLen, string value)
			: this(index, name, description, EnumExStarsRequired.M, elementType, minLen, maxLen, -1, value)
		{
		}
		public ExStarsElement(int index, string name, string description, EnumExStarsElementTypes elementType, int minLen, int maxLen, int decimalPlaces, string value)
			: this(index, name, description, EnumExStarsRequired.M, elementType, minLen, maxLen, decimalPlaces, value)
		{			
		}
		public ExStarsElement(int index, string name, string description, EnumExStarsRequired required, EnumExStarsElementTypes elementType, int minLen, int maxLen, int decimalPlaces, string value)
		{
			Errors = "";
			// validate the data
			if (elementType != EnumExStarsElementTypes.MultiPart)
			{

				if (value == null)
				{
					Errors = string.Format("For index={0:00}, name=\"{1}\", the value is missing", index, name);
					value = "????????????????????????????????".Substring(0, MinLen);
					throw new ExStarsElementsException("For index={0:00}, name=\"{1}\", the value is missing\n", index, name);
				}
				if (value.Length > maxLen)
				{
					Errors = string.Format("For index={0:00}, name=\"{1}\", the value \"{2}\" is too long it must have length <= {3}\n", index, name, value, maxLen);
					value = value.Substring(0, maxLen);
					throw new ExStarsElementsException("For index={0:00}, name=\"{1}\", the value \"{2}\" is too long it must have length <= {3}", index, name, value, maxLen);
				}
				else if (value.Length < minLen)
				{
					Errors = string.Format("For index={0:00}, name=\"{1}\", the value \"{2}\" is too short it must have length >= {3}\n", index, name, value, minLen);
					value = value.PadRight(minLen);
					throw new ExStarsElementsException("For index={0:00}, name=\"{1}\", the value \"{2}\" is too short it must have length >= {3}", index, name, value, minLen);
				}
			}
			if (ValidateDataFormat(index, name, elementType, minLen, maxLen, value))
			{
				if (elementType == EnumExStarsElementTypes.N0)
				{
					elementType = EnumExStarsElementTypes.N;
					decimalPlaces = 0;
				}
				this.Index = index;
				this.Name = name;
				this.Description = description;
				this.Required = required;
				this.ElementType = elementType;
				this.DecimalPlaces = decimalPlaces;
				this.MinLen = minLen;
				this.MaxLen = maxLen;
				this.Value = value;
				this.SubElements = null;
			}
		}

		public void AppendSubElement(int subElementIndex, string name, string description, EnumExStarsRequired required, EnumExStarsElementTypes elementType, int minLen, int maxLen, int decimalPlaces, string subElementValue)
		{
			if (this.ElementType != EnumExStarsElementTypes.MultiPart)
			{
				throw new ExStarsElementsException("Sub-elements are not allowed");
			}
			if (this.SubElements == null)
			{
				this.SubElements = new ExStarsElementList();
			}
			if (this.SubElements.Count + 1 != subElementIndex)
			{
				throw new ExStarsElementsException("Sub-element added out of order for sub index {0}", subElementIndex);
			}
			try
			{
				this.SubElements.Add(new ExStarsElement(subElementIndex, name, description, required, elementType, minLen, maxLen, decimalPlaces, subElementValue));
			}
			catch (Exception subException)
			{
				throw new ExStarsElementsException("Sub-elements[{0}]: {1}", subElementIndex, subException.Message);
			}
		}

		/// <summary>
		/// Throw exception if the format of value is invalid
		/// </summary>
		/// <param name="index"></param>
		/// <param name="name"></param>
		/// <param name="elementType"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool ValidateDataFormat(int index, string name, EnumExStarsElementTypes elementType, int minLen, int maxLen, string value)
		{
			switch (elementType)
			{
				case EnumExStarsElementTypes.dontValidate:
					break;

				case EnumExStarsElementTypes.undefined:
					throw new ExStarsElementsException( "For index={0:00}, name={1}, the element type is undefined",index,name);

				case EnumExStarsElementTypes.MultiPart:
				{
					if (value != null)
					{
						throw new ExStarsElementsException("For index={0:00}, name={1}, the element value must be null", index, name);						
					}
						
				}
				break;

				case EnumExStarsElementTypes.N:
					{
						int i;
						if (!int.TryParse(value, out i))
						{
							throw new ExStarsElementsException(
								"For index={0:00}, name={1}, the value \"{2}\" must be an integer.",
								index,
								name,
								value);
						}
						break;
					}
				case EnumExStarsElementTypes.R:
					{
						double i;
						if (!double.TryParse(value, out i))
						{
							throw new ExStarsElementsException(
								"For index={0:00}, name={1}, the value \"{2}\" must be numeric (floating point is OK).",
								index,
								name,
								value);
						}
						break;
					}
				case EnumExStarsElementTypes.AN:
					{
						Regex hasNonSpace = new Regex(@"\S");
						if (!hasNonSpace.IsMatch(value))
						{
							throw new ExStarsElementsException(
								"For index={0:00}, name={1}, the value \"{2}\" must not be all spaces.",
								index,
								name,
								value);
						}
						break;
					}
				case EnumExStarsElementTypes.DT:
					{
						if (minLen != maxLen || ( minLen !=6 && minLen != 8))
						{
							throw new ExStarsElementsException("For index={0:00}, name={1}, invalid field length length specified", index,name);
						}
						DateTime dt;
						System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
						string dateFmt;
						switch (minLen)
						{
							case 6:
								dateFmt = "yyMMdd";
								break;
							case 8:
								dateFmt = "yyyyMMdd";
								break;
							default:
								throw new ExStarsElementsException("specified length for EnumExStarsElementTypes.DT");
						}
						if (!DateTime.TryParseExact(value, dateFmt, provider, System.Globalization.DateTimeStyles.None, out dt))
						{
							throw new ExStarsElementsException(
								"For index={0:00}, name={1}, the value \"{2}\" must be a date in the format \"{3}\".",
								index,
								name,
								value,
								dateFmt);
						}
						break;
					}
				case EnumExStarsElementTypes.TM:
					{
						string timeFmt;
						switch (value.Length)
						{
							case 4:
								timeFmt = "HHmm";
								break;
							case 6:
								timeFmt = "HHmmss";
								break;
							default:
								throw new ExStarsElementsException("Value length for EnumExStarsElementTypes.TM");
						}
						DateTime dt;
						System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
						if (!DateTime.TryParseExact(value, timeFmt, provider, System.Globalization.DateTimeStyles.None, out dt))
						{
							throw new ExStarsElementsException(
								"For index={0:00}, name={1}, the value \"{2}\" must be a time in the format \"{3}\".",
								index,
								name,
								value,
								timeFmt);
						}
						break;
					}
				default:
					break;
			}
			return true;
		}

		public override string ToString()
		{
			if (this.SubElements != null)
			{
				bool firstTime = true;
				StringBuilder compositeValue = new StringBuilder("");
				foreach (ExStarsElement subElement in this.SubElements)
				{
					if (firstTime)
					{
						firstTime = false;
					}
					else
					{
						compositeValue.Append(ExStarsConstants.SubElementSeparator);
					}
					compositeValue.Append(subElement.ToString());
				}
				return compositeValue.ToString();
			}
			return Value;
		}
	}
}
