<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReadyAccessFilterSummaryForm.aspx.cs" Inherits="FuelsManager.DispatchWebApp.ReadyAccessFilterSummaryForm" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
    <style type="text/css">
        .auto-style1
        {
            width: 456px;
        }
    </style>
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
                    <FMControls:FMLabel id="FMLabel1" runat="server" CssClass="headline" Text="Ready Access Filters for Dispatch" style="left:-24px; position:relative" />
		        </td>
		    </tr>
            <tr>
                <td>
                    <FMControls:FMCheckBox runat="server" ID="ShowActiveOnlyCheckBox" CssClass="formfieldtitle" Text="Show Active Filters Only"/>
                </td>
                <td>
                    <FMControls:FMLabel id="FindLabel" style="Z-INDEX: 111" runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text="Find String:"/>&nbsp;
                    <asp:TextBox id="FindTextBox" style="Z-INDEX: 108" runat="server" CssClass="formfield" Width="250px" tabIndex="2" MaxLength="100"/>&nbsp;
                    <FMCONTROLS:FMBUTTON id="ShowAllBtn" style="Z-INDEX: 110" runat="server" CssClass="formfieldtitle" Width="64px" Text="Show All" tabIndex="4" onclick="FindAllBtn_OnClick"/>&nbsp;
                    <FMCONTROLS:FMBUTTON id="FindBtn" style="Z-INDEX: 109" runat="server" CssClass="formfieldtitle" Text="Find" Width="64px" tabIndex="3" onclick="FindBtn_OnClick"/>
                </td>
            </tr>
		    <tr>
		        <td colspan="2">
		            <FMControls:FMButton ID="AddButton1" runat="server" CssClass="formfieldtitle" style="width:100px" Text="Add" />
                    <FMControls:FMPageSizeDropDown ID="CompanySummaryPageSizeDropDown" runat="server" onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />
		        </td>
		    </tr>
		    <tr>
                <td colspan="2">
                    <FMControls:FMGridView ID="FilterGrid" runat="server" FixedHeaders="true" Width="700px" AllowPaging="false" 
                        ShowFooter="true">
                        <Columns>
                            <asp:TemplateField HeaderText="Edit">
                                <HeaderStyle Width="0.5in" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <FMControls:FMEditLinkButton ID="EditButton" OnCommand="FilterGridRowCommand" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <FMControls:FMDeleteCommandField HeaderText="Delete" DeleteText="Delete Filter" />
                            <asp:TemplateField HeaderText="Name">
                                <HeaderStyle Width="150px" />
                                <ItemTemplate>
						            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.FilterName") %>' ID="FilterNameLabel"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="True" HeaderText="Description">
                                <HeaderStyle Width="500px"/>
                                <ItemTemplate>
						            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.FilterDescription") %>' ID="FilterDescriptionLabel"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Active">
                                <HeaderStyle Width="0.5in" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <FMControls:FMCheckBox runat="server" ID="ActiveCheckBox"/>
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
