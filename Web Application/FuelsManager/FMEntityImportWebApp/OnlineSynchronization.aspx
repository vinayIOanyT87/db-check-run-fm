<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OnlineSynchronization.aspx.cs" Inherits="FuelsManager.FMEntityImportWebApp.OnlineSynchronization" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="OnlineSynchronizationForm" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <asp:Image ID="FadeImage" Style="z-index: 101; left: 0px; position: absolute; top: 0px"
                runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
            <FMControls:FMLabel ID="OnlineSynchronizationLabel" Style="z-index: 102; left: 8px; position: absolute; top: 8px"
                runat="server" BackColor="Transparent" Width="272px"
                CssClass="headline">Online Synchronization</FMControls:FMLabel>
            <table id="MainTable" style="z-index: 103; left: 8px; position: absolute; top: 48px; height: 74px; border-collapse:collapse;"
                cellspacing="1" cellpadding="1" width="737px" border="0">
                <tr style="padding-bottom:1em;">
                    <td style="font-weight:bolder;">
                        <FMControls:FMLabel ID="ServiceStatusLabel" runat="server" CssClass="formfield">
                        Service Status:
                        </FMControls:FMLabel>
                    </td>
                    <td class="formfield" colspan="2">
                        <FMControls:FMLabel ID="ServiceStatusIdleLabel" runat="server" style="font-weight:bolder; color:green;">
                        Synchronization Idle
                        </FMControls:FMLabel>
                        <FMControls:FMLabel ID="ServiceStatusInProgressLabel" runat="server" style="font-weight:bolder; color:darkblue;">
                        Synchronization in Progress
                        </FMControls:FMLabel>
                        <FMControls:FMLabel ID="ServiceStatusNotAcceptingLabel" runat="server" style="font-weight:bolder; color:brown;">
                        Enterprise Server is currently not accepting synchronization requests
                        </FMControls:FMLabel>
                        <FMControls:FMLabel ID="ServiceStatusDisabledLocallyLabel" runat="server" style="font-weight:bolder; color:black;">
                        Synchronization is locally disabled.  Check synchronization configuration settings
                        </FMControls:FMLabel>
                        <FMControls:FMLabel ID="ServiceStatusWindowsServiceUnavailable" runat="server" style="font-weight:bolder; color:red;">
                        Windows Synchronization Service Unavailable
                        </FMControls:FMLabel>
                    </td>
                </tr>
                <tr>
                    <td style="z-index: 103;" colspan="3">
                        <FMControls:FMLabel ID="PageDescription" runat="server" CssClass="formfield">
                        Manually initiate Enterprise Synchronization.
                        </FMControls:FMLabel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMButton ID="SynchronizeButton" TabIndex="2" runat="server" Text="Synchronize" Width="100px"
                            CssClass="formfieldtitle" OnClick="SynchronizeButtonClick"></FMControls:FMButton>
                    </td>
                    <td>
                        <FMControls:FMButton ID="StopSynchronizationButton" TabIndex="3" runat="server" Text="Stop" Width="100px"
                            CssClass="formfieldtitle" OnClick="StopSynchronizeButtonClick" Enabled="false"></FMControls:FMButton>
                    </td>
                    <td width="500px">&nbsp;</td>
                </tr>
                <tr>
                    <td style="z-index: 103;" colspan="3">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="formfield" style="z-index: 103;" colspan="3">
                        <FMControls:FMLabel ID="SyncMessage" runat="server" style="font-weight:bolder; color:blue;">
                        </FMControls:FMLabel>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
