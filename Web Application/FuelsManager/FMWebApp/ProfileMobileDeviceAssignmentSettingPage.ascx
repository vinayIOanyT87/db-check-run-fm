<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileMobileDeviceAssignmentSettingPage.ascx.cs" Inherits="FuelsManager.FMWebApp.ProfileMobileDeviceAssignmentSettingPage" %>

<html>
	<head>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
		<style type="text/css">
			 .style9
			{
				width: 197px;
				height: 10px;
			}
			.style12
			{
				width: 200px;
				height: 10px;
			}
			.style13
			{
				width: 38px;
			}
			</style>
	</head>
	<body>
		<table style="Z-INDEX: 104; width:59%; LEFT: 5px; POSITION: absolute; TOP: 50px; height: 32px;">
			<tr>
				<td>
					<FMCONTROLS:FMLABEL id="AssignedMobileDeviceLB" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
						Width="100%">Assigned Mobile Devices</FMCONTROLS:FMLABEL>
				</td>
				<td class="style13">
				&nbsp;
				</td>
				<td>
					<FMCONTROLS:FMLABEL id="UnassignedMobileDeviceLB" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
						Width="100%">Unassigned Mobile Devices</FMCONTROLS:FMLABEL>
				</td>
			</tr>
			<tr>
				<td>
					<FMControls:FMListBox ID="AssignedListBox" runat="server" Height="203px" 
						Width="100%" SelectionMode="Multiple">
					</FMControls:FMListBox>
				</td>
				<td class="style13" style="text-align: center">
					<FMControls:FMButton ID="AssignBtn" runat="server" Text="<<" 
						onclick="AssignedButtonOnClick" />
					<br /><br />
					<FMControls:FMButton ID="UnassignBtn" runat="server" Text=">>" 
						onclick="UnassignedButtonOnClick" />
				</td>
				<td>
					<FMControls:FMListBox ID="UnassignedListBox" runat="server" Height="203px" 
						Width="100%" SelectionMode="Multiple">
					</FMControls:FMListBox>
				</td>
			</tr>
		</table>
	</body>
</html>