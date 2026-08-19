<%@ Page  language="c#" Codebehind="OrderSummary.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.OrderEntryWebApp.OrderSummaryForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
	<head>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
		<style type="text/css">
         .style3
         {
            width: 98px;
            height: 8px;
         }
         .style4
         {
            width: 315px;
            height: 8px;
         }
         .style5
         {
            height: 8px;
         }
         .style6
         {
            width: 98px;
            height: 14px;
         }
         .style7
         {
            width: 315px;
            height: 14px;
         }
         .style8
         {
            height: 14px;
         }
         .style9
         {
            width: 98px;
            height: 13px;
         }
         .style10
         {
            width: 315px;
            height: 13px;
         }
         .style11
         {
            height: 13px;
         }
         .style12
         {
            width: 98px;
            height: 16px;
         }
         .style13
         {
            width: 315px;
            height: 16px;
         }
         .style14
         {
            height: 16px;
         }
         .style15
         {
            width: 98px;
            height: 18px;
         }
         .style16
         {
            width: 315px;
            height: 18px;
         }
         .style17
         {
            height: 18px;
         }
         .style25
         {
            width: 98px;
            height: 40px;
         }
         .style26
         {
            width: 315px;
            height: 40px;
         }
         .style27
         {
            height: 40px;
         }
      </style>
	</head>
	<body MS_POSITIONING="GridLayout">
	    <!--
		<OBJECT id="RSClientPrint" codeBase="../bin" data="data:application/x-oleobject;base64,jd+R+qtTXUWrIPLwI+SY0wAIAAAAAAAAAAAAAA=="
			classid="CLSID:FA91DF8D-53AB-455D-AB20-F2F023E498D3" VIEWASTEXT>
		</OBJECT>
		-->
        <form id="OrderListForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <script type="text/javascript">
                    var oRefreshButton = document.getElementById("RefreshButton");
                    if (oRefreshButton != null) {
                        oRefreshButton.setActive();
                    }

                    function CompanySelect(role, companyTextBoxId) {
                        var companyTextBox = document.getElementById(companyTextBoxId);
                        var inhibitStartupLoad = document.getElementById("InhibitAutoLoadTextBox");

                        showModalDialogFrame({
                            url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&All=true" + "&Inhibit=" + inhibitStartupLoad.value,
                            width: 870,
                            height: 700,
                            title: "Company Select",
                            onClose: function () {
                                if (this.returnValue != null) {
                                    var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
                                    var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

                                    companyTextBox.value = asciiValue1;
                                    companyTextBox.title = asciiValue2;
                                }
                            }
                        });
                    }
//				function PrintMultipleBol(Server,ReportUrl,ReportName)
//				{
//					RSClientPrint.MarginLeft = 12.7;
//					RSClientPrint.MarginTop = 12.7;
//					RSClientPrint.MarginRight = 12.7;
//					RSClientPrint.MarginBottom = 12.7;

//					RSClientPrint.Culture = 1033;
//					RSClientPrint.UICulture = 9;
//					RSClientPrint.Authenticate = 1;

