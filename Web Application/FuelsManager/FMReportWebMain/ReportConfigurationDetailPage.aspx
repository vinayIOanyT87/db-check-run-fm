<%@ Page Language="c#" CodeBehind="ReportConfigurationDetailPage.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMReportWebMain.ReportConfigurationDetailPage" %>
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
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body ms_positioning="GridLayout">
	<form id="Form1" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">
			<FMControls:FMPageFadeImage></FMControls:FMPageFadeImage>
			<asp:Label ID="ReportDetailLabel" Style="z-index: 111; left: 16px; position: absolute; top: 16px"
				runat="server" CssClass="headline" Width="500px">Report Detail Configuration</asp:Label>
			<div style="z-index: 101; left: 16px; position: absolute; top: 56px">
				<asp:Label ID="ReportNameLabel" AssociatedControlID="ReportNameTextBox" runat="server" CssClass="formfieldtitle">Report Name</asp:Label>
				<span style="color: red;">*</span>
			</div>
			<asp:TextBox ID="ReportNameTextBox" Style="z-index: 104; left: 208px; position: absolute; top: 56px" aria-required="true"
				runat="server" CssClass="formfield" Width="525px" MaxLength="60" TabIndex="1"></asp:TextBox>
			<div style="z-index: 102; left: 16px; position: absolute; top: 88px">
				<asp:Label ID="ReportPathLabel" AssociatedControlID="ReportPathDropDownList" runat="server" CssClass="formfieldtitle">Report Path</asp:Label>
				<span style="color: red;">*</span>
			</div>
			<asp:DropDownList ID="ReportPathDropDownList" Style="z-index: 115; left: 208px; position: absolute; top: 88px" aria-required="true"
				runat="server" CssClass="formfield" Width="526px" TabIndex="2">
			</asp:DropDownList>
			<div style="z-index: 103; left: 16px; position: absolute; top: 120px">
				<asp:Label ID="ReportDescriptionLabel" AssociatedControlID="ReportDescriptionTextBox" runat="server" CssClass="formfieldtitle">Report Description</asp:Label>
				<span style="color: red;">*</span>
			</div>
			<FMControls:FMTextBox ID="ReportDescriptionTextBox" Style="z-index: 106; left: 208px; position: absolute; top: 120px" aria-required="true"
				runat="server" CssClass="formfield" Width="526px" Height="64px" MaxLength="255"
				TabIndex="3" Wrap="True" TextMode="MultiLine"></FMControls:FMTextBox>
			<asp:Label ID="GroupAssociationLabel" AssociatedControlID="GroupDropDownList" Style="z-index: 107; left: 16px; position: absolute; top: 200px"
				runat="server" CssClass="formfieldtitle">Assigned Group</asp:Label>
			<asp:DropDownList ID="GroupDropDownList" Style="z-index: 112; left: 208px; position: absolute; top: 200px"
				runat="server" CssClass="formfield" Width="521px" TabIndex="4">
			</asp:DropDownList>
			<asp:CheckBox ID="ReportForPrintOnlyCheckBox" Style="z-index: 120; left: 16px; position: absolute; top: 232px"
				runat="server" CssClass="formfieldtitle" Text="Report For Print Only" TabIndex="10"></asp:CheckBox>
			<asp:CheckBox ID="PrintAtEndOfDayCheckBox" Style="z-index: 118; left: 16px; position: absolute; top: 264px"
				runat="server" CssClass="formfieldtitle" Text="Print At End Of Day" TabIndex="5"></asp:CheckBox>
			<asp:CheckBox ID="PrintAtEndOfMonthCheckBox" Style="z-index: 119; left: 16px; position: absolute; top: 296px"
				runat="server" CssClass="formfieldtitle" Text="Print At End Of Month" TabIndex="6"></asp:CheckBox>
			<asp:CheckBox ID="DWReportCheckBox" Style="z-index: 120; left: 16px; position: absolute; top: 328px"
				runat="server" CssClass="formfieldtitle" Text="Data Warehouse Report" TabIndex="6"></asp:CheckBox>
			<asp:Label ID="PrinterName1Label" AssociatedControlID="PrimaryPrinterDropDownList" Style="z-index: 116; left: 16px; position: absolute; top: 368px"
				runat="server" CssClass="formfieldtitle">Primary Printer Name</asp:Label>
			<asp:DropDownList ID="PrimaryPrinterDropDownList" Style="z-index: 122; left: 210px; position: absolute; top: 368px"
				runat="server" CssClass="formfield" Width="517px">
			</asp:DropDownList>
			<asp:Label ID="PrinterName2Label" AssociatedControlID="SecondaryPrinterDropDownList" Style="z-index: 117; left: 16px; position: absolute; top: 400px"
				runat="server" CssClass="formfieldtitle">Secondary Printer Name</asp:Label>
			<asp:DropDownList ID="SecondaryPrinterDropDownList" Style="z-index: 123; left: 210px; position: absolute; top: 400px"
				runat="server" CssClass="formfield" Width="517px">
			</asp:DropDownList>
			<asp:Label ID="AssignedUserGroupsLabel" AssociatedControlID="AssignedUserGroupsListBox" Style="z-index: 120; left: 208px; position: absolute; top: 436px; right: 637px; width: 209px;"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle">Assigned User Groups:</asp:Label>
			<asp:Label ID="UnassignedUserGroupsLabel" AssociatedControlID="UnassignedUserGroupsListBox" Style="z-index: 112; left: 496px; position: absolute; top: 436px; width: 188px;"
				runat="server" CssClass="formfieldtitle">Unassigned User Groups:</asp:Label>
			<asp:ListBox ID="AssignedUserGroupsListBox" Style="z-index: 121; left: 208px; position: absolute; top: 468px"
				TabIndex="15" runat="server" BackColor="White" Width="240px" CssClass="formfield" Height="88px"
				SelectionMode="Multiple"></asp:ListBox>
			<asp:ListBox ID="UnassignedUserGroupsListBox" Style="z-index: 124; left: 496px; position: absolute; top: 468px"
				TabIndex="11" runat="server" BackColor="White" Width="240px" CssClass="formfield" Height="88px"
				SelectionMode="Multiple"></asp:ListBox>
			<asp:Button ID="AssignGroupsButton" Style="z-index: 122; left: 459px; position: absolute; top: 479px; padding-left: 1px; padding-right: 1px; width:20px;"
				TabIndex="9" runat="server" CssClass="formfieldtitle" Text="<<"></asp:Button>
			<asp:Button ID="UnassignGroupsButton" Style="z-index: 123; left: 459px; position: absolute; top: 517px; padding-left: 1px; padding-right: 1px; width:20px;"
				TabIndex="10" runat="server" CssClass="formfieldtitle" Text=">>"></asp:Button>
			<asp:Button ID="SaveButton" Style="z-index: 109; left: 495px; position: absolute; top: 582px; min-width: 100px; padding-left:3px;padding-right:3px;"
				runat="server" CssClass="formfieldtitle" Text="OK" OnClick="SaveButtonOnClick"></asp:Button>
			<asp:Button ID="CancelButton" Style="z-index: 110; left: 625px; position: absolute; top: 582px; min-width: 100px; padding-left: 3px"
				runat="server" CssClass="formfieldtitle" Text="Cancel"
				OnClick="CancelButtonOnClick"></asp:Button>
		</div>
	</form>
	<script type="text/javascript">
        document.getElementById("ReportNameTextBox").focus();
    </script>
</body>
</html>
