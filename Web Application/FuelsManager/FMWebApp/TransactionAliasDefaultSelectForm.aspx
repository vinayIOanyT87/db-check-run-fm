<%@ Page language="c#" Codebehind="TransactionAliasDefaultSelectForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.TransactionAliasDefaultSelectForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ OutputCache Location="None" VaryByParam="None" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Default Transaction Aliases Selection</title>
		<base target="_self">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
<%=  Global.LinkAccessibilityCssUrl(Session) %>

		<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" defer="defer"></script>
		<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
		<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<script type="text/javascript">
			function Select(DefaultsID, Title)
			{
				var Result = new Array();
				Result[0] = DefaultsID;
				Result[1] = Title;
				window.returnValue = Result;
				window.close();
				setWindowReturnValue(Result);
				closeDialogWindow();
			}


			function NoSelect()
			{
				var Result=new Array();
				window.returnValue=Result;
				window.close();
				setWindowReturnValue(Result);
				closeDialogWindow();
			}
		</script>
		<form id="Form1" method="post" runat="server">
			<asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
			<FMControls:FMDataGrid ID="DefaultDataGrid" Style="z-index: 102; left: 8px; position: absolute; top: 45px" RowHeaderColumn="ID"
				TabIndex="5" runat="server" BackColor="White" Width="95%" CssClass="tabletext" PageSize="12" CellPadding="3"
				BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical" AutoGenerateColumns="False"
				BorderStyle="None">
				<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
				<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<HeaderStyle Width="50px"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="DefaultSelectionId" HeaderText="ID" Visible="False">
					</asp:BoundColumn>
					<asp:BoundColumn DataField="DefaultSelectionName" HeaderText="Transaction Alias Group">
						<HeaderStyle />
					</asp:BoundColumn>
				</Columns>
				<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
			</FMControls:FMDataGrid>
		</form>
	</body>
</HTML>
