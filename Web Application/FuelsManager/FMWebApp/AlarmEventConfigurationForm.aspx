<%@ Page language="c#" Codebehind="AlarmEventConfigurationForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.AlarmEventConfigurationForm" %>
<%@ Register TagPrefix="FMWebApp" TagName="AlarmEventCategoriesPage" Src="AlarmEventCategoriesPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="AlarmEventPrioritiesPage" Src="AlarmEventPrioritiesPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="AlarmEventEmailGroupsPage" Src="AlarmEventEmailGroupsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="AlarmEventAssignmentPage" Src="AlarmEventAssignmentPage.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly = "FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<!DOCTYPE html >
<HTML>
  <HEAD runat="server">
    <title></title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body tabindex=-1>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
  <script type="text/javascript">
	function SetHelpKey(sender, e) {
		CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
	}
</script>

	<asp:ScriptManager ID="ScriptManager" runat="server"/>
    <form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
    		<asp:image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 99; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
	    	<FMControls:FMLabel id="Label9" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"	Width="336px" BackColor="Transparent" CssClass="headline">Alarm & Event Configuration</FMControls:FMLabel>
			<FMControls:FMTabContainer id="tcAlarmEventConfig" runat="server" style="z-index:104;position:absolute;top:40px;left:32px;width:775px;height:425px">
                <ajaxToolkit:TabPanel runat="server" HeaderText="Categories" ID="tpCategories" HelpKey="FMWebApp/AlarmEventCategoriesPage.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                        <FMWebApp:AlarmEventCategoriesPage id="AlarmEventCategoriesPage" runat="server"></FMWebApp:AlarmEventCategoriesPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Priorities" ID="tpPriorities" HelpKey="FMWebApp/AlarmEventPrioritiesPage.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                    <FMWebApp:AlarmEventPrioritiesPage id="AlarmEventPrioritiesPage" runat="server"></FMWebApp:AlarmEventPrioritiesPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="E-mail Groups" ID="tpEmailGroups" HelpKey="FMWebApp/AlarmEventEmailGroupsPage.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                    <FMWebApp:AlarmEventEmailGroupsPage id="AlarmEventEmailGroupsPage" runat="server"></FMWebApp:AlarmEventEmailGroupsPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Assignments" ID="tpAssignments" HelpKey="FMWebApp/AlarmEventAssignmentPage.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                    <FMWebApp:AlarmEventAssignmentPage id="AlarmEventAssignmentPage" runat="server"></FMWebApp:AlarmEventAssignmentPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
			</FMControls:FMTabContainer>
        </div>
    </form>
  </body>
</HTML>
