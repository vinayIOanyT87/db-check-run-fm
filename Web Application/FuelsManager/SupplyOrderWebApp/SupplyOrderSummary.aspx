<%@ Page  language="c#" Codebehind="SupplyOrderSummary.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.SupplyOrderWebApp.SupplyOrderSummaryForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="SupplyOrderListForm" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<SCRIPT>
                var oRefreshButton = document.getElementById("RefreshButton");
                if (oRefreshButton != null)
				{
					oRefreshButton.setActive();
				}
		
				function CompanySelect(role, companyTextBoxId) {
				    var companyTextBox = document.getElementById(companyTextBoxId);
				    var inhibitStartupLoad = document.getElementById("InhibitAutoLoadTextBox");

				    showModalDialogFrame({
				        url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&All=true" + "&Inhibit=" + inhibitStartupLoad.value,
				        width: 870,
				        height: 700,
				        title: "Company Select",
				        onClose: function () {
				            if (this.returnValue != null) {
				                var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
				                var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

				                companyTextBox.value = asciiValue1;
				                companyTextBox.title = asciiValue2;
				            }
				        }
				    });
				}
            </SCRIPT>
            <asp:Image ID="FadeImage" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
            <!-- The InhibitAutoLoadTextBox is hidden with display:none because if you use visible = false the control won't be rendered to the client,
                and that will mess up the javascript in the CompanySelect function -->
            <asp:TextBox ID="InhibitAutoLoadTextBox" Style="z-index: 99; left: 190px; position: absolute; top: 5px; display: none"
                runat="server" Width="45px" MaxLength="25">False</asp:TextBox>
            <FMControls:FMLabel ID="PageTitle" Style="z-index: 101; left: 16px; position: absolute; top: 8px" runat="server"
                BackColor="Transparent" Width="500px" CssClass="headline" Text="Order Summary">Supply Order Summary</FMControls:FMLabel>
            <FMControls:FMLabel ID="FMLABEL1" Style="z-index: 124; left: 24px; position: absolute; top: 32px" runat="server"
                BackColor="Transparent" CssClass="formfieldtitle" Width="101" Height="16" Text="Order Nunber">Order Number</FMControls:FMLabel>
            <asp:TextBox ID="OrderNumberTextBox" Style="z-index: 124; left: 144px; position: absolute; top: 32px"
                TabIndex="0" runat="server" CssClass="formfield" Width="88px" MaxLength="14"></asp:TextBox>
            <FMControls:FMLabel ID="FMLABEL4" Style="z-index: 111; left: 368px; position: absolute; top: 32px" runat="server"
                BackColor="Transparent" Width="71" CssClass="formfieldtitle" Text="Manager" Height="15">Manager</FMControls:FMLabel>
            <FMControls:FMCompanyTextBox ID="ManagerTextBox" Style="z-index: 108; left: 456px; position: absolute; top: 32px"
                runat="server" Role="MANAGER" Width="145px" CssClass="formfield"></FMControls:FMCompanyTextBox>
            <FMControls:FMLabel ID="Fmlabel9" Style="z-index: 121; left: 24px; position: absolute; top: 56px" runat="server"
                BackColor="Transparent" Width="101px" CssClass="formfieldtitle" Text="Order Status" Height="16px"></FMControls:FMLabel>
            <asp:DropDownList ID="OrderStatusDropDownList" Style="z-index: 104; left: 144px; position: absolute; top: 56px"
                runat="server" Width="160px" CssClass="formfield">
            </asp:DropDownList>
            <FMControls:FMLabel ID="FMLABEL2" Style="z-index: 110; left: 368px; position: absolute; top: 64px" runat="server"
                BackColor="Transparent" Width="71px" CssClass="formfieldtitle" Text="Owner" Height="15px"></FMControls:FMLabel>
            <FMControls:FMCompanyTextBox ID="OwnerTextBox" Style="z-index: 105; left: 456px; position: absolute; top: 64px"
                runat="server" Role="OWNER" Width="145px" CssClass="formfield"></FMControls:FMCompanyTextBox>
            <FMControls:FMLabel ID="FMLABEL7" Style="z-index: 122; left: 24px; position: absolute; top: 80px" runat="server"
                BackColor="Transparent" Width="101px" CssClass="formfieldtitle" Text="Order Type" Height="16px"></FMControls:FMLabel>
            <asp:DropDownList ID="OrderTypeDropDown" Style="z-index: 103; left: 144px; position: absolute; top: 80px"
                runat="server" Width="160px" CssClass="formfield">
            </asp:DropDownList>
            <FMControls:FMLabel ID="FMLABEL6" Style="z-index: 113; left: 368px; position: absolute; top: 96px" runat="server"
                BackColor="Transparent" Width="71px" CssClass="formfieldtitle" Text="Shipper" Height="15px">Shipper</FMControls:FMLabel>
            <FMControls:FMCompanyTextBox ID="ShipperTextBox" Style="z-index: 106; left: 456px; position: absolute; top: 96px"
                runat="server" Role="SHIPPER" Width="145px" CssClass="formfield"></FMControls:FMCompanyTextBox>
            <FMControls:FMLabel ID="OrderStatusLabel" Style="z-index: 114; left: 24px; position: absolute; top: 104px"
                runat="server" BackColor="Transparent" Width="101px" CssClass="formfieldtitle" Text="Product" Height="16px"></FMControls:FMLabel>
            <asp:DropDownList ID="ProductDropDown" Style="z-index: 102; left: 144px; position: absolute; top: 104px"
                runat="server" Width="160px" CssClass="formfield">
            </asp:DropDownList>
            <FMControls:FMLabel ID="FMLABEL10" Style="z-index: 125; left: 24px; position: absolute; top: 128px"
                runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="100px" Height="16px" Text="Start Date">Date Filter Type</FMControls:FMLabel>
            <FMControls:FMDropDownList ID="DateFilterTypeDropDown" Style="z-index: 123; left: 144px; position: absolute; top: 128px"
                runat="server" CssClass="formfield" Sort="false" OnSelectedIndexChanged="DateFilterTypeDropDown_SelectedIndexChanged" AutoPostBack="True">
            </FMControls:FMDropDownList>
            <FMControls:FMLabel ID="Fmlabel8" Style="z-index: 112; left: 368px; position: absolute; top: 128px"
                runat="server" BackColor="Transparent" Width="71px" CssClass="formfieldtitle" Text="Supplier" Height="15px">Supplier</FMControls:FMLabel>
            <FMControls:FMCompanyTextBox ID="SupplierTextBox" Style="z-index: 107; left: 456px; position: absolute; top: 128px"
                runat="server" Role="SUPPLIER" Width="145px" CssClass="formfield"></FMControls:FMCompanyTextBox>
            <FMControls:FMLabel ID="StartDateLabel" Style="z-index: 116; left: 24px; position: absolute; top: 152px"
                runat="server" BackColor="Transparent" Width="101" CssClass="formfieldtitle" Text="Start Date" Height="16">
			Start Date</FMControls:FMLabel>
            <FMControls:FMDate ID="StartDate" Style="z-index: 175; left: 144px; position: absolute; top: 152px"
                runat="server" Width="160px" CssClass="formfield"></FMControls:FMDate>
            <FMControls:FMLabel ID="EndDateLabel" Style="z-index: 118; left: 24px; position: absolute; top: 176px"
                runat="server" BackColor="Transparent" Width="101px" CssClass="formfieldtitle" Text="End Date" Height="16px" />
            <FMControls:FMDate ID="EndDate" Style="z-index: 175; left: 144px; position: absolute; top: 176px" runat="server"
                Width="160px" CssClass="formfield"></FMControls:FMDate>
            <FMControls:FMButton ID="SELECTALL" Style="z-index: 135; left: 328px; position: absolute; top: 208px"
                TabIndex="14" runat="server" Text="Select All" Height="22px" CssClass="formfield" Width="100px" OnClick="OnSelectAll"></FMControls:FMButton>
            <FMControls:FMButton ID="DESELECTALL" Style="z-index: 134; left: 443px; position: absolute; top: 208px"
                TabIndex="14" runat="server" Text="Unselect All" Height="22px" CssClass="formfield" Width="100px" OnClick="UnSelectAll"></FMControls:FMButton>
            <FMControls:FMButton ID="PrintSelection" Style="z-index: 121; left: 558px; position: absolute; top: 208px"
                TabIndex="14" runat="server" CssClass="formfield" Text="Print Selected" Height="22" Width="100px" OnClick="PrintSelectionClick"></FMControls:FMButton>
            <FMControls:FMButton ID="RefreshButton" Style="z-index: 115; left: 673px; position: absolute; top: 208px"  Height="22" Width="100px" 
                runat="server" CssClass="formfield" Text="Refresh" OnClick="RefreshButtonClick"></FMControls:FMButton>
            <table style="z-index: 109; left: 24px; position: absolute; top: 210px">
                <tr>
                    <td height="36">
                        <FMControls:FMPageSizeDropDown ID="OrderSummarySizeDropDown" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged"></FMControls:FMPageSizeDropDown>&nbsp;&nbsp;
						<FMControls:FMLabel ID="WarningLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"></FMControls:FMLabel></td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMBaseDataGrid ID="TransactionDataGrid" Style="z-index: 109; left: 24px" runat="server" BackColor="White"
                            Width="736px" CssClass="tabletext" PageSize="8" CellPadding="3" BorderColor="White" AllowSorting="True"
                            BorderWidth="1px" GridLines="Vertical" BorderStyle="None" AllowPaging="True" AutoGenerateColumns="False">
                            <FooterStyle ForeColor="Black" BackColor="#CCCCCC" Font-Bold="False"
                                Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" HorizontalAlign="Center" VerticalAlign="Middle"
                                Width="736px" Wrap="False"></FooterStyle>
                            <SelectedItemStyle Font-Bold="True" ForeColor="White" CssClass="tablelink" BackColor="#008A8C"></SelectedItemStyle>
                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                            <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                            <HeaderStyle Font-Bold="True" Wrap="False" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                            <Columns>
                                <asp:TemplateColumn HeaderText="Edit">
                                    <HeaderStyle Width="55px"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMEditLinkButton runat="server" ID="btnEdit" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Multiple Select">
                                    <HeaderStyle Width="0.5in"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="MultipleSelectCheckbox" runat="server" Checked="false" Enabled="true"></asp:CheckBox>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                            <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                        </FMControls:FMBaseDataGrid></td>
                </tr>
            </table>
        </div>
        </form>
	</body>
</HTML>
