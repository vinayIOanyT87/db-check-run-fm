<%@ Register TagPrefix="FMWebApp" TagName="IATACodeUserDataPage" Src="IATACodeUserDataPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="IATACodeGeneralPage" Src="IATACodeGeneralPage.ascx" %>
<%@ Page language="c#" Codebehind="IATACodeMainForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.IATACodeMainForm"  %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls"%>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<!DOCTYPE html>
<HTML>
	<head runat="server">
		<title></title>
		<base target="_self">
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</head>
	<body tabindex="-1">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
		<script type="text/javascript">
		    var theMoment = new Date();
		    var theDisplacement = (theMoment.getTimezoneOffset() / 60);
		    document.cookie = "Displacement=" + theDisplacement;
		</script>
		<form id="IATACodeMainForm" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
            <asp:ScriptManager ID="ScriptManager" runat="server"/>
			<FMControls:FMLabel id="IATACodeTitleLabel"
            style="LEFT: 8px; POSITION: absolute; TOP: 8px; width: 600px;" runat="server"
				BackColor="Transparent" CssClass="headline">Delivery Location Configuration</FMControls:FMLabel>
			<asp:image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" CssClass="formfieldtitle"></asp:image>
            <FMControls:FMTabContainer ID="tcIATACodeTabs" runat="server" ActiveTabIndex="0" 
                style="position:absolute;top:35px;left:12px;width:780px;height:440px">
                <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" ToolTip="General Page" HelpKey="FMWebApp/IATACodeGeneralPage.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                        <FMWebApp:IATACodeGeneralPage runat="server" ID="IATACodeGeneralPage"></FMWebApp:IATACodeGeneralPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="User Data" ID="tpUserDataPage" ToolTip="User Data Page" HelpKey="FMWebApp/IATACodeUserDataPage.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                        <FMWebApp:IATACodeUserDataPage runat="server" ID="IATACodeUserDataPage"></FMWebApp:IATACodeUserDataPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </FMControls:FMTabContainer>
			<table width="700px" style="LEFT:32px; POSITION: absolute; TOP: 590px">
			<tr><td align="right">
			<table>
			   <tr>
			      <td><FMControls:FMLabel id="DenotesRequiredFieldLabel"  runat="server"
				      Width="176px" CssClass="formfieldtitle" Height="8px" ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel></td>
			      <td>&nbsp;&nbsp;</td>
			      <td><FMControls:FMButton id="New"  tabIndex="100"
				      runat="server" Width="66px" CssClass="formfieldtitle" Text="New"></FMControls:FMButton></td>
			      <td>&nbsp;</td>
			      <td><FMControls:FMButton id="OK"  tabIndex="101"
				      runat="server" Width="66px" CssClass="formfieldtitle" Text="OK"></FMControls:FMButton></td>
			      <td>&nbsp;</td>
			      <td><FMControls:FMButton id="Cancel" tabIndex="102"
				      runat="server" Width="66px" CssClass="formfieldtitle" Text="Cancel" CommandName="Cancel"></FMControls:FMButton></td>
			   </tr>
			</table>
			</td>
			</tr>
			</table>
		</div>
    </form>
	<script type="text/javascript">
	    var okButton = document.getElementById("OK");
	    if(!okButton.disabled) okButton.setActive();

	    function SetHelpKey(sender, e)
	    {
	        CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
	    }
	</script>
    </body>
</HTML>
