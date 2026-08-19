<%@ Page language="c#" Codebehind="EntityImportForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMEntityImportWebApp.EntityImportForm" %>
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
	    <style type="text/css">
            .style2
            {
                width: 362px;
            }
            .style3
            {
                width: 170px;
            }
            .style4
            {
                width: 142px;
            }
            .includeStrapTables
            {
				padding-left: 10px !important;
            }
            .formfield
            {
                margin-right: 0px;
            }
            .formfield
            {
                margin-right: 0px;
            }
        </style>
	</HEAD>
	    <script>
        function updatePointTemplateCBRestriction() {
            if (document.getElementById('ImportPointsCB').checked) {
                document.getElementById('ImportPointTagsCB').checked = false;
                document.getElementById('IncludeStrapTablesCB').disabled = false;
            }
            else {
                document.getElementById('IncludeStrapTablesCB').checked = false;
                document.getElementById('IncludeStrapTablesCB').disabled = true;
            }
        }

        function updatePointCBRestriction() {        
            if (document.getElementById('ImportPointTemplatesCB').checked) {
                //document.getElementById('IncludeStrapTablesCB').checked = false;
                //document.getElementById('IncludeStrapTablesCB').disabled = true;
                document.getElementById('ImportPointTagsCB').checked = false;
            }
		}

        function updatePointTagsCBRestriction() {
            if (document.getElementById('ImportPointTagsCB').checked) {
                document.getElementById('ImportPointsCB').checked = false;
                document.getElementById('IncludeStrapTablesCB').checked = false;
                document.getElementById('IncludeStrapTablesCB').disabled = true;
                document.getElementById('ImportPointTemplatesCB').checked = false;
            }
        }
        </script>
	<body MS_POSITIONING="GridLayout">
		<form id="EntityImportForm" method="post" encType="multipart/form-data" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<asp:image id="FadeImage" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image><FMCONTROLS:FMLABEL id="EntityImportTitleLabel" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" BackColor="Transparent" Width="272px" CssClass="headline">Entity Import</FMCONTROLS:FMLABEL>
			<asp:panel id="EntitiesPanel" style="Z-INDEX: 103; LEFT: 1px; POSITION: absolute; TOP: 94px; width: 754px;"
				runat="server" Height="125px" BorderColor="LightSteelBlue" BorderStyle="Solid"
				BorderWidth="1px"></asp:panel>
			<asp:label id="EntitiesPanelLabel" style="Z-INDEX: 103; LEFT: 320px; POSITION: absolute; TOP: 96px"
				runat="server" CssClass="formfieldtitle">Entities to Import</asp:label>
			<TABLE id="MainTable" style="Z-INDEX: 103; LEFT: 8px; WIDTH: 737px; POSITION: absolute; TOP: 48px; HEIGHT: 74px"
				cellSpacing="1" cellPadding="1" width="737" border="0" role="presentation" aria-label="layout">
				<TR>
					<TD style="WIDTH: 168px; HEIGHT: 44px"><FMCONTROLS:FMLABEL id="ImportTypeLabel" AssociatedControlID="ImportTypeDropdown" runat="server" BackColor="Transparent" Width="80px" CssClass="formfieldtitle">Import Type</FMCONTROLS:FMLABEL></TD>
					<TD class="style2"><FMCONTROLS:FMDROPDOWNLIST id="ImportTypeDropdown" tabIndex="14" runat="server" Width="100px" CssClass="formfield"
							MaxLength="6" AutoPostBack="True"></FMCONTROLS:FMDROPDOWNLIST></TD>
					<td class="style4"></td>
					<td>&nbsp;</td>
				</TR>
				<TR>
					<TD style="WIDTH: 168px"><br>
						<FMCONTROLS:FMCHECKBOX id="ImportEquipmentCB" tabIndex="15" runat="server" Width="100px" CssClass="formfieldtitle"
							Text="Equipment"></FMCONTROLS:FMCHECKBOX></TD>
					<TD class="style2"><br>
						<FMCONTROLS:FMCHECKBOX id="ImportFuelCardCB" tabIndex="15" 
                            runat="server" Width="176px" CssClass="formfieldtitle"
							Text="Fuel Cards"></FMCONTROLS:FMCHECKBOX></TD>
					<TD class="style4"><br>
					    <FMCONTROLS:FMCHECKBOX id="ImportProductsCB" tabIndex="15" runat="server" Width="100px" CssClass="formfieldtitle"
							Text="Products"></FMCONTROLS:FMCHECKBOX></TD>
					<TD style="WIDTH: 168px"><br>
					   <FMCONTROLS:FMCHECKBOX id="ImportEquipmentTypesCB" 
                            tabIndex="15" runat="server" Width="176px" CssClass="formfieldtitle"
							Text="Equipment Types"></FMCONTROLS:FMCHECKBOX></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 168px"><FMCONTROLS:FMCHECKBOX id="ImportCompaniesCB" 
                            tabIndex="15" runat="server" Width="176px" CssClass="formfieldtitle"
							Text="Companies"></FMCONTROLS:FMCHECKBOX></TD>
					<TD class="style2">
						<FMCONTROLS:FMCHECKBOX id="ImportPersonnelCB" tabIndex="15" runat="server" 
                            Width="176px" CssClass="formfieldtitle"
							Text="Personnel"></FMCONTROLS:FMCHECKBOX></TD>
					<td class="style4">
						<FMCONTROLS:FMCHECKBOX id="ImportStandingOfferCB" tabIndex="15" runat="server" 
                            Width="168px" CssClass="formfieldtitle"
							Text="Price List"></FMCONTROLS:FMCHECKBOX></td>
					<td class="style4">
						<FMCONTROLS:FMCHECKBOX id="ImportIATACodesCB" tabIndex="15" runat="server" 
                            Width="168px" CssClass="formfieldtitle"
							Text="Delivery Locations"></FMCONTROLS:FMCHECKBOX></td>
				</TR>
				<tr>
					<TD class="style5">
						<FMCONTROLS:FMCHECKBOX id="ImportAssignmentsCB" tabIndex="15" runat="server" Width="184px" CssClass="formfieldtitle" Text="Assignments"></FMCONTROLS:FMCHECKBOX>
					</TD>
					<td>
						<FMCONTROLS:FMCHECKBOX id="ImportPointsCB" onclick="updatePointTemplateCBRestriction();" tabIndex="15" runat="server" Width="184px" CssClass="formfieldtitle" Text="Points"></FMCONTROLS:FMCHECKBOX>
					</td>
					<td>						<FMCONTROLS:FMCHECKBOX id="ImportPointTemplatesCB" onclick="updatePointCBRestriction();" tabIndex="15"  runat="server" Width="184px" CssClass="formfieldtitle" Text="Point Templates"></FMCONTROLS:FMCHECKBOX></td>
					<td><FMCONTROLS:FMCHECKBOX id="ImportPointCategoriesCB" tabIndex="15" runat="server" Width="184px" CssClass="formfieldtitle" Text="Point Categories"></FMCONTROLS:FMCHECKBOX></td></td>
				</tr>
				<tr>
					<TD class="style5">
						<FMCONTROLS:FMCHECKBOX id="ImportPointTypesCB" tabIndex="15" runat="server" Width="184px" CssClass="formfieldtitle" Text="Point Types"></FMCONTROLS:FMCHECKBOX>
					</TD>
					<TD class="includeStrapTables">						
						<FMCONTROLS:FMCHECKBOX id="IncludeStrapTablesCB" tabIndex="16" runat="server" Width="184px" CssClass="formfieldtitle" Text="Include Strap Tables"></FMCONTROLS:FMCHECKBOX>
					</TD>
					<td>
					<FMCONTROLS:FMCHECKBOX id="ImportPointTagsCB" onclick="updatePointTagsCBRestriction();" tabIndex="17" runat="server" Width="184px" CssClass="formfieldtitle" Text="Point Tags (Modify Tags Only)"></FMCONTROLS:FMCHECKBOX>
					</td>
					<td></td>
				</tr>
				<tr><td>&nbsp;</td><td></td><td></td><td></td></tr>
				<TR>
					<TD align="left">
						<FMCONTROLS:FMBUTTON id="ClearChkBtn" tabIndex="1" runat="server" CssClass="formfieldtitle" Text="Clear All"
							width="70px"></FMCONTROLS:FMBUTTON></TD>
					<td ></td>
					<td class="style4"></td>
					<td>&nbsp;</td>
				</TR>
				<TR>
					<TD class="style2" colspan="4" align="center" style="padding-top: 30px !important;">
						<FMCONTROLS:FMLABEL id="FMLABEL1" runat="server" BackColor="Transparent" ForeColor="red" CssClass="formfieldtitle">
							Entity Import is not intended to restore deleted entities. Data corruption is likely to occur for syncing sites. 
						</FMCONTROLS:FMLABEL>

					</TD>

				</TR>
				<TR>
					<TD style="WIDTH: 168px"><br>
						<FMCONTROLS:FMLABEL id="ImportFileLable" runat="server" BackColor="Transparent" Width="80px" CssClass="formfieldtitle">Import File</FMCONTROLS:FMLABEL></TD>
					<TD class="style2" colspan="2"><br>
						<INPUT class="formfieldtitle" id="File1" alt="Select file import" style="WIDTH: 360px; HEIGHT: 22px" type="file" accept=".xml"	size="51" name="file">
					</TD>
					<td align="center" class="style4"><br>
						<FMCONTROLS:FMBUTTON id="ImportBtn"  OnClientClick="try{ResultsTB.value='';}catch(err){;} document.all['progress'].style.visibility='visible'; document.all['progressImg'].src = '../FMWebApp/images/progress-bar-clipart5.gif'" tabIndex="1" runat="server" CssClass="formfieldtitle" Text="Import"
							width="70px" ></FMCONTROLS:FMBUTTON></td>
					<td align="center">&nbsp;</td>
				</TR>
			</TABLE>
			<TABLE id="Table1" style="Z-INDEX: 105; LEFT: 8px; WIDTH: 738px; POSITION: absolute; TOP: 362px; HEIGHT: 272px"
				cellSpacing="1" cellPadding="1" width="738" border="0" aria-label="results">
				<TR>
					<TD><FMCONTROLS:FMLABEL id="ResultsLabel" runat="server" BackColor="Transparent" Width="80px" CssClass="formfieldtitle" AssociatedControlID="ResultsTB">Results</FMCONTROLS:FMLABEL></TD>
				</TR>
				<TR>
					<TD><asp:textbox id="ResultsTB" tabIndex="2" runat="server" Width="728px" CssClass="formfield" 
							Height="240px" TextMode="MultiLine" ReadOnly="True"></asp:textbox></TD>
				</TR>
			</TABLE>
		</div>
</form>

		<span id="progress" 
         style="Z-INDEX: 105; LEFT: 200px; WIDTH: 449px; POSITION: absolute; TOP: 315px; HEIGHT: 44px; visibility:hidden; ">
         Import in progress...<BR><img id="progressImg" alt="Importing" src="" /></span>
	</body>
</HTML>
