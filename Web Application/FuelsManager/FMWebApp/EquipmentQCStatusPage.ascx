<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="EquipmentQCStatusPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EquipmentQcStatusPage"%>

	<SCRIPT>
		function CompanySelect(role, companyTextBoxId)
		{
		    var companyTextBox = document.getElementById(companyTextBoxId);

		    showModalDialogFrame({
		        url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&Unassigned=true",
		        width: 855,
		        height: 560,
		        onClose: function ()
		        {
		            if (this.returnValue != null)
		            {
		                var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
		                var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

		                companyTextBox.value = asciiValue1;
		                companyTextBox.title = asciiValue2;
		            }
		        }
		    });
		}
	</SCRIPT>
	
	<FMControls:FMCheckBox id="InServiceCheckBox" style="Z-INDEX: 142; LEFT: 400px; POSITION: absolute; TOP: 13px; width: 129px;"
		tabIndex="19" runat="server" CssClass="formfieldtitle" 
         Text="In Service Flag" TextAlign="Left" Enabled="False" AutoPostBack="true" />
   <FMControls:FMLabel id="Label10" AssociatedControlID="NotesTextbox" style="Z-INDEX: 121; LEFT: 0px; POSITION: absolute; TOP: 107px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">Note:</FMControls:FMLabel>	
	<FMControls:FMTextBox id="NotesTextbox" style="Z-INDEX: 122; LEFT: 150px; POSITION: absolute; TOP: 107px"
		runat="server" CssClass="formfield" Width="312px" Height="48px" 
         TextMode="MultiLine" MaxLength="1000" tabIndex="2" />
   <FMControls:FMLabel id="FMLabel1" AssociatedControlID="MaintenanceNoteTextbox" style="Z-INDEX: 121; LEFT: 0px; POSITION: absolute; TOP: 167px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">Maintenance Note:</FMControls:FMLabel>	
	<FMControls:FMTextBox id="MaintenanceNoteTextbox" style="Z-INDEX: 122; LEFT: 150px; POSITION: absolute; TOP: 167px"
		runat="server" CssClass="formfield" Width="312px" Height="48px" TextMode="MultiLine" maxLength="1000" Enabled="False" />
   <FMControls:FMLabel id="FMLabel2" AssociatedControlID="QCNoteTextbox" style="Z-INDEX: 121; LEFT: 0px; POSITION: absolute; TOP: 227px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">QC Note:</FMControls:FMLabel>	
	<FMControls:FMTextBox id="QCNoteTextbox" style="Z-INDEX: 122; LEFT: 150px; POSITION: absolute; TOP: 227px"
		runat="server" CssClass="formfield" Width="312px" Height="48px" 
         TextMode="MultiLine" maxLength="1000" tabIndex="18" Enabled="False" />
		
	<FMControls:FMButton ID="EditQCButton" runat="server" CssClass="formfieldtitle" Text="Add Maintenance Record"
	    style="z-index:122; position: absolute; left:150px; top:287px" />
	
	<FMControls:FMDropDownList ID="StatusDescriptionDropDownList" runat="server" 
	    style="Z-INDEX: 106; LEFT: 150px; POSITION: absolute; TOP: 13px; right: 806px;" Width="208px" 
	    CssClass="formfield" MaxLength="50" Enabled="False"/>
		
	<FMControls:FMLabel id="Label4" AssociatedControlID="StatusDescriptionDropDownList" style="Z-INDEX: 108; LEFT: 0px; POSITION: absolute; TOP: 15px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">Status Description:</FMControls:FMLabel>
	<FMControls:FMLabel id="Fmlabel4" style="Z-INDEX: 117; LEFT: 0px; POSITION: absolute; TOP: 77px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">QC Due Date:</FMControls:FMLabel>
	<FMControls:FMLabel id="MakeLabel" AssociatedControlID="ReturnToServiceFMDate" style="Z-INDEX: 110; LEFT: 0px; POSITION: absolute; TOP: 47px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">Return to Service Date:</FMControls:FMLabel>
	<FMControls:FMLabel id="TagAssignmentLabel" AssociatedControlID="TagAssignmentTextbox" style="Z-INDEX: 110; LEFT: 400px; POSITION: absolute; TOP: 47px" runat="server"
		CssClass="formfieldtitle" BackColor="Transparent">Tag Assignment:</FMControls:FMLabel>
	<asp:textbox id="TagAssignmentTextbox" style="Z-INDEX: 113; LEFT: 550px; POSITION: absolute; TOP: 47px"
		runat="server" MaxLength="50" Width="112px" CssClass="formfield" tabIndex="2"  
         Enabled="false" ></asp:textbox>
	
	<FMControls:FMDate ID="ReturnToServiceFMDate" runat="server" style="z-index:152; left:150px; position:absolute; top:47px"
	    cssclass="formfield" TabIndex="2" Enabled="false" Width="150px" />
	<FMControls:FMDate id="QCDueDate" style="Z-INDEX: 151; LEFT: 150px; POSITION: absolute; TOP: 77px"
		runat="server" MaxLength="50" CssClass="formfield" tabIndex="1" Width="150px" />
	