<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="PIDXProfileCompaniesPage.ascx.cs" Inherits="FuelsManager.FMWebApp.PIDXProfileCompaniesPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
<SCRIPT>
	function CompanySelect(role, companyTextBoxId)
	{
	    var companyTextBox = document.getElementById(companyTextBoxId);

		showModalDialogFrame({
		    url: "../FMWebApp/CompanySelectForm.aspx?Mode=PIDXProfile",
		    width: 855,
		    height: 560,
		    title: "Company Select",
		    onClose: function ()
		    {
		        var theform;

		        if (this.returnValue != null)
		        {
		            var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
		            var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

		            companyTextBox.value = asciiValue1;
		            companyTextBox.title = asciiValue2;

		            if (window.navigator.appName.toLowerCase().indexOf("microsoft") > -1)
		            {
		                theform = document.Form1;
		            }
		            else
		            {
		                theform = document.forms["Form1"];
		            }

		            theform.submit();
		        }
		    }
		});
	}
</SCRIPT>
<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 0px; WIDTH: 740px; POSITION: absolute; TOP: 0px; HEIGHT: 10px"
	cellSpacing="0" cellPadding="1" border="0" role="presentation" aria-label="layout">
	<tr>
		<TD style="WIDTH: 740px" vAlign="middle" height="36"><FMCONTROLS:FMBUTTON id="AddButton2" runat="server" CssClass="formfieldtitle" Text="Add" width="100px"></FMCONTROLS:FMBUTTON>&nbsp;&nbsp;
			<FMCONTROLS:FMPAGESIZEDROPDOWN id="PIDXProfileCompaniesPageSizeDropDown" ToolTip="Page size" runat="server" onselectedindexchanged="PageSizeDropDown_SelectedIndexChanged"></FMCONTROLS:FMPAGESIZEDROPDOWN></TD>
	</tr>
	<TR>
		<TD style="WIDTH: 740px; HEIGHT: 10px">
			<div style="height:370px; overflow-y:scroll">
             <FMCONTROLS:FMDATAGRID id="PIDXProfileCompaniesDataGrid" style="LEFT: 1px; TOP: 0px" tabIndex="3" runat="server" RowHeaderColumn="Ship To Company"
				CssClass="tabletext" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" Width="740px" GridLines="Vertical"
				AutoGenerateColumns="False" BackColor="White" BorderStyle="None" aria-label="PIDX Profile Companies">
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
							<FMControls:FMEditLinkButton runat="server" ID="Fmeditlinkbutton1" />
						</ItemTemplate>
						<EditItemTemplate>
							<FMControls:FMUpdateLinkButton runat="server" ID="Fmupdatelinkbutton1" />&nbsp;
							<FMControls:FMCancelLinkButton runat="server" ID="Fmcancellinkbutton1" />
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Ship To Company">
						<HeaderStyle Width="2.5in"></HeaderStyle>
						<ItemStyle Wrap="False"></ItemStyle>
						<ItemTemplate>
							<asp:Label Width="2.4in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipToID") %>' ID="CompanyLabel">
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<FMControls:FMCompanyTextBox Width="2.0in" CssClass="tabletext" runat="server" ID="ShipToTextBox" ToolTip="Company" ReadOnly="False" AutoPostBack="False" OnTextChanged="ItemShipToTextBox_TextChanged">
							</FMControls:FMCompanyTextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Load ID">
						<ItemTemplate>
							<FMControls:FMLabel width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ID="Label9">
							</FMControls:FMLabel>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList width=".5in" CssClass="tabletext" runat="server" Enabled="True" ID="LoadIDDropDownList" ToolTip="Load ID"></asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Consignee Number">
						<HeaderStyle Width=".7in"></HeaderStyle>
						<ItemTemplate>
							<asp:Label Width=".7in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ConsigneeNumber") %>' ID="ConsigneeNumberLabel">
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox Width=".8in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ConsigneeNumber") %>' CssClass="tabletext" ID="ConsigneeNumberTextBox" ToolTip="Consignee Number" MaxLength=14>
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Seller ID">
						<HeaderStyle Width=".3in"></HeaderStyle>
						<ItemTemplate>
							<asp:Label Width=".3in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SellerID") %>' ID="SellerIDLabel">
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox Width=".4in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SellerID") %>' CssClass="tabletext" ID="SellerIDTextBox" ToolTip="Seller ID" MaxLength=3>
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Shipper ID">
						<HeaderStyle Width=".3in"></HeaderStyle>
						<ItemTemplate>
							<asp:Label Width=".3in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipperID") %>' ID="ShipperIDLabel">
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox Width=".4in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ShipperID") %>' CssClass="tabletext" ID="ShipperIDTextBox" ToolTip="Shipper ID" MaxLength=3>
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Denial Override">
						<HeaderStyle Width=".3in"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						<ItemTemplate>
							<asp:CheckBox runat="server" CssClass=tabletext Checked='<%# DataBinder.Eval(Container, "DataItem.DenialOverride") %>' ID="DenialOverrideCheckbox">
							</asp:CheckBox>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Unavailable Override">
						<HeaderStyle Width=".3in"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						<ItemTemplate>
							<asp:CheckBox runat="server" CssClass=tabletext Checked='<%# DataBinder.Eval(Container, "DataItem.UnavailableOverride") %>' ID="UnavailableOverrideCheckbox">
							</asp:CheckBox>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Delete">
						<HeaderStyle Width="0.5in"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						<ItemTemplate>
							<FMControls:FMDeleteLinkButton runat="server" ID="Fmdeletelinkbutton1" NAME="Fmdeletelinkbutton1" />
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
			</FMCONTROLS:FMDATAGRID>
			</div>
		</TD>
	</TR>
	<TR>
		<TD style="WIDTH: 740px; HEIGHT: 50px" vAlign="middle"><FMCONTROLS:FMBUTTON id="AddButton" tabIndex="4" runat="server" CssClass="formfieldtitle" Text="Add"
				Width="98px"></FMCONTROLS:FMBUTTON></TD>
	</TR>
</TABLE>
