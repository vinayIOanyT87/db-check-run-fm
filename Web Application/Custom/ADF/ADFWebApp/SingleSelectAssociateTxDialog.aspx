<%@ Page language="c#" Codebehind="SingleSelectAssociateTxDialog.aspx.cs" AutoEventWireup="True" Inherits="ADFWebApp.SingleSelectAssociateTxDialog" %>
<%@ Register assembly="FMControls" namespace="FMControls" tagprefix="FMCONTROLS" %>
<%@ Register TagPrefix="FM7Accounting" TagName="TransactionFilterControl" Src="../Accounting/TransactionFilterControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
	<HEAD>
		<base target="_self" />
		<title>FuelsManager - Select Associated Transactions</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../FuelsManager.css" rel="stylesheet">
		<script>
			// The following is needed to fix a multiple postback problem with
			// IE modal dialog windows.  In addition you have to set the target
			// property of the form tag to the window name
			window.name = "selectAssociated";
			
			// This function is called by the all company text box/button controls.
			function CompanySelect(role, CompanyTextBoxID)
			{
				var sFeatures		 = "dialogWidth: 855px; dialogHeight: 560px";
				var CompanyTextBox = document.getElementById(CompanyTextBoxID);
				var CompanyNameTextBox = document.getElementById("CompanyName" + CompanyTextBoxID);
				var result         = null;
				var ManagerString = null;
				var OwnerString = null;
				var ShipperString = null;
				var BillToString = null;
				var limitSelectionsBasedOnHierarchy = "false";
				
				if (role == "CARRIER")
				{
					var companyShipToTextBox = document.getElementById("TransactionFields.ShipToFG");
					if(companyShipToTextBox != null)
					{
						var shipToID=companyShipToTextBox.value;
						
						if (shipToID.substr(0, 1) == "<")
						{
							shipToID = "";
						}
				
						result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Unassigned=true&Role=" + role +  
																	"&Map=AUTHORIZED_CARRIER_MAP" + "&IDLink=" + encodeURIComponent(shipToID), "", sFeatures);
					}
					else
						result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Unassigned=true&Role=" + role, "", sFeatures);
					
				}
				else
				{
					limitSelectionsBasedOnHierarchy = document.getElementById("LimitSelectionsBasedOnHierarchy");
					if(limitSelectionsBasedOnHierarchy != null && limitSelectionsBasedOnHierarchy.value == "true")
					{
						ManagerString = document.getElementById("TransactionFields.ManagerFG");
						OwnerString = document.getElementById("TransactionFields.OwnerFG");
						ShipperString = document.getElementById("TransactionFields.ShipperFG");
						BillToString = document.getElementById("TransactionFields.BilltoFG");
							
						if(role == "MANAGER")
							result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?All=true&Role=" + role, 
																		"", sFeatures);
						else if(role == "OWNER" && ManagerString != null)
						   result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?All=true&Role=" + role +
																		"&UseHierarchy=true&ManagerST="+ManagerString.value, 
																		"", sFeatures);
						else if(role == "SHIPPER" && ManagerString != null && OwnerString != null)
						   result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?All=true&Role=" + role +
																		"&UseHierarchy=true&ManagerST="+ManagerString.value +
																		"&OwnerST="+OwnerString.value, 
																		"", sFeatures);
						else if(role == "CUSTOMER_BILLTO" && ManagerString != null && OwnerString != null && ShipperString != null)
						   result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?All=true&Role=" + role +
																		"&UseHierarchy=true&ManagerST="+ManagerString.value +
																		"&OwnerST="+OwnerString.value +
																		"&ShipperST="+ShipperString.value, 
																		"", sFeatures);
						else if(role == "CUSTOMER_SHIPTO" && ManagerString != null && OwnerString != null && ShipperString != null && BillToString != null)
						   result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?All=true&Role=" + role +
																		"&UseHierarchy=true&ManagerST="+ManagerString.value +
																		"&OwnerST="+OwnerString.value +
																		"&ShipperST="+ShipperString.value +
																		"&BillToST="+BillToString.value, 
																		"", sFeatures);
																		
						if(role == "MANAGER" && result != null && (ManagerString != result[0]))
						{
							CompanyTextBox.value = result[0];
							CompanyTextBox.title = result[1];
							__mydoPostBack( 'MANAGER_CHANGED', result[0] );
						}
						else if(role == "OWNER" && result != null && (OwnerString != result[0]))
						{
							CompanyTextBox.value = result[0];
							CompanyTextBox.title = result[1];
							__mydoPostBack( 'OWNER_CHANGED', result[0] );
						}
						else if(role == "SHIPPER" && result != null && (ShipperString != result[0]))
						{
							CompanyTextBox.value = result[0];
							CompanyTextBox.title = result[1];
							__mydoPostBack( 'SHIPPER_CHANGED', result[0] );
						}
						else if(role == "CUSTOMER_BILLTO" && result != null && (BillToString != result[0]))
						{
							CompanyTextBox.value = result[0];
							CompanyTextBox.title = result[1];
							__mydoPostBack( 'BILLTO_CHANGED', result[0] );
						}
					}
					else
					{
						result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Null=true&Role=" + role, 
																	"", sFeatures);
					}
				}

				if (result != null)
				{
					if (role == "CUSTOMER_SHIPTO")
					{
						//var companyShipToTextBox = document.getElementById("TransactionFields.ShipToFG");
						//var oldShipToValue = companyShipToTextBox.value;
					}

					if (role == "CARRIER")
					{
						var carrierTextBox  = document.getElementById("TransactionFields.CarrierFG");
						var oldCarrierValue = carrierTextBox.value;
					}
					
					CompanyTextBox.value = result[0];
					CompanyTextBox.title = result[1];
					CompanyNameTextBox.value = result[2];
					
					if ( role == "CUSTOMER_SHIPTO" )
					{
						//__mydoPostBack( 'SHIPTO_REFRESH', result[0] );
					}
				}
			}


			function ProductSelect(productTextBoxID) {
			   var sFeatures = "dialogWidth: 855px; dialogHeight: 560px";
			   var productTextBox = document.getElementById(productTextBoxID);
			   var result = null;
			   var companyShipToTextBox = document.getElementById("TransactionFields.ShipToFG");
			   var companyManagerTextBox = document.getElementById("TransactionFields.ManagerFG");
			   var companyID = "";

			   if (companyShipToTextBox == null) {
			      if (companyManagerTextBox != null) {
			         companyID = companyManagerTextBox.value + "|manager";
			      }
			   }
			   else {
			      companyID = companyShipToTextBox.value + "|shipto";
			   }

			   if (companyID.substr(0, 1) == "<") {
			      companyID = "";
			   }

			   result = window.showModalDialog("../FMWebApp/ProductSelectForm.aspx?Type=MAX_PRODUCT" +
					                                "&Map=MAX_MAP" + "&IDLink=" + encodeURIComponent(companyID), "", sFeatures);


			   if (result != null) {
			      productTextBox.value = result[0];
			      productTextBox.title = result[1];
			   }


			}	
					
			function SetSelected(selected)
			{
				var transGrid = document.getElementById("dgTransactions");
				if (transGrid != null)
				{
					for (index=1; index < transGrid.rows.length; index++)
					{
						transGrid.rows(index).cells(0).all[0].checked = selected;
					}
				}
			}
			
			function Cancel_Clicked()
			{
				window.returnValue = null;
				window.close();
			}
		</script>
	</HEAD>
	<body ms_positioning="GridLayout">
	<asp:image id="backImage" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" Runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
		<form id="Form1" method="post" runat="server" target="selectAssociated">
			<div style="Z-INDEX: 101; LEFT: 10px; WIDTH: 7.0in; POSITION: absolute; TOP: 10px">
				<table cellSpacing="1" cellPadding="1" width="100%" border="0">
					<tr>
						<td valign="top"><FM7Accounting:TransactionFilterControl id="filterControl" runat="server"></FM7Accounting:TransactionFilterControl></td>
						<td align="right" valign="top">&nbsp; 
                            <fmcontrols:fmbutton class="formfieldtitle" style="WIDTH: 67px" Runat="server" onclick="Cancel_Clicked" 
								Text="Cancel" CssClass="formfieldtitle"></fmcontrols:fmbutton></td>
					</tr>
					<tr>
						<td colSpan="2">&nbsp;</td>
					</tr>
					<tr>
						<td colSpan="2">&nbsp; </td>
					</tr>
					<tr>
						<td colSpan="2"><FMCONTROLS:FMDATAGRID id="dgTransactions" Runat="server" 
                                BackColor="White" CssClass="tabletext" BorderStyle="None"
								AutoGenerateColumns="True" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" 
                                BorderColor="White" Cellpadding="3"
								Width="8.5in">
								<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
								<SelectedItemStyle Font-Bold="true" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
								<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
								<HeaderStyle Font-Bold="true" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Selection">
										<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Font-Bold="False" 
                                            Font-Italic="False" Font-Overline="False" Font-Strikeout="False" 
                                            Font-Underline="False"></ItemStyle>
										<ItemTemplate>
											<FMCONTROLS:FMSelectLinkButton ID="selectLinkButton" runat="server" />
											<input type="hidden" runat="server" id="hidTransID" name="hidTransID" value='' />
											<input type="hidden" runat="server" id="hidLineItemID" name="hidLineItemID" value='' />
											<input type="hidden" runat="server" id="hidQty" name="hidQty" value='' />
											<asp:Label ID="labTransID" Runat="server"></asp:Label>
											<asp:Label ID="labLineItemID" Runat="server"></asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
							</FMCONTROLS:FMDATAGRID></td>
					</tr>
					<tr>
						<td colSpan="2">&nbsp; </td>
					</tr>
				</table>
			</div>
		</form>
	</body>
</html>
