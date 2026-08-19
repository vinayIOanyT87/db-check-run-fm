<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="RackStatusForm.aspx.cs" AutoEventWireup="True" Inherits="LoadRackWebApp.RackStatusForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
      <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	  <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	  <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
      <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
      <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
		<script language="jscript"> 
			var rackStatusRefreshTimeoutID=0;
		
			function RackStatusRefresh()
			{
				rackStatusRefreshTimeoutID = setTimeout("RackStatusRefresh()", 5000);
				__doPostBack('RackStatusUpdatePanel','');
			}

			function RackStatusUnload()
			{
				if(rackStatusRefreshTimeoutID != 0)
					window.clearTimeout(rackStatusRefreshTimeoutID);
			}

			function PermissivesButton_Click(IdentityGuid) {
				if(rackStatusRefreshTimeoutID != 0)
					window.clearTimeout(rackStatusRefreshTimeoutID);

				showModalDialogFrame({
							url:"PermissivesStatusForm.aspx?IdentityGuid=" + IdentityGuid,
							width: 725,
							height: 530,
							title: "Permissives Status",
    						onClose: function () {
								RackStatusRefresh ();
							}
				});
			}

        </script>
</head>
<body ms_positioning="GridLayout">
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="224px" BackColor="Transparent">Load Rack Status</FMControls:FMLabel>
			<table id="Table1" style="z-index: 100; left: 8px; width: 80%; position: absolute; top: 48px; height: 50px"
				cellspacing="0" cellpadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<TD width="498" height="36" vAlign="middle">
						<FMControls:FMPageSizeDropDown ID="RackStatusFormPageSizeDropDown" ToolTip="Page size" runat="server" onselectedindexchanged="PageSizeDropDown_SelectedIndexChanged" />
					</TD>
				</tr>
				<tr>
					<td style="height: 10px" width="80%">
			            <asp:ScriptManager ID="RackStatusScriptManager" runat="server" EnablePartialRendering="true" />
							<asp:UpdatePanel ID="RackStatusUpdatePanel" runat="server">
								<ContentTemplate>
								<FMControls:FMDataGrid ID="RackStatusDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False" RowHeaderColumn="Rack"
									GridLines="Vertical" Width="776px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext"
									aria-label="Rack Status Grid">
									<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
									<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
									<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
									<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
									<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
									<Columns>
										<asp:BoundColumn DataField="RackID" HeaderText="Rack">
											<HeaderStyle Wrap="False"></HeaderStyle>
										</asp:BoundColumn>
										<asp:BoundColumn DataField="Status" HeaderText="Status"></asp:BoundColumn>
										<asp:BoundColumn DataField="DriverID" HeaderText="Driver">
											<HeaderStyle Wrap="False"></HeaderStyle>
											<ItemStyle Wrap="False"></ItemStyle>
										</asp:BoundColumn>
										<asp:BoundColumn DataField="CarrierID" HeaderText="Carrier">
											<HeaderStyle Wrap="False"></HeaderStyle>
											<ItemStyle Wrap="False"></ItemStyle>
										</asp:BoundColumn>
										<asp:BoundColumn DataField="Equipment1ID" HeaderText="Tractor/Tanker">
											<HeaderStyle Wrap="False"></HeaderStyle>
											<ItemStyle Wrap="False"></ItemStyle>
										</asp:BoundColumn>
										<asp:BoundColumn DataField="Equipment2ID" HeaderText="Trailer 1">
											<HeaderStyle Wrap="False"></HeaderStyle>
											<ItemStyle Wrap="False"></ItemStyle>
										</asp:BoundColumn>
										<asp:BoundColumn DataField="Equipment3ID" HeaderText="Trailer 2">
											<HeaderStyle Wrap="False"></HeaderStyle>
											<ItemStyle Wrap="False"></ItemStyle>
										</asp:BoundColumn>
										<asp:BoundColumn DataField="ShipToID/SupplierID" HeaderText="Ship To/Supplier">
											<HeaderStyle Width="1.0in"></HeaderStyle>
										</asp:BoundColumn>
										<asp:BoundColumn DataField="LoadID" HeaderText="Load ID"></asp:BoundColumn>
										<asp:TemplateColumn HeaderText="Permissives">
											<HeaderStyle Width="0.5in"></HeaderStyle>
											<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
											<ItemTemplate>
												<INPUT class=formfieldtitle id=PermissivesButton onclick='<%# this.Server.HtmlDecode(Convert.ToString(DataBinder.Eval(Container, "DataItem.PermissivesClick"))) %>' type=button value="..." runat="server" Name="PermissivesButton" style="width: 20px; height:20px; padding-left:0;padding-right:0">
											</ItemTemplate>
										</asp:TemplateColumn>
									</Columns>
									<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
								</FMControls:FMDataGrid>
				            </ContentTemplate>
						</asp:UpdatePanel>
					</td>
				</tr>
			</table>
		</div>
	</form>
	<script type="text/javascript">
		window.onbeforeunload = RackStatusUnload;
		rackStatusRefreshTimeoutID = setTimeout("RackStatusRefresh()", 5000);
    </script>
</body>
</html>
