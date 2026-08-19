<%@ Page Language="C#"  AutoEventWireup="true" CodeBehind="InvoicePaymentSummary.aspx.cs" Inherits="ADFWebApp.InvoicePaymentSummary" %>

<%@ Register assembly="FMControls" namespace="FMControls" tagprefix="FMCONTROLS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
	<link href="../FuelsManager.css" rel="stylesheet"></link>
	<script type="text/javascript" src="ADFCustomScripts.js"></script>
	<script type="text/javascript">
	function CompanySelect(role, CompanyTextBoxID)
	{
		var sFeatures		 = "dialogWidth: 855px; dialogHeight: 560px";
		var CompanyTextBox = document.getElementById(CompanyTextBoxID);
		var CompanyNameTextBox = document.getElementById("CompanyName" + CompanyTextBoxID);
						
	    var result = window.showModalDialog("../FMWebApp/CompanySelectForm.aspx?Role=SUPPLIER", 
		        "", sFeatures);

	    if (result != null) {
	        CompanyTextBox.value = result[0];
	        CompanyTextBox.title = result[1];
	        CompanyNameTextBox.value = result[2];
	    }
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
		}		
	}
	</script>
</head>
<body ms_positioning="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:image id="FadeImage" 
                style="Z-INDEX: -100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
			<FMCONTROLS:FMLABEL id="lblHeading" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" BackColor="Transparent" CssClass="headline" Width="272px">
                Placeholder</FMCONTROLS:FMLABEL>
			<br />
			<br />
			<table id="tblGrid" runat="server" style="z-index:100" cellpadding="5" 
                cellspacing="3">
				<tr>
					<td><FMCONTROLS:FMLabel ID="lblInvoiceNumber" runat="server" 
                            CssClass="formfieldtitle">FM Invoice Number</FMCONTROLS:FMLabel></td>
					<td>
                        <asp:TextBox ID="tbInvoiceNumber" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
					<td><FMCONTROLS:FMLabel ID="lblFuelType" runat="server" CssClass="formfieldtitle">
                        Fuel Type</FMCONTROLS:FMLabel></td>
					<td>
                        <FMCONTROLS:FMProductTextBox ID="tbProduct" runat="server"></FMCONTROLS:FMProductTextBox>
                    </td>
					<td>&nbsp;</td>
				</tr>			        
				<tr>
					<td><FMCONTROLS:FMLabel ID="lblAccountCode" runat="server" 
                            CssClass="formfieldtitle">Account Code</FMCONTROLS:FMLabel></td>
					<td>
                        <FMCONTROLS:FMDropDownList ID="ddlAccountCode" runat="server"></FMCONTROLS:FMDropDownList>
                    </td>
					<td><FMCONTROLS:FMLabel ID="lblEnteredBy" runat="server" CssClass="formfieldtitle">
                        Entered By</FMCONTROLS:FMLabel></td>
					<td>
                        <FMCONTROLS:FMDropDownList ID="ddlEnteredBy" runat="server">
                        </FMCONTROLS:FMDropDownList>
                    </td>
					<td valign="top">&nbsp;</td>
				</tr>				
				<tr>
					<td><FMCONTROLS:FMLabel ID="lblStartDate" runat="server" CssClass="formfieldtitle">
                        Start Date</FMCONTROLS:FMLabel></td>
					<td>
                        <FMCONTROLS:FMDate ID="startDateCtrl" runat="server" />
                    </td>
					<td><FMCONTROLS:FMLabel ID="lblEndDate" runat="server" CssClass="formfieldtitle">End 
                        Date</FMCONTROLS:FMLabel></td>
					<td>
                        <FMCONTROLS:FMDate ID="endDateCtrl" runat="server" />
                    </td>
					<td>&nbsp;</td>
				</tr>				
				<tr>
					<td><FMCONTROLS:FMLabel ID="lblInvoiceQuery" runat="server" 
                            CssClass="formfieldtitle">Invoice Query</FMCONTROLS:FMLabel></td>
                            <td>
                                <FMCONTROLS:FMCustomTextBox ID="tbInvoiceQuery" runat="server"></FMCONTROLS:FMCustomTextBox>
                    </td>
					<td><FMCONTROLS:FMLabel ID="lblPaymentID" runat="server" CssClass="formfieldtitle">
                        Payment ID</FMCONTROLS:FMLabel></td>
					<td>
                        <asp:TextBox ID="tbPaymentID" runat="server" MaxLength="50" Visible="false"></asp:TextBox>
                        <FMCONTROLS:FMDropDownList ID="ddlCostCentreCode" runat="server" Visible="false">
                        </FMCONTROLS:FMDropDownList>
                    </td>
					<td>&nbsp;</td>
				</tr>				
				<tr>
					<td><FMCONTROLS:FMLabel ID="lblSupplier" runat="server" CssClass="formfieldtitle">
                        Supplier</FMCONTROLS:FMLabel></td>
					<td>
                        <FMCONTROLS:FMCompanyTextBox ID="tbSupplier" runat="server" Role="SUPPLIER"></FMCONTROLS:FMCompanyTextBox>
                    </td>
					<td><FMCONTROLS:FMLabel ID="lblSection" runat="server" CssClass="formfieldtitle">
                        Section</FMCONTROLS:FMLabel></td>
					<td>
					    <FMCONTROLS:FMDropDownList ID="ddlSection" runat="server">
                        </FMCONTROLS:FMDropDownList>
					</td>
				</tr>				
				<tr>
					<td>
                        <br />
                    </td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
				</tr>
				<tr>
				    <td colspan="5">
				        <table>				
                        <tr>
                            <td><FMControls:FMButton ID="btnAddTop" runat="server" CssClass="formfieldtitle" Text="Add" style="width: 65px" />
                            &nbsp;
                            <FMCONTROLS:FMPageSizeDropDown ID="ddlPageSize" runat="server" CssClass="formfield">
                                </FMCONTROLS:FMPageSizeDropDown></td>
                            <td align="right">
                            <FMCONTROLS:FMButton id="btnShowAll" runat="server" CssClass="formfieldtitle" 
                                    Text="Show All" Width="64px" onclick="btnShowAll_Click" />
                            <FMCONTROLS:FMButton id="btnRefresh" runat="server" CssClass="formfieldtitle" 
                                    Text="Refresh" Width="64px" onclick="btnRefresh_Click" /></td>
                        </tr>
				            <tr>
				                <td colspan="2">
				                <FMCONTROLS:FMDATAGRID id="resultGrid" tabIndex="5" 
                                runat="server" BackColor="White" CssClass="tabletext"
				                Width="736px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" 
                                BorderWidth="1px" AllowSorting="True"
				                BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="20">
			                        <FooterStyle ForeColor="Black" BackColor="#333399"></FooterStyle>
			                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
			                        <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
			                        <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
			                        <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#333399"></HeaderStyle>
			                        <Columns>
			                            <asp:TemplateColumn HeaderText="Edit">
			                                <ItemTemplate>
						                        <FMControls:FMEditLinkButton id="EditLinkButton" runat="server"></FMControls:FMEditLinkButton>
					                        </ItemTemplate>
			                            </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Payment ID">
                                            <ItemTemplate>
                                                <FMCONTROLS:fmlabel ID="lblGridPaymentID" runat="server" 
                                                    Text='<%# DataBinder.Eval(Container, "DataItem.PaymentID") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="FM Invoice Number">
                                            <ItemTemplate>
                                                <FMCONTROLS:fmlabel ID="lblGridInvoiceNumber" runat="server" 
                                                    Text='<%# DataBinder.Eval(Container, "DataItem.InvoiceNumber") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Order Number">
                                            <ItemTemplate>
                                                <FMCONTROLS:fmlabel ID="lblGridOrderNumber" runat="server" 
                                                    Text='<%# DataBinder.Eval(Container, "DataItem.OrderNumber") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Quantity">
                                            <ItemTemplate>
                                                <FMCONTROLS:fmlabel ID="lblGridQuantity" runat="server" 
                                                    Text='<%# DataBinder.Eval(Container, "DataItem.Quantity") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
			                            <asp:TemplateColumn HeaderText="Supplier">
                                            <ItemTemplate>
                                                <FMCONTROLS:fmlabel ID="lblGridSupplier" runat="server" 
                                                    Text='<%# DataBinder.Eval(Container, "DataItem.Supplier") %>' />
                                            </ItemTemplate>
			                            </asp:TemplateColumn>
			                            <asp:TemplateColumn HeaderText="Total Amount">
                                            <ItemTemplate>
                                                <FMCONTROLS:fmlabel ID="lblGridTotalAmount" runat="server" 
                                                    Text='<%# DataBinder.Eval(Container, "DataItem.TotalAmount") %>' />
                                            </ItemTemplate>
			                            </asp:TemplateColumn>
			                            <asp:TemplateColumn HeaderText="Section">
                                            <ItemTemplate>
                                                <FMCONTROLS:fmlabel ID="lblGridSection" runat="server" 
                                                    Text='<%# DataBinder.Eval(Container, "DataItem.Section") %>' />
                                            </ItemTemplate>
			                            </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Entered By">
                                            <ItemTemplate>
                                                <FMCONTROLS:fmlabel ID="lblGridEnteredBy" runat="server" 
                                                    Text='<%# DataBinder.Eval(Container, "DataItem.EnteredBy") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Invoice Query">
                                            <ItemTemplate>
                                                <FMCONTROLS:fmlabel ID="lblGridInvoiceQuery" runat="server" 
                                                    Text='<%# DataBinder.Eval(Container, "DataItem.InvoiceQuery") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Action Required">
                                            <ItemTemplate>
                                                <FMCONTROLS:fmlabel ID="lblGridActionRequired" runat="server" 
                                                    Text='<%# DataBinder.Eval(Container, "DataItem.ActionRequired") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
			                            <asp:TemplateColumn Visible="False"></asp:TemplateColumn>
			                        </Columns>
			                        <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#333399" Mode="NumericPages"></PagerStyle>
		                        </FMCONTROLS:FMDATAGRID>
				                </td>
				            </tr>
				            <tr>
		                        <td colspan="2">
		                            <FMControls:FMButton ID="btnAddBottom" runat="server" CssClass="formfieldtitle" 
                                        Text="Add" style="width: 65px" />
                                </td>
				            </tr>
				        </table>
		            </td>
		        </tr>
		        <tr>
		            <td colspan="5">
		                &nbsp;
		            </td>
		        </tr>
		    </table>
        </form>
				
	</body>
</html>
