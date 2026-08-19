<%@ Register TagPrefix="fmcontrols" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="TFMSImportForm.aspx.cs" AutoEventWireup="false" Inherits="ADFWebApp.TFMSImportForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>TFMSImportForm</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="../FuelsManager.css" rel="stylesheet" />
	</head>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" enctype="multipart/form-data" runat="server">
			<asp:image id="FadeImage" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
			<FMCONTROLS:FMLABEL id="ResultsLabel" style="Z-INDEX: 106; LEFT: 16px; POSITION: absolute; TOP: 160px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="80px">Import results</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="FindFileLabel" style="Z-INDEX: 105; LEFT: 16px; POSITION: absolute; TOP: 96px"
				runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="112px">Enter or select file</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMBUTTON id="ImportButton" style="Z-INDEX: 103; LEFT: 496px; POSITION: absolute; TOP: 96px"
				runat="server" CssClass="formfieldtitle" Width="72px" Text="Import"></FMCONTROLS:FMBUTTON>
			<FMCONTROLS:FMLABEL id="TitleLabel" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				BackColor="Transparent" CssClass="headline" Width="456px">Fuel Transactions</FMCONTROLS:FMLABEL>
         <asp:FileUpload  runat="server" id="FileUpload1" style="Z-INDEX: 104; LEFT: 136px; WIDTH: 344px; POSITION: absolute; TOP: 96px; HEIGHT: 22px"
				 size="38" name="FileUpload1" />
			<asp:textbox id="ResultsTextBox" style="Z-INDEX: 107; LEFT: 16px; POSITION: absolute; TOP: 184px"
				runat="server" CssClass="formfield" Width="552px" Height="240px" TextMode="MultiLine" ReadOnly="True"></asp:textbox>
         <asp:hyperlink id="ExcelTemplateHyperLink" style="Z-INDEX: 108; LEFT: 16px; POSITION: absolute; TOP: 56px"
				runat="server" CssClass="formfieldtitle" 
            NavigateUrl="DirectFuelPurchase_template_v1.0.xls">Download Excel Template - </asp:hyperlink>
            <p>
               &nbsp;
            </p>
        </form>
	</body>
</html>