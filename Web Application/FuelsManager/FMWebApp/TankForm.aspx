<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="TankForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.TankForm" %>
<%@ Register TagPrefix="FMWebApp" TagName="TankGeneralPage" Src="TankGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="TankMeterAssignmentPage" Src="TankMeterAssignmentPage.ascx" %>
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
	<body runat="server" style="BACKGROUND-REPEAT: no-repeat; BACKGROUND-COLOR: white"
		tabIndex="-1" background="<%$ AppSettings: PageFadeImage %>" MS_POSITIONING="GridLayout">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        	<script type="text/javascript">
	function SetHelpKey(sender, e) {
		CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
	}
</script>
		<form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
                <asp:ScriptManager ID="ScriptManager1" runat="server" />
                <FMCONTROLS:FMLABEL id="TankTitleLabel" runat="server" BackColor="Transparent" 
                        CssClass="headline" Width="500px">Tank Configuration</FMCONTROLS:FMLABEL>
                <FMControls:FMTabContainer ID="tcTankTabs" runat="server" ActiveTabIndex="0"
                    Style="z-index: 104; position: absolute; top: 40px; left: 32px; width: 750px;
                    height: 630px" aria-label="Tank Tabs">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" HelpKey="FMWebApp/TankGeneralPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
		                    <FMWebApp:TankGeneralPage runat="server" ID="TankGeneralPage"></FMWebApp:TankGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Meters" ID="tpMetersPage" HelpKey="FMWebApp/TankMeterAssignmentPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
		                    <FMWebApp:TankMeterAssignmentPage runat="server" ID="TankMetersPage"></FMWebApp:TankMeterAssignmentPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </FMControls:FMTabContainer>

                <table style="Z-INDEX: 104; LEFT: 32px; POSITION: absolute; TOP: 670px; width:750px" role="presentation" aria-label="layout">
	                <tr>
		                <td style="float:right">
			                <FMCONTROLS:FMLABEL id="Label10" runat="server" CssClass="formfieldtitle" Width="176px" ForeColor="Crimson"
				                Height="8px">* Denotes Required Field</FMCONTROLS:FMLABEL>&nbsp;&nbsp;&nbsp;
			                <FMCONTROLS:FMBUTTON id="OK" tabIndex="8" runat="server" CssClass="formfieldtitle" Width="67px" Text="OK"></FMCONTROLS:FMBUTTON>
			                &nbsp;&nbsp;<FMCONTROLS:FMBUTTON id="Cancel" tabIndex="9" runat="server" CssClass="formfieldtitle" Text="Cancel"></FMCONTROLS:FMBUTTON></td>
	                </tr>
                </table>
            </div>
		</form>
	</body>
</HTML>
