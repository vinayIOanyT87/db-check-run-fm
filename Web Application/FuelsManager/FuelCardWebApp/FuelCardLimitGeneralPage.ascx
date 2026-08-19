<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FuelCardLimitGeneralPage.ascx.cs" Inherits="FuelsManager.FuelCardWebApp.FuelCardLimitGeneralPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<!DOCTYPE html>

<html>
<head>
    <title></title>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>

    <table style="z-index: 110; left: 0; top: 0; width: 600px; position: absolute">
        <tr>
            <td>
                <FMControls:FMLabel ID="IDLabel" AssociatedControlID="IDTextBox" runat="server" CssClass="formfieldtitle" Text="ID:" Width="15px" /><span style="color: red; width: 3px; display: inline">*</span>
            </td>
            <td>
                <FMControls:FMTextBox ID="IDTextBox" CssClass="formfield" runat="server" Width="400px" MaxLength="50" aria-required="true" TabIndex="6"/>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <FMControls:FMLabel ID="LineItemsLabel" runat="server"
                    CssClass="formfieldtitle" Text="Line Items" Width="200px"
                    Style="left: 0px; position: relative" Font-Italic="True" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <FMControls:FMGridView ID="LineItemsGrid" runat="server" FixedHeaders="false" Width="700px" RowHeaderColumn="ID"
                    AllowPaging="true" PageSize="10" ShowFooter="true" ShowFooterWhenEmpty="true" EmptyDataText="No Line Items found" DataKeyNames="IdentityGuid"
                    OnRowUpdating="LineItemsGridRowUpdating" OnRowEditing="LineItemsGridRowEditing" OnRowCancelingEdit="LineItemsGridRowCancelingEdit"
                    OnRowDataBound="LineItemsGridRowDataBound" OnRowCommand="LineItemsGridRowCommand" OnPageIndexChanging="LineItemsGrid_OnPageIndexChanging" TabIndex="7">
                    <Columns>
                        <FMControls:FMEditCommandField EditText="Edit Line Item" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="70px" ItemStyle-Width="70px" />
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <FMControls:FMLabel ID="TypeHeaderLabel" Text="Type" runat="server" />
                                <span style="color: red">*</span>
                            </HeaderTemplate>
                            <HeaderStyle Width="110px" />
                            <ItemTemplate>
                                <FMControls:FMLabel ID="TypeLabel" Text='<%# DataBinder.Eval(Container, "DataItem.UserFriendlyLineItemType") %>' runat="server" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <FMControls:FMDropDownList ID="TypeDropDownList" runat="server" DataSource="<%#EnumerateLineItemTypes()%>" DataTextField="Value" DataValueField="Key" OnSelectedIndexChanged="TypeDropDownList_OnSelectedIndexChanged" AutoPostBack="True" aria-required="true">
                                </FMControls:FMDropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <FMControls:FMLabel ID="ProductOrGroupIDHeaderLabel" Text="ID" runat="server" />
                            </HeaderTemplate>
                            <HeaderStyle Width="110px" />
                            <ItemTemplate>
                                <FMControls:FMLabel ID="ProductOrGroupIDLabel" Text='<%# DataBinder.Eval(Container, "DataItem.AssignedProductGroupOrProductID") %>' runat="server" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <FMControls:FMDropDownList ID="ProductDropDownList" runat="server" DataTextField="ID" DataValueField="MasterRecordGuid" DataSource="<%#EnumerateProducts()%>" Visible="true" />
                                <FMControls:FMDropDownList ID="ProductGroupDropDownList" runat="server" DataTextField="ID" DataValueField="IdentityGuid" DataSource="<%#EnumerateProductGroups()%>" Visible="false" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <FMControls:FMLabel ID="LimitHeaderLabel" Text="Limit" runat="server" />
                                <span style="color: red">*</span>
                            </HeaderTemplate>
                            <HeaderStyle Width="75px" />
                            <ItemTemplate>
                                <FMControls:FMLabel ID="LimitLabel" Text='<%# DataBinder.Eval(Container, "DataItem.Limit") %>' runat="server" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <FMControls:FMTextBox ID="LimitTextBox" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Limit") %>' MaxLength="10" aria-required="true"/>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <FMControls:FMLabel ID="PeriodHeaderLabel" Text="Period" runat="server" />
                                <span style="color: red">*</span>
                            </HeaderTemplate>
                            <HeaderStyle Width="85px" />
                            <ItemTemplate>
                                <FMControls:FMLabel ID="PeriodLabel" Text='<%# DataBinder.Eval(Container, "DataItem.Period") %>' runat="server" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <FMControls:FMDropDownList ID="PeriodDropDownList" runat="server" DataSource="<%#EnumeratePeriods()%>" DataTextField="Value" DataValueField="Key" aria-required="true"/>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Delete" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="15px" ItemStyle-Width="15px">
                            <HeaderStyle Width="25px" />
                            <ItemTemplate>
                                <FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" CommandName="Delete" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </FMControls:FMGridView>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <FMControls:FMButton ID="AddButton" runat="server" CssClass="formfieldtitle" Text="Add" Width="100px" TabIndex="8" OnClick="AddButtonClick" />

            </td>
        </tr>
    </table>
</body>
</html>
