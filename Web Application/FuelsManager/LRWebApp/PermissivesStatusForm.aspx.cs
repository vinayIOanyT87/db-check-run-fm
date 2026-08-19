// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissivesStatusForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoadRackWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Globalization;
	using System.Net.Sockets;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
    using FMCore;

	using FMWebApp;

	using FuelsManager.FMWebApp;

	using Opc;
	using Opc.Da;

	using Convert = System.Convert;

	/// <summary>
	/// Code behind for Permissives Status Form.
	/// </summary>
	public partial class PermissivesStatusForm : FMFormBase
	{
		#region Methods

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.Exception">No IdentityGuid in Request</exception>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					string identityGuid = this.Request.GetQueryOrFormValue("IdentityGuid");
					if (string.IsNullOrEmpty(identityGuid))
					{
						throw new Exception("No IdentityGuid in Request.");
					}

					this.Session["StationGuid"] = identityGuid;

					ILoadRackManager loadRackManager = this.GetLoadRackManager();

					StationClass station = null;

					try
					{
						station = loadRackManager.GetStation(this.Security, Guid.Parse(input: this.Session["StationGuid"] as string));
					}
					catch (SocketException)
					{
						// vthompson 10/15/2008
						// Changed to catch the specific exception instead of checking the exception message
					}

					if (station == null)
					{
						throw new Exception("Station could not be retrieved");
					}

					this.TypeDropDownList.Items.Add(new ListItem("Station"));
					for (int armNumber = 0; armNumber < station.LoadArmCollection.Count; armNumber++)
					{
						this.TypeDropDownList.Items.Add(new ListItem("Arm " + (armNumber + 1).ToString(CultureInfo.InvariantCulture), armNumber.ToString(CultureInfo.InvariantCulture)));
					}
				}

				this.TypeDropDownList.Sort = false;

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);

				// Write script to close window
				this.Response.Write("<script language=\"jscript\">\r\n" + "<!--\r\nwindow.close();\r\n" + "\r\n-->\r\n</script>");
			}
		}

		/// <summary>
		/// Adds the permissive.
		/// </summary>
		/// <param name="permissivesDataTable">The PV data table.</param>
		/// <param name="permissive">The permissive.</param>
		private void AddPermissive(DataTable permissivesDataTable, ProcessVariableClass permissive)
		{
			DataRow dataRow = permissivesDataTable.NewRow();

			var url = new URL(permissive.URL);
			dataRow["Host"] = url.HostName;
			dataRow["OPCServerID"] = permissive.ProgID;
			dataRow["OPCItemID"] = permissive.OPCItemID;
			dataRow["CurrentValue"] = permissive.Encode(permissive.GetValue(0, 0), new Quality(permissive.OPCQuality), 0, null);
			if (permissive.Input)
			{
				dataRow["OutputFailed"] = string.Empty;
			}
			else
			{
				dataRow["OutputFailed"] = permissive.OutputFailed ? "True" : "False";
			}

			dataRow["MessageID"] = permissive.MessageID;

			permissivesDataTable.Rows.Add(dataRow);
		}

		/// <summary>
		/// Adds the permissives.
		/// </summary>
		/// <param name="permissivesDataTable">The PV data table.</param>
		/// <param name="permissives">The permissives.</param>
		private void AddPermissives(DataTable permissivesDataTable, PermissivesClass permissives)
		{
			foreach (ProcessVariableClass permissive in permissives.Outputs)
			{
				this.AddPermissive(permissivesDataTable, permissive);
			}

			foreach (ProcessVariableClass permissive in permissives.Inputs)
			{
				this.AddPermissive(permissivesDataTable, permissive);
			}
		}

		/// <summary>
		/// Enumerates the permissives status.
		/// </summary>
		/// <returns>A collection of permissive statuses.</returns>
		private ICollection EnumeratePermissivesStatus()
		{
			var permissivesDataTable = new DataTable();

			permissivesDataTable.Columns.Add("Host", typeof(string));
			permissivesDataTable.Columns.Add("OPCServerID", typeof(string));
			permissivesDataTable.Columns.Add("OPCItemID", typeof(string));
			permissivesDataTable.Columns.Add("CurrentValue", typeof(string));
			permissivesDataTable.Columns.Add("OutputFailed", typeof(string));
			permissivesDataTable.Columns.Add("MessageID", typeof(string));

			ILoadRackManager loadRackManager = this.GetLoadRackManager();

			StationClass station = null;

			try
			{
				station = loadRackManager.GetStation(this.Security, Guid.Parse(this.Session["StationGuid"] as string));
			}
			catch (SocketException)
			{
				// vthompson 10/15/2008
				// Changed to catch the specific exception instead of checking the exception message
			}

			if (station != null)
			{
				if (this.TypeDropDownList.SelectedItem.Text == "Station")
				{
					this.AddPermissives(permissivesDataTable, station.StationPermissives);
				}
				else
				{
					LoadArmClass loadArm = station.LoadArmCollection[Convert.ToInt32(this.TypeDropDownList.SelectedValue)];

					this.AddPermissives(permissivesDataTable, loadArm.LoadArmPermissives);

					this.AddPermissives(permissivesDataTable, loadArm.NoAdditivePermissives);

					foreach (ProductMapClass externalComponent in loadArm.ExternalComponentCollection)
					{
						this.AddPermissives(permissivesDataTable, externalComponent.Permissives);
					}

					foreach (ProductMapClass component in loadArm.ComponentCollection)
					{
						this.AddPermissives(permissivesDataTable, component.Permissives);
					}

					foreach (ProductMapClass additive in loadArm.AdditiveInjectorCollection)
					{
						this.AddPermissives(permissivesDataTable, additive.Permissives);
					}

					foreach (ProductMapClass recipe in loadArm.ProductRecipeCollection)
					{
						this.AddPermissives(permissivesDataTable, recipe.Permissives);
					}
				}
			}

			var permissivesStatusDataView = new DataView(permissivesDataTable);
			return permissivesStatusDataView;
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
			this.PermissivesStatusDataGrid.DataSource = this.EnumeratePermissivesStatus();
			this.PermissivesStatusDataGrid.DataBind();
		}

		#endregion
	}
}