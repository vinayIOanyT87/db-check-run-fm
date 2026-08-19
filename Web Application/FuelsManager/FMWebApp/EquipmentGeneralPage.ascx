<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="EquipmentGeneralPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EquipmentGeneralPage" %>
<html>
<head>
	<style>
		.divTable {
			width: 50%;
			height: 50%;
			display: table;
			padding: 3px;
		}

		.divTableRow {
			width: 100%;
			height: 100%;
			display: table-row;
			padding: 3px;
		}

		.divTableCell {
			width: 100%;
			height: 100%;
			display: table-cell;
			padding: 3px;
			vertical-align: top
		}
	</style>
</head>
	<SCRIPT>
		function CompanySelect(role, companyTextBoxId)
		{
		    var companyTextBox = document.getElementById(companyTextBoxId);

		    showModalDialogFrame({
		        url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&Unassigned=true",
		        width: 855,
		        height: 560,
		        onClose: function ()
		        {
		            if (this.returnValue != null)
		            {
		                var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
		                var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

		                companyTextBox.value = asciiValue1;
		                companyTextBox.title = asciiValue2;
		            }
		        }
		    });
		}

		function ProductSelect(productTextBoxId)
		{
		    var productTextBox = document.getElementById(productTextBoxId);

		    showModalDialogFrame({
		        url: "../FMWebApp/ProductSelectForm.aspx?Unassigned=true",
		        width: 855,
		        height: 560,
		        onClose: function ()
		        {
		            if (this.returnValue != null)
		            {
		                var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
		                var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

		                productTextBox.value = asciiValue1;
		                productTextBox.title = asciiValue2;
		            }
		        }
		    });
		}

		function FuelCardSelect(fuelCardTextBoxId)
		{
		    var fuelCardTextBox = document.getElementById(fuelCardTextBoxId);

		    showModalDialogFrame({
		        url: "../FMWebApp/FuelCardSelectForm.aspx?Unassigned=true",
		        width: 855,
		        height: 560,
		        onClose: function ()
		        {
		            if (this.returnValue != null)
		            {
		                var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
		                var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

		                fuelCardTextBox.value = asciiValue1;
		                fuelCardTextBox.title = asciiValue2;
		            }
		        }
		    });
		}
		function AssetTrackingDeviceSelect(assetTrackingDeviceTextBoxId) {
		    var assetTrackingDeviceTextBox = document.getElementById(assetTrackingDeviceTextBoxId);

		    showModalDialogFrame({
				url: "../FMWebApp/AssetTrackingDeviceSelectForm.aspx?Unassigned=true",
		      width: 855,
		      height: 560,
		      onClose: function ()
              {
                  if (this.returnValue != null)
                  {
                      var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
                      var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

                      assetTrackingDeviceTextBox.value = asciiValue1;
                      assetTrackingDeviceTextBox.title = asciiValue2;
                  }
				}
		    });
		}
    </SCRIPT>
