<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<%@ Page Language="c#" CodeBehind="FCRC_SummaryForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FuelCardWebApp.FCRC_SummaryForm" %>

<!DOCTYPE html>
<html>
<head>
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>">
    <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/lib/jquery-1.7.1.js" %>"></script>
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>" ></script>
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>

	<script type="text/javascript">
		function CompanySelect(role, companyTextBoxId)
		{
		    var companyTextBox = document.getElementById(companyTextBoxId);

		    showModalDialogFrame({
		        url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&All=true",
		        width: 855,
		        height: 560,
		        onClose: function ()
		        {
		            if (this.returnValue != null)
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
</head>
<body tabindex="-1" ms_positioning="GridLayout">
    <form id="Form1" method="post" enctype="multipart/form-data" runat="server" defaultbutton="FindBtn">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <FMControls:FMLabel ID="FuelCardTitle" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                CssClass="headline" Width="368px" BackColor="Transparent">Fuel Card Configuration</FMControls:FMLabel>

            <table id="TableHeader" style="z-index: 102; left: 32px; width: 700px; position: absolute; top: 35px; height: 10px"
                cellspacing="0" cellpadding="1" border="0">
                <tr>
                    <td>
                        <FMControls:FMLabel ID="ManagerLabel" runat="server"
                            CssClass="formfieldtitle" BackColor="Transparent">Manager:</FMControls:FMLabel>
                    </td>
                    <td>
                        <FMControls:FMCompanyTextBox runat="server" ID="ManagerSelect" Role="MANAGER"
                            TabIndex="1" CssClass="formfield" Width="136px" />
                    </td>
                    <td>
                        <FMControls:FMLabel ID="FindStringLabel" AssociatedControlID="FindTextBox" runat="server" CssClass="formfieldtitle"
                            BackColor="Transparent">Find String:</FMControls:FMLabel>
                    </td>
                    <td>
                        <asp:TextBox ID="FindTextBox" TabIndex="2" runat="server" Width="150px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td>
                        <FMControls:FMButton ID="FindBtn" TabIndex="3" runat="server" Text="Find" CssClass="formfieldtitle" Width="64px"
                            OnClick="FindBtnOnClick"></FMControls:FMButton>
                        &nbsp;&nbsp;
						<FMControls:FMButton ID="ShowAllButton" TabIndex="4" runat="server" Text="Show All" CssClass="formfieldtitle" Width="64px"
                            OnClick="ShowAllBtnOnClick"></FMControls:FMButton>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="OwnerLabel" runat="server"
                            CssClass="formfieldtitle" BackColor="Transparent">Owner:</FMControls:FMLabel>
                    </td>
                    <td>
                        <FMControls:FMCompanyTextBox runat="server" ID="OwnerSelect" Role="OWNER"
                            TabIndex="1" CssClass="formfield" Width="136px" />
                    </td>
                    <td>
                        <FMControls:FMLabel runat="server" ID="FuelCardTypeLabel" AssociatedControlID="FuelCardTypeDropDownList" CssClass="formfieldtitle" Text="Type:"></FMControls:FMLabel></td>
                    <td>
                        <FMControls:FMDropDownList ID="FuelCardTypeDropDownList" runat="server" CssClass="formfield" DataSource="<%#EnumerateFuelCardTypes()%>" DataTextField="ID" DataValueField="IdentityGuid" AutoPostBack="False" Width="150px"></FMControls:FMDropDownList></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="ShipperLabel" runat="server"
                            CssClass="formfieldtitle" BackColor="Transparent">Shipper:</FMControls:FMLabel>
                    </td>
                    <td>
                        <FMControls:FMCompanyTextBox runat="server" ID="ShipperSelect" Role="SHIPPER"
                            TabIndex="1" CssClass="formfield" Width="136px" />
                    </td>
                    <td>
                        <FMControls:FMCheckBox ID="TransientCheckBox" Text="Transient Card" CssClass="formfieldtitle" BackColor="Transparent" runat="server" />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="BillToLabel" runat="server"
                            CssClass="formfieldtitle" BackColor="Transparent">Bill To:</FMControls:FMLabel>
                    </td>
                    <td>
                        <FMControls:FMCompanyTextBox runat="server" ID="BillToSelect" Role="CUSTOMER_BILLTO"
                            TabIndex="1" CssClass="formfield" Width="136px" />
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="ShipToLabel" runat="server"
                            CssClass="formfieldtitle" BackColor="Transparent">Ship To:</FMControls:FMLabel>
                    </td>
                    <td>
                        <FMControls:FMCompanyTextBox runat="server" ID="ShipToSelect" Role="CUSTOMER_SHIPTO"
                            TabIndex="1" CssClass="formfield" Width="136px" />
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
            </table>
            <table id="Table1" style="z-index: 102; left: 32px; width: 50%; position: absolute; top: 190px; height: 10px"
                cellspacing="0" cellpadding="1" border="0">
                <tr>
                    <td valign="middle" width="350" height="36">
                        <FMControls:FMButton ID="AddButton2" TabIndex="6" runat="server" Width="100px" Text="Add"
                            CssClass="formfieldtitle"></FMControls:FMButton>
                        &nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="FuelCardSummaryPageSizeDropDown" ToolTip="Page size" runat="server"
                            OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
                        <FMControls:FMLabel Width="500px" ID="lblWarning" runat="server" CssClass="formfield" Text="abc"
                            Visible="false" ForeColor="Red" />
                    </td>
                    <td align="right">
                        <FMControls:FMButton ID="RefreshButton" TabIndex="8" style="min-width: 100px" runat="server" Text="Refresh" CssClass="formfieldtitle" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 407px; height: 10px" width="407" colspan="2">
                        <FMControls:FMDataGridFixedPaging
                            ID="fuelCardsDataGrid"
                            RowHeaderColumn="Fuel Card ID"
                            Style="left: 1px; top: 0px" runat="server"
                            AutoGenerateColumns="False"
                            DataKeyNames="SiteGuid, IdentityGuid"
                            BorderStyle="Solid"
                            BackColor="White"
                            GridLines="Vertical"
                            Width="880px"
                            BorderWidth="1px"
                            AllowSorting="True"
                            CellPadding="3"
                            AllowPaging="True"
                            CssClass="tabletext"
                            EmptyDataText="No records found"
                            PageSize="12"
                            BorderColor="White"
                            TabIndex="7"
                            ShowHeaderWhenEmpty="True"
                            ShowFooterWhenEmpty="False"
                            FixedHeaders="True"
                            GroupColumnOffset="0"
                            GroupingDepth="0" FixedHeight="475px" Height="475px">
                            <SelectedItemStyle BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SelectedItemStyle>
                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                            <ItemStyle BackColor="#EEEEEE" ForeColor="Black"></ItemStyle>
                            <Columns>
                                <asp:TemplateColumn HeaderText="Edit">
                                    <HeaderStyle Width="55px"></HeaderStyle>
                                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMEditLinkButton ID="Fmeditlinkbutton1" runat="server"></FMControls:FMEditLinkButton>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn HeaderText="SiteGuid" DataField="SiteGuid" Visible="False"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="IdentityGuid" DataField="IdentityGuid" Visible="False"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Fuel Card ID" DataField="ID"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Manager">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="ManagerLabel" ToolTip='<%# DataBinder.Eval(Container, "DataItem.ManagerTip") %>' Text='<%# DataBinder.Eval(Container, "DataItem.Manager") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Owner">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="OwnerLabel" ToolTip='<%# DataBinder.Eval(Container, "DataItem.OwnerTip") %>' Text='<%# DataBinder.Eval(Container, "DataItem.Owner") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Shipper">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="ShipperLabel" ToolTip='<%# DataBinder.Eval(Container, "DataItem.ShipperTip") %>' Text='<%# DataBinder.Eval(Container, "DataItem.Shipper") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Bill To">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="BillToLabel" ToolTip='<%# DataBinder.Eval(Container, "DataItem.BillToTip") %>' Text='<%# DataBinder.Eval(Container, "DataItem.BillTo") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Ship To">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="ShipToLabel" ToolTip='<%# DataBinder.Eval(Container, "DataItem.ShipToTip") %>' Text='<%# DataBinder.Eval(Container, "DataItem.ShipTo") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn HeaderText="Provider" DataField="Provider"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Activation Status" DataField="Status"></asp:BoundColumn>
                                <asp:BoundColumn HeaderText="Type" DataField="FuelCardTypeApplicationStringID"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Delete">
                                    <HeaderStyle Width="0.5in"></HeaderStyle>
                                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMDeleteLinkButton ID="Fmdeletelinkbutton1" runat="server"></FMControls:FMDeleteLinkButton>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                            <PagerStyle CssClass="GVFixedFooter" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages" />
                        </FMControls:FMDataGridFixedPaging>
                    </td>
                </tr>
                <tr>
                    <td style="width: 163px; height: 36px" valign="middle" width="163">
                        <FMControls:FMButton ID="AddButton" TabIndex="6" runat="server" Text="Add" CssClass="formfieldtitle"
                            Width="98px"></FMControls:FMButton></td>
                </tr>
            </table>
            <script language="jscript">
                var findBtn = document.getElementById("FindBtn");
                var findTbBtn = document.getElementById("FindTextBox");

                if (findBtn != null && findTbBtn != null) {
                    try {
                        findBtn.setActive();
                        findTbBtn.focus();
                    }
                    catch (err) { }
                }
            </script>
        </div>
    </form>
</body>
</html>
