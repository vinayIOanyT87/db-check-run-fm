
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	public class FMAutoSubmitFormBase : FMFormBase
	{
		#region Methods

		protected void FMAutoSubmitFormBaseLoad(object sender, EventArgs e)
		{
			// This is a simpler, more reliable, cross-browser way of setting the
			// default button that avoids writing javascript
			if (this.FindControl("OK") is Button)
			{
				this.Form.DefaultButton = "OK";
			}
			else if (this.FindControl("OKButton") is Button)
			{
				this.Form.DefaultButton = "OKButton";
			}
			else if (this.FindControl("FindBtn") is Button)
			{
				this.Form.DefaultButton = "FindBtn";
			}
			else if (this.FindControl("RefreshButton") is Button)
			{
				this.Form.DefaultButton = "RefreshButton";
			}
			else if (this.FindControl("AddButton") is Button)
			{
				this.Form.DefaultButton = "AddButton";
			}
			else if (this.FindControl("SaveButton") is Button)
			{
				this.Form.DefaultButton = "SaveButton";
			}
			else if (this.FindControl("CloseButton") is Button)
			{
				this.Form.DefaultButton = "CloseButton";
			}
		}

		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent()
		{
			this.Load += this.FMAutoSubmitFormBaseLoad;
		}

		#endregion
	}
}