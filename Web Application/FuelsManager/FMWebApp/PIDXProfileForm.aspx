<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly = "FMControls" %>
<%@ Page language="c#" Codebehind="PIDXProfileForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.PIDXProfileForm" %>
<%@ Register TagPrefix="FMWebApp" TagName="PIDXProfileGeneralPage" Src="PIDXProfileGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="PIDXProfileCompaniesPage" Src="PIDXProfileCompaniesPage.ascx" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<!DOCTYPE html>
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
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
		<form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
               <asp:ScriptManager ID="ScriptManager1" runat="server" />
					<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			    <FMCONTROLS:FMLABEL id="Label9" style="Z-INDEX: 125; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				    BackColor="Transparent" Width="360px" CssClass="headline">Data Exchange Profile Configuration</FMCONTROLS:FMLABEL>
				<FMControls:FMTabContainer ID="tcPIDXProfileTabs" runat="server" ActiveTabIndex="0" Style="z-index: 104; position: absolute; top: 40px; left: 32px; width: 725px; height: 455px"
					aria-label="PIDX Profiles Tab">
             <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" HelpKey="FMWebApp/PIDXProfileGeneralPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
    					    <FMWebApp:PIDXProfileGeneralPage runat="server" ID="PIDXProfileGeneralPage" NAME="PIDXProfileGeneralPage"></FMWebApp:PIDXProfileGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Companies" ID="tpCompaniesPage" HelpKey="FMWebApp/PIDXProfileCompaniesPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
    					    <FMWebApp:PIDXProfileCompaniesPage runat="server" ID="PIDXProfileCompaniesPage" NAME="PIDXProfileCompaniesPage"></FMWebApp:PIDXProfileCompaniesPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </FMControls:FMTabContainer>
					<table style="z-index: 104; left: 432px; position: absolute; top: 495px; width: 350px" role="presentation" aria-label="layout">
						<tr>
							<td style="float: right">
							  <table>
								  <tr>
										<td>
											<FMControls:FMButton ID="OK" TabIndex="8"
												runat="server" Width="66px" CssClass="formfieldtitle" Text="OK"></FMControls:FMButton></td>
										<td>&nbsp;</td>
										<td>
											<FMControls:FMButton ID="Cancel" TabIndex="9"
												runat="server" Width="66px" CssClass="formfieldtitle" Text="Cancel"></FMControls:FMButton></td>
									</tr>
							  </table>
						  </td>
						</tr>
                </table>
            </div>
		</form>
	</body>
</HTML>
