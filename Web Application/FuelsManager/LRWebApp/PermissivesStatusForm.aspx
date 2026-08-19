<%@ Page language="c#" Codebehind="PermissivesStatusForm.aspx.cs" AutoEventWireup="True" Inherits="LoadRackWebApp.PermissivesStatusForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title></title>
		<base target="_self">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
		<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:image id="Image1" style="Z-INDEX: 99; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image><FMCONTROLS:FMLABEL id="Label2" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				BackColor="Transparent" Width="224px" CssClass="headline">Permissives Status</FMCONTROLS:FMLABEL><FMCONTROLS:FMLABEL id="TypeLabel" style="Z-INDEX: 105; LEFT: 24px; POSITION: absolute; TOP: 40px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Type:</FMCONTROLS:FMLABEL><FMCONTROLS:FMDROPDOWNLIST id="TypeDropDownList" style="Z-INDEX: 106; LEFT: 24px; POSITION: absolute; TOP: 64px"
				tabIndex="1" runat="server" Width="256px" CssClass="formfield" AutoPostBack="True"></FMCONTROLS:FMDROPDOWNLIST>
			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 24px; WIDTH: 39.37%; POSITION: absolute; TOP: 96px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<TD width="710" height="10">
			            <asp:ScriptManager ID="PermissivesScriptManager" runat="server" EnablePartialRendering="true" />
							<asp:UpdatePanel ID="PermissivesUpdatePanel" runat="server">
								<ContentTemplate>
									<FMCONTROLS:FMDATAGRID id="PermissivesStatusDataGrid" tabIndex="1" runat="server" BackColor="White" Width="656px"
										CssClass="tabletext" PageSize="5" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical"
										AutoGenerateColumns="False" BorderStyle="None" Height="10px">
										<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
										<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
										<EditItemStyle Wrap="False"></EditItemStyle>
										<AlternatingItemStyle Wrap="False" BackColor="Gainsboro"></AlternatingItemStyle>
										<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
										<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
										<Columns>
											<asp:BoundColumn DataField="Host" HeaderText="System"></asp:BoundColumn>
											<asp:BoundColumn DataField="OPCServerID" HeaderText="OPC Server"></asp:BoundColumn>
											<asp:BoundColumn DataField="OPCItemID" HeaderText="OPC Item ID"></asp:BoundColumn>
											<asp:BoundColumn DataField="CurrentValue" HeaderText="Current Value"></asp:BoundColumn>
											<asp:BoundColumn DataField="OutputFailed" HeaderText="Output Failed"></asp:BoundColumn>
											<asp:BoundColumn DataField="MessageID" HeaderText="Message"></asp:BoundColumn>
										</Columns>
										<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
									</FMCONTROLS:FMDATAGRID>
				            </ContentTemplate>
						</asp:UpdatePanel>
					</TD>
				</tr>
			</TABLE>
		</form>
		<script type="text/javascript">
			var permissivesRefreshTimeoutID=0;
		
			function PermissiveRefresh()
			{
				permissivesRefreshTimeoutID = setTimeout("PermissiveRefresh()", 5000);
				__doPostBack('PermissivesUpdatePanel','');
			}

			function PermissiveUnload()
			{
				if(permissivesRefreshTimeoutID != 0)
					window.clearTimeout(permissivesRefreshTimeoutID);
			}

			window.onbeforeunload = PermissiveUnload;
			permissivesRefreshTimeoutID = setTimeout("PermissiveRefresh()", 5000);
        </script>
	</body>
</HTML>
