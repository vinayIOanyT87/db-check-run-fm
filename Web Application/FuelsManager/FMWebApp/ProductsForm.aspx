<%@ Page language="c#" Codebehind="ProductsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ProductsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
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
	<body ms_positioning="GridLayout" role="application">
        <form id="Form1" method="post" enctype="multipart/form-data" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label2" Style="z-index: 104; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="264px" BackColor="Transparent">Products Configuration</FMControls:FMLabel>
                <FMControls:FMLabel ID="TypeLabel" AssociatedControlID="ProductTypeDropDownList" Style="z-index: 105; left: 32px; position: absolute; top: 40px" runat="server"
                    CssClass="formfieldtitle" BackColor="Transparent">Type:</FMControls:FMLabel>
                <FMControls:FMDropDownList ID="ProductTypeDropDownList" Style="z-index: 106; left: 32px; position: absolute; top: 64px"
                    runat="server" CssClass="formfield" Width="136px" AutoPostBack="True" OnSelectedIndexChanged="ProductTypeDropDownListSelectedIndexChanged">
                </FMControls:FMDropDownList>
                <FMControls:FMLabel ID="FindLabel" AssociatedControlID="FindTextBox" Style="z-index: 111; left: 232px; position: absolute; top: 40px"
                    runat="server" CssClass="formfieldtitle" BackColor="Transparent">Find String:</FMControls:FMLabel>
                <asp:TextBox ID="FindTextBox" Style="z-index: 108; left: 232px; position: absolute; top: 64px"
                    runat="server" CssClass="formfield" Width="264px" MaxLength="100"></asp:TextBox>
                <FMControls:FMButton ID="FindBtn" Style="z-index: 109; left: 504px; position: absolute; top: 58px" runat="server"
                    CssClass="formfieldtitle" Width="64px" Text="Find" OnClick="FindBtnOnClick"></FMControls:FMButton>
                <FMControls:FMButton ID="ShowAllBtn" Style="z-index: 110; left: 584px; position: absolute; top: 58px"
                    runat="server" CssClass="formfieldtitle" Width="64px" Text="Show All" OnClick="FindAllOnClick"></FMControls:FMButton>
                <FMControls:FMCheckBox ID="ShowHiddenCheckBox" Style="z-index: 110; left: 660px; position: absolute; top: 65px"
                    CssClass="formfieldtitle" runat="server" TextAlign="Left" Text="Show Hidden" AutoPostBack="True" OnCheckedChanged="ShowHiddenCheckBox_OnCheckedChanged"></FMControls:FMCheckBox>
                <table id="Table1" style="z-index: 101; left: 32px; width: 50%; position: absolute; top: 96px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
                    <tr>
                        <td width="350" height="36" valign="middle">
                            <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle" />
                            <FMControls:FMPageSizeDropDown ID="ProductSummaryPageSizeDropDown" ToolTip="Page Size" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
                            <FMControls:FMLabel Width="500px" ID="lblWarning" runat="server"
                                CssClass="formfield" Text="abc" Visible="false" ForeColor="Red" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 549px; height: 10px">
                            <FMControls:FMDataGridFixedPaging ID="ProductsDataGrid" runat="server" RowHeaderColumn="Product ID"
                                BorderStyle="None"
                                BackColor="White"
                                AutoGenerateColumns="False"
                                GridLines="Vertical"
                                Width="624px"
                                BorderWidth="1px"
                                AllowSorting="True"
                                BorderColor="White" CellPadding="3"
                                AllowPaging="True"
                                CssClass="tabletext"
                                Style="left: 1px; top: 0px"
                                PageSize="12"
                                ShowHeaderWhenEmpty="True"
                                ShowFooter="False"
                                ShowFooterWhenEmpty="False"
                                FixedHeaders="True"
                                GroupColumnOffset="0"
                                GroupingDepth="0"
                                FixedHeight="550px"
                                Height="550px">
                                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                                <Columns>
                                    <asp:TemplateColumn HeaderText="Edit">
                                        <HeaderStyle Width="55px"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        <ItemTemplate>
                                            <FMControls:FMEditLinkButton runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
                                    <asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="ID" HeaderText="Product ID"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="Code" HeaderText="Product Code"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="Description" HeaderText="Description"></asp:BoundColumn>
                                    <asp:BoundColumn DataField="Type" HeaderText="Type"></asp:BoundColumn>
                                    <asp:TemplateColumn HeaderText="Vapor Recovery">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        <ItemTemplate>
                                            <FMControls:FMCheckBox runat="server" CssClass="tabletext" Enabled="false" Checked='<%# DataBinder.Eval(Container, "DataItem.VaporRecovery") %>'></FMControls:FMCheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn DataField="HiddenDate" Visible="False"></asp:BoundColumn>
                                    <asp:TemplateColumn HeaderText="Delete">
                                        <HeaderStyle Width="0.5in"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        <ItemTemplate>
                                            <FMControls:FMDeleteLinkButton runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                                <PagerStyle CssClass="GVFixedFooter" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages" />
                            </FMControls:FMDataGridFixedPaging></td>
                    </tr>
                    <tr>
                        <td style="width: 163px; height: 36px" valign="middle">
                            <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></td>
                    </tr>
                </table>
            </div>
        </form>
		<script language="jscript">
            var findBtn = document.getElementById("FindBtn");
            var findTbBtn = document.getElementById("FindTextBox");

            if (findBtn != null && findTbBtn != null)
			{
			    try
			    {
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
</HTML>
