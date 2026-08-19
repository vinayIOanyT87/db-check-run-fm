<%@ Page language="c#" Codebehind="AllocationsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.AllocationsForm"  %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
  </HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<SCRIPT>
		    function CompanySelect(role, companyTextBoxId)
		    {
		        var companyTextBox = document.getElementById(companyTextBoxId);

		        showModalDialogFrame({
		            url: "../FMWebApp/CompanySelectForm.aspx?All=true",
		            width: 855,
		            height: 690,
		            title: "Company Select",
		            onClose: function ()
		            {
		                if (this.returnValue != null)
		                {
		                    var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
		                    var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

		                    companyTextBox.value = asciiValue1;
		                    companyTextBox.title = asciiValue2;
		                    companyTextBox.onchange();
		                }
		            }
		        });
		    }
		</SCRIPT>
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position: absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="Label2" Style="z-index: 102; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" Width="256px" BackColor="Transparent">Allocations Configuration</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label" AssociatedControlID="CompanyMapTypeDropDownList" Style="z-index: 103; left: 32px; position: absolute; top: 40px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent">Type:</FMControls:FMLabel>
			<FMControls:FMLabel ID="Label3" AssociatedControlID="AllocationGroupsDropDownList" Style="z-index: 105; left: 288px; position: absolute; top: 40px" runat="server"
				CssClass="formfieldtitle" BackColor="Transparent">Allocation Group:</FMControls:FMLabel>
			<FMControls:FMLabel ID="CompanyLabel" AssociatedControlID="CompanyTextBox" Style="z-index: 109; left: 496px; position: absolute; top: 40px" runat="server" CssClass="formfieldtitle" BackColor="Transparent">Company:</FMControls:FMLabel>
			<FMControls:FMDropDownList ID="CompanyMapTypeDropDownList" Style="z-index: 104; left: 32px; position: absolute; top: 72px"
				runat="server" CssClass="formfield" Width="248px" AutoPostBack="True" TabIndex="1" OnSelectedIndexChanged="CompanyRoleDropDownList_SelectedIndexChanged">
			</FMControls:FMDropDownList>
			<asp:DropDownList ID="AllocationGroupsDropDownList" Style="z-index: 107; left: 288px; position: absolute; top: 72px"
				runat="server" CssClass="formfield" Width="192px" AutoPostBack="True" TabIndex="2" OnSelectedIndexChanged="AllocationGroupsDropDownList_SelectedIndexChanged">
			</asp:DropDownList>
			<FMControls:FMCompanyTextBox Role="CARRIER" ID="CompanyTextBox" Style="z-index: 108; left: 488px; position: absolute; top: 72px"
				TabIndex="1" runat="server" Width="201px" CssClass="formfield" AutoPostBack="True" OnTextChanged="CompanyTextBox_TextChanged"></FMControls:FMCompanyTextBox>
			<table id="Table1" style="z-index: 101; left: 32px; width: 50%; position: absolute; top: 112px; height: 10px"
				cellspacing="0" cellpadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<td width="498" height="36" valign="middle">
						<FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="AllocationsFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDown_SelectedIndexChanged" />
					</td>
				</tr>
				<tr>
					<td style="width: 407px; height: 10px" width="407">
						<FMControls:FMDataGrid ID="AllocationsDataGrid" runat="server" BorderStyle="None" BackColor="White" AutoGenerateColumns="False" RowHeaderColumn="Company"
							GridLines="Vertical" Width="680px" BorderWidth="1px" AllowSorting="True" BorderColor="White" CellPadding="3"
							AllowPaging="True" CssClass="tabletext" Style="left: 1px; top: 0px" TabIndex="3" aria-label="Allocations">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
								<asp:BoundColumn DataField="CompanyMapAssignedID" HeaderText="Company"></asp:BoundColumn>
								<asp:BoundColumn DataField="CompanyMapAssignedToID" HeaderText="Hierarchy"></asp:BoundColumn>
								<asp:BoundColumn DataField="EffectiveDate" HeaderText="Effective Date"></asp:BoundColumn>
								<asp:BoundColumn DataField="ExpirationDate" HeaderText="Expiration Date"></asp:BoundColumn>
								<asp:BoundColumn DataField="AllocationGroupId" HeaderText="Group"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid></td>
				</tr>
				<tr>
					<td style="width: 407px; height: 50px" valign="middle" width="407">
						<FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
							TabIndex="4"></FMControls:FMButton></td>
				</tr>
			</table>
		</div>
	</form>
	<script type="text/javascript">
		var AddButton = document.getElementById("AddButton");

		if (!AddButton.disabled) {
			AddButton.setActive();
		}

		document.getElementById("CompanyMapTypeDropDownList").focus();

	</script>
</body>
</html>
