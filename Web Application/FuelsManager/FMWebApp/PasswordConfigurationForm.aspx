<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Page Language="c#" CodeBehind="PasswordConfigurationForm.aspx.cs" AutoEventWireup="True"
	Inherits="FuelsManager.FMWebApp.PasswordConfigurationForm" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
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
        <div id="pageContent" style="position:absolute">
		<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>

		<FMControls:FMLabel ID="PwdConfigurationLabel" Style="z-index: 102; left: 16px; position: absolute; top: 16px" runat="server" BackColor="Transparent" CssClass="headline" Width="328px">Security Settings</FMControls:FMLabel>

		<FMControls:FMLabel ID="MinTimeAllowedLabel" AssociatedControlID="MinTimeAllowedTextbox" Style="z-index: 108; left: 16px; position: absolute; top: 64px;" runat="server" CssClass="formfieldtitle">Minimum time allowed between changes:</FMControls:FMLabel>
		<FMControls:FMTextBox ID="MinTimeAllowedTextbox" Style="z-index: 107; left: 264px; position: absolute; top: 64px" TabIndex="1" runat="server" CssClass="formfield" Width="40px" Columns="2" MaxLength="2"></FMControls:FMTextBox>

		<FMControls:FMLabel ID="Days1Label" Style="z-index: 115; left: 320px; position: absolute; top: 66px" runat="server" CssClass="formfieldtitle">days</FMControls:FMLabel>
		   
		<FMControls:FMCheckBox ID="StrongPwdCheckBox" Style="z-index: 112; left: 400px; position: absolute; top: 64px" TabIndex="5" runat="server" CssClass="formfieldtitle" Text="Strong password" Width="200px" AutoPostBack="True" OnCheckedChanged="StrongPwdCheckBox_CheckedChanged"></FMControls:FMCheckBox>
		<FMControls:FMLabel ID="StrongPasswordDescriptionLabel" AssociatedControlID="StrongPwdCheckBox" Style="z-index: 104; left: 432px; position: absolute; top: 86px; color: rgb(160, 160, 160); font-size: 11.15px; font-weight: 100; word-wrap: break-word; word-break: break-all; " Width="380px" runat="server" CssClass="formfieldtitle"></FMControls:FMLabel>

		<FMControls:FMLabel ID="PwdAgingLabel" AssociatedControlID="PwdAgingTextBox" Style="z-index: 101; left: 16px; position: absolute; top: 98px" runat="server" CssClass="formfieldtitle">Maximum time allowed between changes:</FMControls:FMLabel>	
		<FMControls:FMTextBox ID="PwdAgingTextBox" Style="z-index: 111; left: 264px; position: absolute; top: 96px" TabIndex="2" runat="server" CssClass="formfield" Width="40px" Columns="3" MaxLength="3"></FMControls:FMTextBox>
                		   
		<FMControls:FMLabel ID="Days2Label" Style="z-index: 117; left: 320px; position: absolute; top: 98px" runat="server" CssClass="formfieldtitle">days</FMControls:FMLabel>
		   	
		<FMControls:FMCheckBox ID="EnhancedStrongPwdCheckBox" Style="z-index: 112; left: 400px; position: absolute; top: 116px" TabIndex="5" runat="server" CssClass="formfieldtitle" Text="Enhanced Strong password" Width="188px" AutoPostBack="True" OnCheckedChanged="EnhancedStrongPwdCheckBox_CheckedChanged"></FMControls:FMCheckBox>
		<FMControls:FMTextBox ID="MinNumOfCharTextbox" Style="z-index: 105; left: 264px; position: absolute; top: 128px" TabIndex="3" runat="server" CssClass="formfield" Width="40px" Columns="2" MaxLength="2"></FMControls:FMTextBox>	
		<FMControls:FMLabel ID="MinNumOfCharLabel" AssociatedControlID="MinNumOfCharTextbox" Style="z-index: 106; left: 16px; position: absolute; top: 128px; word-wrap: break-word; word-break: break-all; " Width="190px" runat="server" CssClass="formfieldtitle"></FMControls:FMLabel>
		<FMControls:FMLabel ID="EnhancedPasswordDescriptionLabel" AssociatedControlID="EnhancedStrongPwdCheckBox" Style="z-index: 104; left: 432px; position: absolute; top: 138px; color: rgb(160, 160, 160); font-size: 11.15px; font-weight:100; word-wrap: break-word; word-break: break-all; " Width="385px" runat="server" CssClass="formfieldtitle"></FMControls:FMLabel>

		<FMControls:FMLabel ID="MinNumOfCharLabel2" AssociatedControlID="MinNumOfCharTextbox" Style="z-index: 106; left: 16px; position: absolute; top: 145px; color: rgb(160, 160, 160); font-size: 11.15px; font-weight: 100; word-wrap: break-word; word-break: break-all; " Width="190px" runat="server" CssClass="formfieldtitle"></FMControls:FMLabel>

		<FMControls:FMTextBox ID="LockoutThresholdTextbox" Style="z-index: 109; left: 264px; position: absolute; top: 169px" TabIndex="4" runat="server" CssClass="formfield" Width="40px" Columns="2" MaxLength="2"></FMControls:FMTextBox>
		<FMControls:FMCheckBox ID="PreviousPwdCheckBox" Style="z-index: 113; left: 400px; position: absolute; top: 170px" TabIndex="6" runat="server" CssClass="formfieldtitle" Text="Check for previous passwords" Width="232px" AutoPostBack="True" OnCheckedChanged="ChkPrevPwdOnChange"></FMControls:FMCheckBox>
		<FMControls:FMLabel ID="LockoutThresholdLabel" AssociatedControlID="LockoutThresholdTextbox" Style="z-index: 110; left: 16px; position: absolute; top: 172px" runat="server" CssClass="formfieldtitle">Lockout Threshold:</FMControls:FMLabel>	

		<FMControls:FMTextBox ID="DisableArchivePeriodTextbox" Style="z-index: 120; left: 264px; position: absolute; top: 198px" TabIndex="4" runat="server" Width="40px" CssClass="formfield" MaxLength="3" Columns="3"></FMControls:FMTextBox>	
                

		<FMControls:FMTextBox ID="HowManyTextbox" Style="z-index: 103; left: 512px; position: absolute; top: 198px" TabIndex="8" runat="server" CssClass="formfield" Width="40px" Columns="2" MaxLength="2"></FMControls:FMTextBox>
		<FMControls:FMLabel ID="HowManyLabel" AssociatedControlID="HowManyTextbox" Style="z-index: 104; left: 432px; position: absolute; top: 200px" runat="server" CssClass="formfieldtitle">How many:</FMControls:FMLabel>

		<FMControls:FMLabel ID="Days3Label" Style="z-index: 121; left: 320px; position: absolute; top: 200px" runat="server" CssClass="formfieldtitle">days</FMControls:FMLabel>
		<FMControls:FMLabel ID="InactivityLabel" AssociatedControlID="InactivityDisplayPeriodTextbox" Style="z-index: 119; left: 16px; position: absolute; top: 230px" runat="server" CssClass="formfieldtitle">Inactivity disable period:</FMControls:FMLabel>	

		<FMControls:FMCheckBox ID="EnablePasswordHintCheckbox" Style="z-index: 118; left: 400px; position: absolute; top: 225px" TabIndex="6" runat="server" CssClass="formfieldtitle" Text="Enable Password Hint Feature" Width="232px" AutoPostBack="True"></FMControls:FMCheckBox>
            
		<FMControls:FMTextBox ID="InactivityDisplayPeriodTextbox" Style="z-index: 120; left: 264px; position: absolute; top: 228px" TabIndex="4" runat="server" Width="40px" CssClass="formfield" MaxLength="3" Columns="3"></FMControls:FMTextBox>	
		<FMControls:FMLabel ID="DisableArchiveLabel" AssociatedControlID="DisableArchivePeriodTextbox" Style="z-index: 119; left: 16px; position: absolute; top: 201px" runat="server" CssClass="formfieldtitle">Disable archive period:</FMControls:FMLabel>
                
		<FMControls:FMLabel ID="Day4Label" Style="z-index: 121; left: 320px; position: absolute; top: 231px" runat="server" CssClass="formfieldtitle">days</FMControls:FMLabel>  
                
		<FMControls:FMCheckBox ID="EnablePasswordResetCheckbox" Style="z-index: 118; left: 400px; position: absolute; top: 253px" TabIndex="6" runat="server" CssClass="formfieldtitle" Text="Enable Password Reset Feature" Width="232px" AutoPostBack="True"></FMControls:FMCheckBox>
	
		<FMControls:FMCheckBox ID="ApplySettingToMemSitesCheckbox" Style="z-index: 118; left: 400px; position: absolute; top: 280px" TabIndex="6" runat="server" CssClass="formfieldtitle" Text="Propagate to all member sites on Apply" Width="232px" AutoPostBack="True"></FMControls:FMCheckBox>
	
		<FMControls:FMCheckBox ID="AllowSpecialCharsCheckbox" Style="z-index: 118; left: 400px; position: absolute; top: 283px" TabIndex="7" runat="server" CssClass="formfieldtitle" Text="Allow Use of Special Characters" Width="232px" AutoPostBack="True" Visible="false"></FMControls:FMCheckBox>
	
		<FMControls:FMButton ID="ApplyBtn" Style="z-index: 114; left: 408px; position: absolute; top: 331px" TabIndex="9" runat="server" CssClass="formfieldtitle" Text="Apply" Width="67px" OnClick="ApplyBtnOnClick"></FMControls:FMButton>
        </div>
	</form>
</body>
</html>
