<%@ Page Language="c#" CodeBehind="CompanyGroupsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.CompanyGroupsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
        <form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="320px" BackColor="Transparent">Company Groups Configuration</FMControls:FMLabel>
                <table id="Table1" style="z-index: 100; left: 32px; width: 43.18%; position: absolute; top: 48px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
                    <tr>
                        <td width="350" height="36" valign="middle">
                            <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
                                TabIndex="6" />
                            &nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="CompanyGroupsFormPageSizeDropDown" ToolTip="Page Size" runat="server" TabIndex="7" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 498px; height: 10px" width="498">
                            <FMControls:FMDataGrid ID="CompanyGroupsDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False" RowHeaderColumn="ID"
                                GridLines="Vertical" Width="400px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext"
                                Style="left: 1px; top: 0px" PageSize="16">
                                <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                                <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                                <Columns>
                                    <asp:TemplateColumn HeaderText="Edit">
                                        <HeaderStyle Width="55px"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        <ItemTemplate>
                                            <FMControls:FMEditLinkButton runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="IdentityGuid"
                                        HeaderText="IdentityGuid">
                                        <HeaderStyle Wrap="False"></HeaderStyle>
                                        <ItemStyle Wrap="False"></ItemStyle>
                                        <FooterStyle Wrap="False"></FooterStyle>
                                    </asp:BoundColumn>
                                    <asp:BoundColumn DataField="ID" HeaderText="ID">
                                        <HeaderStyle Wrap="False"></HeaderStyle>
                                        <ItemStyle Wrap="False"></ItemStyle>
                                        <FooterStyle Wrap="False"></FooterStyle>
                                    </asp:BoundColumn>
                                    <asp:TemplateColumn HeaderText="Delete">
                                        <HeaderStyle Width="0.5in"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        <ItemTemplate>
                                            <FMControls:FMDeleteLinkButton runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                                <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                            </FMControls:FMDataGrid></td>
                    </tr>
                    <tr>
                        <td style="width: 498px; height: 36px" valign="middle" width="498">
                            <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></td>
                    </tr>
                </table>
            </div>
        </form>
        <script language="jscript">
            var AddButton = document.getElementById("AddButton2");
            if (!AddButton.disabled)
                AddButton.focus();
        </script>
	</body>
</HTML>
