<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Page Language="c#" CodeBehind="ChangePasswordForm.aspx.cs" AutoEventWireup="True" Inherits="FMWebApp.ChangePasswordForm" %>

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

    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Scripts/jquery-2.2.1.min.js" %>" type="text/javascript"></script>
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Scripts/jquery-ui-1.10.3.js" %>" type="text/javascript"></script>
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/UmnyangoForm.js" %>" type="text/javascript"></script>
	<style>
		#PasswordPopupBubbleLabel [title]:hover::after {
			content: attr(title);
			padding: 5px 5px 5px 5px;
			word-break: keep-all;
		}
	</style>
</head>
<body ms_positioning="GridLayout" tabindex="-1">
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			
            <FMControls:FMLabel ID="ChangePasswordLabel" Style="z-index: 109; left: 88px; position: absolute; top: 40px" runat="server" CssClass="headline" BackColor="Transparent">Change Password</FMControls:FMLabel>

			<FMControls:FMLabel ID="CurrentPasswordLabel" AssociatedControlID="CurrentPasswordTextBox" Style="z-index: 5; left: 88px; position: absolute; top: 96px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Current Password:</FMControls:FMLabel>
			
            <asp:TextBox ID="CurrentPasswordTextBox" Style="z-index: 5; left: 256px; position: absolute; top: 96px" TabIndex="1" runat="server" CssClass="formfield" Width="156px" Height="24" TextMode="Password" MaxLength="25" AutoCompleteType="None"></asp:TextBox>

			<FMControls:FMLabel ID="NewPasswordLabel" AssociatedControlID="NewPasswordTextBox" Style="z-index: 5; left: 88px; position: absolute; top: 136px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">New Password:</FMControls:FMLabel>
			
            <asp:TextBox ID="NewPasswordTextBox" Style="z-index: 5; left: 256px; position: absolute; top: 136px" TabIndex="1" runat="server" CssClass="formfield" Width="156px" Height="24" TextMode="Password" MaxLength="25" AutoCompleteType="None"></asp:TextBox>

            <FMControls:FMLabel ID="PasswordPopupBubbleLabel" title="PasswordPopupBubble" Style="left: 440px; top: 144px; color: #666666; font-family: Arial, Helvetica,sans-serif; font-size: 11.15px; position: absolute" Width="100px" CssClass="DefaultLoginPageLink" runat="server" Text="Password Policy" TabIndex="5"></FMControls:FMLabel>
			
            <FMControls:FMLabel ID="ReenterPasswordLabel" AssociatedControlID="ReenterPasswordTextBox" Style="z-index: 107; left: 88px; position: absolute; top: 176px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Re-enter Password:</FMControls:FMLabel>

			<asp:TextBox ID="ReenterPasswordTextBox" Style="z-index: 108; left: 256px; position: absolute; top: 176px" TabIndex="2" runat="server" CssClass="formfield" Width="156px" Height="24" TextMode="Password" MaxLength="25" AutoCompleteType="None"></asp:TextBox>

			<FMControls:FMButton ID="OK" Style="z-index: 105; left: 224px; position: absolute; top: 224px" TabIndex="3" runat="server" CssClass="formfieldtitle" Width="88px" Text="OK"></FMControls:FMButton>

			<FMControls:FMButton ID="Cancel" Style="z-index: 106; left: 328px; position: absolute; top: 224px" TabIndex="4" runat="server" CssClass="formfieldtitle" Width="88px" Text="Cancel"></FMControls:FMButton>

			<FMControls:FMLabel ID="ErrorMsgLabel" Style="z-index: 107; left: 88px; position: absolute; top: 300px" runat="server" CssClass="formfieldtitle" BackColor="Transparent"></FMControls:FMLabel>
		</div>
	</form>
	<script type="text/javascript">
		//debugger;
		//document.getElementById("OK").setActive();

		var oInitialPasswordTextBox = document.getElementById("InitialPasswordTextBox");
		var oPasswordTextBox = document.getElementById("PasswordTextBox");

		if ((oInitialPasswordTextBox != null) && (oPasswordTextBox != null))
		{
			oPasswordTextBox.focus();
			oPasswordTextBox.value = oInitialPasswordTextBox.value;
		}

		var oInitialReenterPasswordTextBox = document.getElementById("InitialReenterPasswordTextBox");
		var oReenterPasswordTextBox = document.getElementById("ReenterPasswordTextBox");

		if ((oInitialReenterPasswordTextBox != null) && (oReenterPasswordTextBox != null))
		{
			oReenterPasswordTextBox.value = oInitialReenterPasswordTextBox.value;
		}
	</script>
</body>
</html>
