<%@ Page language="c#" Codebehind="PersonForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.TrainingWebApp.PersonForm" %>
<%@ Register TagPrefix="TrainingWebApp" TagName="PersonTrainingPage" Src="PersonTrainingPage.ascx" %>
<%@ Register TagPrefix="TrainingWebApp" TagName="PersonQualificationsPage" Src="PersonQualificationsPage.ascx" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<!DOCTYPE html>
<HTML>
	<HEAD runat="server">
		<title></title>
		<base target="_self">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="PersonForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:ScriptManager ID="ScriptManager" runat="server" />
                <asp:Image ID="Image1" Style="z-index: 1; left: 0px; position: absolute; top: 0px; height: 452px;" runat="server"
                    CssClass="formfieldtitle" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="MainHeaderLabel" Style="z-index: 102; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="800px" BackColor="Transparent">Qualifications/Training Configuration</FMControls:FMLabel>
                <FMControls:FMTabContainer ID="tcPersonTabs" runat="server" ActiveTabIndex="0"
                    Style="z-index: 104; position: absolute; top: 40px; left: 32px; width: 700px; height: 455px">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Qualifications" ID="tpQualificationsPage">
                        <ContentTemplate>
                            <TrainingWebApp:PersonQualificationsPage runat="server" ID="PersonQualificationsPage"></TrainingWebApp:PersonQualificationsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Training" ID="tpTrainingPage">
                        <ContentTemplate>
                            <TrainingWebApp:PersonTrainingPage runat="server" ID="PersonTrainingPage"></TrainingWebApp:PersonTrainingPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </FMControls:FMTabContainer>
                <table style="z-index: 200; left: 32px; position: absolute; top: 500px; width: 700px">
                    <tr>
                        <td align="right">
                            <table>
                            <tr>
                                <td>
                                    <FMControls:FMButton ID="OK" TabIndex="102"
                                        runat="server" CssClass="formfieldtitle" Width="66px" Text="OK"></FMControls:FMButton>
                                </td>
                                <td>&nbsp;</td>
                                <td>
                                    <FMControls:FMButton ID="Cancel" TabIndex="103"
                                        runat="server" CssClass="formfieldtitle" Text="Cancel" CommandName="Cancel"></FMControls:FMButton>
                                </td>
                            </tr>
	                    </table>
                        </td>
                    </tr>
                </table>
			    <script>
			        var okButton = document.getElementById("OK");
			        if (!okButton.disabled)
			            okButton.setActive();
			    </script>
            </div>
		</form>
	</body>
</HTML>
