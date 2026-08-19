<%@ Page language="c#" Codebehind="AccessibilityForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.AccessibilityForm"  %>
<%@ Register TagPrefix="FMWebApp" TagName="AccessibilityGeneralPage" Src="AccessibilityGeneralPage.ascx" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<!DOCTYPE html>
<HTML>
	<head runat="server">
		<title></title>
		<base target="_self">
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">

	</head>
	<body tabindex="-1">
  		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />


        <form id="AccessibilityForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:ScriptManager ID="ScriptManager" runat="server" />
                <FMControls:FMLabel ID="AccessibilityTitleLabel" Style="z-index: 102; left: 8px; position: absolute; top: 8px; width: 600px;" runat="server"
                    BackColor="Transparent" CssClass="headline">Accessibility Configuration</FMControls:FMLabel>
                <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
                <FMControls:FMTabContainer ID="tcAccessibilityTabs" runat="server" ActiveTabIndex="0"
                    Style="z-index: 104; position: absolute; top: 35px; left: 12px; width: 780px; height: 440px">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" aria-label="Accessibility Tabs">
                        <ContentTemplate>
                            <FMWebApp:AccessibilityGeneralPage runat="server" ID="AccessibilityGeneralPageTab"></FMWebApp:AccessibilityGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>

                </FMControls:FMTabContainer>
                <div style="z-index: 200; left: 32px; position: absolute; top: 590px; display: table; width: 700px">
                    <div style="display: table-row">
                        <div style="display: table-cell;">
                            <div style="display: table">
                                <div style="display: table-row">
                                    <div style="display: table-cell;">&nbsp; </div>
                                    <div style="display: table-cell;">&nbsp;&nbsp;</div>
                                    <div style="display: table-cell;">&nbsp;</div>
                                    <div style="display: table-cell;">&nbsp;</div>
                                    <div style="display: table-cell;">
                                        <FMControls:FMButton ID="OK" TabIndex="101"
                                            runat="server" Width="66px" CssClass="formfieldtitle" Text="OK"></FMControls:FMButton>
                                    </div>
                                    <div style="display: table-cell;">&nbsp;</div>
                                    <div style="display: table-cell;">
                                        <FMControls:FMButton ID="Cancel" TabIndex="102"
                                            runat="server" Width="66px" CssClass="formfieldtitle" Text="Cancel" CommandName="Cancel"></FMControls:FMButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </form>
	<script type="text/javascript">
	    var okButton = document.getElementById("OK");
	    if(!okButton.disabled)
	        okButton.setActive();
	</script>
    </body>
</HTML>
