<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentTrailerPage" Src="EquipmentTrailerPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentTractorPage" Src="EquipmentTractorPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentTankerPage" Src="EquipmentTankerPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentTestsAndInspectionsPage" Src="EquipmentTestsAndInspectionsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentTagsAndLicensesPage" Src="EquipmentTagsAndLicensesPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentRailcarPage" Src="EquipmentRailcarPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentGeneralPage" Src="EquipmentGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentAirplaneGeneralPage" Src="EquipmentAirplaneGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentAdditionalDataPage" Src="EquipmentAdditionalDataPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentQCStatusPage" Src="EquipmentQCStatusPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentUserDataPage" Src="EquipmentUserDataPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentBargePage" Src="EquipmentBargePage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentShipPage" Src="EquipmentShipPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentPipelinePage" Src="EquipmentPipelinePage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentCompartmentsPage" Src="EquipmentCompartmentsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentMeterPage" Src="EquipmentMeterPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentHistoryTab" Src="EquipmentHistoryTab.ascx" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>

<%@ Page Language="c#" CodeBehind="EquipmentForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EquipmentForm" %>

<!DOCTYPE html >
<HTML>
	<HEAD runat="server">
		<title></title>
		<base target="_self">
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
        <style>
             /* This style is here to resolve a problem with the tab page headers (e.g. "General", "Compartments") stacking on top of each other 
                 rather than appearing in a row when the QC Status tab was hidden */
             .ajax__tab_outer 
             {
                 display:inline-block !important;
             }
        </style>
	</HEAD>
	<body ms_positioning="GridLayout" tabindex="-1">
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>">
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
        <script type="text/javascript">
            function SetHelpKey(sender, e) {
                CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
            }
        </script>
		<form id="EquipForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
    	        <asp:ScriptManager ID="ScriptManager1" runat="server" />
			    <FMControls:FMLabel id="EquipmentTitleLabel" 
                style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				    BackColor="Transparent" CssClass="headline">Equipment Configuration</FMControls:FMLabel>
			    <asp:image id="Image1" style="Z-INDEX: 99; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				    ImageUrl="<%$ AppSettings: PageFadeImage %>" CssClass="formfieldtitle"></asp:image>
                <FMControls:FMButton ID="EquipNo" Style="z-index: 98; top: -1000px; position: absolute" TabIndex="100"
                    runat="server" CssClass="formfieldtitle" Width="67px" Text="EquipNo"></FMControls:FMButton>
                <FMControls:FMButton ID="EquipYes" Style="z-index: 100; top: -1000px; position: absolute" TabIndex="100"
                    runat="server" CssClass="formfieldtitle" Width="67px" Text="EquipYes"></FMControls:FMButton>
                <FMControls:FMTabContainer ID="tcEquipment" runat="server" Style="z-index: 103; left: 32px; position: absolute; top: 40px; width: 725px; height: 425px">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" HelpKey="FMWebApp/EquipmentGeneralPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentGeneralPage runat="server" ID="EquipmentGeneralPage"></FMWebApp:EquipmentGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpAirplaneGeneralPage" HelpKey="FMWebApp/EquipmentAirplaneGeneralPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentAirplaneGeneralPage runat="server" ID="EquipmentAirplaneGeneralPage"></FMWebApp:EquipmentAirplaneGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="TrailerPage" ID="tpTrailerPage" HelpKey="FMWebApp/EquipmentTrailerPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentTrailerPage runat="server" ID="EquipmentTrailerPage"></FMWebApp:EquipmentTrailerPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="TractorPage" ID="tpTractorPage" HelpKey="FMWebApp/EquipmentTractorPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentTractorPage runat="server" ID="EquipmentTractorPage"></FMWebApp:EquipmentTractorPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="TankerPage" ID="tpTankerPage" HelpKey="FMWebApp/EquipmentTankerPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentTankerPage runat="server" ID="EquipmentTankerPage"></FMWebApp:EquipmentTankerPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="QCStatusPage" ID="tpQCStatusPage" HelpKey="FMWebApp/EquipmentQCStatusPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentQCStatusPage runat="server" ID="EquipmentQCStatusPage"></FMWebApp:EquipmentQCStatusPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Compartments" ID="tpCompartmentsPage" HelpKey="FMWebApp/EquipmentCompartmentsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentCompartmentsPage runat="server" ID="EquipmentCompartmentsPage"></FMWebApp:EquipmentCompartmentsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Tests & Inspections" ID="tpTestsAndInspectionsPage" HelpKey="FMWebApp/EquipmentTestsAndInspectionsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentTestsAndInspectionsPage runat="server" ID="EquipmentTestsAndInspectionsPage"></FMWebApp:EquipmentTestsAndInspectionsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Tags & Licenses" ID="tpTagsAndLicensesPage" HelpKey="FMWebApp/EquipmentTagsAndLicensesPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentTagsAndLicensesPage runat="server" ID="EquipmentTagsAndLicensesPage"></FMWebApp:EquipmentTagsAndLicensesPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Meter" ID="tpMeterPage" HelpKey="FMWebApp/EquipmentMeterPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentMeterPage runat="server" ID="EquipmentMeterPage"></FMWebApp:EquipmentMeterPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Additional Data" ID="tpAdditionalDataPage" HelpKey="FMWebApp/EquipmentAdditionalDataPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentAdditionalDataPage runat="server" ID="EquipmentAdditionalDataPage"></FMWebApp:EquipmentAdditionalDataPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="User Data" ID="tpUserDataPage" HelpKey="FMWebApp/EquipmentUserDataPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentUserDataPage runat="server" ID="EquipmentUserDataPage"></FMWebApp:EquipmentUserDataPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="History" ID="HistoryTabPanel" HelpKey="FMWebApp/EquipmentHistoryTab.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentHistoryTab runat="server" ID="EquipmentHistoryTab"></FMWebApp:EquipmentHistoryTab>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="RailcarPage" ID="tpRailcarPage" HelpKey="FMWebApp/EquipmentRailcarPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentRailcarPage runat="server" ID="EquipmentRailcarPage"></FMWebApp:EquipmentRailcarPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="BargePage" ID="tpBargePage" HelpKey="FMWebApp/EquipmentBargePage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentBargePage runat="server" ID="EquipmentBargePage"></FMWebApp:EquipmentBargePage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="ShipPage" ID="tpShipPage" HelpKey="FMWebApp/EquipmentShipPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentShipPage runat="server" ID="EquipmentShipPage"></FMWebApp:EquipmentShipPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="PipelinePage" ID="tpPipelinePage" HelpKey="FMWebApp/EquipmentPipelinePage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:EquipmentPipelinePage runat="server" ID="EquipmentPipelinePage"></FMWebApp:EquipmentPipelinePage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </FMControls:FMTabContainer>
                <table style="z-index: 104; left: 32px; position: absolute; top: 595px; width: 700px">
                    <tr>
                        <td style="float: right">
                            <table>
                                <tr>
                                    <td>
                                        <FMControls:FMLabel ID="Label10" runat="server" Width="176px" CssClass="formfieldtitle" Height="8px" ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel>
                                    </td>
                                    <td>&nbsp;&nbsp;</td>
                                    <td>
                                        <FMControls:FMButton ID="New" TabIndex="100" runat="server" Width="66px" CssClass="formfieldtitle" Text="New" Visible="False"></FMControls:FMButton>

                                    </td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <FMControls:FMButton ID="OK" TabIndex="101"
                                            runat="server" Width="66px" CssClass="formfieldtitle" Text="OK" OnClientClick="this.disabled=true;" UseSubmitBehavior="false"></FMControls:FMButton>
                                    </td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <FMControls:FMButton ID="Cancel" TabIndex="102"
                                            runat="server" Width="66px" CssClass="formfieldtitle" Text="Cancel" CommandName="Cancel"></FMControls:FMButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>

		        <script type="text/javascript">
		            var okButton = document.getElementById("OK");
		            if (!okButton.disabled)
		                okButton.focus();
			    </script>
            </div>
		</form>
	</body>
</HTML>
