<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Page Language="c#" CodeBehind="SystemSettingForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SystemSettingForm" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body ms_positioning="GridLayout">
    <form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent">
            <div  style="position: absolute">
            <FMControls:FMLabel ID="Label5" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                BackColor="Transparent" Width="912px" CssClass="headline">System Settings Configuration</FMControls:FMLabel>

            <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 101; left: 0px; position: absolute; top: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
        </div>
            <table style="position: absolute; top: 140px">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <FMControls:FMLabel ID="ReportServerUrlLabel" AssociatedControlID="ReportServerURLTextBox" Style="z-index: 102;" runat="server"
                                    BackColor="Transparent" CssClass="formfieldtitle">Report Server URL:</FMControls:FMLabel>
                            </td>
                            <td>
                                <asp:TextBox ID="ReportServerURLTextBox" Style="z-index: 106;"
                                    TabIndex="1" runat="server" BackColor="White" Width="288px" CssClass="formfield" MaxLength="80" Visible="True"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <FMControls:FMLabel ID="ReportSrvUserNameLable" AssociatedControlID="txtReportServerUserName" Style="z-index: 102;" runat="server"
                                    BackColor="Transparent" CssClass="formfieldtitle">Report Server User Name:</FMControls:FMLabel>
                            </td>
                            <td>
                                <asp:TextBox ID="txtReportServerUserName" Style="z-index: 106;"
                                    TabIndex="1" runat="server" BackColor="White" Width="288px" CssClass="formfield" MaxLength="80" Visible="True"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <FMControls:FMLabel ID="ReportSrvPwdLabel" AssociatedControlID="txtReportServerPassword" Style="z-index: 102;" runat="server"
                                    BackColor="Transparent" CssClass="formfieldtitle">Report Password:</FMControls:FMLabel>
                            </td>
                            <td>
                                <asp:TextBox ID="txtReportServerPassword" Style="z-index: 106;"
                                    TabIndex="1" runat="server" BackColor="White" Width="288px" CssClass="formfield" MaxLength="80" Visible="True" TextMode="Password" AutoCompleteType="None"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <FMControls:FMLabel ID="StationMsgTimeoutLabel" AssociatedControlID="StationMessageTimeoutTextBox" Style="z-index: 107;" runat="server"
                                    BackColor="Transparent" CssClass="formfieldtitle">Station Message Timeout:</FMControls:FMLabel>
                            </td>
                            <td>
                                <asp:TextBox ID="StationMessageTimeoutTextBox" Style="z-index: 108;"
                                    TabIndex="2" runat="server" BackColor="White" Width="44px" CssClass="formfield" MaxLength="50" Visible="True"></asp:TextBox>
                                &nbsp;&nbsp;&nbsp;
                                <FMControls:FMLabel ID="SecondsLabel1" Style="z-index: 111;" runat="server"
                                    BackColor="Transparent" CssClass="formfieldtitle">seconds</FMControls:FMLabel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <FMControls:FMLabel ID="FMLabel2" AssociatedControlID="StationPromptTimeoutTextBox" Style="z-index: 109;" runat="server"
                                    BackColor="Transparent" CssClass="formfieldtitle">Station Prompt Timeout:</FMControls:FMLabel>
                            </td>
                            <td>
                                <asp:TextBox ID="StationPromptTimeoutTextBox" Style="z-index: 110;"
                                    TabIndex="3" runat="server" BackColor="White" Width="44px" CssClass="formfield" MaxLength="50" Visible="True"></asp:TextBox>
                                &nbsp;&nbsp;&nbsp;
                                <FMControls:FMLabel ID="SecondsLabel2" Style="z-index: 111;" runat="server"
                                    BackColor="Transparent" CssClass="formfieldtitle">seconds</FMControls:FMLabel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">&nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <FMControls:FMCheckBox ID="SsoModeCheckBox" Style="z-index: 128;" TabIndex="4" runat="server" CssClass="formfieldtitle" Width="296px" Text="Single Sign On Mode"></FMControls:FMCheckBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">&nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <FMControls:FMLabel ID="Label2" Style="z-index: 113;" runat="server"
                                    CssClass="formfieldtitle">Note: Certain devices may have timeout limits below these settings.</FMControls:FMLabel>
                            </td>
                        </tr>
                    </table>
                    <br/><br/>
                    <table>
                        <tr>
                            <td>
                                <FMControls:FMButton runat="server" ID="ConfigButton" Text="Configuration Settings" CssClass="formfieldtitle" TabIndex="5" Style="z-index: 105;" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <FMControls:FMButton ID="OK" Style="z-index: 104;" TabIndex="5"
                                    runat="server" Width="67px" CssClass="formfieldtitle" Text="Apply"></FMControls:FMButton>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        </div>
    </form>
    <script language="jscript">
        document.getElementById("OK").setActive();
        document.getElementById("ReportServerURLTextBox").focus();
    </script>
</body>
</html>
