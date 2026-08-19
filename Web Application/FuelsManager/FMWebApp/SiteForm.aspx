<%@ Page language="c#" Codebehind="SiteForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SiteForm" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteGeneralPage" Src="SiteGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteContactsPage" Src="SiteContactsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteUnitsPage" Src="SiteUnitsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteLeakDetectionPage" Src="SiteLeakDetectionPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteLoadRackPage" Src="SiteLoadRackPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteReportsPage" Src="SiteReportsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteTransactionPage" Src="SiteTransactionPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteOperatingSchedulePage" Src="SiteOperatingSchedulePage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteSystemPage" Src="SiteSystemPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteVaporRecoveryPage" Src="SiteVaporRecoveryPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteProcessVariablesPage" Src="SiteProcessVariablesPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteUserDataPage" Src="SiteUserDataPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteGroupPage" Src="SiteGroupPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteNotesPage" Src="SiteNotesPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteCertificatePage" Src="SiteCertificatePage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="SiteOpcUaPage" Src="SiteOpcUaPage.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly = "FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<!DOCTYPE html>
<html>
	<head runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</head>
	<body  tabIndex="-1" MS_POSITIONING="GridLayout">
        <link href="<%=HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        <script type="text/javascript">
		    function SetHelpKey(sender, e) {
		    	CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
		    }
		</script>
        <form id="SiteForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
                <asp:ScriptManager ID="ScriptManager" runat="server"/>
			   <FMControls:FMButton ID="No" Style="top: -1000px; position: absolute" TabIndex="100"
				   runat="server" CssClass="formfieldtitle" Width="67px" Text="No"></FMControls:FMButton>
			   <FMControls:FMButton ID="Yes" Style="top: -1000px; position: absolute" TabIndex="100"
				   runat="server" CssClass="formfieldtitle" Width="67px" Text="Yes"></FMControls:FMButton>
			   <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			   <FMControls:FMLabel ID="labSiteConfig" Style="left: 8px; position: absolute; top: 8px" runat="server"
				   CssClass="headline" Width="720px" BackColor="Transparent">Site Configuration</FMControls:FMLabel>
 			         <FMControls:FMTabContainer ID="tcSiteTabs" runat="server" ActiveTabIndex="0" TabWidth="60px"
				         Style="position: absolute; top: 40px; left: 32px; width: 700px; height: 555px"
				         aria-label="Site Tabs">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" HelpKey="FMWebApp/SiteGeneralPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteGeneralPage runat="server" ID="SiteGeneralPage"></FMWebApp:SiteGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Contacts" ID="tpContactsPage" HelpKey="FMWebApp/SiteContactsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteContactsPage runat="server" ID="SiteContactsPage"></FMWebApp:SiteContactsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Units" ID="tpUnitsPage" HelpKey="FMWebApp/SiteUnitsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteUnitsPage runat="server" ID="SiteUnitsPage"></FMWebApp:SiteUnitsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Load Rack" ID="tpLoadRackPage" HelpKey="FMWebApp/SiteLoadRackPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteLoadRackPage runat="server" ID="SiteLoadRackPage"></FMWebApp:SiteLoadRackPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Transaction Tickets" ID="tpTransactionPage" HelpKey="FMWebApp/SiteTransactionPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteTransactionPage runat="server" ID="SiteTransactionPage"></FMWebApp:SiteTransactionPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Operating Schedule" ID="tpOperatingSchedulePage" HelpKey="FMWebApp/SiteOperatingSchedulePage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteOperatingSchedulePage runat="server" ID="SiteOperatingSchedulePage"
                                NAME="Siteoperatingschedulepage1"></FMWebApp:SiteOperatingSchedulePage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>                 
                    <ajaxToolkit:TabPanel runat="server" HeaderText="System" ID="tpSystemPage" HelpKey="FMWebApp/SiteSystemPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteSystemPage runat="server" ID="SiteSystemPage"></FMWebApp:SiteSystemPage >
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Vapor Recovery" ID="tpVaporRecoveryPage" HelpKey="FMWebApp/SiteVaporRecoveryPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteVaporRecoveryPage runat="server" ID="SiteVaporRecoveryPage"></FMWebApp:SiteVaporRecoveryPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Process Variables" ID="tpProcessVariablesPage" HelpKey="FMWebApp/SiteProcessVariablesPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteProcessVariablesPage runat="server" ID="SiteProcessVariablesPage"></FMWebApp:SiteProcessVariablesPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="User Data" ID="tpUserDataPage" HelpKey="FMWebApp/SiteUserDataPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteUserDataPage runat="server" ID="SiteUserDataPage"></FMWebApp:SiteUserDataPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Sites" ID="tpGroupPage" HelpKey="FMWebApp/SiteGroupPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteGroupPage runat="server" ID="SiteGroupPage"></FMWebApp:SiteGroupPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Notes" ID="tpNotesPage" HelpKey="FMWebApp/SiteNotesPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteNotesPage runat="server" ID="SiteNotesPage"></FMWebApp:SiteNotesPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Certificates" ID="tpCertificatePage" HelpKey="FMWebApp/SiteCertificatePage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteCertificatePage runat="server" ID="SiteCertificatePage"></FMWebApp:SiteCertificatePage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Opc Ua" ID="tpOpcUaPage" HelpKey="FMWebApp/SiteOpcUaPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteOpcUaPage runat="server" ID="SiteOpcUaPage"></FMWebApp:SiteOpcUaPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Leak Detection" ID="tpSiteLeakDetectionPage" HelpKey="FMWebApp/SiteLeakDetectionPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteLeakDetectionPage runat="server" ID="SiteLeakDetectionPage"></FMWebApp:SiteLeakDetectionPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Reports" ID="tpSiteReportsPage" HelpKey="FMWebApp/SiteReportsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:SiteReportsPage runat="server" ID="SiteReportsPage"></FMWebApp:SiteReportsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </FMControls:FMTabContainer>

                <table style="LEFT: 32px; POSITION: absolute; TOP: 640px; width:700px">
			        <tr><td style="float:right">
			            <table><tr><td>
			                <FMCONTROLS:FMLABEL id="Label10" runat="server"
				                CssClass="formfieldtitle" Height="8px" ForeColor="Crimson" Width="176px">* Denotes Required Field</FMCONTROLS:FMLABEL></td>
                            <td>&nbsp;&nbsp;</td>
                            <td><FMCONTROLS:FMBUTTON id="OK" tabIndex="100"
				                runat="server" CssClass="formfieldtitle" Style="min-width:75px;" Width="75px" Text="OK" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"></FMCONTROLS:FMBUTTON></td>
                            <td>&nbsp;</td>
                            <td><FMCONTROLS:FMBUTTON id="Cancel" Style="min-width:75px;" Width="75px" tabIndex="101"
				                runat="server" CssClass="formfieldtitle" Text="Cancel"></FMCONTROLS:FMBUTTON></td></tr>
                        </table>
				    </td></tr>
				</table>
			    <script type="text/javascript">
			        var okButton = document.getElementById("OK");
			        if (!okButton.disabled)
			            okButton.focus();
			    </script>
            </div>
		</form>
	</body>
</html>
