/******************************************************************************

	FILE NAME:		DanLoadForm.aspx.cs


	PURPOSE:			Implementation of DanLoadForm


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

using System;
using System.Net;
using System.Web.UI.WebControls;
using DanielOPCObjectsLib;
using DanielOPCServerLib;
using FMBusinessObjects.DataObjects;

namespace OPCWebApp.DanielOPCWebApp
{
	/// <summary>
	/// Summary description for DanLoadForm.
	/// </summary>
	public partial class DanLoadForm : DanielFormBase
	{
		protected FMControls.FMLabel Label12;



		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (! this.Page.IsPostBack) 
				{
					this.Session.Remove("DanLoad");

					DanLoadClass	danLoad;


					// Get Index
					if(this.Session["Index"] != null)
					{
						// Get DanLoad
						IDanLoads danLoads=(IDanLoads) OpcCom.Interop.CreateInstance(	new Guid("{54F57ECB-6111-4A9A-AFA6-ABC5B3C4FF59}"),
																												(string) this.Session["DanielSystem"],
																												new NetworkCredential());

						danLoad=(DanLoadClass) danLoads.Get(Convert.ToInt32(this.Session["Index"] as string));

						this.IDTextBox.Text=danLoad.ID;
					}
					else
						danLoad=new DanLoadClass();

					// Populate TypeDropDownList
					ListItem newItem;
					
					for(DANLOAD_TYPE type=DANLOAD_TYPE.DANLOAD6000;type < DANLOAD_TYPE.MAX_DANLOAD_TYPE;type++)
					{
						newItem=new ListItem(danLoad.TypeID(type),((int) type).ToString());
						foreach(ListItem existingItem in this.TypeDropDownList.Items)
						{
							if(string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) < 0)
							{
								int index=this.TypeDropDownList.Items.IndexOf(existingItem);
								this.TypeDropDownList.Items.Insert(index,newItem);
								if(((int) danLoad.Type).ToString() == newItem.Value)
									this.TypeDropDownList.SelectedIndex=index;
								newItem=null;
								break;
							}
						}

						if(newItem != null)
						{
							this.TypeDropDownList.Items.Add(newItem);
							if(((int) danLoad.Type).ToString() == newItem.Value)
								this.TypeDropDownList.SelectedIndex=this.TypeDropDownList.Items.Count-1;
						}
					}

					// Populate PortDropDownList
					newItem=new ListItem( this.GetDictionaryText("{None}"),"0" );
					this.PortDropDownList.Items.Add(newItem);
					
					IPorts ports=(IPorts) OpcCom.Interop.CreateInstance(	new Guid("{265331A0-40D0-4DEC-B614-1A21CDC5CC1F}"),
																							(string) this.Session["DanielSystem"],
																							new NetworkCredential());

					PortCollectionClass portCollection=(PortCollectionClass) ports.Enumerate();
					for(int item=0;item < portCollection.Count;item++)
					{
						PortClass port=(PortClass) portCollection.Item(item);
						newItem=new ListItem(port.ID,port.Index.ToString());
						foreach(ListItem existingItem in this.PortDropDownList.Items)
						{
							if(string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) > 0)
							{
								int index=this.PortDropDownList.Items.IndexOf(existingItem);
								this.PortDropDownList.Items.Insert(index,newItem);
								if(danLoad.PortIndex == port.Index)
									this.PortDropDownList.SelectedIndex=index;
								newItem=null;
								break;
							}
						}

						if(newItem != null)
						{
							this.PortDropDownList.Items.Add(newItem);
							if(danLoad.PortIndex == port.Index)
								this.PortDropDownList.SelectedIndex=this.PortDropDownList.Items.Count-1;
						}
					}

					// Populate the Address DropDownList
					for(ushort address=1;address < 256;address++)
					{
						newItem=new ListItem(address.ToString(),address.ToString());
						this.AddressDropDownList.Items.Add(newItem);
						if(danLoad.Address == address)
							this.AddressDropDownList.SelectedIndex=this.AddressDropDownList.Items.Count-1;
					}

					this.Session["DanLoad"]=danLoad;

					if(!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
						this.OKButton.Enabled=false;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}




		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.CancelButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.Cancel_Command);
			this.OKButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OK_Command);

		}
		#endregion

		private void OK_Command(object sender, CommandEventArgs e)
		{
			try
			{
				IDanLoads danLoads=(IDanLoads) OpcCom.Interop.CreateInstance(	new Guid("{54F57ECB-6111-4A9A-AFA6-ABC5B3C4FF59}"),
																										(string) this.Session["DanielSystem"],
																										new NetworkCredential());

				DanLoadClass	danLoad=(DanLoadClass) this.Session["DanLoad"];

				danLoad.ID=this.IDTextBox.Text;
				if(this.TypeDropDownList.SelectedIndex != -1)
					danLoad.Type=(DANLOAD_TYPE) Convert.ToInt32(this.TypeDropDownList.SelectedValue);

				if(this.PortDropDownList.SelectedIndex != -1)
					danLoad.PortIndex=Convert.ToInt32(this.PortDropDownList.SelectedItem.Value);

				if(this.AddressDropDownList.SelectedIndex != -1)
					danLoad.Address=Convert.ToByte(this.AddressDropDownList.SelectedItem.Value);

				if(danLoad.Index != 0)
					danLoads.Modify(danLoad);
				else
					danLoads.Add(danLoad);

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}
			this.Redirect("DanLoadsForm.aspx");
			this.Session.Remove("DanLoad");
		}

		private void Cancel_Command(object sender, CommandEventArgs e)
		{
			this.Redirect("DanLoadsForm.aspx");
			this.Session.Remove("DanLoad");
		}

		protected void TypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

	}
}