//					RSClientPrint.Print(Server, 
//												ReportUrl, 
//												ReportName);
//				}
                </SCRIPT>
            <asp:Image ID="FadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <asp:TextBox ID="InhibitAutoLoadTextBox" Style="z-index: 99; left: 190px; position: absolute; top: 5px; display: none"
                runat="server" Width="45px">False</asp:TextBox>
            <FMControls:FMLabel ID="PageTitle" Style="z-index: 101; left: 16px; position: absolute; top: 8px" runat="server"
                BackColor="Transparent" Text="Order Summary" Width="500px" CssClass="headline"></FMControls:FMLabel>
            <!-- The InhibitAutoLoadTextBox is hidden with display:none because if you use visible = false the control won't be rendered to the client,
                and that will mess up the javascript in the CompanySelect function -->
                <table style="z-index: 128; left: 27px; position: absolute; top: 39px; width: 806px; height: 236px;"
                    border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="style3">
                            <FMControls:FMLabel ID="FMLABEL10" AssociatedControlID="OrderNumberTextBox" runat="server" BackColor="Transparent" Text="Order Number"
                                Width="101" CssClass="formfieldtitle">Order Number</FMControls:FMLabel>
                        </td>
                        <td class="style4">
                            <asp:TextBox ID="OrderNumberTextBox" TabIndex="0" runat="server" Width="88px" CssClass="formfield"></asp:TextBox>
                        </td>
                        <td class="style5">
                            <FMControls:FMLabel ID="FMLABEL4" runat="server" BackColor="Transparent" Text="Manager" Width="100"
                                CssClass="formfieldtitle"></FMControls:FMLabel>
                        </td>
                        <td class="style5">
                            <FMControls:FMCompanyTextBox ID="ManagerTextBox" runat="server" Width="145px" CssClass="formfield"
                                Role="MANAGER"></FMControls:FMCompanyTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style6">
                            <FMControls:FMLabel ID="Fmlabel9" AssociatedControlID="OrderStatusDropDownList" runat="server" BackColor="Transparent" Text="Order Status"
                                Width="101px" CssClass="formfieldtitle"></FMControls:FMLabel>
                        </td>
                        <td class="style7">
                            <asp:DropDownList ID="OrderStatusDropDownList" runat="server" Width="160px" CssClass="formfield"></asp:DropDownList>
                        </td>
                        <td class="style8">
                            <FMControls:FMLabel ID="FMLABEL2" runat="server" BackColor="Transparent" Text="Owner" Width="100px"
                                CssClass="formfieldtitle"></FMControls:FMLabel>
                        </td>
                        <td class="style8">
                            <FMControls:FMCompanyTextBox ID="OwnerTextBox" runat="server" Width="145px" CssClass="formfield"
                                Role="OWNER"></FMControls:FMCompanyTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style9">
                            <FMControls:FMLabel ID="FMLABEL7" AssociatedControlID="OrderTypeDropDown" runat="server" BackColor="Transparent" Text="Order Type"
                                Width="101px" CssClass="formfieldtitle"></FMControls:FMLabel>
                        </td>
                        <td class="style10">
                            <asp:DropDownList ID="OrderTypeDropDown" runat="server" Width="160px" CssClass="formfield"></asp:DropDownList>
                        </td>
                        <td class="style11">
                            <FMControls:FMLabel ID="Fmlabel8" runat="server" BackColor="Transparent" Text="Shipper"
                                Width="100px" CssClass="formfieldtitle"></FMControls:FMLabel>
                        </td>
                        <td class="style11">
                            <FMControls:FMCompanyTextBox ID="ShipperTextBox" runat="server" Width="145px" CssClass="formfield" Role="SHIPPER"></FMControls:FMCompanyTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style12">
                            <FMControls:FMLabel ID="OrderStatusLabel" AssociatedControlID="ProductDropDown" runat="server" BackColor="Transparent" Text="Product"
                                Width="101px" CssClass="formfieldtitle"></FMControls:FMLabel>
                        </td>
                        <td class="style13">
                            <asp:DropDownList ID="ProductDropDown" runat="server" Width="160px" CssClass="formfield"></asp:DropDownList>
                        </td>
                        <td class="style14">
                            <FMControls:FMLabel ID="FMLABEL6" runat="server" BackColor="Transparent" Text="Bill-To"
                                Width="100px" CssClass="formfieldtitle"></FMControls:FMLabel>
                        </td>
                        <td class="style14">
                            <FMControls:FMCompanyTextBox ID="BillToTextBox" runat="server" Width="145px" CssClass="formfield" Role="CUSTOMER_BILLTO"></FMControls:FMCompanyTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style15">
                            <FMControls:FMLabel ID="FMLABEL11" AssociatedControlID="DateFilterTypeDropDown" runat="server" BackColor="Transparent" Text="Product"
                                Width="102" CssClass="formfieldtitle">Date Filter Type</FMControls:FMLabel>
                        </td>
                        <td class="style16">
                            <FMControls:FMDropDownList ID="DateFilterTypeDropDown" runat="server" CssClass="formfield" Sort="false" OnSelectedIndexChanged="DateFilterTypeDropDown_SelectedIndexChanged" AutoPostBack="True"></FMControls:FMDropDownList>
                        </td>
                        <td class="style17">
                            <FMControls:FMLabel ID="OwnerLabel" runat="server" BackColor="Transparent" Text="Ship-To" Width="100"
                                CssClass="formfieldtitle"></FMControls:FMLabel>
                        </td>
                        <td class="style17">
                            <FMControls:FMCompanyTextBox ID="ShipToTextBox" runat="server" Width="145px" CssClass="formfield" Role="CUSTOMER_SHIPTO"></FMControls:FMCompanyTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style25">
                            <FMControls:FMLabel ID="StartDateLabel" runat="server" BackColor="Transparent" Text="Start Date"
                                Width="100px" CssClass="formfieldtitle">Start Date</FMControls:FMLabel>
                        </td>
                        <td class="style26">
                            <FMControls:FMDate ID="StartDate" runat="server" Width="160px" Style="position: relative; z-index: 101"
                                CssClass="formfield" Height="16px"></FMControls:FMDate>
                        </td>
                        <td class="style27">
                            <FMControls:FMLabel ID="FMLABEL1" runat="server" BackColor="Transparent" Text="Carrier"
                                Width="100px" CssClass="formfieldtitle"></FMControls:FMLabel>
                        </td>
                        <td class="style27">
                            <FMControls:FMCompanyTextBox ID="CarrierTextBox" runat="server" Width="145px" CssClass="formfield" Role="CARRIER"></FMControls:FMCompanyTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style9">
                            <FMControls:FMLabel ID="EndDateLabel" runat="server" BackColor="Transparent" Text="End Date"
                                Width="100px" CssClass="formfieldtitle"></FMControls:FMLabel>
                        </td>
                        <td class="style10">
                            <FMControls:FMDate ID="EndDate" runat="server" Width="160px" Style="position: relative; z-index: 100"
                                CssClass="formfield" Height="16px"></FMControls:FMDate>
                        </td>
                        <td class="style11">
                            <asp:DropDownList ID="ChangeOrderStatusDropdownlist" ToolTip="Change order status" TabIndex="0" runat="server" Width="160px"
                                CssClass="formfield">
                            </asp:DropDownList>
                        </td>
                        <td class="style11">
                            <FMControls:FMButton ID="ChangeSelection" TabIndex="14" runat="server" Text="Change Selected"
                                Width="115px" CssClass="formfield" OnClick="ChangeSelectedClick"></FMControls:FMButton>
                        </td>
                    </tr>
                </table>
                <table style="z-index: 113; left: 22px; position: absolute; top: 303px; width: 789px;">
                    <tr>
                        <%--					<td height="36">
					   <FMCONTROLS:FMPAGESIZEDROPDOWN id="OrderSummarySizeDropDown" runat="server" onselectedindexchanged="PageSizeDropDown_SelectedIndexChanged"></FMCONTROLS:FMPAGESIZEDROPDOWN>&nbsp;&nbsp;
						<FMCONTROLS:FMLABEL id="FMLABEL3" runat="server" BackColor="Transparent" CssClass="formfieldtitle"></FMCONTROLS:FMLABEL>
					</td>
                        --%>
                        <td align="right">
                            <FMControls:FMButton ID="SELECTALL" TabIndex="14" runat="server" Text="Select All" Height="22" Width="100"
                                CssClass="formfield" OnClick="OnSelectAll"></FMControls:FMButton>
                            &nbsp;&nbsp;
                  <FMControls:FMButton ID="DESELECTALL" TabIndex="14" runat="server" Text="Unselect All" Height="22" Width="100"
                      CssClass="formfield" OnClick="OnUnselectAll"></FMControls:FMButton>
                            &nbsp;&nbsp;
			         <FMControls:FMButton ID="PrintSelection" TabIndex="14" runat="server" Text="Print Selected" Height="22" Width="100" CssClass="formfield"
                         OnClick="PrintSelectionClick"></FMControls:FMButton>
                            &nbsp;&nbsp;
			         <FMControls:FMButton ID="RefreshButton" runat="server" Text="Refresh" CssClass="formfield" Height="22" Width="100" 
                         OnClick="RefreshButtonClick"></FMControls:FMButton>
                        </td>
                    </tr>
                </table>
                <table style="z-index: 113; left: 22px; position: absolute; top: 342px; width: 808px;">
                    <tr>
                        <td>
                            <FMControls:FMLabel ID="WarningLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle"></FMControls:FMLabel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <FMControls:FMDataGridFixed ID="TransactionDataGrid" runat="server" BackColor="White" Width="736px" CssClass="tabletext"
                                PageSize="8" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical"
                                BorderStyle="None" AllowPaging="True" AutoGenerateColumns="False" Height="500px">
                                <SelectedItemStyle Font-Bold="True" ForeColor="White" CssClass="tablelink" BackColor="#008A8C"></SelectedItemStyle>
                                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                                <Columns>
                                    <asp:TemplateColumn HeaderText="Edit">
                                        <HeaderStyle Width="55px"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        <ItemTemplate>
                                            <FMControls:FMEditLinkButton runat="server" ID="btnEdit" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Multiple Select">
                                        <HeaderStyle Width="0.5in"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                        <ItemTemplate>
                                            <FMControls:FMCheckBox ID="MultipleSelectCheckbox" ToolTip="Multiple Select" runat="server" Checked="false" Enabled="true"></FMControls:FMCheckBox>
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                </Columns>
                            </FMControls:FMDataGridFixed></td>
                    </tr>
                </table>
            </div>
        </form>
    </body>
</HTML>
