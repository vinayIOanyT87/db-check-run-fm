<%@ Control language="c#" Codebehind="SiteHolidaySchedulePage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SiteHolidaySchedulePage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<LINK href="../CSS/FuelsManager.css" rel="stylesheet">
	<TABLE id="Table1" style="Z-INDEX: 113; LEFT: 0px; WIDTH: 238px; POSITION: absolute; TOP: 16px; HEIGHT: 10px"
		cellSpacing="0" cellPadding="1" width="238" border="0">
		<TR>
			<TD width="648" height="10"><FMControls:FMDataGrid id="HolidayScheduleDataGrid" runat="server" Width="700px" CssClass="tabletext" BackColor="White"
					BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White"
					CellPadding="3" PageSize="5" AllowPaging="True">
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
								<FMControls:FMEditLinkButton runat="server" />
							</ItemTemplate>
							<EditItemTemplate>
								<FMControls:FMUpdateLinkButton runat="server" />
								<FMControls:FMCancelLinkButton runat="server" />
						</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Delete">
							<HeaderStyle Width="0.5in"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<FMControls:FMDeleteLinkButton runat="server" ID="Fmdeletelinkbutton1" />
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn Visible="False" HeaderText="Index">
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ScheduleHolidayGuid") %>' ID="IndexLabel">
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Holiday Date">
							<HeaderStyle Wrap="False"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DayText") %>' ID="Label4">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<FMControls:FMDate CssClass="tabletext" ID="HolidayDate" runat="server"></FMControls:FMDate>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Enabled">
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<asp:CheckBox runat="server" Enabled="False" Checked='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' ID="Checkbox1">
								</asp:CheckBox>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:CheckBox runat="server" CssClass=tabletext Checked='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' ID="EnabledCheckBox">
								</asp:CheckBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Opening Time">
							<HeaderStyle Wrap="False"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
							<ItemTemplate>
								<asp:Label width=.75in runat="server" Visible='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' Text='<%# DataBinder.Eval(Container, "DataItem.OpeningTime") %>' ID="Label1">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<FMControls:FMTime CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.OpeningTime") %>' ID="OpeningTime">
								</FMControls:FMTime>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Closing Time">
							<HeaderStyle Wrap="False"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
							<ItemTemplate>
								<asp:Label width=.75in runat="server" Visible='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' Text='<%# DataBinder.Eval(Container, "DataItem.ClosingTime") %>' ID="Label2">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<FMControls:FMTime CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ClosingTime") %>' ID="ClosingTime">
								</FMControls:FMTime>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="EOD">
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<asp:CheckBox runat="server" Enabled="False" Checked='<%# DataBinder.Eval(Container, "DataItem.EndOfDayEnabled") %>' ID="Checkbox2">
								</asp:CheckBox>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:CheckBox runat="server" CssClass=tabletext Checked='<%# DataBinder.Eval(Container, "DataItem.EndOfDayEnabled") %>' ID="EndOfDayEnabledCheckBox">
								</asp:CheckBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="End Of Day Time">
							<HeaderStyle Wrap="False"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
							<ItemTemplate>
								<asp:Label width=.75in runat="server" Visible='<%# DataBinder.Eval(Container, "DataItem.EndOfDayEnabled") %>' Text='<%# DataBinder.Eval(Container, "DataItem.EndOfDayTime") %>' ID="Label3">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<FMControls:FMTime CssClass=tabletext runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.EndOfDayTime") %>' ID="EndOfDayTime">
								</FMControls:FMTime>
							</EditItemTemplate>
						</asp:TemplateColumn>
					</Columns>
					<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
				</FMControls:FMDataGrid></TD>
		</TR>
		</TR>
		<TR>
			<TD vAlign="middle" width="549" height="50"><FMControls:FMButton id="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfield"></FMControls:FMButton></TD>
		</TR>
	</TABLE>
