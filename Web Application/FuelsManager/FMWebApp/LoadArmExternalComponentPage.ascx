<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="LoadArmExternalComponentPage.ascx.cs" Inherits="FuelsManager.FMWebApp.LoadArmExternalComponentPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 0px; WIDTH: 39.37%; POSITION: absolute; TOP: 16px; HEIGHT: 10px"
	cellSpacing="0" cellPadding="1" border="0" role="presentation" aria-label="layout">
	<tr>
		<TD width="710" height="10"><FMControls:FMDataGrid id="DataGrid" runat="server" BackColor="White" Width="520px" CssClass="tabletext" RowHeaderColumn="Component"
				Height="10px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
				CellPadding="3" PageSize="8" AllowPaging="True" aria-label="External Components">
				<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
				<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
				<EditItemStyle Wrap="False"></EditItemStyle>
				<AlternatingItemStyle Wrap="False" BackColor="Gainsboro"></AlternatingItemStyle>
				<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn HeaderText="Edit">
						<HeaderStyle Width="55px"></HeaderStyle>
						<ItemStyle  HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						<ItemTemplate>
							<FMControls:FMEditLinkButton runat="server" ID="FMEditLineButton" NAME="FMEditLinkButton" />
						</ItemTemplate>
						<EditItemTemplate>
								<FMControls:FMUpdateLinkButton runat="server" ID="FMUpdateLinkButton" NAME="FMUpdateLinkButton" />&nbsp;
								<FMControls:FMCancelLinkButton runat="server" ID="FMCancelLinkButton" NAME="FMCancelLinkButton" />
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn Visible="False" HeaderText="Index">
                        <ItemTemplate>
                            <asp:Label ID="IndexLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Component">
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' ID="Label2">
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tabletext" runat="server" Enabled="True" ID="ProductsDropDownList" ToolTip="Product" DataSource="<%# EnumerateProducts()%>" DataTextField="Text" DataValueField="Value" AutoPostBack="True" OnSelectedIndexChanged="ProductDropDownList_SelectedIndexChanged">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Type">
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Type") %>' ID="Label5">
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <FMControls:FMDropDownList CssClass="tabletext" runat="server" Enabled="True" ID="TypeDropDownList" ToolTip="Type" DataSource="<%# EnumerateLocationTypes()%>" DataTextField="Text" DataValueField="Value" AutoPostBack="True" OnSelectedIndexChanged="TypeDropDownList_SelectedIndexChanged">
                            </FMControls:FMDropDownList>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Location">
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.LocationID") %>' ID="Label3">
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tabletext" runat="server" Enabled="True" ID="LocationDropDownList" ToolTip="Location" DataSource="<%# EnumerateLocations()%>" DataTextField="Text" DataValueField="Value">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Input">
                        <HeaderStyle Width="0.5in"></HeaderStyle>
                        <ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        <ItemTemplate>
                            <input class="formfieldtitle" id="InputButton" onclick='<%# DataBinder.Eval(Container, "DataItem.InputsClick") %>' type="button" value="..." runat="server" name="InputButton" style="width: 20px; height: 20px; padding-left: 0; padding-right: 0">
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Permissives">
                        <HeaderStyle Width="0.5in"></HeaderStyle>
                        <ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        <ItemTemplate>
                            <input class="formfieldtitle" id="PermissivesButton" onclick='<%# this.Server.HtmlDecode(Convert.ToString(DataBinder.Eval(Container, "DataItem.PermissivesClick"))) %>' type="button" value="..." runat="server" name="PermissivesButton" style="width: 20px; height: 20px; padding-left: 0; padding-right: 0">
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Delete">
                        <HeaderStyle Width="0.5in"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                        <ItemTemplate>
                            <FMControls:FMDeleteLinkButton runat="server" ID="Fmdeletelinkbutton1" NAME="Fmdeletelinkbutton1" />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
                <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
            </FMControls:FMDataGrid>
        </td>
    </tr>
    <tr>
        <td height="35">
            <FMControls:FMButton ID="AddButton" TabIndex="8" runat="server" Width="67px" CssClass="formfield" Text="Add" CommandName="AddExternalComponent"></FMControls:FMButton></td>
    </tr>
</table>
