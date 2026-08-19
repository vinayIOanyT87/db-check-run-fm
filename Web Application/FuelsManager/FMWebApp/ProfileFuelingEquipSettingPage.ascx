<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileFuelingEquipSettingPage.ascx.cs"
	Inherits="FuelsManager.FMWebApp.ProfileFuelingEquipSettingPage" %>
<html>
<head>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
	<table style="z-index: 103; width: 56%; left: 5px; position: absolute; top: 50px;
		height: 100px;">
		<tr>
			<td>
				<FMControls:FMLabel ID="RTDTempRangeMinLabel" CssClass="formfieldtitle" runat="server"
					Width="220px" BackColor="Transparent">RTD Temperature Range Minimum</FMControls:FMLabel>
			</td>
			<td>
				<FMControls:FMTextBox ID="RTDTempRangeMinTB" TabIndex="1" Width="168px" CssClass="formfield"
					runat="server" MaxLength="30" Columns="30"></FMControls:FMTextBox>
			</td>
		</tr>
		<tr>
			<td>
				<FMControls:FMLabel ID="RTDTempRangeMaxLabel" Width="220px" CssClass="formfieldtitle"
					runat="server" BackColor="Transparent">RTD Temperature Range Maximum</FMControls:FMLabel>
			</td>
			<td>
				<FMControls:FMTextBox ID="RTDTempRangeMaxTB" TabIndex="2" Width="168px" CssClass="formfield"
					runat="server" MaxLength="30" Columns="30"></FMControls:FMTextBox>
			</td>
		</tr>
		<tr>
			<td>
				<FMControls:FMLabel ID="DefaultTemperatureLbl" Width="220px" CssClass="formfieldtitle"
					runat="server" BackColor="Transparent">Default Temperature</FMControls:FMLabel>
			</td>
			<td>
				<FMControls:FMTextBox ID="DefaultTemperatureTB" TabIndex="3" Width="168px" CssClass="formfield"
					runat="server" MaxLength="30" Columns="30"></FMControls:FMTextBox>
			</td>
		</tr>
	</table>
</body>
</html>
