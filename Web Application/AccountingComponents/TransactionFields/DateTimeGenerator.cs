//*****************************************************************************************************************
//  FILE NAME:		DateTimeGenerator.cs
//	PURPOSE:		This class inherits from the FieldGenerator class. It is an abstract class
//					to be derived when creating a date/time field.
//
//	COMMENTS:
//		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Endress+Hauser.
//
//	AUTHOR(S):	Thomas Beckum
//	VERSION:	1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:		By:					Reason:
//		----------	-----------------	-------------------------------------------
//		2006-11-06	Richard Panachida	Increased the date field lenght to 36 due to not potentially 
//										longer date/time (CSI 3603).
//*****************************************************************************************************************

namespace TransactionFields
{
	using System;
	using System.Web.UI;

	using FMControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for DateTimeGenerator.
	/// </summary>
	public abstract class DateTimeGenerator : FieldGenerator
	{
		private FMDateTime fmDateTime;

		public virtual bool ConvertToSiteTime
		{
			get
			{
				return true;
			}
		}


		public override void Generate(bool editable)
		{
			var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional, ID = this.ID + "Panel" };

			//Create DateTime 
			fmDateTime = new FMDateTime
			             {
				             ID = this.ID + " DateTime",
				             Enabled = (editable && this.Editable),
				             Visible = true
			             };

			fmDateTime.ToolTip = this.DisplayName;

			updatePanel.ContentTemplateContainer.Controls.Add(fmDateTime);
			this.cell.Controls.Add(updatePanel);

			object dateValue = GetDataValue();

			if (dateValue is DateTimeOffset)
			{
				var date = (DateTimeOffset)dateValue;

				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(transContext.security, trans.SiteGuid, false, false, false));

				if (ConvertToSiteTime)
				{
					var timeZoneInfo = site.GetTimeZoneInfo();
					date = TimeZoneInfo.ConvertTime(date, timeZoneInfo);
				}

				this.fmDateTime.Text = fieldGenerator.accountingSite.FormatDateTime(date);
			}
			else
			{
				this.fmDateTime.Text = string.Empty;
			}
		}

		/// <summary>
		/// This method overrides and implements the get new value method. It will throw
		/// an exception if the date is in the incorrect format or if the date field is
		/// required and no date is present.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override object GetNewValue(System.Web.UI.WebControls.WebControl control)
		{
			cell.BackColor = System.Drawing.Color.Red;

			if (this.Required && (fmDateTime.Text.Trim().Length == 0))
			{
				const string Msg = "Date is required.";
				throw new Exception(Msg);
			}

			cell.BackColor = System.Drawing.Color.Transparent;

			if (fmDateTime.Text.Trim().Length == 0)
				return null;

			try
			{
				return fmDateTime.CurrentValue;
			}
			catch (FormatException)
			{
				fmDateTime.Text = "";
				string msg = "Date Time Format is invalid.";
				throw new Exception(msg);
			}
		}

		public override string GetFormattedValue()
		{
			object dateValue = GetDataValue();
			if (dateValue == null)
			{
				return string.Empty;
			}
			if (dateValue is DateTimeOffset)
			{
				var date = (DateTimeOffset)dateValue;
				return this.fieldGenerator.accountingSite.FormatDate(date);
			}

			return null;
		}


		/// <summary>
		/// Update the FMDateTime control that is held in the TableCell control of this FieldGenerator.
		/// </summary>
		/// <param name="value"></param>
		public void SetDisplayValue(DateTimeOffset value)
		{
			if (this.cell == null)
			{
				return;
			}

			var fmdt = this.cell.Controls[0] as FMDateTime;

			if (fmdt != null)
			{
				fmDateTime.Text = fieldGenerator.accountingSite.FormatDateTime(value);
			}
		}
	}
}
