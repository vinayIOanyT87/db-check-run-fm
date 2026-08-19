<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserGeneralPage.ascx.cs" Inherits="FuelsManager.FMWebApp.UserGeneralPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
	<script type="text/javascript">
		function RemoveSpecialChar(txtVal) {
			if (txtVal.value != '' && txtVal.value.match(/^[\w ]+$/) == null) {
				txtVal.value = txtVal.value.replace(/\'/ig, '');
			}
		}
	</script>


        <FMControls:FMLabel ID="Label1" AssociatedControlID="Name" Style="z-index: 102; left: 32px; position: absolute;
		top: 40px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">ID:</FMControls:FMLabel>
	<FMControls:FMCheckBox ID="LockedOutCheckBox" Style="z-index: 128; left: 32px; position: absolute;
		top: 200px" TabIndex="5" runat="server" CssClass="formfieldtitle" Width="296px"
		Text="Locked Out"></FMControls:FMCheckBox>
	<asp:TextBox ID="EmailAddressTextBox" Style="z-index: 127; left: 240px; position: absolute;
		top: 264px" TabIndex="7" runat="server" CssClass="formfield" BackColor="White"
		Width="152px" MaxLength="50" Visible="True"></asp:TextBox>
	<FMControls:FMLabel ID="FMLabel1" AssociatedControlID="EmailAddressTextBox" Style="z-index: 126; left: 32px; position: absolute;
		top: 264px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">E-mail Address:</FMControls:FMLabel>
	<FMControls:FMCheckBox ID="ChangePasswordCheckBox" Style="z-index: 125; left: 32px;
		position: absolute; top: 164px" runat="server" CssClass="formfieldtitle" Width="296px"
		Text="Change Password at Login" TabIndex="4"></FMControls:FMCheckBox>
	<asp:TextBox ID="FullNameTextBox" Style="z-index: 123; left: 240px; position: absolute;
		top: 232px" runat="server" BackColor="White" CssClass="formfield" Width="152px"
		Visible="True" TabIndex="6" MaxLength="50"></asp:TextBox>
	<FMControls:FMLabel ID="Label11" AssociatedControlID="FullNameTextBox" Style="z-index: 122; left: 32px; position: absolute;
		top: 232px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Name:</FMControls:FMLabel>
	<asp:TextBox ID="ReenterPasswordTextBox" Style="z-index: 121; left: 240px; position: absolute;
		top: 104px" runat="server" BackColor="White" CssClass="formfield" Width="154px" aria-required="true"
		TextMode="Password" TabIndex="3" MaxLength="25" AutoCompleteType="None"></asp:TextBox>
	<asp:TextBox ID="PasswordHintTextBox" Style="z-index: 121; left: 240px; position: absolute;
		top: 136px" runat="server" BackColor="White" CssClass="formfield" Width="154px"
		TabIndex="3" MaxLength="80" AutoCompleteType="None" onkeyup="javascript:RemoveSpecialChar(this)"></asp:TextBox>
	<FMControls:FMLabel ID="Label8" Style="z-index: 119; left: 224px; position: absolute;
		top: 104px" runat="server" BackColor="Transparent" Width="8px" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>
	<FMControls:FMLabel ID="Label7" AssociatedControlID="ReenterPasswordTextBox" Style="z-index: 118; left: 32px; position: absolute;
		top: 104px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Re-enter Password:</FMControls:FMLabel>
	<FMControls:FMLabel ID="PasswordHintLabel" Style="z-index: 118; left: 32px; position: absolute;
		top: 136px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Password Hint:</FMControls:FMLabel>
	<asp:Button ID="UnassignGroupsButton" Style="z-index: 114; left: 200px; position: absolute;
		top: 436px; padding-left:1px;padding-right:1px;width:20px" runat="server" CssClass="formfieldtitle" Text=">>" TabIndex="12">
	</asp:Button>
	<FMControls:FMLabel ID="PhoneNumberLabel" Style="z-index: 126; left: 32px; position: absolute;
		top: 296px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Phone Number:</FMControls:FMLabel>
	<asp:TextBox ID="PhoneNumberTextbox" Style="z-index: 127; left: 240px; position: absolute;
		top: 296px" TabIndex="8" runat="server" CssClass="formfield" BackColor="White"
		Width="152px" MaxLength="50" Visible="True"></asp:TextBox>
	<FMControls:FMLabel ID="AccountExpirationDateLabel" Style="z-index: 126; left: 32px;
		position: absolute; top: 331px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Account Expiration Date:</FMControls:FMLabel>
	<FMControls:FMDate ID="AccountExpirationDate" ToolTip="Account Expiration Date"  TabIndex="9" runat="server" CssClass="formfield" 
		Style="z-index: 127; position: absolute; left: 240px; top: 328px; width: 152px"></FMControls:FMDate>
	<FMControls:FMLabel ID="Label4" AssociatedControlID="UnassignedGroupsListBox" Style="z-index: 112; left: 240px; position: absolute;
		top: 363px" runat="server" CssClass="formfieldtitle" Width="144px">Unassigned Groups:</FMControls:FMLabel>
	<FMControls:FMLabel ID="Label3" AssociatedControlID="AssignedGroupsListBox" Style="z-index: 111; left: 32px; position: absolute;
		top: 363px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Assigned Groups:</FMControls:FMLabel>
	<FMControls:FMLabel ID="Label2" AssociatedControlID="PasswordTextBox" Style="z-index: 104; left: 32px; position: absolute;
		top: 72px" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Password:</FMControls:FMLabel>
	<asp:TextBox ID="Name" Style="z-index: 103; left: 240px; position: absolute; top: 40px" aria-required="true"
		runat="server" BackColor="White" CssClass="formfield" Width="152px" TabIndex="1"
		MaxLength="75"></asp:TextBox>
	<asp:TextBox ID="PasswordTextBox" Style="z-index: 106; left: 240px; position: absolute;
		top: 72px" runat="server" BackColor="White" CssClass="formfield" Width="154px" aria-required="true"
		TextMode="Password" TabIndex="2" MaxLength="25" AutoCompleteType="None"></asp:TextBox>
<FMControls:FMLabel ID="PasswordPopupBubbleLabel" title=" " Style="left: 420px; top: 76px; color: #666666; font-family: Arial, Helvetica,sans-serif; font-size: 11.15px; position: absolute"
	Width="100px" CssClass="DefaultLoginPageLink" runat="server" Text="Password Policy" TabIndex="5"></FMControls:FMLabel>

	<asp:ListBox ID="AssignedGroupsListBox" Style="z-index: 109; left: 32px; position: absolute;
		top: 378px; height: 114px;" runat="server" BackColor="White" CssClass="formfield" Width="152px"
		SelectionMode="Multiple" TabIndex="10"></asp:ListBox>
	<asp:ListBox ID="UnassignedGroupsListBox" Style="z-index: 110; left: 240px; position: absolute;
		top: 378px; height: 114px;" runat="server" BackColor="White" CssClass="formfield" Width="152px"
		SelectionMode="Multiple" TabIndex="13"></asp:ListBox>
	<asp:Button ID="AssignGroupsButton" Style="z-index: 113; left: 200px; position: absolute;
		top: 400px; padding-left:1px;padding-right:1px;width:20px" runat="server" CssClass="formfieldtitle" Text="<<" TabIndex="11">
	</asp:Button>


	<FMControls:FMLabel ID="Label6" Style="z-index: 116; left: 224px; position: absolute;
		top: 72px" runat="server" BackColor="Transparent" Width="8px" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>
	<FMControls:FMLabel ID="UserNameRequiredLabel" Style="z-index: 115; left: 224px;
		position: absolute; top: 40px" runat="server" BackColor="Transparent" Width="8px"
		Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>

	<FMControls:FMButton id="EntityAssignment" 
		style="Z-INDEX: 107; LEFT: 32px; POSITION: absolute; TOP: 498px" runat="server"
		CssClass="formfieldtitle" Text="Entity Assignment..." Width="150px" TabIndex="14" 
		Enabled="False"></FMControls:FMButton>
	<FMControls:FMButton id="SiteAssignment" 
		style="Z-INDEX: 107; LEFT: 32px; POSITION: absolute; TOP: 535px" runat="server"
		CssClass="formfieldtitle" Text="Group Assignments..." Width="150px" TabIndex="15" 
		Enabled ="false"></FMControls:FMButton>
        
    <script type="text/javascript">
        var oUserID = document.getElementById("Name");

        if (!oUserID.disabled) {
            oUserID.focus();
            oUserID.setActive();
        }
	</script>
