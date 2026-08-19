<%@ Page Language="c#" CodeBehind="AdditiveProfilesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.AdditiveProfilesForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
  </HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="312px" BackColor="Transparent">Additive Profiles Configuration</FMControls:FMLabel>
                <table id="Table1" style="z-index: 100; left: 32px; width: 43.18%; position: absolute; top: 48px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
                    <tr>
                        <td width="498" height="36" valign="middle">
                            <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle" />
                            &nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="AdditiveProfilesFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 498px; height: 10px" width="498">
                            <FMControls:FMDataGrid ID="AdditiveProfilesDataGrid" RowHeaderColumn="ID"
                                Style="left: 1px; top: 0px" runat="server" PageSize="16"
                                CssClass="tabletext" AllowPaging="True" CellPadding="3" BorderColor="White"
                                AllowSorting="True" BorderWidth="1px" Width="467px" GridLines="Vertical"
                                AutoGenerateColumns="False" BackColor="White" BorderStyle="None">
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
                                    <asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid">
                                        <HeaderStyle Wrap="False"></HeaderStyle>
                                        <ItemStyle Wrap="False"></ItemStyle>
                                        <FooterStyle Wrap="False"></FooterStyle>
                                    </asp:BoundColumn>
                                    <asp:BoundColumn DataField="ID" HeaderText="ID">
                                        <HeaderStyle Wrap="False"></HeaderStyle>
                                        <ItemStyle Wrap="False"></ItemStyle>
                                        <FooterStyle Wrap="False"></FooterStyle>
                                    </asp:BoundColumn>
                                    <asp:BoundColumn DataField="Description" HeaderText="Description">
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
                        <td style="width: 498px; height: 50px" valign="middle">
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
