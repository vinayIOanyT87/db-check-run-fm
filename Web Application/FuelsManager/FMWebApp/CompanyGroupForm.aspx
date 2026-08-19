<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="CompanyGroupForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.CompanyGroupForm" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyGroupGeneralPage" Src="CompanyGroupGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="CompanyGroupProductsPage" Src="CompanyGroupProductsPage.ascx" %>
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
	<body MS_POSITIONING="GridLayout" role="application">
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
                <asp:ScriptManager ID="ScriptManager" runat="server"/>
			    <asp:Image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 98; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				    ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
			    <FMControls:FMLabel id="CompanyGroupTitleLabel" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				    BackColor="Transparent" CssClass="headline" Width="500px">Company Group Configuration</FMControls:FMLabel>
                <FMControls:FMTabContainer ID="tcCompanyGroupTabs" runat="server" ActiveTabIndex="0"
                    Style="z-index:103;position: absolute; top: 40px; left: 32px">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" HelpKey="FMWebApp/CompanyGroupGeneralPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyGroupGeneralPage runat="server" ID="CompanyGroupGeneralPage"></FMWebApp:CompanyGroupGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Products" ID="tpProductsPage" HelpKey="FMWebApp/CompanyGroupProductsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:CompanyGroupProductsPage runat="server" ID="CompanyGroupProductsPage">
                            </FMWebApp:CompanyGroupProductsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </FMControls:FMTabContainer>
                <table style="Z-INDEX: 104; LEFT: 32px; POSITION: absolute; TOP: 465px; width:700px" >
			        <tr><td style="float:right">
			        <table>
			           <tr>
			              <td><FMControls:FMLabel id="FMRequiredFieldLabel"  Height="8px"
				        ForeColor="Crimson" CssClass="formfieldtitle" Width="144px" runat="server">* Denotes Required Field</FMControls:FMLabel></td>
			              <td>&nbsp;&nbsp;</td>
			              <td><FMControls:FMButton id="OK"  runat="server"
				        CssClass="formfieldtitle minWidth70" Text="OK"></FMControls:FMButton></td>
			              <td>&nbsp;</td>
			              <td><FMControls:FMButton id="Cancel"  runat="server"
				        CssClass="formfieldtitle minWidth70" Text="Cancel"></FMControls:FMButton></td>
			           </tr>
			        </table>
			        </td></tr>
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
