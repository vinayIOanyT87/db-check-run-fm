<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileCommunicationSettingPage.ascx.cs"
	Inherits="FuelsManager.FMWebApp.ProfileCommunicationSettingPage" %>
<html>
<head>
	<title></title>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<style type="text/css">
		.style1
		{
			width: 213px;
		}
		.style2
		{
			width: 183px;
		}
	</style>
</head>
<body>
	<table style="z-index: 103; width: 78%; left: 5px; position: absolute; top: 50px;
		height: 130px;">
		<tr>
			<td class="style1">
				<FMControls:FMLabel ID="CommunicationTimeoutSecondsLbl" CssClass="formfieldtitle"
					runat="server" BackColor="Transparent" Width="180px">Communication Timeout Seconds</FMControls:FMLabel>
			</td>
			<td>
				<FMControls:FMTextBox ID="CommunicationTimeoutSecondsTB" TabIndex="18" Width="168px"
					 MaxLength="4" Columns="4" runat="server"></FMControls:FMTextBox>
			</td>
			<td>&nbsp;</td>
			<td class="style2">
				<FMControls:FMLabel ID="ConnectionRetryTimeoutLbl" CssClass="formfieldtitle" runat="server"
					Width="220px" BackColor="Transparent">Connection Retry Timeout</FMControls:FMLabel>
			</td>
			<td>
				<FMControls:FMTextBox ID="ConnectionRetryTimeoutTB" TabIndex="2" Width="168px" CssClass="formfield"
					runat="server" MaxLength="4" Columns="4"></FMControls:FMTextBox>
			</td>
		</tr>
		<tr>
			<td class="style1">
				<FMControls:FMLabel ID="ConnectionRetriesLbl" Width="220px" CssClass="formfieldtitle"
					runat="server" BackColor="Transparent">Connection Retries</FMControls:FMLabel>
			</td>
			<td>
				<FMControls:FMTextBox ID="ConnectionRetriesTB" TabIndex="2" Width="168px" CssClass="formfield"
					runat="server" MaxLength="4" Columns="4"></FMControls:FMTextBox>
			</td>
			<td>&nbsp;</td>
			<td class="style2">
				<FMControls:FMLabel ID="ConnectionTypeLbl" Width="220px" CssClass="formfieldtitle"
					runat="server" BackColor="Transparent">Connection Type</FMControls:FMLabel>
			</td>
			<td>
				<FMControls:FMDropDownList ID="ConnectionTypeDD" TabIndex="2" Width="168px" CssClass="formfield"
					runat="server" MaxLength="10">
				</FMControls:FMDropDownList>
			</td>
		</tr>
		<tr>
			<td class="style1">
				<FMControls:FMLabel ID="UpdateIntervalLbl" CssClass="formfieldtitle" runat="server"
					BackColor="Transparent" Width="160px">Update Interval</FMControls:FMLabel>
			</td>
			<td>
				<FMControls:FMTextBox ID="UpdateIntervalTB" TabIndex="17" Width="168px" CssClass="formfield"
					MaxLength="4" Columns="4" runat="server"></FMControls:FMTextBox>
			</td>
			<td>&nbsp;</td>
			<td class="style2">
				<FMControls:FMLabel ID="PresubmitDelayLbl" Width="220px" CssClass="formfieldtitle"
					runat="server" BackColor="Transparent">Presubmit Delay</FMControls:FMLabel>
			</td>
			<td>
				<FMControls:FMTextBox ID="PresubmitDelayTB" TabIndex="2" Width="168px" CssClass="formfield"
					runat="server" MaxLength="4" Columns="4"></FMControls:FMTextBox>
			</td>
		</tr>
		<tr>
			<td class="style1">
				<FMControls:FMCheckBox ID="PingVerificationIpAddressCB" 
					Text="Ping Verification IP Address" CssClass="formfieldtitle" runat="server" 
					AutoPostBack="True" oncheckedchanged="PingIpAddressCheckedChange" />
			</td>
			<td>
				&nbsp;</td>
			<td>&nbsp;</td>
			<td class="style2">
				<FMControls:FMLabel ID="VehicleUpdateIntervalLbl" Width="220px" CssClass="formfieldtitle"
					runat="server" BackColor="Transparent">Vehicle Update Interval</FMControls:FMLabel>
			</td>
			<td>
				<FMControls:FMTextBox ID="VehicleUpdateIntervalTB" TabIndex="2" Width="168px"
					CssClass="formfield" runat="server" MaxLength="4" Columns="4"></FMControls:FMTextBox>
			</td>
		</tr>
		<tr>
			<td class="style1">
				<FMControls:FMLabel ID="VerificationIpAddressLbl" Width="220px" CssClass="formfieldtitle"
					runat="server" BackColor="Transparent">Verification IP Address</FMControls:FMLabel>
			</td>
			<td>
				<FMControls:FMTextBox ID="VerificationIpAddressTB" TabIndex="2" Width="168px" CssClass="formfield"
					runat="server" MaxLength="15" Columns="15"></FMControls:FMTextBox>
			</td>
			<td>&nbsp;</td>
			<td class="style2">&nbsp;</td>
			<td>&nbsp;</td>
		</tr>
	</table>
</body>
</html>
