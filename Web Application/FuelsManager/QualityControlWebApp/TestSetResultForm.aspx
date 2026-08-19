<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestSetResultForm.aspx.cs" Inherits="FuelsManager.QualityControlWebApp.TestSetResultForm" %>
<%@ Register TagPrefix="QualityControlWebApp" TagName="TestSetResultGeneralPage" Src="TestSetResultGeneralPage.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls"%>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<!DOCTYPE html>
<html>
	<head runat="server">
			<title></title>
			<base target="_self">
			<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
			<meta name="CODE_LANGUAGE" Content="C#">
			<meta name="vs_defaultClientScript" content="JavaScript">
			<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
			<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</head>
    <body>
	    <form id="TestSetResultForm" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
                <asp:ScriptManager ID="ScriptManager" runat="server"/>
 			    <asp:image id="FadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 99; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				    ImageUrl="<%$ AppSettings: PageFadeImage %>" CssClass="formfieldtitle"></asp:image>
			    <FMControls:FMLabel id="MainLabel" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				    BackColor="Transparent" Width="296px" CssClass="headline">Test Set Result</FMControls:FMLabel>
                <div style="position:absolute; TOP: 40px; LEFT: 32px;">
        					<QualityControlWebApp:TestSetResultGeneralPage runat="server" ID="TestSetResultGeneralPage"></QualityControlWebApp:TestSetResultGeneralPage>
                </div>
                 <table style="z-index: 200; left: 32px; position: absolute; top: 725px; width: 900px">
                    <tr>
                        <td>
                            <asp:Label ID="DenotesLabel" Style="z-index: 105; left: 16px;" runat="server" CssClass="formfieldtitle"
                                BackColor="Transparent" ForeColor="Red">
	    	                    * Denotes Required Field</asp:Label>
                        </td>
                        <td style="float: right">
                            <table>
                                <tr>
                                    <td>
                                        <FMControls:FMButton ID="OK" TabIndex="101" runat="server" Width="66px" CssClass="formfieldtitle"
                                            Text="OK" OnCommand="OkCommand"></FMControls:FMButton>
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td>
                                        <FMControls:FMButton ID="Cancel" TabIndex="102" runat="server"  Width="66px" CssClass="formfieldtitle"
                                            Text="Cancel" CommandName="Cancel" OnCommand="CancelCommand"></FMControls:FMButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <script>
                    var okButton = document.getElementById("OK");
                    if (!okButton.disabled)
                        okButton.setActive();
                </script>
            </div>
		</form>
	</body>
</html>
