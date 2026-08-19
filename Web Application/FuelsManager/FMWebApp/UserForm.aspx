<%@ Page Language="c#" CodeBehind="UserForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.UserForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMWebApp" TagName="UserUserDataPage" Src="UserUserDataPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="UserGeneralPage" Src="UserGeneralPage.ascx" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title></title>
	<base target="_self">
	<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
</head>
<body leftmargin="20" rightmargin="20" ms_positioning="GridLayout" tabindex="-1">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
     <script type="text/javascript">
	   function SetHelpKey(sender, e) {
			CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
	   }
	  </script>

	<form id="UserForm" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position:absolute">
		  <asp:ScriptManager ID="ScriptManager" runat="server"/>  
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="UserTitleLabel" Style="left: 8px; position: absolute;
				top: 8px" runat="server" CssClass="headline" BackColor="Transparent">User Configuration</FMControls:FMLabel>

			<FMControls:FMTabContainer ID="tcUserTabs" runat="server" ActiveTabIndex="0" TabWidth="60px"
					Style="position: absolute; top: 40px; left: 32px; width: 700px;
					height: 555px" aria-label="User Tabs">
				<ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" HelpKey="FMWebApp/UserGeneralPage.ascx" OnClientClick='SetHelpKey'>
					<ContentTemplate>
						<FMWebApp:UserGeneralPage runat="server" ID="UserGeneralPage"></FMWebApp:UserGeneralPage>
					</ContentTemplate>
				</ajaxToolkit:TabPanel>
				<ajaxToolkit:TabPanel runat="server" HeaderText="User Data" ID="tpUserUserDataPage" HelpKey="FMWebApp/UserUserDataPage.ascx" OnClientClick='SetHelpKey'>
					<ContentTemplate>
						<FMWebApp:UserUserDataPage runat="server" ID="UserUserDataPage"></FMWebApp:UserUserDataPage>
					</ContentTemplate>
				</ajaxToolkit:TabPanel>
			</FMControls:FMTabContainer>
			
			<table style="left:32px; position:absolute; top:550px; width:750px" role="presentation" aria-label="layout">
				<tr>
					<td style="float:right">
						<table>
							<tr>
								<td>
									<FMControls:FMLabel ID="RequiredLabel" runat="server" CssClass="formfieldtitle" Width="144px" Height="8px"
										ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel>
									<FMControls:FMButton ID="OK" runat="server" CssClass="formfieldtitle" Text="OK" Width="75px" TabIndex="12">
									</FMControls:FMButton>
									<FMControls:FMButton ID="Cancel" runat="server" CssClass="formfieldtitle" Width="75px" Text="Cancel" TabIndex="13">
									</FMControls:FMButton>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>

	
	</div>
</form>

</body>
</html>
