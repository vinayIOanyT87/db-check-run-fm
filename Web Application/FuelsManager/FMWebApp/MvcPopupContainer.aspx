<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MvcPopupContainer.aspx.cs" Inherits="FuelsManager.FMWebApp.MvcPopupContainer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
</head>
<script type="text/javascript">
	function iframeLoaded()
	{
		var iFrameId = document.getElementById('iframeContent');

		if (iFrameId)
		{
			iFrameId.scrolling = "no";
			iFrameId.height = "800px";
			iFrameId.width = "1100px";
		}
	}
</script>
<body>
	<form id="MvcContainerform" runat="server">
		<div style="overflow-y: hidden; overflow-x:hidden">
			<asp:PlaceHolder ID="content" runat="server"></asp:PlaceHolder>
		</div>
	</form>
</body>
</html>
