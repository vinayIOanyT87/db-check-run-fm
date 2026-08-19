<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StationDeFuelMeterPage.ascx.cs" Inherits="FuelsManager.FMWebApp.StationDeFuelMeterPage" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
	<FMControls:FMDataGrid id="ProcessVariablesDataGrid" style="Z-INDEX: 107; LEFT: 0px; POSITION: absolute; TOP: 16px"
		runat="server" PageSize="1" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
		GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None" Width="552px" CssClass="tabletext"
		BackColor="White" aria-label="Process Variables">
		<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
		<EditItemStyle Wrap="False"></EditItemStyle>
		<AlternatingItemStyle Wrap="False" BackColor="Gainsboro"></AlternatingItemStyle>
		<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
		<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
		<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
		<Columns>
			<asp:ButtonColumn Text="&lt;img src=Images/Edit.gif border=0 align=absmiddle alt='Edit this item'&gt;"
				HeaderText="Edit" CommandName="Edit">
				<HeaderStyle Width="55px"></HeaderStyle>
				<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
			</asp:ButtonColumn>
			<asp:BoundColumn Visible="False" DataField="Index" HeaderText="Index"></asp:BoundColumn>
			<asp:BoundColumn DataField="OPCServerID" HeaderText="OPC Server"></asp:BoundColumn>
			<asp:BoundColumn DataField="OPCItemID" HeaderText="Item ID"></asp:BoundColumn>
		</Columns>
		<PagerStyle CssClass="tablepager" ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
	</FMControls:FMDataGrid>
	</body>
</HTML>
