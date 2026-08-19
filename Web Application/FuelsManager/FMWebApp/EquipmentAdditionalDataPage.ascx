<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" CodeBehind="EquipmentAdditionalDataPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EquipmentAdditionalDataPage" %>
<html>
<head>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
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
</script>
<body>
    <FMControls:FMLabel ID="Label1" AssociatedControlID="RatedGPMTextbox" Style="z-index: 102; left: 0px; position: absolute; top: 16px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Rated GPM:</FMControls:FMLabel>
    <asp:TextBox ID="RatedGPMTextbox" Style="z-index: 104; left: 150px; position: absolute; top: 14px;" runat="server"
        MaxLength="30" Width="208px" CssClass="formfield"></asp:TextBox>
    <FMControls:FMLabel ID="Label2" AssociatedControlID="ActualGPMTextbox" Style="z-index: 105; left: 0px; position: absolute; top: 48px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Actual GPM:</FMControls:FMLabel>
    <asp:TextBox ID="ActualGPMTextbox" Style="z-index: 106; left: 150px; position: absolute; top: 48px"
        runat="server" Width="208px" CssClass="formfield" MaxLength="30" Columns="30"></asp:TextBox>

    <FMControls:FMLabel ID="FMLabel4" AssociatedControlID="IssptTextbox" Style="z-index: 112; left: 0px; position: absolute; top: 82px"
        runat="server" CssClass="formfieldtitle" BackColor="Transparent">Issue Point:</FMControls:FMLabel>
    <asp:TextBox ID="IssptTextbox"
        Style="z-index: 104; left: 150px; position: absolute; top: 82px" runat="server"
        MaxLength="20" Width="125px" CssClass="formfield" Enabled="false"></asp:TextBox>
    <FMControls:FMLabel ID="FMLabel7" AssociatedControlID="IssPtNumTextbox" Style="z-index: 112; left: 0px; position: absolute; top: 112px"
        runat="server" CssClass="formfieldtitle" BackColor="Transparent">Issue Point Number:</FMControls:FMLabel>
    <asp:TextBox ID="IssPtNumTextbox"
        Style="z-index: 104; left: 150px; position: absolute; top: 112px" runat="server"
        MaxLength="20" Width="125px" CssClass="formfield"></asp:TextBox>
	<FMControls:FMLabel id="TankBottomLabel" AssociatedControlID="TankBottomTextbox" style="Z-INDEX: 112; LEFT: 0px; POSITION: absolute; TOP: 142px"
		runat="server" CssClass="formfieldtitle" BackColor="Transparent">Tank Bottom:</FMControls:FMLabel>
	<asp:textbox id="TankBottomTextbox" style="Z-INDEX: 106; LEFT: 150px; POSITION: absolute; TOP: 142px; width: 125px;"
		runat="server" CssClass="formfield" MaxLength="15" tabIndex="3"></asp:textbox>

    <FMControls:FMLabel ID="Fmlabel2" Style="z-index: 134; left: 96px; position: absolute; top: 213px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Units:</FMControls:FMLabel>
    <FMControls:FMLabel ID="Fmlabel3" Style="z-index: 134; left: 232px; position: absolute; top: 213px"
        runat="server" CssClass="formfieldtitle" BackColor="Transparent">Decimal Places:</FMControls:FMLabel>
    <FMControls:FMLabel ID="FMLabel1" AssociatedControlID="VolumeUnitsDropDownList" Style="z-index: 117; left: 0px; position: absolute; top: 237px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Volume:</FMControls:FMLabel>
    <asp:DropDownList ID="VolumeUnitsDropDownList" Style="z-index: 120; left: 96px; position: absolute; top: 237px"
        runat="server" Width="88px" CssClass="formfield" AutoPostBack="True">
    </asp:DropDownList>
    <asp:TextBox ID="VolumeDecimalPlacesTextbox" ToolTip="Volume decimal places" Style="z-index: 119; left: 232px; position: absolute; top: 237px"
        runat="server" Width="32px" CssClass="formfield" AutoPostBack="True"></asp:TextBox>
    <FMControls:FMLabel ID="TmpFmlabel2" AssociatedControlID="TemperatureUnitsDropDownList" Style="z-index: 117; left: 0px; position: absolute; top: 269px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Temperature:</FMControls:FMLabel>
    <asp:DropDownList ID="TemperatureUnitsDropDownList" Style="z-index: 120; left: 96px; position: absolute; top: 269px"
        runat="server" Width="88px" CssClass="formfield" >
    </asp:DropDownList>
    <asp:TextBox ID="TemperatureDecimalPlacesTextbox" ToolTip="Temperature decimal places" Style="z-index: 119; left: 232px; position: absolute; top: 269px"
        runat="server" Width="32px" CssClass="formfield"></asp:TextBox>
    <FMControls:FMLabel ID="Fmlabel5" AssociatedControlID="DensityUnitsDropDownList" Style="z-index: 117; left: 0px; position: absolute; top: 301px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Density:</FMControls:FMLabel>
    <asp:DropDownList ID="DensityUnitsDropDownList" Style="z-index: 120; left: 96px; position: absolute; top: 301px; right: 980px;"
        runat="server" Width="88px" CssClass="formfield">
    </asp:DropDownList>
    <asp:TextBox ID="DensityDecimalPlacesTextbox" ToolTip="Density decimal places" Style="z-index: 119; left: 232px; position: absolute; top: 301px"
        runat="server" Width="32px" CssClass="formfield"></asp:TextBox>
    <FMControls:FMLabel ID="Fmlabel6" AssociatedControlID="MassUnitsDropDownList" Style="z-index: 117; left: 0px; position: absolute; top: 333px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Mass:</FMControls:FMLabel>
    <asp:DropDownList ID="MassUnitsDropDownList" Style="z-index: 120; left: 96px; position: absolute; top: 333px; right: 980px;"
        runat="server" Width="88px" CssClass="formfield">
    </asp:DropDownList>
    <asp:TextBox ID="MassDecimalPlacesTextbox" ToolTip="Mass decimal places" Style="z-index: 119; left: 232px; position: absolute; top: 333px"
        runat="server" Width="32px" CssClass="formfield"></asp:TextBox>

    <FMControls:FMCheckBox ID="FuelAdditiveCheckBox" Style="z-index: 142; left: 388px; position: absolute; top: 14px; width: 129px;"
        runat="server" CssClass="formfieldtitle"
        Text="Fuel Additive Flag" TextAlign="Left"></FMControls:FMCheckBox>
    <FMControls:FMCheckBox ID="SecondaryStorageCheckBox" Style="z-index: 142; left: 388px; position: absolute; top: 44px; width: 142px;"
        runat="server" CssClass="formfieldtitle"
        Text="Secondary Storage" TextAlign="Left"></FMControls:FMCheckBox>
    <FMControls:FMCheckBox ID="ManagedEquipmentCheckBox" Style="z-index: 142; left: 388px; position: absolute; top: 74px; width: 142px;"
        runat="server" CssClass="formfieldtitle"
        Text="Managed Equipment" TextAlign="Left"></FMControls:FMCheckBox>

    <FMControls:FMLabel ID="InstallationLabel"
        Style="z-index: 115; left: 388px; position: absolute; top: 112px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Installation Date:</FMControls:FMLabel>
    <FMControls:FMDate ID="InstallationDate" Style="z-index: 413; left: 510px; position: absolute; top: 112px"
        runat="server" MaxLength="50" Width="150px" CssClass="formfield" />
    <FMControls:FMLabel ID="ModelLabel" Style="z-index: 112; left: 388px; position: absolute; top: 142px"
        runat="server" CssClass="formfieldtitle" BackColor="Transparent">Manufacture Date:</FMControls:FMLabel>
    <FMControls:FMDate ID="ManufactureDate" Style="z-index: 412; left: 510px; position: absolute; top: 142px"
        runat="server" MaxLength="50" Width="150px" CssClass="formfield" />
    <FMControls:FMLabel ID="Label7"
        Style="z-index: 117; left: 388px; position: absolute; top: 174px" runat="server"
        CssClass="formfieldtitle" BackColor="Transparent">Calibration Date:</FMControls:FMLabel>
    <FMControls:FMDate ID="CalibrationDate" Style="z-index: 411; left: 510px; position: absolute; top: 174px"
        runat="server" MaxLength="50" Width="150px" CssClass="formfield" />
    <FMControls:FMLabel ID="SerialNumLabel" Style="z-index: 134; left: 388px; position: absolute; top: 204px"
        runat="server" CssClass="formfieldtitle" BackColor="Transparent">Inspection Date:</FMControls:FMLabel>
    <FMControls:FMDate ID="InspectionDate" Style="z-index: 410; left: 510px; position: absolute; top: 204px"
        runat="server" MaxLength="50" Width="150px" CssClass="formfield" />

    <FMControls:FMButton ID="VolumeOutputButton" Style="z-index: 104; left: 390px; position: absolute; top: 268px; width: 98px;"
        runat="server" CssClass="formfieldtitle" Text="Volume Output"
        OnCommand="VolumeOutputButtonCommand"></FMControls:FMButton>
    <FMControls:FMLabel ID="VolumeServerLabel" AssociatedControlID="VolumeHostNameTextbox" Style="z-index: 112; left: 388px; position: absolute; top: 303px; width: 108px;"
        runat="server" CssClass="formfieldtitle" BackColor="Transparent">Volume Server:</FMControls:FMLabel>
    <asp:TextBox ID="VolumeHostNameTextbox"
        Style="z-index: 104; left: 508px; position: absolute; top: 303px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis" runat="server"
        MaxLength="20" Width="200px" CssClass="formfield" ReadOnly="True"></asp:TextBox>
    <FMControls:FMLabel ID="VolumeOpcLabel" AssociatedControlID="VolumeProgIDTextbox" Style="z-index: 112; left: 388px; position: absolute; top: 335px; width: 118px;"
        runat="server" CssClass="formfieldtitle" BackColor="Transparent">Volume OPC Server:</FMControls:FMLabel>
    <asp:TextBox ID="VolumeProgIDTextbox"
        Style="z-index: 104; left: 507px; position: absolute; top: 335px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis" runat="server"
        MaxLength="20" Width="200px" CssClass="formfield" ReadOnly="True"></asp:TextBox>
    <FMControls:FMLabel ID="VolumeItemLabel" AssociatedControlID="VolumeItemIDTextbox" Style="z-index: 112; left: 388px; position: absolute; top: 365px; width: 102px;"
        runat="server" CssClass="formfieldtitle" BackColor="Transparent">Volume Item ID:</FMControls:FMLabel>
    <asp:TextBox ID="VolumeItemIDTextbox"
        Style="z-index: 104; left: 507px; position: absolute; top: 365px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis" runat="server"
        MaxLength="20" Width="200px" CssClass="formfield" ReadOnly="True"></asp:TextBox>
</body>
</html>
