<%@ Page language="c#" Codebehind="OffLoadingExternalProductInputForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.OffLoadingExternalProductInputForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>OffLoadingExternalProductInputForm</title>
		<base target="_self">
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
		<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" defer="defer"></script>
      <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
      <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<FMCONTROLS:FMLABEL id="ConfigurationLabel" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" Width="632px" CssClass="headline" BackColor="Transparent">External Product Input Configuration</FMCONTROLS:FMLABEL>
			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 24px; WIDTH: 39.37%; POSITION: absolute; TOP: 48px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<TD width="710" height="10">
						<FMCONTROLS:FMDATAGRID id="InputDataGrid" tabIndex="1" runat="server" Width="680px" CssClass="tabletext"
							BackColor="White" Height="10px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical"
							BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" PageSize="5">
							<FooterStyle ForeColor="Black" BackColor="#0d246a"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<EditItemStyle Wrap="False"></EditItemStyle>
							<AlternatingItemStyle Wrap="False" BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#0d246a"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMCONTROLS:FMEDITLINKBUTTON id="FMEditLinkButton" runat="server" NAME="FMEditLinkButton"></FMCONTROLS:FMEDITLINKBUTTON>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Index">
									<ItemTemplate>
										<asp:Label id=InputIndexLabel runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="TypeID" HeaderText="Type"></asp:BoundColumn>
								<asp:BoundColumn DataField="Host" HeaderText="System">
									<HeaderStyle Width="0.5in"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="OPCServerID" HeaderText="OPC Server">
									<HeaderStyle Width="1.5in"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="OPCItemID" HeaderText="OPC Item ID">
									<HeaderStyle Width="3in"></HeaderStyle>
								</asp:BoundColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#0d246a" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDATAGRID>
					</TD>
				</tr>
			</TABLE>
		</form>
	</body>
</HTML>
