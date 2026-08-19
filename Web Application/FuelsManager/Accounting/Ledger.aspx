<%@ Register TagPrefix="fmcontrols" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="Ledger.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.Accounting.Ledger" enableViewState="True"%>
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

	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<SCRIPT>
			    function CompanySelect(role, companyTextBoxId) {
			        var companyTextBox = document.getElementById(companyTextBoxId);

			        showModalDialogFrame({
			            url: '../FMWebApp/CompanySelectForm.aspx?Role=' + role + '',
			            width: 855,
			            height: 710,
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

			    function ProductSelect(productTextBoxId) {
			        var productTextBox = document.getElementById(productTextBoxId);

			        showModalDialogFrame({
			            url: '../FMWebApp/ProductSelectForm.aspx?Type=MaxProduct' + '&Map=MAX_MAP',
			            width: 855,
			            height: 710,
			            title: "Product Select",
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
			</SCRIPT>
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position:absolute;height: 100%;margin-left: 15px;">
			    <asp:image id="FadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				    ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
			    <FMCONTROLS:FMCHECKBOX id="FinanceCheckBox" style="Z-INDEX: 136; LEFT: 634px; POSITION: absolute; TOP: 56px"
				    tabIndex="22" runat="server" BackColor="Transparent" CssClass="formfieldtitle" 
				    Width="88px" Text="Show cost">			    
			    </FMCONTROLS:FMCHECKBOX>
				<FMCONTROLS:FMPRODUCTTEXTBOX id="ProductTextBox" style="Z-INDEX: 113; LEFT: 88px; POSITION: absolute; TOP: 48px"
				    tabIndex="4" runat="server" AutoPostBack="True" CssClass="formfield" Width="169px">
				</FMCONTROLS:FMPRODUCTTEXTBOX>
				<FMCONTROLS:FMCOMPANYTEXTBOX id="ManagerTextBox" style="Z-INDEX: 112; LEFT: 368px; POSITION: absolute; TOP: 16px"
				    tabIndex="3" runat="server" AutoPostBack="True" CssClass="formfield" Width="169px" Role="MANAGER">
				</FMCONTROLS:FMCOMPANYTEXTBOX>
				<FMCONTROLS:FMBaseDataGrid id="ledgerDataGrid" UseAccessibleHeader="True" style="Z-INDEX: 110; LEFT: 8px; POSITION: absolute; TOP: 112px"
				    tabIndex="7" runat="server" BackColor="White" CssClass="tabletext" Width="718px" BorderStyle="None" GridLines="Vertical" BorderWidth="1px" AllowSorting="False" BorderColor="#999999" CellPadding="1" PageSize="31" HorizontalAlign="Left">
				    <FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
				    <SelectedItemStyle Font-Bold="True" ForeColor="White" CssClass="tablelink" BackColor="#008A8C"></SelectedItemStyle>
				    <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
				    <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				    <HeaderStyle Font-Bold="True" Wrap="False" ForeColor="White" CssClass="tablecolheadcentered"
					    BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
				    <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
			    </FMCONTROLS:FMBaseDataGrid>
				<asp:label id="dateTypeDropDownLabel" style="Z-INDEX: 105; left: 288px; position: absolute; top: 50px;"
					runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="72px" 
					Height="18px">Date Type:</asp:label>
				<!-- item values are populated by code behind page -->
				<asp:DropDownList ID="DateTypeDropDownList" runat="server" 
					style="z-index:110; position:absolute; left:369px; top:48px; width:168px" 
					CssClass="formfield" onselectedindexchanged="DateTypeDropDownListSelectedIndexChanged">
					<asp:ListItem Value="0"></asp:ListItem>
					<asp:ListItem Value="2"></asp:ListItem>
					<asp:ListItem Value="3"></asp:ListItem>
					<asp:ListItem Value="4"></asp:ListItem>
					<asp:ListItem Value="1"></asp:ListItem>
				</asp:DropDownList>
			    <FMCONTROLS:FMLabel ID="ViewLabel" AssociatedControlID="ViewDropDownList" runat="server" Text="View:" style="z-index:110; position:absolute; left:8px; top:80px; width:72px" CssClass="formfieldtitle"/>
			    <fmcontrols:FMDropDownList ID="QuantityDropDownList" runat="server" 
				    style="z-index:110; position:absolute; left:369px; top:80px; width:168px" 
				    CssClass="formfield" onselectedindexchanged="QuantityDropDownListSelectedIndexChanged">
				    <asp:ListItem Value="0">Gross and Net</asp:ListItem>
				    <asp:ListItem Value="1">Gross</asp:ListItem>
				    <asp:ListItem Value="2">Net</asp:ListItem>
				    <asp:ListItem Value="3">Mass</asp:ListItem>
				    <asp:ListItem Value="4">Package</asp:ListItem>
			    </fmcontrols:FMDropDownList>
			    <asp:DropDownList ID="ViewDropDownList" runat="server" style="z-index:110; position:absolute; left:88px; top:80px; width:168px" CssClass="formfield" />
			    <FMCONTROLS:FMLabel ID="ViewSelection" runat="server" Text="View:" style="z-index:110; position:absolute; left:88px; top:80px; width:168px; visibility:hidden;" CssClass="formfield" />
			    <FMControls:FMButton id="refreshButton" style="Z-INDEX: 107; LEFT: 639px; POSITION: absolute; TOP: 16px; min-width: 100px;"
				    tabIndex="6" runat="server" CssClass="formfieldtitle" Text="Refresh" 
				    onclick="RefreshButtonClick"/>
				<FMControls:FMButton id="MovementButton" style="Z-INDEX: 107; LEFT: 639px; POSITION: absolute; TOP: 80px"
					tabIndex="6" runat="server" CssClass="formfieldtitle" Text="Movement Calendar" visible="false"
					onclick="MovementButtonClick"/>
			    <FMCONTROLS:FMLabel id="ownerLabel0" AssociatedControlID="QuantityDropDownList" style="Z-INDEX: 105; LEFT: 288px; POSITION: absolute; TOP: 81px; right: 622px;"
				    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="72px" 
				    Height="18px">Quantity:</FMCONTROLS:FMLabel>
                <FMCONTROLS:FMLabel id="ownerLabel" style="Z-INDEX: 105; LEFT: 288px; POSITION: absolute; TOP: 48px"
				    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="72px" Height="18px">Owner:</FMCONTROLS:FMLabel>
                <FMCONTROLS:FMLabel id="productLabel" style="Z-INDEX: 104; LEFT: 8px; POSITION: absolute; TOP: 48px"
				    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="72px" Height="18px">Product:</FMCONTROLS:FMLabel>
                <FMCONTROLS:FMLabel id="managerLabel" style="Z-INDEX: 102; LEFT: 288px; POSITION: absolute; TOP: 16px"
				    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="78px" Height="18px">Manager:</FMCONTROLS:FMLabel>
                <FMCONTROLS:FMLabel id="monthLabel" AssociatedControlID="monthDropdown" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 16px" runat="server"
				    BackColor="Transparent" CssClass="formfieldtitle" Width="63px" Height="18px">Month:</FMCONTROLS:FMLabel><asp:dropdownlist id="monthDropdown" style="Z-INDEX: 111; LEFT: 88px; POSITION: absolute; TOP: 16px"
				    tabIndex="1" runat="server" CssClass="formfield" Width="168px" onselectedindexchanged="MonthSelectionChange"></asp:dropdownlist><FMCONTROLS:FMCOMPANYTEXTBOX id="OwnerTextBox" style="Z-INDEX: 108; LEFT: 368px; POSITION: absolute; TOP: 48px"
				    tabIndex="4" runat="server" AutoPostBack="True" CssClass="formfield" Width="169px" Role="OWNER"></FMCONTROLS:FMCOMPANYTEXTBOX>
            </div>
        </form>
		<script language="jscript">
			document.getElementById("monthDropdown").setActive();
			document.getElementById("monthDropdown").focus();
		</script>
	</body>
</HTML>
