<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FMAETranslationsForm.aspx.cs" Inherits="FuelsManager.FMWebApp.FMAETranslationsForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <title></title>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<script>
		function EnableButtons() {
			$("input").removeAttr("disabled");
		}
	</script>
</head>
<body>
    <form id="form1" runat="server" DefaultButton="FindButton">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent">
            <table style="z-index: 110; left: 15px; top: 115px; width: 300px; position: absolute">
                <tr>
                    <td colspan="2">
                        <FMControls:FMLabel ID="TitleLabel" runat="server" CssClass="headline" Text="FMAE Translation Configuration" Width="280px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="EntityTypeLabel" AssociatedControlID="EntityTypeDropDownList" runat="server" Text="Entity Type:" CssClass="formfieldtitle" Width="75px"></FMControls:FMLabel>
                    </td>
                    <td>
                        <FMControls:FMDropDownList ID="EntityTypeDropDownList" ToolTip="Enterprise Entity" runat="server" CssClass="formfield" DataSource="<%#EnumerateTranslationTypes()%>" AutoPostBack="true" TabIndex="1" OnSelectedIndexChanged="EntityTypeDropDownList_SelectedIndexChanged" Width="200px"></FMControls:FMDropDownList>
                    </td>
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
                        <FMControls:FMButton ID="AddButtonTop" runat="server" CssClass="formfieldtitle" Text="Add" Width="100px" TabIndex="5" OnClick="AddButton_Click" />
                        <FMControls:FMPageSizeDropDown ID="PageSizeDropDown" ToolTip="Page size" runat="server" TabIndex="6" OnSelectedIndexChanged="PageSizeDropDown_OnSelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMGridView ID="TranslationsGrid" runat="server" FixedHeaders="false" Width="600px" RowHeaderColumn="FMAE ID"
                            AllowPaging="true" PageSize="10" ShowFooter="true" ShowFooterWhenEmpty="true" EmptyDataText="No translations found" DataKeyNames="IdentityGuid"
                            OnRowUpdating="TranslationsGrid_RowUpdating" OnRowEditing="TranslationsGrid_RowEditing" OnRowCancelingEdit="TranslationsGrid_RowCancelingEdit" 
                            OnRowDataBound="TranslationsGrid_RowDataBound" OnRowCommand="TranslationsGrid_RowCommand" OnPageIndexChanging="TranslationsGrid_OnPageIndexChanging" TabIndex="7">
                            <Columns>
                                <FMControls:FMEditCommandField EditText="Edit Translation" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="70px" ItemStyle-Width="70px" />
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <FMControls:FMLabel ID="FMAEIDHeaderLabel" Text="FMAE ID" runat="server" />
                                        <span style="COLOR: red">*</span>
                                    </HeaderTemplate>
                                    <HeaderStyle Width="110px" />
                                    <ItemTemplate>
                                        <FMControls:FMLabel ID="FMAEIDLabel" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' runat="server" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <FMControls:FMTextBox ID="FMAEIDTextBox" ToolTip="FMAE ID" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' runat="server" aria-required="true" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <FMControls:FMLabel ID="EnterpriseEntityHeaderLabel" Text="Enterprise Entity" runat="server" />
                                        <span style="COLOR: red">*</span>
                                    </HeaderTemplate>
                                    <HeaderStyle Width="110px" />
                                    <ItemTemplate>
                                        <FMControls:FMLabel ID="EnterpriseEntityLabel" Text='<%# DataBinder.Eval(Container, "DataItem.EntityID") %>' runat="server" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <FMControls:FMDropDownList ID="EnterpriseEntityDropDownList" ToolTip="Enterprise Entity select" runat="server" aria-required="true" DataTextField="ID" DataValueField="MasterRecordGuid" DataSource="<%#EnumerateEntities()%>" />
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
                    <td>
                        <FMControls:FMButton ID="AddButton" runat="server" CssClass="formfieldtitle" Text="Add" Width="100px" TabIndex="8" OnClick="AddButton_Click" />
                        <asp:FileUpload ID="UploadControl" ToolTip="File upload" runat="server" CssClass="formfieldtitle" style="width: 394px; height: 22px; vertical-align: top "/>
                        <FMControls:FMButton ID="ImportButton" runat="server" CssClass="formfieldtitle" Text="Import" Width="80px" TabIndex="8" OnClick="ImportButton_OnClick" />
                        <FMControls:FMButton ID="ExportButton" runat="server" CssClass="formfieldtitle" Text="Export" Width="80px" TabIndex="8" OnClick="ExportButton_OnClick" OnClientClick="CheckDownloadComplete(EnableButtons);" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="ImportResultsLabel" runat="server" CssClass="formfieldtitle" Text="Import Results:" style="vertical-align: top;" Visible="false"/>                    
                        <FMControls:FMTextBox ID="ImportResultsTextBox" runat="server" CssClass="formfield" TextMode="MultiLine" ReadOnly="True" Width="500px" Height="200px" Visible="false"/>  
                    </td>
                        
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
