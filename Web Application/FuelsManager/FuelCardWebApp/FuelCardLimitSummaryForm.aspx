<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FuelCardLimitSummaryForm.aspx.cs" Inherits="FuelsManager.FuelCardWebApp.FuelCardLimitSummaryForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <title></title>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" defaultbutton="FindButton">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent">
            <table style="z-index: 110; left: 15px; top: 115px; width: 300px; position: absolute">
                <tr>
                    <td colspan="2">
                        <FMControls:FMLabel ID="TitleLabel" runat="server" CssClass="headline" Text="Fuel Card Limit Summary" Width="280px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="FindLabel" AssociatedControlID="FindTextBox" runat="server" Text="Find String:" CssClass="formfieldtitle" Width="100px"></FMControls:FMLabel>
                    </td>
                    <td>
                        <FMControls:FMTextBox ID="FindTextBox" runat="server" CssClass="formfield" Width="200px" TabIndex="2" MaxLength="25"></FMControls:FMTextBox>
                    </td>
                    <td>
                        <FMControls:FMButton ID="FindButton" runat="server" Text="Find" CssClass="formfieldtitle" Width="65px" TabIndex="3" OnClick="FindButton_OnClick"></FMControls:FMButton>
                    </td>
                    <td>
                        <FMControls:FMButton ID="ShowAllButton" runat="server" Text="Show All" CssClass="formfieldtitle" Width="65px" TabIndex="4" OnClick="ShowAllButton_OnClick"></FMControls:FMButton>
                    </td>
                </tr>
            </table>
            <table style="z-index: 110; left: 15px; top: 165px; width: 1000px; position: absolute">
                <tr>
                    <td>
                        <FMControls:FMButton ID="AddButtonTop" runat="server" CssClass="formfieldtitle" Text="Add" Width="100px" TabIndex="5" OnClick="AddButtonClick" />
                        <FMControls:FMPageSizeDropDown ID="PageSizeDropDown" ToolTip="Page size" runat="server" TabIndex="6" OnSelectedIndexChanged="PageSizeDropDown_OnSelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMGridView ID="FuelCardLimitsGrid" runat="server" FixedHeaders="false" Width="600px" RowHeaderColumn="ID"
                            AllowPaging="true" PageSize="10" ShowFooter="true" ShowFooterWhenEmpty="true" EmptyDataText="No Fuel Card Limits Found" DataKeyNames="IdentityGuid"
                            OnRowEditing="FuelCardLimitsGrid_OnRowEditing" OnRowCommand="FuelCardLimitsGrid_OnRowCommand" OnRowDataBound="FuelCardLimitsGrid_OnRowDataBound"
                            OnPageIndexChanging="FuelCardLimitsGrid_OnPageIndexChanging" TabIndex="7">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <FMControls:FMLabel ID="EditLabel" Text="Edit" runat="server" />
                                    </HeaderTemplate>
                                    <HeaderStyle Width="30px" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMEditLinkButton EditText="Edit Fuel Card Limit" runat="server" HeaderStyle-HorizontalAlign="Center"
                                            ItemStyle-HorizontalAlign="Center" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <FMControls:FMLabel ID="FuelCardLimitIDHeaderLabel" Text="ID" runat="server" />
                                    </HeaderTemplate>
                                    <HeaderStyle Width="540px" />
                                    <ItemTemplate>
                                        <asp:Label ID="FuelCardLimitIDLabel" CssClass="formFieldTitle" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Delete" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="15px" ItemStyle-Width="15px">
                                    <HeaderStyle Width="30px" />
                                    <ItemTemplate>
                                        <FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" CommandName="Delete" CommandArgument='<%# Container.DataItemIndex %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </FMControls:FMGridView>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMButton ID="AddButton" runat="server" CssClass="formfieldtitle" Text="Add" Width="100px" TabIndex="8" OnClick="AddButtonClick" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
