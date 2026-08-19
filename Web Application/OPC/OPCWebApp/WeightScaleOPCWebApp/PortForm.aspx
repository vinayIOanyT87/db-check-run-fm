<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="PortForm.aspx.cs" AutoEventWireup="true" Inherits="WeightScaleOPCWebApp.PortForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
		<title>PortForm</title>
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
			<asp:image id="Image1" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				ImageUrl="..\FMWebApp\images\fade.jpg" BackColor="Transparent"></asp:image><FMCONTROLS:FMButton id="CancelButton" style="Z-INDEX: 118; LEFT: 480px; POSITION: absolute; TOP: 208px"
				runat="server" Width="98px" Text="WeightScale|Cancel" CssClass="formfieldtitle" tabIndex="101"></FMCONTROLS:FMButton><FMCONTROLS:FMButton id="OKButton" style="Z-INDEX: 117; LEFT: 352px; POSITION: absolute; TOP: 208px"
				runat="server" Width="107px" Text="WeightScale|OK" CssClass="formfieldtitle" tabIndex="100"></FMCONTROLS:FMButton><asp:dropdownlist id="StopBitsDropDownList" style="Z-INDEX: 115; LEFT: 440px; POSITION: absolute; TOP: 112px"
				runat="server" Width="136px" CssClass="formfield" tabIndex="5"></asp:dropdownlist><FMCONTROLS:FMLABEL id="Label8" style="Z-INDEX: 114; LEFT: 280px; POSITION: absolute; TOP: 112px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">WeightScale|Stop Bits:</FMCONTROLS:FMLABEL>
				<asp:dropdownlist id="DataBitsDropDownList" style="Z-INDEX: 113; LEFT: 440px; POSITION: absolute; TOP: 80px"
				runat="server" Width="136px" CssClass="formfield" tabIndex="4"></asp:dropdownlist>
				<FMCONTROLS:FMLABEL id="Label7" style="Z-INDEX: 112; LEFT: 280px; POSITION: absolute; TOP: 80px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">WeightScale|Data Bits:</FMCONTROLS:FMLABEL><FMCONTROLS:FMDropdownlist id="ParityDropDownList" style="Z-INDEX: 111; LEFT: 440px; POSITION: absolute; TOP: 48px"
				runat="server" Width="136px" CssClass="formfield" tabIndex="3"></FMCONTROLS:FMDropdownlist><FMCONTROLS:FMLABEL id="Label6" style="Z-INDEX: 110; LEFT: 280px; POSITION: absolute; TOP: 48px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">WeightScale|Parity:</FMCONTROLS:FMLABEL><asp:dropdownlist id="BaudDropDownList" style="Z-INDEX: 109; LEFT: 88px; POSITION: absolute; TOP: 80px"
				runat="server" Width="136px" CssClass="formfield" tabIndex="2"></asp:dropdownlist>
			<FMCONTROLS:FMLABEL id="Label5" style="Z-INDEX: 108; LEFT: 16px; POSITION: absolute; TOP: 80px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">WeightScale|Baud:</FMCONTROLS:FMLABEL>
			<asp:DropDownList id="PortDropDownList" style="Z-INDEX: 119; LEFT: 88px; POSITION: absolute; TOP: 48px"
				runat="server" Width="136px" CssClass="formfield" tabIndex="1"></asp:DropDownList>
			<FMCONTROLS:FMLABEL id="Label2" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="312px" BackColor="Transparent">WeightScale|Port Configuration</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="Label1" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 48px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">WeightScale|ID:</FMCONTROLS:FMLABEL>
				
			<script language="jscript">
				document.getElementById("OKButton").setActive();
				document.getElementById("PortDropDownList").focus();
			</script>
            </div>
		</form>
	</body>
</HTML>
