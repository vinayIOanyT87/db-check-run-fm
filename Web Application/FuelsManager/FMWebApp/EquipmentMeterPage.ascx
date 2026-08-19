<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EquipmentMeterPage.ascx.cs" Inherits="FuelsManager.FMWebApp.EquipmentMeterPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

	<style type="text/css">
        .auto-style1 {
            width: 166px;
        }
        .formfieldcb input[type=checkbox],
        .table td input[type=radio]
        {
	        margin-left: 0 !important;
        }
        #tcEquipment_tpMeterPage_EquipmentMeterPage_MeterConfigRadioGroup > tbody > tr > td > label
        {
            vertical-align: middle;
            margin-right: 10px;
        }
        #tcEquipment_tpMeterPage > table > tbody > tr > td > div > table > tbody > tr > td
        {
            border-top:unset;
            vertical-align:middle;
        }
	</style>

	<table style="z-index:110; left:0px; top: 10px; position:absolute" cellpadding="5">
		<tr>
			<td colspan="4">
				<div style="width:600px;height:210px;border:1px solid #000;">
                    <table class="table" width="100%" style="width: 590px; table-layout: fixed; margin-top: 10px; margin-bottom: 10px; margin-left: 10px;">
                        <tr>
                            <td colspan="4" style="width: 122px;">
                                <FMControls:FMLabel ID="MeterConfigLabel" AssociatedControlID="MeterConfigRadioGroup" runat="server" 
                                   CssClass="formfieldtitle"  Text="Meter Config" /><span style="color: red; width: 3px">*</span>
                            </td>
                            <td colspan="16">
                                <FMControls:FMRadioButtonList ID="MeterConfigRadioGroup" runat="server" 
                                    AutoPostBack="True" RepeatDirection="Horizontal" TabIndex="1" >
                                    <asp:ListItem>None</asp:ListItem>
                                    <asp:ListItem>Single</asp:ListItem>
                                    <asp:ListItem>Dual</asp:ListItem>
                                </FMControls:FMRadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="10">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="MeterIDLabel" AssociatedControlID="MeterIDTextBox" runat="server" CssClass="formfieldtitle" Text="Meter ID" /><span style="color: red; width: 3px">*</span>
                                        </td>
                                        <td>
                                            <FMControls:FMTextBox ID="MeterIDTextBox" Enabled="false" CssClass="formfield" runat="server" MaxLength="30" TabIndex="2" aria-required="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="NumberOfDigitsLabel" AssociatedControlID="NumberOfDigitsTextBox" runat="server" CssClass="formfieldtitle" Text="Number of Digits" /><span style="color: red; width: 3px">*</span>
                                        </td>
                                        <td>
                                            <FMControls:FMTextBox ID="NumberOfDigitsTextBox" Enabled="false" CssClass="formfield" runat="server" MaxLength="2" TabIndex="3" Width="30px" aria-required="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="RotatesBackwardLabel" AssociatedControlID="RotatesBackwardCheckBox" runat="server" CssClass="formfieldtitle" Text="Rotates Backwards" />
                                        </td>
                                        <td>
                                            <FMControls:FMCheckBox ID="RotatesBackwardCheckBox" Enabled="false" CssClass="formfieldcb" runat="server" TabIndex="4" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="ReceiptMeterLabel" AssociatedControlID="ReceiptMeterCheckBox" runat="server" CssClass="formfieldtitle" Text="Receipt Meter" />
                                        </td>
                                        <td>
                                            <FMControls:FMCheckBox ID="ReceiptMeterCheckBox" Enabled="false" CssClass="formfieldcb" runat="server" TabIndex="5" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="MeterFactorLabel" AssociatedControlID="MeterFactorTextBox" runat="server" CssClass="formfieldtitle" Text="Meter Factor" /><span style="color: red; width: 3px">*</span>
                                        </td>
                                        <td>
                                            <FMControls:FMTextBox ID="MeterFactorTextBox" Enabled="false" CssClass="formfield" runat="server" MaxLength="8" TabIndex="6" Width="60px" aria-required="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="FuelCompressionLabel" AssociatedControlID="FuelCompressionTextBox" runat="server" CssClass="formfieldtitle" Text="Fuel CP" /><span style="color: red; width: 3px">*</span>
                                        </td>
                                        <td>
                                            <FMControls:FMTextBox ID="FuelCompressionTextBox" Enabled="false" CssClass="formfield" runat="server" MaxLength="8" TabIndex="7" Width="60px" aria-required="true" />
                                        </td>
                                    </tr>
                                </table>
                            </td>

                            <td colspan="10">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="MeterID2Label" AssociatedControlID="MeterID2TextBox" runat="server" CssClass="formfieldtitle" Text="Second Meter ID" /><span style="color: red; width: 3px">*</span>
                                        </td>
                                        <td>
                                            <FMControls:FMTextBox ID="MeterID2TextBox" Enabled="false" CssClass="formfield" runat="server" MaxLength="30" TabIndex="8" aria-required="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="NumberOfDigits2Label" AssociatedControlID="NumberOfDigits2TextBox" runat="server" CssClass="formfieldtitle" Text="Number of Digits" /><span style="color: red; width: 3px">*</span>
                                        </td>
                                        <td>
                                            <FMControls:FMTextBox ID="NumberOfDigits2TextBox" Enabled="false" CssClass="formfield" runat="server" MaxLength="2" TabIndex="9" Width="30px" aria-required="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="RotatesBackward2Label" AssociatedControlID="RotatesBackward2CheckBox" runat="server" CssClass="formfieldtitle" Text="Rotates Backwards" />
                                        </td>
                                        <td>
                                            <FMControls:FMCheckBox ID="RotatesBackward2CheckBox" Enabled="false" CssClass="formfieldcb" runat="server" TabIndex="10" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="ReceiptMeter2Label" AssociatedControlID="ReceiptMeter2CheckBox" runat="server" CssClass="formfieldtitle" Text="Receipt Meter" Width="90px" />
                                        </td>
                                        <td>
                                            <FMControls:FMCheckBox ID="ReceiptMeter2CheckBox" Enabled="false" CssClass="formfieldcb" runat="server" TabIndex="11" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="MeterFactor2Label" AssociatedControlID="MeterFactor2TextBox" runat="server" CssClass="formfieldtitle" Text="Meter Factor" /><span style="color: red; width: 3px">*</span>
                                        </td>
                                        <td>
                                            <FMControls:FMTextBox ID="MeterFactor2TextBox" Enabled="false" CssClass="formfield" runat="server" MaxLength="8" TabIndex="12" Width="60px" aria-required="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="FuelCompression2Label" AssociatedControlID="FuelCompression2TextBox" runat="server" CssClass="formfieldtitle" Text="Fuel CP" /><span style="color: red; width: 3px">*</span>
                                        </td>
                                        <td>
                                            <FMControls:FMTextBox ID="FuelCompression2TextBox" Enabled="false" CssClass="formfield" runat="server" MaxLength="8" TabIndex="13" Width="60px" aria-required="true" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
				</div>
			</td>
		</tr>

		<tr>
			<td colspan="4">
				<div style="width:600px;height:180px;border:1px solid #000; margin-top: 5px;">
                    <table class="table" width="100%" style="width: 590px; table-layout: fixed; margin-top: 10px; margin-bottom: 10px; margin-left: 10px;">
						<tr>
							<td style="width: 47px;">
								<FMControls:FMLabel ID="HasDcuLabel" AssociatedControlID="HasDcuCheckBox" runat="server" CssClass="formfieldtitle" Text="DCU Config" /> 
							</td>
							<td class="auto-style1" colspan="3">
								<FMControls:FMCheckBox ID="HasDcuCheckBox" Checked="false" CssClass="formfieldcb" runat="server"  AutoPostBack="True" TabIndex="14" Enabled="false"/>
							</td>
						</tr>
						<tr>
							<td colspan="4">
								<table>
									<tr>
										<td>
											<FMControls:FMLabel ID="DcuIDLabel" AssociatedControlID="DcuIDTextBox" runat="server" CssClass="formfieldtitle" Text="DCU ID" />				
										</td>
										<td class="auto-style1">
											<FMControls:FMTextBox ID="DcuIDTextBox" Enabled="false" CssClass="formfield"  runat="server" MaxLength="60"  TabIndex="15"/>
										</td>
										<td>
											<FMControls:FMLabel ID="DcuVoltsLabel" AssociatedControlID="DcuVoltsTextBox" runat="server" CssClass="formfieldtitle" Text="Battery Volts" />
										</td>
										<td class="auto-style1">
											<FMControls:FMTextBox ID="DcuVoltsTextBox" Enabled="false" CssClass="formfield"  runat="server" MaxLength="8"  Width="60px" TabIndex="16" />
										</td>
									</tr>
									<tr>
										<td>
											<FMControls:FMLabel ID="DcuAmpsLabel" AssociatedControlID="DcuAmpsTextBox" runat="server" CssClass="formfieldtitle" Text="Battery Amps"/>
										</td>
										<td class="auto-style1">
											<FMControls:FMTextBox ID="DcuAmpsTextBox" Enabled="false" CssClass="formfield"  runat="server" MaxLength="9" Width="60px"  TabIndex="17" />
										</td>
										<td>
											<FMControls:FMLabel ID="DcuTemperatureLabel" AssociatedControlID="DcuTemperatureTextBox" runat="server" CssClass="formfieldtitle" Text="DCU Temperature C&#176" />
										</td>
										<td class="auto-style1">
											<FMControls:FMTextBox ID="DcuTemperatureTextBox" Enabled="false" CssClass="formfield"  runat="server" MaxLength="10" Width="60px" TabIndex="18" />
										</td>

									</tr>

									<tr>
										<td>
											<FMControls:FMLabel ID="DcuResetsLabel" AssociatedControlID="DcuResetsTextBox" runat="server" CssClass="formfieldtitle" Text="DCU Resets"/>
										</td>
										<td class="auto-style1">
											<FMControls:FMTextBox ID="DcuResetsTextBox" Enabled="false" CssClass="formfield"  runat="server" MaxLength="8" Width="60px" TabIndex="19" />
										</td>
										<td>
											<FMControls:FMLabel ID="DcuUpdatedLabel" AssociatedControlID="DcuUpdatedTextBox" runat="server" CssClass="formfieldtitle" Text="Date Updated"/>
										</td>
										<td class="auto-style1">
											<FMControls:FMTextBox ID="DcuUpdatedTextBox" Enabled="false" CssClass="formfield"  runat="server" MaxLength="12" Width="60px" TabIndex="20" />
										</td>

									</tr>

									<tr>
										<td>
											<FMControls:FMLabel ID="DcuConfigurationDateLabel" AssociatedControlID="DcuConfigurationDateTextBox" runat="server" CssClass="formfieldtitle" Text="DCU Configuration Date"/>
										</td>
										<td class="auto-style1">
											<FMControls:FMTextBox ID="DcuConfigurationDateTextBox" Enabled="false" CssClass="formfield"  runat="server" MaxLength="12" TabIndex="21" />
										</td>
										<td>
											<FMControls:FMLabel ID="DcuFirmwareVersionLabel" AssociatedControlID="DcuFirmwareVersionTextBox" runat="server" CssClass="formfieldtitle" Text="DCU Firmware Version"/>
										</td>
										<td class="auto-style1">
											<FMControls:FMTextBox ID="DcuFirmwareVersionTextBox" Enabled="false" CssClass="formfield"  runat="server" MaxLength="50" TabIndex="22" />
										</td>

									</tr>
									<tr>
										<td>
											<FMControls:FMLabel ID="DcuBluetoothAddressLabel" AssociatedControlID="DcuBluetoothAddressTextBox" runat="server" CssClass="formfieldtitle" Text="DCU Bluetooth Address" />
										</td>
										<td class="auto-style1">
											<FMControls:FMTextBox ID="DcuBluetoothAddressTextBox" Enabled="false" CssClass="formfield"  runat="server" MaxLength="20" TabIndex="23" />
										</td>
									</tr>
								</table>
						    </td>
						</tr>
					</table>
				</div>
			</td>
		</tr>
	</table>


