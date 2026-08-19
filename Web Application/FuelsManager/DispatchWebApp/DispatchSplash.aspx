<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DispatchSplash.aspx.cs" Inherits="FuelsManager.DispatchWebApp.DispatchSplash" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" type="text/css" rel="stylesheet" />
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
</head>
<body>
    <form id="form1" runat="server">
		<FMControls:FMLabel id="Label6" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
			CssClass="headline" Width="400px" BackColor="Transparent">Dispatch</FMControls:FMLabel>
		<asp:Image id="Image1" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
			ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
    </form>
</body>
</html>
