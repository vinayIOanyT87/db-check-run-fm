<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="Accounting" TagName="ExStarsCreateReports" Src="ExStarsCreateReports.ascx" %>
<%@ Register TagPrefix="Accounting" TagName="ExStarsUploadToServer" Src="ExStarsUploadToServer.ascx" %>
<%@ Register TagPrefix="Accounting" TagName="ExStarsViewHistory" Src="ExStarsViewHistory.ascx" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExStarsForm.aspx.cs" Inherits="FuelsManager.Accounting.ExStarsForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<%@ Register TagPrefix="Accounting" Namespace="FuelsManager.Accounting" Assembly="FuelsManager" %>
<!DOCTYPE html>
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body tabIndex="-1" MS_POSITIONING="GridLayout">
     <link href="<%=HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
     <script type="text/javascript">
	   function SetHelpKey(sender, e) {
		   CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
	   }
     </script>
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute; top: 150px">
            <asp:ScriptManager ID="ScriptManager" runat="server"/>
            <div style="margin-left: 40px">
                <asp:Image ID="FadeImage"
                    Style="z-index: -3; left: 0px; position: absolute; top: 0px" runat="server"
                    ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
                <FMControls:FMLabel id="Label2" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: -32px" runat="server"
				    CssClass="headline" Width="352px" BackColor="Transparent">ExSTARS Reporting</FMControls:FMLabel>
            </div>

            <FMControls:FMTabContainer ID="tcExStarTabs" runat="server" ActiveTabIndex="0" Style="z-index: 104;
                    position: absolute; top: 1px; left: 32px; width: 795px; height: 455px">
                
                <ajaxToolkit:TabPanel runat="server" HeaderText="Create Report" ID="tpExStarsCreateReport" HelpKey="Accounting/ExStarsCreateReports.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                        <Accounting:ExStarsCreateReports  runat="server" ID="ExStarsCreateReports"></Accounting:ExStarsCreateReports>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                
                <ajaxToolkit:TabPanel runat="server" HeaderText="Upload To Server" ID="tpExStarsUploadToServer" HelpKey="Accounting/ExStarsUploadToServer.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                        <Accounting:ExStarsUploadToServer runat="server" ID="ExStarsUploadToServer"></Accounting:ExStarsUploadToServer>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                
                <ajaxToolkit:TabPanel runat="server" HeaderText="View&nbsp;&nbsp;&nbsp; History" ID="tpExStarsViewHistory" HelpKey="Accounting/ExStarsViewHistory.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                        <Accounting:ExStarsViewHistory  runat="server" ID="ExStarsViewHistory"></Accounting:ExStarsViewHistory>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>


            </FMControls:FMTabContainer>

                  <asp:Image id="Image1" style="Z-INDEX: 99; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
        </div>
  
    </form>
</body>
</html>
