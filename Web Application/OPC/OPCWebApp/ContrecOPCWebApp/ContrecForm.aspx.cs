/******************************************************************************

	FILE NAME:		ContrecForm.aspx.cs


	PURPOSE:			Implementation of ContrecForm


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	B. Schaal


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

using System;
using System.Net;
using System.Web.UI.WebControls;

using ContrecOPCObjectsLib;

using ContrecOPCServerLib;

using FMBusinessObjects.DataObjects;

using FMControls;

namespace OPCWebApp.ContrecOPCWebApp
{
   /// <summary>
   /// Summary description for ContrecForm.
   /// </summary>
   public partial class ContrecForm : ContrecFormBase
	{
      protected FMLabel Label12;

      protected void Page_Load(object sender, EventArgs e)
		{
         try
         {
            this.GetSecurity();

            if (!this.Page.IsPostBack)
            {
               this.Session.Remove("Contrec");

               ContrecClass contrec;


               // Get Index
               if (this.Session["Index"] != null)
               {
                  // Get Contrec
                  IContrecs contrecs = (IContrecs)OpcCom.Interop.CreateInstance(new Guid("{59DB8E98-D175-49A8-997B-8D342154B9D7}"),
                                                                                    (string)this.Session["ContrecSystem"],
                                                                                    new NetworkCredential());

                  contrec = (ContrecClass)contrecs.Get(Convert.ToInt32(this.Session["Index"] as string));

                  this.IDTextBox.Text = contrec.ID;
               }
               else
                  contrec = new ContrecClass();

               // Populate TypeDropDownList
               ListItem newItem;

               for (CONTREC_TYPE type = CONTREC_TYPE.CONTREC1010; type < CONTREC_TYPE.MAX_CONTREC_TYPE; type++)
               {
                  newItem = new ListItem(contrec.TypeID(type), ((int)type).ToString());
                  foreach (ListItem existingItem in this.TypeDropDownList.Items)
                  {
                     if (string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) < 0)
                     {
                        int index = this.TypeDropDownList.Items.IndexOf(existingItem);
                        this.TypeDropDownList.Items.Insert(index, newItem);
                        if (((int)contrec.Type).ToString() == newItem.Value)
                           this.TypeDropDownList.SelectedIndex = index;
                        newItem = null;
                        break;
                     }
                  }

                  if (newItem != null)
                  {
                     this.TypeDropDownList.Items.Add(newItem);
                     if (((int)contrec.Type).ToString() == newItem.Value)
                        this.TypeDropDownList.SelectedIndex = this.TypeDropDownList.Items.Count - 1;
                  }
               }

               // Populate PortDropDownList
               newItem = new ListItem(this.GetDictionaryText("{None}"), "0");
               this.PortDropDownList.Items.Add(newItem);

               IPorts ports = (IPorts)OpcCom.Interop.CreateInstance(new Guid("{2B2CCFD9-9EF7-48BB-BEF4-C58C0C43409D}"),
                                                                     (string)this.Session["ContrecSystem"],
                                                                     new NetworkCredential());

               PortCollectionClass portCollection = (PortCollectionClass)ports.Enumerate();
               for (int item = 0; item < portCollection.Count; item++)
               {
                  PortClass port = (PortClass)portCollection.Item(item);
                  newItem = new ListItem(port.ID, port.Index.ToString());
                  foreach (ListItem existingItem in this.PortDropDownList.Items)
                  {
                     if (string.Compare(existingItem.Text, newItem.Text, StringComparison.Ordinal) > 0)
                     {
                        int index = this.PortDropDownList.Items.IndexOf(existingItem);
                        this.PortDropDownList.Items.Insert(index, newItem);
                        if (contrec.PortIndex == port.Index)
                           this.PortDropDownList.SelectedIndex = index;
                        newItem = null;
                        break;
                     }
                  }

                  if (newItem != null)
                  {
                     this.PortDropDownList.Items.Add(newItem);
                     if (contrec.PortIndex == port.Index)
                        this.PortDropDownList.SelectedIndex = this.PortDropDownList.Items.Count - 1;
                  }
               }

               // Populate the Address DropDownList
               for (ushort address = 1; address < 256; address++)
               {
                  newItem = new ListItem(address.ToString(), address.ToString());
                  this.AddressDropDownList.Items.Add(newItem);
                  if (contrec.Address == address)
                     this.AddressDropDownList.SelectedIndex = this.AddressDropDownList.Items.Count - 1;
               }

               this.Session["Contrec"] = contrec;

               if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
                  this.OKButton.Enabled = false;
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
         this.CancelButton.Command += new CommandEventHandler(this.Cancel_Command);
         this.OKButton.Command += new CommandEventHandler(this.OK_Command);
      }
		#endregion

      private void OK_Command(object sender, CommandEventArgs e)
      {
         try
         {
            IContrecs contrecs = (IContrecs)OpcCom.Interop.CreateInstance(new Guid("{59DB8E98-D175-49A8-997B-8D342154B9D7}"),
                                                                              (string)this.Session["ContrecSystem"],
                                                                              new NetworkCredential());

            ContrecClass contrec = (ContrecClass)this.Session["Contrec"];

            contrec.ID = this.IDTextBox.Text;
            if (this.TypeDropDownList.SelectedIndex != -1)
               contrec.Type = (CONTREC_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);

            if (this.PortDropDownList.SelectedIndex != -1)
               contrec.PortIndex = Convert.ToInt32(this.PortDropDownList.SelectedItem.Value);

            if (this.AddressDropDownList.SelectedIndex != -1)
               contrec.Address = Convert.ToByte(this.AddressDropDownList.SelectedItem.Value);

            if (contrec.Index != 0)
               contrecs.Modify(contrec);
            else
               contrecs.Add(contrec);

         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
            return;
         }
         this.Redirect("ContrecsForm.aspx");
         this.Session.Remove("Contrec");
      }

      private void Cancel_Command(object sender, CommandEventArgs e)
      {
         this.Redirect("ContrecsForm.aspx");
         this.Session.Remove("Contrec");
      }

   }
}
