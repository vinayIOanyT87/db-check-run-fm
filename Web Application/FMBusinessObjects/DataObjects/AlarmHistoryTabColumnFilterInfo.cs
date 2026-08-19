

namespace FMBusinessObjects.DataObjects
{
	using System.Collections.Generic;

	public class AlarmHistoryTabColumnFilterInfo
	{
		#region Public members
		public enum ColumnFilterNameEnums
		{
			DateAndTime,
			Site,
			PointType,
			Point,
			PointDescription,
			Variable,
			Value,
			Units,
			AlarmState,
			Priority,
			Action,
			User,
			Comment,
			CommentUserName,
			CommentDateTime,
			None = -99
		}
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AlarmHistoryTabColumnFilterInfo()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string Name { get; set; }
		public int Index { get; set; }
		public List<string> FilterCollection { get; set; }
		public string FromDateStr { get; set; }
		public string ToDateStr { get; set; }
		public string CommentFromDateStr { get; set; }
		public string CommentToDateStr { get; set; }
		public ColumnFilterNameEnums SelectedColumnFilterEnum
		{
			get { return (ColumnFilterNameEnums)this.Index; }
			set { this.Index = (int)value; }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.Name				= string.Empty;
			this.Index				= -99;
			this.FilterCollection	= new List<string>();
			this.FromDateStr		= string.Empty;
			this.ToDateStr			= string.Empty;
			this.CommentFromDateStr = string.Empty;
			this.CommentToDateStr	= string.Empty;
		}
		#endregion
	}
}
