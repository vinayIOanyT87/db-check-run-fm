<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileOpsConfigSettingPage.ascx.cs" Inherits="FuelsManager.FMWebApp.ProfileOpsConfigSettingPage" %>

<html>
	<head>
		<title></title>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
		<style type="text/css">
			.style1
			{
				width: 220px;
			}
			.style2
			{
				width: 196px;
			}
		</style>
	</head>
	<body>
		<table style="Z-INDEX: 103; width:78%; LEFT: 5px; POSITION: absolute; TOP: 50px; height: 160px;">     
			<tr>
				<td class="style1" >
					<FMControls:FMLabel ID="GseWaitMsecGetMeterLbl" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
						Width="180px">GSE Wait MSec For Get Meter</FMControls:FMLabel>
				</td>
				<td class="style2">
					<FMControls:FMTextBox ID="GseWaitMsecGetMeterTB" TabIndex="18" Width="168px" MaxLength="6" Columns="6" runat="server"></FMControls:FMTextBox>
				</td>
				<td >&nbsp;</td>
				<td >
					<FMCONTROLS:FMCHECKBOX id="ConfirmFuelCapsCB" TextAlign="Right" Text="Confirm Fuel Caps" Height="27px" 
						width="160px" CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMCHECKBOX>
				</td>
			</tr>
			<tr>
				<td class="style1" >
					<FMCONTROLS:FMLABEL id="GseInactiveLogoutLbl" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
						Width="160px">GSE Inactive Logout Minutes</FMCONTROLS:FMLABEL>                
				</td>
				<td class="style2" >
					<FMCONTROLS:FMTextBox id="GseInactiveLogoutTB" tabIndex="17" Width="168px" CssClass="formfield" 
						MaxLength="5" Columns="5" runat="server"></FMCONTROLS:FMTextBox>
				</td>
				<td >&nbsp;</td>
				<td >
					<FMCONTROLS:FMCHECKBOX id="VtoEnableCB" TextAlign="Right" Text="VTO Enabled" Height="27px" 
						width="160px" CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMCHECKBOX>
				</td>
		   </tr>
			<tr>
				<td class="style1" >
					<FMCONTROLS:FMLABEL id="GseInactivityTimeoutLbl" CssClass="formfieldtitle" runat="server" 
						Width="220px" BackColor="Transparent">GSE Inactivity Timeout</FMCONTROLS:FMLABEL>
				</td>
				<td class="style2" >
					<FMControls:FMTextBox id="GseInactivityTimeoutTB" tabIndex="2" Width="168px" CssClass="formfield" runat="server" 
						MaxLength="5" Columns="5"></FMControls:FMTextBox>
				</td>
				<td >&nbsp;</td>
				<td >
					<FMCONTROLS:FMCHECKBOX id="EnableInOpGaugesCB" TextAlign="Right" Text="Enable InOp Gauges" Height="27px"
					    Width="160px" CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMCHECKBOX>
				</td>
		  </tr>
			<tr>
				<td class="style1" >
					<FMCONTROLS:FMLABEL id="BarcodeInvalidWarningLbl" CssClass="formfieldtitle" runat="server" 
						Width="220px" BackColor="Transparent">Barcode Invalid Warning Seconds</FMCONTROLS:FMLABEL>
				</td>
				<td class="style2" >
					<FMControls:FMTextBox id="BarcodeInvalidWarningTB" tabIndex="2" Width="168px" CssClass="formfield" runat="server" 
						MaxLength="5" Columns="5"></FMControls:FMTextBox>
				</td>
				<td >&nbsp;</td>
				<td >
					<FMCONTROLS:FMCHECKBOX id="UseDispensingVehicleGseCB" TextAlign="Right" Text="Use Dispensing Vehicle for GSE Transactions" Height="27px"
					    Width="180px" CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMCHECKBOX>
				</td>
		  </tr>
		  <tr>
		  	<td class="style1">
		  		<FMCONTROLS:FMLABEL id="DeIceBlendDefaultLbl" Width="220px" CssClass="formfieldtitle" runat="server" 
					BackColor="Transparent">De-Ice Blend Default</FMCONTROLS:FMLABEL>
				</td>
		  	<td class="style2">
		  		<FMControls:FMTextBox id="DeIceBlendDefaultTB" tabIndex="2" Width="168px" CssClass="formfield" runat="server" 
					MaxLength="10" Columns="10"></FMControls:FMTextBox>
				</td>
		  	<td>&nbsp;</td>
		  	<td>&nbsp;</td>
		  </tr>
		  </table>
	</body>
</html>