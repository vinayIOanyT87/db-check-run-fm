<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="StationLoadArmsPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.StationLoadArmsPage" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<TABLE id="Table1" style="Z-INDEX: 100; LEFT: 0px; WIDTH: 43.18%; POSITION: absolute; TOP: 16px; HEIGHT: 10px"
		cellSpacing="0" cellPadding="1" border="0" role="presentation" aria-label="layout">
		<TR>
			<TD width="498" height="10"><FMControls:FMDataGrid id="LoadArmsDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False"
					GridLines="Vertical" Width="624px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True"
					CssClass="tabletext" style="LEFT: 1px; TOP: 0px" PageSize="8" tabIndex="1">
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
								<FMControls:FMEditLinkButton runat="server" />
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn Visible="False" DataField="Index" HeaderText="Index">
							<HeaderStyle Wrap="False"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
							<FooterStyle Wrap="False"></FooterStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="ArmNumber" HeaderText="Arm">
							<HeaderStyle Width="0.5in"></HeaderStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="LoadRackText" HeaderText="Load Rack Text">
							<HeaderStyle Wrap="False"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="PresetTypeID" HeaderText="Preset Type">
							<HeaderStyle Wrap="False"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="OPCServer" HeaderText="OPC Server">
							<HeaderStyle Wrap="False"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="OPCItemID" HeaderText="OPC Item ID">
							<HeaderStyle Wrap="False"></HeaderStyle>
							<ItemStyle Wrap="False"></ItemStyle>
						</asp:BoundColumn>
						<asp:TemplateColumn HeaderText="Position">
							<HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
							<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<FMControls:FMButton runat="server" CssClass="tabletext" ID="UpButton" Text="Up" style="Width:40px;"
									CommandName="UpButton" />
								<FMControls:FMButton runat="server" CssClass="tabletext" ID="DownButton" Text="Down" style="Width:40px;"
									CommandName="DownButton" />
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Delete">
							<HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
							<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<FMControls:FMDeleteLinkButton runat="server" />
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
					<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
				</FMControls:FMDataGrid></TD>
		</TR>
		<TR>
			<TD vAlign="middle" width="498" height="29"><FMControls:FMButton id="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
					tabIndex="2"></FMControls:FMButton></TD>
		</TR>
	</TABLE>
