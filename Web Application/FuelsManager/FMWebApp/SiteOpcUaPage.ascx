<%@ register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ control language="c#" Codebehind="SiteOpcUaPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.SiteOpcUaPage" %>
<HTML>
	<HEAD>
	</HEAD>
	<body>
	    <table style="Z-INDEX: 103; width: 66%; LEFT: 5px; POSITION: absolute; TOP: 5px; height: 300px;" role="presentation" aria-label="layout">
			<tr>
				<td>
					<FMControls:FMLabel ID="ServerEndPointLabel" AssociatedControlID="ServerEndPointTextBox" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Server End Point:</FMControls:FMLabel>
				</td>
				<td>
					<asp:TextBox ID="ServerEndPointTextBox" ToolTip="Server End Point" TabIndex="2" runat="server" Width="400px" CssClass="formfield" MaxLength="250"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td>
					<FMControls:FMLabel ID="SecurityModeLabel" AssociatedControlID="SecurityModeDropDownList" runat="server" Width="144px" CssClass="formfieldtitle">Security Mode:</FMControls:FMLabel>
	            </td>
		        <td>
	                <FMControls:FMDropDownList ID="SecurityModeDropDownList" TabIndex="18" runat="server" Width="250px" CssClass="formfield" MaxLength="6" AutoPostBack="true"></FMControls:FMDropDownList>
				</td>
			</tr>
			<tr>
				<td>
					<FMControls:FMLabel ID="SecurityPolicyLabel" AssociatedControlID="SecurityPolicyDropDownList" runat="server" Width="144px" CssClass="formfieldtitle">Security Policy:</FMControls:FMLabel>
	            </td>
		        <td>
	                <FMControls:FMDropDownList ID="SecurityPolicyDropDownList" TabIndex="18" runat="server" Width="250px" CssClass="formfield" MaxLength="6" AutoPostBack="false"></FMControls:FMDropDownList>
				</td>
			</tr>
			<tr>
				<td>
					<FMControls:FMLabel ID="MessageEncodingLabel" AssociatedControlID="MessageEncodingDropDownList" runat="server" Width="144px" CssClass="formfieldtitle">Message Encoding:</FMControls:FMLabel>
	            </td>
		        <td>
	                <FMControls:FMDropDownList ID="MessageEncodingDropDownList" TabIndex="18" runat="server" Width="250px" CssClass="formfield" MaxLength="6" AutoPostBack="false"></FMControls:FMDropDownList>
				</td>
			</tr>
			<tr>
				<td>
					<FMControls:FMLabel ID="UserIdentityMethodLabel" AssociatedControlID="UserIdentityMethodDropDownList" runat="server" Width="144px" CssClass="formfieldtitle">User Identity:</FMControls:FMLabel>
	            </td>
		        <td>
	                <FMControls:FMDropDownList ID="UserIdentityMethodDropDownList" TabIndex="18" runat="server" Width="250px" CssClass="formfield" MaxLength="6" AutoPostBack="true"></FMControls:FMDropDownList>
				</td>
			</tr>
			<tr>
				<td>
					<FMControls:FMLabel ID="UserIdLabel" AssociatedControlID="UserNameOrCertificateTextBox" runat="server" CssClass="formfieldtitle" BackColor="Transparent">User Name:</FMControls:FMLabel>
				</td>
				<td>
					<asp:TextBox ID="UserNameOrCertificateTextBox" ToolTip="User Name" TabIndex="2" runat="server" Width="400px" CssClass="formfield" MaxLength="250"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td>
					<FMControls:FMLabel ID="Password" AssociatedControlID="UserNameOrCertificateTextBox" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Password:</FMControls:FMLabel>
				</td>
				<td>
					<asp:TextBox ID="PasswordTextBox" ToolTip="Password" TabIndex="2" runat="server" Width="250px" CssClass="formfield" MaxLength="250" TextMode="Password"></asp:TextBox>
				</td>
			</tr>
	    </table>
	</body>
</html>
