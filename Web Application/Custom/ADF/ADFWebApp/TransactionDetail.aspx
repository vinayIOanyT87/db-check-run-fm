<%@ Page language="c#" Codebehind="TransactionDetail.aspx.cs" AutoEventWireup="True" Inherits="ADFWebApp.TransactionDetail" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="fmcontrols" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml" >
	<head id="Head1" runat="server">
		<title>TransactionDetail</title>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
		<link href="../FuelsManager.css" rel="stylesheet"/>
		<script type="text/javascript" language="javascript" src="../Accounting/TransactionDetail_min.js"></script>
	</head>
	<body MS_POSITIONING="GridLayout" xmlns:FMControls="urn:http://schemas.varec.com/FMControls">
		<form id="Form1" method="post" runat="server" SubmitDisabledControls="true" onsubmit="formSubmit();">
			<asp:ScriptManager ID="ScriptManager" runat="server" />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
               <ContentTemplate>
 				<input type="hidden" name="__MYEVENTTARGET">
				<input type="hidden" name="__MYEVENTARGUMENT">
				<script>
					// This function is called by the all company text box/button controls.
					function CompanySelect(role, subrole, CompanyTextBoxID)
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
							result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&SubRole=" + subrole, "", sFeatures);

							if (CompanyTextBox.value != result[0]){
								CompanyTextBox.value = result[0];
								CompanyTextBox.title = result[1];
								CompanyNameTextBox.value = result[2];
								__doPostBack(CompanyTextBox.id, result[0]);
								return;
							}
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
									result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&SubRole=" + subrole, 
																				"", sFeatures);
								else if(role == "OWNER" && ManagerString != null)
									result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&SubRole=" + subrole +
																				"&UseHierarchy=true&ManagerST="+ManagerString.value, 
																				"", sFeatures);
								else if(role == "SHIPPER" && ManagerString != null && OwnerString != null)
									result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&SubRole=" + subrole +
																				"&UseHierarchy=true&ManagerST="+ManagerString.value +
																				"&OwnerST="+OwnerString.value, 
																				"", sFeatures);
								else if(role == "CUSTOMER_BILLTO" && ManagerString != null && OwnerString != null && ShipperString != null)
									result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&SubRole=" + subrole +
																				"&UseHierarchy=true&ManagerST="+ManagerString.value +
																				"&OwnerST="+OwnerString.value +
																				"&ShipperST="+ShipperString.value, 
																				"", sFeatures);
								else if(role == "CUSTOMER_SHIPTO" && ManagerString != null && OwnerString != null && ShipperString != null && BillToString != null)
									result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&SubRole=" + subrole +
																				"&UseHierarchy=true&ManagerST="+ManagerString.value +
																				"&OwnerST="+OwnerString.value +
																				"&ShipperST="+ShipperString.value +
																				"&BillToST="+BillToString.value, 
																				"", sFeatures);
								
								if(role == "MANAGER" && result != null && (ManagerString != result[0]))
								{
									CompanyTextBox.value = result[0];
									CompanyTextBox.title = result[1];
									__doPostBack(CompanyTextBox.id, result[0]);
								}
								else if(role == "OWNER" && result != null && (OwnerString != result[0]))
								{
									CompanyTextBox.value = result[0];
									CompanyTextBox.title = result[1];
									__doPostBack(CompanyTextBox.id, result[0]);
								}
								else if(role == "SHIPPER" && result != null && (ShipperString != result[0]))
								{
									CompanyTextBox.value = result[0];
									CompanyTextBox.title = result[1];
									__doPostBack(CompanyTextBox.id, result[0]);
								}
								else if(role == "CUSTOMER_BILLTO" && result != null && (BillToString != result[0]))
								{
									CompanyTextBox.value = result[0];
									CompanyTextBox.title = result[1];
									__doPostBack(CompanyTextBox.id, result[0]);
								}
							}
							else
							{
								result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&SubRole=" + subrole, 
																			"", sFeatures);
							}
						}

						if (result != null)
						{
							CompanyTextBox.value = result[0];
							CompanyTextBox.title = result[1];
							CompanyNameTextBox.value = result[2];
							
							if ( role == "CUSTOMER_SHIPTO" )
							{
								__mydoPostBack( 'SHIPTO_REFRESH', result[0] );
							}
						}
					}

					function FuelCardSelect(fuelCardTextBoxID) {
						var sFeatures = "dialogWidth: 8.81in; dialogHeight: 6in";
						var fuelCardTextBox = document.getElementById(fuelCardTextBoxID);
						var result = null;
						var fuelCardID = "";

						result = window.showModalDialog("../FMWebApp/FuelCardSelectForm.aspx?Null=true", "", sFeatures);

						if (result != null) {
							fuelCardTextBox.value = result[0];
							fuelCardTextBox.title = result[1];
							__doPostBack(fuelCardTextBox.id, '');
						}
					}				
					
					function __mydoPostBack(eventTarget, eventArgument) 
					{
						var theform;
						if (window.navigator.appName.toLowerCase().indexOf("microsoft") > -1) {
							theform = document.Form1;
						}
						else {
							theform = document.forms["Form1"];
						}
						theform.__MYEVENTTARGET.value = eventTarget.split("$").join(":");
						theform.__MYEVENTARGUMENT.value = eventArgument;
						
						// Display a wait message
						var waitImage = document.getElementById("waitDiv");
						waitImage.style.display = "inline";
						theform.submit();
					}
					
					function formSubmit()
					{
						// Display a wait message
						var waitImage = document.getElementById("waitDiv");
						waitImage.style.display = "inline";
					}

					
					function ProductSelect(productTextBoxID)
					{
						var sFeatures				  = "dialogWidth: 855px; dialogHeight: 560px";
						var productTextBox        = document.getElementById(productTextBoxID);
						var result                = null;
						var companyShipToTextBox  = document.getElementById("TransactionFields.ShipToFG");
						var companyManagerTextBox = document.getElementById("TransactionFields.ManagerFG");
						var companySupplierTextBox = document.getElementById("TransactionFields.SupplierFG");
						var companyID = "";
						
						if (companyShipToTextBox == null)
						{
							if (companySupplierTextBox != null)
							{
								companyID = companySupplierTextBox.value + "|supplier";
							}
							else if (companyManagerTextBox != null)
							{
								companyID = companyManagerTextBox.value + "|manager";		
							}
						}
						else
						{
							companyID = companyShipToTextBox.value + "|shipto";
						}
						
						if (companyID.substr(0, 1) == "<")
						{
							companyID = "";
						}

						result = window.showModalDialog("../FMWebApp/ProductSelectForm.aspx?Type=MAX_PRODUCT" + 
																  "&Map=MAX_MAP" + "&IDLink=" + encodeURIComponent(companyID), "", sFeatures);
	 
						                                    
						if (result != null)
						{
							productTextBox.value = result[0];
							productTextBox.title = result[1];

							__mydoPostBack('PRODUCT_REFRESH', result[0]);
			            }

		
					}

					function OperatorSelect(operatorTextBoxID)
					{
						var sFeatures		   = "dialogWidth: 855px; dialogHeight: 560px";
 						var operatorTextBox  = document.getElementById(operatorTextBoxID);
						var result           = null;
						var carrierTextBox	= document.getElementById("TransactionFields.CarrierFG");
						var carrierID			= "";
						var driverIdentificationNumberTextBox = document.getElementById("TransactionFields.DriverIdentificationNumberFG");
						
						if (carrierTextBox != null)
						{
							carrierID = carrierTextBox.value;
							
							if (carrierID.substr(0, 1) == "<")
							{
								carrierID = "";
							}
						}

						result = window.showModalDialog("../FMWebApp/PersonSelectForm.aspx?Role=MAX_PERSON_ROLE" + 
																  "&IDCarrierLink=" + encodeURIComponent(carrierID), "", sFeatures);
	 
						                                    
						if (result != null)
						{
							operatorTextBox.value = result[0];
							operatorTextBox.title = result[1];
							if(driverIdentificationNumberTextBox != null)
								driverIdentificationNumberTextBox.value = result[2];
						}
					}
					
					function TankSelect(tankTextBoxID)
					{
						var sFeatures		 = "dialogWidth: 855px; dialogHeight: 560px";
						var tankTextBox    = document.getElementById(tankTextBoxID);
						var result         = null;
						var productID      = "";
						var productTextBox = null;
						var prodIDListID   = tankTextBoxID.replace("StorageLocation", "Product");

						if (prodIDListID != null)
						{
						   productTextBox = document.getElementById(prodIDListID);

						   if (productTextBox == null)
						   {
						      if (prodIDListID.indexOf("ToProduct") > -1)
						      {
						         prodIDListID = prodIDListID.replace("ToProduct", "Product");
						      }
						      else if (prodIDListID.indexOf("FromProduct") > -1)
						      {
						         prodIDListID = prodIDListID.replace("FromProduct", "Product");
						      }

						      productTextBox = document.getElementById(prodIDListID);
						   }
						}
						
						if (productTextBox != null)
						{
							productID = productTextBox.value;
							
							if (productID.substr(0, 1) == "<")
								productID = "";
						}

						var managerTextBox = document.getElementById("TransactionFields.ManagerFG");
						var managerID      = "";
						
						if (managerTextBox != null)
							managerID = managerTextBox.value;		


						result = window.showModalDialog("../FMWebApp/TankSelectForm.aspx?IDProductLink=" + encodeURIComponent(productID)+
																	"&IDManagerLink=" + encodeURIComponent(managerID), 
																	"", sFeatures);
	 
						                                    
						if (result != null)
						{
							tankTextBox.value = result[0];
							tankTextBox.title = result[1];
							__mydoPostBack('TANK_REFRESH', result[0]);
						}
					}

					function EquipmentSelect(equipmentTextBoxID)
					{
						var sFeatures			= "dialogWidth: 855px; dialogHeight: 560px";
						var equipmentTextBox = document.getElementById(equipmentTextBoxID);
						var result           = null;
						var carrierToTextBox = document.getElementById("TransactionFields.CarrierFG");
						var carrierID = null;
						var transactionTypeTextBox = document.getElementById("TransactionFields.TransAliasFG");
						var shipToID = null;
						var shipToTextBox = document.getElementById("TransactionFields.ShipToFG");
						var toShipToTextBox = document.getElementById("TransactionFields.ToShipToFG");
						
						if (carrierToTextBox != null)
						{
							carrierID = carrierToTextBox.value;
						 }
						
						if (equipmentTextBoxID == "TransactionFields.DestinationEquipmentFG2")
						{
							 if (toShipToTextBox != null)
							 {
								  shipToID = toShipToTextBox.value;
							 }
						}
						else
						{
							 if (shipToTextBox != null)
							 {
								  shipToID = shipToTextBox.value;
							 }
						}

						result = window.showModalDialog("../FMWebApp/EquipmentSelectForm.aspx?EquipmentTextBoxID="+equipmentTextBoxID + 
																  ((carrierID != null) ? "&IDCarrierLink=" + encodeURIComponent(carrierID) : "") +
																  ((shipToID != null) ? "&IDShipToLink=" + encodeURIComponent(shipToID) : ""), "", sFeatures)

						if (result != null && result.length > 1 )
						{
							equipmentTextBox.value = result[0];
							equipmentTextBox.title = result[1];

							if ((transactionTypeTextBox != null) &&
								((equipmentTextBoxID == "TransactionFields.DestinationEquipmentFG1") 
								|| (equipmentTextBoxID == "TransactionFields.SourceEquipmentFG1")))
								{
								   var transactionTypeID = transactionTypeTextBox.value;
								   
								   if (transactionTypeID != null &&
								      (transactionTypeID.substr(0, 5) == "Issue"
								       || transactionTypeID.substr(0, 4) == "Sale"
								       || transactionTypeID.substr(0, 10) == "Commercial"
								       || transactionTypeID.substr(0, 20) == "Direct Fuel Purchase"
								       || transactionTypeID.substr(0, 6) == "Defuel")) 
								   {
									   __mydoPostBack('TAIL_NUMBER_CHANGED', equipmentTextBox.value);
								   }
							}
						}

					}

					function CompartmentSelect(compartmentTextBoxID)
					{
						var sFeatures				= "dialogWidth: 855px; dialogHeight: 560px";
						var compartmentTextBox  = document.getElementById(compartmentTextBoxID);
						var result           = null;
						var equipmentTextBoxID = compartmentTextBoxID.replace("CompartmentID_FG","EquipmentFG0");
						var equipmentTextBox  = document.getElementById(equipmentTextBoxID);
						var equipmentID="";

						if (equipmentTextBox != null)
							equipmentID = equipmentTextBox.value;

						// Line Item equipment isn't configured, use last equipment in header
						// the idea here is that the last equipment will be the one with compartments
						else
						{
							if(compartmentTextBoxID.indexOf("Destination") == -1)
							{
								equipmentTextBox=document.getElementById("TransactionFields.SourceEquipmentFG3");
								if(equipmentTextBox == null)
									equipmentTextBox=document.getElementById("TransactionFields.SourceEquipmentFG2");
								if(equipmentTextBox == null)
									equipmentTextBox=document.getElementById("TransactionFields.SourceEquipmentFG1");
							}
							else
							{
								equipmentTextBox=document.getElementById("TransactionFields.DestinationEquipmentFG3");
								if(equipmentTextBox == null)
									equipmentTextBox=document.getElementById("TransactionFields.DestinationEquipmentFG2");
								if(equipmentTextBox == null)
									equipmentTextBox=document.getElementById("TransactionFields.DestinationEquipmentFG1");
							}
							if (equipmentTextBox != null)
								equipmentID = equipmentTextBox.value;
						}
						
						result = window.showModalDialog("../FMWebApp/CompartmentSelectForm.aspx?EquipmentID=" + encodeURIComponent(equipmentID), "", sFeatures);
						                                    
						if (result != null)
						{
							compartmentTextBox.value = result[0];
							compartmentTextBox.title = result[1];
						}
					}

					function InstructionsButton_Click ( ItemIndex )
					{
						var sFeatures = "dialogWidth: 725px;dialogHeight: 530px;status: No;";
						var Result = window.showModalDialog("../FMWebApp/SpecialInstructionsForm.aspx?mode=txdetail&ItemIndex=" + ItemIndex, "", sFeatures );
					}
					
					function AssociateTx(lineItemIndex, aggregate)
					{
					    // product
						var productTextBox = document.getElementById("LineItemDataGrid$" + lineItemIndex +
									".-1$TransactionFields.LineItemProductFG");
						if (productTextBox == null)
							productTextBox = document.getElementById("LineItemDataGrid_" + lineItemIndex + ".-1_TransactionFields.LineItemProductFG_TextBox");
						// delivery location
						var deliveryLocationTextBox = document.getElementById("LineItemDataGrid$" + lineItemIndex +
									".-1$TransactionFields.LineItemDeliveryLocationFG");
						if (deliveryLocationTextBox == null)
							deliveryLocationTextBox = document.getElementById("LineItemDataGrid_" + lineItemIndex + ".-1_TransactionFields.LineItemDeliveryLocationFG_TextBox");
						var deliveryLocationValue = "";
						if (deliveryLocationTextBox != null) {
						    deliveryLocationValue = encodeURIComponent(deliveryLocationTextBox.value);
						}
						
						var currencyParam = "";
						var ddlCurrency = document.getElementById("LineItemDataGrid_" + lineItemIndex + ".-1_TransactionFields.LineItemCurrencyUnitFG");
						if (ddlCurrency != null)
						{
                            var selectedCurrency = ddlCurrency.selectedIndex;
                            if (selectedCurrency >= 0) {
                                currencyParam = ddlCurrency.options[selectedCurrency].text;
                            }
                        }

						if (productTextBox == null) {
							alert("Product Textbox not found.");
						}
						else if (productTextBox.value == "") {
							alert("Please select a Fuel Type.");
						}
						else {
						    var sFeatures = "dialogWidth: 855px;dialogHeight: 530px;status: yes;";
						    var result = null;
						    result = window.showModalDialog("../Accounting/SelectAssociatedTxDialog.aspx?lineItemID=" + lineItemIndex +
								"&product=" + encodeURIComponent(productTextBox.value) + "&deliveryLocation=" + deliveryLocationValue // JS20100316 delivery location fix
								+ "&currency=" + currencyParam // JS20100607 WI-14861
								, "", sFeatures);
							
							// Retrieve the hidden field holding the transaction id's
   						if (result != null )
							{
								__mydoPostBack('ASSOCIATIONS_CHANGED', lineItemIndex);
							}
						}
					}

				</script>
				<asp:button id="EnterKeyButton" style="Z-INDEX: -111; LEFT: 8px; POSITION: absolute; TOP: 0px"
					runat="server" Height="0px" ForeColor="Transparent" BorderStyle="None" Width="0px" BackColor="Transparent" onclick="EnterKeyButton_Click"></asp:button><input id="EnterKeySource" style="LEFT: 24px; TOP: 16px" type="hidden" runat="server">
				<asp:image id="FadeImage" style="Z-INDEX: -101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
					BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image>
				<TABLE id="TransDetailTable" style="Z-INDEX: 104; LEFT: 24px; TOP: 39px" runat="server">
					<tr id="PreviousNextRow">
						<td align="left">
						   <FMCONTROLS:FMBUTTON id="PreviousButton" style="Z-INDEX: 101" runat="server" Width="152px" Text="<< Previous Transaction" onclick="PreviousButton_Click" cssstyle="formfieldtitle" />
                     &nbsp;&nbsp;
  						   <fmcontrols:fmbutton id="NextButton" style="Z-INDEX: 103" runat="server" Width="136px" Text="Next Transaction >>" onclick="NextButton_Click" cssstyle="formfieldtitle" />
						</td>
						<td align="left">
						</td>
						<TD align="left"></TD>
					</tr>
					<TR id="FieldRow">
						<TD id="FieldRowCell" colSpan="2"><asp:table id="FieldTable" Runat="server"></asp:table></TD>
						<TD></TD>
					</TR>
					<tr id="GaugeReadingsRow">
						<td><FMCONTROLS:FMBaseDataGrid onkeypress="javascript:DataGridKeyPress('AGR');" id="GaugeReadingsDataGrid" style="LEFT: 1px; TOP: 0px"
								tabIndex="1" runat="server" BorderStyle="None" BackColor="White" PageSize="8" CssClass="tabletext"
								CellPadding="3" BorderColor="#999999" AllowSorting="True" BorderWidth="1px" GridLines="Vertical"
								AutoGenerateColumns="False">
								<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
								<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
								<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Edit">
										<HeaderStyle Width="55px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<fmcontrols:fmeditlinkbutton ID="EditButton2" runat="server" />
										</ItemTemplate>
										<EditItemTemplate>
											<fmcontrols:fmupdatelinkbutton ID="UpdateButton2" runat="server" />&nbsp;
											<fmcontrols:fmcancellinkbutton ID="CancelButton2" runat="server" />
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Delete">
										<HeaderStyle Width="0.5in"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<fmcontrols:fmdeletelinkbutton runat="server" ID="DeleteButton2" />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
							</FMCONTROLS:FMBaseDataGrid><fmcontrols:fmbutton id="NewAGRButton" style="Z-INDEX: 108; LEFT: 0px; POSITION: relative; TOP: 0px"
								runat="server" Text="Add" onclick="AGRNewButton_Click"></fmcontrols:fmbutton></td>
						<TD></TD>
					</tr>
					<tr id="LineItemPageRow">
						<td colSpan="3"><FMCONTROLS:FMBaseDataGrid onkeypress="javascript:DataGridKeyPress('LineItem');" id="LineItemDataGrid" style="LEFT: 1px; TOP: 0px"
								tabIndex="1" runat="server" BorderStyle="None" BackColor="White" PageSize="8" CssClass="tabletext" CellPadding="3"
								BorderColor="#999999" AllowSorting="True" BorderWidth="1px" GridLines="Vertical" AutoGenerateColumns="False">
								<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
								<SelectedItemStyle Wrap="False"></SelectedItemStyle>
								<EditItemStyle CssClass="tabletext"></EditItemStyle>
								<AlternatingItemStyle CssClass="tabletext" BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle Wrap="False" ForeColor="Black" CssClass="tabletext" BackColor="#EEEEEE"></ItemStyle>
								<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Add Subline-item">
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<fmcontrols:fmaddsublineitemlinkbutton runat="server" />
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Edit">
										<HeaderStyle Width="55px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<fmcontrols:fmeditlinkbutton runat="server" />
										</ItemTemplate>
										<EditItemTemplate>
											<fmcontrols:fmupdatelinkbutton runat="server" />&nbsp;
											<fmcontrols:fmcancellinkbutton runat="server" />
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Delete">
										<HeaderStyle Width="0.5in"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<fmcontrols:fmdeletelinkbutton runat="server" />
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Transactions">
										<HeaderStyle Width="0.5in"></HeaderStyle>
										<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<fmcontrols:fmviewassociatedtxlinkbutton id="FMViewAssociatedTxLinkButton1" 
												runat="server"></fmcontrols:fmviewassociatedtxlinkbutton>&nbsp;&nbsp;
											<fmcontrols:fmaddassociatedtxlinkbutton id="lbAddAssociatedTx" runat="server"></fmcontrols:fmaddassociatedtxlinkbutton>
											<fmcontrols:fmelipsebutton ID="btnAddAssocTx" Runat="server" 
												CssClass="formfieldtitle" Enabled="false"></fmcontrols:fmelipsebutton>
											<fmcontrols:fmelipsebutton ID="btnAddAssocSingleTx" Runat="server"
											    CssClass="formfieldtitle" Enabled="false"></fmcontrols:fmelipsebutton>
										</ItemTemplate>
										<EditItemTemplate>
											<fmcontrols:fmviewassociatedtxlinkbutton id="FMViewAssociatedTxLinkButton1" 
												runat="server"></fmcontrols:fmviewassociatedtxlinkbutton>&nbsp;&nbsp;
											<fmcontrols:fmaddassociatedtxlinkbutton id="lbAddAssociatedTx2" Enabled="false" 
												runat="server"></fmcontrols:fmaddassociatedtxlinkbutton>
											<fmcontrols:fmelipsebutton ID="btnAddAssocTx" Runat="server" 
												CssClass="formfieldtitle" Enabled="true"></fmcontrols:fmelipsebutton>
											<fmcontrols:fmelipsebutton ID="btnAddAssocSingleTx" Runat="server"
											    CssClass="formfieldtitle" Enabled="true"></fmcontrols:fmelipsebutton>
										</EditItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
							</FMCONTROLS:FMBaseDataGrid><fmcontrols:fmbutton id="NewLineItemButton" style="Z-INDEX: 107; LEFT: 0px; POSITION: relative; TOP: 0px"
								runat="server" Text="Add" onclick="NewLineItemButton_Click"></fmcontrols:fmbutton></td>
						<TD></TD>
					</tr>
					<tr id="TransportLineItemPageRow">
					    <td colspan="2">
					    <FMCONTROLS:FMBaseDataGrid onkeypress="javascript:DataGridKeyPress('TransportLineItem');" id="TransportDataGrid" style="LEFT: 1px; TOP: 0px"
								tabIndex="1" runat="server" BorderStyle="None" BackColor="White" PageSize="8" CssClass="tabletext"
								CellPadding="3" BorderColor="#999999" AllowSorting="True" BorderWidth="1px" GridLines="Vertical"
								AutoGenerateColumns="False">
								<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
								<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
								<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Edit">
										<HeaderStyle Width="55px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<fmcontrols:fmeditlinkbutton ID="EditButton3" runat="server" />
										</ItemTemplate>
										<EditItemTemplate>
											<fmcontrols:fmupdatelinkbutton ID="UpdateButton3" runat="server" />&nbsp;
											<fmcontrols:fmcancellinkbutton ID="CancelButton3" runat="server" />
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Delete">
										<HeaderStyle Width="0.5in"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<fmcontrols:fmdeletelinkbutton runat="server" ID="DeleteButton3" />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
							</FMCONTROLS:FMBaseDataGrid>
							<fmcontrols:fmbutton id="NewTransportButton" style="Z-INDEX: 108; LEFT: 0px; POSITION: relative; TOP: 0px"
								runat="server" Text="Add" onclick="TransportInfoNewButton_Click"></fmcontrols:fmbutton>
					    </td>
					    <td></td>
					</tr>
					<tr>
						<td colspan="2">
							<fmcontrols:FMLabel id="Label10" 
								style="Z-INDEX: 105; POSITION: relative; TOP: -1px; left: 1px;" runat="server"
											Width="176px" CssClass="formfieldtitle" Height="8px" ForeColor="Crimson">* Denotes Required Field</fmcontrols:FMLabel>
						</td>
						<TD valign="top" align="left">
							&nbsp;</TD>
					</tr>
					<tr>
						<td colspan="2">
							<table>
								<tr>
									<td><fmcontrols:fmbutton id="SaveButton" width="68px" style="Z-INDEX: 101; LEFT: 0px; POSITION: relative" runat="server"
											Text="&Apply" onclick="SaveButton_Click" cssstyle="formfieldtitle"/></td>
									<td><fmcontrols:fmbutton id="NewButton" width="68px" style="Z-INDEX: 102; LEFT: 0px; POSITION: relative" runat="server"
											Text="&New" onclick="NewButton_Click" cssstyle="formfieldtitle"/></td>
									<td><fmcontrols:FMConfirmationButton id="ReverseButton" style="Z-INDEX: 105; LEFT: 0px; POSITION: relative" runat="server"
											Text="Reverse" ConfirmationText="Reverse this transaction?" onclick="ReverseButton_Click" cssstyle="formfieldtitle"/></td>
									<td><fmcontrols:fmdeletebutton id="DeleteButton" style="Z-INDEX: 104; LEFT: 0px; POSITION: relative" runat="server"
											Text="Delete" onclick="DeleteButton_Click" visible="false" cssstyle="formfieldtitle" /></td>
									<td><fmcontrols:FMConfirmationButton id="ReverseUpdateButton" style="Z-INDEX: 108; POSITION: relative" runat="server"
											Text="Reverse / Update" ConfirmationText="Reverse this transaction and create an update?" onclick="ReverseUpdateButton_Click" cssstyle="formfieldtitle"/></td>
									<td><fmcontrols:fmbutton id="CloseButton" style="Z-INDEX: 106; LEFT: 0px; POSITION: relative" runat="server"
											Text="Close" onclick="CloseButtonClick" cssstyle="formfieldtitle"/></td>
									<td><fmcontrols:fmbutton id="ViewPrintableBtn" runat="server" Text="View Printable" onclick="ViewPrintableBtn_Click"></fmcontrols:fmbutton></td>
									<td><fmcontrols:fmbutton id="CombineBtn" runat="server" Text="Combine" onclick="CombineBtnClick" cssstyle="formfieldtitle" /></td>
							<td>&nbsp;</td>
						<td>&nbsp;</td>	
								</tr>
							</table>
						</td>
						<td valign="top" align="left">
							<table>
							</table>
						</td>
					</tr>
					<tr align="center">
						<td colspan="2"><fmcontrols:fmlabel id="TransIDLabelLabel" style="Z-INDEX: 103; POSITION: relative; TOP: 0px" runat="server"
								CssClass="tabletext">Transaction ID:</fmcontrols:fmlabel><asp:label id="TransIDLabel" runat="server" CssClass="tabletext"></asp:label></td>
					</tr>
				</TABLE>
				<input id="LimitSelectionsBasedOnHierarchy" style="Z-INDEX: 1; LEFT: 505px; POSITION: absolute; TOP: 486px"
					runat="server" width="0px" type="hidden" forecolor="White" />
				<input id="FieldsAndRights" style="Z-INDEX: 1; LEFT: 505px; POSITION: absolute; TOP: 486px"
					runat="server" width="0px" type="hidden" forecolor="White" />
				<div id="waitDiv" style="z-index: 500; left: 375px; top: 250px; position:absolute; display: none;">
				   <img src="../FMWebApp/images/pleaseWait.jpg" />
				</div>
				</ContentTemplate>
			</asp:UpdatePanel>
		</form>
		<script language="jscript">
			function document.onkeydown()
			{
				if (event.keyCode == 13
				&& event.srcElement != null
				&& (event.srcElement.type == "submit"
				|| event.srcElement.type == "button")
				&& (event.srcElement.id == "SaveButton"
				|| event.srcElement.id == "NewButton"
				|| event.srcElement.id == "DeleteButton"
				|| event.srcElement.id == "ReverseButton"
				|| event.srcElement.id == "ReverseUpdateButton"
				|| event.srcElement.id == "CloseButton"
				|| event.srcElement.id == "ViewPrintableBtn"
				|| event.srcElement.id == "CombineBtn"
				|| event.srcElement.id == "PreviousButton"
				|| event.srcElement.id == "NextButton"
				|| event.srcElement.id == "NewTransportButton"
				|| event.srcElement.id == "NewAGRButton"
				|| event.srcElement.id == "NewLineItemButton"))
				{
					event.srcElement.onclick();
					event.returnValue = false;
					event.cancel = true;
				}
			}
		</script>
	</body>
</html>
