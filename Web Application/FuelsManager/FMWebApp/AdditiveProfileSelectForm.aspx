<%@ Page language="c#" Codebehind="AdditiveProfileSelectForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.AdditiveProfileSelectForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ OutputCache Location="None" VaryByParam="None" %>
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
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<script type="text/javascript">

		    function Select(AdditiveProfileID, Title, AssignToGuid)
			{
				var Result = new Array();
				Result[0] = AdditiveProfileID;
				Result[1] = Title;
				Result[2] = AssignToGuid;
				setWindowReturnValue(Result);
				closeDialogWindow();
			}

		    function MultipleSelect()
		    {
		        var Result = new Array();
		        var AdditiveProfileTable = document.getElementById("AdditiveProfileDataGrid");
		        if (AdditiveProfileTable != null) 
		        {
		            var resultIndex = 0;
		            for (index = 0; index < AdditiveProfileTable.rows.length; index++) {
		                if (AdditiveProfileTable.rows[index].className == "GVFixedFooter" ||
					        AdditiveProfileTable.rows[index].className == "GVFixedHeader") {
		                    continue;
		                }

		                if (AdditiveProfileTable.rows[index].cells[0].childNodes[0].checked) {
		                    Result[resultIndex] = AdditiveProfileTable.rows[index].cells[1].innerText;
		                    resultIndex++;
		                }
		            }
		        }
		        setWindowReturnValue(Result);
		        closeDialogWindow();
		    }

			function NoSelect()
			{
				var Result=new Array();
				setWindowReturnValue(Result);
				closeDialogWindow();
			}
		</script>
		<form id="Form1" method="post" runat="server">
			<asp:image id="Image1" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
            <asp:textbox id="FindTextBox" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 14px" tabIndex="2"
				runat="server" Width="300px" CssClass="formfield" MaxLength="100"></asp:textbox>
            <FMCONTROLS:FMBUTTON id="ShowAllBtn" style="Z-INDEX: 104; LEFT: 408px; POSITION: absolute; TOP: 8px"
				tabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All" onclick="FindAllBtn_OnClick"></FMCONTROLS:FMBUTTON>
            <FMCONTROLS:FMBUTTON id="FindBtn" style="Z-INDEX: 103; LEFT: 328px; POSITION: absolute; TOP: 8px" tabIndex="3"
				runat="server" Width="64px" CssClass="formfieldtitle" Text="Find" onclick="FindBtn_OnClick"></FMCONTROLS:FMBUTTON>
			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 8px; WIDTH: 50%; POSITION: absolute; TOP: 45px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<TD width="350" height="36" vAlign="middle">
						<FMControls:FMLabel Width="500px" ID="lblWarning" runat="server" 
						    CssClass="formfield" Text="abc" Visible="false" ForeColor="Red" />
					</TD>
				</tr>
				<TR>
					<TD style="WIDTH: 549px; HEIGHT: 10px" width="549">
						<FMCONTROLS:FMDATAGRIDFIXED id="AdditiveProfileDataGrid" tabIndex="5" runat="server" BackColor="White" Width="800px"
							CssClass="tabletext" PageSize="12" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
							GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None" Height="450px">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<Columns>
								<asp:TemplateColumn>
									<HeaderStyle Width="0.125in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="ID" HeaderText="ID">
									<HeaderStyle Width="2in"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Description" HeaderText="Description">
									<HeaderStyle Width="1in"></HeaderStyle>
								</asp:BoundColumn>
							</Columns>
						</FMCONTROLS:FMDATAGRIDFIXED></TD>
				<tr>
					<TD width="350" height="36" vAlign="middle">
					</TD>
				</tr>
			</TABLE>
		</form>
	</body>
</HTML>
