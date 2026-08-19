<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FuelCardAssignmentForm.aspx.cs" Inherits="FuelsManager.FuelCardWebApp.FuelCardAssignmentForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <title></title>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	<script type="text/javascript" src='../Javascripts/CSRFToken_min.js'></script>
    <script type="text/javascript" src='../Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js'></script>
    <base target="_self">
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
</head>
<script type="text/javascript">
	var rndTokenStr = '<%= Security.CSRFToken%>';
</script>
<body>
    <style>
        .HiddenColumn {
            display: none;
        }
    </style>
    <script type="text/javascript">
        function Cancel()
        {
            var result = new Array();
            setWindowReturnValue(result);
            closeDialogWindow();
        }

        function ReturnSelectedFuelCards()
        {
            var string = '<%=this.SelectedIdentityGuids %>';
        	setWindowReturnValue(string);
        	closeDialogWindow();
        }

    </script>
    <form id="form1" runat="server" defaultbutton="FindButton">
        <div>

            <table style="z-index: 110; left: 15px; top: 0px; width: 300px; position: absolute">
                <tr>
                    <td>
                        <FMControls:FMPageSizeDropDown ID="PageSizeDropDown" runat="server" TabIndex="6" OnSelectedIndexChanged="PageSizeDropDown_OnSelectedIndexChanged" />
                    </td>
                    <td>
                        <FMControls:FMLabel ID="FindLabel" runat="server" Text="Find String:" CssClass="formfieldtitle" Width="60px"></FMControls:FMLabel>
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
                <tr>
                    <td>
                        <FMControls:FMButton ID="SelectAllButton" runat="server" Text="Select All" CssClass="formfieldtitle" Width="75px" TabIndex="3" OnClick="SelectAllButton_OnClick"></FMControls:FMButton>
                    </td>
                    <td>
                        <FMControls:FMButton ID="UnselectAllButton" runat="server" Text="Unselect All" CssClass="formfieldtitle" Width="75px" TabIndex="4" OnClick="UnselectAllButton_OnClick"></FMControls:FMButton>
                    </td>
					<td></td>
 					<td></td>
					<td></td>
               </tr>
				<tr>
					<td colspan="5">
						<FMControls:FMLabel Width="500px" ID="lblWarning" runat="server" CssClass="formfield" Text="abc" 
							Visible="false" ForeColor="Red" />						
					</td>
				</tr>
                <tr>
                    <td colspan="5">
                        <FMControls:FMGridView 
						    RowHeaderColumn="ID"
							ID="FuelCardsGrid" 
							runat="server" 
							FixedHeaders="false" 
							Width="500px" 
							OnRowDataBound="FuelCardsGrid_OnRowDataBound"
                            AllowPaging="true" 
							PageSize="10" 
							ShowFooter="true" 
							ShowFooterWhenEmpty="true" 
							EmptyDataText="No Fuel Cards Available for Assignment" 
							OnPageIndexChanging="FuelCardsGrid_OnPageIndexChanging"
                            TabIndex="7" 
							DataKeyNames="IdentityGuid">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <FMControls:FMLabel ID="AssignLabel" Text="Assign" runat="server" />
                                    </HeaderTemplate>
                                    <HeaderStyle Width="30px" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMCheckBox ID="AssignedCheckBox" runat="server" Checked='<%# DataBinder.Eval(Container, "DataItem.IsSelected") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderStyle CssClass="HiddenColumn"></HeaderStyle>
                                    <ItemStyle CssClass="HiddenColumn"></ItemStyle>
                                    <ItemTemplate>
                                        <asp:Label ID="IdentityGuidLabel" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %>' runat="server" CssClass="formfield" />
                                    </ItemTemplate>
                                    <FooterStyle CssClass="HiddenColumn"></FooterStyle>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <FMControls:FMLabel ID="FuelCardIDHeaderLabel" Text="ID" runat="server" />
                                    </HeaderTemplate>
                                    <HeaderStyle Width="540px" />
                                    <ItemTemplate>
                                        <asp:Label ID="FuelCardIDLabel" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' runat="server" CssClass="formfield" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <FMControls:FMLabel ID="ManagerHeaderLabel" Text="Manager" runat="server" />
                                    </HeaderTemplate>
                                    <HeaderStyle Width="540px" />
                                    <ItemTemplate>
                                        <asp:Label ID="ManagerLabel" Text='<%# DataBinder.Eval(Container, "DataItem.ManagerID") %>' runat="server" CssClass="formfield" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <FMControls:FMLabel ID="BillToHeaderLabel" Text="Bill To" runat="server" />
                                    </HeaderTemplate>
                                    <HeaderStyle Width="540px" />
                                    <ItemTemplate>
                                        <asp:Label ID="BillToLabel" Text='<%# DataBinder.Eval(Container, "DataItem.BillToID") %>' runat="server" CssClass="formfield" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </FMControls:FMGridView>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMButton ID="btnOK" TabIndex="101" runat="server" Width="66px" CssClass="formfieldtitle"
                            Text="OK" OnClick="btnOK_OnClick"></FMControls:FMButton>
                    </td>
                    <td>
                        <FMControls:FMButton ID="btnCancel" TabIndex="102" runat="server" Width="66px" CssClass="formfieldtitle"
                            Text="Cancel" CommandName="Cancel" OnClientClick="Cancel();"></FMControls:FMButton>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
