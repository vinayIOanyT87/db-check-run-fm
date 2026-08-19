namespace FuelsManager.FMWebApp
{
	using System;
	using System.Globalization;
	using System.Linq;
    using System.Security;
    using System.Web;
   using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

   public class FMUserControlBase : UserControl
	{

		#region Properties

		protected SecurityClass Security => ((FMFormBase)this.Page).Security;
		protected bool IsEnterprise => ((FMFormBase)this.Page).IsEnterprise;
      protected string ApplicationRoot = "~";

      public const string DROP_DOWN_NONE = "{None}";
      #endregion

		#region Public Methods and Operators

		/// <summary>
		/// The redirect.
		/// </summary>
		/// <param name="url">
		/// The url.
		/// </param>
      public void Redirect(string url)
		{
			try
			{
				this.Server.ClearError();
				((FMFormBase)this.Page).Redirect(url);
				this.Context.ApplicationInstance.CompleteRequest();
			}
			catch (Exception ex)
			{
				LogErrorMessage(ex.Message);
			}
		}

		public static void LogErrorMessage(string errorMessage)
		{
			// Log the error in the application event log
			try
			{
				FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(errorMessage, FMEventLogEntryType.Error));
			}
				// ReSharper disable once EmptyGeneralCatchClause
			catch
			{
			}
		}

		public ILoadRackManager GetLoadRackManager()
		{
			return ((FMFormBase)this.Page).GetLoadRackManager();
		}

		/// <summary>
		///    This method will return the data dictionary text if the data dictionary is in use. Otherwise, it returns the original text.
		/// </summary>
		/// <param name="inText">
		/// </param>
		/// <returns>
		///    The System.String.
		/// </returns>
		public string GetTranslatedText(string inText)
		{
			return ((FMFormBase)this.Page).GetTranslatedText(inText);
		}

		public string GetDataDictionaryValueByKey(Guid siteGuid, string inText)
		{
			return ((FMFormBase)this.Page).GetDataDictionaryValueByKey(siteGuid, inText);
		}

		#endregion

		#region Methods

		protected void ErrorHandler(Exception except)
		{
			((FMFormBase)this.Page).ErrorHandler(except);
		}

		[SecurityCritical]
		protected void InitializeUnitsDropDownList(
			DropDownList unitsDropDownList,
			EngineeringUnit beginningUnits,
			EngineeringUnit endingUnits,
			EngineeringUnit selectedUnits)
		{
			for (EngineeringUnit index = beginningUnits; index <= endingUnits; index++)
			{
				if (Enum.IsDefined(typeof(EngineeringUnit), index) == false)
				{
					continue;
				}

				string abbrevString;
				try
				{
					abbrevString = EngineeringUnits.GetUnitAbbreviation(index);
				}
				catch
				{
					continue;
				}

				var newUnitsListItem = new ListItem(abbrevString, ((int)index).ToString(CultureInfo.InvariantCulture));

				foreach (ListItem existingUnitsItem in unitsDropDownList.Items)
				{
					if (string.Compare(existingUnitsItem.Text, newUnitsListItem.Text, StringComparison.Ordinal) > 0)
					{
						int insert = unitsDropDownList.Items.IndexOf(existingUnitsItem);
						unitsDropDownList.Items.Insert(insert, newUnitsListItem);
						if (selectedUnits == index)
						{
							unitsDropDownList.SelectedIndex = insert;
						}

						newUnitsListItem = null;
						break;
					}
				}

				if (newUnitsListItem != null)
				{
					unitsDropDownList.Items.Add(newUnitsListItem);
					if (selectedUnits == index)
					{
						unitsDropDownList.SelectedIndex = unitsDropDownList.Items.Count - 1;
					}
				}
			}
		}

		protected void DisableControls()
		{
			DisableControls(this);
		}

		static protected void DisableControls(Control c)
		{
			Type t = c.GetType();
			System.Reflection.PropertyInfo p = t.GetProperty("Enabled");
		    p?.SetValue(c, false, null);

		    foreach (Control c1 in c.Controls)
			{
				DisableControls(c1);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);

			base.OnInit(e);

         ApplicationRoot = ResolveUrl("~");
         if (Session["ApplicationRoot"] == null)
         {
            Session["ApplicationRoot"] = ApplicationRoot;
            //FMFormBase.LogErrorMessage($"Menu Bar Application root1={ApplicationRoot}");
         }
      }

        public void PopulateDropDown(DropDownList dropDownList, string[] options,  string currentValue, bool addCurrent)
        {
            ListItem newItem;
            options = options.Distinct().OrderBy(p => p).ToArray();

            newItem = new ListItem(this.GetTranslatedText(DROP_DOWN_NONE), "0");
            dropDownList.Items.Add(newItem);

            int index = 1;
            foreach (string option in options)
            {
                newItem = new ListItem(option, index.ToString());
                dropDownList.Items.Add(newItem);
                if (currentValue == newItem.Text)
                {
                    dropDownList.SelectedIndex = dropDownList.Items.Count - 1;
                }
                index++;
            }

            // In case current selected option is temporarily unavailable
            // add as selected option so we dont lose value on save
            if (addCurrent && !string.IsNullOrWhiteSpace(currentValue) && !options.Contains(currentValue))
            {
                newItem = new ListItem(currentValue, index.ToString());
                dropDownList.Items.Add(newItem);
                dropDownList.SelectedIndex = dropDownList.Items.Count - 1;
            }
        }
        #endregion
    }
}