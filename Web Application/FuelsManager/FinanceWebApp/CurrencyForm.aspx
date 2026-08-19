<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="CurrencyForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FinanceWebApp.CurrencyForm" %>
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
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<asp:image id="FadeImage" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image><FMCONTROLS:FMLABEL id="labCurrencyConfig" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" BackColor="Transparent" CssClass="headline" Width="400px">Currency Configuration</FMCONTROLS:FMLABEL>
			<div style="Z-INDEX: 102; LEFT: 25px; POSITION: absolute; TOP: 50px">
				<table cellspacing="2" cellpadding="2" border="0">
					<tr>
						<td width="100"><FMCONTROLS:FMLABEL id="labName" AssociatedControlID="txtName" runat="server" CssClass="formfieldtitle">Name:</FMCONTROLS:FMLABEL></td>
						<td width="8" align="center"><FMCONTROLS:FMLABEL id="labRequired" runat="server" CssClass="formfieldtitle" ForeColor="Crimson">*</FMCONTROLS:FMLABEL></td>
						<td><asp:textbox id="txtName" CssClass="formfield" Runat="server" MaxLength="50" aria-required="true"></asp:textbox></td>
					</tr>
					<tr>
						<td width="100"><FMCONTROLS:FMLABEL id="labCountry" runat="server" CssClass="formfieldtitle">Country:</FMCONTROLS:FMLABEL></td>
						<td>&nbsp;</td>
						<td><asp:textbox id="txtCountry" CssClass="formfield" Runat="server" MaxLength="50"></asp:textbox></td>
					</tr>
					<tr>
						<td width="100"><FMCONTROLS:FMLABEL id="labUnit" runat="server" CssClass="formfieldtitle">Unit:</FMCONTROLS:FMLABEL></td>
						<td>&nbsp;</td>
						<td><asp:dropdownlist id="ddlUnit" CssClass="formfield" Runat="server"></asp:dropdownlist></td>
					</tr>
					<tr>
						<td width="100"><FMCONTROLS:FMLABEL id="labDisplay" runat="server" CssClass="formfieldtitle">Display:</FMCONTROLS:FMLABEL></td>
						<td>&nbsp;</td>
						<td><asp:checkbox id="chkDisplay" CssClass="formfield" Runat="server"></asp:checkbox></td>
					</tr>
				</table>
			</div>
			<div style="Z-INDEX: 103; LEFT: 25px; POSITION: absolute; TOP: 169px">
				<table cellSpacing="2" cellPadding="2" border="0">
					<tr>
						<td><asp:button id="btnAddTop" Runat="server" width="67px" Text="Add" 
                                onclick="BtnAddTopClick" CssClass="formfieldtitle"></asp:button></td>
					</tr>
					<tr><td></td></tr>
					<tr>
						<td><FMCONTROLS:FMDATAGRID id="dgLineItems" runat="server" BackColor="White" CssClass="tabletext" Width="400px"
								PageSize="16" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
								GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None" >
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
											<FMControls:FMEditLinkButton runat="server" id="btnEdit" CommandName="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdentityGuid") %>'/>
										</ItemTemplate>
										<EditItemTemplate>
											<FMControls:FMUpdateLinkButton runat="server" id="btnConfirm" CommandName="Update" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdentityGuid") %>' />&nbsp;
											<FMControls:FMCancelLinkButton runat="server" id="btnCancelLineItem" CommandName="Cancel" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdentityGuid") %>' />
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Effective Date">
										<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<%# DataBinder.Eval(Container.DataItem, "EffectiveDate", DateFormat) %>
										</ItemTemplate>
										<EditItemTemplate>
											<FMControls:FMDate id="dtExpirationDate" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "EffectiveDate", DateFormat) %>' CssClass="tabletext">
											</FMControls:FMDate>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<HeaderTemplate>Rate<span style="COLOR: red"> *</span></HeaderTemplate>
										<ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<%# DataBinder.Eval(Container.DataItem, "Rate") %>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox ID="txtRate" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Rate") %>' CssClass="tabletext" MaxLength="10" aria-required="true"/>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Delete">
										<HeaderStyle width="0.5in"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<FMControls:FMDeleteLinkButton runat="server" ID="btnDelete" NAME="btnDelete" />
										</ItemTemplate>
										<EditItemTemplate>
											<FMControls:FMDeleteLinkButton runat="server" ID="btnDeleteEdit" NAME="btnDeleteEdit" enabled="false" />
										</EditItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
							</FMCONTROLS:FMDATAGRID></td>
					</tr>
					<tr><td></td></tr>
					<tr>
						<td><asp:button id="btnAddBottom" Runat="server" width="67px" Text="Add" 
                                onclick="BtnAddBottomClick" CssClass="formfieldtitle"></asp:button></td>
					</tr>
					<tr>
						<td>&nbsp;</td>
					</tr>
					<tr>
						<td align="right"><FMCONTROLS:FMLABEL id="Label10" style="Z-INDEX: 107" runat="server"
				CssClass="formfieldtitle" Height="8px" ForeColor="Crimson" Width="176px">* Denotes Required Field</FMCONTROLS:FMLABEL>&nbsp;&nbsp;<asp:button id="btnOK" CssClass="formfieldtitle" Width="67px" Runat="server" Text="OK" onclick="BtnOkClick"></asp:button>&nbsp;
							<asp:Button ID="btnCancel" Runat="server" Text="Cancel" Width="67px" CssClass="formfieldtitle" onclick="BtnCancelClick" /></td>
					</tr>
				</table>
			</div>
		</div>
</form>
	</body>
</HTML>
