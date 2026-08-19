/******************************************************************************

	FILE NAME:		CardReaderForm.aspx.cs


	PURPOSE:			Implementation of CardReaderForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaAccuload.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;

using AcculoadOPCObjectsLib;

using AcculoadOPCServerLib;

using AcculoadOPCWebApp;

using FMBusinessObjects.DataObjects;

using FMControls;

using OpcCom;

namespace OPCWebApp.AcculoadOPCWebApp
{
   /// <summary>
   /// Summary description for CardReaderForm.
   /// </summary>
   public class CardReaderForm : AcculoadFormBase
   {
      protected Image Image1;
      protected DropDownList TypeDropDownList;
      protected DropDownList PortDropDownList;
      protected FMLabel UserNameRequiredLabel;
      protected FMLabel Label2;
      protected FMLabel Label1;
      protected TextBox IDTextBox;
      protected FMLabel Label12;
      protected FMButton OKButton;
      protected FMButton CancelButton;
      protected FMLabel Label3;
      protected DropDownList AddressDropDownList;
      protected FMLabel Label4;
      protected FMRadioButton SerialCommunicationsRadioButton;
      protected FMRadioButton NetworkCommunicationsRadioButton;
      protected TextBox IPAddressTextBox;

      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {
            if (!this.Page.IsPostBack)
            {
               this.GetSecurity();

               this.Session.Remove("CardReader");

               AcculoadClass cardReader;
               ArmCollectionClass arms;

               // Get Index
               if (this.Session["Index"] != null)
               {
                  // Get CardReader
                  IAcculoads Devices = (IAcculoads)Interop.CreateInstance(new Guid("{41D54854-8705-400A-9B22-F58B58088BE7}"),
                                                                                                      (string)Session["SmithMeterSystem"],
                                                                                                      new NetworkCredential());
                  cardReader = (AcculoadClass)Devices.Get(Convert.ToInt32(Session["Index"] as string));

                  this.IDTextBox.Text = cardReader.ID;
               }
               else
               {
                  cardReader = new AcculoadClass
                  {
                     Type = ACCULOAD_TYPE.SMITH_PROXIMITY
                  };
                  arms = (ArmCollectionClass)cardReader.Arms;
                  ArmClass arm = new ArmClass
                  {
                     Number = Convert.ToByte(arms.Count + 1)
                  };
                  arms.Add(arm);
               }


               // Populate AddressDropDownList
               ListItem newItem;
               int[] addresses = { 0, 1, 2, 3 };
               arms = (ArmCollectionClass)cardReader.Arms;
               foreach (int address in addresses)
               {
                  newItem = new ListItem(address.ToString(), address.ToString());
                  AddressDropDownList.Items.Add(newItem);
                  if (arms.Item(0).Address == address)
                     AddressDropDownList.SelectedIndex = AddressDropDownList.Items.Count - 1;
               }

               // Populate TypeDropDownList
               for (ACCULOAD_TYPE Type = ACCULOAD_TYPE.ACCULOAD_2_STD; Type < ACCULOAD_TYPE.MAX_ACCULOAD_TYPE; Type++)
               {
                  // Skip Types that are not supported
                  if (Type != ACCULOAD_TYPE.SMITH_PROXIMITY
                  && Type != ACCULOAD_TYPE.RCU_II_RCU)
                  {
                     continue;
                  }

                  newItem = new ListItem(cardReader.TypeID(Type), ((int)Type).ToString());
                  foreach (ListItem ExistingItem in TypeDropDownList.Items)
                  {
                     if (ExistingItem.Text.CompareTo(newItem.Text) < 0)
                     {
                        int Index = TypeDropDownList.Items.IndexOf(ExistingItem);
                        TypeDropDownList.Items.Insert(Index, newItem);
                        if (((int)cardReader.Type).ToString() == newItem.Value)
                        {
                           TypeDropDownList.SelectedIndex = Index;
                        }
                        newItem = null;
                        break;
                     }
                  }

                  if (newItem != null)
                  {
                     TypeDropDownList.Items.Add(newItem);
                     if (((int)cardReader.Type).ToString() == newItem.Value)
                        TypeDropDownList.SelectedIndex = TypeDropDownList.Items.Count - 1;
                  }
               }

               // Populate PortDropDownList					
               string ItemText = GetDictionaryText("<None>");

               newItem = new ListItem(ItemText, "0");
               PortDropDownList.Items.Add(newItem);

               IPorts ports = (IPorts)Interop.CreateInstance(new Guid("{2070F4BA-651D-4268-9F5A-1EBE0A137141}"),
                                                                     (string)this.Session["SmithMeterSystem"],
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
                        if (cardReader.PortIndex == port.Index)
                           this.PortDropDownList.SelectedIndex = index;
                        newItem = null;
                        break;
                     }
                  }

                  if (newItem != null)
                  {
                     this.PortDropDownList.Items.Add(newItem);
                     if (cardReader.PortIndex == port.Index)
                        this.PortDropDownList.SelectedIndex = this.PortDropDownList.Items.Count - 1;
                  }
               }

               NetworkCommunicationsRadioButton.Checked = cardReader.NetworkCommunications;
               SerialCommunicationsRadioButton.Checked = !cardReader.NetworkCommunications;

               if (cardReader.NetworkCommunications)
               {
                  IPAddressTextBox.Text = cardReader.IPAddress;
               }

               string itemText = this.GetDictionaryText("{None}");

               newItem = new ListItem(itemText, "0");
               this.PortDropDownList.Items.Insert(0, newItem);

               this.Session["CardReader"] = cardReader;

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
            IAcculoads Devices = (IAcculoads)Interop.CreateInstance(new Guid("{41D54854-8705-400A-9B22-F58B58088BE7}"),
                                                                                                (string)Session["SmithMeterSystem"],
                                                                                                new NetworkCredential());

            AcculoadClass CardReader = (AcculoadClass)Session["CardReader"];

            CardReader.ID = IDTextBox.Text.Trim();
            if (AddressDropDownList.SelectedIndex != -1)
            {
               ArmCollectionClass Arms = (ArmCollectionClass)CardReader.Arms;
               Arms.Item(0).Address = Convert.ToByte(AddressDropDownList.SelectedValue);
            }
            if (PortDropDownList.SelectedIndex != -1)
               CardReader.PortIndex = Convert.ToInt32(PortDropDownList.SelectedItem.Value);

            CardReader.NetworkCommunications = NetworkCommunicationsRadioButton.Checked;
            CardReader.IPAddress = IPAddressTextBox.Text.Trim();

            try
            {
               if (CardReader.Index != 0)
                  Devices.Modify(CardReader);
               else
                  Devices.Add(CardReader);
            }
            catch (COMException ex)
            {
               if (ex.Message.Contains("duplicate key") || ex.Message.Contains("Accuload Exists"))
               {
                  throw new Exception("OPC Server Exists");
               }
               throw new Exception("Database Error");
            }
         }

         catch (Exception except)
         {
            this.ErrorHandler(except);
            return;
         }
         this.Redirect("CardReadersForm.aspx");
         this.Session.Remove("CardReader");
      }

      private void Cancel_Command(object sender, CommandEventArgs e)
      {
         this.Redirect("CardReadersForm.aspx");
         this.Session.Remove("CardReader");
      }

      protected void SerialCommunicationsRadioButton_CheckedChanged(object sender, EventArgs e)
      {
         PortDropDownList.Enabled = SerialCommunicationsRadioButton.Checked;
         IPAddressTextBox.Enabled = !SerialCommunicationsRadioButton.Checked;
         if (SerialCommunicationsRadioButton.Checked)
         {
            IPAddressTextBox.Text = "";
         }
         else
            PortDropDownList.SelectedIndex = 0;
      }

      protected void NetworkCommunicationsRadioButton_CheckedChanged(object sender, EventArgs e)
      {
         PortDropDownList.Enabled = !NetworkCommunicationsRadioButton.Checked;
         IPAddressTextBox.Enabled = NetworkCommunicationsRadioButton.Checked;
         if (!NetworkCommunicationsRadioButton.Checked)
         {
            IPAddressTextBox.Text = "";
         }
         else
            PortDropDownList.SelectedIndex = 0;
      }

      protected void TypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
      {
         AcculoadClass CardReader = (AcculoadClass)Session["CardReader"];

         if (TypeDropDownList.SelectedIndex != -1)
         {
            CardReader.Type = (ACCULOAD_TYPE)Convert.ToInt32(TypeDropDownList.SelectedValue);
            if (CardReader.Type == ACCULOAD_TYPE.SMITH_PROXIMITY)
            {
               NetworkCommunicationsRadioButton.Enabled = false;
               NetworkCommunicationsRadioButton.Checked = false;
               NetworkCommunicationsRadioButton_CheckedChanged(null, null);
               SerialCommunicationsRadioButton.Checked = true;
               SerialCommunicationsRadioButton_CheckedChanged(null, null);
            }
            else
               NetworkCommunicationsRadioButton.Enabled = true;
         }
      }
   }
}
