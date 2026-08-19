<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LedgerAggregateColumnForm.aspx.cs" Inherits="FuelsManager.Accounting.LedgerAggregateColumnForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
</head>
<body>
    <form id="form2" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
		<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
        <table style="z-index:110; left:32px; top: 10px; width:650px; position:absolute" cellpadding="5" role="presentation" aria-label="layout">
            <tr>
                <td colspan="3">
                    <FMControls:FMLabel id="TitleLabel" runat="server" CssClass="headline" Text="Ledger Aggregate Column" style="left:-24px; position:relative" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel id="NameLabel" AssociatedControlID="NameTextBox" runat="server" CssClass="formfieldtitle" Text="Column Name" />
                    &nbsp;
                    <FMControls:FMLabel ID="required" runat="server" CssClass="formfieldtitle" ForeColor="Red" Text="*" />
                </td>
                <td colspan="2">
                    <asp:TextBox ID="NameTextBox" runat="server" style="width:250px" aria-required="true" />
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel id="FieldLabel" AssociatedControlID="FieldList" runat="server" CssClass="formfieldtitle" Text="Aggregate Field" />
                </td>
                <td>
                    <FMControls:FMDropDownList id="FieldList" runat="server" CssClass="formfield" AutoPostBack="true">
                        <asp:ListItem Text="Net & Gross" Value="NetGross" />
                        <asp:ListItem Text="Number01" Value="Number01" />
                        <asp:ListItem Text="Number02" Value="Number02" />
                        <asp:ListItem Text="Number03" Value="Number03" />
                        <asp:ListItem Text="Number04" Value="Number04" />
                        <asp:ListItem Text="Number05" Value="Number05" />
                        <asp:ListItem Text="Number06" Value="Number06" />
                        <asp:ListItem Text="Custom Function" Value="CustomFunction" />
                    </FMControls:FMDropDownList>
                </td>
                <td>
                    <table role="presentation" aria-label="layout">
                        <tr>
                            <td>
                                <FMControls:FMLabel runat="server" AssociatedControlID="CustomFunctionTextBox" ID="CustomFunctionLabel" CssClass="formfieldtitle" Text="Function Name:"/>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox runat="server" ID="CustomFunctionTextBox" CssClass="formfield" Width="275px"/>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <FMControls:FMButton id="AddButton" runat="server" Text="Add" style="width:75px" CssClass="formfieldtitle" />
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <FMControls:FMGridView ID="AliasGrid" runat="server" FixedHeaders="false" Width="100%" RowHeaderColumn="Alias" 
                        AllowPaging="false" ShowFooter="true" ShowFooterWhenEmpty="true" EmptyDataText="No aliases selected" aria-label="Aliases">
                        <Columns>
                            <FMControls:FMEditCommandField EditText="Edit Column Definition" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                            <asp:TemplateField HeaderText="Alias" HeaderStyle-HorizontalAlign="Left">
                                <HeaderStyle Width="225px" />
                                <ItemTemplate>
						            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.AliasName") %>' ID="AliasNameLabel"/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <FMControls:FMDropDownList ID="AliasDropDown" ToolTip="Alias" runat="server" Width="300px" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Symbol" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                <HeaderStyle Width="50px" />
                                <ItemTemplate>
						            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Symbol") %>' ID="SymbolNameLabel"/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="SymbolTextBox" ToolTip="Symbol" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Symbol") %>' MaxLength="1" Width="50px" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <HeaderStyle Width="25px" />
                                <ItemTemplate>
                                    <FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" align="center" style="display:block;" CommandName="Delete" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
		            </FMControls:FMGridView>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMButton id="AddButton2" runat="server" Text="Add" style="width:75px" CssClass="formfieldtitle" />
                </td>
                <td>
                    <FMControls:FMLabel ID="denotes" runat="server" ForeColor="Red" CssClass="formfieldtitle" Text="* Denotes Required Field" />
                </td>
                <td align="right">
                    <FMControls:FMButton id="NewButton" runat="server" Text="New" CssClass="formfieldtitle" Width="65px" />
                    &nbsp;&nbsp;
                    <FMControls:FMButton id="OKButton" runat="server" Text="OK" CssClass="formfieldtitle" Width="65px" />
                    &nbsp;&nbsp;
                    <FMControls:FMButton id="CancelButton" runat="server" Text="Cancel" CssClass="formfieldtitle" Width="65px" />
                </td>
            </tr>
        </table>
        </div>
    </form>
</body>
</html>
