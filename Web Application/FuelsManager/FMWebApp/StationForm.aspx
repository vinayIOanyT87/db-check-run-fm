<%@ Register TagPrefix="FMWebApp" TagName="StationLoadArmsPage" Src="StationLoadArmsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationLoadRackPage" Src="StationLoadRackPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationGeneralPage" Src="StationGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationEntryGatePage" Src="StationEntryGatePage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationExitGatePage" Src="StationExitGatePage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationWeightScalePage" Src="StationWeightScalePage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationPreloadPage" Src="StationPreloadPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationBillOfLadingPage" Src="StationBillOfLadingPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationSignatureStationPage" Src="StationSignatureStationPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationMeterPage" Src="StationMeterPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationDeFuelPage" Src="StationDeFuelPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationRequiredTrainingPage" Src="StationRequiredTrainingPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationRequiredQualificationPage" Src="StationRequiredQualificationPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationRequiredLicensePage" Src="StationRequiredLicensePage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationRequiredTestsandInspectionsPage" Src="StationRequiredTestsandInspectionsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationDeFuelMeterPage" Src="StationDeFuelMeterPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationOffLoadingProductPage" Src="StationOffLoadingProductPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="StationRequiredEquipmentTagAndLicensePage" Src="StationRequiredEquipmentTagAndLicensePage.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="StationForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.StationForm" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

		<SCRIPT type="text/javascript">
		    function InputsButton_Click(Index) {
		        // used for offloading products via DET
				
				showModalDialogFrame({
					url: "../FMWebApp/OffLoadingExternalProductInputForm.aspx?Index=" + Index,
					width: 725,
					height: 530,
					title: "Products"
				});
		    }

			function PermissivesButton_Click(Mode,Index) {
				showModalDialogFrame({
					url: "../FMWebApp/PermissivesForm.aspx?Mode=" + Mode + "&Index=" + Index,
					width: 725,
					height: 530,
					title: "Permissives"
				});
			}

			function SetHelpKey(sender, e) {
			    CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
            }

            function OKButton_Click()
            {
                    //"This Station has Enable Dynamic Recipes configured. Any change made to 'Swing Arm' requires that the partner station has the same Enable Dynamic Recipes setting.\nClick OK to apply the change or Cancel to undo the Swing Arm change on this station.";
                if (confirm("This station has Swing Arms configured. Any change made to 'Enable Dynamic Recipes' will be applied to the partner station.\nClick OK to apply the change or Cancel to undo the change.") == true) {
                    $('#UndoDynamicRecipeChanges').val('false');
                }
                else {
                    $('#UndoDynamicRecipeChanges').val('true');
                }
                return true;
            }
        </SCRIPT>
	</HEAD>
	<body tabIndex="-1" MS_POSITIONING="GridLayout">
		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">        
		<link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
    
		<form id="Form1" method="post" runat="server">
            <asp:HiddenField ID="UndoDynamicRecipeChanges" ClientIDMode="Static" runat="server" Value="false" />
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
    	        <asp:ScriptManager ID="ScriptManager1" runat="server" />
			    <asp:label id="StationTitleLabel" 
                    style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				    CssClass="headline" Width="600px" BackColor="Transparent">Station Configuration</asp:label>
                <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
                <FMControls:FMTabContainer ID="tcStation" runat="server" Style="z-index:103;position: absolute;
                    top: 40px; left:32px; width: 725px; height: 455px" aria-label="Station Tabs">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage">
                        <ContentTemplate>
                            <FMWebApp:StationGeneralPage runat="server" ID="StationGeneralPage"></FMWebApp:StationGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="EntryGatePage" ID="tpEntryGatePage" HelpKey="FMWebApp/StationEntryGatePage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationEntryGatePage runat="server" ID="StationEntryGatePage"></FMWebApp:StationEntryGatePage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="LoadRackPage" ID="tpLoadRackPage" HelpKey="FMWebApp/StationLoadRackPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationLoadRackPage runat="server" ID="StationLoadRackPage"></FMWebApp:StationLoadRackPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="ExitGatePage" ID="tpExitGatePage" HelpKey="FMWebApp/StationExitGatePage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationExitGatePage runat="server" ID="StationExitGatePage"></FMWebApp:StationExitGatePage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="WeightScalePage" ID="tpWeightScalePage" HelpKey="FMWebApp/StationWeightScalePage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationWeightScalePage runat="server" ID="StationWeightScalePage"></FMWebApp:StationWeightScalePage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="PreloadPage" ID="tpPreloadPage" HelpKey="FMWebApp/StationPreloadPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationPreloadPage runat="server" ID="StationPreloadPage"></FMWebApp:StationPreloadPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="BillOfLadingPage" ID="tpBillOfLadingPage" HelpKey="FMWebApp/StationBillOfLadingPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationBillOfLadingPage runat="server" ID="StationBillOfLadingPage"></FMWebApp:StationBillOfLadingPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="DeFuelPage" ID="tpDeFuelPage" HelpKey="FMWebApp/StationDeFuelPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationDeFuelPage runat="server" ID="Stationdefuelpage"></FMWebApp:StationDeFuelPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="ReqStationDeFuelMeterPage" ID="tpReqStationDeFuelMeterPage" HelpKey="FMWebApp/StationDeFuelMeterPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationDeFuelMeterPage runat="server" ID="StationDeFuelMeterPage"></FMWebApp:StationDeFuelMeterPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="LoadArmsPage" ID="tpLoadArmsPage" HelpKey="FMWebApp/StationLoadArmsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationLoadArmsPage runat="server" ID="StationLoadArmsPage"></FMWebApp:StationLoadArmsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="SignatureStationPage" ID="tpSignatureStationPage" HelpKey="FMWebApp/StationSignatureStationPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationSignatureStationPage runat="server" ID="StationSignatureStationPage">
                            </FMWebApp:StationSignatureStationPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="MeterPage" ID="tpMeterPage" HelpKey="FMWebApp/StationMeterPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationMeterPage runat="server" ID="StationMeterPage"></FMWebApp:StationMeterPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="ReqOffLoadingProductPage" ID="tpReqOffLoadingProductPage" HelpKey="MWebApp/StationDeFuelPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
					        <FMWebApp:StationOffLoadingProductPage runat="server" ID="StationOffLoadingProductPage">
					        </FMWebApp:StationOffLoadingProductPage>
				        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="ReqQualificationsPage" ID="tpReqQualificationsPage" HelpKey="FMWebApp/StationRequiredQualificationPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationRequiredQualificationPage runat="server" ID="StationRequiredQualificationPage">
                            </FMWebApp:StationRequiredQualificationPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="ReqTrainingPage" ID="tpReqTrainingPage" HelpKey="FMWebApp/StationRequiredTrainingPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationRequiredTrainingPage runat="server" ID="StationRequiredTrainingPage">
                            </FMWebApp:StationRequiredTrainingPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="ReqLicensePage" ID="tpReqLicensePage" HelpKey="FMWebApp/StationRequiredLicensePage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationRequiredLicensePage runat="server" ID="StationRequiredLicensePage">
                            </FMWebApp:StationRequiredLicensePage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>    
                    <ajaxToolkit:TabPanel runat="server" HeaderText="ReqEquipmentLicensePage" ID="tpReqEquipmentLicensePage" HelpKey="FMWebApp/StationRequiredEquipmentTagAndLicensePage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationRequiredEquipmentTagAndLicensePage runat="server" ID="StationRequiredEquipmentTagAndLicensePage">
                            </FMWebApp:StationRequiredEquipmentTagAndLicensePage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>           
                    <ajaxToolkit:TabPanel runat="server" HeaderText="ReqTestsandInspectionsPage" ID="tpReqTestsandInspectionsPage" HelpKey="FMWebApp/StationRequiredTestsandInspectionsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:StationRequiredTestsandInspectionsPage runat="server" ID="StationRequiredTestsandInspectionsPage">
                            </FMWebApp:StationRequiredTestsandInspectionsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>                    
                </FMControls:FMTabContainer>
                <table style="z-index: 104; left: 32px; position: absolute; top: 495px" role="presentation" aria-label="layout">
                    <tr>
                        <td>
                            <table role="presentation" aria-label="layout">
                                <tr>
                                    <td align="left">
                                        <FMControls:FMButton ID="Apply" TabIndex="108" runat="server" CssClass="formfieldtitle"
                                            Text="Apply Deferred Changes" CommandName="Apply"></FMControls:FMButton>
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td align="right">
                                        <table role="presentation" aria-label="layout">
                                            <tr>
                                                <td>
                                                    <FMControls:FMLabel ID="Label10" runat="server" CssClass="formfieldtitle" Width="176px"
                                                        ForeColor="Crimson" Height="8px">* Denotes Required Field</FMControls:FMLabel>
                                                </td>
                                                <td>
                                                    &nbsp;&nbsp;
                                                </td>
                                                <td>
                                                    <FMControls:FMButton ID="New" TabIndex="109" runat="server" CssClass="formfieldtitle"
                                                        Width="66px" Text="New" ClientIDMode="Static"></FMControls:FMButton>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td>
                                                    <FMControls:FMButton ID="OK" TabIndex="110" runat="server" CssClass="formfieldtitle"
                                                        Width="66px" Text="OK" ClientIDMode="Static"></FMControls:FMButton>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td>
                                                    <FMControls:FMButton ID="Cancel" TabIndex="111" runat="server" CssClass="formfieldtitle"
                                                        Width="66px" Text="Cancel" CommandName="Cancel" ClientIDMode="Static"></FMControls:FMButton>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
		</form>
	</body>
</HTML>
