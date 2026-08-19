<%@ Page language="c#" Codebehind="AuditLogConfigurationForm.aspx.cs" AutoEventWireup="True" Inherits="FMWebApp.AuditLogConfigurationForm" EnableSessionState="True"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<base target="_self">
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1" >
		<SCRIPT>
		</SCRIPT>
        <form id="AuditForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="264px" BackColor="Transparent">Database Audit Log Configuration</FMControls:FMLabel>
                <FMControls:FMCheckBox ID="AuditReadAccessCheckBox" Style="z-index: 120; left: 16px; position: absolute; top: 50px"
                    runat="server" CssClass="formfieldtitle" Text="Audit Read Access" TabIndex="1" Checked="true" />
                <FMControls:FMCheckBox ID="AuditSelectedUsersCheckBox" Style="z-index: 120; left: 16px; position: absolute; top: 110px"
                    runat="server" CssClass="formfieldtitle" Text="Audit only selected users" Checked="true"
                    TabIndex="2" AutoPostBack="True"
                    OnCheckedChanged="AuditSelectedUsersCheckBox_CheckedChanged" />
                <FMControls:FMLabel ID="UnassignedUserGroupsLabel" Style="z-index: 112; left: 16px; position: absolute; top: 150px; width: 188px;"
                    runat="server" CssClass="formfieldtitle">Assigned Users:</FMControls:FMLabel>
                <FMControls:FMListBox ID="AssignedUsersListBox" Style="z-index: 124; left: 16px; position: absolute; top: 182px; right: 808px;"
                    TabIndex="3" runat="server" BackColor="White" Width="240px" CssClass="formfield" Height="88px" Enabled="true" SelectionMode="Multiple">
                </FMControls:FMListBox>
                <asp:Button ID="AssignUsersButton" Style="z-index: 122; left: 267px; position: absolute; top: 195px; padding-left: 1px; padding-right: 1px"
                    TabIndex="4" runat="server" CssClass="formfieldtitle" Text="<<"
                    OnCommand="AssignUsersButton_Command"></asp:Button>
                <asp:Button ID="UnAssignUsersButton" Style="z-index: 123; left: 267px; position: absolute; top: 225px; padding-left: 1px; padding-right: 1px"
                    TabIndex="5" runat="server" CssClass="formfieldtitle" Text=">>"
                    OnCommand="UnAssignUsersButton_Command"></asp:Button>
                <FMControls:FMLabel ID="AssignedUserGroupsLabel" Style="z-index: 120; left: 304px; position: absolute; top: 150px; width: 209px; right: 862px;"
                    runat="server" BackColor="Transparent" CssClass="formfieldtitle">Available Users:</FMControls:FMLabel>
                <FMControls:FMListBox ID="AvailableUsersListBox" Style="z-index: 121; left: 304px; position: absolute; top: 182px"
                    TabIndex="6" runat="server" BackColor="White" Width="240px" CssClass="formfield" Height="88px" Enabled="true"
                    SelectionMode="Multiple">
                </FMControls:FMListBox>
                <FMControls:FMButton ID="SaveButton" Style="z-index: 109; left: 485px; position: absolute; top: 297px; width: 55px;"
                    runat="server" CssClass="formfieldtitle" Text="Apply" OnClick="OKButton_Click"
                    TabIndex="7"></FMControls:FMButton>
                <script type="text/javascript">
                    document.getElementById("AuditReadAccessCheckBox").focus();
                </script>
            </div>
        </form>
	</body>
</HTML>
