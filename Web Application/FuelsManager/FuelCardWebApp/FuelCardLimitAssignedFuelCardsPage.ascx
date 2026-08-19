<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FuelCardLimitAssignedFuelCardsPage.ascx.cs" Inherits="FuelsManager.FuelCardWebApp.FuelCardLimitAssignedFuelCardsPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>

<html>
<head>
    <title></title>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
    <style>
        #GridViewContainer {
            height: auto;
            overflow: auto;
            max-height: 500px;
        }
    </style>
    <script type="text/javascript">
        function ShowFuelCardAssignmentForm(fuelCardTextBoxId, mode) {

            showModalDialogFrame({
                url: "FuelCardAssignmentForm.aspx?Mode=" + mode,
                width: 800,
                height: 480,
                onClose: function ()
                {
                    if (this.returnValue != null)
                    {
                        var fuelCardTextBox = document.getElementById(fuelCardTextBoxId);
                        if (fuelCardTextBox != null)
                        {
                            var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue);

                            fuelCardTextBox.value = asciiValue1;
                            fuelCardTextBox.onchange();
                        }
                    }
                }
            });
        };
    </script>
    <div>
        <table style="z-index: 110; left: 0px; top: 0px; width: 600px; position: absolute">
            <tr>
                <td>
                    <FMControls:FMLabel ID="AssignedFuelCardsLabel" runat="server" CssClass="formfieldtitle" Text="Assigned Fuel Cards" Width="200px"
                        Style="left: 0; position: relative" Font-Italic="True" />
                    <FMControls:FMPageSizeDropDown ID="PageSizeDropDown" ToolTip="Page size" runat="server" TabIndex="6" OnSelectedIndexChanged="PageSizeDropDown_OnSelectedIndexChanged" />
                </td>
            </tr>
			<tr>
				<td colspan="2">
					<FMControls:FMLabel Width="500px" ID="lblWarning" runat="server" CssClass="formfield" Text="abc" 
							Visible="false" ForeColor="Red" />						
				</td>
			</tr>
            <tr>
                <td colspan="2">
                    <div id="GridViewContainer">
                        <FMControls:FMGridView ID="AssignedFuelCardsGrid" runat="server" FixedHeaders="false" Width="600px" RowHeaderColumn="ID"
                            AllowPaging="true" PageSize="10" ShowFooter="true" ShowFooterWhenEmpty="true" EmptyDataText="No Fuel Cards Assigned" DataKeyNames="IdentityGuid"
                            OnPageIndexChanging="AssignedFuelCardsGrid_OnPageIndexChanging" TabIndex="7">
                            <Columns>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <FMControls:FMLabel ID="FuelCardIDHeaderLabel" Text="ID" runat="server" />
                                    </HeaderTemplate>
                                    <HeaderStyle Width="540px" />
                                    <ItemTemplate>
                                        <asp:Label ID="FuelCardIDLabel" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' runat="server" CssClass="formfield" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </FMControls:FMGridView>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMButton ID="AssignButton" runat="server" CssClass="formfieldtitle" Text="Assign" Width="100px" TabIndex="8" OnClientClick="ShowFuelCardAssignmentForm('tcFuelCardLimit_tpAssignedFuelCardsPage_FuelCardLimitAssignedFuelCardsPage_AssignmentTextBox', 'Assign'); return false;" />
                    <FMControls:FMButton ID="UnassignButton" runat="server" CssClass="formfieldtitle" Text="Unassign" Width="100px" TabIndex="8" OnClientClick="ShowFuelCardAssignmentForm('tcFuelCardLimit_tpAssignedFuelCardsPage_FuelCardLimitAssignedFuelCardsPage_UnassignmentTextBox','Unassign'); return false;" />
                    <FMControls:FMTextBox ID="AssignmentTextBox" runat="server" AutoPostBack="True" OnTextChanged="AssignmentTextBoxTextChanged" Style="display: none"></FMControls:FMTextBox>
                    <FMControls:FMTextBox ID="UnassignmentTextBox" runat="server" AutoPostBack="True" OnTextChanged="UnassignmentTextBoxTextChanged" Style="display: none"></FMControls:FMTextBox>
                </td>
            </tr>
        </table>
    </div>
</body>
</html>
