<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportConfigurationSettingsPage.aspx.cs" Inherits="FuelsManager.FMReportWebMain.ReportConfigurationSettingsPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMReportWebMain" TagName="ReportConfigurationAssignmentReportsPage" Src="ReportConfigurationAssignmentReportsPage.ascx" %>
<%@ Register TagPrefix="FMReportWebMain" TagName="ReportConfigurationAssignmentDirectoriesPage" Src="ReportConfigurationAssignmentDirectoriesPage.ascx" %>
<%@ Register src="../FMWebApp/..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
	<head id="Head1" runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</head>
<body>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<script type="text/javascript">
	function SetHelpKey(sender, e) {
		CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
	}
</script>

    <form id="ReportConfigurationSettingsPage" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
            <asp:ScriptManager ID="ScriptManager" runat="server"/>
		<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
            <FMCONTROLS:FMLabel id="ReportLabel" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 16px"
				runat="server" CssClass="headline" Width="453px">Report Assignment Configuration</FMCONTROLS:FMLabel>
            <FMControls:FMTabContainer ID="tcReportsTabs" runat="server" ActiveTabIndex="0" TabWidth="90px"
                        Style="z-index: 104; position: absolute; top: 70px; left: 32px; width: 700px;
                        height: 100%" aria-label="Report Assigment Tab">
                        <ajaxToolkit:TabPanel runat="server" HeaderText="Reports" ID="tpReportsPage" HelpKey="FMReportWebMain/ReportConfigurationAssignmentReportsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMReportWebMain:ReportConfigurationAssignmentReportsPage runat="server" ID="ReportConfigurationAssignmentReportsPage"></FMReportWebMain:ReportConfigurationAssignmentReportsPage>
                        
</ContentTemplate>
                    
</ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Directories" ID="tpDirectoriesPage" HelpKey="FMReportWebMain/ReportConfigurationAssignmentDirectoriesPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
<FMReportWebMain:ReportConfigurationAssignmentDirectoriesPage runat="server" ID="ReportConfigurationAssignmentDirectoriesPage">
</FMReportWebMain:ReportConfigurationAssignmentDirectoriesPage>
</ContentTemplate>
                    
</ajaxToolkit:TabPanel>
            </FMControls:FMTabContainer>
			</div>
    </form>
</body>
</html>
