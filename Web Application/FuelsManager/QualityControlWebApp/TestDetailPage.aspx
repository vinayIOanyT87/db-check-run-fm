<%@ Page language="c#" Codebehind="TestDetailPage.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.QualityControlWebApp.TestDetailPage"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body ms_positioning="GridLayout">

	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="TestTitleLabel"
				Style="z-index: 118; left: 16px; position: absolute; top: 8px" runat="server"
				BackColor="Transparent" CssClass="headline">Test and Inspections Detail Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label2" AssociatedControlID="TestNameTextbox" Style="z-index: 101; left: 56px; position: absolute; top: 53px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Test Name:</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label6"
				Style="z-index: 102; left: 156px; position: absolute; top: 52px; width: 12px;" runat="server"
				BackColor="Transparent" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>
			<asp:TextBox ID="TestNameTextbox" Style="z-index: 109; left: 176px; position: absolute; top: 48px" aria-required="true"
				runat="server" CssClass="formfield" Width="264px" MaxLength="80" TabIndex="1"></asp:TextBox>
			<FMControls:FMLabel ID="TestCodeLabel" Style="z-index: 101; left: 490px; position: absolute; top: 54px; width: 67px;" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" AssociatedControlID="TestCodeTextbox">Test Code:</FMControls:FMLabel>
			<asp:TextBox ID="TestCodeTextbox" Style="z-index: 109; left: 577px; position: absolute; top: 48px; width: 137px;" aria-required="true"
				runat="server" CssClass="formfield" MaxLength="80" TabIndex="8"></asp:TextBox>
			<FMControls:FMLabel ID="Label4" AssociatedControlID="MeasurementUnitTextbox" Style="z-index: 104; left: 56px; position: absolute; top: 85px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Measurement Unit:</FMControls:FMLabel>
			<asp:TextBox ID="MeasurementUnitTextbox"
				Style="z-index: 110; left: 176px; position: absolute; top: 80px" runat="server"
				CssClass="formfield" Width="264px" MaxLength="32" TabIndex="2"></asp:TextBox>
			<FMControls:FMLabel ID="TestMethodLabel" Style="z-index: 101; left: 490px; position: absolute; top: 86px; width: 81px;" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" AssociatedControlID="TestMethodTextbox">Test Method:</FMControls:FMLabel>
			<asp:TextBox ID="TestMethodTextbox" Style="z-index: 109; left: 577px; position: absolute; top: 80px; width: 137px;"
				runat="server" CssClass="formfield" MaxLength="80" TabIndex="9"></asp:TextBox>
			<FMControls:FMLabel ID="Label5" AssociatedControlID="SampleSizeTextbox" Style="z-index: 105; left: 56px; position: absolute; top: 117px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Sample Size:</FMControls:FMLabel>
			<asp:TextBox ID="SampleSizeTextbox" Style="z-index: 109; left: 176px; position: absolute; top: 112px"
				runat="server" CssClass="formfield" Width="264px" TabIndex="3"></asp:TextBox>
			<FMControls:FMLabel ID="ProductLabel" Style="z-index: 101; left: 490px; position: absolute; top: 118px;" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" AssociatedControlID="ProductDropDownList">Product:</FMControls:FMLabel>
			<FMControls:FMDropDownList ID="ProductDropDownList" Style="z-index: 109; left: 577px; position: absolute; top: 112px; width: 137px;"
				runat="server" CssClass="formfield" TabIndex="10" AutoPostBack="False">
			</FMControls:FMDropDownList>
			<FMControls:FMLabel ID="FMLABEL8"
				Style="z-index: 106; left: 56px; position: absolute; top: 150px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Validation Rules:</FMControls:FMLabel>
			<FMControls:FMLabel ID="FMLABEL1" AssociatedControlID="CurrentValueTextbox"
				Style="z-index: 106; left: 57px; position: absolute; top: 179px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Current Value</FMControls:FMLabel>
			<asp:TextBox ID="CurrentValueTextbox" Style="z-index: 109; left: 176px; position: absolute; top: 174px" aria-required="true"
				runat="server" CssClass="formfield" Width="264px" TabIndex="4" ReadOnly="true" MaxLength="64" Enabled="false"></asp:TextBox>
			<FMControls:FMButton ID="ClearAllButton"
				Style="z-index: 120; left: 613px; position: absolute; top: 177px; height: 25px; width: 100px;" runat="server"
				CssClass="formfieldtitle" Text="Clear All" OnClick="ClearAllCommand" TabIndex="11"></FMControls:FMButton>
			<FMControls:FMRadioButtonList ID="RuleTypeRadioButtonList" runat="server" CssClass="formfieldtitle" aria-required="true"
				Style="z-index: 109; left: 51px; position: absolute; top: 220px; height: 57px; width: 110px;"
				OnSelectedIndexChanged="RuleTypeRadioButtonListSelectedIndexChanged"
				AutoPostBack="True" ToolTip="Rule Type" aria-label="Rule Type">
				<asp:ListItem Value="Adjust" Selected="True">Adjust Rule:</asp:ListItem>
				<asp:ListItem Value="Range">Range</asp:ListItem>
			</FMControls:FMRadioButtonList>
			<asp:TextBox ID="AdjustRuleTextbox" ToolTip="Adjust rule" runat="server" CssClass="formfield" Width="264px"
				Style="z-index: 109; left: 177px; position: absolute; top: 224px"
				TabIndex="5"></asp:TextBox>
			<FMControls:FMButton ID="AppendButton"
				Style="z-index: 120; left: 613px; position: absolute; top: 212px; height: 25px; width: 100px; bottom: 313px;" runat="server"
				CssClass="formfieldtitle" Text="Append" OnClick="AppendCommand" TabIndex="12"></FMControls:FMButton>
			<FMControls:FMLabel ID="FMLABEL9"
				Style="z-index: 105; left: 115px; position: absolute; top: 258px; height: 17px; width: 32px;" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" AssociatedControlID="RangeFromTextBox">From:</FMControls:FMLabel>
			<asp:TextBox ID="RangeFromTextBox" ToolTip="Range from" runat="server" CssClass="formfield" Enabled="false"
				Style="z-index: 109; left: 177px; position: absolute; top: 253px; width: 90px;"
				TabIndex="6"></asp:TextBox>
			<FMControls:FMLabel ID="FMLABEL2"
				Style="z-index: 105; left: 324px; position: absolute; top: 257px; height: 8px;" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle" AssociatedControlID="RangeToTextbox">To:</FMControls:FMLabel>
			<asp:TextBox ID="RangeToTextbox" ToolTip="Range to" runat="server" CssClass="formfield" Enabled="false"
				Style="z-index: 109; left: 349px; position: absolute; top: 253px; width: 90px;"
				TabIndex="7"></asp:TextBox>
			<FMControls:FMButton ID="RemoveButton"
				Style="z-index: 120; left: 613px; position: absolute; top: 246px; height: 25px; width: 100px;" runat="server"
				CssClass="formfieldtitle" Text="Remove" OnClick="RemoveCommand"
				TabIndex="13"></FMControls:FMButton>

			<FMControls:FMLabel ID="FMLABEL7" Style="z-index: 122; left: 150px; position: absolute; top: 304px; width: 157px;"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle"
				ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel>
			<FMControls:FMButton ID="New"
				Style="z-index: 119; left: 385px; position: absolute; top: 299px; min-width:100px" runat="server"
				CssClass="formfieldtitle" Text="New" Width="66px" OnClick="NewCommand"
				TabIndex="14"></FMControls:FMButton>
			<FMControls:FMButton ID="OK"
				Style="z-index: 120; left: 495px; position: absolute; top: 299px; min-width:100px" runat="server"
				CssClass="formfieldtitle" Text="OK" Width="66px" OnClick="OkCommand" TabIndex="15"></FMControls:FMButton>
			<FMControls:FMButton ID="Cancel"
				Style="z-index: 121; left: 615px; position: absolute; top: 299px; min-width:100px" runat="server"
				CssClass="formfieldtitle" Text="Cancel" OnClick="CancelCommand" TabIndex="16"></FMControls:FMButton>

			<!-- ==================================================================== -->
			<!--                        Client side code                              -->
			<!-- ==================================================================== -->
			<script type="text/javascript">
				if (Form1.TestNameTextbox.readOnly == false) {
					Form1.TestNameTextbox.setActive();
					Form1.TestNameTextbox.focus();
				}
				else {
					Form1.MeasurementUnitTextbox.setActive();
					Form1.MeasurementUnitTextbox.focus();
				}
			</script>
		</div>
	</form>
</body>
</html>
