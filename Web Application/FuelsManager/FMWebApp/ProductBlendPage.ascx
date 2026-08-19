<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="ProductBlendPage.ascx.cs" AutoEventWireup="True"
	Inherits="FuelsManager.FMWebApp.ProductBlendPage" %>
<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
<FMControls:FMLabel ID="Label3" Style="z-index: 123; left: 0px; position: absolute;
	top: 24px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Components:</FMControls:FMLabel>
<asp:TextBox ID="AllowableToleranceTextbox" Style="z-index: 135; left: 152px; position: absolute;
	top: 248px" runat="server" Width="66px" CssClass="formfield"></asp:TextBox>
<FMControls:FMLabel ID="Label10" Style="z-index: 134; left: 0px; position: absolute;
	top: 248px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Allowable Tolerance:</FMControls:FMLabel>
<asp:Label ID="Fmlabel1" Style="z-index: 134; left: 232px; position: absolute; top: 248px"
	runat="server" CssClass="formfieldtitle" BackColor="Transparent">%</asp:Label>
<table id="Table1" style="z-index: 133; left: 0px; width: 238px; position: absolute;
	top: 56px; height: 10px" cellspacing="0" cellpadding="1" width="238" border="0">
	<tr>
		<td height="10" valign="top">
			<FMControls:FMDataGrid ID="ComponentsDataGrid" runat="server" CssClass="tabletext" RowHeaderColumn="Component ID"
				BackColor="White" Width="320px" BorderStyle="None" AutoGenerateColumns="False"
				GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
				CellPadding="3" AllowPaging="True" PageSize="4">
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
				<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>">
				</HeaderStyle>
				<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
				<Columns>
					<asp:TemplateColumn HeaderText="Select">
						<HeaderStyle Width="0.5in" />
						<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
						<ItemTemplate>
							<FMControls:FMSelectLinkButton runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Edit">
						<HeaderStyle Width="0.5in" />
						<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
						<ItemTemplate>
							<FMControls:FMEditLinkButton ID="RecordEditBtn" runat="server" />
						</ItemTemplate>
						<EditItemTemplate>
							<FMControls:FMUpdateLinkButton runat="server" />&nbsp;<FMControls:FMCancelLinkButton
								runat="server" />
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn Visible="False" HeaderText="Index">
						<ItemTemplate>
							<asp:Label ID="IndexLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'>
							</asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Component ID">
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList CssClass="tabletext" runat="server" Enabled="True" ID="ProductsDropDownList"
								DataSource="<%# EnumerateComponentProducts()%>" DataTextField="Text" DataValueField="Value">
							</asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Percent">
						<ItemTemplate>
							<asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Percent") %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox Width=".5in" CssClass="tabletext" ID="PercentTextBox" runat="server"
								Text='<%# DataBinder.Eval(Container, "DataItem.Percent") %>'>
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Delete">
						<HeaderStyle Width="0.5in" />
						<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
						<ItemTemplate>
							<FMControls:FMDeleteLinkButton ID="RecordDeleteBtn" runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>"
					Mode="NumericPages"></PagerStyle>
			</FMControls:FMDataGrid>
		</td>
        <td valign="top">
            <FMControls:FMButton ID="UpButton" Style="z-index: 138" runat="server" CssClass="formfieldtitle" Width="50px" Text="Up"/><br/><br/>
            <FMControls:FMButton ID="DownButton" Style="z-index: 137" runat="server" CssClass="formfieldtitle" Width="50px" Text="Down"/>
        </td>
	</tr>
	<tr>
		<td height="10">
			<FMControls:FMButton ID="AddButton" Width="66px" runat="server" CssClass="formfieldtitle" Text="Add">
			</FMControls:FMButton>
		</td>
	</tr>
</table>
