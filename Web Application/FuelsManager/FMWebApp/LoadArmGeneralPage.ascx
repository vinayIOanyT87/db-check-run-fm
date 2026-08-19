<%@ Control language="c#" Codebehind="LoadArmGeneralPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.LoadArmGeneralPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<html>
<head>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">

	<script type="text/javascript">
		function DisableLoadArmFormControlButtons()
		{
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
	<FMControls:FMLabel ID="Fmlabel2" AssociatedControlID="LoadRackTextTextBox" Style="z-index: 105; left: 0px; position: absolute; top: 16px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">Load Rack Text:</FMControls:FMLabel>
	<asp:TextBox ID="LoadRackTextTextBox" Style="z-index: 107; left: 168px; position: absolute; top: 16px"
		runat="server" CssClass="formfield" BackColor="White" Width="104px" MaxLength="9" TabIndex="1"></asp:TextBox>
	<FMControls:FMCheckBox ID="EnabledCheckBox" Style="z-index: 101; left: 0px; position: absolute; top: 48px"
		runat="server" CssClass="formfieldtitle" BackColor="Transparent" Width="72px" Text="Enabled:" TextAlign="Left"
		TabIndex="2"></FMControls:FMCheckBox>
	<FMControls:FMCheckBox ID="SwingArmCheckBox" Style="z-index: 102; left: 0px; position: absolute; top: 80px"
		runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text="Swing Arm:" TextAlign="Left"
		AutoPostBack="True" TabIndex="3" OnCheckedChanged="SwingArmCheckBoxCheckedChanged" onclick="DisableLoadArmFormControlButtons();"></FMControls:FMCheckBox>
	<asp:DropDownList ID="StationDropDownList" ToolTip="Station" Style="z-index: 110; left: 168px; position: absolute; top: 80px"
		runat="server" CssClass="formfield" Width="256px" TabIndex="4" OnSelectedIndexChanged="StationDropDownListSelectedIndexChanged" AutoPostBack="True">
	</asp:DropDownList>
	<FMControls:FMLabel ID="Label4" AssociatedControlID="PresetTypeDropDownList" Style="z-index: 105; left: 0px; position: absolute; top: 120px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">Preset Type:</FMControls:FMLabel>
	<asp:DropDownList ID="PresetTypeDropDownList" Style="z-index: 106; left: 168px; position: absolute; top: 112px"
		runat="server" CssClass="formfield" Width="256px" AutoPostBack="True" TabIndex="5" OnSelectedIndexChanged="PresetTypeDropDownListSelectedIndexChanged">
	</asp:DropDownList>
	<FMControls:FMLabel ID="Fmlabel1" AssociatedControlID="SystemDropDownList" CssClass="formfieldtitle" Style="z-index: 102; left: 0px; position: absolute; top: 152px"
		runat="server" Width="80px">System:</FMControls:FMLabel>
	<FMControls:FMDropDownList ID="SelectSystemModeDropDownList" ToolTip="System Mode" Style="z-index: 106; left: 168px; position: absolute; top: 144px"
		TabIndex="6" runat="server" Width="58px" CssClass="formfield" AutoPostBack="True" OnSelectedIndexChanged="SelectSystemModeDropDownListSelectedIndexChanged">
	</FMControls:FMDropDownList>
	<asp:DropDownList ID="SystemDropDownList" Style="z-index: 106; left: 238px; position: absolute; top: 144px"
		runat="server" CssClass="formfield" Width="186px" AutoPostBack="True" TabIndex="7" OnSelectedIndexChanged="SystemDropDownListSelectedIndexChanged">
	</asp:DropDownList>
	<asp:TextBox ID="SystemTextBox" ToolTip="System Textbox" Style="z-index: 106; left: 238px; position: absolute; top: 144px"
		TabIndex="7" runat="server" Width="186px" CssClass="formfield" AutoPostBack="True" MaxLength="80"></asp:TextBox>
	<FMControls:FMLabel ID="Label2" AssociatedControlID="OPCServerTextBox" Style="z-index: 108; left: 0px; position: absolute; top: 184px" runat="server"
		CssClass="formfieldtitle" Width="80px">OPC Server:</FMControls:FMLabel>
	<asp:TextBox ID="OPCServerTextBox" Style="z-index: 103; left: 168px; position: absolute; top: 176px"
		runat="server" CssClass="formfield" BackColor="White" Width="256px" Enabled="False" TabIndex="8"></asp:TextBox>
	<FMControls:FMLabel ID="Label3" Style="z-index: 103; left: 0px; position: absolute; top: 216px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">OPC Item Path:</FMControls:FMLabel>
	<asp:TextBox ID="OPCItemIDTextBox" Style="z-index: 107; left: 168px; position: absolute; top: 208px"
		runat="server" CssClass="formfield" BackColor="White" Width="256px" MaxLength="256" TabIndex="8"></asp:TextBox>
	<input class="formfieldtitle" id="LoadArmPermissivesButton" onclick="PermissivesButton_Click('LoadArmPermissives', '0')"
		type="button" value="Arm Permissives" runat="server" name="LoadArmPermissivesButton"
		style="z-index: 105; left: 168px; width: 200px; position: absolute; top: 240px" size="30">
	<input class="formfieldtitle" id="NoAdditivePermissivesButton" onclick="PermissivesButton_Click('NoAdditivePermissives', '0')"
		type="button" value="No Additive Permissives" runat="server" name="NoAdditivePermissivesButton"
		style="z-index: 105; left: 168px; width: 200px; position: absolute; top: 278px" size="30">
</body>
</html>
