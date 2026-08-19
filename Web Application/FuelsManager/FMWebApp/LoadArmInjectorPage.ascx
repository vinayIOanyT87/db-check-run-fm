<%@ Control language="c#" Codebehind="LoadArmInjectorPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.LoadArmInjectorPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 0px; WIDTH: 39.37%; POSITION:absolute; TOP: 16px; HEIGHT: 10px"
		cellSpacing="0" cellPadding="1" border="0"LoadRackTextTextBox role="presentation" aria-label="layout">
		<tr>
			<TD width="1000px" height="10">
				<FMControls:FMDataGrid id="DataGrid" runat="server" BackColor="White" Width="950px" CssClass="tabletext"  RowHeaderColumn="Injector"
					Height="10px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
					CellPadding="3" PageSize="8" AllowPaging="True" aria-label="Load Arm Injectors">
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
								<FMControls:FMUpdateLinkButton runat="server" />&nbsp;<FMControls:FMCancelLinkButton runat="server" />
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn Visible="False" HeaderText="Index">
							<ItemTemplate>
								<asp:Label ID="IndexLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'>
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Injector">
							<HeaderStyle Width="0.25in"></HeaderStyle>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.PresetNumber") %>' ID="Label1">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox Width=".25in" runat="server" CssClass=tabletext Text='<%# DataBinder.Eval(Container, "DataItem.PresetNumber") %>' ID="PresetNumberTextBox" ToolTip="Injector">
								</asp:TextBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Meter">
							<ItemTemplate>
								<asp:Label runat="server" Width="1in" Text='<%# DataBinder.Eval(Container, "DataItem.MeterID") %>' ID="MeterIDLabel">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
							    <FMControls:FMTextBox runat="server" Width="100px" ID="MeterIDTextBox" ToolTip="Meter" MaxLength="30" Text='<%# DataBinder.Eval(Container, "DataItem.MeterID") %>'/>
							</EditItemTemplate>
						</asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Meter # of Digits">
							<HeaderStyle Width="75px" />
							<ItemTemplate>
								<FMControls:FMLabel ID="NumberOfDigitsLabel" Text='<%# DataBinder.Eval(Container, "DataItem.NumberOfDigits") %>' runat="server" />
							</ItemTemplate>
						    <EditItemTemplate>
								<FMControls:FMTextBox ID="NumberOfDigitsTextBox" ToolTip="Number of Digits" Text='<%# DataBinder.Eval(Container, "DataItem.NumberOfDigits") %>' runat="server" Width="50px" MaxLength="2"/>
							</EditItemTemplate>
						</asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Meter Rotates Backwards">
							<HeaderStyle Width="115px" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<FMControls:FMCheckBox ID="RotatesBackwardsDisplayCheckBox" Checked='<%# DataBinder.Eval(Container, "DataItem.RotatesBackwardsFlag") %>' runat="server" Enabled="false"/>
							</ItemTemplate>
                            <EditItemTemplate>
								<FMControls:FMCheckBox ID="RotatesBackwardsEditCheckBox" Checked='<%# DataBinder.Eval(Container, "DataItem.RotatesBackwardsFlag") %>' runat="server" />
							</EditItemTemplate>
						</asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Receipt Meter">
							<HeaderStyle Width="100px" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<FMControls:FMCheckBox ID="ReceiptMeterDisplayCheckBox" Checked='<%# DataBinder.Eval(Container, "DataItem.ReceiptMeterFlag") %>' runat="server" Enabled="false" />
							</ItemTemplate>
                            <EditItemTemplate>
								<FMControls:FMCheckBox ID="ReceiptMeterEditCheckBox" Checked='<%# DataBinder.Eval(Container, "DataItem.ReceiptMeterFlag") %>' runat="server" />
							</EditItemTemplate>
						</asp:TemplateColumn>
                        
						<asp:TemplateColumn HeaderText="Internal">
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<asp:CheckBox runat="server" CssClass=tabletext Enabled=false Checked='<%# DataBinder.Eval(Container, "DataItem.Internal") %>' ID="ItemInternalCheckBox" NAME="ItemInternalCheckBox">
								</asp:CheckBox>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:CheckBox runat="server" CssClass=tabletext Enabled=true Checked='<%# DataBinder.Eval(Container, "DataItem.Internal") %>' ID="EditInternalCheckBox" NAME="EditInternalCheckBox">
								</asp:CheckBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Roll Over">
							<ItemTemplate>
								<asp:Label runat="server" Width="1in" Text='<%# DataBinder.Eval(Container, "DataItem.RollOver") %>' ID="RollOverLabel">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox runat="server" CssClass=tabletext Width="1in" Text='<%# DataBinder.Eval(Container, "DataItem.RollOver") %>' ID="RollOverTextBox" ToolTip="Roll Over" MaxLength=10>
								</asp:TextBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Additive">
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ProductID") %>' ID="Label2">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:dropdownlist CssClass=tabletext runat="server" Enabled="True" ID="ProductsDropDownList" ToolTip="Additive" DataSource="<%# EnumerateProducts()%>" DataTextField="Text" DataValueField="Value" AutoPostBack="True" OnSelectedIndexChanged="ProductDropDownList_SelectedIndexChanged">
								</asp:dropdownlist>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Location">
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.LocationID") %>' ID="Label3">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:dropdownlist CssClass=tabletext runat="server" Enabled="True" ID="LocationDropDownList" ToolTip="Location" DataSource="<%# EnumerateLocations()%>" DataTextField="Text" DataValueField="Value">
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
				</FMControls:FMDataGrid>
			</TD>
		</tr>
		<tr>
			<td height="35"><FMControls:FMButton id="AddButton" tabIndex="8" runat="server" Width="67px" CssClass="formfield" Text="Add" CommandName="AddInjector"></FMControls:FMButton></td>
		</tr>
	</TABLE>
