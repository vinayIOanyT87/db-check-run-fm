<%@ Control language="c#" Codebehind="LoadArmRecipePage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.LoadArmRecipePage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 0px; WIDTH: 39.37%; POSITION: absolute; TOP: 16px; HEIGHT: 10px"
		cellSpacing="0" cellPadding="1" border="0" role="presentation" aria-label="layout">
		<tr>
			<TD width="710" height="10">
				<FMControls:FMDataGrid id="DataGrid" runat="server" BackColor="White" Width="536px" CssClass="tabletext" RowHeaderColumn="Recipe"
					Height="10px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px"
					AllowSorting="True" BorderColor="White" CellPadding="3" PageSize="8" AllowPaging="True" aria-label="Load Arm Recipes">
					<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
					<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
					<EditItemStyle Wrap="False"></EditItemStyle>
					<AlternatingItemStyle Wrap="False" BackColor="Gainsboro"></AlternatingItemStyle>
					<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
					<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
					<Columns>
						<asp:TemplateColumn HeaderText="Edit">
							<HeaderStyle Width="55px"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<FMControls:FMEditLinkButton runat="server" />
							</ItemTemplate>
							<EditItemTemplate>
								<FMControls:FMUpdateLinkButton runat="server" />&nbsp;
								<FMControls:FMCancelLinkButton runat="server" />
							</EditItemTemplate>
						</asp:TemplateColumn>
                  <asp:TemplateColumn HeaderText="Enabled">
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<asp:CheckBox ID="ItemEnableRecipeCheckbox" runat="server" CssClass=tabletext Enabled=false Checked='<%# DataBinder.Eval(Container, "DataItem.EnableRecipe") %>'>
								</asp:CheckBox>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:CheckBox ID="EnableRecipeCheckbox" runat="server" CssClass=tabletext Enabled=true Checked='<%# DataBinder.Eval(Container, "DataItem.EnableRecipe") %>'>
								</asp:CheckBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn Visible="False" HeaderText="Index">
							<ItemTemplate>
								<asp:Label ID="IndexLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'>
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Recipe">
							<HeaderStyle Wrap="False"></HeaderStyle>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.PresetNumber") %>' ID="Label4">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox runat="server" CssClass=tabletext Text='<%# DataBinder.Eval(Container, "DataItem.PresetNumber") %>' ID="PresetNumberTextBox" ToolTip="Recipe">
								</asp:TextBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Product">
							<HeaderStyle Wrap="False"></HeaderStyle>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' ID="Label2">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:dropdownlist CssClass=tabletext runat="server" Enabled="True" ID="ProductsDropDownList" ToolTip="Product" DataSource="<%# EnumerateProducts()%>" DataTextField="Text" DataValueField="Value" AutoPostBack="True" OnSelectedIndexChanged="ProductDropDownList_SelectedIndexChanged">
								</asp:dropdownlist>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Permissives">
							<HeaderStyle Width="0.5in"></HeaderStyle>
							<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<INPUT class=formfieldtitle id=PermissivesButton onclick='<%# this.Server.HtmlDecode(Convert.ToString(DataBinder.Eval(Container, "DataItem.PermissivesClick"))) %>' type=button value="..." runat="server" Name="PermissivesButton" style="width: 20px; height:20px; padding-left:0;padding-right:0">
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Delete">
							<HeaderStyle Width="0.5in"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<FMControls:FMDeleteLinkButton runat="server" />
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
					<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
				</FMControls:FMDataGrid></TD>
		</tr>
		<tr>
			<td height="35"><FMControls:FMButton id="AddButton" tabIndex="8" runat="server" Width="67px" CssClass="formfield" Text="Add" CommandName="AddRecipe"></FMControls:FMButton></td>
		</tr>
	</TABLE>
