<%@ Page Language="c#" CodeBehind="ProductSelectForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ProductSelectForm" %>
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
            function Select(productId, title)
			{
                var result = new Array();
                result[0] = productId;
                result[1] = title;

                window.returnValue = result;
                window.close();
                setWindowReturnValue(result);
                closeDialogWindow();
			}

            function MultipleSelect()
			{
                var result = new Array();
                var productTable = document.getElementById("ProductDataGrid");

                if (productTable != null)
				{
                    var resultIndex = 0;
                    for (var index = 0; index < productTable.rows.length; index++)
					{										
                        if (productTable.rows[index].className === "GVFixedFooter" ||
                            productTable.rows[index].className === "GVFixedHeader")
					    {
                            continue;
					    }
					    
                        if (productTable.rows[index].cells[0].childNodes[0].checked)
						{
                            result[resultIndex] = productTable.rows[index].cells[2].innerText;
                            resultIndex++;
						}
					}
				}
                window.returnValue = result;
                window.close();

                setWindowReturnValue(result);
                closeDialogWindow();
			}

            function NoSelect()
			{
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
            <asp:TextBox ID="FindTextBox" Style="z-index: 101; left: 10px; position: absolute; top: 14px" 
                runat="server" Width="300px" CssClass="formfield"></asp:TextBox>
            <FMControls:FMButton ID="FindBtn" Style="z-index: 103; left: 328px; position: absolute; top: 8px" 
                runat="server" Width="64px" CssClass="formfieldtitle" Text="Find" OnClick="FindBtn_OnClick"></FMControls:FMButton>
            <FMControls:FMButton ID="ShowAllBtn" Style="z-index: 104; left: 408px; position: absolute; top: 8px"
                 runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All" OnClick="FindAllBtn_OnClick"></FMControls:FMButton>
            <table id="Table1" style="z-index: 101; left: 8px; width: 50%; position: absolute; top: 45px; height: 10px"
                cellspacing="0" cellpadding="1" border="0">
                <tr>
                    <td width="350" height="36" valign="middle">
                        <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle" />
                        <FMControls:FMLabel Width="500px" ID="lblWarning" runat="server"
                            CssClass="formfield" Text="abc" Visible="false" ForeColor="Red" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 549px; height: 10px">
                        <FMControls:FMDataGridFixed ID="ProductDataGrid" runat="server" BackColor="White" Width="800px"
                            CssClass="tabletext" PageSize="12" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
                            GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None" Height="380px">
                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                            <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                            <Columns>
                                <asp:TemplateColumn>
                                    <HeaderStyle Width="0.125in"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Edit">
                                    <HeaderStyle Width="55px"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMEditLinkButton runat="server" ID="Fmeditlinkbutton1" NAME="Fmeditlinkbutton1" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
                                <asp:BoundColumn DataField="ID" HeaderText="Product ID">
                                    <HeaderStyle Width="2in"></HeaderStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Code" HeaderText="Code">
                                    <HeaderStyle Width="2in"></HeaderStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Description" HeaderText="Description">
                                    <HeaderStyle Width="1in"></HeaderStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Type" HeaderText="Type">
                                    <HeaderStyle Width="1in"></HeaderStyle>
                                </asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Delete">
                                    <HeaderStyle Width="0.5in"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMDeleteLinkButton runat="server" ID="Fmdeletelinkbutton1" NAME="Fmdeletelinkbutton1" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                        </FMControls:FMDataGridFixed></td>
                </tr>
                <tr>
                    <td width="350" height="36" valign="middle">
                        <FMControls:FMButton ID="AddButton1" runat="server" CssClass="formfieldtitle" Width="100px"
                            Text="Add"></FMControls:FMButton>
                    </td>
                </tr>
            </table>
        </form>
    </body>
</HTML>
