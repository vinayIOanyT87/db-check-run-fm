<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceRequestPlanSummaryForm.aspx.cs" Inherits="FuelsManager.DispatchWebApp.ServiceRequestPlanSummaryForm" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
	<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
    <div id="pageContent">
        <div id="content" style="position: absolute">
		<asp:Image ID="fadeImage" Style="z-index: 100; left: 0px; top: 0px; position: absolute;"
			runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
        
        <table style="z-index:110; left:32px; top: 10px; width:700px; position:absolute" cellpadding="5">
		    <tr>
		        <td colspan="2">
                    <FMControls:FMLabel id="FMLabel1" runat="server" CssClass="headline" Text="Service Request Plans" style="left:-24px; position:relative" />
		        </td>
		    </tr>
		    <tr>
		        <td>
		            <FMControls:FMButton ID="AddButton1" runat="server" CssClass="formfieldtitle" style="width:100px" Text="Add" />
		        </td>
                <td class="auto-style1">
                </td>
		    </tr>
		    <tr>
                <td colspan="2">
                    <FMControls:FMGridView ID="ServiceRequestPlanGrid" runat="server" FixedHeaders="true" Width="750px" AllowPaging="false" 
                        ShowFooter="true">
                        <Columns>
                            <asp:TemplateField HeaderText="Edit">
                                <HeaderStyle Width="0.5in" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <FMControls:FMEditLinkButton ID="EditButton" OnCommand="PlanGridRowCommand" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Name">
                                <HeaderStyle Width="150px" />
                                <ItemTemplate>
						            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.FilterName") %>' ID="FilterNameLabel"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="True" HeaderText="Description">
                                <HeaderStyle Width="450px"/>
                                <ItemTemplate>
						            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.FilterDescription") %>' ID="FilterDescriptionLabel"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <HeaderStyle Width="0.5in" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <FMControls:FMDeleteLinkButton ID="DeleteButton" OnCommand="PlanGridRowCommand" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
		            </FMControls:FMGridView>
                </td>
            </tr>
            <tr>
			    <td style="WIDTH: 163px; HEIGHT: 36px" valign="middle" width="163">
                    <FMControls:FMButton id="AddButton2" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle" />
				</td>
                <td style="text-align: right" class="auto-style1">
                    <FMControls:FMButton id="FMButton1" runat="server" Width="98px" Text="Close" CssClass="formfieldtitle" />
                </td>
            </tr>
        </table>
    </div>
    </div>
    </form>
</body>
</html>
