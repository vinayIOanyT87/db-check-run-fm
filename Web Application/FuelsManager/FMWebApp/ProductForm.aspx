<%@ Register TagPrefix="FMWebApp" TagName="ProductVolumeCorrectionPage" Src="ProductVolumeCorrectionPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProductUserDataPage" Src="ProductUserDataPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProductAuthorizedCustomersPage" Src="ProductAuthorizedCustomersPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProductMessagesPage" Src="ProductMessagesPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProductComponentPage" Src="ProductComponentPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProductBlendPage" Src="ProductBlendPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProductAlarmsPage" Src="ProductAlarmsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProductAdditivePage" Src="ProductAdditivePage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProductUnitsPage" Src="ProductUnitsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProductGeneralPage" Src="ProductGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="ProductGraphicsPage" Src="ProductGraphicsPage.ascx" %>
<%@ Page language="c#" Codebehind="ProductForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ProductForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<!DOCTYPE html>
<HTML>
	<HEAD runat="server">
		<title></title>
		<base target="_self">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <style>
            .tabletext.small {
                height: 19px !important;
            }
            .ajax__tab_tab {
                height: 29px !important;
                min-width: 85px !important;
                margin-right: 5px !important;
            }
            td.ajax__tab_tab {
                border-right-width: 4px !important;
            }
        </style>
	</HEAD>
	<body MS_POSITIONING="GridLayout" >

        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>


		<script type="text/javascript">		    function SetHelpKey(sender, e) {
		    	CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
		    }</script>
		<form id="ProductForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
                <asp:ScriptManager ID="ScriptManager" runat="server"/>
			    <asp:image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 98; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				    ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image>
			    <FMControls:FMLabel id="ProductTitleLabel" 
                    style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				    BackColor="Transparent" Width="550px" CssClass="headline">Product Configuration</FMControls:FMLabel>
                <FMControls:FMTabContainer ID="tcProductTabs" runat="server" ActiveTabIndex="0" Style="z-index: 104; position: absolute; top: 40px; left: 32px; width: 725px; height: 455px">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" HelpKey="FMWebApp/ProductGeneralPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:ProductGeneralPage runat="server" ID="ProductGeneralPage"></FMWebApp:ProductGeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="BlendPage" ID="tpBlendPage" HelpKey="FMWebApp/ProductBlendPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:ProductBlendPage runat="server" ID="ProductBlendPage"></FMWebApp:ProductBlendPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="ComponentPage" ID="tpComponentPage" HelpKey="FMWebApp/ProductComponentPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:ProductComponentPage runat="server" ID="ProductComponentPage"></FMWebApp:ProductComponentPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="AdditivePage" ID="tpAdditivePage" HelpKey="FMWebApp/ProductAdditivePage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:ProductAdditivePage runat="server" ID="ProductAdditivePage"></FMWebApp:ProductAdditivePage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="UnitsPage" ID="tpUnitsPage" HelpKey="FMWebApp/ProductUnitsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:ProductUnitsPage runat="server" ID="ProductUnitsPage"></FMWebApp:ProductUnitsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Authorized Customers" ID="tpAuthorizedCustomersPage" HelpKey="FMWebApp/ProductAuthorizedCustomersPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:ProductAuthorizedCustomersPage runat="server" ID="ProductAuthorizedCustomersPage"></FMWebApp:ProductAuthorizedCustomersPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Volume Correction" ID="tpVolumeCorrectionPage"
                        HelpKey="FMWebApp/ProductVolumeCorrectionPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:ProductVolumeCorrectionPage runat="server" ID="ProductVolumeCorrectionPage"></FMWebApp:ProductVolumeCorrectionPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Alarms" ID="tpAlarmsPage" HelpKey="FMWebApp/ProductAlarmsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:ProductAlarmsPage runat="server" ID="ProductAlarmsPage"></FMWebApp:ProductAlarmsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Messages" ID="tpMessagesPage" HelpKey="FMWebApp/ProductMessagesPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:ProductMessagesPage runat="server" ID="ProductMessagesPage"></FMWebApp:ProductMessagesPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Graphics" ID="tpGraphicsPage" HelpKey="FMWebApp/ProductGraphicsPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:ProductGraphicsPage runat="server" ID="ProductGraphicsPage"></FMWebApp:ProductGraphicsPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="User Data" ID="tpUserDataPage" HelpKey="FMWebApp/ProductUserDataPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FMWebApp:ProductUserDataPage runat="server" ID="ProductUserDataPage"></FMWebApp:ProductUserDataPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </FMControls:FMTabContainer>

                <table style="Z-INDEX: 104; LEFT: 32px; POSITION: absolute; TOP: 495px; width:700px" >
        			<tr><td style="float:right">
    			        <table>
				           <tr>
				              <td><FMControls:FMLabel id="Label10"  runat="server"
				                    Width="176px" CssClass="formfieldtitle" Height="8px" ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel>
				              </td>
				              <td>&nbsp;&nbsp;</td>
				              <td><FMControls:FMButton id="New"  
				                    runat="server" TabIndex="100" Width="66px" CssClass="formfieldtitle" Text="New"></FMControls:FMButton>
				              </td>
				              <td>&nbsp;</td>
				              <td><FMControls:FMButton id="OK" 
				                    runat="server" TabIndex="101" Width="66px" CssClass="formfieldtitle" Text="OK"></FMControls:FMButton>
				              </td>
				              <td>&nbsp;</td>
				              <td><FMControls:FMButton id="Cancel" 
				                    runat="server" TabIndex="102" Width="66px" CssClass="formfieldtitle" Text="Cancel" CommandName="Cancel"></FMControls:FMButton>
				              </td>
				           </tr>
				        </table>
					</td></tr>
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
