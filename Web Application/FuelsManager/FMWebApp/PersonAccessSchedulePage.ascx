<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PersonAccessSchedulePage.ascx.cs" Inherits="FuelsManager.FMWebApp.PersonAccessSchedulePage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<TABLE id="Table1" style="Z-INDEX: 113; LEFT: 0px; WIDTH: 238px; POSITION: absolute; TOP: 16px; HEIGHT: 10px"
	cellSpacing="0" cellPadding="1" width="238" border="0">
	<TR>
		<TD width="445" height="10">
			<FMControls:FMDataGrid id="AccessScheduleDataGrid" runat="server" Width="552px" CssClass="tabletext" BackColor="White" RowHeaderColumn="Day"
				BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
				CellPadding="3" PageSize="8" OnItemDataBound="AccessScheduleDataGridItemDataBound">
				<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
				<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn HeaderText="Edit">
						<HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
						<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						<ItemTemplate>
							<FMControls:FMEditLinkButton runat="server" ID="Fmeditlinkbutton1" />
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
					<asp:TemplateColumn HeaderText="Day">
						<HeaderStyle Wrap="False"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Day") %>' ID="Label2">
							</asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Enabled">
						<HeaderStyle Wrap="False"></HeaderStyle>
						<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						<ItemTemplate>
							<FMControls:FMCheckBox runat="server" ToolTip="Enabled" Enabled="False" Checked='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' ID="EnabledCheckBox">
							</FMControls:FMCheckBox>
						</ItemTemplate>
						<EditItemTemplate>
							<FMControls:FMCheckBox runat="server" ToolTip="Enabled" CssClass=tabletext Checked='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' ID="EnabledCheckBox">
							</FMControls:FMCheckBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Opening Time">
						<HeaderStyle Wrap="False"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label width=.75in runat="server" Visible='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' Text='<%# DataBinder.Eval(Container, "DataItem.OpeningTime") %>' ID="Label3">
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<FMControls:FMTime width=.75in CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.OpeningTime") %>' ID="OpeningTime">
							</FMControls:FMTime>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Closing Time">
						<HeaderStyle Wrap="False"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label width=.75in runat="server" Visible='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' Text='<%# DataBinder.Eval(Container, "DataItem.ClosingTime") %>' ID="Label4">
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<FMControls:FMTime width=.75in CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ClosingTime") %>' ID="ClosingTime">
							</FMControls:FMTime>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
			</FMControls:FMDataGrid></TD>
	</TR>
</TABLE>
