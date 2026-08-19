<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="EntityExportForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMEntityImportWebApp.EntityExportForm" %>
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
	    <style type="text/css"> 
            .style3
            {
                height: 38px;
                width: 165px;
            }
            .style4
            {
                width: 165px;
            }
            .style5
            {
                width: 123px;
            }
            .style6
            {
                height: 38px;
                width: 142px;
            }
            .style7
            {
                width: 142px;
            }
            .includeStrapTables
            {
				padding-left: 10px !important;
            }
        </style>
		<script>
            function EnableButtons() {
                $("input").removeAttr("disabled");
            }

		</script>
	</HEAD>
    <script>
        function updatePointTemplateCBRestriction() {
			if (document.getElementById('ExportPointsCB').checked) {
				document.getElementById('ExportPointTemplatesCB').checked = false;
				document.getElementById('ExportPointTagsCB').checked = false;
				document.getElementById('IncludeStrapTablesCB').disabled = false;
			}
			else {
                document.getElementById('IncludeStrapTablesCB').checked = false;
                document.getElementById('IncludeStrapTablesCB').disabled = true;
			}
        }

        function updatePointCBRestriction() {        
            if (document.getElementById('ExportPointTemplatesCB').checked) {
				document.getElementById('ExportPointsCB').checked = false;
				document.getElementById('IncludeStrapTablesCB').checked = false;
				document.getElementById('IncludeStrapTablesCB').disabled = true;
                document.getElementById('ExportPointTagsCB').checked = false;
            }
		}

        function updatePointTagsCBRestriction() {
            if (document.getElementById('ExportPointTagsCB').checked) {
                document.getElementById('ExportPointsCB').checked = false;
				document.getElementById('IncludeStrapTablesCB').checked = false;
				document.getElementById('IncludeStrapTablesCB').disabled = true;
				document.getElementById('ExportPointTemplatesCB').checked = false;
            }
        }
    </script>
	<body MS_POSITIONING="GridLayout">
		<form id="EntityExportForm" method="post" encType="multipart/form-data" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<asp:image id="FadeImage" 
                style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image><FMCONTROLS:FMLABEL id="EntityExportTitleLabel" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" BackColor="Transparent" Width="272px" CssClass="headline">Entity Export</FMCONTROLS:FMLABEL>
			<asp:panel id="EntitiesPanel" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 88px"
				runat="server" Width="734px" Height="125px" BorderColor="LightSteelBlue" BorderStyle="Solid"
				BorderWidth="1px"></asp:panel>
			<asp:label id="EntitiesPanelLabel" style="Z-INDEX: 103; LEFT: 320px; POSITION: absolute; TOP: 90px"
				runat="server" CssClass="formfieldtitle">Entities to Export</asp:label>
			<TABLE id="MainTable" style="Z-INDEX: 103; LEFT: 8px; WIDTH: 737px; POSITION: absolute; TOP: 48px; HEIGHT: 74px"
				cellSpacing="1" cellPadding="1" width="737" border="0" role="presentation" aria-label="layout">
				<TR>
					<TD class="style6"><FMCONTROLS:FMLABEL id="ExportTypeLabel" AssociatedControlID="ExportTypeDropdown" runat="server" BackColor="Transparent" Width="80px" CssClass="formfieldtitle">Export Type</FMCONTROLS:FMLABEL></TD>
					<TD class="style3"><FMCONTROLS:FMDROPDOWNLIST id="ExportTypeDropdown" tabIndex="14" runat="server" Width="192px" CssClass="formfield"
							MaxLength="6" AutoPostBack="True"></FMCONTROLS:FMDROPDOWNLIST></TD>
					<TD class="style5"></TD>
					<TD>&nbsp;</TD>
				</TR>
				<TR>
					<TD class="style5"><br>
						<FMCONTROLS:FMCHECKBOX id="ExportEquipmentCB" tabIndex="15" runat="server" 
                            Width="138px" CssClass="formfieldtitle"
							Text="Equipment"></FMCONTROLS:FMCHECKBOX></TD>
					<TD class="style5"><br>
						<FMCONTROLS:FMCHECKBOX id="ExportFuelCardCB" tabIndex="15" 
                            runat="server" Width="184px" CssClass="formfieldtitle"
							Text="Fuel Cards"></FMCONTROLS:FMCHECKBOX></TD>
					<TD class="style5"><br>
					    <FMCONTROLS:FMCHECKBOX id="ExportProductsCB" tabIndex="15" runat="server" Width="184px" CssClass="formfieldtitle"
							Text="Products"></FMCONTROLS:FMCHECKBOX></TD>
					<TD class="style5"><br>
					<FMCONTROLS:FMCHECKBOX id="ExportEquipmentTypesCB" tabIndex="15" 
                            runat="server" Width="225px" CssClass="formfieldtitle"
							Text="Equipment Types"></FMCONTROLS:FMCHECKBOX></TD>
				</TR>
				<TR>
					<TD class="style5"><FMCONTROLS:FMCHECKBOX id="ExportCompaniesCB" tabIndex="15" 
                            runat="server" Width="184px" CssClass="formfieldtitle"
							Text="Companies"></FMCONTROLS:FMCHECKBOX></TD>
					<TD class="style5">
						<FMCONTROLS:FMCHECKBOX id="ExportPersonnelCB" tabIndex="15" runat="server" Width="184px" CssClass="formfieldtitle"
							Text="Personnel"></FMCONTROLS:FMCHECKBOX></TD>
					<TD class="style5">
						<FMCONTROLS:FMCHECKBOX id="ExportStandingOffersCB" tabIndex="15" runat="server" Width="184px" CssClass="formfieldtitle"
							Text="Price List"></FMCONTROLS:FMCHECKBOX></TD>
					<TD class="style5">
						<FMCONTROLS:FMCHECKBOX id="ExportIATACodesCB" tabIndex="15" runat="server" Width="184px" CssClass="formfieldtitle"
							Text="Delivery Locations"></FMCONTROLS:FMCHECKBOX></TD>
				</TR>
				<tr>
					<TD class="style5"><FMCONTROLS:FMCHECKBOX id="ExportAssignmentsCB" tabIndex="15" 
                            runat="server" Width="184px" CssClass="formfieldtitle"
							Text="Assignments"></FMCONTROLS:FMCHECKBOX>
					</TD>
					<td class="style5">
						<FMCONTROLS:FMCHECKBOX id="ExportPointsCB" onclick="updatePointTemplateCBRestriction();" tabIndex="15" runat="server" Width="184px" CssClass="formfieldtitle" Text="Points"></FMCONTROLS:FMCHECKBOX>
					</td>
					<td>	<FMCONTROLS:FMCHECKBOX id="ExportPointTemplatesCB" onclick="updatePointCBRestriction();" tabIndex="15" runat="server" Width="184px" CssClass="formfieldtitle" Text="Point Templates"></FMCONTROLS:FMCHECKBOX></td>
					<td>	<FMCONTROLS:FMCHECKBOX id="ExportPointCategoriesCB" tabIndex="15" runat="server" Width="184px" CssClass="formfieldtitle" Text="Point Categories"></FMCONTROLS:FMCHECKBOX></td>
				</tr>
				<tr>
					<TD class="style5">						
						<FMCONTROLS:FMCHECKBOX id="ExportPointTypesCB" tabIndex="16" runat="server" Width="184px" CssClass="formfieldtitle" Text="Point Types"></FMCONTROLS:FMCHECKBOX>
					</TD>
					<TD class="includeStrapTables">						
						<FMCONTROLS:FMCHECKBOX id="IncludeStrapTablesCB" tabIndex="17" runat="server" Width="184px" CssClass="formfieldtitle" Text="Include Strap Tables"></FMCONTROLS:FMCHECKBOX>
					</TD>
					<td class="style5">
						<FMCONTROLS:FMCHECKBOX id="ExportPointTagsCB" onclick="updatePointTagsCBRestriction();" tabIndex="18" 
                            runat="server" Width="184px" CssClass="formfieldtitle"
							Text="Point Tags (Modify Tags Only)"></FMCONTROLS:FMCHECKBOX>
					</td>
					<td>
				</tr>
				<tr><td>&nbsp;</td><td></td><td></td><td></td></tr>
				<TR>
					<TD align="left" class="style7">
						<FMCONTROLS:FMBUTTON id="ClearChkBtn" tabIndex="1" runat="server" CssClass="formfieldtitle" Text="Clear All"
							width="70px"></FMCONTROLS:FMBUTTON></TD>
					<td align="left" class="style7">
						<FMCONTROLS:FMBUTTON id="ExportBtn" tabIndex="2" runat="server" CssClass="formfieldtitle" Text="Export" OnClientClick="CheckDownloadComplete(EnableButtons);"
							width="70px"></FMCONTROLS:FMBUTTON></td>
					<TD class="style5">&nbsp;</TD>
					<TD>&nbsp;</TD>
				</TR>
				<tr><td>&nbsp;</td><td></td><td></td><td></td></tr>
			</TABLE>
			<TABLE id="Table1" style="Z-INDEX: 105; LEFT: 8px; WIDTH: 738px; POSITION: absolute; TOP: 262px; HEIGHT: 296px"
				cellSpacing="1" cellPadding="1" width="738" border="0" aria-label="results" >
				<TR>
					<TD><FMCONTROLS:FMLABEL id="ResultsLabel" runat="server" BackColor="Transparent" Width="80px" CssClass="formfieldtitle"
							Visible="False" AssociatedControlID="ResultsTB">Results</FMCONTROLS:FMLABEL></TD>
				</TR>
				<TR>
					<TD><asp:textbox id="ResultsTB" tabIndex="2" runat="server" Width="728px" CssClass="formfield" Visible="False"
							Height="240px" TextMode="MultiLine" ReadOnly="True"></asp:textbox></TD>
				</TR>
			</TABLE>
		</div>
</form>
	</body>
</HTML>
