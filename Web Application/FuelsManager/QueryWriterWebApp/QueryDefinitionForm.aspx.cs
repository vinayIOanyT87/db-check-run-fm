// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryDefinitionForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.QueryWriterWebApp
{
	using System;

	using FMBusinessObjects.DataObjects;

    using FMCore;

	using FuelsManager.FMWebApp;


	public partial class QueryDefinitionForm : FMFormBaseAjax
	{
		#region Constants and Fields

		public static string QuerywriterQueryObject = "QueryWriter.QueryDefinitionForm.QueryObject";

		#endregion

		#region Public Properties

		public QueryDefinitionAdvanced QueryDefinitionAdvancedPage
		{
			get
			{
				return this.QueryDefinitionAdvancedPage1;
			}
		}

		#endregion

		#region Methods

		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					this.Session.Remove(QuerywriterQueryObject);

					QueryClass query;

					if (this.Request.GetQueryOrFormValue("Mode").DefaultIfNull("").Equals("Edit"))
					{
						query = (QueryClass)this.Session[ManageQueriesForm.ManageQueriesObject];
						this.Session[QueryDefinitionBasic.QuerywriterQueryTopic] = query.Topic;

						//Set the title label with a key field from the bound object appended
						if (query != null)
						{
							this.PageTitle.Text = this.GetTitleLabelText(this.PageTitle.Text, query.QueryName);
						}
					}
					else
					{
						// Create a new Query object
						query = new QueryClass();
					}

					this.Session.Add(QuerywriterQueryObject, query);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion
	}

	public class QueryPageBase : FMUserControlBase
	{
		#region Properties

		protected new SecurityClass Security
		{
			get
			{
				return ((QueryDefinitionForm)this.Page).Security;
			}
		}

		#endregion
	}
}