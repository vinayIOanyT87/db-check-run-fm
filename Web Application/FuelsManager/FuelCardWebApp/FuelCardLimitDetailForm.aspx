<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FuelCardLimitDetailForm.aspx.cs" Inherits="FuelsManager.FuelCardWebApp.FuelCardLimitDetailForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FuelCardWebApp" TagName="FuelCardLimitGeneralPage" Src="FuelCardLimitGeneralPage.ascx" %>
<%@ Register TagPrefix="FuelCardWebApp" TagName="FuelCardLimitAssignedFuelCardsPage" Src="FuelCardLimitAssignedFuelCardsPage.ascx" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title></title>
</head>
<script type="text/javascript">
	var rndTokenStr = '<%= Security.CSRFToken%>';
</script>
<body>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken_min.js" %>"></script>
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
    <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
    <form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <FMControls:FMLabel ID="FuelCardLimitDetailLabel" Style="z-index: 102; left: 8px; position: absolute; top: 8px"
                runat="server" BackColor="Transparent" CssClass="headline">Fuel Card Limit Configuration</FMControls:FMLabel>
            <FMControls:FMTabContainer ID="tcFuelCardLimit" runat="server" Style="z-index: 103; left: 32px; position: absolute; top: 40px; width: 725px; height: 425px">
                <ajaxToolkit:TabPanel runat="server" HeaderText="General" ID="tpGeneralPage">
                    <ContentTemplate>
                        <FuelCardWebApp:FuelCardLimitGeneralPage runat="server" ID="FuelCardLimitGeneralPage"></FuelCardWebApp:FuelCardLimitGeneralPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" HeaderText="Assigned Fuel Cards" ID="tpAssignedFuelCardsPage">
                    <ContentTemplate>
                        <FuelCardWebApp:FuelCardLimitAssignedFuelCardsPage runat="server" ID="FuelCardLimitAssignedFuelCardsPage"></FuelCardWebApp:FuelCardLimitAssignedFuelCardsPage>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </FMControls:FMTabContainer>
            <table style="z-index: 104; left: 32px; position: absolute; top: 500px">
                <tr>
                    <td style="float: right">
                        <table>
                            <tr>
                                <td>
                                    <FMControls:FMLabel ID="lblRequiredFields" runat="server" Width="176px" CssClass="formfieldtitle"
                                        Height="8px" ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel>
                                </td>
                                <td>&nbsp;&nbsp;
                                </td>
                                <td>
                                    <FMControls:FMButton ID="btnOK" TabIndex="101" runat="server" Width="66px" CssClass="formfieldtitle"
                                        Text="OK" OnClick="btnOK_Click"></FMControls:FMButton>
                                </td>
                                <td>&nbsp;
                                </td>
                                <td>
                                    <FMControls:FMButton ID="btnCancel" TabIndex="102" runat="server" Width="66px" CssClass="formfieldtitle"
                                        Text="Cancel" CommandName="Cancel" OnClick="btnCancel_Click"></FMControls:FMButton>
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
