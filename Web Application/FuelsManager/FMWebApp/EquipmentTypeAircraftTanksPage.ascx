<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="true" CodeBehind="EquipmentTypeAircraftTanksPage.ascx.cs" Inherits="FuelsManager.FMWebApp.EquipmentTypeAircraftTanksPage" %>
<HTML>
	<HEAD>  
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
        <table id="Table1" style="z-index: 101; left: 0px; width: 407px; position: absolute; top: 40px; height: 10px" cellspacing="0" cellpadding="1" width="407" border="0">
            <tbody>
                <tr>
                    <td>
                        <FMControls:FMDataGrid ID="TanksDataGrid" runat="server" CssClass="tabletext"
                            RowHeaderColumn="Alias"
                            BackColor="White" PageSize="5"
                            AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True"
                            BorderWidth="1px" GridLines="Vertical"
                            AutoGenerateColumns="False" BorderStyle="None" Width="392px"
                            OnCancelCommand="TanksDataGrid_CancelCommand"
                            OnDeleteCommand="TanksDataGrid_DeleteCommand"
                            OnEditCommand="TanksDataGrid_EdiCommand"
                            OnItemDataBound="TanksDataGrid_ItemDataBound"
                            OnPageIndexChanged="TanksDataGrid_PageIndexChanged"
                            OnUpdateCommand="TanksDataGrid_UpdateCommand">
                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                            <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                            <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                            <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                            <Columns>
                                <asp:TemplateColumn HeaderText="Edit">
                                    <HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
                                    <ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMEditLinkButton runat="server" ID="EditButton" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <FMControls:FMUpdateLinkButton runat="server" ID="Fmupdatelinkbutton1" />&nbsp;
                        <FMControls:FMCancelLinkButton runat="server" ID="Fmcancellinkbutton1" />
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn Visible="False" HeaderText="Index">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn Visible="True" HeaderText="Alias">
                                    <HeaderTemplate>
                                        Alias <span style="color: Red">*</span>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label Width="1.00in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Alias") %>' ID="AliasLabel">
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox Width="1.00in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Alias") %>' ID="AliasTextBox" CssClass="tabletext" aria-required="true" />
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Customer Tank ID">
                                    <ItemTemplate>
                                        <asp:Label Width="0.75in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CustomerTankID") %>' ID="CustomerTankIDLabel" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox Width="0.75in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CustomerTankID") %>' ID="CustomerTankIDTextBox" CssClass="tabletext" />
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Description">
                                    <ItemTemplate>
                                        <asp:Label Width="1.5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' ID="DescriptionLabel">
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="DescriptionTextBox" Width="1.5in" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>'>
                                        </asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Capacity">
                                    <ItemTemplate>
                                        <asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Capacity") %>' ID="CapacityLabel">
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="CapacityTextBox" CssClass="tabletext" Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Capacity") %>'>
                                        </asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Position">
                                    <ItemTemplate>
                                        <asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Position") %>' ID="PositionLabel">
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="PositionTextBox" CssClass="tabletext" Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Position") %>'>
                                        </asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Location">
                                    <ItemTemplate>
                                        <asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Location") %>' ID="LocationLabel">
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="LocationDropDownList" DataValueField="LocationField" DataSource='<%# PopulateLocationDropDownList() %>' DataTextField="LocationField" runat="server"></asp:DropDownList>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Order">
                                    <ItemTemplate>
                                        <asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.GuiOrder") %>' ID="GuiOrderLabel">
                                        </asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="GuiOrderTextBox" CssClass="tabletext" Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.GuiOrder") %>'>
                                        </asp:TextBox>
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Delete">
                                    <HeaderStyle Width="0.5in"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server"
                                            Text="&lt;img src=Images/Delete.gif border=0 align=absmiddle alt='Delete this item'&gt;" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                            <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                        </FMControls:FMDataGrid></td>
                </tr>
                <tr>
                    <td height="21">
                        <FMControls:FMButton ID="AddButton" runat="server"
                            CssClass="formfieldtitle" Text="Add" OnClick="AddButton_Click"></FMControls:FMButton></td>
                </tr>
            </tbody>
        </table>
    </body>
</HTML>
