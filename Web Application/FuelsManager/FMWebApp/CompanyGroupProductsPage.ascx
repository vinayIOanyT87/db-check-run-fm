<%@ Control Language="c#" AutoEventWireup="True" Codebehind="CompanyGroupProductsPage.ascx.cs" Inherits="FMWebApp.CompanyGroupProductsPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
<script>
		function InstructionsButton_Click ( itemIndex )
		{
			showModalDialogFrame({
                url: "../FMWebApp/SpecialInstructionsForm.aspx?mode=companygroup&ItemIndex=" + itemIndex,
                width: 780,
                height: 580,
                title: "Special Instructions",
                onClose: function ()
                {
                    if (this.returnValue != null && this.returnValue)
                    {
                        __doPostBack('InstructionsButton', '');
                    }
                }
            });
		}
		
</script>
	<body role="application">
<FMCONTROLS:FMLABEL id="Label5" BackColor="Transparent" CssClass="formfieldtitle" Width="120px" runat="server">Authorized Products:</FMCONTROLS:FMLABEL>
        <table id="Table2" style="width: 238px; height: 10px" cellspacing="0" cellpadding="1" width="238"
            border="0">
            <tr>
                <td width="507" height="10">
                    <FMControls:FMDataGrid ID="AuthorizedProductsDataGrid" BackColor="White" CssClass="tabletext" Width="696px" RowHeaderColumn="ID"
                        runat="server" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
                        GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None">
                        <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                        <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                        <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                        <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                        <Columns>
                            <asp:TemplateColumn HeaderText="Edit">
                                <HeaderStyle Width="55px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <FMControls:FMEditLinkButton ID="FMEditLinkButton1" runat="server"></FMControls:FMEditLinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <FMControls:FMUpdateLinkButton ID="FMUpdateLinkButton1" runat="server"></FMControls:FMUpdateLinkButton>&nbsp; 
                                    <FMControls:FMCancelLinkButton ID="FMCancelLinkButton1" runat="server"></FMControls:FMCancelLinkButton>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn Visible="False" HeaderText="Index">
                                <ItemTemplate>
                                    <asp:Label ID="IndexLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Product ID">
                                <ItemTemplate>
                                    <asp:Label Width="1.5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" ID="Label2">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList Width="1.5in" CssClass="tabletext" runat="server" Enabled="True" ID="ProductsDropDownList" DataSource="<%# EnumerateProducts()%>" DataTextField="Text" DataValueField="Value">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Additive Profile">
                                <ItemTemplate>
                                    <asp:Label Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.AdditiveProfileID") %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.AdditiveProfileID") %>' Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" ID="Label3">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList CssClass="tabletext" runat="server" Enabled="True" ID="AdditiveProfilesDropDownList" DataSource="<%# EnumerateAdditiveProfiles()%>" DataTextField="Text" DataValueField="Value">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="ID">
                                <ItemTemplate>
                                    <asp:Label Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToProductID") %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.ShipToProductID") %>' Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" ID="Label6">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Width="1in" MaxLength="30" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToProductID") %>' Enabled="True" ID="ShipToProductIDTextBox">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Code">
                                <ItemTemplate>
                                    <asp:Label Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToProductCode") %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.ShipToProductCode") %>' Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" ID="Label7">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Width="1in" MaxLength="10" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToProductCode") %>' Enabled="True" ID="ShipToProductCodeTextBox">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Station Text">
                                <ItemTemplate>
                                    <asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToLoadRackDisplayText") %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.ShipToLoadRackDisplayText") %>' Style="overflow: hidden; white-space: nowrap; text-overflow: ellipsis" ID="Label8">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Width=".5in" MaxLength="10" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToLoadRackDisplayText") %>' Enabled="True" ID="ShipToLoadRackDisplayTextTextBox">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Instructions">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <input class="tabletext" id="InstructionsButton" onclick='<%# DataBinder.Eval(Container, "DataItem.SpecialInstructionsClick") %>' type="button" value='<%# DataBinder.Eval(Container, "DataItem.SpecialInstructionsText") %>' runat="server" name="InstructionsButton">
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Delete">
                                <HeaderStyle Width="0.5in"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <FMControls:FMDeleteLinkButton ID="FMDeleteLinkButton1" runat="server"></FMControls:FMDeleteLinkButton>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                        <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                    </FMControls:FMDataGrid></td>
            </tr>
            <tr>
                <td width="507" height="10">
                    <FMControls:FMButton ID="AddProductButton" TabIndex="1" CssClass="formfieldtitle"  style="min-width: 100px" runat="server" Text="Add"></FMControls:FMButton></td>
            </tr>
        </table>
	</body>
</HTML>
