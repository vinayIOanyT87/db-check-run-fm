<%@ Page Language="c#" CodeBehind="AdditiveProfileForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.AdditiveProfileForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
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
	<body MS_POSITIONING="GridLayout" >
        <form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                    ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
                <FMControls:FMLabel ID="AdditiveProfileTitleLabel"
                    Style="z-index: 101; left: 8px; position: absolute; top: 8px; width: 500px;" runat="server"
                    BackColor="Transparent" CssClass="headline">Additive Profile Configuration</FMControls:FMLabel>
                <FMControls:FMLabel ID="Label1" AssociatedControlID="AdditiveProfileIDTextbox" Style="z-index: 103; left: 32px; position: absolute; top: 40px" runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle">ID:</FMControls:FMLabel>
                <FMControls:FMLabel ID="Label8" Style="z-index: 105; left: 80px; position: absolute; top: 40px" runat="server"
                    BackColor="Transparent" Width="8px" ForeColor="Crimson" Height="8px">*</FMControls:FMLabel>
                <asp:TextBox ID="AdditiveProfileIDTextbox" Style="z-index: 104; left: 107px; position: absolute; top: 40px; width: 147px;" aria-required="true"
                    runat="server" BackColor="White" CssClass="formfield" MaxLength="30" ></asp:TextBox>
                <FMControls:FMLabel ID="FMLabel2" AssociatedControlID="AdditiveProfileDescriptionTextbox"
                    Style="z-index: 103; left: 32px; position: absolute; top: 71px" runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle">Description:</FMControls:FMLabel>
                <asp:TextBox ID="AdditiveProfileDescriptionTextbox" Style="z-index: 104; left: 107px; position: absolute; top: 70px; width: 226px;"
                    runat="server" BackColor="White" CssClass="formfield" MaxLength="50" ></asp:TextBox>
                <FMControls:FMLabel ID="FMLabel1"
                    Style="z-index: 110; left: 32px; position: absolute; top: 99px" runat="server"
                    BackColor="Transparent" CssClass="formfieldtitle"> Additives:</FMControls:FMLabel>

                <table id="Table1" style="z-index: 109; left: 32px; width: 238px; position: absolute; top: 120px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
                    <tr>
                        <td style="width: 422px; height: 10px">
                            <FMControls:FMDataGrid ID="AdditivesDataGrid" runat="server" RowHeaderColumn="ID"
                                CssClass="tabletext" BackColor="White" Width="597px"
                                BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px"
                                AllowSorting="True" BorderColor="White" CellPadding="3"
                                AllowPaging="True" PageSize="3" >
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
                                            <FMControls:FMEditLinkButton ID="EditButton" runat="server" />
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
                                    <asp:TemplateColumn HeaderText="ID">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ID="Label2">
                                            </asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:DropDownList CssClass="tabletext" runat="server" Enabled="True" ID="AdditivesDropDownList" ToolTip="Additive" DataSource="<%# EnumerateAdditiveProducts()%>" DataTextField="Text" DataValueField="Value">
                                            </asp:DropDownList>
                                        </EditItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Rate">
                                        <ItemTemplate>
                                            <asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Rate") %>' ID="Label3">
                                            </asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox Width=".5in" CssClass="tabletext" ID="RateTextBox" ToolTip="Rate" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Rate") %>'>
                                            </asp:TextBox>
                                        </EditItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Cycle Volume">
                                        <ItemTemplate>
                                            <FMControls:FMLabel Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CycleVolume") %>' ID="Label4">
                                            </FMControls:FMLabel>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox Width=".5in" CssClass="tabletext" ID="CycleVolumeTextbox" ToolTip="Cycle volume" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CycleVolume") %>'>
                                            </asp:TextBox>
                                        </EditItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Treat Rate">
                                        <ItemTemplate>
                                            <FMControls:FMLabel Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.TreatRate") %>' ID="Label5">
                                            </FMControls:FMLabel>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Desired Treat Rate">
                                        <ItemTemplate>
                                            <FMControls:FMLabel Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DesiredTreatRate") %>' ID="Label6">
                                            </FMControls:FMLabel>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox Width=".5in" CssClass="tabletext" ID="DesiredTreatRateTextBox" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DesiredTreatRate") %>'>
                                            </asp:TextBox>
                                        </EditItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Tolerance">
                                        <ItemTemplate>
                                            <FMControls:FMLabel Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tolerance") %>' ID="Label7">
                                            </FMControls:FMLabel>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox Width=".5in" CssClass="tabletext" ID="ToleranceTextBox" ToolTip="Tolerance" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tolerance") %>'>
                                            </asp:TextBox>
                                        </EditItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Delete">
                                        <HeaderStyle Width="0.5in"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        <ItemTemplate>
                                            <FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                                <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
                            </FMControls:FMDataGrid></td>
                    </tr>
                    <tr>
                        <td style="width: 422px; height: 10px">
                            <FMControls:FMButton ID="AddButton" Width="67px" runat="server" CssClass="formfieldtitle" Text="Add" ></FMControls:FMButton>
                        </td>
                    </tr>
                </table>
                <FMControls:FMButton ID="OK"
                    Style="z-index: 106; left: 481px; position: absolute; top: 288px" runat="server"
                    Width="67px" CssClass="formfieldtitle" Text="OK" ></FMControls:FMButton>
                <FMControls:FMButton ID="Cancel"
                    Style="z-index: 107; left: 573px; position: absolute; top: 288px" runat="server"
                    Width="67px" CssClass="formfieldtitle" Text="Cancel" ></FMControls:FMButton>
                <FMControls:FMLabel ID="Label10"
                    Style="z-index: 108; left: 485px; position: absolute; top: 320px" runat="server"
                    Width="144px" CssClass="formfieldtitle" ForeColor="Crimson" Height="8px">* Denotes Required Field</FMControls:FMLabel>

                <script>
                    var okButton = document.getElementById("OK");
                    if (!okButton.disabled)
                        okButton.setActive();
                </script>
            </div>
        </form>
	</body>
</HTML>
