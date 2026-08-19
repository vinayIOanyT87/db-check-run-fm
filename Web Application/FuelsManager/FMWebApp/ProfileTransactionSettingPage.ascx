<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileTransactionSettingPage.ascx.cs" Inherits="FuelsManager.FMWebApp.ProfileTransactionSettingPage" %>

<html>
	<head>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
		<style type="text/css">
			.style1
			{
				width: 159px;
			}
		</style>
	</head>
	<body>
		<asp:Panel ID="TxAssociationPanel" CssClass="formfieldtitle" 
			GroupingText="Transaction Association" runat="server" Width="300px"
			Style="Position:absolute; top:50px; left: 10px; height: 175px;">
			<table style="width:95%;">
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="IssueTransactionLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Issue Transaction</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMCONTROLS:FMDropDownList id="IssueTransactionDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="DefuelTransactionLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Defuel Transaction</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMCONTROLS:FMDropDownList id="DefuelTransactionDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="RotationTransactionLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Rotation Transaction</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMCONTROLS:FMDropDownList id="RotationTransactionDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="MeterCloseoutLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Meter Closeout</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMCONTROLS:FMDropDownList id="MeterCloseoutDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="DeIceTransactionLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">De-Ice Transaction</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMCONTROLS:FMDropDownList id="DeIceTransactionDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<FMCONTROLS:FMLABEL id="GSETransactionLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">GSE Transaction</FMCONTROLS:FMLABEL>
					</td>
					<td>
						<FMCONTROLS:FMDropDownList id="GSETransactionDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
			</table>
		</asp:Panel>
		<asp:Panel ID="DefaultMeterCloseout" CssClass="formfieldtitle" 
			GroupingText="Default Value for Meter Closeout" runat="server" Width="300px"
			Style="Position:absolute; top:300px; left: 10px; height: 100px;">
			<table style="width:95%;">
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="ConsumerCloseoutLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Consumer</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMCONTROLS:FMDropDownList id="ConsumerCloseoutDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="OwnerLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Owner</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMCONTROLS:FMDropDownList id="OwnerDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="VenderLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Vendor</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMCONTROLS:FMDropDownList id="VendorCloseoutDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
			</table>
		</asp:Panel>
		<asp:Panel ID="DefaultManualTransaction" CssClass="formfieldtitle" 
			GroupingText="Default Value for Manual Transaction" runat="server"
			Style="Position:absolute; top:50px; left: 359px; height: 326px; width: 350px;">
			<table style="width:95%; height: 297px;">
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="ConsumerManualLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Consumer</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMCONTROLS:FMDropDownList id="ConsumerManualDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<FMCONTROLS:FMLABEL id="ShipperLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Shipper</FMCONTROLS:FMLABEL>
					</td>
					<td>
						<FMCONTROLS:FMDropDownList id="ShipperDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<FMCONTROLS:FMLABEL id="ManagerLbl" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Manager</FMCONTROLS:FMLABEL>
					</td>
					<td>
						<FMCONTROLS:FMDropDownList id="ManagerDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<FMCONTROLS:FMLABEL id="SupplierLbl" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Supplier</FMCONTROLS:FMLABEL>
					</td>
					<td>
						<FMCONTROLS:FMDropDownList id="SupplierDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td>
						<FMCONTROLS:FMLABEL id="BillToLbl" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Bill To</FMCONTROLS:FMLABEL>
					</td>
					<td>
						<FMCONTROLS:FMDropDownList id="BillToDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="ProductLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Product</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMCONTROLS:FMDropDownList id="ProductDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="VendorManualLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Vendor</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMCONTROLS:FMDropDownList id="VendorManualDD" tabIndex="17" TextAlign="Left" Height="27px" Width="160px" 
							CssClass="formfieldtitle" runat="server"></FMCONTROLS:FMDropDownList>
					</td>
				</tr>
				<tr>
					<td colspan="2" align="left">
						<FMCONTROLS:FMCHECKBOX id="InhibitOverridingTemperatureCB" TextAlign="Right" 
							Text="Inhibit overriding temperature" Height="27px" 
							width="200px" CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
							oncheckedchanged="InhibitTempCheckedChanged"></FMCONTROLS:FMCHECKBOX>
					</td>
				</tr>
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="TemperatureLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Temperature</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMControls:FMTextBox id="TemperatureTB" tabIndex="2" Width="160px" CssClass="formfield" runat="server" 
						MaxLength="10" Columns="10"></FMControls:FMTextBox>
					</td>
				</tr>
				<tr>
					<td >
						<FMCONTROLS:FMLABEL id="DensityLB" CssClass="formfieldtitle" 
							runat="server" BackColor="Transparent"  Width="160px">Density</FMCONTROLS:FMLABEL>
					</td>
					<td >
						<FMControls:FMTextBox id="DensityTB" tabIndex="2" Width="160px" CssClass="formfield" runat="server" 
						MaxLength="10" Columns="10"></FMControls:FMTextBox>
					</td>
				</tr>
			</table>
		</asp:Panel>
	</body>
</html>