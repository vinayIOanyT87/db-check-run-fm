<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="StationOffLoadingProductPage.ascx.cs" Inherits="FuelsManager.FMWebApp.StationOffLoadingProductPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
<table id="Table1" style="Z-INDEX: 102; LEFT: 0; WIDTH: 39.37%; POSITION: absolute; TOP: 16px; HEIGHT: 10px"
	cellSpacing="0" cellPadding="1" border="0">
	<tr>
		<TD width="710" height="10"><FMControls:FMDataGrid id="DataGrid" runat="server" BackColor="White" Width="520px" CssClass="tabletext"
				Height="10px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
				CellPadding="3" PageSize="8" AllowPaging="True">
				<FooterStyle ForeColor="Black" BackColor="#0d246a"></FooterStyle>
				<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
				<EditItemStyle Wrap="False"></EditItemStyle>
				<AlternatingItemStyle Wrap="False" BackColor="Gainsboro"></AlternatingItemStyle>
				<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#0d246a"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn HeaderText="Edit">
						<HeaderStyle Width="0.5in"></HeaderStyle>
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
					<asp:TemplateColumn HeaderText="Meter">
						<ItemTemplate>
							<asp:Label runat="server" Width="1in" Text='<%# DataBinder.Eval(Container, "DataItem.MeterID") %>' ID="Label4">
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox runat="server" CssClass=tabletext Width="1in" Text='<%# DataBinder.Eval(Container, "DataItem.MeterID") %>' ID="MeterIDTextBox" MaxLength=20>
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Component">
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' ID="Label2">
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:dropdownlist CssClass="tabletext" runat="server" Enabled="True" ID="ProductsDropDownList" DataSource="<%# EnumerateProducts()%>" DataTextField="Text" DataValueField="Value" AutoPostBack="True" OnSelectedIndexChanged="ProductDropDownListSelectedIndexChanged">
							</asp:dropdownlist>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Location">
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.LocationID") %>' ID="Label3">
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:dropdownlist CssClass="tabletext" runat="server" Enabled="True" ID="LocationDropDownList" DataSource="<%# EnumerateLocations()%>" DataTextField="Text" DataValueField="Value">
							</asp:dropdownlist>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Input">
						<HeaderStyle Width="0.5in"></HeaderStyle>
						<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						<ItemTemplate>
						    <input class="formfieldtitle" id="InputButton" onclick='<%# DataBinder.Eval(Container, "DataItem.InputsClick") %>' type="button" value="..." runat="server" name="InputButton" style="width: 20px; height: 20px" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Permissives">
						<HeaderStyle Width="0.5in"></HeaderStyle>
						<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						<ItemTemplate>
						    <input class="formfieldtitle" id="PermissivesButton" onclick='<%# this.Server.HtmlDecode(Convert.ToString(DataBinder.Eval(Container, "DataItem.PermissivesClick"))) %>' type="button" value="..." runat="server" name="PermissivesButton" style="width: 20px; height: 20px" />
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
				<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#0d246a" Mode="NumericPages"></PagerStyle>
			</FMControls:FMDataGrid>
		</TD>
	</tr>
	<tr>
		<td height="35"><FMControls:FMButton id="AddButton" tabIndex="8" runat="server" Width="67px" CssClass="formfield" Text="Add"></FMControls:FMButton></td>
	</tr>
</table>
