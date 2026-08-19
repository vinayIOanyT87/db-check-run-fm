<%@ Page language="c#" Codebehind="FuelTicketsForm.aspx.cs" AutoEventWireup="True" Inherits="TicketingWebApp.FuelingTicketsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly = "FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
		<title>FuelingTicketsForm</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../FuelsManager.css" rel="stylesheet">
  </HEAD>
	<body MS_POSITIONING="GridLayout" tabindex=-1>
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
			<TABLE id="Table1" style="Z-INDEX: 100; LEFT: 32px; WIDTH: 43.18%; POSITION: absolute; TOP: 72px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 10px" width="498"><FMCONTROLS:FMDATAGRID id="FuelTicketsDataGrid" style="LEFT: 1px; TOP: 0px" runat="server" PageSize="8"
							BorderStyle="None" BackColor="White" AutoGenerateColumns="False" GridLines="Vertical" Width="600px" BorderWidth="1px" AllowSorting="True" BorderColor="White"
							CellPadding="3" AllowPaging="True" CssClass="tabletext">
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
										<asp:LinkButton ID="EditButton" runat="server" Text="<img src=..\FMWebApp\Images/Edit.gif border=0 align=absmiddle alt='Edit this item'>" CommandName="Edit" CausesValidation="false"></asp:LinkButton>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:LinkButton runat="server" Text="<img src=..\FMWebApp\Images\Update.gif border=0 align=absmiddle alt='Update this item'>" CommandName="Update" ID="Linkbutton1"></asp:LinkButton>&nbsp;
										<asp:LinkButton runat="server" Text="<img src=..\FMWebApp\Images\Cancel.gif border=0 align=absmiddle alt='Cancel Edit on this item'>" CommandName="Cancel" CausesValidation="false" ID="Linkbutton2"></asp:LinkButton>
									
</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:LinkButton ID="DeleteButton" runat="server" Text="<img src=..\FMWebApp\Images\Delete.gif border=0 align=absmiddle alt='Delete this item'>" CommandName="Delete" CausesValidation="false"></asp:LinkButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="Index" HeaderText="Index"></asp:BoundColumn>
								<asp:BoundColumn DataField="SequenceNumber" HeaderText="Sequence"></asp:BoundColumn>
								<asp:BoundColumn DataField="Date" HeaderText="Date"></asp:BoundColumn>
								<asp:BoundColumn DataField="Type" HeaderText="Type"></asp:BoundColumn>
								<asp:BoundColumn DataField="ManagerID" HeaderText="Manager"></asp:BoundColumn>
								<asp:BoundColumn DataField="OwnerID" HeaderText="Owner"></asp:BoundColumn>
								<asp:BoundColumn DataField="CarrierID" HeaderText="Agent"></asp:BoundColumn>
								<asp:BoundColumn DataField="ShipToID" HeaderText="Airline"></asp:BoundColumn>
								<asp:BoundColumn DataField="FlightNumber" HeaderText="Flight Number"></asp:BoundColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#0d246a" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDATAGRID></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 498px; HEIGHT: 10px" vAlign="middle" width="600">
						<table style="WIDTH: 272px; HEIGHT: 10px">
							<tr>
								<td style="WIDTH: 235px; HEIGHT: 10px"><FMCONTROLS:FMBUTTON id="AddButton" runat="server" Width="98px" Text="Add" tabIndex=1 CssClass="formfieldtitle"></FMCONTROLS:FMBUTTON></td>
								<td style="WIDTH: 498px; HEIGHT: 10px"><FMCONTROLS:FMBUTTON id="SendButton" runat="server" Width="104px" Text="Send" tabIndex=2 CssClass="formfieldtitle"></FMCONTROLS:FMBUTTON></td>
							</tr>
						</table>
					</TD>
				</TR>
			</TABLE>
			<FMCONTROLS:FMLABEL id="Label2" style="Z-INDEX: 102; LEFT: 32px; POSITION: absolute; TOP: 40px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Fuel Tickets:</FMCONTROLS:FMLABEL><asp:image id="Image1" style="Z-INDEX: 99; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="..\FMWebApp\images\Page_Fade_7.jpg"></asp:image><FMCONTROLS:FMLABEL id="Label3" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				BackColor="Transparent" Width="136px" CssClass="headline">Aviation</FMCONTROLS:FMLABEL></div>
</form>
	</body>
</HTML>
