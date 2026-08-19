<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls"%>
<%@ Page language="c#" Codebehind="DatabaseAuditLogForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.DatabaseAuditLogForm" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
	<HEAD>
		<base target="_self">
		<title></title>
		<meta content="False" name="vs_snapToGrid">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body tabIndex="-1" MS_POSITIONING="GridLayout">

		<form id="DatabaseAuditLogForm" method="post" runat="server">
		<asp:ScriptManager ID="ScriptManager1" runat="server">
		</asp:ScriptManager>

        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
      
         <FMCONTROLS:FMLABEL id="FMLABEL1" style="Z-INDEX: 122; LEFT: 16px; POSITION: absolute; TOP: 61px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent" Width="81px" Height="15px">Login Name:</FMCONTROLS:FMLABEL>
			<input class="formfieldtitle" id="file" tabindex="9" 
            
            
            style="Z-INDEX: 111; WIDTH: 550px; HEIGHT: 22px; POSITION: absolute; TOP: 208px; LEFT: 16px; right: 482px;" type="file"
							size="75" name="file" />
         <FMCONTROLS:FMBUTTON id="ViewReport" style="Z-INDEX: 122; LEFT: 601px; POSITION: absolute; TOP: 208px"
				tabIndex="5" runat="server" CssClass="formfield" Text="View Report" 
            onclick="OnViewReportClick"></FMCONTROLS:FMBUTTON><asp:image id="Image1" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image>
				<FMCONTROLS:FMDATE id="EndingDate" style="Z-INDEX: 114; LEFT: 112px; POSITION: absolute; TOP: 158px"
				tabIndex="8" runat="server" CssClass="formfield" Width="160px"></FMCONTROLS:FMDATE>
				<FMCONTROLS:FMDATE id="BeginningDate" style="Z-INDEX: 115; LEFT: 112px; POSITION: absolute; TOP: 134px"
				tabIndex="7" runat="server" CssClass="formfield" Width="160px"></FMCONTROLS:FMDATE>
				<FMCONTROLS:FMLABEL id="Label6" style="Z-INDEX: 111; LEFT: 16px; POSITION: absolute; TOP: 158px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent" Width="81px" Height="15px">End Date:</FMCONTROLS:FMLABEL>
				<FMCONTROLS:FMLABEL id="FMLABEL4" style="Z-INDEX: 111; LEFT: 16px; POSITION: absolute; TOP: 188px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent" Width="81px" Height="15px">Audit Log File:</FMCONTROLS:FMLABEL>
				
				<FMCONTROLS:FMLABEL id="Label5" style="Z-INDEX: 110; LEFT: 16px; POSITION: absolute; TOP: 133px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent" Width="81" Height="15">Begin Date:</FMCONTROLS:FMLABEL>
				<FMCONTROLS:FMLABEL id="Label27" style="Z-INDEX: 105; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" BackColor="Transparent" Width="200px">Database Audit Log</FMCONTROLS:FMLABEL>&nbsp;&nbsp;
			<FMCONTROLS:FMDROPDOWNLIST id="LoginNameDropDownList" style="Z-INDEX: 123; LEFT: 125px; POSITION: absolute; TOP: 59px"
				runat="server" CssClass="formfield" Width="126px" TabIndex="3" 
            DataSource="<%# EnumerateUserNames()%>"></FMCONTROLS:FMDROPDOWNLIST>
         <FMCONTROLS:FMLABEL id="FMLABEL2" 
            style="Z-INDEX: 126; LEFT: 16px; POSITION: absolute; TOP: 85px; right: 990px;" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent" Width="81px" Height="15px">Event:</FMCONTROLS:FMLABEL>
         <FMCONTROLS:FMDROPDOWNLIST id="ResultDropDown" style="Z-INDEX: 129; LEFT: 125px; POSITION: absolute; TOP: 108px"
				runat="server" CssClass="formfield" Width="126px" TabIndex="4">
				<asp:ListItem Selected="True">{All}</asp:ListItem>
				<asp:ListItem Text="Success" Value="Success"/>
				<asp:ListItem Text="Failure" Value="Failure"/>
				</FMCONTROLS:FMDROPDOWNLIST>
         <FMCONTROLS:FMDROPDOWNLIST id="EventDropDown" style="Z-INDEX: 128; LEFT: 125px; POSITION: absolute; TOP: 83px"
				runat="server" CssClass="formfield" TabIndex="5">
				<asp:ListItem Selected="True">{All}</asp:ListItem>
				<asp:ListItem>Audit Login</asp:ListItem>
            <asp:ListItem>Audit Logout</asp:ListItem>
            <asp:ListItem>Audit Server Start/Stop Event</asp:ListItem>
            <asp:ListItem>Login Failed</asp:ListItem>
            <asp:ListItem>Audit Statement Grant/Deny/Revoke (GDR) Event</asp:ListItem>
            <asp:ListItem>Audit Object GDR Event</asp:ListItem>
            <asp:ListItem>Audit Add Logging Event</asp:ListItem>
            <asp:ListItem>Audit Login GDR Event</asp:ListItem>
            <asp:ListItem>Audit Login Change Property</asp:ListItem>
            <asp:ListItem>Audit Login Change Password</asp:ListItem>
            <asp:ListItem>Audit Add Login to Server Role Event</asp:ListItem>
            <asp:ListItem>Audit Add DB User Event</asp:ListItem>
            <asp:ListItem>Audit Add Member to DB Role Event</asp:ListItem>
            <asp:ListItem>Audit Add Role Event</asp:ListItem>
            <asp:ListItem>Audit App Role Change Password Event</asp:ListItem>
            <asp:ListItem>Audit Statement Permission Event</asp:ListItem>
            <asp:ListItem>Audit Object Permission Event</asp:ListItem>
            <asp:ListItem>Audit Backup/Restore Event</asp:ListItem>
            <asp:ListItem>Audit Database Consistency Check (DBCC) Event</asp:ListItem>
            <asp:ListItem>Audit Change Audit Event</asp:ListItem>
            <asp:ListItem>Audit Object Derived Permission Event</asp:ListItem>
			</FMCONTROLS:FMDROPDOWNLIST>
			<FMCONTROLS:FMLABEL id="FMLABEL3" style="Z-INDEX: 127; LEFT: 16px; POSITION: absolute; TOP: 109px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent" Width="81px" Height="15px">Result:</FMCONTROLS:FMLABEL>

    <div>
        <asp:Label ID="ErrorLabel" runat="server" style="Z-INDEX: 127; margin-top: 78px; LEFT: 16px; POSITION: absolute; TOP: 200px" Visible="False" Width="695px"></asp:Label>
        <FMControls:FMReportViewer ID="RptViewer" runat="server" Height="450px" Width="730px" 
            SizeToReportContent="True" ZoomMode="PageWidth" style="Z-INDEX: 127; margin-top: 78px; LEFT: 16px; POSITION: absolute; TOP: 200px" >
        </FMControls:FMReportViewer>
    </div>

		</div>
</form>
		<script language="jscript">
		   document.getElementById("LoginNameDropDownList").focus();
		</script>
		
	</body>
</HTML>
