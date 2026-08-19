<%@ Page Language="c#" CodeBehind="FCRC_DetailForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FuelCardWebApp.FCRC_DetailForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FCRC_Detail" TagName="FCRC_GeneralPage" Src="FCRC_GeneralPage.ascx" %>
<%@ Register TagPrefix="FCRC_Detail" TagName="FCRC_EquipmentPage" Src="FCRC_EquipmentPage.ascx" %>
<%@ Register TagPrefix="FCRC_Detail" TagName="FCRC_UserDataPage" Src="FCRC_UserDataPage.ascx" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" %>

<!DOCTYPE html>
<html>
	<head runat="server">
        <title></title>
        <base target="_self" />
        <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
        <meta content="C#" name="CODE_LANGUAGE" />
        <meta content="JavaScript" name="vs_defaultClientScript" />
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
	</head>
	<body>
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
	<script type ="text/javascript">
		    function SetHelpKey(sender, e) {
		    	CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
		    }</script>
        <form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:ScriptManager ID="ScriptManager" runat="server" />
                <FMControls:FMLabel ID="FuelCardTitleLabel"
                    Style="z-index: 118; left: 16px; position: absolute; top: 8px" runat="server"
                    BackColor="Transparent" CssClass="headline">Fuel Card Detail Configuration</FMControls:FMLabel>
                <asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px" TabIndex="-1"
                    runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMTabContainer ID="tcFCRCDetailTabs" runat="server" ActiveTabIndex="0"
                    Style="position: absolute; top: 40px; left: 32px; width: 725px; height: 455px">
                    <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage" HelpKey="FuelCardWebApp/FCRC_GeneralPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FCRC_Detail:FCRC_GeneralPage runat="server" ID="FCRC_GeneralPage"></FCRC_Detail:FCRC_GeneralPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="Equipment" ID="tpEquipmentPage" HelpKey="FuelCardWebApp/FCRC_EquipmentPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FCRC_Detail:FCRC_EquipmentPage runat="server" ID="FCRC_EquipmentPage"></FCRC_Detail:FCRC_EquipmentPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel runat="server" HeaderText="User Data" ID="tpUserDataPage" HelpKey="FuelCardWebApp/FCRC_UserDataPage.ascx" OnClientClick='SetHelpKey'>
                        <ContentTemplate>
                            <FCRC_Detail:FCRC_UserDataPage runat="server" ID="FCRC_UserDataPage"></FCRC_Detail:FCRC_UserDataPage>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                </FMControls:FMTabContainer>
                <table style="z-index: 104; left: 32px; position: absolute; top: 495px; width: 725px">
                    <tr>
                        <td style="float: right">
                            <table>
                                <tr>
                                    <td>
                                        <FMControls:FMLabel ID="FMLABEL7" runat="server" BackColor="Transparent" CssClass="formfieldtitle" ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel></td>
                                    <td>&nbsp;&nbsp;</td>
                                    <td>
                                        <FMControls:FMButton ID="New" runat="server"
                                            CssClass="formfieldtitle" Text="New" OnClientClick="return CheckIfExpirationDateIsToday()" OnClick="NewCommand" Style="min-width: 66px"></FMControls:FMButton></td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <FMControls:FMButton ID="OK" runat="server"
                                            CssClass="formfieldtitle" Text="OK" OnClientClick="return CheckIfExpirationDateIsToday()" OnClick="OkCommand" Style="min-width: 66px"></FMControls:FMButton></td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <FMControls:FMButton ID="Cancel" Style="z-index: 121; min-width: 66px" runat="server" CssClass="formfieldtitle" Text="Cancel" OnClick="CancelCommand"></FMControls:FMButton></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </form>
	</body>
</html>
