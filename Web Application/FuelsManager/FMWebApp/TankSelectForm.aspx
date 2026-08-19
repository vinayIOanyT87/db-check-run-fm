<%@ Page language="c#" Codebehind="TankSelectForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.TankSelectForm" %>
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
		<script type="text/javascript">
			function Select(TankID, Title)
			{
                var Result = new Array();
                Result[0] = TankID;
                Result[1] = Title;
                window.returnValue = Result;
                window.close();
                setWindowReturnValue(Result);
                closeDialogWindow();
            }

			function MultipleSelect()
			{
				var Result = new Array();
				var tankTable = document.getElementById("TankDataGrid");
				
				if (tankTable != null)
				{
					var resultIndex = 0;
					for(index = 0; index < tankTable.rows.length; index++)
					{										
					    if (tankTable.rows(index).className == "GVFixedFooter" ||
					        tankTable.rows(index).className == "GVFixedHeader")
					    {
					        continue;
					    }
					    
					    if (tankTable.rows(index).cells(0).childNodes[0].checked)
						{
							Result[resultIndex] = tankTable.rows(index).cells(1).innerText;
							resultIndex++;
						}
					}
				}
				
				window.returnValue = Result;
				window.close();
			}

			function NoSelect()
			{
				var Result=new Array();
				window.returnValue=Result;
				window.close();
			}
        </script>
        <form id="Form1" method="post" runat="server">
            <asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
            <asp:TextBox ID="FindTextBox" Style="z-index: 101; left: 8px; position: absolute; top: 14px" TabIndex="2"
                runat="server" Width="300px" CssClass="formfield" MaxLength="100"></asp:TextBox>
            <FMControls:FMButton ID="FindBtn" Style="z-index: 103; left: 328px; position: absolute; top: 8px" TabIndex="3"
                runat="server" Width="64px" CssClass="formfieldtitle" Text="Find" OnClick="FindBtn_OnClick"></FMControls:FMButton>
            <FMControls:FMButton ID="ShowAllBtn" Style="z-index: 104; left: 408px; position: absolute; top: 8px"
                TabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All" OnClick="FindAllBtn_OnClick"></FMControls:FMButton>
            <FMControls:FMDataGrid ID="TankDataGrid" Style="z-index: 102; left: 8px; position: absolute; top: 45px" RowHeaderColumn="ID"
                TabIndex="5" runat="server" BackColor="White" Width="8.5in" CssClass="tabletext" PageSize="12" CellPadding="3"
                BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical" AutoGenerateColumns="False"
                BorderStyle="None">
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
                    <asp:BoundColumn DataField="ProductID" HeaderText="Product">
                        <HeaderStyle Width="2in"></HeaderStyle>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="ManagerID" HeaderText="Manager">
                        <HeaderStyle Width="1in"></HeaderStyle>
                    </asp:BoundColumn>
                </Columns>
                <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
            </FMControls:FMDataGrid>
        </form>
	</body>
</HTML>
