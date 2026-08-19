<%@ Page Language="c#" CodeBehind="GeneralConfiguration.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.GeneralConfiguration" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html>
<head>
	<title></title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body ms_positioning="GridLayout">
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<asp:Label ID="GeneralConfigTitleLabel" Style="z-index: 102; left: 8px; position: absolute; top: 8px"
				runat="server" Width="400px" CssClass="headline">General Configuration</asp:Label>
			<asp:Panel ID="GeneralPanel"
				Style="z-index: 103; left: 8px; position: absolute; top: 40px" runat="server"
				Width="664px" Height="104px" BorderColor="LightSteelBlue" BorderStyle="Solid"
				BorderWidth="1px">
			</asp:Panel>
			<asp:Label ID="GeneralLabel" Style="z-index: 108; left: 312px; position: absolute; top: 48px"
				runat="server" CssClass="formfieldtitle">General</asp:Label>
			<asp:Label ID="ForceDateLabel" AssociatedControlID="NumOfDaysDropdown" Style="z-index: 111; left: 24px; position: absolute; top: 73px; width: 117px;"
				runat="server" CssClass="formfieldtitle">Force closeout after:</asp:Label>
			<asp:DropDownList ID="NumOfDaysDropdown" Style="z-index: 112; left: 160px; position: absolute; top: 71px"
				runat="server" CssClass="formfieldtitle" TabIndex="4">
			</asp:DropDownList>
			<asp:Label ID="ReverseTrxDateLabel" Style="z-index: 106; left: 344px; position: absolute; top: 72px; bottom: 526px;"
				runat="server" CssClass="formfieldtitle">Reverse Transaction Date:</asp:Label>
			<asp:RadioButtonList ID="ReverseTrxDateRadioButtonList" Style="z-index: 107; left: 336px; position: absolute; top: 88px"
				runat="server" Width="248px" CssClass="formfieldtitle" Height="40px" TabIndex="3">
				<asp:ListItem Value="Current" Selected="True">Current Date</asp:ListItem>
				<asp:ListItem Value="Original">Date of the Original Transaction</asp:ListItem>
			</asp:RadioButtonList>
			<asp:CheckBox ID="ShowDeletedCheckBox" Style="z-index: 105; left: 16px; position: absolute; top: 97px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle"
				BorderColor="Transparent" BorderStyle="None" Text="Show Deleted Transactions"
				TabIndex="2"></asp:CheckBox>
			<p>
				<asp:CheckBox ID="SetBeginInvCheckBox" Style="z-index: 105; left: 16px; position: absolute; top: 116px"
					runat="server" BackColor="Transparent" CssClass="formfieldtitle"
					BorderColor="Transparent" BorderStyle="None" Text="Set Begin Inventory to zero"
					TabIndex="2"></asp:CheckBox>
			</p>
			<asp:Panel ID="ExstarsPanel" Style="z-index: 113; left: 8px; position: absolute; top: 153px"
				runat="server" Width="664px" Height="64px" BorderColor="LightSteelBlue" BorderStyle="Solid"
				BorderWidth="1px">
			</asp:Panel>
			<asp:Label ID="ExstarsLabel" Style="z-index: 114; left: 296px; position: absolute; top: 163px"
				runat="server" CssClass="formfieldtitle" Enabled="False">ExSTARS</asp:Label>
			<asp:Label ID="SecurityCodeLabel" AssociatedControlID="SecurityCodeTextBox" Style="z-index: 115; left: 16px; position: absolute; top: 187px"
				runat="server" CssClass="formfieldtitle" Enabled="False">Security Code:</asp:Label>
			<asp:TextBox ID="SecurityCodeTextBox" Style="z-index: 116; left: 112px; position: absolute; top: 187px"
				runat="server" Width="168px" CssClass="formfieldtitle" TabIndex="5" Enabled="False"></asp:TextBox>
			<asp:Label ID="AuthorizationCodeLabel" AssociatedControlID="AuthorizationCodeTextBox" Style="z-index: 117; left: 304px; position: absolute; top: 187px"
				runat="server" CssClass="formfieldtitle" Enabled="False">Authorization Code:</asp:Label>
			<asp:TextBox ID="AuthorizationCodeTextBox" Style="z-index: 118; left: 440px; position: absolute; top: 187px"
				runat="server" Width="208px" CssClass="formfieldtitle" TabIndex="6" Enabled="False"></asp:TextBox>
			<asp:Panel ID="AdjustmentPanel" Style="z-index: 119; left: 8px; position: absolute; top: 227px"
				runat="server" Width="664px" Height="184px" BorderColor="LightSteelBlue" BorderStyle="Solid"
				BorderWidth="1px">
			</asp:Panel>
			<asp:Label ID="AdjustmentLabel" Style="z-index: 120; left: 256px; position: absolute; top: 233px"
				runat="server" CssClass="formfieldtitle" Enabled="False">Adjustment Distribution</asp:Label>
			<asp:Label ID="AdjustmentMethodsLabel" Style="z-index: 121; left: 16px; position: absolute; top: 257px"
				runat="server" CssClass="formfieldtitle" Enabled="False">Adjustment Distribution Methods:</asp:Label>
			<asp:Label ID="AssignedLabel" AssociatedControlID="AssignedListBox" Style="z-index: 124; left: 328px; position: absolute; top: 257px"
				runat="server" CssClass="formfieldtitle" Enabled="False">Assigned</asp:Label>
			<asp:Label ID="UnassignedLabel" AssociatedControlID="UnassignedListBox" Style="z-index: 125; left: 544px; position: absolute; top: 257px"
				runat="server" CssClass="formfieldtitle" Enabled="False">Unassigned</asp:Label>
			<asp:RadioButtonList ID="MethodsRadioButtonList" Style="z-index: 122; left: 16px; position: absolute; top: 273px"
				runat="server" CssClass="formfieldtitle" AutoPostBack="True" TabIndex="7" Enabled="False" OnSelectedIndexChanged="AdjMethodsOnClick">
				<asp:ListItem Value="Allocation">Allocation</asp:ListItem>
				<asp:ListItem Value="Throughput">Throughput</asp:ListItem>
				<asp:ListItem Value="Manual" Selected="True">Manual</asp:ListItem>
			</asp:RadioButtonList>
			<asp:ListBox ID="AssignedListBox" Style="z-index: 126; left: 288px; position: absolute; top: 273px"
				runat="server" Width="152px" CssClass="formfieldtitle" Height="128px" TabIndex="9" Enabled="False"></asp:ListBox>
			<asp:ListBox ID="UnassignedListBox" Style="z-index: 127; left: 496px; position: absolute; top: 273px"
				runat="server" Width="153px" CssClass="formfieldtitle" Height="128px" TabIndex="12" Enabled="False"></asp:ListBox>
			<asp:Button ID="AssignButton" Style="z-index: 128; left: 456px; position: absolute; top: 297px"
				runat="server" CssClass="formfieldtitle" Text="<<" TabIndex="10" Enabled="False" OnClick="AssignBtnOnClick"></asp:Button>
			<asp:Button ID="UnassignButton" Style="z-index: 129; left: 456px; position: absolute; top: 332px"
				runat="server" CssClass="formfieldtitle" Text=">>" TabIndex="11" Enabled="False" OnClick="UnassignBtnOnClick"></asp:Button>
			<asp:CheckBox ID="ConsortiumCheckBox" Style="z-index: 123; left: 16px; position: absolute; top: 353px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle" BorderColor="Transparent"
				Text="Use consortium members only?" TabIndex="8" Enabled="False"></asp:CheckBox>
			<asp:Button ID="OK" Style="z-index: 130; left: 210px; position: absolute; top: 425px; min-width: 100px" runat="server"
				CssClass="formfieldtitle" Text="Apply" TabIndex="13" OnClick="OkBtnOnClick"></asp:Button>
			<asp:Button ID="CancelButton" Style="z-index: 132; left: 344px; position: absolute; top: 425px; min-width: 100px"
				runat="server" CssClass="formfieldtitle" Text="Cancel" TabIndex="14" OnClick="CancelBtnOnClick"></asp:Button>
			<script type="text/javascript">
				document.getElementById("OK").setActive();
				document.getElementById("NumOfDaysDropdown").focus();
			</script>
		</div>
	</form>
</body>
</html>
