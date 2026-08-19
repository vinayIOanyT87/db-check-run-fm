<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FMMenuBar.aspx.cs" Inherits="FuelsManager.MenuBar.FMMenuBar" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Scripts/jquery-2.2.1.min.js" %>"></script>
    
	<style>    
		 .mainLoadingDiv {
			  position: absolute;
			  padding:0;
			  margin:0;
			  top:0;
			  left:0;
			  width: 100%;
			  height: 100%;
			  background:rgba(255,255,255,0.3);
			  z-index: 10000;
		 }

		 .mainLoadingDiv img {
			  position: absolute;
			  top: 50%;
			  left: 50%;
			  margin: -60px 0px 0px -60px;
			  z-index: 10000;
		 }

	</style>

    <script>
    	var FMMenuBar = {};
    	FMMenuBar.SetIFrameSize = function () {
    		$('#iframeContent').width('100%');

            var displayCUI = true;
            var cuiHeight = 0;
            if (displayCUI) {
                cuiHeight = 28;
            }
    		// detect if the iframe is in full screen
    		var fullscreenElement = document.fullscreenElement || document.mozFullScreenElement || document.webkitFullscreenElement || document.msFullscreenElement;
    		if (fullscreenElement != null) {
    			$('#iframeContent').height('100%');
    		}
    		else {
                $('#iframeContent').height($(window).height() - $("#bdyMenuBarBody").height() - 6 - cuiHeight);
    		}
        };

    	$(window).resize(function () {
    		FMMenuBar.SetIFrameSize();
    	});

    	$(document).ready(function () {
            $('#iframeContent').load(function () {
                FMMenuBar.SetIFrameSize();
                 $(".mainLoadingDiv").fadeOut("slow", FMMenuBar.SetIFrameSize);

    		});
    	});
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <asp:PlaceHolder ID="content" runat="server"></asp:PlaceHolder>
        <div class="mainLoadingDiv"><img src="../fmwebapp/images/loader_squares_120.gif" /></div>
    </form>
</body>
</html>
