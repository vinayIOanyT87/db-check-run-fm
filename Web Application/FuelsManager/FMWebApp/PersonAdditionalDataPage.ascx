<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="PersonAdditonalDataPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.PersonAdditionalDataPage"%>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<SCRIPT>
	    function PersonSelect(role, personTextBoxId)
	    {
	        var personTextBox = document.getElementById(personTextBoxId);

			showModalDialogFrame({
			    url: "../FMWebApp/PersonSelectForm.aspx?Role=" + role + "&Null=true&Unassigned=true&ExcludeGuid=<%= ExcludeGuid %>",
			    width: 855,
			    height: 560,
			    title: "Person Select",
			    onClose: function ()
			    {
			        if (this.returnValue != null)
			        {
			            var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
			            var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

			            personTextBox.value = asciiValue1;
			            personTextBox.title = asciiValue2;
			        }
			    }
			});
	    }

	    function UserSelect(userTextBoxId)
	    {
	        var userTextBox = document.getElementById(userTextBoxId);
            showModalDialogFrame({
			    url: "../FMWebApp/UserSelectForm.aspx",
			    width: 855,
			    height: 560,
			    title: "User Select",
			    onClose: function ()
			    {
			        if (this.returnValue != null)
			        {
			            var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
			            var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

			            userTextBox.value = asciiValue1;
			            userTextBox.title = asciiValue2;
			        }
			    }
		    });
        }
   </SCRIPT>	
	<body>
		<FMCONTROLS:FMLABEL id="Fmlabel1" 
         style="Z-INDEX: 104; LEFT: 6px; POSITION: absolute; TOP: 20px; width: 82px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Supervisor ID:</FMCONTROLS:FMLABEL>
	   <FMControls:FMPersonTextBox Role="1" id="SupervisorIDTextBox" style="Z-INDEX: 105; LEFT: 133px; POSITION: absolute; TOP: 15px"
		   runat="server" CssClass="formfield" Width="150px" tabIndex="1">
		</FMControls:FMPersonTextBox>      
	   <FMControls:FMCheckBox ID="ResponsibleOfficerCheckBox" CssClass="formfieldtitle" 
         style="Z-INDEX: 104; LEFT: 323px; POSITION: absolute; TOP: 17px" 
         runat="server"  Text="Responsible Officer" TabIndex="2"/>
		<FMCONTROLS:FMLABEL id="Label26" 
         style="Z-INDEX: 150; LEFT: 6px; POSITION: absolute; TOP: 52px; width: 115px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Date of Supervision:</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMDate id="DateOfSupervisionTextbox" style="Z-INDEX: 151; LEFT: 133px; POSITION: absolute; TOP: 50px; right: 841px;"
			tabIndex="3" runat="server" CssClass="formfield" Width="138px" MaxLength="10" 
         Enabled="True"></FMCONTROLS:FMDate>
		<FMCONTROLS:FMLABEL id="Label9" AssociatedControlID="DepartmentTextbox"
         style="Z-INDEX: 116; LEFT: 326px; POSITION: absolute; TOP: 53px; height: 5px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle" Width="70px">Department:</FMCONTROLS:FMLABEL>
		<asp:textbox id="DepartmentTextbox" style="Z-INDEX: 117; LEFT: 412px; POSITION: absolute; TOP: 50px; left: 450px;"
			tabIndex="4" runat="server" CssClass="formfield" Width="80px" MaxLength="20"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label19" 
         style="Z-INDEX: 133; LEFT: 6px; POSITION: absolute; TOP: 83px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle" Width="112px" Height="24px">Date Assigned:</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMDate id="DateAssignedTextbox" style="Z-INDEX: 134; LEFT: 133px; POSITION: absolute; TOP: 82px; width: 138px;"
			tabIndex="5" runat="server" CssClass="formfield" MaxLength="10" Enabled="True"
			ReadOnly="True"></FMCONTROLS:FMDate>
        <FMCONTROLS:FMLABEL id="Label28"   
         style="Z-INDEX: 104; LEFT: 326px; POSITION: absolute; TOP: 83px; width: 68px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">ADC User Login:</FMCONTROLS:FMLABEL>
        <FMControls:FMUserTextBox Role="1" id="ADCUserLoginTextBox" style="Z-INDEX: 105; LEFT: 450px; POSITION: absolute; TOP: 80px;"
		   runat="server" CssClass="formfield" Width="150px" tabIndex="1">
		</FMControls:FMUserTextBox>      
		<FMCONTROLS:FMLABEL id="Fmlabel2" AssociatedControlID="LaborRate1Textbox"
         style="Z-INDEX: 104; LEFT: 7px; POSITION: absolute; TOP: 126px; width: 84px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Labor Rate 1:</FMCONTROLS:FMLABEL>
		<asp:textbox id="LaborRate1Textbox" style="Z-INDEX: 105; LEFT: 133px; POSITION: absolute; TOP: 126px; left: 133px;"
			tabIndex="6" runat="server" CssClass="formfield" Width="80px" MaxLength="10"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label10" AssociatedControlID="LaborRate2Textbox"
         style="Z-INDEX: 119; LEFT: 326px; POSITION: absolute; TOP: 126px; bottom: 317px; width: 76px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Labor Rate 2:</FMCONTROLS:FMLABEL>
		<asp:textbox id="LaborRate2Textbox" style="Z-INDEX: 120; LEFT: 410px; POSITION: absolute; TOP: 126px; left: 450px;"
			tabIndex="7" runat="server" CssClass="formfield" Width="80px" MaxLength="10"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label11" AssociatedControlID="LaborRate3Textbox"
         style="Z-INDEX: 121; LEFT: 6px; POSITION: absolute; TOP: 156px; width: 110px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Labor Rate 3:</FMCONTROLS:FMLABEL>
		<asp:textbox id="LaborRate3Textbox" style="Z-INDEX: 122; LEFT: 133px; POSITION: absolute; TOP: 155px; left: 133px;"
			tabIndex="8" runat="server" CssClass="formfield" Width="80px" MaxLength="10"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label14" AssociatedControlID="LaborRate4Textbox"
         style="Z-INDEX: 123; LEFT: 326px; POSITION: absolute; TOP: 156px; height: 15px; width: 82px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Labor Rate 4:</FMCONTROLS:FMLABEL>
		<asp:textbox id="LaborRate4Textbox" style="Z-INDEX: 124; LEFT: 410px; POSITION: absolute; TOP: 155px; left: 450px;"
			tabIndex="9" runat="server" CssClass="formfield" Width="80px" MaxLength="10"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label27"   
         style="Z-INDEX: 104; LEFT: 6px; POSITION: absolute; TOP: 203px; width: 68px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Shift:</FMCONTROLS:FMLABEL>
      <FMControls:FMRadioButtonList ID="ShiftRadioButtonList" runat="server" style="Z-INDEX: 106; LEFT: 42px; POSITION: absolute; TOP: 199px"
			tabIndex="10" CssClass="formfield" Width="160px" 
         RepeatDirection="Horizontal">
         <asp:ListItem Selected="True">first</asp:ListItem>
         <asp:ListItem>second</asp:ListItem>
         <asp:ListItem>third</asp:ListItem>
      </FMControls:FMRadioButtonList>
	</body>
</HTML>
