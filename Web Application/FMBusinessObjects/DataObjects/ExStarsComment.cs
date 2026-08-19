namespace FMBusinessObjects.DataObjects
{
	using System;
	
	/// <summary>
	/// A base class so that Reports and Segments can share a List
	/// </summary>
	[Serializable]
	public abstract class ExStarReportAndSegmentElementBase
	{
		public virtual int EstimatedTextLength { get { return 0;}}
	}


	/// <summary>
	/// Supports showing comments in the easy-read format, but hidding those comments in the EDI format
	/// </summary>
	[Serializable]
	public class ExStarsComment : ExStarReportAndSegmentElementBase
	{
		public string Id { get; protected set; }

		public bool IsComment { get { return string.IsNullOrEmpty(this.Id);}}

		/// <summary>
		/// The description field becomes a place to put a comment that will appear only with EasyRead
		/// </summary>
		public string Description { get; protected set; }

		public new virtual int EstimatedTextLength { get { return 100 + Description.Length;}}

		public static int NextSequenceNumber { get; protected set; }

		public int SequenceNumber { get; set; }

		#region ExStarsSegmentBase Overrides

		public override int GetHashCode()
		{
			return this.Id.GetHashCode() ^ this.Description.GetHashCode() ^ this.SequenceNumber;
		}

		/// <summary>
		/// This Equals() is good for verifying deserialization, but comparing SequenceNumber and Description are not
		/// necessary to validate the actual data
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ExStarsComment compareTo = obj as ExStarsComment;
			bool isEqual = compareTo != null
			               && this.Id.Equals(compareTo.Id)
			               && this.SequenceNumber == compareTo.SequenceNumber
			               && this.Description.Equals(compareTo.Description);
			if (!isEqual)
			{
				System.Diagnostics.Debug.WriteLine("ExStarsSegmentBase Not Equal:" + this.ToString());
			}
			return isEqual;
		}


		#endregion

		#region ExStarsSegmentBase Constructors

		static ExStarsComment()
		{
			Reset();
		}

		public ExStarsComment()
			: this("")
		{
		}

		public ExStarsComment(string description)
		{
			this.Description = description;
			this.Id = null;
			SequenceNumber = ++NextSequenceNumber;
		}

		#endregion

		public static void Reset()
		{
			NextSequenceNumber = 0;
		}

		public string ToStringEasyRead()
		{
			return ToStringEdi(true);
		}

		/// <summary>
		/// Create the EDI text
		/// </summary>
		/// <returns>If there are no elements return the empty string</returns>
		public string ToStringEdi()
		{
			return ToStringEdi(false);
		}

		public virtual string ToStringEdi(bool outputEasyRead)
		{
			return outputEasyRead ? Description : "";
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", SequenceNumber, Id ?? Description);
		}
	}
}


