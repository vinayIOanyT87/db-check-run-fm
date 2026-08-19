<%@ Page language="c#" Codebehind="CompanyHierarchyForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.CompanyHierarchyForm" maintainScrollPositionOnPostback="true"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1" />
		<meta name="CODE_LANGUAGE" Content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
                <script>
                    function saveScroll() {
                        document.getElementById('__SAVESCROLLVERT').value = document.getElementById('HierarchyTreeView').scrollWidth;
                        document.getElementById('__SAVESCROLLHORZ').value = document.getElementById('HierarchyTreeView').scrollTop;
                    }

                    function restoreScroll() {
                        document.getElementById('HierarchyTreeView').scrollWidth = document.getElementById('__SAVESCROLLVERT').value;
                        document.getElementById('HierarchyTreeView').scrollTop = document.getElementById('__SAVESCROLLHORZ').value;


                    }


                    window.onload = restoreScroll;
        </script>
        <style>
            #HierarchyTreeView a img {
                width: 30px;
            }
            #HierarchyTreeView {
                height: calc( 100vh - 250px); 
                border: 1px solid black !important;
                margin-bottom:10px;
            }
            #UnassignedCompanyListBox {
                 height: calc( 100vh - 250px); 
                  margin-bottom:10px;
            }
        </style>
	</HEAD>
	<body tabIndex="-1" MS_POSITIONING="GridLayout">
        <form id="Form1" method="post" runat="server" onsubmit="saveScroll()">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" class="container-fluid">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 101; left: 0px; position: absolute; top: 0px" runat="server"
                    ImageUrl="<%$ AppSettings: PageFadeImage %>" CssClass="formfieldtitle" />
                <div class="row" style="padding-bottom: 10px;">
                    <FMControls:FMLabel ID="Label3" Style="z-index: 102; left: 8px;" runat="server"
                        BackColor="Transparent" Width="344px" CssClass="col-sm-12 headline">Company Hierarchy Configuration</FMControls:FMLabel>
                </div>
                <div class="row" style="padding-bottom: 10px;">
                    <div class="col-sm-11">
                        <FMControls:FMRadioButton ID="LoadingRadioButton" runat="server" Text="Loading" GroupName="Communications"
                            Style="z-index: 107; left: 32px;" CssClass="formfieldtitle" AutoPostBack="True" />
                        <FMControls:FMRadioButton ID="OffLoadingRadioButton" runat="server"
                            Text="Off-Loading" GroupName="Communications"
                            Style="z-index: 107; left: 120px;"
                            CssClass="formfieldtitle" AutoPostBack="True" />
                    </div>
                 </div>
                <div class="row">
                    <div class="col-sm-6" style="padding-left: 25px;">
                        <div>
                            <FMControls:FMLabel ID="CompanyRoleLabel" Style="z-index: 108; left: 32px"
                                runat="server" BackColor="Transparent" CssClass="formfieldtitle">Manager:</FMControls:FMLabel>
                        </div>
                        <FMControls:FMTreeView ID="HierarchyTreeView" Style="overflow: auto; z-index: 101; "
                            runat="server" BackColor="Transparent" Width="100%" CssClass="menu1" AutoPostBack="True"
                            DefaultStyle="font-family:Verdana, Arial, Helvetica, sans-serif;font-size:11px;font-style:normal;line-height:normal;font-weight:bold;font-variant:normal;text-transform:none;color:#000000;text-decoration:none"
                            BorderColor="black" TabIndex="1">
                        </FMControls:FMTreeView>

                    <table style="z-index: 107; left: 32px; ">
                        <tr>
                            <td>
                                <FMControls:FMButton ID="UnassignButton"
                                    runat="server" Width="100px" CssClass="formfieldtitle" Enabled="False" Text="Unassign" TabIndex="2"></FMControls:FMButton></td>
                            <td>&nbsp;</td>
                            <td>
                                <FMControls:FMButton ID="AssignButton"
                                    runat="server" Width="100px" CssClass="formfieldtitle" Enabled="False" Text="Assign" TabIndex="3"></FMControls:FMButton></td>
                        </tr>
                    </table>
                </div>
                <div class="col-sm-5">
                <FMControls:FMLabel ID="FunctionLabel" AssociatedControlID="UnassignedCompanyListBox" Style="z-index: 108; left: 32px; padding-bottom: 8px; "
                    runat="server" BackColor="Transparent" CssClass="formfieldtitle">Unassigned Companies:</FMControls:FMLabel>
                <asp:ListBox ID="UnassignedCompanyListBox" Style="z-index: 103; left: 32px;"
                    runat="server" Width="100%" CssClass="formfield" SelectionMode="Multiple" TabIndex="4"></asp:ListBox>
                <table style="z-index: 102; left: 32px; width: 100%; height: 10px"
                    cellspacing="0" cellpadding="1" width="324" border="0">
                    <tr>
                        <td style="height: 10px">
                            <FMControls:FMDataGrid ID="LoadIDDataGrid" Style="left: 1px; top: 408px" runat="server" BackColor="White" RowHeaderColumn="Load ID"
                                Width="100%" CssClass="tabletext" PageSize="5" AutoGenerateColumns="False" AllowPaging="True" CellPadding="3" BorderColor="White"
                                AllowSorting="True" BorderWidth="1px" GridLines="Vertical" BorderStyle="None">
                                <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
                                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                                <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
                                <Columns>
                                    <asp:TemplateColumn HeaderText="Edit">
                                        <HeaderStyle Width="35px"></HeaderStyle>
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
                                    <asp:TemplateColumn HeaderText="Load ID">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Width="1in" Text='<%# DataBinder.Eval(Container, "DataItem.LoadID") %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="LoadIDTextBox" runat="server" Width="1in" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.LoadID") %>' MaxLength="30">
                                            </asp:TextBox>
                                        </EditItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Driver">
                                        <ItemTemplate>
                                            <asp:Label Width="1in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.PersonID") %>' ID="Label1">
                                            </asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:DropDownList Width="1in" CssClass="tabletext" runat="server" Enabled="True" ID="DriverDropDownList" DataSource="<%# EnumerateDrivers()%>" DataTextField="Text" DataValueField="Value">
                                            </asp:DropDownList>
                                        </EditItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Delete">
                                        <HeaderStyle Width="25px"></HeaderStyle>
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
                        <td>
                            <FMControls:FMButton ID="AddButton" runat="server" Width="80px" CssClass="formfieldtitle" Text="Add"
                                TabIndex="5"></FMControls:FMButton></td>
                    </tr>
                </table>
                </div>                                        
                <input id="__SAVESCROLLVERT" name="__SAVESCROLLVERT" value="0" type="hidden" runat="server" />
                <input id="__SAVESCROLLHORZ" name="__SAVESCROLLHORZ" value="0" type="hidden" runat="server" />
            </div>
            </div>
        </form>

        <script>
            
                    $(document).ready(function () {
                        var data = HierarchyTreeView_Data;

                        if ((typeof(data.selectedClass) != "undefined") && (data.selectedClass != null)) 
                        {
	                        var id = data.selectedNodeID.value;
	                        if (id.length > 0) 
	                        {
		                        var selectedNode = document.getElementById(id);
		                        if ((typeof(selectedNode) != "undefined") && (selectedNode != null))
		                        {
			                           selectedNode.scrollIntoView(true)
		                        }
	                        }

                        }
                    });
        </script>
    </body>
</HTML>
