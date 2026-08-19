<%@ Page  language="c#" Codebehind="InvoiceSummary.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.InvoiceWebApp.InvoiceSummary" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
	<head>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript"/>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
	</head>
	<body MS_POSITIONING="GridLayout">
		<form id="InvoiceSummaryForm" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<script type="text/javascript">
				function CompanySelect(role, companyTextBoxId)
				{
					var companyTextBox = document.getElementById(companyTextBoxId);
					
					showModalDialogFrame({
					    url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&All=true" + "&Inhibit=False",
					    width: 855,
					    height: 690,
					    title: "Company Select",
					    onClose: function ()
					    {
					        if (this.returnValue != null && this.returnValue.length > 0)
					        {
					            var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
					            var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

					            companyTextBox.value = asciiValue1;
					            companyTextBox.title = asciiValue2;
					        }
					    }
					});
				}
			</script>
			<asp:image id="FadeImage" 
                style="Z-INDEX: 101; LEFT: -8px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image><FMCONTROLS:FMLABEL id="PageTitle" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 8px" runat="server"
				BackColor="Transparent" Text="Invoice Summary" Width="500px" CssClass="headline"></FMCONTROLS:FMLABEL>
			<table id="TableFilters" style="Z-INDEX: 103; LEFT: 16px; WIDTH: 744px; POSITION: absolute; TOP: 40px; HEIGHT: 176px"
				cellspacing="1" cellpadding="1" width="744" border="0">
				<tr>
					<td style="WIDTH: 121px; HEIGHT: 23px"><FMCONTROLS:FMLABEL id="InvoiceNumberLabel" runat="server" BackColor="Transparent" Text="Order Number"
							Width="101" CssClass="formfieldtitle" Height="16">Invoice Number</FMCONTROLS:FMLABEL></td>
					<td style="WIDTH: 163px; HEIGHT: 23px"><asp:textbox id="InvoiceNumberTB" runat="server" Width="152px" CssClass="formfield" ontextchanged="InvoiceNumberTextBoxTextChanged"></asp:textbox></td>
					<td style="HEIGHT: 23px"><FMCONTROLS:FMLABEL id="ProductLabel" runat="server" BackColor="Transparent" Text="Order Number" Width="88px"
							CssClass="formfieldtitle" Height="16">Product</FMCONTROLS:FMLABEL></td>
					<td style="HEIGHT: 23px"><asp:dropdownlist id="ProductDropdown" runat="server" Width="155px" CssClass="formfield"></asp:dropdownlist></td>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td style="WIDTH: 121px; HEIGHT: 23px"><FMCONTROLS:FMLABEL id="AccountCodeLabel" runat="server" BackColor="Transparent" Text="Order Number"
							Width="88px" CssClass="formfieldtitle" Height="16">Account Code</FMCONTROLS:FMLABEL></td>
					<td style="WIDTH: 163px; HEIGHT: 23px"><asp:dropdownlist id="AccountCodeDropdown" runat="server" Width="155px" CssClass="formfield"></asp:dropdownlist></td>
					<td style="HEIGHT: 23px"><FMCONTROLS:FMLABEL id="CostCenterLabel" runat="server" BackColor="Transparent" Text="Order Number"
							Width="112px" CssClass="formfieldtitle" Height="16">Cost Centre Code</FMCONTROLS:FMLABEL></td>
					<td style="HEIGHT: 20px"><asp:dropdownlist id="CostCenterDropdown" runat="server" Width="155px" CssClass="formfield"></asp:dropdownlist></td>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td><FMCONTROLS:FMLABEL id="StartDateLabel" runat="server" BackColor="Transparent" Text="Order Number" Width="88px"
							CssClass="formfieldtitle" Height="16">Start Date</FMCONTROLS:FMLABEL></td>
					<td><FMCONTROLS:FMDATE id="StartDate" runat="server" Width="160px" CssClass="formfield"></FMCONTROLS:FMDATE></td>
				    <td><FMCONTROLS:FMLABEL id="EndDateLabel" runat="server" BackColor="Transparent" Text="Order Number" Width="88px"
							CssClass="formfieldtitle" Height="16">End Date</FMCONTROLS:FMLABEL></td>
				    <td><FMCONTROLS:FMDATE id="EndDate" runat="server" Width="160px" CssClass="formfield"></FMCONTROLS:FMDATE></td>
					<td>&nbsp;</td>
				</tr>
				<tr>
				    <td><FMCONTROLS:FMLABEL id="SupplierLabel" runat="server" BackColor="Transparent" Text="Order Number" Width="88px"
							CssClass="formfieldtitle" Height="16">Supplier</FMCONTROLS:FMLABEL></td>
				    <td><FMCONTROLS:FMCOMPANYTEXTBOX id="SupplierTextBox" 
                            runat="server" Width="200px" CssClass="formfield" Role="SUPPLIER"></FMCONTROLS:FMCOMPANYTEXTBOX></td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td><FMCONTROLS:FMLABEL id="ShipToLabel" runat="server" BackColor="Transparent" Text="Order Number" Width="88px"
							CssClass="formfieldtitle" Height="16">Ship-To</FMCONTROLS:FMLABEL></td>
					<td><FMCONTROLS:FMCOMPANYTEXTBOX id="ShipToTextBox" runat="server" Width="200px" CssClass="formfield" Role="CUSTOMER_SHIPTO"></FMCONTROLS:FMCOMPANYTEXTBOX></td>
					<td>&nbsp;</td>
					<td><FMCONTROLS:FMBUTTON id="RefreshButton" runat="server" Text="Refresh" CssClass="formfield" onclick="OnClickRefreshBtn"></FMCONTROLS:FMBUTTON></td>
					<td>&nbsp;</td>
				</tr>
                <tr>
                    <td colspan="4">
                        <table id="TableGrid" style="z-index: 104; width: 745px; height: 120px" cellspacing="1" cellpadding="1" border="0">
                            <tr>
                                <td colspan="3" style="height: 65px">
                                    <FMControls:FMDataGridFixed ID="InvoiceDataGrid" runat="server" BackColor="White"
                                        Width="735px" CssClass="tabletext" AutoGenerateColumns="False" AllowPaging="True"
                                        BorderStyle="None" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
                                        BorderColor="White" cellpadding="3" PageSize="8">
                                        <SelectedItemStyle Font-Bold="True" ForeColor="White" CssClass="tablelink" BackColor="#008A8C">
                                        </SelectedItemStyle>
                                        <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                                        <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                                        <Columns>
                                            <asp:TemplateColumn HeaderText="Edit">
                                                <HeaderStyle Width="55px"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                                <ItemTemplate>
                                                    <FMControls:FMEditLinkButton runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                        </Columns>
                                    </FMControls:FMDataGridFixed>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Label ID="WarningLabel" runat="server" Width="728px" CssClass="formfieldtitle"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
		    </table>
		</div>
</form>
	</body>
</html>
