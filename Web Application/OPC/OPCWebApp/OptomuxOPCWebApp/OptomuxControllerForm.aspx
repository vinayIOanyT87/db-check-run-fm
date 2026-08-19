<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="OptomuxControllerForm.aspx.cs" AutoEventWireup="True" Inherits="OPCWebApp.OptomuxOPCWebApp.OptomuxControllerForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../css/FuelsManager.css" rel="stylesheet">
  </HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 101; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="400px" BackColor="Transparent">Optomux|Controller Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label1" AssociatedControlID="IDTextBox" Style="z-index: 102; left: 16px; position: absolute; top: 48px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Optomux|ID:</FMControls:FMLabel>
			<FMControls:FMLabel ID="UserNameRequiredLabel" Style="z-index: 104; left: 136px; position: absolute; top: 48px"
				runat="server" BackColor="Transparent" Width="8px" ForeColor="Crimson" Height="8px" CssClass="formfieldtitle">*</FMControls:FMLabel>
			<asp:TextBox ID="IDTextBox" Style="z-index: 103; left: 168px; position: absolute; top: 48px" aria-required="true"
				runat="server" CssClass="formfield" Width="169px" TabIndex="1"></asp:TextBox>
			<FMControls:FMLabel ID="Label3" Style="z-index: 105; left: 16px; position: absolute; top: 80px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Optomux|Type:</FMControls:FMLabel>
			<asp:DropDownList ID="TypeDropDownList" Style="z-index: 106; left: 168px; position: absolute; top: 80px"
				runat="server" Width="169px" CssClass="formfield" TabIndex="2">
			</asp:DropDownList>
			<FMControls:FMRadioButton ID="SerialCommunicationsRadioButton" Style="z-index: 142; left: 168px; position: absolute; top: 112px; width: 223px;"
				TabIndex="3" runat="server" GroupName="Communications" Text="Optomux|Serial Communications" CssClass="formfieldtitle" AutoPostBack="True" OnCheckedChanged="SerialCommunicationsRadioButton_CheckedChanged"></FMControls:FMRadioButton>
			<FMControls:FMLabel ID="Label4" Style="z-index: 107; left: 16px; position: absolute; top: 144px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Optomux|Port:</FMControls:FMLabel>
			<asp:DropDownList ID="PortDropDownList" Style="z-index: 108; left: 168px; position: absolute; top: 144px"
				runat="server" Width="169px" CssClass="formfield" TabIndex="4">
			</asp:DropDownList>
			<FMControls:FMLabel ID="Label9" Style="z-index: 112; left: 16px; position: absolute; top: 176px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Optomux|Address:</FMControls:FMLabel>
			<asp:DropDownList ID="AddressDropDownList" Style="z-index: 113; left: 168px; position: absolute; top: 176px"
				runat="server" CssClass="formfield" Width="169px" TabIndex="5">
			</asp:DropDownList>
			<FMControls:FMRadioButton ID="NetworkCommunicationsRadioButton" Style="z-index: 141; left: 168px; position: absolute; top: 208px; width: 137px; height: 23px;"
				TabIndex="6" runat="server" GroupName="Communications" Text="Optomux|Network Communications" CssClass="formfieldtitle"
				AutoPostBack="True" OnCheckedChanged="NetworkCommunicationsRadioButton_CheckedChanged"></FMControls:FMRadioButton>
			<FMControls:FMLabel ID="Label5" Style="z-index: 143; left: 16px; position: absolute; top: 232px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Optomux|IP Address:</FMControls:FMLabel>
			<asp:TextBox ID="IPAddressTextBox" Style="z-index: 145; left: 168px; position: absolute; top: 232px"
				runat="server" Width="168px" CssClass="formfield" TabIndex="7"></asp:TextBox>
			<FMControls:FMLabel ID="Label6" Style="z-index: 146; left: 16px; position: absolute; top: 264px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Optomux|Port:</FMControls:FMLabel>
			<asp:TextBox ID="PortTextBox" Style="z-index: 147; left: 168px; position: absolute; top: 264px"
				runat="server" CssClass="formfield" Width="64px" TabIndex="8"></asp:TextBox>
			<FMControls:FMLabel ID="Label10" Style="z-index: 114; left: 16px; position: absolute; top: 304px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Optomux|I/O Module:</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label14" Style="z-index: 117; left: 104px; position: absolute; top: 304px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">1</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label15" Style="z-index: 118; left: 136px; position: absolute; top: 304px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">2</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label16" Style="z-index: 119; left: 168px; position: absolute; top: 304px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">3</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label17" Style="z-index: 120; left: 200px; position: absolute; top: 304px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">4</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label18" Style="z-index: 121; left: 232px; position: absolute; top: 304px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">5</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label19" Style="z-index: 122; left: 264px; position: absolute; top: 304px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">6</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label20" Style="z-index: 123; left: 296px; position: absolute; top: 304px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">7</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label21" Style="z-index: 124; left: 328px; position: absolute; top: 304px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">8</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label11" Style="z-index: 115; left: 16px; position: absolute; top: 328px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Optomux|Input:</FMControls:FMLabel>
			<asp:RadioButton ID="Module1InputRadioButton" Style="z-index: 125; left: 96px; position: absolute; top: 328px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module1" TabIndex="9"></asp:RadioButton>
			<asp:RadioButton ID="Module2InputRadioButton" Style="z-index: 127; left: 128px; position: absolute; top: 328px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module2" TabIndex="11"></asp:RadioButton>
			<asp:RadioButton ID="Module3InputRadioButton" Style="z-index: 129; left: 160px; position: absolute; top: 328px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module3" TabIndex="14"></asp:RadioButton>
			<asp:RadioButton ID="Module4InputRadioButton" Style="z-index: 131; left: 192px; position: absolute; top: 328px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module4" TabIndex="16"></asp:RadioButton>
			<asp:RadioButton ID="Module5InputRadioButton" Style="z-index: 133; left: 224px; position: absolute; top: 328px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module5" TabIndex="18"></asp:RadioButton>
			<asp:RadioButton ID="Module6InputRadioButton" Style="z-index: 135; left: 256px; position: absolute; top: 328px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module6" TabIndex="20"></asp:RadioButton>
			<asp:RadioButton ID="Module7InputRadioButton" Style="z-index: 137; left: 288px; position: absolute; top: 328px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module7" TabIndex="22"></asp:RadioButton>
			<asp:RadioButton ID="Module8InputRadioButton" Style="z-index: 139; left: 320px; position: absolute; top: 328px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module8" TabIndex="24"></asp:RadioButton>
			<FMControls:FMLabel ID="Label13" Style="z-index: 116; left: 16px; position: absolute; top: 352px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Optomux|Output:</FMControls:FMLabel>
			<asp:RadioButton ID="Module1OutputRadioButton" Style="z-index: 126; left: 96px; position: absolute; top: 352px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module1" TabIndex="10"></asp:RadioButton>
			<asp:RadioButton ID="Module2OutputRadioButton" Style="z-index: 128; left: 128px; position: absolute; top: 352px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module2" TabIndex="13"></asp:RadioButton>
			<asp:RadioButton ID="Module3OutputRadioButton" Style="z-index: 130; left: 160px; position: absolute; top: 352px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module3" TabIndex="15"></asp:RadioButton>
			<asp:RadioButton ID="Module4OutputRadioButton" Style="z-index: 132; left: 192px; position: absolute; top: 352px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module4" TabIndex="17"></asp:RadioButton>
			<asp:RadioButton ID="Module5OutputRadioButton" Style="z-index: 134; left: 224px; position: absolute; top: 352px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module5" TabIndex="19"></asp:RadioButton>
			<asp:RadioButton ID="Module6OutputRadioButton" Style="z-index: 136; left: 256px; position: absolute; top: 352px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module6" TabIndex="21"></asp:RadioButton>
			<asp:RadioButton ID="Module7OutputRadioButton" Style="z-index: 138; left: 288px; position: absolute; top: 352px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module7" TabIndex="23"></asp:RadioButton>
			<asp:RadioButton ID="Module8OutputRadioButton" Style="z-index: 140; left: 320px; position: absolute; top: 352px"
				runat="server" CssClass="formfieldtitle" Text=" " GroupName="Module8" TabIndex="25"></asp:RadioButton>
			<FMControls:FMLabel ID="Label12" runat="server" BackColor="Transparent" Height="8px" ForeColor="Crimson"
				Width="146px" CssClass="formfieldtitle" Style="z-index: 111; left: 16px; position: absolute; top: 400px">Optomux|* Denotes Required Field</FMControls:FMLabel>
			<FMControls:FMButton ID="OKButton" runat="server" Width="88px" Text="Optomux|OK" Style="z-index: 109; left: 168px; position: absolute; top: 392px"
				TabIndex="100" CssClass="formfieldtitle"></FMControls:FMButton>
			<FMControls:FMButton ID="CancelButton" runat="server" Width="80px" Text="Optomux|Cancel" Style="z-index: 110; left: 272px; position: absolute; top: 392px"
				TabIndex="101" CssClass="formfieldtitle"></FMControls:FMButton>
				
			<script language="jscript">
				document.getElementById("OKButton").setActive();
				document.getElementById("IDTextBox").focus();
			</script>
		</div>
</form>
	</body>
</HTML>
