<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LedgerViewForm.aspx.cs" Inherits="FuelsManager.Accounting.LedgerViewForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
</head>
<body>
    <form id="form1" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
                <table style="z-index:110; left:32px; top: 10px; width:500px; position:absolute" cellpadding="5" role="presentation" aria-label="layout">
                    <tr>
                        <td colspan="4">
                            <FMControls:FMLabel id="TitleLabel" runat="server" CssClass="headline" Text="Ledger View" style="left:-24px; position:relative" />
                        </td>
                    </tr>
                    <tr>
                        <td style="white-space:nowrap">
                            <FMControls:FMLabel id="NameLabel" AssociatedControlID="NameTextBox" runat="server" CssClass="formfieldtitle" Text="Name" />
                            &nbsp;
                            <FMControls:FMLabel ID="required" runat="server" CssClass="formfieldtitle" ForeColor="Red" Text="*" />
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="NameTextBox" runat="server" style="width:250px" CssClass="formfield" aria-required="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td class="formfieldtitle" style="width:2in" colspan="2">
                            <FMControls:FMLabel ID="SelectedFieldsLabel" AssociatedControlID="SelectedFieldsList" runat="server" CssClass="formfieldtitle" Text="Assigned Fields:" />
                        </td>
                        <td class="formfieldtitle" style="width:2in">
                            <FMControls:FMLabel ID="AvailableFieldsLabel" AssociatedControlID="AvailableFieldsList" runat="server" CssClass="formfieldtitle" Text="Unassigned Fields:" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="right">
                            <FMControls:FMUpLinkButton ID="MoveUpButton" runat="server"/>
                        </td>
                        <td rowspan="2">
                            <FMControls:FMListBox ID="SelectedFieldsList" runat="server" CssClass="formfield" Width="175px" Height="157px" Sort="false" SelectionMode="Multiple"/>
                        </td>
                        <td valign="middle">
                            <FMControls:FMButton ID="AssignFieldButton" runat="server" CssClass="formfieldtitle" style="width:20px;" Text="<<" />
                        </td>
                        <td rowspan="2">
                            <FMControls:FMListBox ID="AvailableFieldsList" runat="server" CssClass="formfield" Width="175px" Height="157px" SelectionMode="Multiple" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="bottom" align="right">
                            <FMControls:FMDownLinkButton ID="MoveDownButton" runat="server" />
                        </td>
                        <td valign="top">
                            <FMControls:FMButton ID="RemoveFieldButton" runat="server" CssClass="formfieldtitle" style="width:20px;" Text=">>"/>
                        </td>
                    </tr>
                    <tr><td colspan="4"><hr style="width:100%; color:Black; size:1pt"/></td></tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td class="formfieldtitle" style="width:2in" colspan="2">
                            <FMControls:FMLabel ID="SelectedProductsLabel" AssociatedControlID="SelectedProductsList" runat="server" CssClass="formfieldtitle" Text="Assigned Products:" />
                        </td>
                        <td class="formfieldtitle" style="width:2in">
                            <FMControls:FMLabel ID="AvailableProductsLabel" AssociatedControlID="AvailableProductsList" runat="server" CssClass="formfieldtitle" Text="Unassigned Products:" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="right">
                            &nbsp;
                        </td>
                        <td rowspan="2">
                            <FMControls:FMListBox ID="SelectedProductsList" runat="server" CssClass="formfield" Width="175px" Height="100px" Sort="false" SelectionMode="Multiple"/>
                        </td>
                        <td valign="middle">
                            <FMControls:FMButton ID="AssignProductButton" runat="server" CssClass="formfieldtitle" style="width:20px;" Text="<<" />
                        </td>
                        <td rowspan="2">
                            <FMControls:FMListBox ID="AvailableProductsList" runat="server" CssClass="formfield" Width="175px" Height="100px" SelectionMode="Multiple" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="bottom" align="right">
                            &nbsp;
                        </td>
                        <td valign="top">
                            <FMControls:FMButton ID="RemoveProductButton" runat="server" CssClass="formfieldtitle" style="width:20px;" Text=">>"/>
                        </td>
                    </tr>
                    <tr><td colspan="4"><hr style="width:100%; color:Black; size:1pt"/></td></tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td class="formfieldtitle" style="width:2in" colspan="2">
                            <FMControls:FMLabel ID="SelectedGroupsLabel" AssociatedControlID="SelectedGroupsList" runat="server" CssClass="formfieldtitle" Text="Assigned User Groups:" />
                        </td>
                        <td class="formfieldtitle" style="width:2in">
                            <FMControls:FMLabel ID="AvailableGroupsLabel" AssociatedControlID="AvailableGroupsList" runat="server" CssClass="formfieldtitle" Text="Unassigned User Groups:" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="right">
                            &nbsp;
                        </td>
                        <td rowspan="2">
                            <FMControls:FMListBox ID="SelectedGroupsList" runat="server" CssClass="formfield" Width="175px" Height="100px" Sort="false" SelectionMode="Multiple"/>
                        </td>
                        <td valign="middle">
                            <FMControls:FMButton ID="AssignGroupButton" runat="server" CssClass="formfieldtitle" style="width:20px;" Text="<<" />
                        </td>
                        <td rowspan="2">
                            <FMControls:FMListBox ID="AvailableGroupsList" runat="server" CssClass="formfield" Width="175px" Height="100px" SelectionMode="Multiple" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="bottom" align="right">
                            &nbsp;
                        </td>
                        <td valign="top">
                            <FMControls:FMButton ID="RemoveGroupButton" runat="server" CssClass="formfieldtitle" style="width:20px;" Text=">>"/>
                        </td>
                    </tr>
                    <tr><td colspan="4"><hr style="width:100%; color:Black; size:1pt"/></td></tr>
                    <tr>
                        <td colspan="2">
                            <FMControls:FMLabel ID="denotes" runat="server" ForeColor="Red" CssClass="formfieldtitle" Text="* Denotes Required Field" />
                        </td>
                        <td colspan="2" align="right" style="height:35px" style="white-space:nowrap">
                            <FMControls:FMButton id="NewButton" runat="server" Text="New" CssClass="formfieldtitle" Width="65px" />
                            &nbsp;&nbsp;
                            <FMControls:FMButton id="OKButton" runat="server" Text="OK" CssClass="formfieldtitle" Width="65px" />
                            &nbsp;&nbsp;
                            <FMControls:FMButton id="CancelButton" runat="server" Text="Cancel" CssClass="formfieldtitle" Width="65px" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</form>
</body>
</html>
