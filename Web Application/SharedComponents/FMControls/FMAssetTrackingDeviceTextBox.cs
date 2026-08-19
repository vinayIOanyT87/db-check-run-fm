namespace FMControls
{
	using System;
	using System.Collections;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public class FMAssetTrackingDeviceTextBox : FMTextBoxButtonControl
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor the FMProductTextBox base class
		/// </summary>
		public FMAssetTrackingDeviceTextBox()
		{
		}
		#endregion

		/// <summary>
		/// This method will perform the page load by getting the asset tracking device
		/// information and setting the tooltip.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		override protected void Page_Load ( object sender, EventArgs e )
		{
			if (string.IsNullOrEmpty(this.Text) == false 
				&& this.Text != "{All}" 
				&& this.Text != "{Unassigned}"  
				&& this.Text != "{None}")
			{
				if (this.Page.Session["Security"] == null)
				{
					throw new ArgumentNullException ( "Security" );
				}

				SecurityClass security = this.Page.Session["Security"] as SecurityClass;

				if (security == null)
				{
					return;
				}

				Guid assetTrackingDeviceGuid = FMChannelHelper.MakeCall<IAssetTrackingDevices, Guid>(
																	x =>
																	x.GetIdentityGuid(security, this.Text)
															);

				var assetTrackingDevice = FMChannelHelper.MakeCall<IAssetTrackingDevices, AssetTrackingDeviceClass>(
																	x =>
																	x.GetByIdentityGuid(security, assetTrackingDeviceGuid)
															);

				this.ToolTip = assetTrackingDevice.AssetTrackingDeviceToolTip;
			}
			else
			{
				this.ToolTip = string.Empty;
			}
		}

		/// <summary>
		/// This method will render the text box and button control. It overrides the 
		/// web control.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render ( HtmlTextWriter writer )
		{
			writer.WriteBeginTag ( "input" );
			writer.WriteAttribute ( "name", this.UniqueID );
			writer.WriteAttribute ( "type", "text" );
			writer.WriteAttribute ( "value", HttpUtility.HtmlEncode(this.Text) );
			writer.WriteAttribute ( "readonly", "readonly" );

			if (this.AutoPostBack)
			{
				writer.WriteAttribute ( "onchange", "__doPostBack('" + this.UniqueID + "','')" );
			}

			if (!this.Enabled)
			{
				writer.WriteAttribute ( "disabled", "disabled" );
			}

			writer.WriteAttribute ( "id", this.UniqueID );
			writer.WriteAttribute ( "tabindex", "-1" );
			writer.WriteAttribute ( "title", HttpUtility.HtmlEncode(this.ToolTip));
			writer.WriteAttribute ( "class", this.CssClass );
			IEnumerator keys = this.Style.Keys.GetEnumerator();

			string style = "background:#DDDDDD;width:" + (this.Width.Value - 5) + "px";

			while (keys.MoveNext ( ))
			{
				string key = (string) keys.Current;
				style += ";" + key + ": " + this.Style[key];
			}

			writer.WriteAttribute ( "style", style );
			writer.Write ( HtmlTextWriter.SelfClosingTagEnd );
			writer.Write ( writer.NewLine );

			// Add the Select button
			writer.WriteBeginTag ( "input" );
			writer.WriteAttribute ( "class", "formfieldtitle" );

			// JS20100809 WI-14889 allow the read-only of this control to trigger
			// the disable behaviour of the button, leaving text readable
			if (!this.Enabled || this.ReadOnly)
			{
				writer.WriteAttribute ( "disabled", "disabled" );
			}

			keys = this.Style.Keys.GetEnumerator ( );
			style = "padding:0;width: 20px; height:20px";

			while (keys.MoveNext ( ))
			{
				string key = (string) keys.Current;

				if (key == "height")
				{
					continue;
				}

				if (key == "LEFT")
				{
					style += ";" + key + ": " + (Unit.Parse(this.Style[key]).Value + this.Width.Value + 5) + "px";
				}
				else
				{
					style += ";" + key + ": " + this.Style[key];
				}
			}

			writer.WriteAttribute ( "style", style );
			writer.WriteAttribute("onclick", "AssetTrackingDeviceSelect('" + this.UniqueID + "')");
			writer.WriteAttribute ( "type", "button" );
			writer.WriteAttribute ( "value", "..." );
			writer.WriteAttribute("id", this.UniqueID + " Select Button");
			writer.Write(HtmlTextWriter.SelfClosingTagEnd);
		}
	}
}
