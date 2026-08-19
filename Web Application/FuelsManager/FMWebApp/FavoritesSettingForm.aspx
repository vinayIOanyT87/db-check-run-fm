<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FavoritesSettingForm.aspx.cs" Inherits="FuelsManager.FMWebApp.FavoritesSettingForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html>
    <head runat="server">
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
    </head>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
        <form id="FavoritesForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <FMControls:FMLabel ID="lblTitle" Style="z-index: 103; left: 8px; position: absolute;
                    top: 8px" runat="server" CssClass="headline" Width="312px" BackColor="Transparent">Favorites Configuration</FMControls:FMLabel>
                <FMControls:FMDataGrid ID="dgFavorites" runat="server" CssClass="tabletext" RowHeaderColumn="Menu Item"
                    BackColor="White" Width="784px" BorderStyle="None" AutoGenerateColumns="False"
                    GridLines="Vertical" BorderWidth="1px" AllowSorting="False" BorderColor="White"
                    CellPadding="3" AllowPaging="True" PageSize="10" style="position: absolute;top:40px; left:32px" aria-label="Favorites">
                    <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                    <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>">
                    </HeaderStyle>
                    <Columns>
					    <asp:TemplateColumn HeaderText="Edit">
						    <HeaderStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
						    <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						    <ItemTemplate>
							    <FMControls:FMEditLinkButton runat="server" id="btnEdit" CommandName="Edit" />
						    </ItemTemplate>
						    <EditItemTemplate>
							    <FMControls:FMUpdateLinkButton runat="server" id="btnConfirm" CommandName="Update" />&nbsp;
							    <FMControls:FMCancelLinkButton runat="server" id="btnCancelLineItem" CommandName="Cancel"  />
						    </EditItemTemplate>
					    </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Menu Item">
                            <HeaderStyle Width="400px" Wrap="False"></HeaderStyle>
                            <ItemStyle Width="400px" Wrap="False"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label runat="server"
                                    ID="lblDisplayPath">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
					    <asp:TemplateColumn HeaderText="Custom Name">
                            <HeaderStyle Width="150px" />
						    <ItemStyle Width="150px" HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False"></ItemStyle>
						    <ItemTemplate>
							    <%# DataBinder.Eval(Container.DataItem, "CustomName") %>
						    </ItemTemplate>
						    <EditItemTemplate>
							    <asp:TextBox ID="txtCustomName" ToolTip="Custom Name" Runat="server" MaxLength="40" Text='<%# DataBinder.Eval(Container.DataItem, "CustomName") %>' CssClass="tabletext"/>
						    </EditItemTemplate>
					    </asp:TemplateColumn>
					    <asp:TemplateColumn HeaderText="Move">
						    <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
						    <ItemStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						    <ItemTemplate>
                                <asp:button runat="server" id="btnMoveUp" CommandName="MoveUp" CssClass="formfieldtitle" Text="Up" />
							    <asp:button runat="server" id="btnMoveDown" CommandName="MoveDown" CssClass="formfieldtitle" Text="Down" />
						    </ItemTemplate>
					    </asp:TemplateColumn>
					    <asp:TemplateColumn HeaderText="Delete">
						    <HeaderStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
						    <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						    <ItemTemplate>
							    <FMControls:FMDeleteLinkButton runat="server" ID="btnDelete" NAME="btnDelete" />
						    </ItemTemplate>
						    <EditItemTemplate>
							    <FMControls:FMDeleteLinkButton runat="server" ID="btnDeleteEdit" NAME="btnDeleteEdit" enabled="false" />
						    </EditItemTemplate>
					    </asp:TemplateColumn>
                    </Columns>
                    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>"
                        Mode="NumericPages"></PagerStyle>
                </FMControls:FMDataGrid>
                <div style="position:absolute;top:455px;width:785px;left:32px">
                    <div style="float:right">
                        <asp:button id="btnOK" CssClass="formfieldtitle" Width="67px" Runat="server" Text="OK" onclick="BtnOkClick"></asp:button>&nbsp;
                        <asp:Button ID="btnCancel" Runat="server" Text="Cancel" Width="67px" CssClass="formfieldtitle" onclick="BtnCancelClick" />
                    </div>
                </div>
            </div>
		</form>
	</body>
</html>
