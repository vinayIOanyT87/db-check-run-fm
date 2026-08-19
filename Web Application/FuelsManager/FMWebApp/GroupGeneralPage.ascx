<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="GroupGeneralPage.ascx.cs" Inherits="FuelsManager.FMWebApp.GroupGeneralPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
		
		<FMControls:FMLabel AssociatedControlID="Name" id="Label1" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 16px" CssClass="formfieldtitle"
			runat="server">Name:</FMControls:FMLabel>
		<asp:label id="GroupNameRequiredLabel" style="Z-INDEX: 101; LEFT: 128px; POSITION: absolute; TOP: 16px"
			runat="server" BackColor="Transparent" Width="8px" Height="8px" ForeColor="Crimson">*</asp:label>
		<FMControls:FMTextBox id="Name" style="Z-INDEX: 101; LEFT: 144px; POSITION: absolute; TOP: 16px" CssClass="formfield" aria-required="true"
			runat="server" Width="164px" MaxLength="30" tabIndex="1"></FMControls:FMTextBox>
		<FMControls:FMLabel id="Label2" AssociatedControlID="Description" style="Z-INDEX: 101; LEFT: 328px; POSITION: absolute; TOP: 16px" CssClass="formfieldtitle"
			runat="server">Description:</FMControls:FMLabel>
		<FMControls:FMTextBox id="Description" TextMode="MultiLine" style="Z-INDEX: 101; LEFT: 408px; POSITION: absolute; TOP: 16px; resize: none;"
			CssClass="formfield" runat="server" Rows="2" Columns="45" MaxLength="80" tabIndex="2"></FMControls:FMTextBox>
        
      <FMControls:FMCheckBox ID="SessionTimeoutEnabled" runat="server" AutoPostBack="True" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 50px" 
			CssClass="formfieldtitle" TabIndex="101" Text="Session Timeout:" />

		<FMControls:FMLabel id="FMLabel1" style="Z-INDEX: 102; LEFT: 328px; POSITION: absolute; TOP: 52px" CssClass="formfieldtitle" 
			runat="server">Min(s)</FMControls:FMLabel>



		<FMControls:FMTextBox id="SessionTimeout" style="Z-INDEX: 101; LEFT: 144px; POSITION: absolute; TOP: 50px" CssClass="formfield" aria-required="true"
			runat="server" Width="164px" MaxLength="30" tabIndex="1"></FMControls:FMTextBox>


		<FMControls:FMLabel AssociatedControlID="Name" id="AdUserGroupLabel" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 85px" CssClass="formfieldtitle"
			runat="server">Active Directory User Group:</FMControls:FMLabel>
        <asp:DropDownList ID="AdGrpDropdownList" TabIndex="12" runat="server" style="left: 184px; top: 85px; position: absolute"
                    Width="129px" CssClass="formfield"
                    MaxLength="6" AutoPostBack="false">
        </asp:DropDownList>
		<FMControls:FMLabel id="Label3" AssociatedControlID="AssignedUsersListBox" style="Z-INDEX: 112; LEFT: 8px; POSITION: absolute; TOP: 135px" runat="server"
			CssClass="formfieldtitle">Assigned Users:</FMControls:FMLabel>
		<FMControls:FMLabel id="Label5" AssociatedControlID="UnassignedUsersListBox" style="Z-INDEX: 114; LEFT: 272px; POSITION: absolute; TOP: 135px" runat="server"
			CssClass="formfieldtitle" Width="120px">Unassigned Users:</FMControls:FMLabel>


		<asp:listbox id="AssignedUsersListBox" style="Z-INDEX: 109; LEFT: 8px; POSITION: absolute; TOP: 150px"
			runat="server" CssClass="formfield" Height="160px" Width="208px" SelectionMode="Multiple"
			tabIndex="3"></asp:listbox>
		<asp:listbox id="UnassignedUsersListBox" style="Z-INDEX: 111; LEFT: 272px; POSITION: absolute; TOP: 150px"
			runat="server" CssClass="formfield" Height="160px" Width="208px" SelectionMode="Multiple"
			tabIndex="6"></asp:listbox>

				<asp:button id="AssignUsersButton" style="Z-INDEX: 117; LEFT: 232px; POSITION: absolute; TOP: 186px"
			runat="server" CssClass="formfieldtitle" Text="<<" tabIndex="4"></asp:button>
		<asp:button id="UnassignUsersButton" style="Z-INDEX: 120; LEFT: 232px; POSITION: absolute; TOP: 225px"
			runat="server" CssClass="formfieldtitle" Text=">>" tabIndex="5"></asp:button>





	</body>
	<script>
		$(document).ready(function () {
			$('#<%= Description.ClientID %>').val($.trim($('#<%= Description.ClientID %>').val()))
		});
    </script>
</HTML>
