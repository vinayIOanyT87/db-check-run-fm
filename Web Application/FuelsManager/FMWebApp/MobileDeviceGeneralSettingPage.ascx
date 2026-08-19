<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MobileDeviceGeneralSettingPage.ascx.cs" Inherits="FuelsManager.FMWebApp.MobileDeviceGeneralSettingPage" %>

<html>
	<head>
		<title></title>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
		<style type="text/css">
		    .style1
		    {
		        width: 292px;
		    }
		    .style2
		    {
		        width: 265px;
		    }
		</style>
	</head>
	<body>
		<table style="Z-INDEX: 103; width:31%; LEFT: 5px; POSITION: absolute; TOP: 50px; height: 64px;">     
			<tr>
				<td>
					<FMCONTROLS:FMLABEL id="MobileDeviceIdLbl" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
						Width="160px">Mobile Device ID</FMCONTROLS:FMLABEL>                
				</td>
				<td class="style1" >
					<asp:textbox id="MobileDeviceIdTxtBox" tabIndex="1" Width="271px" CssClass="formfield" runat="server" 
						MaxLength="50" Columns="50"></asp:textbox>               
				</td>
			</tr>
			<tr>
				<td >
					<FMCONTROLS:FMLABEL id="DescriptionLbl" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
						Width="160px">Description</FMCONTROLS:FMLABEL>
				</td>
				<td class="style1" >
					<asp:textbox id="DescriptionTxtBox" tabIndex="2" Width="278px" CssClass="formfield" runat="server" 
					MaxLength="200" TextMode="MultiLine"></asp:textbox>                
				</td>
			</tr>
		</table>	
		<table style="Z-INDEX: 103; width:45%; LEFT: 5px; POSITION: absolute; TOP: 150px; height: 64px;">
			<tr>
				<td>
					<FMCONTROLS:FMLABEL id="AssignedProfilesLbl" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
						Width="160px">Assigned Mobile Device Profiles</FMCONTROLS:FMLABEL> 					
				</td>
				<td>&nbsp;</td>
				<td>
					<FMCONTROLS:FMLABEL id="UnassignedProfilesLbl" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
						Width="160px">Unassigned Mobile Device Profiles</FMCONTROLS:FMLABEL> 										
				</td>
			</tr>
			<tr>
				<td>
					<FMControls:FMListBox ID="AssignedListBox" CssClass="formfieldtitle" 
						runat="server" Height="180px" Width="235px" SelectionMode="Multiple">
					</FMControls:FMListBox>
				</td>
				<td>
					<FMControls:FMButton ID="AssignButton" Text="<<" CssClass="formfieldtitle" 
						runat="server" onclick="AssignButtonOnClick" />
					<br/>
					<br/>
					<FMControls:FMButton ID="UnassignButton" Text=">>" CssClass="formfieldtitle" 
						runat="server" onclick="UnassignButtonOnClick" />
				</td>
				<td>
					<FMControls:FMListBox ID="UnassignedListBox" CssClass="formfieldtitle" 
						runat="server" Height="180px" Width="235px">
					</FMControls:FMListBox>
				</td>
			</tr>
		</table>
	</body>
</html>