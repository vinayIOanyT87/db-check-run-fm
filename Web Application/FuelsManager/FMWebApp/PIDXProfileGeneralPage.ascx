<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PIDXProfileGeneralPage.ascx.cs" Inherits="FuelsManager.FMWebApp.PIDXProfileGeneralPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
      <FMCONTROLS:FMLABEL id="Label1" AssociatedControlID="IDTextBox" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 6px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">ID:</FMCONTROLS:FMLABEL>
		<asp:textbox id="IDTextBox" style="Z-INDEX: 103; LEFT: 144px; POSITION: absolute; TOP: 6px"
			tabIndex="2" runat="server" CssClass="formfield" Width="192px" MaxLength="30"></asp:textbox>
      <FMCONTROLS:FMLABEL id="Label4" AssociatedControlID="TypeDropDownList" style="Z-INDEX: 106; LEFT: 0px; POSITION: absolute; TOP: 36px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Type:</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMDROPDOWNLIST id="TypeDropDownList" style="Z-INDEX: 107; LEFT: 144px; POSITION: absolute; TOP: 36px"
			tabIndex="1" runat="server" CssClass="formfield" AutoPostBack="True" Width="192px"></FMCONTROLS:FMDROPDOWNLIST>
      <FMCONTROLS:FMLABEL id="Label9"  AssociatedControlID="VersionDropDownList" style="Z-INDEX: 106; LEFT: 0px; POSITION: absolute; TOP: 66px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Version:</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMDROPDOWNLIST id="VersionDropDownList" style="Z-INDEX: 107; LEFT: 144px; POSITION: absolute; TOP: 66px"
			tabIndex="1" runat="server" CssClass="formfield" AutoPostBack="False" Width="192px"></FMCONTROLS:FMDROPDOWNLIST>
		<FMCONTROLS:FMLABEL id="Label2" AssociatedControlID="IPAddressTextBox" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 96px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">IP Address:</FMCONTROLS:FMLABEL>
		<asp:textbox id="IPAddressTextBox" style="Z-INDEX: 103; LEFT: 144px; POSITION: absolute; TOP: 96px"
			tabIndex="3" runat="server" CssClass="formfield" Width="192px" MaxLength="60"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label3" AssociatedControlID="PortTextBox" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 126px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Port:</FMCONTROLS:FMLABEL>
		<asp:textbox id="PortTextBox" style="Z-INDEX: 103; LEFT: 144px; POSITION: absolute; TOP: 126px"
			tabIndex="4" runat="server" CssClass="formfield" Width="72px" MaxLength="30"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label5" AssociatedControlID="TerminalIDTextBox" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 156px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Terminal ID:</FMCONTROLS:FMLABEL>
		<asp:textbox id="TerminalIDTextBox" style="Z-INDEX: 103; LEFT: 144px; POSITION: absolute; TOP: 156px"
			tabIndex="5" runat="server" CssClass="formfield" Width="192px" MaxLength="10"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label6" AssociatedControlID="UserIDTextBox" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 186px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">User ID:</FMCONTROLS:FMLABEL>
		<asp:textbox id="UserIDTextBox" style="Z-INDEX: 103; LEFT: 144px; POSITION: absolute; TOP: 186px"
			tabIndex="6" runat="server" CssClass="formfield" Width="192px" MaxLength="30"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label7" AssociatedControlID="PasswordTextBox" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 216px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Password:</FMCONTROLS:FMLABEL>
		<asp:textbox id="PasswordTextBox" style="Z-INDEX: 103; LEFT: 144px; POSITION: absolute; TOP: 216px" tabIndex="7" runat="server" CssClass="formfield" Width="192px" MaxLength="30" 
			TextMode="Password"  AutoCompleteType="None"></asp:textbox>
		<asp:textbox id="InitialPasswordTextBox" ToolTip="Initial password" style="Z-INDEX: 103; LEFT: 144px; POSITION: absolute; TOP: 216px"
			tabIndex="-1" runat="server" BackColor="Transparent" CssClass="formfield" Width="0px" MaxLength="10" Enabled="False" ReadOnly="True" BorderColor="Transparent" BorderStyle="None"></asp:textbox>
		<FMCONTROLS:FMCHECKBOX id="EnabledCheckBox" ToolTip="Enabled" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 246px"
			tabIndex="8" runat="server" CssClass="formfieldtitle" Width="150px" TextAlign="Right" Text="Enabled"></FMCONTROLS:FMCHECKBOX>
		<FMCONTROLS:FMCHECKBOX id="LoggingEnabledCheckBox" ToolTip="Logging enabled" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 276px"
			tabIndex="9" runat="server" CssClass="formfieldtitle" Width="150px" TextAlign="Right" Text="Logging Enabled"></FMCONTROLS:FMCHECKBOX>
		<FMCONTROLS:FMLABEL id="Label8" AssociatedControlID="LogFileTextbox" style="Z-INDEX: 102; LEFT: 0px; POSITION: absolute; TOP: 306px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Log File:</FMCONTROLS:FMLABEL>
		<asp:textbox id="LogFileTextbox" style="Z-INDEX: 103; LEFT: 144px; POSITION: absolute; TOP: 300px"
			tabIndex="10" runat="server" CssClass="formfield" Width="192px" MaxLength="255" TextMode="SingleLine" Enabled="true" Wrap="false" Visible="true"></asp:textbox>
	</body>
</HTML>
<script type="text/javascript">
    var oInitialPasswordTextBox = document.getElementById("tcPIDXProfileTabs_tpGeneralPage_PIDXProfileGeneralPage_InitialPasswordTextBox");
	var oPasswordTextBox = document.getElementById("tcPIDXProfileTabs_tpGeneralPage_PIDXProfileGeneralPage_PasswordTextBox");

	if(oInitialPasswordTextBox != null
	&& oPasswordTextBox != null)
	{
		oPasswordTextBox.value=oInitialPasswordTextBox.value;
		oPasswordTextBox.attachEvent("onactivate",PasswordActive);
	}

	function PasswordActive()
	{
	    var oPasswordTextBox = document.getElementById("tcPIDXProfileTabs_tpGeneralPage_PIDXProfileGeneralPage_PasswordTextBox");
		oPasswordTextBox.select();
	}
</script>
