<%@ Page language="c#" Codebehind="StateSelectForm.aspx.cs" AutoEventWireup="true" Inherits="FuelsManager.FMWebApp.StateSelectForm"  %>
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

		<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" defer="defer"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<SCRIPT>
		    function Select(stateId, title)
		    {
		        var result = new Array();
		        result[0] = ProductID;
		        result[1] = title;

		        setWindowReturnValue(result);
		        closeDialogWindow();
		    }

		    function MultipleSelect()
		    {
		        var result = new Array();
		        var stateTable = document.getElementById("StateDataGrid");

		        if (stateTable != null)
		        {
		            var resultIndex = 0;
		            for (var index = 0; index < stateTable.rows.length; index++)
		            {
		                if (stateTable.rows[index].className === "GVFixedFooter" ||
							stateTable.rows[index].className === "GVFixedHeader")
		                {
		                    continue;
		                }

		                if (stateTable.rows[index].cells[0].childNodes[0].checked)
		                {
		                    result[resultIndex] = stateTable.rows[index].cells[1].innerText;
		                    resultIndex++;
		                }
		            }
		        }

		        setWindowReturnValue(result);
		        closeDialogWindow();
		    }

		    function NoSelect()
		    {
		        var result = new Array();
		        setWindowReturnValue(result);
		        closeDialogWindow();
		    }
		</SCRIPT>
        <form id="Form1" method="post" runat="server">
            <asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <asp:TextBox ID="FindTextBox" Style="z-index: 101; left: 8px; position: absolute; top: 14px" TabIndex="2"
                runat="server" CssClass="formfield" Width="300px" MaxLength="100"></asp:TextBox>
            <FMControls:FMButton ID="FindBtn" Style="z-index: 103; left: 328px; position: absolute; top: 8px" TabIndex="3"
                runat="server" CssClass="formfieldtitle" Width="64px" Text="Find"></FMControls:FMButton>
            <FMControls:FMButton ID="ShowAllBtn" Style="z-index: 104; left: 408px; position: absolute; top: 8px"
                TabIndex="4" runat="server" CssClass="formfieldtitle" Width="64px" Text="Show All"></FMControls:FMButton>
            <table id="Table1" style="z-index: 101; left: 8px; width: 50%; position: absolute; top: 45px; height: 10px"
                cellspacing="0" cellpadding="1" border="0">
                <tr>
                    <td style="width: 819px; height: 10px" width="819">
                        <FMControls:FMDataGrid ID="StateDataGrid" TabIndex="5" runat="server" BackColor="White" CssClass="tabletext" RowHeaderColumn="ID"
                            Width="464px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3"
                            PageSize="12">
                            <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                            <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                            <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                            <Columns>
                                <asp:TemplateColumn>
                                    <HeaderStyle Width="0.125in"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="ID" HeaderText="ID">
                                    <HeaderStyle Width="2in"></HeaderStyle>
                                </asp:BoundColumn>
                            </Columns>
                            <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                        </FMControls:FMDataGrid></td>
                    <tr>
                    </tr>
            </table>
        </form>
	</body>
</HTML>
