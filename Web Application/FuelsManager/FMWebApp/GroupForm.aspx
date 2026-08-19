<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly = "FMControls" %>
<%@ Register TagPrefix="FMWebApp" TagName="GroupCompaniesPage" Src="GroupCompaniesPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="GroupRightsPage" Src="GroupRightsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="GroupGeneralPage" Src="GroupGeneralPage.ascx" %>
<%@ Page language="c#" Codebehind="GroupForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.GroupForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<!DOCTYPE html>
<HTML>
  <HEAD runat="server">
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
	<body leftMargin="20" rightMargin="20" ms_positioning="GridLayout" tabindex="-1">
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
		<form id="GroupForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
                <asp:ScriptManager ID="ScriptManager" runat="server"/>
			  <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			    <FMControls:FMLabel id="UserGroupTitleLabel" 
                    style="Z-INDEX: 125; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				    CssClass="headline" Width="500px" BackColor="Transparent">User Group Configuration</FMControls:FMLabel>
			    
                <FMControls:FMTabContainer ID="tcGroupTabs" runat="server" ActiveTabIndex="0" Style="z-index: 104;
                    position: absolute; top: 40px; left: 32px; width: 700px; height: 455px"
				 aria-label="Group Tabs">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" HelpKey="FMWebApp/GroupGeneralPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
        					<FMWebApp:GroupGeneralPage runat="server" ID="GroupGeneralPage" NAME="GroupGeneralPage"></FMWebApp:GroupGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Security Rights" ID="tpRightsPage" HelpKey="FMWebApp/GroupRightsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
        					<FMWebApp:GroupRightsPage runat="server" ID="GroupRightsPage" NAME="GroupRightsPage"></FMWebApp:GroupRightsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Companies" ID="tpCompaniesPage" HelpKey="FMWebApp/GroupCompaniesPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
        					<FMWebApp:GroupCompaniesPage runat="server" ID="GroupCompaniesPage" NAME="GroupCompaniesPage"></FMWebApp:GroupCompaniesPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </FMControls:FMTabContainer>
        
                <table style="Z-INDEX: 104; LEFT: 32px; POSITION: absolute; TOP: 495px; width:700px">
        			<tr>
		            	<td style="float:right">
			                <table role="presentation" aria-label="layout">
			                   <tr>
			                      <td><FMControls:FMLabel id="Label1" 
                                  runat="server" ForeColor="Crimson" Height="8px" CssClass="formfieldtitle">* Denotes Required Field</FMControls:FMLabel></td>
			                      <td>&nbsp;&nbsp;</td>
			                      <td><FMControls:FMButton id="OK"  tabIndex="8"
				                      runat="server" Width="100px" Text="OK" CssClass="formfieldtitle"></FMControls:FMButton></td>
			                      <td>&nbsp;</td>
			                      <td><FMControls:FMButton id="Cancel"  tabIndex="9"
				                      runat="server" Width="100px" Text="Cancel" CssClass="formfieldtitle"></FMControls:FMButton></td>
			                   </tr>
			                </table>
            			</td>
			        </tr>
			    </table>			
            </div>
		</form>
	</body>
</HTML>
