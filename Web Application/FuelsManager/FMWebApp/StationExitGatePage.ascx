<%@ Control language="c#" Codebehind="StationExitGatePage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.StationExitGatePage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
<div style="margin-left: 40px">            
        </div>
	<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 0px; WIDTH: 600px; POSITION: absolute; TOP: 16px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">				
				<TR>
					<TD style="WIDTH: 500px; HEIGHT: 10px">
						<FMControls:FMDataGrid id="ProcessVariablesDataGrid"  runat="server" BorderStyle="None" 
                            BackColor="White" AutoGenerateColumns="False"
							GridLines="Vertical" Width="648px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3"
							AllowPaging="True" CssClass="tabletext" PageSize="6" tabIndex="7">
							<FooterStyle ForeColor="Black" BackColor="#0d246a"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#0d246a"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton ID="FMEditLinkButton1" runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
								 <asp:BoundColumn Visible="False" DataField="Index" HeaderText="Index"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Host" HeaderText="System"></asp:BoundColumn>           
                                <asp:BoundColumn DataField="OPCServerID" HeaderText="OPC Server"></asp:BoundColumn>
                                <asp:BoundColumn DataField="OPCItemID" HeaderText="Item ID"></asp:BoundColumn>
                                <asp:BoundColumn DataField="MessageID" HeaderText="Message"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton ID="FMDeleteLinkButton1" runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#0d246a" Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid></TD>
				</TR>
				<tr>
					<TD vAlign="middle">
                        <FMControls:FMButton id="AddButton" runat="server" width="100px" Text="Add" CssClass="formfieldtitle" tabIndex="8"></FMControls:FMButton>
                        <FMControls:FMCheckbox width="150px" id="QueryForTrailers" runat="server" Text="Prompt for Equipment" CssClass="formfieldtitle" />
					</TD>
				</tr>
			</TABLE>
	</body>
</HTML>
