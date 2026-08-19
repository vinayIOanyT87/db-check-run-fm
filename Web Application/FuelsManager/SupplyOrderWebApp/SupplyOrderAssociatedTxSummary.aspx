<%@ Page language="c#" Codebehind="SupplyOrderAssociatedTxSummary.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.SupplyOrderWebApp.SupplyOrderAssociatedTxSummary" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
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
        <form id="SupplyOrderAssociatedTxSummary" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="FadeImage" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                    ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
                <FMControls:FMLabel ID="PageTitle" Style="z-index: 101; left: 16px; position: absolute; top: 8px" runat="server"
                    BackColor="Transparent" Width="500px" CssClass="headline">Supply Order Associated Transactions</FMControls:FMLabel>
                <FMControls:FMLabel ID="OrderNumberLabel" Style="z-index: 103; left: 16px; position: absolute; top: 40px"
                    runat="server" BackColor="Transparent" Width="248px" CssClass="formfieldtitle">Order Number</FMControls:FMLabel>
                <FMControls:FMLabel ID="FMLABEL1" Style="z-index: 109; left: 288px; position: absolute; top: 40px" runat="server"
                    BackColor="Transparent" Width="144px" CssClass="formfieldtitle">Transaction Date</FMControls:FMLabel>
                <asp:TextBox ID="OrderNumberTextBox" Style="z-index: 104; left: 16px; position: absolute; top: 56px"
                    runat="server" Width="256px" CssClass="formfield" ReadOnly="True"></asp:TextBox>
                <asp:TextBox ID="TransactionDateTextBox" Style="z-index: 108; left: 288px; position: absolute; top: 56px"
                    runat="server" Width="120px" CssClass="formfield" ReadOnly="True"></asp:TextBox>
                <FMControls:FMButton ID="CloseBtn" Style="z-index: 110; left: 624px; position: absolute; top: 56px" runat="server"
                    Width="98px" Text="Close" OnClick="CloseBtnClick"></FMControls:FMButton>
                <FMControls:FMLabel ID="FMLABEL3" Style="z-index: 112; left: 16px; position: absolute; top: 80px" runat="server"
                    BackColor="Transparent" Width="248px" CssClass="formfieldtitle">PO Number</FMControls:FMLabel>
                <FMControls:FMLabel ID="FMLABEL4" Style="z-index: 113; left: 288px; position: absolute; top: 80px" runat="server"
                    BackColor="Transparent" Width="112px" CssClass="formfieldtitle">Line Number</FMControls:FMLabel>
                <FMControls:FMLabel ID="FMLABEL5" Style="z-index: 114; left: 424px; position: absolute; top: 80px" runat="server"
                    BackColor="Transparent" Width="176px" CssClass="formfieldtitle">Product</FMControls:FMLabel>
                <asp:TextBox ID="CustomerOrderNumberTextBox" Style="z-index: 107; left: 16px; position: absolute; top: 96px"
                    runat="server" Width="256px" CssClass="formfield" ReadOnly="True"></asp:TextBox>
                <asp:TextBox ID="LineNumberTextBox" Style="z-index: 106; left: 288px; position: absolute; top: 96px"
                    runat="server" Width="120px" CssClass="formfield" ReadOnly="True"></asp:TextBox>
                <asp:TextBox ID="ProductTextBox" Style="z-index: 105; left: 424px; position: absolute; top: 96px"
                    runat="server" Width="184px" CssClass="formfield" ReadOnly="True"></asp:TextBox>
                <FMControls:FMLabel ID="FMLABEL2" Style="z-index: 111; left: 16px; position: absolute; top: 128px" runat="server"
                    BackColor="Transparent" Width="248px" CssClass="formfieldtitle">Transactions</FMControls:FMLabel>
                <FMControls:FMBaseDataGrid ID="TransactionDataGrid" Style="z-index: 102; left: 16px; position: absolute; top: 144px"
                    runat="server" BackColor="White" Width="760px" CssClass="tabletext" PageSize="8" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical" BorderStyle="None" AllowPaging="True"
                    AutoGenerateColumns="False">
                    <FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
                    <SelectedItemStyle Font-Bold="True" ForeColor="White" CssClass="tablelink" BackColor="#008A8C"></SelectedItemStyle>
                    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                    <HeaderStyle Font-Bold="True" Wrap="False" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                    <Columns>
                        <asp:TemplateColumn HeaderText="Edit">
                            <HeaderStyle Width="55px"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                            <ItemTemplate>
                                <FMControls:FMEditLinkButton runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                </FMControls:FMBaseDataGrid>
            </div>
        </form>
	</body>
</HTML>
