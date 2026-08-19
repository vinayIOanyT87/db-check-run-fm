<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="GroupCompaniesPage.ascx.cs" Inherits="FuelsManager.FMWebApp.GroupCompaniesPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<html>
<head>
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<script>
    $(document).ready(function () { DisableAssignButtons(); });

    function CompanySelect(companyTextBoxId, mode) {
        var companyTextBox = document.getElementById(companyTextBoxId);

        showModalDialogFrame({
            url: "../FMWebApp/CompanySelectForm.aspx?Mode=" + mode + "&Map=USER_GROUP_COMPANY_MAP&All=true",
            width: 855,
            height: 690,
            title: "Company Select",
            onClose: function () {
                if (this.returnValue != null) {
                    var result = this.returnValue;

                    if (result != null && result.length > 0) {
                        for (var i = 0; i < result.length; i++) {
                            var sResult = result[i];

                            sResult = sResult.replace("<", "&lt;");
                            sResult = sResult.replace(">", "&gt;");

                            companyTextBox.value = companyTextBox.value + sResult + "|";
                        }

                        companyTextBox.onchange();
                    }
                }
            }
        });
    }

    //-----------------------------------------------------------------------
    // This function will disable the assigned and unassigned buttons based
    // on the disableFlag.
    //-----------------------------------------------------------------------
    function DisableAssignButtons()
    {
        var disableFlagControl = document.getElementById("tcGroupTabs_tpCompaniesPage_GroupCompaniesPage_DisableButtonFlag");

        if (disableFlagControl == null)
        {
            return;
        }

        var disableFlag = disableFlagControl.value;
        if (disableFlag === "TRUE")
        {
            var assignBtn = document.getElementById("GroupCompaniesPage_AssignButton");
            var unassignBtn = document.getElementById("GroupCompaniesPage_UnassignButton");

            if (assignBtn != null)
            {
                assignBtn.disabled = true;
            }

            if (unassignBtn != null)
            {
                unassignBtn.disabled = true;
            }
        }
    }
</script>
<body >
    <FMControls:FMLabel ID="Label3" Style="z-index: 112; left: 8px; position: absolute; top: 8px" runat="server"
        CssClass="formfieldtitle">Assigned Companies:</FMControls:FMLabel>
    <table id="Table1" style="z-index: 113; left: 0px; position: absolute; top: 32px; height: 10px"
        cellspacing="0" cellpadding="1" width="341" border="0" role="presentation" aria-label="layout">
        <tr>
            <td width="341" height="10" valign="top">
                <FMControls:FMDataGrid ID="AssignedCompaniesDataGrid" TabIndex="5" runat="server" CssClass="tabletext" RowHeaderColumn="ID"
                    Width="341px" PageSize="8" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True"
                    BorderWidth="1px" GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None" BackColor="White"
                    aria-label="Assigned Companies">
                    <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>" />
                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C" />
                    <AlternatingItemStyle BackColor="Gainsboro" />
                    <ItemStyle ForeColor="Black" BackColor="#EEEEEE" />
                    <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>" />
                    <Columns>
                        <asp:TemplateColumn HeaderText="ID">
                            <HeaderStyle Width="3in" />
                            <ItemStyle Wrap="False" />
                            <ItemTemplate>
                                <asp:Label Width="2.5in" runat="server" ID="IDLabel" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages" />
                </FMControls:FMDataGrid>
            </td>
        </tr>
        <tr>
            <td width="341" height="36">
                <table role="presentation" aria-label="layout">
                    <tr>
                        <td width="83" height="10">
                            <input class="formfieldtitle" id="GroupCompaniesPage_AssignButton" style="width: 80px" onclick="CompanySelect('tcGroupTabs_tpCompaniesPage_GroupCompaniesPage_AssignCompaniesTextBox','Assign')"
                                type="button" value="Assign"></td>
                        <td height="10">
                            <input class="formfieldtitle" id="GroupCompaniesPage_UnassignButton" style="width: 80px" onclick="CompanySelect('tcGroupTabs_tpCompaniesPage_GroupCompaniesPage_UnassignCompaniesTextBox','Unassign')"
                                type="button" value="Unassign"></td>
                        <td>
                            <asp:TextBox ID="AssignCompaniesTextBox" ToolTip="Assign company" runat="server" Width="0px"
                                BackColor="Transparent" BorderStyle="None"
                                BorderColor="Transparent" ForeColor="Transparent" AutoPostBack="True"
                                OnTextChanged="AssignCompaniesTextBoxTextChanged"></asp:TextBox></td>
                        <td>
                            <asp:TextBox ID="UnassignCompaniesTextBox" ToolTip="Unassign company" runat="server" Width="0px"
                                BackColor="Transparent" BorderStyle="None"
                                BorderColor="Transparent" ForeColor="Transparent" AutoPostBack="True"
                                OnTextChanged="UnassignCompaniesTextBoxTextChanged"></asp:TextBox></td>
                    </tr>
                </table>
            </td>
            <td>
                <asp:HiddenField ID="DisableButtonFlag" runat="server" Value="FALSE" />
            </td>
        </tr>
    </table>
</body>
</html>
