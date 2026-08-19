<%@ Page Language="c#" CodeBehind="EquipmentTypeDetailsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EquipmentTypeDetailsForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentTypeGeneralPage" Src="EquipmentTypeGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentTypeAircraftGeneralPage" Src="EquipmentTypeAircraftGeneralPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentTypeAircraftTanksPage" Src="EquipmentTypeAircraftTanksPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentTypeReqQualificationsPage" Src="EquipmentTypeReqQualificationsPage.ascx" %>
<%@ Register TagPrefix="FMWebApp" TagName="EquipmentTypeReqTrainingPage" Src="EquipmentTypeReqTrainingPage.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
</head>
<body ms_positioning="GridLayout">
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
    <script type="text/javascript">
        function SetHelpKey(sender, e) {
            CurrentHelpKey = sender.get_element().getAttribute("HelpKey");
        }
    </script>
    <asp:ScriptManager ID="ScriptManager" runat="server" />
    <form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">

            <asp:Image ID="Image1" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
                ImageUrl="<%$ AppSettings: PageFadeImage %>" CssClass="formfieldtitle"></asp:Image>

            <FMControls:FMLabel ID="EquipmentTypeTitleLabel"
                Style="z-index: 102; left: 8px; position: absolute; top: 8px" runat="server"
                BackColor="Transparent" CssClass="headline">Equipment Type Configuration</FMControls:FMLabel>

            <FMControls:FMTabContainer ID="tcEquipmentTypeDetails" runat="server" Style="z-index: 103; position: absolute; top: 40px; left: 32px; width: 725px; height: 425px" ActiveTabIndex="0">
                <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneral" HelpKey="FMWebApp/EquipmentTypeGeneralPage.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                        <FMWebApp:EquipmentTypeGeneralPage runat="server" ID="EquipmentTypeGeneralPage"></FMWebApp:EquipmentTypeGeneralPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Aircraft-General" ID="tpAircraftGeneral" HelpKey="FMWebApp/EquipmentTypeAircraftGeneralPage.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                        <FMWebApp:EquipmentTypeAircraftGeneralPage runat="server" ID="EquipmentTypeAircraftGeneralPage" Visible="true"></FMWebApp:EquipmentTypeAircraftGeneralPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Tanks" ID="tpTanks" HelpKey="FMWebApp/EquipmentTypeAircraftTanksPage.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                        <FMWebApp:EquipmentTypeAircraftTanksPage runat="server" ID="EquipmentTypeAircraftTanksPage" Visible="true"></FMWebApp:EquipmentTypeAircraftTanksPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Required Qualifications" ID="tpRequiredQualifications" HelpKey="FMWebApp/EquipmentTypeReqQualificationsPage.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                        <FMWebApp:EquipmentTypeReqQualificationsPage runat="server" ID="EquipmentTypeReqQualificationsPage"></FMWebApp:EquipmentTypeReqQualificationsPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Required Training" ID="tpRequiredTraining" HelpKey="FMWebApp/EquipmentTypeReqTrainingPage.ascx" OnClientClick='SetHelpKey'>
                    <ContentTemplate>
                        <FMWebApp:EquipmentTypeReqTrainingPage runat="server" ID="EquipmentTypeReqTrainingpage"></FMWebApp:EquipmentTypeReqTrainingPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </FMControls:FMTabContainer>
            <table width="700px" style="z-index: 104; left: 32px; position: absolute; top: 465px">
                <tr>
                    <td align="right">
                        <table>
                            <tr>
                                <td>
                                    <FMControls:FMLabel ID="Label10" runat="server"
                                        Width="176px" CssClass="formfieldtitle" Height="8px" ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel></td>
                                <td>&nbsp;&nbsp;</td>
                                <td>
                                    <FMControls:FMButton ID="New" TabIndex="100"
                                        runat="server" Width="66px" CssClass="formfieldtitle" Text="New"></FMControls:FMButton></td>
                                <td>&nbsp;</td>

                                <td>
                                    <FMControls:FMButton ID="OK" TabIndex="101"
                                        runat="server" Width="66px" CssClass="formfieldtitle" Text="OK"></FMControls:FMButton></td>
                                <td>&nbsp;</td>
                                <td>
                                    <FMControls:FMButton ID="Cancel" TabIndex="102" Width="66px" 
                                        runat="server" CssClass="formfieldtitle" Text="Cancel" CommandName="Cancel"></FMControls:FMButton></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <script>
                document.getElementById("OK").focus();
            </script>
        </div>
    </form>
</body>
</html>
