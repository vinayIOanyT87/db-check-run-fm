<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecirculationForm.aspx.cs" Inherits="FuelsManager.DispatchWebApp.RecirculationForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <base target="_self" />
    <title></title>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" type="text/javascript"  defer="defer"></script>
</head>
<body>
    <form id="recirculationForm" runat="server">
        <script type="text/javascript">
            function ShowAlertDialog(alertMessage) {
                setTimeout(function () {
                    alert(alertMessage);
                }, 0);
            }

            function SetFocus(controlName) {
                setTimeout(function () {

                    // Focus on the control 
                    var controlToSetFocus = document.getElementById(controlName);

                    if (controlToSetFocus != null) {
                        controlToSetFocus.focus();
                    }

                }, 5); //It seems that we need a slightly longer wait period or otherwise focus may not get set
            }
        </script>
        <div>
            <asp:ScriptManager ID="theScriptManager" runat="server" />
            <asp:Panel ID="DetailsPanel" runat="server" GroupingText="Details" CssClass="formfieldtitle" Width="775px">
                <table>
                    <tr>
                        <td>
                            <FMControls:FMLabel ID="TypeLabel" runat="server" CssClass="formfieldtitle" Text="Type:" />
                        </td>
                        <td>
                            <FMControls:FMDropDownList ID="TypeDropDownList" runat="server" CssClass="formfield" Width="170px">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem>Maintenance</asp:ListItem>
                                <asp:ListItem>Quality Control</asp:ListItem>
                                <asp:ListItem>Simulation Dry Run</asp:ListItem>
                                <asp:ListItem>Hose Pressure Test</asp:ListItem>
                                <asp:ListItem>Other</asp:ListItem>
                            </FMControls:FMDropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <FMControls:FMLabel ID="RegistrationIDLabel" runat="server" CssClass="formfieldtitle" Text="Registration ID:" Width="100px" />
                        </td>
                        <td>
                            <FMControls:FMDropDownList ID="RegistrationIDDropDownList" runat="server" CssClass="formfield" Width="170px" DataTextField="ID" DataValueField="MasterRecordGuid"/>
                        </td>
                        <td style="padding-left: 50px">
                            <FMControls:FMLabel ID="StartDateTimeLabel" runat="server" CssClass="formfieldtitle" Text="Start Time:" Width="125px" />
                        </td>
                        <td>
                            <FMControls:FMDateTime ID="StartDateTimeControl" runat="server" CssClass="formfield" Width="200px" Height="25px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <FMControls:FMLabel ID="OperatorLabel" runat="server" CssClass="formfieldtitle" Text="Operator:" Width="100px" />
                        </td>
                        <td>
                            <FMControls:FMDropDownList ID="OperatorDropDownList" runat="server" CssClass="formfield" Width="170px" DataTextField="FullName" DataValueField="IdentityGuid" />
                        </td>
                        <td style="padding-left: 50px">
                            <FMControls:FMLabel ID="StopDateTimeLabel" runat="server" CssClass="formfieldtitle" Text="Stop Time:" Width="125px" />
                        </td>
                        <td>
                            <FMControls:FMDateTime ID="StopDateTimeControl" runat="server" CssClass="formfield" Width="200px" Height="25px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <FMControls:FMLabel ID="ProductLabel" runat="server" CssClass="formfieldtitle" Text="Product:" Width="100px" />
                        </td>
                        <td>
                            <FMControls:FMDropDownList ID="ProductDropDownList" runat="server" CssClass="formfield" Width="170px" DataTextField="ID" DataValueField="MasterRecordGuid"/>
                        </td>
                        <td style="padding-left: 50px">
                            <FMControls:FMLabel ID="BOSLabel" runat="server" CssClass="formfieldtitle" Text="BOS:" Width="125px" />
                        </td>
                        <td>
                            <FMControls:FMTextBox ID="BOSTextBox" runat="server" CssClass="formfield" Width="160px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <FMControls:FMLabel ID="CardNumberLabel" runat="server" CssClass="formfieldtitle" Text="Card Number:" Width="100px" />
                        </td>
                        <td>
                            <FMControls:FMTextBox ID="CardNumberTextBox" runat="server" CssClass="formfield" Width="170px" />
                        </td>
                        <td style="padding-left: 50px">
                            <FMControls:FMLabel ID="IssuePointNumberLabel" runat="server" CssClass="formfieldtitle" Text="Issue Point Number:" Width="125px" />
                        </td>
                        <td>
                            <FMControls:FMTextBox ID="IssuePointNumberTextBox" runat="server" CssClass="formfield" Width="160px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <FMControls:FMLabel ID="IssuePointLabel" runat="server" CssClass="formfieldtitle" Text="Issue Point:" Width="100px" />
                        </td>
                        <td>
                            <FMControls:FMTextBox ID="IssuePointTextBox" runat="server" CssClass="formfield" Width="160px" />
                        </td>
                        <td style="padding-left: 50px">
                            <FMControls:FMLabel ID="SerialNumberLabel" runat="server" CssClass="formfieldtitle" Text="Serial Number:" Width="125px" />
                        </td>
                        <td>
                            <FMControls:FMTextBox ID="SerialNumberTextBox" runat="server" CssClass="formfield" Width="160px" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="VolumesPanel" runat="server" GroupingText="Volumes" CssClass="formfieldtitle" Width="775px" >
                <table>
                    <tr>
                        <td>
                            <FMControls:FMLabel ID="NetVolumeLabel" runat="server" CssClass="formfieldtitle" Text="Net:" Width="100px" />
                        </td>
                        <td>
                            <FMControls:FMTextBox ID="NetVolumeTextBox" runat="server" CssClass="formfield" Width="160px" MaxLength="10"/>
                        </td>
                        <td style="padding-left: 55px">
                            <FMControls:FMLabel ID="GrossVolumeLabel" runat="server" CssClass="formfieldtitle" Text="Gross:" Width="125px" />
                        </td>
                        <td>
                            <FMControls:FMTextBox ID="GrossVolumeTextBox" runat="server" CssClass="formfield" Width="160px" MaxLength="10"/>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <table>
                <tr>
                    <td style="padding-left: 7px">
                        <FMControls:FMLabel ID="MemoLabel" runat="server" CssClass="formfieldtitle" Text="Memo:" Width="100px" />
                    </td>
                    <td>
                        <FMControls:FMTextBox ID="MemoTextBox" TextMode="MultiLine" runat="server" CssClass="formfield" Width="655px" Height="80px" MaxLength="1000" />
                    </td>
                </tr>
            </table>

            <table style="z-index: 104; left: 32px; position: absolute; top: 452px; width: 275px; height:20px">
                <tr>
                    <td style="padding-left: 75px;">
                        <FMControls:FMButton ID="OKButton" TabIndex="100"
                            runat="server" Width="66px" CssClass="formfieldtitle" Text="OK" OnClick="OkButtonClick" /></td>
                    <td style="padding-left: 200px;">
                        <FMControls:FMButton ID="CancelButton" TabIndex="101"
                            runat="server" Width="66px" CssClass="formfieldtitle" Text="Cancel" OnClientClick="window.close(); return false;" /></td>
                    <td style="padding-left: 200px; padding-right:75px ">
                        <FMControls:FMButton ID="ApplyButton" TabIndex="102"
                            runat="server" CssClass="formfieldtitle" Text="Apply" Width="66px" OnClick="ApplyButtonClick" /></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
