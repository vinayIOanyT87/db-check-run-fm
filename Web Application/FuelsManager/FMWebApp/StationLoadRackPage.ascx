<%@ Control language="c#" Codebehind="StationLoadRackPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.StationLoadRackPage"  TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<html>
<head>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">

	<script type="text/javascript">
		function DisableStationFormControlButtons()
		{
            var newButton = document.getElementById('New');
            if (newButton) {
                newButton.disabled = true;
            }
			var okButton = document.getElementById('OK');
            if (okButton) {
                okButton.disabled = true;
			}
            var cancelButton = document.getElementById('Cancel');
            if (cancelButton) {
                cancelButton.disabled = true;
            }
		}
    </script>
</head>
<body>
	<FMControls:FMLabel ID="Label2" Style="z-index: 106; left: 0px; position: absolute; top: 16px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">Swing Arm Position:</FMControls:FMLabel>
	<asp:DropDownList ID="SwingArmPositionDropDownList" Style="z-index: 107; left: 152px; position: absolute; top: 16px"
		runat="server" Width="48px" CssClass="formfield" TabIndex="1">
	</asp:DropDownList>
	<FMControls:FMLabel ID="Label1" Style="z-index: 120; left: 296px; position: absolute; top: 16px" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" Width="80px">BOL Printer:</FMControls:FMLabel>
	<asp:DropDownList ID="BOLPrinterDropDownList" Style="z-index: 121; left: 448px; position: absolute; top: 16px"
		runat="server" CssClass="formfield" Width="240px" TabIndex="11">
	</asp:DropDownList>
	<FMControls:FMCheckBox ID="VaporRecoveryCheckBox" Style="z-index: 103; left: 0px; position: absolute; top: 48px"
		runat="server" Text="Vapor Recovery" CssClass="formfieldtitle" TabIndex="2"></FMControls:FMCheckBox>
	<FMControls:FMLabel ID="NumberOfCopiesLabel" Style="z-index: 120; left: 296px; position: absolute; top: 48px"
		runat="server" CssClass="formfieldtitle" Width="112px" BackColor="Transparent">Number of copies:</FMControls:FMLabel>
	<asp:TextBox ID="NumberOfCopiesTextBox" Style="z-index: 120; left: 448px; position: absolute; top: 48px"
		runat="server" Width="80px" Columns="2" MaxLength="2" CssClass="formfield"
		TabIndex="12"></asp:TextBox>
	<FMControls:FMCheckBox ID="SetPreloadToZeroCheckBox" Style="z-index: 103; left: 0px; position: absolute; top: 80px"
		TabIndex="3" runat="server" CssClass="formfieldtitle" Text="Set Default Preset To Zero"></FMControls:FMCheckBox>
	<FMControls:FMLabel ID="Label16" Style="z-index: 109; left: 296px; position: absolute; top: 80px" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" Width="112px">Issue Transaction:</FMControls:FMLabel>
	<asp:DropDownList ID="IssueTransactionDropDownList" Style="z-index: 102; left: 448px; position: absolute; top: 80px"
		TabIndex="13" runat="server" CssClass="formfield" Width="240px">
	</asp:DropDownList>
	<FMControls:FMCheckBox ID="InhibitLoadingByLoadIDCheckBox" Style="z-index: 103; left: 0px; position: absolute; top: 112px"
		runat="server" Text="Inhibit Loading By LoadID" CssClass="formfieldtitle" TabIndex="4"></FMControls:FMCheckBox>
	<input class="formfieldtitle" id="StationPermissivesButton" onclick="PermissivesButton_Click('StationPermissives', '0')"
		type="button" value="Station Permissives" runat="server" name="StationPermissivesButton"
		style="z-index: 105; left: 448px; width: 240px; position: absolute; top: 112px; height: 22px" tabindex="14">
	<FMControls:FMCheckBox ID="SynchronizeReferenceDensityCheckBox" Style="z-index: 103; left: 0px; position: absolute; top: 144px"
		runat="server" Text="Synchronize Reference Density" CssClass="formfieldtitle" TabIndex="5"></FMControls:FMCheckBox>
	<FMControls:FMLabel ID="Label17"
		Style="z-index: 109; left: 296px; position: absolute; top: 149px; width: 142px;" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" AssociatedControlID="MeterRecircCardNumber">Meter Recirc Card No:</FMControls:FMLabel>
	<asp:TextBox ID="MeterRecircCardNumber" Style="z-index: 120; left: 448px; position: absolute; top: 146px; width: 240px;"
		runat="server" Columns="2" MaxLength="30" CssClass="formfield" TabIndex="15"></asp:TextBox>
	<FMControls:FMCheckBox ID="InhibitSettingRecipeNamesCheckBox" Style="z-index: 103; left: 0px; position: absolute; top: 176px"
		runat="server" Text="Inhibit Setting Recipe Name" CssClass="formfieldtitle" TabIndex="6"></FMControls:FMCheckBox>
	<FMControls:FMCheckBox ID="EthanolExcessCheckBox" Style="z-index: 103; left: 0px; position: absolute; top: 208px"
		runat="server" Text="Ethanol Excess" CssClass="formfieldtitle" TabIndex="7"></FMControls:FMCheckBox>
	<FMControls:FMCheckbox id="EnableScullyCheckBox" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 244px;"
		runat="server" Text="Enable Scully" CssClass="formfieldtitle" tabIndex="8" AutoPostBack="True" 
        oncheckedchanged="EnableScullyCheckBox_CheckedChanged"></FMControls:FMCheckbox>
	<FMControls:FMCheckbox id="EnableEquipmentValidateCheckBox" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 278px"
		runat="server" Text="Enable Equipment Validation" Visible="False" CssClass="formfieldtitle" 
        tabIndex="9"></FMControls:FMCheckbox>
	<FMControls:FMCheckBox ID="EnableDynamicRecipesCheckBox" Style="z-index: 103; left: 0px; position: absolute; top: 310px"
		TabIndex="10" runat="server" CssClass="formfieldtitle" Text="Enable Dynamic Recipes" AutoPostBack="True" 
        oncheckedchanged="EnableDynamicRecipesCheckBox_CheckedChanged" onclick="DisableStationFormControlButtons();"></FMControls:FMCheckBox>
	<FMControls:FMLabel ID="Label18"
		Style="z-index: 109; left: 296px; position: absolute; top: 179px; width: 121px;" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" AssociatedControlID="RecircTransactionDropDownList">Recirc Transaction:</FMControls:FMLabel>
	<asp:DropDownList ID="RecircTransactionDropDownList" Style="z-index: 102; left: 448px; position: absolute; top: 178px"
		TabIndex="16" runat="server" CssClass="formfield" Width="240px">
	</asp:DropDownList>
	<FMControls:FMLabel ID="Label19"
		Style="z-index: 109; left: 296px; position: absolute; top: 212px;" runat="server"
		BackColor="Transparent" CssClass="formfieldtitle" Width="112px" AssociatedControlID="NumberOfLastTransaction">Last Transaction:</FMControls:FMLabel>
	<asp:TextBox ID="NumberOfLastTransaction" Style="z-index: 120; left: 448px; position: absolute; top: 210px"
		runat="server" Width="80px" Columns="2" MaxLength="6" CssClass="formfield"
		TabIndex="17"></asp:TextBox>
	<FMControls:FMLabel id="OPCServerLbl" AssociatedControlID="OPCServerDropDownList" CssClass="formfieldtitle" style="Z-INDEX: 102; LEFT: 296px; POSITION: absolute; TOP: 244px"
			runat="server" Width="80px">OPC Server:</FMControls:FMLabel>
	<asp:dropdownlist id="OPCServerDropDownList" style="Z-INDEX: 102; LEFT: 448px; POSITION: absolute; TOP: 244px"
		    tabIndex="18" runat="server" Width="240px" CssClass="formfield"></asp:dropdownlist>
	<FMControls:FMLabel id="OPCItemPathLbl" AssociatedControlID="OPCItemPathTextBox" CssClass="formfieldtitle" style="Z-INDEX: 102; LEFT: 296px; POSITION: absolute; TOP: 278px"
			runat="server" Width="96px">OPC Item Path:</FMControls:FMLabel>
	<asp:textbox id="OPCItemPathTextBox" style="Z-INDEX: 103; LEFT: 448px; POSITION: absolute; TOP: 278px"
		runat="server" CssClass="formfield" BackColor="White" Width="240px" MaxLength="256" tabIndex="19"></asp:textbox>	

</body>
</html>

