<%@ Page language="c#" Codebehind="AccountingTankSelectForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.AccountingTankSelectForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<!DOCTYPE html>
<html>
	<head>
		<title></title>
		<base target="_self" />
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	</head>
	<body MS_POSITIONING="GridLayout">
        <script type="text/javascript">
			function Select(TankID, Title)
			{
                var Result = new Array();
                Result[0] = TankID;
                Result[1] = Title;
                window.returnValue = Result;
                window.close();
			}

			function MultipleSelect()
			{
				var Result = new Array();
				var tankTable = document.getElementById("TankDataGrid");
				
                if (tankTable != null)
				{
                    var resultIndex = 0;
                    for (index = 1; index < tankTable.rows.length; index++)
					{
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
                var Result = new Array();
                window.returnValue = Result;
                window.close();
			}
        </script>
        <form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                    ImageUrl="..\FMWebApp\images\Page_Fade_7.jpg" BackColor="Transparent"></asp:Image>
                <asp:TextBox ID="FindTextBox" Style="z-index: 101; left: 8px; position: absolute; top: 8px" TabIndex="2"
                    runat="server" Width="300px" CssClass="formfield" MaxLength="100"></asp:TextBox>
                <FMControls:FMButton ID="FindBtn" Style="z-index: 103; left: 328px; position: absolute; top: 8px" TabIndex="3"
                    runat="server" Width="64px" CssClass="formfieldtitle" Text="Find" OnClick="FindBtn_OnClick"></FMControls:FMButton>
                <FMControls:FMButton ID="ShowAllBtn" Style="z-index: 104; left: 408px; position: absolute; top: 8px"
                    TabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All" OnClick="FindAllBtn_OnClick"></FMControls:FMButton>
                <FMControls:FMDataGrid ID="TankDataGrid" Style="z-index: 102; left: 8px; position: absolute; top: 40px" RowHeaderColumn="ID"
                    TabIndex="5" runat="server" BackColor="White" Width="8.5in" CssClass="tabletext" PageSize="12" CellPadding="3"
                    BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical" AutoGenerateColumns="False"
                    BorderStyle="None">
                    <FooterStyle ForeColor="Black" BackColor="#0d246a"></FooterStyle>
                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                    <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#0d246a"></HeaderStyle>
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
                    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#0d246a" Mode="NumericPages"></PagerStyle>
                </FMControls:FMDataGrid>
            </div>
        </form>
    </body>
</html>
