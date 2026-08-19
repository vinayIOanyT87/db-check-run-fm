<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls"%>
<%@ Page language="c#" Codebehind="BillOfLadingsForm.aspx.cs" AutoEventWireup="True" Inherits="FMAccountingWebApp.BillOfLadingsForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
	<HEAD>
		<base target="_self">
		<title></title>
		<meta content="False" name="vs_snapToGrid">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body tabIndex="-1" MS_POSITIONING="GridLayout">
        <style>
            .searchControlDiv{
                width: 350px; 
                padding-left:50px;
                margin:2px;
                position: relative;
            }
            .seacrhDiv{
                position: relative;
                display: flex;
                width: 1000px;
                flex-wrap: wrap;
            }

             .searchControlDiv > div
            {
                display: inline-block;
            }

            .searchControlDiv > label, .searchControlDiv > span
            {
                display: inline-block;
                width:95px;
            }

            .searchButtons {
                position: absolute;
                width:100px;
                height:22px;

            }

            #grid_scroll_div {
                max-height: calc(100vh - 400px) !important;
			    overflow: auto;
            }
        </style>
		<SCRIPT>
            document.addEventListener('keydown', function (ev) {
                if (ev.keyCode == 13) {
                    ev.returnValue = false;
                    ev.cancel = true;
                    var refreshvar = document.getElementById("RefreshButton");
                    if (refreshvar != null) {
                        refreshvar.click();
                    }
                }
            });

            function TransactionSelect(TransID) {
                var Result = new Array();
                Result[0] = TransID;
                setWindowReturnValue(Result);
                closeDialogWindow();
            }

            function CompanySelect(role, companyTextBoxId) {
                var companyTextBox = document.getElementById(companyTextBoxId);
                var inhibitStartupLoad = document.getElementById("InhibitAutoLoadTextBox");

                showModalDialogFrame({
                    url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&All=true" + "&Inhibit=" + inhibitStartupLoad.value,
                    width: 855,
                    height: 690,
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
        </SCRIPT>
		<form id="BillOfLadingsForm" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent">
            <div style="padding-left:50px; padding-bottom:5px;">
                <FMControls:FMLabel ID="Label27" runat="server"
                    CssClass="headline" BackColor="Transparent" Width="136px">Bills of Lading</FMControls:FMLabel>
            </div>
            <div class="seacrhDiv" >
                <!-- The InhibitAutoLoadTextBox is hidden with display:none because if you use visible = false the control won't be rendered to the client,
                    and that will mess up the javascript in the CompanySelect function -->
                <asp:TextBox ID="InhibitAutoLoadTextBox" Style="z-index: 100; left: 190px; position: absolute; top: 5px; display: none"
                    runat="server" Width="45px">False</asp:TextBox>

                <div class="searchControlDiv" >
                    <FMControls:FMLabel ID="Label1" AssociatedControlID="BOLNumberTextBox" runat="server" CssClass="formfieldtitle" BackColor="Transparent">BOL Number</FMControls:FMLabel>
                    <asp:TextBox ID="BOLNumberTextBox" TabIndex="1" runat="server" CssClass="formfield" Width="120px" OnTextChanged="BOLNumberTextBox_TextChanged"></asp:TextBox>
                </div>
                <div class="searchControlDiv">
                    <FMControls:FMLabel ID="Label7" runat="server" CssClass="formfieldtitle" Width="94px" AssociatedControlID="ManagerTextBox">Manager</FMControls:FMLabel>
                    <FMControls:FMCompanyTextBox ID="ManagerTextBox" TabIndex="1" runat="server" CssClass="formfield" Width="201px" Role="MANAGER"></FMControls:FMCompanyTextBox>
                </div>
                <div class="searchControlDiv">
                    <FMControls:FMLabel ID="FMLABEL1" AssociatedControlID="StatusDropDownList" runat="server"
                        CssClass="formfieldtitle" BackColor="Transparent" Height="15px">Status</FMControls:FMLabel>
                    <FMControls:FMDropDownList ID="StatusDropDownList" runat="server" CssClass="formfield" Width="126px">
                    </FMControls:FMDropDownList>

                </div>
                <div class="searchControlDiv">
                    <FMControls:FMLabel ID="Label9" runat="server" CssClass="formfieldtitle" AssociatedControlID="OwnerTextBox">Owner</FMControls:FMLabel>
                    <FMControls:FMCompanyTextBox ID="OwnerTextBox" TabIndex="1" runat="server" CssClass="formfield" Width="201px" Role="OWNER"></FMControls:FMCompanyTextBox>
                </div>
                <div class="searchControlDiv">
                    <FMControls:FMLabel ID="FMLABEL2" AssociatedControlID="LocationIDDropDown" runat="server"
                        CssClass="formfieldtitle" BackColor="Transparent" Height="15px">Location ID</FMControls:FMLabel>
                    <FMControls:FMDropDownList ID="LocationIDDropDown" runat="server" CssClass="formfield" Width="126px">
                    </FMControls:FMDropDownList>

                </div>
                <div class="searchControlDiv">
                    <FMControls:FMLabel ID="Label2" runat="server"
                        CssClass="formfieldtitle" AssociatedControlID="ShipperTextBox">Shipper</FMControls:FMLabel>
                    <FMControls:FMCompanyTextBox ID="ShipperTextBox" TabIndex="1" runat="server" CssClass="formfield" Width="201px" Role="SHIPPER"></FMControls:FMCompanyTextBox>

                </div>
                <div class="searchControlDiv">
                    <FMControls:FMLabel ID="FMLABEL3" AssociatedControlID="ProductDropDown" runat="server"
                        CssClass="formfieldtitle" BackColor="Transparent" Height="15px">Product</FMControls:FMLabel>
                    <FMControls:FMDropDownList ID="ProductDropDown" runat="server" CssClass="formfield" Width="126px">
                    </FMControls:FMDropDownList>

                </div>
                <div class="searchControlDiv">
                    <FMControls:FMLabel ID="Label3" runat="server" CssClass="formfieldtitle" AssociatedControlID="BillToTextBox">Bill To</FMControls:FMLabel>
                    <FMControls:FMCompanyTextBox ID="BillToTextBox" TabIndex="1" runat="server" CssClass="formfield" Width="201px" Role="CUSTOMER_BILLTO"></FMControls:FMCompanyTextBox>

                </div>
                <div class="searchControlDiv">
                    <FMControls:FMLabel ID="Label5" runat="server" CssClass="formfieldtitle" BackColor="Transparent" Height="15">Beginning</FMControls:FMLabel>
                    <FMControls:FMDate ID="BeginningDate" 
                        TabIndex="2" runat="server" CssClass="formfield" Width="160px" ToolTip="Beginning Date"></FMControls:FMDate>

                </div>
                <div class="searchControlDiv">
                    <FMControls:FMLabel ID="Label4" runat="server"
                        CssClass="formfieldtitle" AssociatedControlID="ShipToTextBox">Ship To</FMControls:FMLabel>
                    <FMControls:FMCompanyTextBox ID="ShipToTextBox" TabIndex="1" runat="server" CssClass="formfield" Width="201px" Role="CUSTOMER_SHIPTO"></FMControls:FMCompanyTextBox>
                </div>
                <div class="searchControlDiv">
                    <FMControls:FMLabel ID="Label6" runat="server"
                        CssClass="formfieldtitle" BackColor="Transparent" Height="15px">Ending</FMControls:FMLabel>
                    <FMControls:FMDate ID="EndingDate" TabIndex="4" runat="server" CssClass="formfield" Width="160px" ToolTip="Ending Date"></FMControls:FMDate>
                </div>
                <div class="searchControlDiv">
                    <FMControls:FMLabel ID="FMLabelCarrier" runat="server" CssClass="formfieldtitle" AssociatedControlID="CarrierTextBox">Carrier</FMControls:FMLabel>
                    <FMControls:FMCompanyTextBox ID="CarrierTextBox" TabIndex="1" runat="server" CssClass="formfield" Width="201px" Role="CARRIER"></FMControls:FMCompanyTextBox>
                </div>
                <panel ID="DestinationSerialNumber1" class="searchControlDiv" Style="display:none;" runat="server">
                    <FMControls:FMLabel ID="DestinationSerialNumber1Label"  AssociatedControlID="DestinationSerialNumber1TextBox" runat="server"
                        CssClass="formfieldtitle" BackColor="Transparent">Dest Serial #1</FMControls:FMLabel>
                    <asp:TextBox ID="DestinationSerialNumber1TextBox" TabIndex="1" runat="server" CssClass="formfield" Width="120px" OnTextChanged="DestinationSerialNumber1_TextChanged"></asp:TextBox>

                </panel>
                <panel  ID="DestinationSerialNumber2" class="searchControlDiv" Style="display:none;" runat="server">
                    <FMControls:FMLabel ID="DestinationSerialNumber2Label"  AssociatedControlID="DestinationSerialNumber2TextBox" runat="server"
                        CssClass="formfieldtitle" BackColor="Transparent">Dest Serial #2</FMControls:FMLabel>
                    <asp:TextBox ID="DestinationSerialNumber2TextBox" 
                        TabIndex="1" runat="server" CssClass="formfield" Width="120px"  OnTextChanged="DestinationSerialNumber2_TextChanged"></asp:TextBox>
                </panel>
                <panel ID="DestinationSerialNumber3" class="searchControlDiv" Style="display:none;" runat="server">
                    <FMControls:FMLabel ID="DestinationSerialNumber3Label"  AssociatedControlID="DestinationSerialNumber3TextBox" runat="server"
                        CssClass="formfieldtitle" BackColor="Transparent">Dest Serial #3</FMControls:FMLabel>
                    <asp:TextBox ID="DestinationSerialNumber3TextBox" 
                        TabIndex="1" runat="server" CssClass="formfield" Width="120px" OnTextChanged="DestinationSerialNumber3_TextChanged"></asp:TextBox>
                </panel>
                <div style="margin-top: 20px; width: 700px; padding-left: 15px;">
                    <div Style="display:inline;">
                       <FMControls:FMLabel ID="Label8"  runat="server" CssClass="formfieldtitle" BackColor="Transparent">Bills Of Lading</FMControls:FMLabel><br />
                       <FMControls:FMPageSizeDropDown ID="BOLFormPageSizeDropDown" style="margin-top: 10px;" ToolTip="Page size" TabIndex="7" runat="server" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged"></FMControls:FMPageSizeDropDown>
                     </div>
                    <FMControls:FMButton ID="SELECTALL"      Style="left: 317px;" TabIndex="14" runat="server" CssClass="searchButtons formfield" Text="Select All" OnClick="OnSelectAll"></FMControls:FMButton>
			        <FMControls:FMButton ID="DESELECTALL"    Style="left: 437px;" TabIndex="14" runat="server" CssClass="searchButtons formfield" Text="Unselect All" OnClick="UnSelectAll"></FMControls:FMButton>
			        <FMControls:FMButton ID="PrintSelection" Style="left: 557px;" TabIndex="12" runat="server" CssClass="searchButtons formfield" Text="Print Selected" OnClick="OnPrintSelected_Click"></FMControls:FMButton>
			        <FMControls:FMButton ID="RefreshButton"  Style="left: 677px;" TabIndex="11" runat="server" CssClass="searchButtons formfield" Text="Refresh"></FMControls:FMButton> 
                </div>
		    </div>
			    <div id="grid_scroll_div" style="margin-top:15px;margin-left:15px;width:1218px;">
				    <FMControls:FMGridTxSummary ID="BillOfLadingsDataGrid" TabIndex="12" runat="server" CssClass="tabletext" BackColor="White" RowHeaderColumn="Date &amp; Time"
					    AllowPaging="True" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" Width="1200px" BorderWidth="1px" AllowSorting="True" BorderColor="White"
					    CellPadding="3" OnDataBinding="BillOfLadingsDataGrid.Page_DataBinding" aria-label="Transaction Summary" role="presentation" >
					    <FooterStyle BackColor="#CCCCCC" ForeColor="Black"></FooterStyle>
					    <SelectedItemStyle BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SelectedItemStyle>
					    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
					    <ItemStyle BackColor="#EEEEEE" ForeColor="Black"></ItemStyle>
					    <HeaderStyle BackColor="<%$ AppSettings: ColorHeaderBlue %>" CssClass="tablecolhead" ForeColor="White" Font-Bold="True"></HeaderStyle>
					    <Columns>
						    <asp:TemplateColumn HeaderText="Edit">
							    <HeaderStyle Width="0.3in"></HeaderStyle>
							    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
							    <ItemTemplate>
								    <FMControls:FMEditLinkButton ID="FMEditLinkButton1" runat="server"></FMControls:FMEditLinkButton>
							    </ItemTemplate>
						    </asp:TemplateColumn>
						    <asp:TemplateColumn HeaderText="Select">
							    <HeaderStyle Width="0.3in"></HeaderStyle>
							    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
							    <ItemTemplate>
								    <a id="Select" href=javascript:TransactionSelect('<%# DataBinder.Eval(Container, "DataItem.TransID") %>')><img src="../FMWebApp/Images/Select.gif" border="0" align="middle" alt="<%# SelectThisItemText%>"></a>
							    </ItemTemplate>
						    </asp:TemplateColumn>
						    <asp:TemplateColumn HeaderText="Multiple Select">
							    <HeaderStyle Width="0.5in"></HeaderStyle>
							    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							    <ItemTemplate>
								    <asp:CheckBox ID="MultipleSelectCheckbox" runat="server" Checked="false" Enabled="true"></asp:CheckBox>
							    </ItemTemplate>
						    </asp:TemplateColumn>
						    <asp:BoundColumn HeaderText="TransID" DataField="TransID" Visible="False"></asp:BoundColumn>
						    <asp:BoundColumn HeaderText="Number" DataField="DocumentNumber"></asp:BoundColumn>
						    <asp:BoundColumn HeaderText="Date &amp; Time" DataField="TransDateTime"></asp:BoundColumn>
					    </Columns>
					    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
				    </FMControls:FMGridTxSummary>
                </div>
        </div>
</form>
	</body>
</HTML>