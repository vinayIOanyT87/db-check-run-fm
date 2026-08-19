<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WacOverride.aspx.cs" Inherits="ADFWebApp.WacOverride" %>

<%@ Register assembly="FMControls" namespace="FMControls" tagprefix="FMCONTROLS" %>
<%@ Register TagPrefix="FM" Namespace="FMControls" Assembly="FMControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Untitled Page</title>
        <style type="text/css">
            .headline {  font-family: Arial, Helvetica, sans-serif; font-size: 18px; font-style: italic; font-weight: bold; color: #666699; line-height: normal}
            .formfieldtitle {  font-family: Arial, Helvetica, sans-serif; font-size: 12px; font-style: normal; line-height: normal; font-weight: bold; font-variant: normal; text-transform: none; color: #333333}
            .formfield {  font-family: Arial, Helvetica, sans-serif; font-size: 12px; font-style: normal; line-height: normal; font-weight: normal; font-variant: normal; text-transform: none; color: #000000}
            .tabletext {  font-family: Arial, Helvetica, sans-serif; font-size: 10px; font-style: normal; line-height: normal; font-weight: normal; font-variant: normal; text-transform: none; color: #000000; text-decoration: none; list-style-image: none}
            .tablecolhead {  font-family: Arial, Helvetica, sans-serif; font-size: 12px; font-style: normal; line-height: normal; font-weight: bold; font-variant: normal; text-transform: none; color: #FFFFFF; text-align: left; vertical-align: middle; text-decoration: none}
            .tablepager {  font-family: Arial, Helvetica, sans-serif; font-size: 12px; font-style: normal; line-height: normal; font-weight: bold; font-variant: normal; text-transform: none; color: #FFFFFF; text-align: center; vertical-align: middle; text-decoration: none}
            .style1
            {
                width: 434px;
            }
            .style3
            {
                font-size: x-large;
            }
        </style>
    </head>
    <body ms_positioning="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:image id="FadeImage" 
                style="Z-INDEX: -101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image>
			<FMCONTROLS:FMLABEL id="lblHeading" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" BackColor="Transparent" CssClass="headline" Width="272px">WAC 
                Detail</FMCONTROLS:FMLABEL>
			<br />
			<table cellspacing="2" cellpadding="2" border="0">
			    <tr>
			        <td>
			            <FMCONTROLS:FMLabel ID="lblOverrideDate" runat="server" CssClass="formfieldtitle">Override Date</FMCONTROLS:FMLabel>
			        </td>
			        <td>&nbsp;</td>
			        <td>
			            <FM:FMDATETIME id="overrideDateControl" runat="server" CssClass="formfield" />
			        </td>
			    </tr>			
			    <tr>
				    <td>
				        <FMCONTROLS:FMLABEL id="labSite" runat="server" CssClass="formfieldtitle">Site</FMCONTROLS:FMLABEL>
				    </td>
				    <td>&nbsp;</td>
				    <td>
                        <asp:TextBox ID="tbSite" runat="server" Width="183px" Enabled="False"></asp:TextBox>
                    </td>
			    </tr>
			    <tr>
				    <td>
				        <FMCONTROLS:FMLABEL id="labProduct" runat="server" CssClass="formfieldtitle">Fuel Type</FMCONTROLS:FMLABEL>
				    </td>
				    <td>&nbsp;</td>
				    <td>
                        <asp:TextBox ID="tbFuelType" runat="server" Width="183px" Enabled="False"></asp:TextBox>
                    </td>
			    </tr>
			    <tr>
				    <td>
				        <FMCONTROLS:FMLABEL id="labLastEdit" runat="server" CssClass="formfieldtitle">Created By</FMCONTROLS:FMLABEL>
				    </td>
				    <td>&nbsp;</td>
				    <td>
                        <asp:TextBox ID="tbLastEdit" runat="server" Width="183px" Enabled="False"></asp:TextBox>
                    </td>
			    </tr>
			    <tr>
				    <td>
				        <FMCONTROLS:FMLABEL id="labValue" runat="server" CssClass="formfieldtitle">WAC Value</FMCONTROLS:FMLABEL>
				    </td>
				    <td align="left">
				        <FMCONTROLS:FMLABEL id="labRequired" runat="server" CssClass="formfieldtitle" 
                            ForeColor="Crimson"><b><font size="+1">*</font></b></FMCONTROLS:FMLABEL></td>
				    <td>
                        <asp:TextBox ID="tbWacValue" runat="server" Width="183px"></asp:TextBox>
                    </td>
			    </tr>
			    <tr>
				    <td width="100"><FMCONTROLS:FMLABEL id="labNotes" runat="server" CssClass="formfieldtitle">Notes</FMCONTROLS:FMLABEL></td>
				    <td>&nbsp;</td>
				    <td class="style1">
                        <FMControls:FMTextBox ID="tbNotes" runat="server" Height="128px" MaxLength="2047" 
                            TextMode="MultiLine" Width="336px" />
                    </td>
			    </tr>
			    <tr>
				    <td>&nbsp;</td>
				    <td>&nbsp;</td>
				    <td valign="middle">
				        <FMCONTROLS:FMLABEL id="lblRequiredFooter" runat="server" 
                            CssClass="formfieldtitle" ForeColor="Crimson" Width="180px">* Denotes Required Field</FMCONTROLS:FMLABEL>
				        <FM:FMButton id="btnOK" CssClass="formfield" Width="67px" Runat="server" Text="OK" onclick="btnOK_Click" />&nbsp;
					    <FM:FMButton ID="btnCancel" Runat="server" Text="Cancel" Width="67px" CssClass="formfield" onclick="btnCancel_Click" />
					</td>
			    </tr>
		    </table>
	    </form>
	    <p>&nbsp;</p>
	</body>
</html>
