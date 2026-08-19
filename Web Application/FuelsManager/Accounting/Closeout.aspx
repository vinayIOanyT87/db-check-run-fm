<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls"%>
<%@ Page language="c#" Codebehind="Closeout.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.Closeout" %>
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
			 width: 298px;
		 }
		 .style2
		 {
			 height: 3px;
			 width: 298px;
		 }
	</style>
  </HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<SCRIPT>
				function CompanySelect(role, companyTextBoxId)
				{
				    var companyTextBox = document.getElementById(companyTextBoxId);

				    showModalDialogFrame({
				        url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "",
				        width: 855,
				        height: 560,
				        onClose: function ()
				        {
				            if (this.returnValue != null)
				            {
				                var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
				                var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

				                companyTextBox.value = asciiValue1;
				                companyTextBox.title = asciiValue2;
				            }
				        }
				    });
				}

				function ProductSelect(productTextBoxId)
				{
				    var productTextBox = document.getElementById(productTextBoxId);
				    var companyManagerTextBox = document.getElementById("ManagerTextBox");
				    var companyId = "";

				    if (companyManagerTextBox != null)
				    {
				        companyId = companyManagerTextBox.value + "|manager";
				    }

				    if (companyId.substr(0, 1) === "<")
				    {
				        companyId = "";
				    }

				    showModalDialogFrame({
				        url: "../FMWebApp/ProductSelectForm.aspx?Type=MaxProduct" +
					                                "&Map=MAX_MAP" + "&IDLink=" + encodeURIComponent(companyId),
				        width: 855,
				        height: 560,
				        onClose: function ()
				        {
				            if (this.returnValue != null)
				            {
				                var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
				                var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

				                productTextBox.value = asciiValue1;
				                productTextBox.title = asciiValue2;
				            }
				        }
				    });
				}
			</SCRIPT>
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute">
			    <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
				<span id="MainLabel" class="headline" style="display:inline-block;background-color:Transparent;width:296px;Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px">Closeout Summary</span>
			    <TABLE id="Table1" style="Z-INDEX: 118; LEFT: 5px; POSITION: absolute; TOP: 50px" role="presentation" aria-label="layout">
				    <TR width="350">
					    <td>
						    <table style="WIDTH: 700px; HEIGHT: 10px" role="presentation" aria-label="filter layout">
							    <TBODY>
								    <tr>
									    <TD style="HEIGHT: 1px"><FMControls:FMLabel id="FromDateLabel" runat="server" CssClass="formfieldtitle" Text="From Date">From Date</FMControls:FMLabel></TD>
									    <TD align="left" class="style1"><FMControls:FMDate id="FromDate" ToolTip="From Date" runat="server" CssClass="formfield" Width="136px"></FMControls:FMDate></TD>
									    <TD style="HEIGHT: 1px" align="left"><FMControls:FMLabel id="ManagerLabel" runat="server" CssClass="formfieldtitle" Text="Manager"></FMControls:FMLabel></TD>
									    <TD style="HEIGHT: 1px" align="left" nowrap="true"><FMControls:FMCompanyTextBox id="ManagerTextBox" ToolTip="Company" runat="server" AutoPostBack="True" Role="MANAGER" CssClass="formfield"
											    Width="200px" ontextchanged="ManagerTextBoxTextChanged"></FMControls:FMCompanyTextBox></TD>
									    <TD style="HEIGHT: 1px" align="left"><FMControls:FMLabel id="QuantityLabel" AssociatedControlID="QuantityDropDownList" runat="server" CssClass="formfieldtitle" Text="Quantity:"></FMControls:FMLabel></TD>
									    <TD align="left" rowSpan="1"><FMControls:FMDropDownList ID="QuantityDropDownList" runat="server" 
				    style="z-index:110;" 
				    CssClass="formfield" onselectedindexchanged="QuantityDropDownListSelectedIndexChanged" alt="Quantity Type">
				    <asp:ListItem Value="0">Gross and Net</asp:ListItem>
				    <asp:ListItem Value="1">Gross</asp:ListItem>
				    <asp:ListItem Value="2">Net</asp:ListItem>
				    <asp:ListItem Value="3">Mass</asp:ListItem>
				    <asp:ListItem Value="4">Package</asp:ListItem>
			    </FMControls:FMDropDownList></TD>

								    </tr>
								    <TR>
									    <TD style="HEIGHT: 3px">
										    <FMControls:FMLabel id="ToDateLabel" runat="server" CssClass="formfieldtitle" Text='To Date'></FMControls:FMLabel>
									    </TD>
									    <TD class="style2">
										    <FMControls:FMDate id="ToDate" ToolTip="To Date" runat="server" Width="136px" CssClass="formfield"></FMControls:FMDate>
									    </TD>
									    <TD style="HEIGHT: 3px">
										    <FMControls:FMLabel id="ProductLabel" AssociatedControlID="ProductTextBox" runat="server" CssClass="formfieldtitle" Text='Product'>Product</FMControls:FMLabel>
									    </TD>
									    <TD style="HEIGHT: 3px" nowrap="true">
										    <FMControls:FMProductTextBox id="ProductTextBox" runat="server" Width="200px" CssClass="formfield" AutoPostBack="True" ontextchanged="ProductTextBoxTextChanged" />
									    </TD>
									    <TD style="HEIGHT: 1px;" align="right" colspan="2">
										    <FMControls:FMButton id="RefreshButton" runat="server" CssClass="formfieldtitle" style="min-width: 100px" Text="Refresh" onclick="RefreshBtnOnClick" /></TD>
								    </TR>
								    <tr>
									    <TD style="HEIGHT: 3px">
										    <FMControls:FMPageSizeDropDown ID="CloseoutFormPageSizeDropDown" ToolTip="Page Size" runat="server" tabIndex="7" onselectedindexchanged="CloseoutFormPageSizeDropDownSelectedIndexChanged" alt="page size"/>
									    </TD>
								    </tr>
							    </TBODY>
						    </table>
					    </td>
				    </TR>
				    <tr>
					    <td>
						    <FMControls:FMDataGrid id="CloseoutDataGrid" CellPadding="3" tabIndex="21" runat="server" BackColor="White"
							    Width="664px" CssClass="tabletext" AutoGenerateColumns="False" AllowPaging="True" AllowSorting="True"
							    GridLines="Vertical" OnPageIndexChanged="CloseoutDataGrid_OnPageIndexChanged"  aria-label="Closeout">
							    <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>" />
							    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C" />
							    <AlternatingItemStyle BackColor="Gainsboro" />
							    <ItemStyle ForeColor="Black" BackColor="#EEEEEE" />
							    <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>" />
							    <Columns></Columns>
							    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages" />
						    </FMControls:FMDataGrid>
					    </td>
				    </tr>
			    </TABLE>
            </div>
		</form>
	</body>
</HTML>
