<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileValidationRuleSettingPage.ascx.cs"
	Inherits="FuelsManager.FMWebApp.ProfileValidationRuleSettingPage" %>
<html>
<head>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<style type="text/css">
		.style6
		{
			width: 52px;
		}
		.style8
		{
			width: 52px;
			height: 31px;
		}
		.style10
		{
			width: 157px;
			height: 31px;
		}
		.style11
		{
			width: 181px;
		}
		.style12
		{
			height: 31px;
			width: 181px;
		}
		.style13
		{
			width: 149px;
		}
		.style14
		{
			height: 31px;
			width: 149px;
		}
		.style15
		{
			width: 152px;
		}
		.style16
		{
			width: 166px;
		}
		.style17
		{
			width: 125px;
		}
		.style18
		{
			width: 157px;
		}
	</style>
</head>
<body>
	<table style="z-index: 103; width: 844px; left: 5px; position: absolute; top: 50px;
		height: 91px;">
		<tr>
			<td class="style6">
				<FMControls:FMLabel ID="StrictUserValidationLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="223px">Strict User Validation</FMControls:FMLabel>
			</td>
			<td class="style11">
				<FMControls:FMDropDownList ID="StrictUserValidationDD" TabIndex="1" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
			<td class="style18">
				<FMControls:FMLabel ID="BypassDistributionToleranceLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Bypass Distribution Tolerance</FMControls:FMLabel>
			</td>
			<td class="style13">
				<FMControls:FMDropDownList ID="BypassDistributionToleranceDD" TabIndex="11" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
		</tr>
		<tr>
			<td class="style6">
				<FMControls:FMLabel ID="VerifyFuelingEquipmentLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="221px">Verify Fueling Equipment</FMControls:FMLabel>
			</td>
			<td class="style11">
				<FMControls:FMDropDownList ID="VerifyFuelingEquipmentDD" TabIndex="2" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
			<td class="style18">
				<FMControls:FMLabel ID="VehicleIdCheckLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Vehicle ID Check</FMControls:FMLabel>
			</td>
			<td class="style13">
				<FMControls:FMDropDownList ID="VehicleIdCheckDD" TabIndex="12" TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
		</tr>
		<tr>
			<td class="style6">
				<FMControls:FMLabel ID="AllowEditRequiredFuelLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Allow Edit of Required Fuel Load</FMControls:FMLabel>
			</td>
			<td class="style11">
				<FMControls:FMDropDownList ID="AllowEditRequiredFuelDD" TabIndex="3" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
			<td class="style18">
				<FMControls:FMLabel ID="GseFuelMustMatchLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">GSE Fuel Must Match</FMControls:FMLabel>
			</td>
			<td class="style13">
				<FMControls:FMDropDownList ID="GseFuelMustMatchDD" TabIndex="13" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
		</tr>
		<tr>
			<td class="style8">
				<FMControls:FMLabel ID="AllowBackAfterArrivalLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Allow Back After Arrival Screen</FMControls:FMLabel>
			</td>
			<td class="style12">
				<FMControls:FMDropDownList ID="AllowBackAfterArrivalDD" TabIndex="4" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
			<td class="style10">
				<FMControls:FMLabel ID="AllowManualMeterLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Allow Manual Meter</FMControls:FMLabel>
			</td>
			<td class="style14">
				<FMControls:FMDropDownList ID="AllowManualMeterDD" TabIndex="14" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
		</tr>
		<tr>
			<td class="style6">
				<FMControls:FMLabel ID="AllowBackAfterTicketLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Allow Back After Ticket Printed</FMControls:FMLabel>
			</td>
			<td class="style11">
				<FMControls:FMDropDownList ID="AllowBackAfterTicketDD" TabIndex="5" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
			<td class="style18">
				<FMControls:FMLabel ID="UseValidationLogicForGaTransactionLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="238px">Use Validation Logic for GA Transactions</FMControls:FMLabel>
			</td>
			<td class="style13">
				<FMControls:FMDropDownList ID="UseValidationLogicForGaTransactionDD" 
					TabIndex="15" TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
		</tr>
		<tr>
			<td class="style6">
				<FMControls:FMLabel ID="RequirePrintLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Require Print</FMControls:FMLabel>
			</td>
			<td class="style11">
				<FMControls:FMDropDownList ID="RequirePrintDD" TabIndex="6" TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
			<td class="style18">
				<FMControls:FMLabel ID="AllowShipNumberModificationLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Allow Ship Number Modification</FMControls:FMLabel>
			</td>
			<td class="style13">
				<FMControls:FMDropDownList ID="AllowShipNumberModificationDD" TabIndex="16" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
		</tr>
		<tr>
			<td class="style6">
				<FMControls:FMLabel ID="TotalFuelLoadCheckLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Total FuelLoad Check</FMControls:FMLabel>
			</td>
			<td class="style11">
				<FMControls:FMDropDownList ID="TotalFuelLoadCheckDD" TabIndex="7" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
			<td class="style18">
				<FMControls:FMLabel ID="AllowAircraftTypeModificationLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Allow Aircraft Type Modification</FMControls:FMLabel>
			</td>
			<td class="style13">
				<FMControls:FMDropDownList ID="AllowAircraftTypeModificationDD" TabIndex="17" TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
		</tr>
		<tr>
			<td class="style6">
				<FMControls:FMLabel ID="VolumetricThresholdValidationLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Volumetric Threshold Validation</FMControls:FMLabel>
			</td>
			<td class="style11">
				<FMControls:FMDropDownList ID="VolumetricThresholdValidationDD" TabIndex="8" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
			<td class="style18">
				<FMControls:FMLabel ID="AllowDestinationModificationLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Allow Destination Modification</FMControls:FMLabel>
			</td>
			<td class="style13">
				<FMControls:FMDropDownList ID="AllowDestinationModificationDD" TabIndex="18" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
		</tr>
		<tr>
			<td class="style6">
				<FMControls:FMLabel ID="ValidateShipNumberLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Validate Ship Number</FMControls:FMLabel>
			</td>
			<td class="style11">
				<FMControls:FMDropDownList ID="ValidateShipNumberDD" TabIndex="9" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
			<td class="style18">
				<FMControls:FMLabel ID="AllowVtoModificationLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Allow VTO Modification</FMControls:FMLabel>
			</td>
			<td class="style13">
				<FMControls:FMDropDownList ID="AllowVtoModificationDD" TabIndex="19" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
		</tr>
		<tr>
			<td class="style6">
				<FMControls:FMLabel ID="AllowFlightGateModificationLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="222px">Allow Flight Gate Modification</FMControls:FMLabel>
			</td>
			<td class="style11">
				<FMControls:FMDropDownList ID="AllowFlightGateModificationDD" TabIndex="10" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
			<td class="style18">
				<FMControls:FMLabel ID="OverrideWingBalancePercentVerficationLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="221px">Override wing balance % Variance</FMControls:FMLabel>
			</td>
			<td class="style13">
				<FMControls:FMDropDownList ID="OverrideWingBalancePercentVerficationDD" 
					TabIndex="20" TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server">
				</FMControls:FMDropDownList>
			</td>
		</tr>
	</table>
	<table style="z-index: 103; width: 843px; left: 5px; position: absolute; top: 400px;
		height: 91px;">
		<tr>
			<td class="style15">
				<FMControls:FMLabel ID="DestinationLbl" CssClass="formfieldtitle" runat="server"
					BackColor="Transparent" Width="220px">Destination</FMControls:FMLabel>
			</td>
			<td class="style16">
				<FMControls:FMDropDownList ID="DestinationDD" TabIndex="21" TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
					onselectedindexchanged="DestinationOnChange">
				</FMControls:FMDropDownList>
			</td>
			<td>
				<FMControls:FMCheckBox ID="EaDestinationCB" runat="server" Text="EA" 
					TextAlign="Right" Height="27px" CssClass="formfieldtitle" TabIndex="22"/>
			</td>
			<td class="style17">
				<FMControls:FMLabel ID="GateLbl" CssClass="formfieldtitle" runat="server"
					BackColor="Transparent" Width="160px">Gate</FMControls:FMLabel>
			</td>
			<td class="style18">
				<FMControls:FMDropDownList ID="GateDD" TabIndex="32" TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
					onselectedindexchanged="GateOnChange">
				</FMControls:FMDropDownList>
			</td>
			<td>
				<FMControls:FMCheckBox ID="EaGateCB" runat="server" Text="EA" TextAlign="Right" 
					Height="27px" CssClass="formfieldtitle" TabIndex="33"/>
			</td>
		</tr>
		<tr>
			<td class="style15">
				<FMControls:FMLabel ID="TicketPrintingLB" CssClass="formfieldtitle" runat="server"
					BackColor="Transparent" Width="160px">Ticket Printing</FMControls:FMLabel>
			</td>
			<td class="style16">
				<FMControls:FMDropDownList ID="TicketPrintingDD" TabIndex="23" TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
					onselectedindexchanged="TicketPrintingOnChange">
				</FMControls:FMDropDownList>
			</td>
			<td>
				<FMControls:FMCheckBox ID="EaTicketPrintingCB" runat="server" Text="EA" 
					TextAlign="Right" Height="27px" CssClass="formfieldtitle" TabIndex="24"/>
			</td>
			<td class="style17">
				<FMControls:FMLabel ID="MeterTotalLB" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
							Width="95%">Meter Total</FMControls:FMLabel>
			</td>
			<td class="style18">
				<FMControls:FMDropDownList ID="MeterTotalDD" TabIndex="34" TextAlign="Left" Height="27px"
								Width="160px" CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
					onselectedindexchanged="MeterTotalOnChange">
				</FMControls:FMDropDownList>
			</td>
			<td>
				<FMControls:FMCheckBox ID="EaMeterTotalCB" runat="server" Text="EA" 
					TextAlign="Right" Height="27px" CssClass="formfieldtitle" TabIndex="35"/>
			</td>
		</tr>
		<tr>
			<td class="style15">
				<FMControls:FMLabel ID="AircraftTypeVerificationLB" CssClass="formfieldtitle" runat="server"
					BackColor="Transparent" Width="160px">Aircraft Type Verification</FMControls:FMLabel>
			</td>
			<td class="style16">
				<FMControls:FMDropDownList ID="AircraftTypeVerificationDD" TabIndex="25" TextAlign="Left"
					Height="27px" Width="160px" CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
					onselectedindexchanged="AircraftTypeOnChange">
				</FMControls:FMDropDownList>
			</td>
			<td>
				<FMControls:FMCheckBox ID="EaAircraftTypeVerificationCB" runat="server" 
					Text="EA" TextAlign="Right" Height="27px" CssClass="formfieldtitle" 
					TabIndex="26"/>
			</td>
			<td class="style17">
				<FMControls:FMLabel ID="VolumePumpedLB" CssClass="formfieldtitle" runat="server"
								BackColor="Transparent" Width="95%">Volume Pumped</FMControls:FMLabel>
			</td>
			<td class="style18">
				<FMControls:FMDropDownList ID="VolumePumpedDD" TabIndex="36" TextAlign="Left" Height="27px"
								Width="160px" CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
					onselectedindexchanged="VolumePumpedOnChange">
				</FMControls:FMDropDownList>
			</td>
			<td>
				<FMControls:FMCheckBox ID="EaVolumePumpedCB" runat="server" Text="EA" 
					TextAlign="Right" Height="27px" CssClass="formfieldtitle" TabIndex="37"/>
			</td>
		</tr>
		<tr>
			<td class="style15">
				<FMControls:FMLabel ID="ShipNumberLB" CssClass="formfieldtitle" runat="server" BackColor="Transparent"
					Width="160px">Ship Number</FMControls:FMLabel>
			</td>
			<td class="style16">
				<FMControls:FMDropDownList ID="ShipNumberDD" TabIndex="27" TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
					onselectedindexchanged="ShipNumberOnChange">
				</FMControls:FMDropDownList>
			</td>
			<td>
				<FMControls:FMCheckBox ID="EaShipNumberCB" runat="server" Text="EA" 
					TextAlign="Right" Height="27px" CssClass="formfieldtitle" TabIndex="28"/>
			</td>
			<td class="style17">
				<FMControls:FMLabel ID="TankCapacityLB" CssClass="formfieldtitle" runat="server"
								BackColor="Transparent" Width="95%">Tank Capacity</FMControls:FMLabel>
			</td>
			<td class="style18">
				<FMControls:FMDropDownList ID="TankCapacityDD" TabIndex="38" TextAlign="Left" Height="27px"
								Width="160px" CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
					onselectedindexchanged="TankCapacityOnChange">
				</FMControls:FMDropDownList>
			</td>
			<td>
				<FMControls:FMCheckBox ID="EaTankCapacityCB" runat="server" Text="EA" 
					TextAlign="Right" Height="27px" CssClass="formfieldtitle" TabIndex="39"/>
			</td>
		</tr>
		<tr>
			<td class="style15">
				<FMControls:FMLabel ID="CheckTanksDifferenceLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="223px">Tank Position Balance Verification</FMControls:FMLabel>
			</td>
			<td class="style16">
				<FMControls:FMDropDownList ID="CheckTanksDifferenceDD" TabIndex="29" 
					TextAlign="Left" Height="27px"
					Width="160px" CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
					onselectedindexchanged="TankPosBalanceOnChange">
				</FMControls:FMDropDownList>
			</td>
			<td>
				<FMControls:FMCheckBox ID="EaCheckTanksDifferenceCB" runat="server" Text="EA" 
					TextAlign="Right" Height="27px" CssClass="formfieldtitle" TabIndex="30"/>
			</td>
			<td class="style17">
				<FMControls:FMLabel ID="TankPositionBalanceLB" CssClass="formfieldtitle" 
					runat="server" BackColor="Transparent"
						Height="27px" Width="120px" TabIndex="31">Tank Position Balance %</FMControls:FMLabel>
				</td>
			<td class="style18">
				<FMControls:FMTextBox ID="TankPositionBalanceTB" TabIndex="2" Width="40px" 
					CssClass="formfield" runat="server"
								MaxLength="5" Columns="5"></FMControls:FMTextBox>
			</td>
			<td></td>
		</tr>
	</table>
</body>
</html>
