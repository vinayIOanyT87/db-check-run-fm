<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DispatchSettingsConfigurationPage.aspx.cs"
	Inherits="FuelsManager.DispatchWebApp.DispatchSettingsConfigurationPage" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
	<form runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent">
			<div id="content" style="position: absolute">
			<asp:Image ID="fadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; top: 0px; position: absolute;"
				runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
			<FMControls:FMLabel ID="titleLabel" Style="z-index: 118; left: 8px; top: 8px; position: absolute; width: 800px"
				runat="server" BackColor="Transparent" CssClass="headline">Dispatch Settings Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="generalSettingsLabel" Style="z-index: 118; left: 32px; position: absolute; top: 45px;"
				runat="server" BackColor="Transparent" Text="General Settings" CssClass="formfieldtitle" />
			<asp:Panel ID="generalSettingsPanel" Style="z-index: 103; left: 32px; position: absolute; top: 65px; width: 760px; height: 75px;"
				runat="server" BorderColor="LightSteelBlue"
				BorderStyle="Solid" BorderWidth="1px" />
			<FMControls:FMCheckBox ID="enableServiceRequestsCheckBox" TabIndex="9" runat="server"
				CssClass="formfieldtitle" Style="z-index: 118; left: 45px; position: absolute; top: 80px; height: 20px"
				Text="Enable Service Requests"></FMControls:FMCheckBox>
			<FMControls:FMLabel ID="refreshPeriodLabel" AssociatedControlID="refreshPeriodTextBox" Style="z-index: 118; left: 235px; position: absolute; top: 84px;"
				runat="server" BackColor="Transparent" Text="Refresh Period:" CssClass="formfieldtitle" />
			<asp:TextBox ID="refreshPeriodTextBox" TabIndex="10" runat="server" Style="z-index: 118; position: absolute; left: 336px; top: 80px; width: 65px"
				CssClass="formfieldNoWrap" />
			<FMControls:FMLabel ID="refreshPeriodUnitsLabel" Style="z-index: 118; left: 413px; position: absolute; top: 85px;"
				runat="server" BackColor="Transparent" Text="seconds"
				CssClass="formfieldNoWrap" />
			<FMControls:FMLabel ID="automaticRestartDelayLabel" AssociatedControlID="automaticRestartDelayTextBox" Style="z-index: 118; left: 513px; position: absolute; top: 84px;"
				runat="server" BackColor="Transparent" Text="Automatic Restart Delay:"
				CssClass="formfieldtitle" />
			<asp:TextBox ID="automaticRestartDelayTextBox" TabIndex="11" runat="server" Style="z-index: 118; position: absolute; left: 666px; top: 80px; width: 55px"
				CssClass="formfieldNoWrap" />
			<FMControls:FMLabel ID="automaticRestartDelayUnitsLabel" Style="z-index: 118; left: 731px; position: absolute; top: 85px;"
				runat="server" BackColor="Transparent" Text="seconds"
				CssClass="formfieldNoWrap" />
			<FMControls:FMCheckBox ID="displayCurrentTimeCheckBox" TabIndex="12" runat="server"
				CssClass="formfieldtitle" Style="z-index: 118; left: 45px; position: absolute; top: 107px; height: 20px"
				Text="Display Current Time"></FMControls:FMCheckBox>
				<FMControls:FMLabel ID="fuelsManagerReportUrlLabel" AssociatedControlID="fuelsManagerReportUrlTextBox" Style="z-index: 118; left: 236px; position: absolute; top: 112px;"
				runat="server" BackColor="Transparent" Text="FuelsManager Report URL:" CssClass="formfieldtitle" />
				<asp:TextBox ID="fuelsManagerReportUrlTextBox" TabIndex="10" runat="server" Style="z-index: 118; position: absolute; left: 398px; top: 107px; width: 375px"
				CssClass="formfieldNoWrap" />
			<FMControls:FMLabel ID="fillstandSettingsLabel" Style="z-index: 118; left: 32px; position: absolute; top: 170px;"
				runat="server" BackColor="Transparent" Text="Fillstand Settings"
				CssClass="formfieldtitle" />
			<asp:Panel ID="fillstandSettingsPanel" Style="z-index: 103; left: 32px; position: absolute; top: 190px; width: 165px; height: 50px;"
				runat="server" BorderColor="LightSteelBlue"
				BorderStyle="Solid" BorderWidth="1px" />
			<FMControls:FMRadioButtonList ID="FMRadioFillToActualStandard" runat="server"
				Style="z-index: 118; position: absolute; left: 45px; top: 190px; height: 42px; width: 162px;"
				CssClass="formfieldtitle">
				<asp:ListItem Selected="True" Value="0">Fill to Actual</asp:ListItem>
				<asp:ListItem Value="1">Fill to Standard</asp:ListItem>
			</FMControls:FMRadioButtonList>

			<FMControls:FMLabel ID="otherSettingsLabel" Style="z-index: 118; left: 212px; position: absolute; top: 170px;"
				runat="server" BackColor="Transparent" Text="Other Settings"
				CssClass="formfieldtitle" />
			<asp:Panel ID="otherSettingsPanel" Style="z-index: 103; left: 212px; position: absolute; top: 190px; width: 580px; height: 50px;"
				runat="server" BorderColor="LightSteelBlue"
				BorderStyle="Solid" BorderWidth="1px" />
			<FMControls:FMCheckBox ID="tabularViewDisplayMilitaryDateCheckBox" TabIndex="14"
				runat="server" CssClass="formfieldtitle" Style="z-index: 118; left: 550px; position: absolute; top: 193px; height: 20px"
				Text="Display Military Julian Date"></FMControls:FMCheckBox>
			<FMControls:FMCheckBox ID="chkUseArrivalTime" TabIndex="14"
				runat="server" CssClass="formfieldtitle" Style="z-index: 118; left: 226px; position: absolute; top: 193px; height: 20px"
				Text="Use Arrival Time"></FMControls:FMCheckBox>
			<FMControls:FMCheckBox ID="chkUseStartTime" TabIndex="14"
				runat="server" CssClass="formfieldtitle" Style="z-index: 118; left: 226px; position: absolute; top: 218px; height: 20px"
				Text="Use Start Time"></FMControls:FMCheckBox>
			<FMControls:FMCheckBox ID="chkUseStopTime" TabIndex="14"
				runat="server" CssClass="formfieldtitle" Style="z-index: 118; left: 388px; position: absolute; top: 193px; height: 20px"
				Text="Use Stop Time"></FMControls:FMCheckBox>

			<!--
			<FMControls:FMLabel ID="displaySettingsLabel" Style="z-index: 118; left: 32px; position: absolute; top: 390px;"
				runat="server" BackColor="Transparent" Text="Display Settings"
				CssClass="formfieldtitle" />
			<asp:Panel ID="displaySettingsPanel" Style="z-index: 103; left: 32px; position: absolute; top: 410px; width: 760px; height: 73px;"
				runat="server" BorderColor="LightSteelBlue"
				BorderStyle="Solid" BorderWidth="1px" />
			<FMControls:FMLabel ID="operationalWindowPastLabel" Style="z-index: 118; left: 45px; position: absolute; top: 429px;"
				runat="server" BackColor="Transparent" Text="Operational Window Hours In The Past:"
				CssClass="formfieldtitle" />
			<asp:TextBox ID="operationalWindowPastHours" TabIndex="17" runat="server" Style="z-index: 118; position: absolute; left: 288px; top: 427px; width: 32px;"
				CssClass="formfieldNoWrap"></asp:TextBox>
			<FMControls:FMLabel ID="operationalWindowFutureLabel" Style="z-index: 118; left: 45px; position: absolute; top: 454px;"
				runat="server" BackColor="Transparent" Text="Operational Window Hours In The Future:"
				CssClass="formfieldtitle" />
			<asp:TextBox ID="operationalWindowFutureHours" TabIndex="18" runat="server" Style="z-index: 118; position: absolute; left: 288px; top: 452px; width: 32px;"
				CssClass="formfieldNoWrap"></asp:TextBox>
			<FMControls:FMCheckBox runat="server" ID="StaticTimeDisplayCheckBox" CssClass="formfieldtitle" Text="Static Time Display" Style="z-index: 119; position: absolute; left: 375px; top: 427px" />
			<FMControls:FMCheckBox runat="server" ID="ShowGridLinesCheckBox" CssClass="formfieldtitle" Text="Show Grid Lines" Style="z-index: 119; position: absolute; left: 375px; top: 454px" />
			-->

			<!-- Original top was 505 -->
			<FMControls:FMButton ID="applyButton" Style="z-index: 118; left: 720px; position: absolute; top: 270px"
				TabIndex="19" runat="server" CssClass="formfieldtitle" Text="Apply"
				Width="72px" OnClick="ApplyButtonOnClick"></FMControls:FMButton>
		</div>
		</div>
	</form>
</body>
</html>
