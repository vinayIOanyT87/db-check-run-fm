<%@ Control language="c#" Codebehind="StationGeneralPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.StationGeneralPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<html>
<head>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<script>
		function ConfirmMeterDelete() {
			var hasReference = document.getElementById("tcStation_tpGeneralPage_StationGeneralPage_MeterReferenced").value;
			if (hasReference == "False") {
				var interfaceType = document.getElementById("tcStation_tpGeneralPage_StationGeneralPage_InterfaceTypeDropDownList").value;
				var stationType = document.getElementById("tcStation_tpGeneralPage_StationGeneralPage_TypeDropDownList").value;
				if (interfaceType == 9 && stationType != 7) {
					var confirmDelete = document.getElementById("tcStation_tpGeneralPage_StationGeneralPage_DeleteMeter");
					if (confirm("Changing station type from Meter Station will cause meter to be deleted. Do you want to continue?") == true) {
						confirmDelete.value = "OK";
					} else {
						confirmDelete.value = "Cancel";
					}
				}
			}
		}
    </script>
</head>
<body>
	<FMControls:FMLabel ID="Label1" AssociatedControlID="IDTextBox" Style="z-index: 102; left: 0px; position: absolute; top: 16px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">Station ID:</FMControls:FMLabel>
	<FMControls:FMLabel ID="Label8" Style="z-index: 105; left: 112px; position: absolute; top: 16px" runat="server"
		BackColor="Transparent" Width="8px" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>
	<asp:TextBox ID="IDTextBox" Style="z-index: 103; left: 128px; position: absolute; top: 16px" aria-required="true"
		runat="server" CssClass="formfield" BackColor="White" Width="240px" MaxLength="50" TabIndex="1"></asp:TextBox>
	<FMControls:FMLabel ID="Label2" AssociatedControlID="TypeDropDownList" Style="z-index: 104; left: 0px; position: absolute; top: 48px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">Type:</FMControls:FMLabel>
	<FMControls:FMDropDownList ID="TypeDropDownList" Style="z-index: 106; left: 128px; position: absolute; top: 48px"
		runat="server" CssClass="formfield" Width="240px" AutoPostBack="True" TabIndex="2" OnSelectedIndexChanged="TypeDropDownList_SelectedIndexChanged" onchange="ConfirmMeterDelete()">
	</FMControls:FMDropDownList>
	<FMControls:FMLabel ID="Label3" AssociatedControlID="InterfaceTypeDropDownList" BackColor="Transparent" CssClass="formfieldtitle" Style="z-index: 104; left: 0px; position: absolute; top: 80px"
		runat="server">Interface Type:</FMControls:FMLabel>
	<asp:DropDownList ID="InterfaceTypeDropDownList" Style="z-index: 106; left: 128px; position: absolute; top: 80px"
		runat="server" CssClass="formfield" Width="240px" AutoPostBack="True" TabIndex="3" OnSelectedIndexChanged="InterfaceTypeDropDownList_SelectedIndexChanged">
	</asp:DropDownList>
	<FMControls:FMLabel ID="Fmlabel1" CssClass="formfieldtitle" Style="z-index: 102; left: 0px; position: absolute; top: 112px"
		runat="server" Width="80px">System:</FMControls:FMLabel>
	<FMControls:FMDropDownList ID="SelectSystemModeDropDownList" ToolTip="Select system mode" Style="z-index: 106; left: 128px; position: absolute; top: 112px"
		TabIndex="4" runat="server" Width="58px" CssClass="formfield" Height="24px" AutoPostBack="True" OnSelectedIndexChanged="SelectSystemModeDropDownList_SelectedIndexChanged">
	</FMControls:FMDropDownList>
	<asp:DropDownList ID="SystemDropDownList" Style="z-index: 106; left: 198px; position: absolute; top: 112px"
		runat="server" CssClass="formfield" Width="170px" AutoPostBack="True" TabIndex="5" OnSelectedIndexChanged="SystemDropDownList_SelectedIndexChanged">
	</asp:DropDownList>
	<asp:TextBox ID="SystemTextBox" ToolTip="System" Style="z-index: 106; left: 198px; position: absolute; top: 112px"
		TabIndex="5" runat="server" Width="170px" CssClass="formfield" AutoPostBack="True" MaxLength="80" OnTextChanged="SystemTextBox_TextChanged"></asp:TextBox>
	<FMControls:FMLabel ID="Label4" AssociatedControlID="OPCServerTextBox" CssClass="formfieldtitle" Style="z-index: 102; left: 0px; position: absolute; top: 144px"
		runat="server" Width="80px">OPC Server:</FMControls:FMLabel>
	<asp:TextBox ID="OPCServerTextBox" Style="z-index: 103; left: 128px; position: absolute; top: 144px"
		runat="server" CssClass="formfield" BackColor="White" Width="240px" Enabled="False" TabIndex="6" ReadOnly="True"></asp:TextBox>
	<FMControls:FMLabel ID="Label5" AssociatedControlID="OPCItemPathTextBox" CssClass="formfieldtitle" Style="z-index: 102; left: 0px; position: absolute; top: 176px; width: 99px;"
		runat="server">OPC Item Path:</FMControls:FMLabel>
	<asp:TextBox ID="OPCItemPathTextBox" Style="z-index: 103; left: 128px; position: absolute; top: 176px"
		runat="server" CssClass="formfield" BackColor="White" Width="240px" MaxLength="256" TabIndex="7"></asp:TextBox>
	<FMControls:FMCheckBox ID="ThirtyFiveBitCardsCheckBox" Style="z-index: 103; left: 0px; position: absolute; top: 208px"
		runat="server" Text="35-bit Card Support" CssClass="formfieldtitle" TextAlign="Right" Width="150px"
		TabIndex="8"></FMControls:FMCheckBox>
	<FMControls:FMCheckBox ID="CardReaderCheckBox" Style="z-index: 103; left: 0px; position: absolute; top: 240px"
		runat="server" Text="Card Reader" CssClass="formfieldtitle" TextAlign="Right" Width="150px" TabIndex="9"></FMControls:FMCheckBox>
	<FMControls:FMCheckBox ID="TouchKeyReaderCheckBox" Style="z-index: 103; left: 0px; position: absolute; top: 269px"
		runat="server" Text="Touch Key Reader" CssClass="formfieldtitle" TextAlign="Right"
		Width="150px" TabIndex="9"></FMControls:FMCheckBox>
	<FMControls:FMCheckbox id="LogCommunicationsCheckBox" style="Z-INDEX: 104; LEFT: 0px; POSITION: absolute; TOP: 304px"
		runat="server" Text="Log Communications" CssClass="formfieldtitle" TextAlign="Right" 
		Width="150px" tabIndex="9"></FMControls:FMCheckbox>
	<FMControls:FMLabel id="LogCommPathLabel" AssociatedControlID="LogCommPathTextbox" style="Z-INDEX: 105; LEFT: 21px; POSITION: absolute; TOP: 334px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">Path:</FMControls:FMLabel>
	<asp:textbox id="LogCommPathTextbox" style="Z-INDEX: 106; LEFT: 72px; POSITION: absolute; TOP: 331px"
		runat="server" CssClass="formfield" BackColor="White" Width="240px" MaxLength="50" tabIndex="1"></asp:textbox>
    <FMControls:FMLabel id="Fmlabel2" AssociatedControlID="PromptTimeoutBox" CssClass="formfieldtitle" style="Z-INDEX: 102; LEFT: 330px; POSITION: absolute; TOP: 212px"
		runat="server" Width="140px">Prompt Timeout (sec):</FMControls:FMLabel>
    <asp:textbox id="PromptTimeoutBox" style="Z-INDEX: 103; LEFT: 500px; POSITION: absolute; TOP: 208px"
		runat="server" CssClass="formfield" BackColor="White" Width="56px" MaxLength="3" tabIndex="9" ontextchanged="OnPromptTimeoutBox_TextChanged" ></asp:textbox>
    <FMControls:FMLabel id="Fmlabel3" AssociatedControlID="MessageTimeoutBox" CssClass="formfieldtitle" style="Z-INDEX: 102; LEFT: 330px; POSITION: absolute; TOP: 244px"
		runat="server" Width="140px">Message Timeout (sec):</FMControls:FMLabel>
	<asp:textbox id="MessageTimeoutBox" style="Z-INDEX: 103; LEFT: 500px; POSITION: absolute; TOP: 240px" 
        runat="server" CssClass="formfield" BackColor="White" Width="56px" MaxLength="3" tabIndex="10" ontextchanged="OnMessageTimeoutBox_TextChanged" ></asp:textbox>
	<asp:HiddenField ID="DeleteMeter" runat="server" Value="Cancel" />
	<asp:HiddenField ID="MeterReferenced" runat="server" Value="True" />
</body>
</html>
