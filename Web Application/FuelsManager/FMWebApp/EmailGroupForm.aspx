<%@ Page Language="c#" CodeBehind="EmailGroupForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EmailGroupForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
        <title></title>
        <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
        <meta name="CODE_LANGUAGE" content="C#">
        <meta name="vs_defaultClientScript" content="JavaScript">
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body ms_positioning="GridLayout">
        <form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="EmailGroupTitleLabel"
                    Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" BackColor="Transparent">E-mail Groups Configuration</FMControls:FMLabel>
                <FMControls:FMLabel ID="Label1" Style="z-index: 101; left: 32px; position: absolute; top: 40px" runat="server"
                    CssClass="formfieldtitle" BackColor="Transparent">Group Name:</FMControls:FMLabel>
                <FMControls:FMLabel ID="UserNameRequiredLabel" Style="z-index: 113; left: 192px; position: absolute; top: 40px"
                    runat="server" BackColor="Transparent" Width="8px" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>
                <asp:TextBox ID="Name"
                    Style="z-index: 102; left: 208px; position: absolute; top: 40px" runat="server" aria-required="true"
                    CssClass="formfield" Width="128px" BackColor="White" MaxLength="80"></asp:TextBox>
                <FMControls:FMCheckBox ID="AlwaysEnabledCheckBox" Style="z-index: 115; left: 208px; position: absolute; top: 64px; bottom: 702px; width: 156px;"
                    runat="server" CssClass="formfieldtitle" Text="Always Enabled"
                    AutoPostBack="True" OnCheckedChanged="AlwaysEnabledCheckBoxCheckedChanged"></FMControls:FMCheckBox>
                <FMControls:FMLabel ID="Label7" Style="z-index: 116; left: 32px; position: absolute; top: 96px" runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle">Start Time:</FMControls:FMLabel>
                <FMControls:FMTime ID="StartTime" TimeFormatInfo="<%# FormatInfo %>" Style="z-index: 117; left: 208px; position: absolute; top: 96px"
                    runat="server" CssClass="formfield" Width="150px" AutoPostBack="True"></FMControls:FMTime>
                <FMControls:FMLabel ID="Label8" Style="z-index: 118; left: 32px; position: absolute; top: 128px" runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle">End Time:</FMControls:FMLabel>
                <FMControls:FMTime ID="EndTime" TimeFormatInfo="<%# FormatInfo %>"
                    Style="z-index: 119; left: 208px; position: absolute; top: 128px" runat="server"
                    CssClass="formfield" Width="150px" AutoPostBack="True"></FMControls:FMTime>
                <table id="Table1" style="z-index: 100; left: 375px; width: 375px; position: absolute; top: 40px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
                    <tr>
                        <td style="width: 498px; height: 10px" width="498">
                            <FMControls:FMDataGrid ID="EmailAddressDataGrid" Style="left: 1px; top: 0px" runat="server" CssClass="tabletext" RowHeaderColumn="E-mail Address"
                                AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" Width="360px"
                                GridLines="Vertical" AutoGenerateColumns="False" BackColor="White" BorderStyle="None" PageSize="2">
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
                                        <EditItemTemplate>
                                            <FMControls:FMUpdateLinkButton runat="server" />&nbsp;
                                        <FMControls:FMCancelLinkButton runat="server" />
                                        </EditItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn Visible="False" HeaderText="Index">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="E-mail Address">
                                        <ItemTemplate>
                                            <asp:Label Width="245px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ID="Label2">
                                            </asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox Width="245px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' CssClass="tabletext" ID="IDTextBox" MaxLength="60">
                                            </asp:TextBox>
                                        </EditItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Delete">
                                        <HeaderStyle Width="25px"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        <ItemTemplate>
                                            <FMControls:FMDeleteLinkButton runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                                <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                            </FMControls:FMDataGrid>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 498px; height: 10px" valign="middle" width="498">
                            <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add"
                                CssClass="formfieldtitle"></FMControls:FMButton></td>
                    </tr>
                </table>
                <table id="AssignmentTable" style="z-index: 109; left: 32px; position: absolute; top: 216px; width: 719px; height: 153px;"
                    border="0">
                    <tr>
                        <td>
                            <FMControls:FMLabel ID="Label3" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Assigned Categories:</FMControls:FMLabel>
                        </td>
                        <td>&nbsp;</td>
                        <td>
                            <FMControls:FMLabel ID="Label4" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Unassigned Categories:</FMControls:FMLabel>
                        </td>
                        <td>&nbsp;</td>
                        <td>
                            <FMControls:FMLabel ID="Label9" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Assigned Priorities:</FMControls:FMLabel>
                        </td>
                        <td>&nbsp;</td>
                        <td>
                            <FMControls:FMLabel ID="Label10" runat="server" BackColor="Transparent" CssClass="formfieldtitle">Unassigned Priorities:</FMControls:FMLabel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ListBox ID="AssignedCategoriesListBox" runat="server" Width="136px" Height="126px" SelectionMode="Multiple"
                                BackColor="White" CssClass="formfield"></asp:ListBox>
                        </td>
                        <td>
                            <FMControls:FMButton ID="AssignCategoriesButton" runat="server" Text="<<" style="width:20px;" CssClass="formfieldtitle"></FMControls:FMButton>
                            <div style="height:10px;"></div>
                            <FMControls:FMButton ID="UnassignCategoriesButton" runat="server" Text=">>" style="width:20px;" CssClass="formfieldtitle"></FMControls:FMButton>
                        </td>
                        <td>
                            <asp:ListBox ID="UnassignedCategoriesListBox" runat="server" Width="134px" Height="126px" SelectionMode="Multiple"
                                BackColor="White" CssClass="formfield"></asp:ListBox>
                        </td>
                        <td>
                            <asp:RadioButton ID="AndRadioButton" runat="server" CssClass="formfieldtitle" Text="AND"
                                GroupName="LogicalCombination"></asp:RadioButton>
                            <br />
                            <br />
                            <asp:RadioButton ID="OrRadioButton" runat="server" CssClass="formfieldtitle" Text="OR"
                                GroupName="LogicalCombination"></asp:RadioButton>
                        </td>
                        <td>
                            <asp:ListBox ID="AssignedPrioritiesListBox" runat="server" BackColor="White" CssClass="formfield" Width="134px" SelectionMode="Multiple"
                                Height="126px"></asp:ListBox>
                        </td>
                        <td>
                            <FMControls:FMButton ID="AssignPrioritiesButton" runat="server" CssClass="formfieldtitle" style="width:20px;" Text="<<"></FMControls:FMButton>
                            <div style="height:10px;"></div>
                            <FMControls:FMButton ID="UnassignPrioritiesButton" runat="server" CssClass="formfieldtitle" style="width:20px;" Text=">>"></FMControls:FMButton>
                        </td>
                        <td>
                            <asp:ListBox ID="UnassignedPrioritiesListBox" runat="server" BackColor="White"
                                CssClass="formfield" Width="136px" SelectionMode="Multiple"
                                Height="126px"
                                OnSelectedIndexChanged="UnassignedPrioritiesListBoxSelectedIndexChanged"></asp:ListBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="7">&nbsp;</td>
                    </tr>
                    <tr>
                        <td colspan="7" align="right" valign="middle">
                            <FMControls:FMLabel ID="RequiredLabel" runat="server" CssClass="formfieldtitle" Width="144px" Height="8px"
                                ForeColor="Crimson">* Denotes Required Field</FMControls:FMLabel>
                            &nbsp;&nbsp;
                  <FMControls:FMButton ID="OK" runat="server" Width="98px" Text="OK" CssClass="formfieldtitle"></FMControls:FMButton>
                            &nbsp;&nbsp;
			         <FMControls:FMButton ID="Cancel" runat="server" Text="Cancel" Width="98px" CssClass="formfieldtitle"></FMControls:FMButton>
                            &nbsp;&nbsp;
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </body>
</HTML>
