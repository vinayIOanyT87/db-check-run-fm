<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control language="c#" Codebehind="PersonGeneralPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.PersonGeneralPage"%>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
      <FMCONTROLS:FMLABEL id="Label1" AssociatedControlID="IDTextbox" 
               
         style="Z-INDEX: 102; LEFT: 1px; POSITION: absolute; TOP: 20px; width: 77px;" runat="server"
			      BackColor="Transparent" CssClass="formfieldtitle">Personnel ID:</FMCONTROLS:FMLABEL>      
		<FMCONTROLS:FMLABEL id="Label8" 
         style="Z-INDEX: 107; LEFT: 84px; POSITION: absolute; TOP: 16px; width: 4px;" 
         runat="server" Height="8px" ForeColor="Crimson">*</FMCONTROLS:FMLABEL>
	   <asp:textbox id="IDTextbox" 
         style="Z-INDEX: 103; LEFT: 95px; POSITION: absolute; TOP: 15px" tabIndex="1" aria-required="true"
			      runat="server" CssClass="formfield" Width="152px" MaxLength="50"></asp:textbox>		
		<FMCONTROLS:FMLABEL id="Label12" AssociatedControlID="FirstNameTextbox"
         style="Z-INDEX: 136; LEFT: 98px; POSITION: absolute; TOP: 40px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">First:</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMLABEL id="Label15" AssociatedControlID="MiddleNameTextbox"
         style="Z-INDEX: 138; LEFT: 167px; POSITION: absolute; TOP: 40px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Middle:</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMLABEL id="Label16" AssociatedControlID="LastNameTextbox"
         style="Z-INDEX: 140; LEFT: 217px; POSITION: absolute; TOP: 40px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Last:</FMCONTROLS:FMLABEL>
      <FMCONTROLS:FMLABEL id="Label5" 
         
         style="Z-INDEX: 106; LEFT: 1px; POSITION: absolute; TOP: 65px; height: 5px; width: 77px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Name:</FMCONTROLS:FMLABEL>		
		<FMCONTROLS:FMLABEL id="Label23" 
         
         style="Z-INDEX: 146; LEFT: 85px; POSITION: absolute; TOP: 63px; width: 4px;" 
         runat="server" Height="8px" ForeColor="Crimson">*</FMCONTROLS:FMLABEL>

		<asp:textbox id="FirstNameTextbox" style="Z-INDEX: 108; LEFT: 95px; POSITION: absolute; TOP: 60px" aria-required="true"
			tabIndex="2" runat="server" CssClass="formfield" Width="64px" MaxLength="20"></asp:textbox>
		<asp:textbox id="MiddleNameTextbox" style="Z-INDEX: 137; LEFT: 168px; POSITION: absolute; TOP: 60px; right: 987px;"
			tabIndex="3" runat="server" CssClass="formfield" Width="24px" MaxLength="20"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label24" 
         style="Z-INDEX: 147; LEFT: 204px; POSITION: absolute; TOP: 63px; width: 1px;" 
         runat="server" Height="8px" ForeColor="Crimson">*</FMCONTROLS:FMLABEL>
		<asp:textbox id="LastNameTextbox" style="Z-INDEX: 139; LEFT: 217px; POSITION: absolute; TOP: 60px" aria-required="true"
			tabIndex="4" runat="server" CssClass="formfield" Width="80px" MaxLength="30"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label27" AssociatedControlID="TitleTextbox"
         style="Z-INDEX: 121; LEFT: 317px; POSITION: absolute; TOP: 63px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle" Width="70px">Title:</FMCONTROLS:FMLABEL>
		<asp:textbox id="TitleTextbox" style="Z-INDEX: 124; LEFT: 400px; POSITION: absolute; TOP: 60px"
			tabIndex="11" runat="server" CssClass="formfield" Width="91px" MaxLength="50"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label13" AssociatedControlID="Address1Textbox"
         
         style="Z-INDEX: 109; LEFT: 1px; POSITION: absolute; TOP: 90px; width: 91px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Address:</FMCONTROLS:FMLABEL>
		<asp:textbox id="Address1Textbox" style="Z-INDEX: 110; LEFT: 95px; POSITION: absolute; TOP: 85px; right: 884px;"
			tabIndex="5" runat="server" CssClass="formfield" Width="200px" MaxLength="50"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label11" AssociatedControlID="Phone1Textbox"
         style="Z-INDEX: 121; LEFT: 318px; POSITION: absolute; TOP: 88px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle" Width="70px">Phone1:</FMCONTROLS:FMLABEL>
		<asp:textbox id="Phone1Textbox" style="Z-INDEX: 122; LEFT: 400px; POSITION: absolute; TOP: 85px"
			tabIndex="12" runat="server" CssClass="formfield" Width="91px" MaxLength="20"></asp:textbox>
		<asp:textbox id="Address2Textbox" ToolTip="Address 2 textbox" style="Z-INDEX: 111; LEFT: 95px; POSITION: absolute; TOP: 110px"
			tabIndex="6" runat="server" CssClass="formfield" Width="200px" MaxLength="50"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label14" AssociatedControlID="Phone2Textbox"
         style="Z-INDEX: 123; LEFT: 319px; POSITION: absolute; TOP: 113px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle" Width="70px">Phone2:</FMCONTROLS:FMLABEL>
		<asp:textbox id="Phone2Textbox" style="Z-INDEX: 124; LEFT: 400px; POSITION: absolute; TOP: 110px;"
			tabIndex="13" runat="server" CssClass="formfield" Width="91px" MaxLength="20"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label4" AssociatedControlID="CityTextbox"
         
         style="Z-INDEX: 112; LEFT: 1px; POSITION: absolute; TOP: 140px; width: 90px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">City:</FMCONTROLS:FMLABEL>
		<asp:textbox id="CityTextbox" style="Z-INDEX: 113; LEFT: 95px; POSITION: absolute; TOP: 135px"
			tabIndex="7" runat="server" CssClass="formfield" Width="152px" MaxLength="60"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label26" AssociatedControlID="EMailTextbox"
         style="Z-INDEX: 123; LEFT: 320px; POSITION: absolute; TOP: 141px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle" Width="70px">E-mail:</FMCONTROLS:FMLABEL>
		<asp:textbox id="EMailTextbox" style="Z-INDEX: 124; LEFT: 400px; POSITION: absolute; TOP: 135px;"
			tabIndex="14" runat="server" CssClass="formfield" Width="91px" MaxLength="50"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label7" AssociatedControlID="StateTextbox"
         
         style="Z-INDEX: 114; LEFT: 1px; POSITION: absolute; TOP: 165px; width: 90px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">State:</FMCONTROLS:FMLABEL>
		<asp:textbox id="StateTextbox" style="Z-INDEX: 115; LEFT: 95px; POSITION: absolute; TOP: 160px"
			tabIndex="8" runat="server" CssClass="formfield" Width="152px" MaxLength="20"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label9" AssociatedControlID="ZipTextbox"
         
         style="Z-INDEX: 116; LEFT: 1px; POSITION: absolute; TOP: 190px; height: 15px; width: 90px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Zip:</FMCONTROLS:FMLABEL>
		<asp:textbox id="ZipTextbox" style="Z-INDEX: 117; LEFT: 95px; POSITION: absolute; TOP: 185px"
			tabIndex="9" runat="server" CssClass="formfield" Width="80px" MaxLength="10"></asp:textbox>
		<FMCONTROLS:FMLABEL id="Label10" AssociatedControlID="CountryTextbox"
         
         style="Z-INDEX: 119; LEFT: 1px; POSITION: absolute; TOP: 215px; bottom: 29px; width: 91px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Country:</FMCONTROLS:FMLABEL>
		
		<asp:textbox id="CountryTextbox" style="Z-INDEX: 120; LEFT: 95px; POSITION: absolute; TOP: 210px"
			tabIndex="10" runat="server" CssClass="formfield" Width="138px" MaxLength="20"></asp:textbox>
			
		<FMCONTROLS:FMLABEL id="Label3" AssociatedControlID="AssignedRolesListBox"
         style="Z-INDEX: 125; LEFT: 1px; POSITION: absolute; TOP: 260px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle" Width="100px">Assigned Roles:</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMLABEL id="Label25" 
            style="Z-INDEX: 148; LEFT: 101px; POSITION: absolute; TOP: 260px; width: 14px; height: 19px;" 
            runat="server" ForeColor="Crimson">*</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMLABEL id="Label6" AssociatedControlID="UnassignedRolesListbox"
         style="Z-INDEX: 127; LEFT: 196px; POSITION: absolute; TOP: 260px" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle" Width="112px">Unassigned Roles:</FMCONTROLS:FMLABEL>
		<FMCONTROLS:FMLISTBOX id="AssignedRolesListBox" 
         style="Z-INDEX: 126; LEFT: 4px; POSITION: absolute; TOP: 280px" runat="server" aria-required="true"
         BackColor="White" CssClass="formfield" Width="138px" 
         Height="56px" SelectionMode="Multiple"></FMCONTROLS:FMLISTBOX>
		<FMCONTROLS:FMLISTBOX id="UnassignedRolesListbox" 
         style="Z-INDEX: 101; LEFT: 192px; POSITION: absolute; TOP: 280px" 
         runat="server" BackColor="White" CssClass="formfield" Width="131px" 
         Height="56px" SelectionMode="Multiple"></FMCONTROLS:FMLISTBOX>
		<FMControls:FMCheckBox id="HiddenCheckBox" style="Z-INDEX: 117; LEFT: 320px; POSITION: absolute; TOP: 160px"
			tabIndex="15" CssClass="formfieldtitle" runat="server" TextAlign="Left" Text="Hidden"></FMControls:FMCheckBox>
		<asp:button id="AssignRolesButton" style="Z-INDEX: 128; LEFT: 152px; POSITION: absolute; TOP: 280px; height:20px;"
			tabIndex="15" runat="server" CssClass="formfieldtitle" Text="<<"></asp:button>
		<asp:button id="UnassignRolesButton" style="Z-INDEX: 129; LEFT: 152px; POSITION: absolute; TOP: 311px; height:20px;"
			tabIndex="16" runat="server" CssClass="formfieldtitle" Text=">>"></asp:button>
         
	</body>
</HTML>
