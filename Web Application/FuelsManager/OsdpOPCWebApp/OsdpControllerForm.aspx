<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="OsdpControllerForm.aspx.cs" AutoEventWireup="True" Inherits="OPCWebApp.OsdpOPCWebApp.OsdpControllerForm" %>
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
			<FMControls:FMLabel ID="Label4" Style="z-index: 107; left: 16px; position: absolute; top: 80px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Optomux|Port:</FMControls:FMLabel>
			<asp:DropDownList ID="PortDropDownList" Style="z-index: 108; left: 168px; position: absolute; top: 80px"
				runat="server" Width="169px" CssClass="formfield" TabIndex="4">
			</asp:DropDownList>
			<FMControls:FMLabel ID="Label9" Style="z-index: 112; left: 16px; position: absolute; top: 112px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Optomux|Address:</FMControls:FMLabel>
			<asp:DropDownList ID="AddressDropDownList" Style="z-index: 113; left: 168px; position: absolute; top: 112px"
				runat="server" CssClass="formfield" Width="169px" TabIndex="5">
			</asp:DropDownList>
			<FMControls:FMLabel ID="Label12" runat="server" BackColor="Transparent" Height="8px" ForeColor="Crimson"
				Width="146px" CssClass="formfieldtitle" Style="z-index: 111; left: 16px; position: absolute; top: 200px">Optomux|* Denotes Required Field</FMControls:FMLabel>
			<FMControls:FMButton ID="OKButton" runat="server" Width="88px" Text="Optomux|OK" Style="z-index: 109; left: 168px; position: absolute; top: 192px"
				TabIndex="100" CssClass="formfieldtitle"></FMControls:FMButton>
			<FMControls:FMButton ID="CancelButton" runat="server" Width="80px" Text="Optomux|Cancel" Style="z-index: 110; left: 272px; position: absolute; top: 192px"
				TabIndex="101" CssClass="formfieldtitle"></FMControls:FMButton>
				
			<script language="jscript">
				document.getElementById("OKButton").setActive();
				document.getElementById("IDTextBox").focus();
			</script>
		</div>
</form>
	</body>
</HTML>
