<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileDCUSettingPage.ascx.cs" Inherits="FuelsManager.FMWebApp.ProfileDCUSettingPage" %>

<html>
	<head>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
		</head>
	<body>
		<asp:Panel ID="DCUPanel" CssClass="formfieldtitle" 
			GroupingText="DCU" runat="server"
			Style="Position:absolute; top:50px; left: 10px; height: 285px; width: 432px;">
			<table style="width:95%;">
				<tr>
					<td>				
						<FMControls:FMCheckBox ID="HasDcuCB" runat="server" CssClass="formfieldtitle" 
							Height="27px" Text="Has DCU" TextAlign="Right" width="100%" TabIndex="1" AutoPostBack="True" 
							oncheckedchanged="HasDcuCheckedChange" />
					</td>
					<td>				
					</td>
				</tr>
				<tr>
					<td>				
						<FMControls:FMCheckBox ID="BluetoothDcuCB" runat="server" 
							CssClass="formfieldtitle" Height="27px" Text="Bluetooth DCU" TextAlign="Right" 
							width="100%" TabIndex="2" />
					</td>
					<td>				
					</td>
				</tr>
				<tr>
					<td>				
						<FMControls:FMCheckBox ID="LogDcuActionsCB" runat="server" 
							CssClass="formfieldtitle" Height="27px" Text="Log DCU Actions" 
							TextAlign="Right" width="100%" TabIndex="3" />
					</td>
					<td>				
					</td>
				</tr>
				<tr>
					<td>				
						<FMControls:FMLabel ID="DcuComPortLB" runat="server" BackColor="Transparent" 
							CssClass="formfieldtitle" Width="100%">DCU COM Port</FMControls:FMLabel>
					</td>
					<td>
						<FMCONTROLS:FMDropDownList id="DcuComPortDD" tabIndex="12" TextAlign="Left" 
							Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td>				
						<FMControls:FMLabel ID="DcuReadyRetryLB" runat="server" BackColor="Transparent" 
							CssClass="formfieldtitle" Width="100%">DCU Read Retry</FMControls:FMLabel>
					</td>
					<td>				
						<FMControls:FMTextBox ID="DcuReadyRetryTB" runat="server" CssClass="formfield" 
							MaxLength="10" tabIndex="5" Width="151px"></FMControls:FMTextBox>
					</td>
				</tr>
				<tr>
					<td>				
						<FMControls:FMLabel ID="DcuDisconnectDelayLB" runat="server" 
							BackColor="Transparent" CssClass="formfieldtitle" Width="100%">DCU Disconnect Delay</FMControls:FMLabel>
					</td>
					<td>				
						<FMControls:FMTextBox ID="DcuDisconnectDelayTB" runat="server" 
							CssClass="formfield" MaxLength="10" tabIndex="6" Width="151px"></FMControls:FMTextBox>
					</td>
				</tr>
				<tr>
					<td>				
						<FMControls:FMLabel ID="DcuCommunicationFailRestartLbl" runat="server" 
							BackColor="Transparent" CssClass="formfieldtitle" Width="100%">DCU Communication Fail Restart</FMControls:FMLabel>
					</td>
					<td>				
						<FMControls:FMTextBox ID="DcuCommunicationFailRestartTB" runat="server" 
							CssClass="formfield" MaxLength="10" tabIndex="7" Width="151px"></FMControls:FMTextBox>
					</td>
				</tr>
			</table>
		</asp:Panel>
		<asp:Panel ID="AveryHardollPanel" CssClass="formfieldtitle" 
			GroupingText="Avery Hardoll" runat="server"
			Style="Position:absolute; top:50px; left: 475px; height: 285px; width: 432px;">
			<table style="width:95%;">
				<tr>
					<td>
						<FMControls:FMCheckBox ID="HasAveryHardollCB" runat="server" 
							CssClass="formfieldtitle" Height="27px" Text="Has Avery Hardoll" 
							TextAlign="Right" width="100%" TabIndex="8" AutoPostBack="True" 
							oncheckedchanged="HasAveryCheckedChange" />
					</td>
					<td>
					</td>
				</tr>
				<tr>
					<td>
						<FMControls:FMLabel ID="AveryHardollComPortLB" runat="server" 
							BackColor="Transparent" CssClass="formfieldtitle" Width="100%">Avery Hardoll COM Port</FMControls:FMLabel>
					</td>
					<td>
						<FMCONTROLS:FMDropDownList id="AveryHardollComPortDD" tabIndex="12" TextAlign="Left" 
							Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<FMControls:FMLabel ID="AveryHardollMeterIDLB" runat="server" 
							BackColor="Transparent" CssClass="formfieldtitle" Width="100%">Avery Hardoll Meter ID</FMControls:FMLabel>
					</td>
					<td>
						<FMControls:FMTextBox ID="AveryHardollMeterIDTB" runat="server" 
							CssClass="formfield" MaxLength="10" tabIndex="10" Width="151px"></FMControls:FMTextBox>
					</td>
				</tr>
			</table>
		</asp:Panel>
	</body>
</html>