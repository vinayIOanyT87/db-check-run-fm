<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="PortForm.aspx.cs" AutoEventWireup="true" Inherits="OPCWebApp.AcculoadOPCWebApp.PortForm" %>
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
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 101; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="312px" BackColor="Transparent">SmithMeter|Port Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label1" Style="z-index: 102; left: 16px; position: absolute; top: 48px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|ID:</FMControls:FMLabel>
			<asp:DropDownList ID="PortDropDownList" Style="z-index: 119; left: 88px; position: absolute; top: 48px"
				runat="server" Width="136px" CssClass="formfield" TabIndex="1">
			</asp:DropDownList>
			<FMControls:FMLabel ID="Label6" Style="z-index: 110; left: 280px; position: absolute; top: 48px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|Parity:</FMControls:FMLabel>
			<FMControls:FMDropDownList ID="ParityDropDownList" Style="z-index: 111; left: 440px; position: absolute; top: 48px"
				runat="server" Width="136px" CssClass="formfield" TabIndex="3">
			</FMControls:FMDropDownList>
			<FMControls:FMLabel ID="Label5" Style="z-index: 108; left: 16px; position: absolute; top: 80px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|Baud:</FMControls:FMLabel>
			<asp:DropDownList ID="BaudDropDownList" Style="z-index: 109; left: 88px; position: absolute; top: 80px"
				runat="server" Width="136px" CssClass="formfield" TabIndex="2">
			</asp:DropDownList>
			<FMControls:FMLabel ID="Label7" Style="z-index: 112; left: 280px; position: absolute; top: 80px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|Data Bits:</FMControls:FMLabel>
			<asp:DropDownList ID="DataBitsDropDownList" Style="z-index: 113; left: 440px; position: absolute; top: 80px"
				runat="server" Width="136px" CssClass="formfield" TabIndex="4">
			</asp:DropDownList>
			<FMControls:FMLabel ID="Label8" Style="z-index: 114; left: 280px; position: absolute; top: 112px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">SmithMeter|Stop Bits:</FMControls:FMLabel>
			<asp:DropDownList ID="StopBitsDropDownList" Style="z-index: 115; left: 440px; position: absolute; top: 112px"
				runat="server" Width="136px" CssClass="formfield" TabIndex="5">
			</asp:DropDownList>
			<FMControls:FMButton ID="OKButton" Style="z-index: 117; left: 352px; position: absolute; top: 208px"
				runat="server" Width="107px" Text="SmithMeter|OK" CssClass="formfieldtitle" TabIndex="100"></FMControls:FMButton>
			<FMControls:FMButton ID="CancelButton" Style="z-index: 118; left: 480px; position: absolute; top: 208px"
				runat="server" Width="98px" Text="SmithMeter|Cancel" CssClass="formfieldtitle" TabIndex="101"></FMControls:FMButton>
				
			<script language="jscript">
				document.getElementById("OKButton").setActive();
				document.getElementById("PortDropDownList").focus();
			</script>
		</div>
</form>
	</body>
</HTML>
