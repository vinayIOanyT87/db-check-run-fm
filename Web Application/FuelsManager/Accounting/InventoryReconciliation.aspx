<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="InventoryReconciliation.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.InventoryReconciliation" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
		<style type="text/css">
			.style1
			{
				height: 1px;
				width: 64px;
			}
			.style2
			{
				height: 3px;
				width: 64px;
			}
			.style3
			{
				height: 1px;
				width: 98px;
			}
			.style4
			{
				width: 255px;
			}
			.style5
			{
				height: 1px;
				width: 138px;
			}
			.style6
			{
				height: 3px;
				width: 138px;
			}
			table.tabletext {
				margin-bottom: 15px;
			}
	  </style>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<SCRIPT>
			    function CompanySelect(role, companyTextBoxId) {
			        var companyTextBox = document.getElementById(companyTextBoxId);

			        showModalDialogFrame({
			            url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "",
			            width: 855,
			            height: 560,
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

			    function ProductSelect(productTextBoxId) {
			        var productTextBox = document.getElementById(productTextBoxId);
			        var companyManagerTextBox = document.getElementById("managerTextBox");
			        var companyId = "";

			        if (companyManagerTextBox != null) {
			            companyId = companyManagerTextBox.value + "|manager";
			        }

			        if (companyId.substr(0, 1) === "<") {
			            companyId = "";
			        }

			        showModalDialogFrame({
			            url: "../FMWebApp/ProductSelectForm.aspx?Type=MaxProduct&Map=MAX_MAP&IDLink=" + encodeURIComponent(companyId),
			            width: 855,
			            height: 560,
			            onClose: function () {
			                if (this.returnValue != null) {
			                    var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
			                    var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

			                    productTextBox.value = asciiValue1;
			                    productTextBox.title = asciiValue2;
			                }
			            }
			        });
			    }
			    function TankSelect(tankTextBoxId) {
			        var tankTextBox = document.getElementById(tankTextBoxId);
			        var productId = "";
			        var productTextBox = null;
			        var prodIdListId = tankTextBoxId.replace("StorageLocation", "Product");

			        if (prodIdListId != null) {
			            productTextBox = document.getElementById(prodIdListId);

			            if (productTextBox == null) {
			                if (prodIdListId.indexOf("ToProduct") > -1) {
			                    prodIdListId = prodIdListId.replace("ToProduct", "Product");
			                }
			                else if (prodIdListId.indexOf("FromProduct") > -1) {
			                    prodIdListId = prodIdListId.replace("FromProduct", "Product");
			                }

			                productTextBox = document.getElementById(prodIdListId);
			            }
			        }

			        if (productTextBox != null) {
			            productId = productTextBox.value;

			            if (productId.substr(0, 1) === "<") {
			                productId = "";
			            }
			        }

			        var managerTextBox = document.getElementById("TransactionFields.ManagerFG");
			        var managerId = "";

			        if (managerTextBox != null) {
			            managerId = managerTextBox.value;
			        }

			        showModalDialogFrame({
			            url: "../FMWebApp/TankSelectForm.aspx?IDProductLink=" + encodeURIComponent(productId) +
                                "&IDManagerLink=" + encodeURIComponent(managerId),
			            width: 855,
			            height: 560,
			            title: "Tank Select",
			            onClose: function () {
			                if (this.returnValue != null) {
			                    var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
			                    var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

			                    tankTextBox.value = asciiValue1;
			                    tankTextBox.title = asciiValue2;

			                    __mydoPostBack('TANK_REFRESH', asciiValue1);
			                }
			            }
			        });
			    }

			</SCRIPT>
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
				<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			    <TABLE id="Table1" 
				    style="Z-INDEX: 118; LEFT: 5px; POSITION: absolute; TOP: 0px; width: 734px;" role="presentation" aria-label="Layout">
				    <TR width="350">
					    <td>
						    <table style="WIDTH: 703px; HEIGHT: 10px" role="presentation" aria-label="Filter Layout">
							    <tr>
								    <TD style="HEIGHT: 1px"><FMCONTROLS:FMLABEL id="dateLabel" AssociatedControlID="dateDropDownList" runat="server" 
										    BackColor="Transparent" Height="0px" CssClass="formfieldtitle"
										    Width="59px">Month:</FMCONTROLS:FMLABEL></TD>
								    <TD align="left" style="HEIGHT: 1px"><asp:dropdownlist id="dateDropDownList" runat="server" CssClass="formfield" Width="128px" onselectedindexchanged="DateDropDownListSelectedIndexChanged"></asp:dropdownlist></TD>
								    <TD style="HEIGHT: 1px"><FMCONTROLS:FMLABEL id="productLabel" runat="server" 
										    BackColor="Transparent" Height="18px" CssClass="formfieldtitle"
										    Width="67px">Product:</FMCONTROLS:FMLABEL></TD>
								    <TD align="left" class="style5" nowrap="true">
									    <FMControls:FMProductTextBox id="productTextBox" tabIndex="4" runat="server" Width="100px" CssClass="formfield"
										    AutoPostBack="True" ontextchanged="ProductTextBoxTextChanged"></FMControls:FMProductTextBox>
								    </TD>
								    <TD class="style1"><FMCONTROLS:FMLABEL id="FMLABEL1" AssociatedControlID="QuantityDropDownList" runat="server" 
										    BackColor="Transparent" Height="18px" CssClass="formfieldtitle"
										    Width="63px">Quantity:</FMCONTROLS:FMLABEL></TD>
								    <TD align="left" rowSpan="1" class="style4"><fmcontrols:FMDropDownList ID="QuantityDropDownList" runat="server" 
												    style="z-index:110"  
												    CssClass="formfield"  onselectedindexchanged="QuantityDropDownListSelectedIndexChanged">
												    <asp:ListItem Value="0">Gross and Net</asp:ListItem>
												    <asp:ListItem Value="1">Gross</asp:ListItem>
												    <asp:ListItem Value="2">Net</asp:ListItem>
												    <asp:ListItem Value="3">Mass</asp:ListItem>
												    <asp:ListItem Value="4">Package</asp:ListItem>
											    </fmcontrols:FMDropDownList>
								    </TD>
								    <TD align="left" class="style3"><FMCONTROLS:FMBUTTON id="refreshButton" style="min-width: 100px" runat="server" CssClass="formfieldtitle" Text="Refresh" onclick="RefreshButtonClick" /></TD>
							    </tr>
							    <TR>
								    <TD style="HEIGHT: 3px"><FMCONTROLS:FMLABEL id="managerLabel" runat="server" 
										    BackColor="Transparent" Height="18px" CssClass="formfieldtitle"
										    Width="59px">Manager:</FMCONTROLS:FMLABEL></TD>
								    <TD style="HEIGHT: 3px;" align="left" nowrap="true">
									    <FMControls:FMCompanyTextBox id="managerTextBox" style="Z-INDEX: 112;" 
										    tabIndex="3" runat="server" CssClass="formfield"
										    Width="150px" Role="MANAGER" ontextchanged="ManagerTextBoxTextChanged"></FMControls:FMCompanyTextBox>
								    </TD>
								    <TD style="HEIGHT: 3px"><FMControls:FMLabel id="ToleranceLabel" AssociatedControlID="ToleranceTextBox" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
										    Width="72px">Tolerance %:</FMControls:FMLabel></TD>
								    <TD align="left" class="style6"><asp:textbox id="ToleranceTextBox" style="Z-INDEX: 101" runat="server" BackColor="Transparent"
										    CssClass="formfield" Width="52px" MaxLength="3" Columns="3" Enabled="False"></asp:textbox></TD>
								    <TD class="style2"></TD>
									<td colspan="2" style="text-align: right">
										<FMControls:FMButton ID="autoDistributionButton" style="min-width:140px" runat="server" CssClass="formfieldtitle" Enabled="false"
											Text="EOM Auto Distribution" OnClick="AutoDistributionButtonClick" />
									</td>

							    </TR>
							    <tr>
								    <td style="HEIGHT: 3px"><FMCONTROLS:FMLABEL id="tankLabel" runat="server" BackColor="Transparent" Height="18px" CssClass="formfieldtitle"
										    Width="72px">Tank:</FMCONTROLS:FMLABEL></td>
								    <td style="HEIGHT: 3px" align="left">
									    <FMControls:FMTankTextBox id="tankTextBox" style="Z-INDEX: 112" tabIndex="3" runat="server" CssClass="formfield"
										    Width="169px" ontextchanged="TankTextBoxTextChanged"></FMControls:FMTankTextBox>
								    </td>
								    <td style="HEIGHT: 3px"></td>
								    <td style="HEIGHT: 3px"></td>
								    <td style="HEIGHT: 3px"></td>
								    <td style="HEIGHT: 3px"></td>
							    </tr>
						    </table>
					    </td>
				    <TR>
					    <TD><FMControls:FMDataGrid id="InventoryRecDataGrid" tabIndex="1" runat="server" BackColor="White" CssClass="tabletext"
							    Width="718px" PageSize="31" HorizontalAlign="Left" GridLines="Vertical" CellPadding="1" BorderWidth="1px"
							    BorderColor="#999999" BorderStyle="None" aria-label="Inventory Records">
							    <SelectedItemStyle ForeColor="White" CssClass="tablelink" BackColor="SteelBlue"></SelectedItemStyle>
							    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							    <HeaderStyle Font-Bold="True" Wrap="False" ForeColor="White" CssClass="tablecolheadcentered"
								    BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							    <Columns>
								    <asp:TemplateColumn HeaderText="Closeout">
									    <HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
									    <ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									    <ItemTemplate>
										    <FMControls:FMButton runat="server" CssClass="tabletext" ID="CloseoutButton" Text="Closeout" style="Width:60px;height:20px;"
											    CommandName="Closeout" />
									    </ItemTemplate>
								    </asp:TemplateColumn>
							    </Columns>
							    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						    </FMControls:FMDataGrid></TD>
				    </TR>
			    </TABLE>
            </div>
		</form>
	</body>
</HTML>
