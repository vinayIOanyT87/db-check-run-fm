<%@ Register TagPrefix="fmcontrols" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="RomanJetFileForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FinanceWebApp.RomanJetFileForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">	   
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form2" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<asp:image id="Image1" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: -16px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image><FMCONTROLS:FMLABEL id="FMLABEL1" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				BackColor="Transparent" CssClass="headline" Width="200px">ROMAN Export</FMCONTROLS:FMLABEL>
			<TABLE id="Table1" style="Z-INDEX: 103; LEFT: 24px; WIDTH: 624px; POSITION: absolute; TOP: 48px; HEIGHT: 138px"
				cellSpacing="3" cellPadding="3" width="624" border="0">
				<TR align="right">
					<td style="WIDTH: 148px; TEXT-ALIGN: right" nowrap="nowrap">
                        <FMCONTROLS:FMLABEL id="PostingPeriodLabel0" runat="server" 
                            BackColor="Transparent" CssClass="formfieldtitle"
							tabIndex="3">Posting Period</FMCONTROLS:FMLABEL><FMCONTROLS:FMLABEL id="PostingDateMandatoryLabel" runat="server" BackColor="Transparent" Width="8px"
							Height="8px" ForeColor="Crimson">*</FMCONTROLS:FMLABEL></td>
					<td align="left"><asp:dropdownlist id="PostingPeriodDropdown" runat="server" CssClass="formfield" AutoPostBack="True"
							tabIndex="4"></asp:dropdownlist></td>
					<td nowrap="nowrap"><FMCONTROLS:FMLABEL id="YearLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							tabIndex="3">Year</FMCONTROLS:FMLABEL>
                        <FMCONTROLS:FMLABEL id="YearMandatoryLabel" runat="server" 
                            BackColor="Transparent" Width="8px"
							Height="8px" ForeColor="Crimson">*</FMCONTROLS:FMLABEL>
                        <asp:textbox id="PostingYearTextBox" tabIndex="14" runat="server" CssClass="formfield"
							Width="40px" MaxLength="4"></asp:textbox></td>
					<td></td>
					<td nowrap="nowrap" align="right"><FMCONTROLS:FMLABEL id="FMLABEL3" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							tabIndex="7">Group ID</FMCONTROLS:FMLABEL><FMCONTROLS:FMLABEL id="GroupIDMandatoryLabel" runat="server" BackColor="Transparent" Width="8px" Height="8px"
							ForeColor="Crimson">*</FMCONTROLS:FMLABEL></td>
					<td><asp:dropdownlist id="GroupIDDropdown" runat="server" CssClass="formfield" 
                            Width="210px" AutoPostBack="True"
							tabIndex="8"></asp:dropdownlist></td>
				</TR>
				<TR>
					<td style="WIDTH: 148px" nowrap="nowrap" align="right"><FMCONTROLS:FMLABEL id="TransactionTypeLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							tabIndex="5">Transaction Type</FMCONTROLS:FMLABEL></td>
					<td style="WIDTH: 204px" colspan="2"><asp:dropdownlist id="TransactionTypeDropdown" runat="server" CssClass="formfield" Width="210px" AutoPostBack="True"
							tabIndex="6"></asp:dropdownlist></td>
					<td></td>
					<td nowrap="nowrap" align="right"><FMCONTROLS:FMLABEL id="DocumentCompanyLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							tabIndex="11">Document Company</FMCONTROLS:FMLABEL></td>
					<td><asp:dropdownlist id="DocumentCompanyDropdown" runat="server" CssClass="formfield" Width="210px" AutoPostBack="True"
							tabIndex="12"></asp:dropdownlist></td>
				</TR>
				<TR>
					<td style="WIDTH: 148px" nowrap="nowrap" align="right"><FMCONTROLS:FMLABEL id="JournalTypeLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							tabIndex="9">Journal Type</FMCONTROLS:FMLABEL></td>
					<td style="WIDTH: 204px" colspan="2"><asp:dropdownlist id="JournalTypeDropdown" runat="server" CssClass="formfield" Width="210px" AutoPostBack="True"
							tabIndex="10"></asp:dropdownlist></td>
					<td></td>
					<td nowrap="nowrap" align="right"><FMCONTROLS:FMLABEL id="JobReferenceLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							tabIndex="15">Jet Reference</FMCONTROLS:FMLABEL></td>
					<td><asp:textbox id="JetReferenceTextBox" tabIndex="16" runat="server" CssClass="formfield" Width="210px"></asp:textbox></td>
				</TR>
				<TR>
					<td style="WIDTH: 148px" nowrap="nowrap" align="right"><FMCONTROLS:FMLABEL id="JournalDescriptionLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							tabIndex="13">Journal Description</FMCONTROLS:FMLABEL></td>
					<td style="WIDTH: 204px" colspan="2"><asp:textbox id="JournalDescriptionTextBox" tabIndex="14" runat="server" CssClass="formfield"
							Width="210px"></asp:textbox></td>
					<td></td>
					<td nowrap="nowrap" align="right">&nbsp;</td>
					<td><FMCONTROLS:FMLABEL id="JetReferenceRangeLabel" runat="server" BackColor="Transparent" CssClass="formfield"
							Width="202px" ForeColor="#C00000"></FMCONTROLS:FMLABEL></td>
				</TR>
				</TABLE>
			<FMCONTROLS:FMBUTTON id="ExportButton" style="Z-INDEX: 104; LEFT: 65px; POSITION: absolute; TOP: 215px"
				runat="server" Text="Export" CssClass="formfieldtitle" Width="72px"></FMCONTROLS:FMBUTTON></div>
</form>
	</body>
</HTML>
