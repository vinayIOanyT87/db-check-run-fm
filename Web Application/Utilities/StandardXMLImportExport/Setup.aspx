<%@ Page language="c#" Codebehind="Setup.aspx.cs" AutoEventWireup="false" Inherits="StandardXMLImportExport.Setup" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Setup</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../css/FuelsManager.css" rel="stylesheet">
	</HEAD>
	<body text="#000e" MS_POSITIONING="GridLayout">
		<asp:image id="FadeImage" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" BackColor="Transparent"
			ImageUrl="../FMWebApp/images/Page_Fade_7.jpg" runat="server"></asp:image>
		<form id="Form1" method="post" runat="server">
			<asp:Label ID="ConfigNameLabel" Style="z-index: 110; left: 8px; position: absolute; top: 16px"
				runat="server" CssClass="formfieldtitle">Configuration Name</asp:Label>
			<asp:TextBox ID="ConfigNameTextBox" Style="z-index: 111; left: 8px; position: absolute; top: 40px"
				runat="server" ReadOnly="True" CssClass="formfieldtitle"></asp:TextBox>
			<asp:CheckBox ID="IncludeDeletedTransactionsCheckBox" Style="z-index: 112; left: 8px; position: absolute; top: 96px"
				runat="server" Text="Include Deleted Transactions" Width="224px" CssClass="formfieldtitle"></asp:CheckBox>
			<asp:Label ID="AvailableLabel" Style="z-index: 116; left: 176px; position: absolute; top: 136px"
				runat="server" CssClass="formfieldtitle">Available</asp:Label>
			<asp:Label ID="AssignedLabel" Style="z-index: 117; left: 472px; position: absolute; top: 136px"
				runat="server" CssClass="formfieldtitle">Assigned</asp:Label>
			<asp:Label ID="FilterByLabel" Style="z-index: 113; left: 8px; position: absolute; top: 160px"
				runat="server" CssClass="formfieldtitle"> Filter by</asp:Label>
			<iewc:TreeView ID="AvailableTreeView" Style="z-index: 114; left: 184px; position: absolute; top: 160px"
				runat="server" Width="192px" AutoPostBack="True" Height="352px" CssClass="formfieldtitle"></iewc:TreeView>
			<iewc:TreeView ID="AssignedTreeView" Style="z-index: 115; left: 472px; position: absolute; top: 160px"
				runat="server" Width="192px" AutoPostBack="True" Height="352px"></iewc:TreeView>
			<asp:CheckBox ID="ManagerCheckBox" Style="z-index: 104; left: 8px; position: absolute; top: 184px"
				runat="server" Text="Manager" AutoPostBack="True" Checked="True" CssClass="formfieldtitle"></asp:CheckBox>
			<asp:CheckBox ID="OwnerCheckBox" Style="z-index: 108; left: 8px; position: absolute; top: 208px"
				runat="server" Text="Owner" AutoPostBack="True" Checked="True" CssClass="formfieldtitle"></asp:CheckBox>
			<asp:CheckBox ID="ProductCheckBox" Style="z-index: 107; left: 8px; position: absolute; top: 232px"
				runat="server" Text="Product" AutoPostBack="True" Checked="True" CssClass="formfieldtitle"></asp:CheckBox>
			<asp:Button ID="AssignButton" Style="z-index: 118; left: 408px; position: absolute; top: 240px"
				runat="server" Text=">>" Enabled="False" CssClass="formfieldtitle"></asp:Button>
			<asp:CheckBox ID="ConsumerCheckBox" Style="z-index: 103; left: 8px; position: absolute; top: 256px"
				runat="server" Text="Consumer" AutoPostBack="True" Checked="True" CssClass="formfieldtitle"></asp:CheckBox>
			<asp:CheckBox ID="CarrierCheckBox" Style="z-index: 101; left: 8px; position: absolute; top: 280px"
				runat="server" Text="Carrier" AutoPostBack="True" Checked="True" CssClass="formfieldtitle"></asp:CheckBox>
			<asp:Button ID="UnassignButton" Style="z-index: 119; left: 408px; position: absolute; top: 280px"
				runat="server" Text="<<" Enabled="False" CssClass="formfieldtitle"></asp:Button>
			<asp:CheckBox ID="SupplierCheckBox" Style="z-index: 102; left: 8px; position: absolute; top: 304px"
				runat="server" Text="Supplier" AutoPostBack="True" Checked="True" CssClass="formfieldtitle"></asp:CheckBox>
			<asp:CheckBox ID="TransactionTypeCheckBox" Style="z-index: 106; left: 8px; position: absolute; top: 328px"
				runat="server" Text="Transaction Type" AutoPostBack="True" Checked="True" CssClass="formfieldtitle"></asp:CheckBox>
			<asp:Button ID="OKButton" Style="z-index: 109; left: 312px; position: absolute; top: 536px"
				runat="server" Text="OK" Width="48px" CssClass="formfieldtitle"></asp:Button>
		</form>
	</body>
</HTML>
