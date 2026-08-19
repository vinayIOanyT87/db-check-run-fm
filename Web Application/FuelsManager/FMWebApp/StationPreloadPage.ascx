<%@ Control Language="c#" AutoEventWireup="True" Codebehind="StationPreloadPage.ascx.cs" Inherits="FuelsManager.FMWebApp.StationPreloadPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
<FMControls:FMCheckbox id="InhibitLoadingByLoadIDCheckBox" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 56px"
	runat="server" Text="Inhibit Loading By LoadID" CssClass="formfieldtitle" tabIndex="2"></FMControls:FMCheckbox>
<FMCONTROLS:FMLABEL id="Label3" style="Z-INDEX: 109; LEFT: 0px; POSITION: absolute; TOP: 88px" runat="server"
	Width="80px" CssClass="formfieldtitle" BackColor="Transparent">Transactions:</FMCONTROLS:FMLABEL>
<FMCONTROLS:FMLABEL id="Label16" style="Z-INDEX: 109; LEFT: 0px; POSITION: absolute; TOP: 120px" runat="server"
	Width="112px" CssClass="formfieldtitle" BackColor="Transparent">Issue By Volume:</FMCONTROLS:FMLABEL>
<asp:dropdownlist id="IssueByVolumeTransactionDropDownList" style="Z-INDEX: 102; LEFT: 128px; POSITION: absolute; TOP: 120px"
	tabIndex="2" runat="server" Width="240px" CssClass="formfield"></asp:dropdownlist>
<FMCONTROLS:FMLABEL id="Fmlabel2" style="Z-INDEX: 109; LEFT: 0px; POSITION: absolute; TOP: 152px" runat="server"
	Width="112px" CssClass="formfieldtitle" BackColor="Transparent">Issue By Weight:</FMCONTROLS:FMLABEL>
<asp:dropdownlist id="IssueByWeightTransactionDropDownList" style="Z-INDEX: 102; LEFT: 128px; POSITION: absolute; TOP: 152px"
	tabIndex="3" runat="server" Width="240px" CssClass="formfield"></asp:dropdownlist>
<FMCONTROLS:FMLABEL id="PreloadLabel" style="Z-INDEX: 103; LEFT: 0px; POSITION: absolute; TOP: 24px"
	runat="server" Width="112px" CssClass="formfieldtitle" BackColor="Transparent">Preload Printer:</FMCONTROLS:FMLABEL>
<asp:dropdownlist id="PreloadPrinterDropDownlist" style="Z-INDEX: 103; LEFT: 128px; POSITION: absolute; TOP: 24px"
	runat="server" Width="240px" CssClass="formfield" tabIndex="1"></asp:dropdownlist>
<FMCONTROLS:FMLABEL id="PreloadNumberOfCopiesLabel" style="Z-INDEX: 103; LEFT: 400px; POSITION: absolute; TOP: 24px"
	runat="server" Width="112px" CssClass="formfieldtitle" BackColor="Transparent">Number of copies:</FMCONTROLS:FMLABEL>
<asp:textbox id="PreloadNumberOfCopiesTextBox" style="Z-INDEX: 103; LEFT: 512px; POSITION: absolute; TOP: 24px"
	runat="server" Width="56px" CssClass="formfield" MaxLength="2" Columns="2" tabIndex="4"></asp:textbox>
<FMCONTROLS:FMCHECKBOX id="SetPreloadToZeroCheckBox" style="Z-INDEX: 103; LEFT: 400px; POSITION: absolute; TOP: 54px"
	tabIndex="5" runat="server" CssClass="formfieldtitle" Text="Set Default Preset To Zero"></FMCONTROLS:FMCHECKBOX>
	</body>
</HTML>
