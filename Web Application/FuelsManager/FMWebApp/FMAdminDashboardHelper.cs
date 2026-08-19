using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMWebApp
{
	using System.Drawing;
	using System.Web.UI.WebControls;

	public class FMAdminDashboardHelper
	{
		public string GetLastSyncDate(object lastSyncDate, object lastSyncHours)
		{
			var syncDateTime = ((DateTimeOffset)lastSyncDate).ToString("g");  // Display as general date/time format.
			var syncHours = Convert.ToInt32(lastSyncHours);

			var totalSyncHours = Convert.ToInt32(syncHours);
			var totalSyncDays = Math.Truncate((totalSyncHours * 1d) / 24);
			var totalSyncMonths = Math.Truncate((totalSyncDays % 356) / 30);

			var comment = "";
			if (totalSyncMonths >= 2.0)
			{
				comment = string.Format("{0} months ago", totalSyncMonths);
			}
			else if (totalSyncDays >= 2.0)
			{
				comment = string.Format("{0} days ago", totalSyncDays);
			}
			else
			{
				comment = string.Format("{0} hours ago", totalSyncHours);
			}

			return string.Format("{0} ({1})", syncDateTime, comment);
		}
		public void SetNodeHealthTotalsCellColor(GridViewRowEventArgs e)
		{
			var colIndex = this.GetColumnIndexByDataField(e.Row, "nodeHealthIndicator");
			if (colIndex.Equals(-1))
			{
				return;
			}

			var nodeHealthIndicator = e.Row.Cells[colIndex].Text;
			if (nodeHealthIndicator == "Critical")
			{
				e.Row.Cells[colIndex].BackColor = Color.Red;
			}
			else if (nodeHealthIndicator == "Caution")
			{
				e.Row.Cells[colIndex].BackColor = Color.Yellow;
			}
			else  
			{
				e.Row.Cells[colIndex].BackColor = Color.Green;
				e.Row.Cells[colIndex].ForeColor = Color.White;
			}
		}


		public void SetNodeHealthCellColor(GridViewRowEventArgs e)
		{
			var colIndex = this.GetColumnIndexByDataField(e.Row, "nodeHealthIndicator");
			if (colIndex.Equals(-1))
			{
				return;
			}

			// "Hide" the text in Node Health column by setting fore and back color to the same value.
			var nodeHealthIndicator = e.Row.Cells[colIndex].Text;
			if (nodeHealthIndicator == "0")
			{
				e.Row.Cells[colIndex].BackColor = Color.Green;
				e.Row.Cells[colIndex].ForeColor = Color.White;
				e.Row.Cells[colIndex].Text = "Satisfactory";
			}
			else if (nodeHealthIndicator == "1")
			{
				e.Row.Cells[colIndex].BackColor = Color.Yellow;
				e.Row.Cells[colIndex].ForeColor = Color.Black;
				e.Row.Cells[colIndex].Text = "Caution";
			}
			else  // Value should be "2"
			{
				e.Row.Cells[colIndex].BackColor = Color.Red;
				e.Row.Cells[colIndex].ForeColor = Color.Black;
				e.Row.Cells[colIndex].Text = "Critical";
			}
		}

		public int GetColumnIndexByDataField(GridViewRow row, string dataFieldName)
		{
			var columnIndex = 0;
			foreach (object cell in row.Cells)
			{
				var fieldCell = cell as DataControlFieldCell;
				if (fieldCell != null)
				{
					var field = ((DataControlFieldCell)cell).ContainingField as BoundField;
					if (field != null)
					{
						if (field.DataField.Equals(dataFieldName))
						{
							return columnIndex;
						}
					}
				}
				columnIndex++; // keep adding 1 while we don't have the correct name
			}
			return -1;
		}

		public int GetColumnIndexByHeader(GridViewRow row, string headerText)
		{
			var columnIndex = 0;
			foreach (object cell in row.Cells)
			{
				var fieldCell = cell as DataControlFieldCell;
				if (fieldCell != null)
				{
					if (fieldCell.ContainingField.HeaderText.Equals(headerText))
					{
						return columnIndex;
					}
				}
				columnIndex++; // keep adding 1 while we don't have the correct name
			}
			return -1;
		}
		
		public int GetEnterpriseQueueRowStatusFromText(string text)
		{
			var rowStatus = 0;

			switch (HttpUtility.HtmlDecode(text))
			{
				// Note: Only values 0,1,2,3 exist in the db.  
				case "Awaiting Processing":
					rowStatus = 0;
					break;
				case "Being Processed":
					rowStatus = 1;
					break;
				case "Awaiting Response":
					rowStatus = 2;
					break;
				case "Error Processing":
					rowStatus = 3;
					break;

				// Status 4 is a derived value, a subset of status 2.
				case "Awaiting Response > 7 Days":
					rowStatus = 4;
					break;
			}

			return rowStatus;
		}
		
	}
}