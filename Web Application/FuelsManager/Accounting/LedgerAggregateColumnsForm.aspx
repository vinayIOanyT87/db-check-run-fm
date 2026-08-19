<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LedgerAggregateColumnsForm.aspx.cs" Inherits="FuelsManager.Accounting.LedgerAggregateColumnsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1">
    <title></title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
	<meta name="CODE_LANGUAGE" content="C#" />
	<meta name="vs_defaultClientScript" content="JavaScript" />
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
</head>
<body>
    <form id="form1" method="post" runat="server" DefaultButton="FindButton">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
        <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
        <table style="z-index:110; left:32px; top: 10px; width:575px; position:absolute" cellpadding="5" role="presentation" aria-label="layout">
            <tr>
                <td colspan="2">
                    <FMControls:FMLabel id="TitleLabel" runat="server" CssClass="headline" Text="Ledger Aggregate Columns" style="left:-24px; position:relative" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMButton id="AddButton" runat="server" Text="Add" style="width:75px" CssClass="formfieldtitle" />
                    &nbsp;&nbsp;
                    <FMControls:FMPageSizeDropDown ToolTip="Page Size" ID="PageSizeDropDown" runat="server" AutoPostBack="true" />
                </td>
                <td align="right">
                    <asp:TextBox ID="FindText" ToolTip="Find" runat="server" style="width:200px" CssClass="formfield" MaxLength="100"/>
                    &nbsp;
                    <FMControls:FMButton ID="FindButton" runat="server" Text="Find" CssClass="formfieldtitle" style="width:75px" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <FMControls:FMGridView ID="AggregateGrid" runat="server" FixedHeaders="false" Width="700px" RowHeaderColumn="Name"
					PagerStyle-CssClass="pgr" aria-label="Aggregate">
                        <Columns>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="IdentityGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Edit">
                                <HeaderStyle Width="0.5in" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <FMControls:FMEditLinkButton OnCommand="AggregateGrid_RowCommand" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Name">
                                <HeaderStyle Width="400px" />
                                <ItemTemplate>
						            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ID="ColumnNameLabel"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <HeaderStyle Width="25px" />
                                <ItemTemplate>
                                    <FMControls:FMDeleteLinkButton runat="server" CommandName="Delete" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
		            </FMControls:FMGridView>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <FMControls:FMButton id="AddButton2" runat="server" Text="Add" style="width:75px" CssClass="formfieldtitle" />
                </td>
            </tr>
        </table>
    </div>
</form>
</body>
</html>
