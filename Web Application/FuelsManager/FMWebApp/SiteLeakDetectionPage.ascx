<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="SiteLeakDetectionPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SiteLeakDetectionPage" %>
<html>
<head>
    <title>Site Leak Detection Configuration</title>
    <style>
        #siteLeakDetectionPageDiv  .formfieldtitle {
            min-width: 220px;
            padding: 5px;
        }
    
    </style>
</head>
<body>
    <div id="siteLeakDetectionPageDiv" style="z-index: 103; left: 5px; position: absolute;width: 600px; top: 5px;" role="presentation" aria-label="layout">
            <div>
                <FMControls:FMLabel ID="NumberQuietTimeSamplesLabel" AssociatedControlID="NumberQuietTimeSamplesTextBox" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Minimum Number Quiet Time Samples</FMControls:FMLabel>
                <asp:TextBox ID="NumberQuietTimeSamplesTextBox" TabIndex="1" runat="server" Width="84px" MaxLength="7" CssClass="formfield"></asp:TextBox>
            </div>
            <div>
                <FMControls:FMLabel ID="MinimumTotalQuietTimeLabel" runat="server" AssociatedControlID="MinimumTotalQuietTimeTextbox"
                    CssClass="formfieldtitle" BackColor="Transparent">Minimum Total Quiet Time</FMControls:FMLabel>
                <asp:TextBox ID="MinimumTotalQuietTimeTextbox" 
                    TabIndex="4" runat="server" CssClass="formfield" Width="84px" MaxLength="7"></asp:TextBox>
            </div>
            <div>
                <FMControls:FMLabel ID="QuietTimeFactorLabel" runat="server" AssociatedControlID="QuietTimeFactorTextBox"
                    CssClass="formfieldtitle" BackColor="Transparent">Quiet Time Factor</FMControls:FMLabel>
                <asp:TextBox ID="QuietTimeFactorTextBox" TabIndex="5" runat="server" Width="84px" MaxLength="7" CssClass="formfield">
                </asp:TextBox>
            </div>
            <div>
                <FMControls:FMLabel ID="UseMinimumIssueWaitLabel" AssociatedControlID="UseMinimumIssueWaitDropDownList"
                    runat="server" CssClass="formfieldtitle" BackColor="Transparent">Use Minimum Issue Wait</FMControls:FMLabel>
                <asp:DropDownList ID="UseMinimumIssueWaitDropDownList"
                    TabIndex="7" runat="server" CssClass="formfield" Width="88px" AutoPostBack="True">
                </asp:DropDownList>
            </div>
        </div>
</body>
</html>
