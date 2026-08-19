<%@ Page Language="c#" AutoEventWireup="True" Codebehind="RegionalSettingsForm.aspx.cs" Inherits="FuelsManager.FMWebApp.RegionalSettingsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" %>
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
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="Form1" method="post" enctype="multipart/form-data" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<FMCONTROLS:FMLABEL id="ConfigurationLabel" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" CssClass="headline" BackColor="Transparent" Width="750px" />
			<asp:image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image>
			<FMControls:FMLabel id="Label1" AssociatedControlID="DecimalSymbolDropDownList" style="Z-INDEX: 116; LEFT: 24px; POSITION: absolute; TOP: 48px" runat="server"
				CssClass="formfieldtitle">Decimal Symbol:</FMControls:FMLabel>
			<asp:dropdownlist id="DecimalSymbolDropDownList" style="Z-INDEX: 108; LEFT: 160px; POSITION: absolute; TOP: 48px"
				tabIndex="1" runat="server" CssClass="formfield" AutoPostBack="True" Width="112px" onselectedindexchanged="DecimalSymbolDropDownListSelectedIndexChanged"></asp:dropdownlist>
			<FMControls:FMLabel id="Label2" AssociatedControlID="DigitGroupingSymbolDropDownList" style="Z-INDEX: 104; LEFT: 24px; POSITION: absolute; TOP: 72px" runat="server"
				CssClass="formfieldtitle">Digit Grouping Symbol:</FMControls:FMLabel>
			<asp:dropdownlist id="DigitGroupingSymbolDropDownList" style="Z-INDEX: 122; LEFT: 160px; POSITION: absolute; TOP: 72px"
				tabIndex="2" runat="server" CssClass="formfield" AutoPostBack="True" Width="112px" onselectedindexchanged="DigitGroupingSymbolDropDownListSelectedIndexChanged"></asp:dropdownlist>
			<FMControls:FMLabel id="Label3" AssociatedControlID="DigitGroupingDropDownList" style="Z-INDEX: 121; LEFT: 24px; POSITION: absolute; TOP: 96px" runat="server"
				CssClass="formfieldtitle">Digit Grouping:</FMControls:FMLabel>
			<asp:dropdownlist id="DigitGroupingDropDownList" style="Z-INDEX: 110; LEFT: 160px; POSITION: absolute; TOP: 96px"
				tabIndex="3" runat="server" CssClass="formfield" AutoPostBack="True" Width="112px" onselectedindexchanged="DigitGroupingDropDownListSelectedIndexChanged"></asp:dropdownlist>
			<FMControls:FMLabel id="Label6" AssociatedControlID="SampleNumberFormat1TextBox" style="Z-INDEX: 118; LEFT: 24px; POSITION: absolute; TOP: 120px" runat="server"
				CssClass="formfieldtitle">Sample Format:</FMControls:FMLabel>
			<asp:textbox id="SampleNumberFormat1TextBox" style="Z-INDEX: 112; LEFT: 160px; POSITION: absolute; TOP: 120px"
				runat="server" CssClass="formfield" AutoPostBack="True" Width="112px" BackColor="#E0E0E0"
				Enabled="False"></asp:textbox>
			<asp:textbox id="SampleNumberFormat2TextBox" ToolTip="Sample number box 2" style="Z-INDEX: 123; LEFT: 160px; POSITION: absolute; TOP: 144px"
				runat="server" CssClass="formfield" AutoPostBack="True" Width="112px" BackColor="#E0E0E0"
				Enabled="False"></asp:textbox>
			<FMControls:FMLabel id="Label5" AssociatedControlID="ListSeparatorTextBox" style="Z-INDEX: 106; LEFT: 24px; POSITION: absolute; TOP: 176px" runat="server"
				CssClass="formfieldtitle">List Separator:</FMControls:FMLabel>
			<asp:textbox id="ListSeparatorTextBox" style="Z-INDEX: 127; LEFT: 160px; POSITION: absolute; TOP: 176px"
				tabIndex="4" runat="server" CssClass="formfield" AutoPostBack="True" Width="112px" MaxLength="1" ontextchanged="ListSeparatorTextChanged"></asp:textbox>
			<FMControls:FMLabel id="Label4" AssociatedControlID="TimePatternDropDownList" style="Z-INDEX: 114; LEFT: 328px; POSITION: absolute; TOP: 48px" runat="server"
				CssClass="formfieldtitle">Time Format:</FMControls:FMLabel>
			<asp:dropdownlist id="TimePatternDropDownList" style="Z-INDEX: 120; LEFT: 440px; POSITION: absolute; TOP: 48px"
				tabIndex="5" runat="server" CssClass="formfield" AutoPostBack="True" Width="208px" onselectedindexchanged="TimePatternDropDownListSelectedIndexChanged"></asp:dropdownlist>
			<FMControls:FMLabel id="Label7" AssociatedControlID="TimeSeparatorTextBox" style="Z-INDEX: 102; LEFT: 328px; POSITION: absolute; TOP: 72px" runat="server"
				CssClass="formfieldtitle">Time Separator:</FMControls:FMLabel>
			<asp:textbox id="TimeSeparatorTextBox" style="Z-INDEX: 103; LEFT: 440px; POSITION: absolute; TOP: 72px"
				tabIndex="6" runat="server" CssClass="formfield" AutoPostBack="True" Width="208px" MaxLength="1" ontextchanged="TimeSeparatorTextBoxTextChanged"></asp:textbox>
			<FMControls:FMLabel id="Label8" AssociatedControlID="AMSymbolTextBox" style="Z-INDEX: 105; LEFT: 328px; POSITION: absolute; TOP: 96px" runat="server"
				CssClass="formfieldtitle">AM Symbol:</FMControls:FMLabel>
			<asp:textbox id="AMSymbolTextBox" style="Z-INDEX: 107; LEFT: 440px; POSITION: absolute; TOP: 96px"
				tabIndex="7" runat="server" CssClass="formfield" AutoPostBack="True" Width="208px" MaxLength="2" ontextchanged="AmSymbolTextBoxTextChanged"></asp:textbox>
			<FMControls:FMLabel id="Label9" AssociatedControlID="PMSymbolTextBox" style="Z-INDEX: 109; LEFT: 328px; POSITION: absolute; TOP: 120px" runat="server"
				CssClass="formfieldtitle">PM Symbol:</FMControls:FMLabel>
			<asp:textbox id="PMSymbolTextBox" style="Z-INDEX: 111; LEFT: 440px; POSITION: absolute; TOP: 120px"
				tabIndex="8" runat="server" CssClass="formfield" AutoPostBack="True" Width="208px" MaxLength="2" ontextchanged="PmSymbolTextBoxTextChanged"></asp:textbox>
			<FMControls:FMLabel id="Label10" AssociatedControlID="SampleTimeFormatTextBox" style="Z-INDEX: 113; LEFT: 328px; POSITION: absolute; TOP: 144px" runat="server"
				CssClass="formfieldtitle">Sample Format:</FMControls:FMLabel>
			<asp:textbox id="SampleTimeFormatTextBox" style="Z-INDEX: 115; LEFT: 440px; POSITION: absolute; TOP: 144px"
				runat="server" CssClass="formfield" Width="208px" BackColor="#E0E0E0" Enabled="False"></asp:textbox>
			<FMControls:FMLabel id="Label11" AssociatedControlID="ShortDatePatternDropDownList" style="Z-INDEX: 117; LEFT: 328px; POSITION: absolute; TOP: 184px" runat="server"
				CssClass="formfieldtitle">Short Date Format:</FMControls:FMLabel>
			<asp:dropdownlist id="ShortDatePatternDropDownList" style="Z-INDEX: 119; LEFT: 440px; POSITION: absolute; TOP: 184px"
				tabIndex="9" runat="server" CssClass="formfield" AutoPostBack="True" Width="208px" onselectedindexchanged="ShortDatePatternDropDownListSelectedIndexChanged"></asp:dropdownlist>
			<FMControls:FMLabel id="Label12" AssociatedControlID="DateSeparatorTextBox" style="Z-INDEX: 124; LEFT: 328px; POSITION: absolute; TOP: 208px" runat="server"
				CssClass="formfieldtitle">Date Separator:</FMControls:FMLabel>
			<asp:textbox id="DateSeparatorTextBox" style="Z-INDEX: 128; LEFT: 440px; POSITION: absolute; TOP: 208px"
				tabIndex="10" runat="server" CssClass="formfield" AutoPostBack="True" Width="208px" MaxLength="1" ontextchanged="DateSeparatorTextBoxTextChanged"></asp:textbox>
			<FMControls:FMLabel AssociatedControlID="SampleShortDateTextBox" id="Label13" style="Z-INDEX: 126; LEFT: 328px; POSITION: absolute; TOP: 232px" runat="server"
				CssClass="formfieldtitle">Sample Format:</FMControls:FMLabel>
			<asp:textbox id="SampleShortDateTextBox" style="Z-INDEX: 101; LEFT: 440px; POSITION: absolute; TOP: 232px"
				runat="server" CssClass="formfield" Width="208px" BackColor="#E0E0E0" Enabled="False"></asp:textbox>
			<FMControls:FMLabel id="Label14" AssociatedControlID="LongDatePatternDropDownList" style="Z-INDEX: 117; LEFT: 328px; POSITION: absolute; TOP: 272px" runat="server"
				CssClass="formfieldtitle">Long Date Format:</FMControls:FMLabel>
			<asp:dropdownlist id="LongDatePatternDropDownList" style="Z-INDEX: 119; LEFT: 440px; POSITION: absolute; TOP: 272px"
				tabIndex="11" runat="server" CssClass="formfield" AutoPostBack="True" Width="208px" onselectedindexchanged="LongDatePatternDropDownListSelectedIndexChanged"></asp:dropdownlist>
			<FMControls:FMLabel id="Label15" AssociatedControlID="SampleLongDateTextBox" style="Z-INDEX: 126; LEFT: 328px; POSITION: absolute; TOP: 296px" runat="server"
				CssClass="formfieldtitle">Sample Format:</FMControls:FMLabel>
			<asp:textbox id="SampleLongDateTextBox" style="Z-INDEX: 101; LEFT: 440px; POSITION: absolute; TOP: 296px"
				runat="server" CssClass="formfield" Width="208px" BackColor="#E0E0E0" Enabled="False"></asp:textbox>
			<FMControls:FMLabel id="Label16" AssociatedControlID="FourDigitCalendarEndYearTextBox" runat="server" style="Z-INDEX: 126; LEFT: 328px; POSITION: absolute; TOP: 336px"
				CssClass="formfieldtitle">Four Digit End Year:</FMControls:FMLabel>
			<asp:TextBox id="FourDigitCalendarEndYearTextBox" runat="server" style="Z-INDEX: 101; LEFT: 440px; POSITION: absolute; TOP: 336px"
				CssClass="formfield" Width="208px" AutoPostBack="True" BackColor="White" tabIndex="12" MaxLength="4" ontextchanged="FourDigitCalendarEndYearTextBoxTextChanged"></asp:TextBox>
			<script>
                var DecimalSymbolDropDownList = document.getElementById("DecimalSymbolDropDownList");
                if (DecimalSymbolDropDownList != null
                    && !DecimalSymbolDropDownList.disabled)
                    DecimalSymbolDropDownList.focus();
            </script>
		</div>
</form>
	</body>
</HTML>
