namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using FMBusinessObjects.Exceptions;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.IO;

	[Serializable]
	public class PureSegmentList : List<ExStarsSegment>
	{
		protected int NextSegmentSequenceNumber = 0;
		protected string DefaultDescription { get; set; }

		/// <summary>
		/// Add new element, and set the sequence number at the same time
		/// </summary>
		/// <param name="segment"></param>
		public new void Add(ExStarsSegment segment)
		{
			segment.SequenceNumber = ++NextSegmentSequenceNumber;
			segment.ParentList = this;
			base.Add(segment);
		}

		public void Add(ExStarsComment baseElement)
		{
			this.Add(new ExStarsSegment(baseElement));
		}


		public PureSegmentList() : base() { }
		public PureSegmentList(string wholeReport, string defaultDescription) : base()
		{
			this.DefaultDescription = defaultDescription;
			Parse(wholeReport);
		}

		public void Parse(string wholeReport)
		{
			//
			// segments are separated by backslash, comments by LF
			// With the EasyRead format segments end with both \\ and LF
			//
			string[] allLines = wholeReport.Split('\\','\n');
			foreach (var line in allLines)
			{
				string trimmedLine = line.Trim('\n', '\r');
				//
				// We are counting on ther being a space for intensional blank lines
				//
				if (trimmedLine.Length > 0)
				{
					ExStarsSegment segment = new ExStarsSegment(trimmedLine, false, DefaultDescription);
					this.Add(segment);
				}
			}
		}


		public ExStarsSegment Prev(ExStarsSegment startingSegment)
		{
			if (startingSegment == null || startingSegment.SequenceNumber <= 1)
			{
				return null;
			}
			//
			// SequenceNumber is 1..n, so subtract 2
			//
			return this[startingSegment.SequenceNumber - 2];
		}


		public ExStarsSegment Next(ExStarsSegment startingSegment)
		{
			int index =  (startingSegment == null) ? 0 : startingSegment.SequenceNumber;
			return (index > this.NextSegmentSequenceNumber)
				? null
				: this[index];		
		}

		/// <summary>
		/// FindNext() differs from Find() in that the search starts with the segment after "startingSegment"
		/// </summary>
		/// <param name="startingSegment"></param>
		/// <param name="minimumNumberOfElements">Throw an error if the segment is not found or it does not have 
		/// at least this number of elements </param>
		/// <param name="segmentId"></param>
		/// <param name="valuesToMatch"></param>
		/// <returns>the segment found or null is not found</returns>
		public ExStarsSegment FindNext(ExStarsSegment startingSegment, int minimumNumberOfElements, string segmentId, params object[] valuesToMatch)
		{
			ExStarsSegment nextSegment = this.Next(startingSegment);
			return null == nextSegment
				?  null
				: this.Find(nextSegment, minimumNumberOfElements, segmentId, valuesToMatch);
		}


		public ExStarsSegment Find(ExStarsSegment startingSegment, string segmentId, params object[] valuesToMatch)
		{
			return Find(startingSegment, 0, segmentId, valuesToMatch);
		}


		/// <summary>
		/// Search the PureSegmentList for a segment whose ID and element values match those listed in the last parameter
		/// </summary>
		/// <param name="startingSegment">Where to start searching, if this segment matches the conditions specified in valuesToMatch, it will be the segment returned</param>
		/// <param name="minimumNumberOfSegments">If this is non-zero, an exception will be thrown if the element count is less</param>
		/// <param name="segmentId">The segment ID that must match</param>
		/// <param name="valuesToMatch">The element values that must match, use null, when an element is not expected to exist (since element.Index may skip values)</param>
		/// <returns>the segment found or null is not found</returns>
		public ExStarsSegment Find(ExStarsSegment startingSegment, int minimumNumberOfSegments, string segmentId, params object[] valuesToMatch)
		{
			int index = 0;
			if (startingSegment != null)
			{
				index = startingSegment.SequenceNumber - 1;
			}
			for (; index < this.Count; index++)
			{
				ExStarsSegment segment = this[index];
				if (segment.Match(segmentId, valuesToMatch))
				{
					if (segment.Elements.MaxIndex < minimumNumberOfSegments)
					{
						break;
					}
					return segment;
				}
			}
			if (minimumNumberOfSegments > 0)
			{
				throw new ExStarsSegmentException("missing the {0} segment or it has an invalid format", segmentId);
			}
			return null;
		}

		/// <summary>
		/// Same as Find() except implimented with Linq, since it searchs the whole list from the beginning
		/// it is not as efficient as Find()
		/// </summary>
		/// <param name="startingSegment"></param>
		/// <param name="segmentId"></param>
		/// <param name="valuesToMatch"></param>
		/// <returns></returns>
		public ExStarsSegment Find2(ExStarsSegment startingSegment, string segmentId, params object[] valuesToMatch)
		{
			int sequenceNum = startingSegment == null ? 1 : startingSegment.SequenceNumber;
			var found = (from segment in this
			             where segment.SequenceNumber >= sequenceNum
			                   && segment.Match(segmentId, valuesToMatch)
			             select segment).FirstOrDefault();
			return found;
		}


		/// <summary>
		/// Binary serialization is used because XML serialization cannot serial IDictionary objects
		/// </summary>
		/// <returns>Binary representation of serialized data as a string</returns>
		public string ToBinary()
		{
			// ref: http://bytes.com/topic/c-sharp/answers/261764-serialize-database
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream mem = new MemoryStream())
			{
				bf.Serialize(mem, this);
				return Convert.ToBase64String(mem.ToArray());
			}
		}

		public static PureSegmentList FromBinary(string serializedBinaryAsStr)
		{
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream mem = new MemoryStream(Convert.FromBase64String(serializedBinaryAsStr)))
			{
				PureSegmentList list = (PureSegmentList)bf.Deserialize(mem);
				return list;
			}
		}

		public string ToStringEdi(bool easyRead)
		{
			StringBuilder stringOut = new StringBuilder(this.Count * 200);
			foreach (var seg in this)
			{
				string elementToEdi = seg.ToStringEdi(easyRead);
				if (string.IsNullOrEmpty(elementToEdi))
				{
					// do nothing
				}
				else if (easyRead)
				{
					stringOut.AppendLine(elementToEdi);
				}
				else
				{
					stringOut.Append(elementToEdi);
				}
			}
			return stringOut.ToString();
		}



		public override string ToString()
		{
			StringBuilder stringOut = new StringBuilder(this.Count * 200);
			foreach (var seg in this)
			{
				stringOut.AppendLine(seg.ToString());
			}
			return stringOut.ToString();
		}

	}


	[Serializable]
	public class SegmentList : List<ExStarReportAndSegmentElementBase>
	{
		protected int NextSegmentSequenceNumber = 0;

		public SegmentList() : base() { }

		/// <summary>
		/// Count only those segments that have a non-null ID
		/// </summary>
		/// <returns></returns>
		public int CountInUse()
		{
			if (!this.Any())
			{
				return 0;
			}

			var count = (from segment
				         in this
						 where segment as ExStarsSegment != null && (segment as ExStarsSegment).Id != null
			             select segment).Count();
			return count;
		}

		#region SegmentList Overrides
		/// <summary>
		/// If parameter is null, there is no Add().  Set the sequence number at the same time
		/// </summary>
		/// <param name="reportAndSegment"> A segment or null</param>
		public new void Add(ExStarReportAndSegmentElementBase reportAndSegment)
		{
			if (reportAndSegment == null)
			{
				return;
			}

			if (reportAndSegment is ExStarsSegment)
			{
				(reportAndSegment as ExStarsSegment).SequenceNumber = ++NextSegmentSequenceNumber;
			}

			base.Add(reportAndSegment);
		}

		public void AddRange(SegmentList list)
		{
			foreach (ExStarReportAndSegmentElementBase reportAndSegment in this)
			{
				if (reportAndSegment is ExStarsSegment)
				{
					(reportAndSegment as ExStarsSegment).SequenceNumber = ++NextSegmentSequenceNumber;
				}
			}
			base.AddRange(list);
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
			SegmentList compareTo = obj as SegmentList;
			bool isEqual = compareTo != null && this.Count == compareTo.Count;
			for (int i = 0; isEqual && i < this.Count; i++)
			{
				isEqual = this[i].Equals(compareTo[i]);
			}
			return isEqual;
		}
		
		#endregion
	}

	// An IRS-defined ExSTARS segment, which is composed on one or more elements
	[Serializable]
	public class ExStarsSegment : ExStarsComment
	{
		#region ExStarsSegment Public Properties

		public string ErrorId { get; set; }
		public string Name { get; protected set; }
		/// <summary>
		/// For EasyRead this is print above the element
		/// </summary>

		public ExStarsElementList Elements { get; protected set; }
		public new int EstimatedTextLength {  get { return base.EstimatedTextLength + ExStarsElement.EstimatedTextLength * Elements.Count;} }
		public string Errors { get; protected set; }
		public PureSegmentList ParentList { get; protected internal set; }

		#endregion

		#region ExStarsSegment Protected and Private variables

		protected static ulong LastUniqueId = 0;

		private static readonly object LockUniqueId;

		private static readonly Regex Segmentpattern = new Regex(@"^(BTA|BTI|DTM|FGS|GE|GS|IEA|ISA|N1|N2|N3|N4|PBI|PER|QTY|REF|SE|ST|TIA|TFS)~\S+");
		private static readonly string[] ValidSegmentIds = new[] { "BTA", "BTI", "DTM", "FGS", "GE", "GS", "IEA", "ISA", "N1", "N2", "N3", "N4", "PBI", "PER", "QTY", "REF", "SE", "ST", "TIA", "TFS" };
		#endregion

		#region ExStarsSegment Constructors

		static ExStarsSegment()
		{
			LockUniqueId = new object();
		}

		public ExStarsSegment() : this(null, null){}

		public ExStarsSegment(string id, string name) : this(id, name, ""){}

		public ExStarsSegment(ExStarsComment baseSegment) : this(baseSegment.Id, "", baseSegment.Description) {}
	
		public ExStarsSegment(string id, string name, string description) : base(description)
		{
			// comments have empty ID's
			if ( !string.IsNullOrEmpty(id) && !IsValidSegmentId(id))
			{
				throw new ExStarsSegmentException("Invalid segment ID:\"{0}\"", id);
			}
			this.Id = id;
			this.Name = name;
			this.Elements = new ExStarsElementList();
			this.Errors = "";
			this.ParentList = null;
		}

		/// <summary>
		/// Used to parse EDI ackowledgement 151
		/// </summary>
		/// <param name="wholeLine">Expects Segment ID followed by tilda then at least one value. All other lines are comments </param>
		/// <param name="unused">All constructor to have unique signature</param>
		/// <param name="defaultElementDescription"></param>
		public ExStarsSegment(string wholeLine, bool unused, string defaultElementDescription )
			: this()
		{
			if (!HasSegmentFormat(wholeLine))
			{
				// 
				// this is a comment
				//
				this.Description = wholeLine;
				return;
			}
			string[] fields = wholeLine.Split('~');
			if (fields.Count() < 2)
			{
				// 
				// most likely, the HasSegmentFormat() prevents this exception from ever firing
				//
				throw new ExStarsSegmentException("Invalid segment data:\"{0}\"", wholeLine);
			}
			this.Id = fields[0];
			if (!IsValidSegmentId(this.Id))
			{
				throw new ExStarsSegmentException("Invalid segment ID:\"{0}\"", this.Id);
			}
			this.Name = "";
			// 
			// fields[0] has already been used
			//
			for (int i = 1; i < fields.Count(); i++)
			{
				ExStarsElement element = new ExStarsElement(i, "Name Unknown", defaultElementDescription, EnumExStarsElementTypes.dontValidate, -1, int.MaxValue, fields[i]);
				this.Elements.Add(element);
			}
		}

		#endregion

		#region ExStarsSegment Overrides

		public override string ToString()
		{
			return string.Format("{0}: {1}"
				, SequenceNumber
				, this.IsComment 
					? this.Description 
					: this.ToStringEdi(false));
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode() ^ base.GetHashCode();
		}

		/// <summary>
		/// Use to match source to source
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ExStarsSegment compareTo = obj as ExStarsSegment;
			return compareTo != null
				&& base.Equals(obj)
				&& this.Name.Equals(compareTo.Name)
				&& this.Elements.Equals(compareTo.Elements);
		}

		#endregion

		#region ExStarsSegment Static Functions

		/// <summary>
		/// Use the parameterized version only for debugging and testing
		/// </summary>
		/// <param name="timeStamp"></param>
		/// <returns></returns>
		public static string UniqueControlNumber(DateTime timeStamp)
		{
			const ulong MaxValue = 1000000000;
			DateTime timeBegan = ExStarsConstants.BeginningOfDateTime;
			const ulong NumberOfDaysIn3Years = 1 + 365 * 3;
			const ulong SecondsIn3Years = 60 * 60 * 24 * NumberOfDaysIn3Years;
			double milliSecSinceTimeBegan = timeStamp.Subtract(timeBegan).TotalMilliseconds;
			ulong unique = (ulong)(milliSecSinceTimeBegan * MaxValue / SecondsIn3Years % MaxValue);

			lock (LockUniqueId)
			{
				// this deals with both rollover and non-unique
				if (LastUniqueId >= unique)
				{
					unique = (LastUniqueId + 1) % MaxValue;
				}
				LastUniqueId = unique;
			}
			return unique.ToString(CultureInfo.InvariantCulture).PadLeft(9, '0');
		}


		/// <summary>
		/// returns a string that is unique to each millisecond within the last 3 years.
		/// It then checks to see if that it really is unique
		/// The caller is responsible to varify uniqueness by looking up previously used values
		/// </summary>
		/// <returns>a 9-digit numeric string</returns>
		public static string UniqueControlNumber()
		{
			return UniqueControlNumber(DateTime.Now);
		}

		/// <summary>
		/// Given a string that starts with a segment ID, return just the segment name.
		/// </summary>
		/// <param name="segmentAndElementKey"></param>
		/// <returns></returns>
		public static string SegmentId(string segmentAndElementKey)
		{
			var segmentId = (from id in ValidSegmentIds
							 where id.Equals(segmentAndElementKey.Left(id.Length))
							 select id).First();
			if (string.IsNullOrEmpty(segmentId))
			{
				throw new ExStarsSegmentException("There is no defined segment type to match the start of {0}", segmentAndElementKey);
			}
			return segmentId;
		}

		#endregion

		#region ExStarsSegment Public  Methods

		public ExStarsSegment Next()
		{
			return this.ParentList == null
				       ? null
				       : this.ParentList.Next(this);
		}

		public ExStarsSegment Prev()
		{
			return this.ParentList == null
					   ? null
					   : this.ParentList.Prev(this);
		}


		/// <summary>
		/// FindNext() differs from Find() in that the search starts with the segment after "startingSegment"
		/// </summary>
		/// <param name="minimumNumberOfElements">Throw an error if the segment is not found or it does not have 
		/// at least this number of elements </param>
		/// <param name="segmentId"></param>
		/// <param name="valuesToMatch"></param>
		/// <returns></returns>
		public ExStarsSegment FindNext(int minimumNumberOfElements, string segmentId, params object[] valuesToMatch)
		{
			return this.ParentList == null
					   ? null
					   : this.ParentList.FindNext(this, minimumNumberOfElements, segmentId, valuesToMatch);
		}

		/// <summary>
		/// FindNext() differs from Find() in that the search starts with the segment after "startingSegment"
		/// </summary>
		/// <param name="segmentId"></param>
		/// <param name="valuesToMatch"></param>
		/// <returns>the segment found or null is not found</returns>
		public ExStarsSegment FindNext(string segmentId, params object[] valuesToMatch)
		{
			return FindNext(0, segmentId, valuesToMatch);
		}

		/// <summary>
		/// Search the PureSegmentList for a segment whose ID and element values match those listed in the last parameter
		/// </summary>
		/// <param name="segmentId">The segment ID that must match</param>
		/// <param name="valuesToMatch">The element values that must match, use null, when an element is not expected to exist (since element.Index may skip values)</param>
		/// <returns>the segment found or null is not found</returns>
		public ExStarsSegment Find(string segmentId, params object[] valuesToMatch)
		{
			return this.ParentList == null
				       ? null
				       : this.ParentList.FindNext(this, 0, segmentId, valuesToMatch);
		}


		/// <summary>
		/// Does the selected segment match when compared for ID and optional first value?
		/// </summary>
		/// <param name="segmentId"></param>
		/// <param name="valuesToMatch"> value to match for elements, use nulls to match to skipped elements  </param>
		/// <returns></returns>
		public bool Match(string segmentId, params object[] valuesToMatch)
		{
			// 
			// Since this.Elements.MaxIndex is 1..n it can equal valuesToMatch.Count()
			//
			if (!segmentId.Equals(this.Id, StringComparison.InvariantCultureIgnoreCase)
				|| this.Elements.MaxIndex < valuesToMatch.Count())
			{
				return false;
			}

			int elementIdx = 0;			
			foreach (var matchValue in valuesToMatch)
			{	
				//
				// null is not the same as empty
				//
				if (matchValue == null)
				{
					// 
					// elementIdx does not increment when paired to null in valuesToMatch
					//
					continue;
				}
				if (elementIdx > this.Elements.MaxIndex
				    || !this.Elements[elementIdx].Value.Equals(matchValue.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					return false;
				}
				elementIdx++;
			}
			return true;
		}



		/// <summary>
		/// get value of element or sub-element
		/// </summary>
		/// <param name="key">has structure "SSSee" or "SSSee-nn", Where 
		///   "SSS" is the segment ID. It may have 2 or three characters
		///   "ee" is the element index (not array position)
		///   "nn" is the sub-element index</param>
		/// <returns></returns>
		public string ElementValue(string key)
		{
			return ElementByKey(key).Value;
		}

		/// <summary>
		/// get value of element or sub-element
		/// </summary>
		/// <param name="key">has structure "SSSee" or "SSSee-nn", Where 
		///   "SSS" is the segment ID. It may have 2 or three characters
		///   "ee" is the element index (not array position)
		///   "nn" is the sub-element index</param>
		/// <returns></returns>
		public ExStarsElement ElementByKey(string key)
		{
			// 
			// validate the argument
			//
			Regex keyFmt =  new Regex( string.Format("^{0}\\d\\d(-\\d\\d){{0,1}}$", this.Id));
			if (!keyFmt.IsMatch(key))
			{
				throw new ExStarsSegmentException("ExStarsSegment.ElementValue({0}) has bad argument for segment ID {1}", key, this.ToString());
			}
			int elementIndexPosition = this.Id.Length;
			const int ElementIndexLength = 2;
			int elementIdx = int.Parse(key.Substring(elementIndexPosition, ElementIndexLength));
			ExStarsElement element = this.ElementByIndex(elementIdx);
			if (key.Length == elementIndexPosition + ElementIndexLength)
			{
				return element;
			}
			int subElementIdx = int.Parse(key.Substring(elementIndexPosition + ElementIndexLength + 1, 2));
			var subElement = (from sub 
						      in element.SubElements
							  where sub.Index == subElementIdx
							  select sub).First();
			return subElement;
		}

		protected ExStarsElement GetElementByIndex(int elementIndex)
		{
			foreach (ExStarsElement element in this.Elements)
			{
				if (element.Index == elementIndex)
				{
					return element;
				}
			}
			throw new ExStarsElementsException("Segment {0} ({1}) does not contain an element for index {2}", this.Name, this.Description, elementIndex);
		}


		public static string PadLeft0(int val, int length)
		{
			return val.ToString(CultureInfo.InvariantCulture).PadLeft(length, '0');
		}

		public static bool IsValidSegmentId(string id)
		{
			return ValidSegmentIds.Contains(id);
		}


		public static bool HasSegmentFormat(string possibleSegment)
		{
			return Segmentpattern.IsMatch(possibleSegment);
		}

		#region Add Elements

		public ExStarsElement AddElement(
			int elementIndex,
			string elementName,
			string description,
			EnumExStarsRequired required,
			EnumExStarsElementTypes elementType)
		{
			if (elementType != EnumExStarsElementTypes.MultiPart)
			{
				throw new ExStarsSegmentException("Element {0}{1:00} is missing parameters", Name, elementIndex);
			}
			return AddElement(elementIndex, elementName, description, required, elementType, -1, -1, -1, null);
		}


		public ExStarsElement AddElement(int elementIndex, string elementName, string description, EnumExStarsElementTypes elementType, int minLen, int maxLen, string value)
		{
			return AddElement(elementIndex, elementName, description, EnumExStarsRequired.M, elementType, minLen, maxLen, -1, value);
		}


		public ExStarsElement AddElement(
			int elementIndex,
			EnumExStarsRequired required,
			EnumExStarsElementTypes elementType,
			int minLen,
			int maxLen,
			string value)
		{
			return AddElement(elementIndex, "", "", required, elementType, minLen, maxLen, -1, value);
		}

		public ExStarsElement AddElement(
			int elementIndex,
			EnumExStarsElementTypes elementType,
			int minLen,
			int maxLen,
			string value)
		{
			return AddElement(elementIndex, "", "", EnumExStarsRequired.M, elementType, minLen, maxLen, -1, value);
		}


		public ExStarsElement AddElement(
			int elementIndex,
			string elementName,
			string description,
			string elementTypeAsString,
			int minLen,
			int maxLen,
			string value)
		{
			EnumExStarsElementTypes elementType;
			int decimalPlace = -1;
			Regex isFixPtNumericFmt = new Regex(@"^N\d$");
			try
			{
				if (isFixPtNumericFmt.IsMatch(elementTypeAsString))
				{
					elementType = EnumExStarsElementTypes.N;
					decimalPlace = int.Parse(elementTypeAsString.Substring(1));
				}
				else
				{
					elementType = (EnumExStarsElementTypes)Enum.Parse(typeof(EnumExStarsElementTypes), elementTypeAsString);
				}
			}
			catch (Exception innerException)
			{
				throw new ExStarsSegmentException(
					innerException,
					"For Segment {0}, element {1} ({2}) has an invalid format of {3}",
					Name,
					elementName,
					description,
					elementTypeAsString);
			}

			return AddElement(
				elementIndex,
				elementName,
				description,
				EnumExStarsRequired.M,
				elementType,
				minLen,
				maxLen,
				decimalPlace,
				value);
		}


		public ExStarsElement AddElement(
			int elementIndex,
			string elementName,
			string description,
			EnumExStarsRequired required,
			EnumExStarsElementTypes elementType,
			int minLen,
			int maxLen,
			string value)
		{
			return AddElement(elementIndex, elementName, description, required, elementType, minLen, maxLen, -1, value);
		}

		public ExStarsElement AddElement(int elementIndex, string value)
		{
			return AddElement(elementIndex, "", "", EnumExStarsRequired.unknown, EnumExStarsElementTypes.dontValidate, 0, int.MaxValue, -1, value);
		}


		public ExStarsElement AddElement(
			int elementIndex,
			string elementName,
			string description,
			EnumExStarsRequired required,
			EnumExStarsElementTypes elementType,
			int minLen,
			int maxLen,
			int decimalPlaces,
			string value)
		{
			// validate uniqueness of index
			var list = from row in Elements where row.Index == elementIndex select row;
			if (list.Any())
			{
				throw new ExStarsSegmentException("Element \"{0}{2:00}\" name=\"{1}\" is already defined", Id, Name, elementIndex);
			}
			ExStarsElement newElement = null;
			try
			{
				System.Diagnostics.Debug.WriteLine("{0}{1:00}=\"{2}\"", Id, elementIndex, value);
				newElement =
					new ExStarsElement(
						elementIndex,
						elementName,
						description,
						required,
						elementType,
						minLen,
						maxLen,
						decimalPlaces,
						value);
				Elements.Add(newElement);
				return newElement;
			}
			catch (Exception innerException)
			{
				string msg = string.Format("For Segment \"{0} ({1})\": \"{2}\", {3}\n  Errort=\"{4}\"\n{5}", Name, Id, description, innerException.Message, newElement == null? "" : newElement.Errors, innerException.StackTrace);
				throw new ExStarsSegmentException(innerException, msg);
			}
		}

		
		#endregion



		public void AppendSubElement(int elementIndex, int subElementIndex, string name, string description, EnumExStarsRequired required, EnumExStarsElementTypes elementType, int minLen, int maxLen, string subElementValue, CompanyClass company)
		{
			AppendSubElement(elementIndex, subElementIndex, name, description, required, elementType, minLen, maxLen, -1, subElementValue, company);
		}


		public void AppendSubElement(int elementIndex, int subElementIndex, string name, string description, EnumExStarsRequired required, EnumExStarsElementTypes elementType, int minLen, int maxLen, int decimalPlaces, string subElementValue, CompanyClass company)
		{
			ExStarsElement element = GetElementByIndex(elementIndex);
			try
			{
				element.AppendSubElement( subElementIndex, name, description, required, elementType, minLen, maxLen, decimalPlaces, subElementValue);
			}
			catch (Exception e)
			{
				string companyText = company == null ? "" : string.Format(" Company {0}", company.ID);
				throw new ExStarsElementsException("AppendToElement() failed for{0} Segment \"{1}\"({2} -- {3})[{4}] {5}\n{6}"
					, companyText, this.Id, this.Name, this.Description, elementIndex, e.Message, e.StackTrace);
			}
		}

		public ExStarsElement ElementByIndex(int index)
		{
			var matchByIndex = (from element in this.Elements
								where element.Index == index
								select element).FirstOrDefault();

			if (matchByIndex != null)
			{
				return matchByIndex;
			}

			throw new ExStarsElementsException("Segment {0} does not have an element with index {1}", this.Id, index);			
		}


		public new string ToStringEdi(bool outputEasyRead)
		{
			StringBuilder retVal = new StringBuilder(10 + this.Description.Length + this.Elements.Count * 30);
			if (outputEasyRead && this.Description.Length > 0)
			{
				retVal.AppendLine(this.Description);
			}
			if (this.Id != null)
			{
				retVal.Append(this.Id);
				// 
				// the index from the element
				//
				int elementIndex = 0;
				// 
				// the position differs from the index when the Elements skip a value;
				//
				int arrayPosition = 0;
				while (arrayPosition < this.Elements.Count)
				{
					retVal.Append("~");
					elementIndex++;
					if (Elements[arrayPosition].Index == elementIndex)
					{
						retVal.Append(this.Elements[arrayPosition]);
						arrayPosition++;
					}
				}
				if (outputEasyRead)
				{
					retVal.AppendLine("\\");
				}
				else
				{
					retVal.Append("\\");
				}
				System.Diagnostics.Debug.WriteLine("{0}.ToStringEdi()={1}", this.Id, retVal);
			}
			return retVal.ToString();
		}

		#endregion
	}



}
