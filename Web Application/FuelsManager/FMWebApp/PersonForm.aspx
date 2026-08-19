<%@ Register TagPrefix="FMWebApp" TagName="PersonLicensesPage" Src="PersonLicensesPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="PersonTrainingPage" Src="PersonTrainingPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="PersonQualificationsPage" Src="PersonQualificationsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="PersonDriverPage" Src="PersonDriverPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="PersonGeneralPage" Src="PersonGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="PersonAdditionalDataPage" Src="PersonAdditionalDataPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="PersonUserDataPage" Src="PersonUserDataPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="PersonLoadRackPage" Src="PersonLoadRackPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="PersonAccessSchedulePage" Src="PersonAccessSchedulePage.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Page Language="c#" CodeBehind="PersonForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.PersonForm" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html>
<HTML>
	<HEAD runat="server">
		<title></title>
		<base target="_self">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
	<script type="text/javascript">
		    function SetHelpKey(sender, e) {
		    	CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
		    }
</script>
		<form id="PersonForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
                <asp:ScriptManager ID="ScriptManager" runat="server"/>
			    <FMControls:FMLabel id="PersonnelTitleLabel" 
                    style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				    CssClass="headline" Width="500px" BackColor="Transparent">Personnel 
                    Configuration</FMControls:FMLabel>
 			    <FMCONTROLS:FMBUTTON id="PersonNo" style="Z-INDEX: 98; top: -1000px; position: absolute" tabIndex="100" 
				    runat="server" CssClass="formfieldtitle" Width="67px" Text="EquipNo"></FMCONTROLS:FMBUTTON>
			    <FMCONTROLS:FMBUTTON id="PersonYes" style="Z-INDEX: 100; top: -1000px; position: absolute" tabIndex="100" 
				    runat="server" CssClass="formfieldtitle" Width="67px" Text="EquipYes"></FMCONTROLS:FMBUTTON>
               <FMControls:FMTabContainer ID="tcPersonTabs" runat="server" ActiveTabIndex="0"
                    Style="z-index: 104; position: absolute; top: 40px; left: 32px; width: 900px; height: 500px">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" HelpKey="FMWebApp/PersonGeneralPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
					        <FMWebApp:PersonGeneralPage runat="server" ID="PersonGeneralPage"></FMWebApp:PersonGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
				    <ajaxToolkit:TabPanel runat="server" HeaderText="Driver" ID="tpDriverPage" HelpKey="FMWebApp/PersonDriverPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
					        <FMWebApp:PersonDriverPage runat="server" ID="PersonDriverPage"></FMWebApp:PersonDriverPage>
				        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Load Rack" ID="tpLoadRackPage" HelpKey="FMWebApp/PersonLoadRackPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
					        <FMWebApp:PersonLoadRackPage runat="server" ID="PersonLoadRackPage"></FMWebApp:PersonLoadRackPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Access Schedule" ID="tpAccessSchedulePage" HelpKey="FMWebApp/PersonAccessSchedulePage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
					        <FMWebApp:PersonAccessSchedulePage runat="server" ID="PersonAccessSchedulePage"></FMWebApp:PersonAccessSchedulePage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Qualifications" ID="tpQualificationsPage" HelpKey="FMWebApp/PersonQualificationsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
					        <FMWebApp:PersonQualificationsPage runat="server" ID="PersonQualificationsPage"></FMWebApp:PersonQualificationsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Training" ID="tpTrainingPage" HelpKey="FMWebApp/PersonTrainingPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
					        <FMWebApp:PersonTrainingPage runat="server" ID="PersonTrainingPage"></FMWebApp:PersonTrainingPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Licenses" ID="tpLicensesPage" HelpKey="FMWebApp/PersonLicensesPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
					        <FMWebApp:PersonLicensesPage runat="server" ID="PersonLicensesPage"></FMWebApp:PersonLicensesPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Additional Data" ID="tpAdditionalDataPage" HelpKey="FMWebApp/PersonAdditionalDataPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
					        <FMWebApp:PersonAdditionalDataPage runat="server" ID="PersonAdditionalDataPage"></FMWebApp:PersonAdditionalDataPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="User Data" ID="tpUserDataPage" HelpKey="FMWebApp/PersonUserDataPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
					        <FMWebApp:PersonUserDataPage runat="server" ID="PersonUserDataPage"></FMWebApp:PersonUserDataPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </FMControls:FMTabContainer>
                <table style="Z-INDEX: 104; LEFT: 32px; POSITION: absolute; TOP: 614px; width: 900px">
		        <tr>
		        <td style="float:right">	
		        <table>
		           <tr>
		              <td><FMControls:FMLabel id="Label10"  runat="server"
				              CssClass="formfieldtitle" Width="176px" ForeColor="Crimson" Height="8px">* Denotes Required Field</FMControls:FMLabel></td>
		              <td>&nbsp;&nbsp;</td>
		              <td><FMControls:FMButton id="CardInButton" Width="66px" tabIndex="99" 
		                 runat="server" CssClass="formfieldtitle" Text="Card In"></FMControls:FMButton></td>
		              <td>&nbsp;</td>
		              <td><FMControls:FMButton id="CardOutButton" Width="66px" tabIndex="100" 
		                 runat="server" CssClass="formfieldtitle" Text="Card Out"></FMControls:FMButton></td>
		              <td>&nbsp;</td>
		              <td><FMControls:FMButton id="New" Width="66px"  tabIndex="101"
				              runat="server" CssClass="formfieldtitle" Text="New" Visible="False"></FMControls:FMButton></td>
		              <td>&nbsp;</td>
		              <td><FMControls:FMButton id="OK" Width="66px"  tabIndex="102"
				              runat="server" CssClass="formfieldtitle" Text="OK"></FMControls:FMButton></td>
		              <td>&nbsp;</td>
		              <td><FMControls:FMButton id="Cancel" Width="66px"  tabIndex="103"
				              runat="server" CssClass="formfieldtitle" Text="Cancel" CommandName="Cancel"></FMControls:FMButton></td>
		           </tr>
		        </table>
		        </td>
		        </tr>
		        </table>
			    <script type="text/javascript">
			        var okButton = document.getElementById("OK");
			        if (!okButton.disabled)
			            okButton.setActive();
			    </script>
            </div>
		</form>
	</body>
</HTML>
