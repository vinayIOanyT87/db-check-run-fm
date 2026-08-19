<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GraphicalView.aspx.cs"
	Inherits="FuelsManager.DispatchWebApp.GraphicalView" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html xml:lang="en" lang="en">
<head>
	<title></title>

	<meta http-equiv="CACHE-CONTROL" content="NO-CACHE" />

	<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/menu.css" %>" type="text/css" />
	<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/css/dispatch.css" %>" type="text/css"/>

	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery-1.7.1.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery-ui-1.8.17.custom.min.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery.event.drop-2.0.min.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/jquery.event.drag-2.0.min.js" %>" type="text/javascript"></script>

	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/canvasExtensions.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/dispatch.js" %>" type="text/javascript"></script>
	<script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/dispatchwebapp/lib/graphicalView.js" %>" type="text/javascript"></script>

	<script type="text/javascript">
		GraphicalViewLib.securityToken = '<%= Security.Token.ToString() %>';

		$(document).ready(DispatchLib.applicationLoad);
		$(document).ready(GraphicalViewLib.graphicalPageLoad);
	</script>
</head>
<body oncontextmenu="return false;">
	<form runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent">
			<div id="graphicalViewPanel" style="position:absolute; text-align: center">
		<FMControls:FMToolbar runat="server" ID="toolBarGraphical" CssClass="listToolBar" />
		<canvas id="graphicalCanvas" class="mainCanvasTransparent" style="margin-top: 5px">
			ERROR: Your current browser does not support the canvas element required by this application.
		</canvas>
	</div>
		</div>
	</form>
</body>
</html>
