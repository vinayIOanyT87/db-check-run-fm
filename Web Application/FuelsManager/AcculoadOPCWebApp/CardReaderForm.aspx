<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="CardReaderForm.aspx.cs" AutoEventWireup="true" Inherits="OPCWebApp.AcculoadOPCWebApp.CardReaderForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../css/FuelsManager.css" rel="stylesheet">
  </HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<asp:DropDownList id="TypeDropDownList" style="Z-INDEX: 108; LEFT: 104px; POSITION: absolute; TOP: 80px"
				runat="server" Width="160px" CssClass="formfield" tabIndex="2" AutoPostBack="True" 
				onselectedindexchanged="TypeDropDownList_SelectedIndexChanged"></asp:DropDownList>
			<FMCONTROLS:FMLABEL id="Label13" 
				style="Z-INDEX: 107; LEFT: 16px; POSITION: absolute; TOP: 80px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|Type:</FMCONTROLS:FMLABEL>
			<asp:DropDownList id="AddressDropDownList" style="Z-INDEX: 120; LEFT: 104px; POSITION: absolute; TOP: 112px"
				runat="server" CssClass="formfield" Width="160px" tabIndex="3"></asp:DropDownList>
			<FMCONTROLS:FMRadioButton id="SerialCommunicationsRadioButton" style="Z-INDEX: 142; LEFT: 88px; POSITION: absolute; TOP: 141px"
				tabIndex="3" runat="server" GroupName="Communications" 
				Text="SmithMeter|Serial Communications" CssClass="formfieldtitle"
				Width="232px" AutoPostBack="True" 
				oncheckedchanged="SerialCommunicationsRadioButton_CheckedChanged"></FMCONTROLS:FMRadioButton>
			<FMCONTROLS:FMRadioButton id="NetworkCommunicationsRadioButton" style="Z-INDEX: 141; LEFT: 88px; POSITION: absolute; TOP: 201px"
				tabIndex="6" runat="server" GroupName="Communications" 
				Text="SmithMeter|Network Communications" CssClass="formfieldtitle"
				AutoPostBack="True" 
				oncheckedchanged="NetworkCommunicationsRadioButton_CheckedChanged"></FMCONTROLS:FMRadioButton>
			<FMCONTROLS:FMLABEL id="Label3" 
				style="Z-INDEX: 119; LEFT: 16px; POSITION: absolute; TOP: 115px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|Address:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMButton id="CancelButton" style="Z-INDEX: 118; LEFT: 144px; POSITION: absolute; TOP: 292px"
				runat="server" Width="98px" Text="SmithMeter|Cancel" CssClass="formfieldtitle" 
				tabIndex="9"></FMCONTROLS:FMButton>
			<FMCONTROLS:FMButton id="OKButton" 
				style="Z-INDEX: 117; LEFT: 16px; POSITION: absolute; TOP: 291px; right: 833px;" runat="server"
				Width="107px" Text="SmithMeter|OK" CssClass="formfieldtitle" tabIndex="8"></FMCONTROLS:FMButton>
			<asp:TextBox id="IPAddressTextBox" style="Z-INDEX: 145; LEFT: 104px; POSITION: absolute; TOP: 226px"
				runat="server" Width="160px" CssClass="formfield" tabIndex="7"></asp:TextBox>
			<FMCONTROLS:FMLABEL id="Label12" 
				style="Z-INDEX: 116; LEFT: 104px; POSITION: absolute; TOP: 258px; width: 275px;" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" Height="8px" 
				ForeColor="Crimson">SmithMeter|* Denotes Required Field</FMCONTROLS:FMLABEL>
			<asp:DropDownList id="PortDropDownList" style="Z-INDEX: 106; LEFT: 104px; POSITION: absolute; TOP: 170px"
				runat="server" Width="160px" CssClass="formfield" tabIndex="3"></asp:DropDownList>
			<FMCONTROLS:FMLABEL id="Label5" 
				style="Z-INDEX: 143; LEFT: 16px; POSITION: absolute; TOP: 228px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|IP Address:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="Label4" 
				style="Z-INDEX: 105; LEFT: 16px; POSITION: absolute; TOP: 174px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|Port:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="UserNameRequiredLabel" style="Z-INDEX: 104; LEFT: 72px; POSITION: absolute; TOP: 48px"
				runat="server" BackColor="Transparent" Width="8px" ForeColor="Crimson" Height="8px" CssClass="formfieldtitle">*</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="Label2" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="368px" BackColor="Transparent">SmithMeter|Card Reader Configuration</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="Label1" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 48px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|ID:</FMCONTROLS:FMLABEL>
			<asp:TextBox id="IDTextBox" style="Z-INDEX: 103; LEFT: 104px; POSITION: absolute; TOP: 48px"
				runat="server" CssClass="formfield" Width="136px" tabIndex="1"></asp:TextBox>
			
			<script language="jscript">
				document.getElementById("OKButton").setActive();
				document.getElementById("IDTextBox").focus();
			</script>
		</div>
</form>
	</body>
</HTML>
