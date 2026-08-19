<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="FuelsManager._default" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Frameset//EN">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/FuelsManager.css" %>" rel="stylesheet"/>
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/cfs.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>"  defer="defer"></script>
</head>
	<body id="PageBody" runat="server" tabIndex="-1" leftMargin="0" rightMargin="0" topMargin="0" MS_POSITIONING="GridLayout" style="background-color:#0D256B">
		<table height="100%" width="100%" border="0" cellpadding="0" cellspacing="0" style="vertical-align:middle">
			<tr>
				<td>
					<form id="Form1" method="post" runat="server">
					
					    <table id="WarnTable" runat="server" align="center" style="background-image:url('FMWebApp\images\Warn_Box_7.jpg'); width:680px; 
					        height:500px; background-position:center; background-repeat:no-repeat;" cellpadding="30" >
					        <tr align="center">
					            <td>
					                <asp:Label ID="TitleLabel" runat="server" CssClass="headline" style="position:relative; left:-10px" />
					                <br /><br />
					                <div ID="WarningLabel" runat="server" class="formfield" style="text-align:justify; width:90%; position:relative; left:-10px"/>
					                <br />
					                <asp:Button ID="AcceptButton" runat="server" CssClass="formfieldtitle" Text="Accept" style="position:relative; left:-10px" />
					            </td>
					        </tr>
					    </table>
					</form>
				</td>
			</tr>
		</table>
	</body>
</html>
