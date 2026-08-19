<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="EquipmentAirplaneGeneralPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EquipmentAirplaneGeneralPage" %>
<script>
    function CompanySelect(role, companyTextBoxId) {
        var companyTextBox = document.getElementById(companyTextBoxId);

        showModalDialogFrame({
            url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&Unassigned=true",
            width: 855,
            height: 560,
            onClose: function () {
                if (this.returnValue != null) {
                    var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
                    var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

                    companyTextBox.value = asciiValue1;
                    companyTextBox.title = asciiValue2;
                }
            }
        });
    }

    function FuelCardSelect(fuelCardTextBoxId) {
        var fuelCardTextBox = document.getElementById(fuelCardTextBoxId);

        showModalDialogFrame({
            url: "../FMWebApp/FuelCardSelectForm.aspx?Unassigned=true",
            width: 855,
            height: 560,
            onClose: function () {
                if (this.returnValue != null) {
                    var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
                    var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

                    fuelCardTextBox.value = asciiValue1;
                    fuelCardTextBox.title = asciiValue2;
                }
            }
        });
    }
    function ProductSelect(productTextBoxId) {
        var productTextBox = document.getElementById(productTextBoxId);

        showModalDialogFrame({
            url: "../FMWebApp/ProductSelectForm.aspx?Unassigned=true",
            width: 855,
            height: 560,
            onClose: function () {
                if (this.returnValue != null) {
                    var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
                    var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

                    productTextBox.value = asciiValue1;
                    productTextBox.title = asciiValue2;
                }
            }
        });
    }
</script>
<table>
    <tr>
        <td>
            <FMControls:FMLabel ID="TailIdLabel"
                Style="z-index: 102;"
                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Tail ID:</FMControls:FMLabel>
            <FMControls:FMLabel ID="Label8" Style="z-index: 103;" runat="server"
                Width="8px" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>
        </td>
        <td>
            <asp:TextBox ID="IDTextbox"
                Style="z-index: 104;" runat="server" aria-required="true"
                MaxLength="30" Width="208px" CssClass="formfield" TabIndex="1"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="DescriptionLabel"
                Style="z-index: 105;"
                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Description:</FMControls:FMLabel>
        </td>
        <td>
            <asp:TextBox ID="DescriptionTextbox" Style="z-index: 106;"
                runat="server" Width="208px" CssClass="formfield" MaxLength="50" TabIndex="2"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="TypeLabel"
                Style="z-index: 108;"
                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Type:</FMControls:FMLabel>
        </td>
        <td>
            <asp:DropDownList ID="EquipmentTypeDropDownList" Style="z-index: 109;"
                runat="server" Width="114px" CssClass="formfield" AutoPostBack="True" TabIndex="3"
                OnSelectedIndexChanged="EquipmentTypeDropDownListSelectedIndexChanged">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="ShipNumLabel"
                Style="z-index: 134;"
                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Ship Number:</FMControls:FMLabel>
        </td>
        <td>
            <asp:TextBox ID="SerialNumberTextbox" Style="z-index: 135;"
                runat="server" Width="208px" CssClass="formfield" TabIndex="4" MaxLength="30"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="MakeLabel"
                Style="z-index: 110; width: 86px;"
                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Make:</FMControls:FMLabel>
        </td>
        <td>
            <asp:TextBox ID="MakeTextbox" Style="z-index: 111;"
                runat="server" MaxLength="20" Width="112px" CssClass="formfield" TabIndex="5"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="ModelLabel"
                Style="z-index: 112; width: 86px;"
                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Model:</FMControls:FMLabel>
        </td>
        <td>
            <asp:TextBox ID="ModelTextbox" Style="z-index: 113;"
                runat="server" MaxLength="50" Width="112px" CssClass="formfield" TabIndex="7"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="YearLabel"
                Style="z-index: 115; width: 88px;"
                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Year:</FMControls:FMLabel>
        </td>
        <td>
            <asp:TextBox ID="YearTextbox" Style="z-index: 116;"
                runat="server" MaxLength="4" Width="48px" CssClass="formfield" TabIndex="8"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="ConsumerLabel" Style="z-index: 109; width: 164px;"
                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Company:</FMControls:FMLabel>
        </td>
        <td>
            <FMControls:FMCompanyTextBox ID="CompanyTextBox" Style="z-index: 110;"
                runat="server" CssClass="formfield" Width="169px" TabIndex="9"></FMControls:FMCompanyTextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="ProductLabel" Style="z-index: 109; width: 106px; height: 15px;"
                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Product:</FMControls:FMLabel>
        </td>
        <td>
            <FMControls:FMProductTextBox ID="ProductTextBox" runat="server" Style="z-index: 146;"
                TabIndex="10" Width="113px" CssClass="formfield" MaxLength="20" Enabled="True" AutoPostBack="True"></FMControls:FMProductTextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="FuelCardLabel" Style="z-index: 109; width: 116px;"
                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Fuel Card:</FMControls:FMLabel>
        </td>
        <td>
            <FMControls:FMFuelCardTextBox ID="FuelCardTextBox" runat="server" Style="z-index: 146;"
                TabIndex="11" Width="113px" CssClass="formfield" MaxLength="20" Enabled="True" AutoPostBack="True"></FMControls:FMFuelCardTextBox>
        </td>
    </tr>
    <tr>
        <td>
            <FMControls:FMLabel ID="CardLabel" Style="z-index: 109; width: 134px;"
                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Card:</FMControls:FMLabel>
        </td>
        <td>
            <asp:TextBox ID="CardTextbox" Style="z-index: 146;"
                TabIndex="12" runat="server" Width="113px" CssClass="formfield" MaxLength="20"
                Enabled="True"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <FMControls:FMCheckBox ID="HiddenCheckBox" TabIndex="11" TextAlign="Left" Text="Hidden"
                Width="120px" CssClass="formfieldtitle" runat="server"></FMControls:FMCheckBox>
        </td>
    </tr>
</table>