<body>
	<div class="divTable">
		<div class="divTableRow">
			<div class="divTableCell">
				<FMControls:FMLabel ID="Label1"
					Style="z-index: 102; width: 78px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Equipment ID:</FMControls:FMLabel>
			</div>
			<div class="divTableCell">
				<FMControls:FMLabel ID="Label8" Style="z-index: 102" runat="server"
					Width="8px" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>				
			</div>
			<div class="divTableCell">
				<asp:TextBox ID="IDTextbox"
					Style="z-index: 104" runat="server" aria-required="true"
					MaxLength="30" Width="208px" CssClass="formfield" TabIndex="1"></asp:TextBox>
			</div>
			<div class="divTable">&nbsp;&nbsp;</div>
			<div class="divTableCell">
				<FMControls:FMLabel ID="GeneralPageProductLabel" Style="z-index: 109; width: 106px; height: 15px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Product:</FMControls:FMLabel>
			</div>
			<div class="divTableCell">
				<FMControls:FMProductTextBox ID="ProductTextBox" runat="server" Style="z-index: 146" aria-labelledby="GeneralPageProductLabel"
					TabIndex="13" Width="113px" CssClass="formfield" MaxLength="20" Enabled="True" AutoPostBack="True"></FMControls:FMProductTextBox>
			</div>
		</div>
		<div class="divTableRow">
			<div class="divTableCell">
				<FMControls:FMLabel ID="Label2"
					Style="z-index: 105; width: 88px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Description:</FMControls:FMLabel>
			</div>
			<div class="divTableCell"></div>
			<div class="divTableCell">
				<asp:TextBox ID="DescriptionTextbox" Style="z-index: 106"
					runat="server" Width="208px" CssClass="formfield" MaxLength="50" TabIndex="2"></asp:TextBox>
			</div>
			<div class="divTable">&nbsp;&nbsp;</div>
			<div class="divTableCell">
				<FMControls:FMLabel ID="GeneralPageFuelCardLabel" Style="z-index: 109; width: 116px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Fuel Card:</FMControls:FMLabel>
			</div>
			<div class="divTableCell">
				<FMControls:FMFuelCardTextBox ID="FuelCardTextBox" runat="server" Style="z-index: 146"  aria-labelledby="GeneralPageFuelCardLabel"
					TabIndex="14" Width="113px" CssClass="formfield" MaxLength="20" Enabled="True" AutoPostBack="True"></FMControls:FMFuelCardTextBox>
			</div>
		</div>
		<div class="divTableRow">
			<div class="divTableCell">
				<FMControls:FMLabel ID="Label4"
					Style="z-index: 108; width: 77px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Type:</FMControls:FMLabel>
			</div>
			<div class="divTableCell">
				<FMControls:FMLabel ID="FMLabel1" Style="z-index: 102" runat="server"
					Width="8px" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>				
			</div>
			<div class="divTableCell">
				<asp:DropDownList ID="EquipmentTypeDropDownList" Style="z-index: 109; max-width: 211px; -moz-min-width: 116px; -ms-min-width: 116px; -o-min-width: 116px; -webkit-min-width: 116px; min-width: 116px"
					runat="server" CssClass="formfield" AutoPostBack="True" TabIndex="3" aria-required="true"
					OnSelectedIndexChanged="EquipmentTypeDropDownListSelectedIndexChanged">
				</asp:DropDownList>
			</div>
			<div class="divTable">&nbsp;&nbsp;</div>
			<div class="divTableCell">
				<FMControls:FMLabel ID="Fmlabel12" Style="z-index: 109; height: 15px; width: 107px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Ref ID:</FMControls:FMLabel>
				<FMControls:FMLabel ID="RefIDRequiredSymbol" Style="z-index: 103" runat="server"
					Width="8px" Height="8px" ForeColor="Crimson" Visible="false">*</FMControls:FMLabel>
			</div>
			<div class="divTableCell">
				<asp:TextBox ID="RefIDTextbox" Style="z-index: 146" aria-required="true"
					TabIndex="15" runat="server" Width="113px" CssClass="formfield" MaxLength="20"
					Enabled="True"></asp:TextBox>
			</div>
		</div>
		<div class="divTableRow">
			<div class="divTableCell">
				<FMControls:FMLabel ID="MakeLabel"
					Style="z-index: 110; width: 86px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Make:</FMControls:FMLabel>
			</div>
			<div class="divTableCell"></div>
			<div class="divTableCell">
				<asp:TextBox ID="MakeTextbox" Style="z-index: 111"
					runat="server" MaxLength="20" Width="112px" CssClass="formfield" TabIndex="4"></asp:TextBox>
			</div>
			<div class="divTable">&nbsp;&nbsp;</div>
			<div class="divTableCell">
				<FMControls:FMCheckBox ID="LockedOutCheckBox" Style="z-index: 142; width: 171px;"
					TabIndex="16" runat="server" CssClass="formfieldtitle"
					Text="Equipment Locked Out" TextAlign="Left"
					AutoPostBack="True" OnCheckedChanged="LockedOutCheckBoxCheckedChanged"></FMControls:FMCheckBox>
			</div>
			<div class="divTableCell">&nbsp;</div>
		</div>
		<div class="divTableRow">
			<div class="divTableCell">
				<FMControls:FMLabel ID="ModelLabel"
					Style="z-index: 112; width: 86px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Model:</FMControls:FMLabel>
			</div>
			<div class="divTableCell"></div>
			<div class="divTableCell">
				<asp:TextBox ID="ModelTextbox" Style="z-index: 113"
					runat="server" MaxLength="50" Width="112px" CssClass="formfield" TabIndex="5"></asp:TextBox>
			</div>
			<div class="divTable">&nbsp;&nbsp;</div>
			<div class="divTableCell">
				<FMControls:FMLabel ID="Label18"
					Style="z-index: 143; width: 238px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Locked Out Reason:</FMControls:FMLabel>
			</div>
			<div class="divTableCell">
				<FMControls:FMTextBox ID="LockedOutReasonTextbox" Style="z-index: 144"
					TabIndex="17" runat="server" TextMode="MultiLine" Height="56px" Width="312px" CssClass="formfield"
					MaxLength="80" Columns="80" />
			</div>
		</div>
		<div class="divTableRow">
			<div class="divTableCell">
				<FMControls:FMLabel ID="YearLabel"
					Style="z-index: 115; width: 88px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Year:</FMControls:FMLabel>
			</div>
			<div class="divTableCell"></div>
			<div class="divTableCell">
				<asp:TextBox ID="YearTextbox" Style="z-index: 116"
					runat="server" MaxLength="4" Width="48px" CssClass="formfield" TabIndex="6"></asp:TextBox>
			</div>
			<div class="divTable">&nbsp;&nbsp;</div>
			<div class="divTableCell">
				<FMControls:FMLabel ID="Label19"
					Style="z-index: 145; width: 135px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Locked Out Date:</FMControls:FMLabel>
			</div>
			<div class="divTableCell">
				<asp:TextBox ID="LockedOutDateTextbox" Style="z-index: 146"
					TabIndex="18" runat="server" Width="113px" CssClass="formfield" MaxLength="20" Enabled="False"
					ReadOnly="True"></asp:TextBox>
			</div>
		</div>
		<div class="divTableRow">
			<div class="divTableCell">
				<FMControls:FMLabel ID="SerialNumLabel"
					Style="z-index: 134; width: 89px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Serial Number:</FMControls:FMLabel>
			</div>
			<div class="divTableCell"></div>
			<div class="divTableCell">
				<asp:TextBox ID="SerialNumberTextbox" Style="z-index: 135"
					runat="server" Width="208px" CssClass="formfield" TabIndex="7" MaxLength="30"></asp:TextBox>
			</div>
			<div class="divTable">&nbsp;&nbsp;</div>
			<div class="divTableCell">
				<FMControls:FMLabel ID="Fmlabel8" Style="z-index: 109; width: 134px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Equipment Card:</FMControls:FMLabel>
			</div>
			<div class="divTableCell">
				<asp:TextBox ID="CardTextbox" Style="z-index: 146"
					TabIndex="19" runat="server" Width="113px" CssClass="formfield" MaxLength="20"
					Enabled="True"></asp:TextBox>
			</div>
		</div>
		<div class="divTableRow">
			<div class="divTableCell">
				<FMControls:FMLabel ID="Fmlabel2"
					Style="z-index: 117; width: 85px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Fueling Type:</FMControls:FMLabel>
			</div>
			<div class="divTableCell"></div>
			<div class="divTableCell">
				<FMControls:FMDropDownList ID="FuelingTypeDropDownList" Style="z-index: 113"
					runat="server" MaxLength="50" Width="112px" CssClass="formfield" TabIndex="8"
					DataSource="<%# EnumerateFuelTypes() %>" DataTextField="Text"
					DataValueField="Value"
					OnSelectedIndexChanged="FuelingTypeSelectedIndexChanged" />
			</div>
			<div class="divTable">&nbsp;&nbsp;</div>
			<div class="divTableCell">
				<FMControls:FMLabel ID="GeneralPageCompanyLabel" Style="z-index: 109; width: 164px;" AssociatedControlID="CompanyTextBox"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Company:</FMControls:FMLabel>
			</div>
			<div class="divTableCell">
				<FMControls:FMCompanyTextBox ID="CompanyTextBox" Style="z-index: 110" aria-labelledby="GeneralPageCompanyLabel"
					runat="server" CssClass="formfield" Width="169px" TabIndex="20"></FMControls:FMCompanyTextBox>
			</div>
		</div>
		<div class="divTableRow">
			<div class="divTableCell">
				<FMControls:FMLabel ID="Fmlabel3"
					Style="z-index: 117; width: 91px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Volume:</FMControls:FMLabel>
			</div>
			<div class="divTableCell"></div>
			<div class="divTableCell">
				<asp:TextBox ID="VolumeTextbox" Style="z-index: 106; width: 111px"
					runat="server" CssClass="formfield" MaxLength="50" TabIndex="9"></asp:TextBox>
			</div>
			<div class="divTable">&nbsp;&nbsp;</div>
			<div class="divTableCell">
				<FMControls:FMLabel ID="Fmlabel7" Style="z-index: 109; width: 179px"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Company Equipment ID:</FMControls:FMLabel>
			</div>
			<div class="divTableCell">
				<asp:TextBox ID="CompanyEquipmentIDTextBox" Style="z-index: 119"
					runat="server" Width="168px" CssClass="formfield" TabIndex="21" MaxLength="30"></asp:TextBox>
			</div>
		</div>
		<div class="divTableRow">
			<div class="divTableCell">
				<FMControls:FMLabel ID="CapacityFmLabel"
					Style="z-index: 117; height: 16px; width: 91px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Capacity:</FMControls:FMLabel>
			</div>
			<div class="divTableCell"></div>
			<div class="divTableCell">
				<asp:TextBox ID="CapacityTextbox" Style="z-index: 106; width: 111px;"
					runat="server" CssClass="formfield" MaxLength="50" TabIndex="10"></asp:TextBox>
			</div>
			<div class="divTable">&nbsp;&nbsp;</div>
			<div class="divTableCell">
				<FMControls:FMLabel ID="AssetTrackingDeviceLabel" Style="z-index: 109; width: 164px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Asset Tracking Device:</FMControls:FMLabel>
			</div>
			<div class="divTableCell">
				<FMControls:FMAssetTrackingDeviceTextBox ID="AssetTrackingDeviceTextBox" Style="z-index: 110"
					runat="server" CssClass="formfield" Width="169px" TabIndex="20"></FMControls:FMAssetTrackingDeviceTextBox>
			</div>           
		</div>
		<div class="divTableRow">
			<div class="divTableCell">
				<FMControls:FMLabel ID="SafeFillFmLabel"
					Style="z-index: 117; height: 16px; width: 90px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">Safe Fill:</FMControls:FMLabel>
			</div>
			<div class="divTableCell"></div>
			<div class="divTableCell">
				<asp:TextBox ID="SafeFillTextbox" Style="z-index: 106; width: 111px;"
					runat="server" CssClass="formfield" MaxLength="50" TabIndex="11"></asp:TextBox>
			</div>
            <div class="divTableCell"></div>
            <div class="divTableCell">
				<FMControls:FMCheckBox ID="ScullyRequiredCheckBox" Style="z-index: 142; width: 171px;"
					TabIndex="22" runat="server" CssClass="formfieldtitle" 
					Text="Scully Required" TextAlign="Left"></FMControls:FMCheckBox>
			</div>
			<div class="divTable">&nbsp;&nbsp;</div>
			<div class="divTableCell">&nbsp;</div>
			<div class="divTableCell">&nbsp;</div>
		</div>
		<div class="divTableRow">
			<div class="divTableCell">
				<FMControls:FMCheckBox ID="HiddenCheckBox" TabIndex="12" TextAlign="Left" Text="Hidden"
					Width="120px" CssClass="formfieldtitle" runat="server"></FMControls:FMCheckBox>
			</div>          
			<div class="divTableCell"></div>
			<div class="divTableCell">&nbsp;</div>
			<div class="divTable">&nbsp;&nbsp;</div>
			<div class="divTableCell">&nbsp;</div>
			<div class="divTableCell">&nbsp;</div>
		</div>
	</div>
</body>
</html>
