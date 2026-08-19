<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Page Language="c#" CodeBehind="CompanySelectForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.CompanySelectForm" %>

<%@ OutputCache Location="None" VaryByParam="None" %>
<!DOCTYPE html>
<html>
<head>
    <title></title>
    <base target="_self">
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
</head>
<body ms_positioning="GridLayout">
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
<%=  Global.LinkAccessibilityCssUrl(Session) %>
    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>"  defer="defer"></script>

    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
    <script>
        function Select(companyId, title, companyName) {
            var result = new Array();
            result[0] = companyId;
            result[1] = title;
            result[2] = companyName;
            window.returnValue = result;
            window.close();
            setWindowReturnValue(result);
            closeDialogWindow();
        }

        function MultipleSelect() {
            var result = new Array();
            var companyTable = document.getElementById("CompaniesDataGrid");

            if (companyTable != null) {
                var resultIndex = 0;
                for (var index = 0; index < companyTable.rows.length; index++) {
                    if (companyTable.rows[index].className === "GVFixedFooter" ||
                        companyTable.rows[index].className === "GVFixedHeader") {
                        continue;
                    }

                    if (companyTable.rows[index].cells[0].childNodes[0].checked) {
                        result[resultIndex] = companyTable.rows[index].cells[2].innerText;
                        resultIndex++;
                    }
                }
            }
            window.returnValue = result;
            window.close();
            setWindowReturnValue(result);
            closeDialogWindow();
        }

        function NoSelect() {
            var result = new Array();
            window.returnValue = result;
            window.close();
            setWindowReturnValue(result);
            closeDialogWindow();
        }
    </script>
    <form id="Form1" method="post" runat="server">
        <asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
            ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
        <asp:TextBox ID="FindTextBox" Style="z-index: 101; left: 8px; position: absolute; top: 18px" TabIndex="2"
            runat="server" Width="300px" CssClass="formfield"></asp:TextBox>
        <FMControls:FMButton ID="ShowAllBtn" Style="z-index: 104; left: 408px; position: absolute; top: 8px"
            TabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All" OnClick="FindAllBtn_OnClick"></FMControls:FMButton>
        <FMControls:FMButton ID="FindBtn" Style="z-index: 103; left: 328px; position: absolute; top: 8px" TabIndex="3"
            runat="server" Width="64px" CssClass="formfieldtitle" Text="Find" OnClick="FindBtn_OnClick"></FMControls:FMButton>
        <table id="Table1" style="z-index: 101; left: 8px; width: 600px; position: absolute; top: 55px; height: 10px"
            cellspacing="0" cellpadding="1" border="0">
            <tr>
                <td width="350" height="36" valign="middle">
                    <FMControls:FMButton ID="AddButton1" TabIndex="3" runat="server" CssClass="formfieldtitle" Width="100px"
                        Text="Add"></FMControls:FMButton>
                    <FMControls:FMLabel Width="500px" ID="lblWarning" runat="server"
                        CssClass="formfield" Text="abc" Visible="false" ForeColor="Red" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMDataGridFixed ID="CompaniesDataGrid" TabIndex="5" runat="server" RowHeaderColumn="Company ID"
                        BackColor="White" Width="810px" RowScope="Company ID"
                        CssClass="tabletext" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical"
                        AutoGenerateColumns="False" BorderStyle="None" Height="380px" FixedHeaders="True"  
                        FixedHeight="350px" ShowFooter="True">
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
                                    <FMControls:FMEditLinkButton runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
                            <asp:BoundColumn Visible="False" DataField="IdentityGuid"
                                HeaderText="IdentityGuid"></asp:BoundColumn>
                            <asp:BoundColumn DataField="ID" HeaderText="Company ID"></asp:BoundColumn>
                            <asp:BoundColumn DataField="Code" HeaderText="Code"></asp:BoundColumn>
                            <asp:BoundColumn DataField="Name" HeaderText="Name"></asp:BoundColumn>
                            <asp:BoundColumn DataField="Address1" HeaderText="Address"></asp:BoundColumn>
                            <asp:BoundColumn DataField="City" HeaderText="City"></asp:BoundColumn>
                            <asp:BoundColumn DataField="State" HeaderText="State"></asp:BoundColumn>
                            <asp:TemplateColumn HeaderText="Delete">
                                <HeaderStyle Width="0.5in"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <FMControls:FMDeleteLinkButton runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </FMControls:FMDataGridFixed>
                </td>
            </tr>
            <tr>
                <td width="350" height="36" valign="middle">
                    <FMControls:FMButton ID="AddButton2" TabIndex="3" runat="server" CssClass="formfieldtitle" Width="100px"
                        Text="Add"></FMControls:FMButton>
                </td>
            </tr>
        </table>
    </form>
    <script language="jscript">
        var findBtn = document.getElementById("FindBtn");
        var findTbBtn = document.getElementById("FindTextBox");

        if (findBtn != null && findTbBtn != null) {
            try {
                findBtn.setActive();
                findTbBtn.focus();
            }
            catch (err) { }
        }
        // Set the Find Button to be activated by the enter key.
        document.addEventListener('keydown', function (ev) {
            if (ev.keyCode == 13) {
                ev.returnValue = false;
                ev.cancel = true;
                document.all("FindBtn").click();
            }
        });
    </script>
</body>
</html>
