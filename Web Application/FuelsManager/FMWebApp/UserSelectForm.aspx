<%@ Page language="c#" Codebehind="UserSelectForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.UserSelectForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title></title>
		<base target="_self">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
 		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
<%=  Global.LinkAccessibilityCssUrl(Session) %>

		<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>"  defer="defer"></script>
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<script type="text/javascript">
		    function Select(UserID) {
		        var Result = new Array();
		        Result[0] = UserID;
		        setWindowReturnValue(Result);
		        closeDialogWindow();
		    }



		    function NoSelect() {
		        var Result = new Array();
		        Result[0] = "{unassigned}";
		        setWindowReturnValue(Result);
		        closeDialogWindow();
		    }
		</script>
		<form id="Form1" method="post" runat="server">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
            <asp:textbox id="FindTextBox" ToolTip="Find" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 8px" tabIndex="2"
				runat="server" Width="300px" CssClass="formfield" MaxLength="100"></asp:textbox>
		 <FMCONTROLS:FMBUTTON id="FindBtn" style="Z-INDEX: 103; LEFT: 328px; POSITION: absolute; TOP: 8px" tabIndex="3"
				runat="server" Width="64px" CssClass="formfieldtitle" Text="Find" onclick="FindBtn_OnClick"></FMCONTROLS:FMBUTTON>
            <FMCONTROLS:FMBUTTON id="ShowAllBtn" style="Z-INDEX: 104; LEFT: 408px; POSITION: absolute; TOP: 8px"
				tabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All" onclick="FindAllBtn_OnClick"></FMCONTROLS:FMBUTTON>

			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 8px; WIDTH: 600px; POSITION: absolute; TOP: 32px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<TD width="350" height="36" vAlign="middle">
						<FMCONTROLS:FMBUTTON id="AddButton1" tabIndex="3" runat="server" CssClass="formfieldtitle" Width="100px"
							Text="Add"></FMCONTROLS:FMBUTTON>
						<FMControls:FMLabel Width="500px" ID="lblWarning" runat="server" 
						    CssClass="formfield" Text="abc" Visible="false" ForeColor="Red" />
					</TD>
				</tr>
				<tr>
					<td>
						<FMCONTROLS:FMDATAGRIDFIXED id="UserDataGrid" tabIndex="5" runat="server" BackColor="White" Width="8.5in" RowHeaderColumn="User ID"
							CssClass="tabletext" PageSize="12" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
							GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None" Height="450px" aria-label="User Data">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<Columns>
								<asp:TemplateColumn>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate></ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" ID="EditButton" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
								<asp:BoundColumn DataField="ID" HeaderText="User ID">
									<HeaderStyle Width="2in"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Name" HeaderText="Name">
									<HeaderStyle Width="2in"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="EmailAddress" HeaderText="Email Address">
									<HeaderStyle Width="2in"></HeaderStyle>
								</asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" ID="DeleteButton" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</FMCONTROLS:FMDATAGRIDFIXED>
					</td>
				</tr>
				<tr>
					<TD width="350" height="36" vAlign="middle">
						<FMCONTROLS:FMBUTTON id="AddButton2" tabIndex="3" runat="server" CssClass="formfieldtitle" Width="100px"
							Text="Add"></FMCONTROLS:FMBUTTON>
					</TD>
				</tr>
			</TABLE>
		</form>
	</body>
</HTML>
