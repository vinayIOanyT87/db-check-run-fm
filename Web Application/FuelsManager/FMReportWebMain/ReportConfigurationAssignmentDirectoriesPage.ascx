<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportConfigurationAssignmentDirectoriesPage.ascx.cs" Inherits="FuelsManager.FMReportWebMain.ReportConfigurationAssignmentDirectoriesPage" %>
<%@ Register assembly="FMControls" namespace="FMControls" tagprefix="FMControls" %>
<!DOCTYPE html >
<html>
<head>
    <meta content>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body>
    <p>
        &nbsp;</p>
    <p>
    <FMControls:FMLabel ID="ReportDirectoryLabel" AssociatedControlID="ReportDirectoryTextBox" Style="z-index: 111; left: 24px; position: absolute;
        top: 31px; width: 104px;" runat="server" BackColor="Transparent" 
    CssClass="formfieldtitle">Report Directory:</FMControls:FMLabel>
    <asp:TextBox ID="ReportDirectoryTextBox" Style="z-index: 110; left: 188px; position: absolute;
        top: 28px" TabIndex="30" runat="server" CssClass="formfield" 
    Width="200px" MaxLength="80"></asp:TextBox>
    <FMControls:FMCheckBox ID="ManageReportsCheckBox" Style="z-index: 103; left: 19px;
        position: absolute; top: 52px" TabIndex="31" runat="server" BackColor="Transparent"
        CssClass="formfieldtitle" Width="128px" Text="Manage Reports"></FMControls:FMCheckBox>
    <FMControls:FMLabel ID="ManagedReportDirectoryLabel" Style="z-index: 111; left: 24px; position: absolute;
        top: 76px" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
        Width="160px" AssociatedControlID="ManagedReportDirectoryTextBox">Managed Report Directory:</FMControls:FMLabel>
    <p>
    <asp:TextBox ID="ManagedReportDirectoryTextBox" Style="z-index: 110; left: 188px;
        position: absolute; top: 70px" TabIndex="32" runat="server" CssClass="formfield"
        Width="200px" MaxLength="80"></asp:TextBox>
    </p>
    <p>
        &nbsp;
    </p>
    <p>
        &nbsp;
    </p>
    <p>
        &nbsp;
    </p>
    <p>
		<FMCONTROLS:FMBUTTON id="OK" style="Z-INDEX: 109" tabIndex="100"
			runat="server" CssClass="formfieldtitle" Width="67px" Text="OK"></FMCONTROLS:FMBUTTON>
        &nbsp;
		<FMCONTROLS:FMBUTTON id="Cancel" style="Z-INDEX: 105" tabIndex="101"
			runat="server" CssClass="formfieldtitle" Width="67px" Text="Cancel"></FMCONTROLS:FMBUTTON>
    
    </p>
</body>
</html>
    
