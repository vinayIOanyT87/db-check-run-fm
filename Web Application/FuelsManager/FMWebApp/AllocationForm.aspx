<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="AllocationForm.aspx.cs" Inherits="FuelsManager.FMWebApp.AllocationForm" %>
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
	<body tabIndex="-1" MS_POSITIONING="GridLayout">
		<SCRIPT>
			var theMoment = new Date();
			var theDisplacement = (theMoment.getTimezoneOffset() / 60);
			document.cookie="Displacement="+theDisplacement;
		</SCRIPT>
		<form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position:absolute">
			<asp:image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image>
			<FMCONTROLS:FMBUTTON id="RefreshButton" style="Z-INDEX: 131; LEFT: 719px; POSITION: absolute; TOP: 67px"
				tabIndex="9" runat="server" Width="67px" CssClass="formfieldtitle" Text="Refresh"></FMCONTROLS:FMBUTTON>
			<FMCONTROLS:FMBUTTON id="ResetButton" style="Z-INDEX: 100; LEFT: 720px; POSITION: absolute; TOP: 128px"
				tabIndex="10" runat="server" Width="66px" CssClass="formfieldtitle" Text="Reset"></FMCONTROLS:FMBUTTON>
			<asp:DropDownList id="AllocationGroupsDropDownList" style="Z-INDEX: 128; LEFT: 152px; POSITION: absolute; TOP: 69px; right: 626px;"
				tabIndex="2" runat="server" Width="176px" CssClass="formfield" AutoPostBack="True"></asp:Dropdownlist>
			<FMCONTROLS:FMLABEL id="Label21" 
				style="Z-INDEX: 127; LEFT: 8px; POSITION: absolute; TOP: 69px; height: 19px;" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Allocation Group:</FMCONTROLS:FMLABEL>
			<asp:textbox id="ContractNumberTextbox" style="Z-INDEX: 126; LEFT: 496px; POSITION: absolute; TOP: 158px"
				tabIndex="8" runat="server" Width="202px" CssClass="formfield" MaxLength="10"></asp:textbox>
			<FMCONTROLS:FMLABEL id="Label20" 
				style="Z-INDEX: 125; LEFT: 336px; POSITION: absolute; TOP: 158px" runat="server"
				BackColor="Transparent" Width="140px" CssClass="formfieldtitle">Contract Number:</FMCONTROLS:FMLABEL>
			<asp:textbox id="LastAllocationDateTextbox" style="Z-INDEX: 124; LEFT: 496px; POSITION: absolute; TOP: 128px"
				tabIndex="7" runat="server" Width="160px" CssClass="formfield" MaxLength="20" 
				Enabled="False"></asp:textbox><FMCONTROLS:FMLABEL id="Label19" 
				style="Z-INDEX: 123; LEFT: 336px; POSITION: absolute; TOP: 128px" runat="server"
				BackColor="Transparent" Width="157px" CssClass="formfieldtitle" Height="24px">Last Allocation Reset Date:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="Label7" 
				style="Z-INDEX: 122; LEFT: 8px; POSITION: absolute; TOP: 158px" runat="server"
				BackColor="Transparent" Width="120px" CssClass="formfieldtitle">Line Items:</FMCONTROLS:FMLABEL>
			<asp:label id="Label6" 
				style="Z-INDEX: 121; LEFT: 200px; POSITION: absolute; TOP: 130px" runat="server"
				BackColor="Transparent" Width="16px" CssClass="formfieldtitle">%</asp:label>
			<asp:textbox id="LoadDenialTextbox" style="Z-INDEX: 120; LEFT: 152px; POSITION: absolute; TOP: 128px"
				tabIndex="4" runat="server" Width="32px" CssClass="formfield" MaxLength="20"></asp:textbox>
			<FMCONTROLS:FMLABEL id="Label4" 
				style="Z-INDEX: 119; LEFT: 8px; POSITION: absolute; TOP: 128px" runat="server"
				BackColor="Transparent" Width="96px" CssClass="formfieldtitle">Load Denial:</FMCONTROLS:FMLABEL>
			<asp:label id="Label3" 
				style="Z-INDEX: 118; LEFT: 200px; POSITION: absolute; TOP: 100px" runat="server"
				BackColor="Transparent" Width="16px" CssClass="formfieldtitle">%</asp:label>
			<asp:textbox id="LoadWarningTextbox" style="Z-INDEX: 117; LEFT: 152px; POSITION: absolute; TOP: 98px"
				tabIndex="3" runat="server" Width="32px" CssClass="formfield" MaxLength="20"></asp:textbox>
			<FMCONTROLS:FMLABEL id="Label2" 
				style="Z-INDEX: 116; LEFT: 8px; POSITION: absolute; TOP: 100px" runat="server"
				BackColor="Transparent" Width="96px" CssClass="formfieldtitle">Load Warning:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMDATE id="ExpirationDate" style="Z-INDEX: 130; LEFT: 496px; POSITION: absolute; TOP: 98px"
				tabIndex="6" runat="server" Width="160px" CssClass="formfield" MaxLength="20"></FMCONTROLS:FMDATE>
			<FMCONTROLS:FMLABEL id="Label17" 
				style="Z-INDEX: 112; LEFT: 336px; POSITION: absolute; TOP: 98px" runat="server"
				BackColor="Transparent" Width="96px" CssClass="formfieldtitle">Expiration Date:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMDATE id="EffectiveDate" style="Z-INDEX: 131; LEFT: 496px; POSITION: absolute; TOP: 69px"
				tabIndex="5" runat="server" Width="160px" CssClass="formfield" MaxLength="20"></FMCONTROLS:FMDATE>
			<FMCONTROLS:FMLABEL id="Label12" 
				style="Z-INDEX: 109; LEFT: 336px; POSITION: absolute; TOP: 69px" runat="server"
				BackColor="Transparent" Width="104px" CssClass="formfieldtitle">Effective Date:</FMCONTROLS:FMLABEL>
			<asp:DropDownList id="CompanyAssignedDropDownList" style="Z-INDEX: 108; LEFT: 152px; POSITION: absolute; TOP: 40px; width: 174px; right: 764px;"
				tabIndex="1" runat="server" Width="176px" CssClass="formfield" AutoPostBack="True"></asp:DropDownList>
			<asp:DropDownList id="CompanyAssignedToDropDownList" style="Z-INDEX: 108; POSITION: absolute; TOP: 40px; width: 290px; right: 303px; left: 497px;"
				tabIndex="1" runat="server" CssClass="formfield" AutoPostBack="True"></asp:DropDownList>
			<FMCONTROLS:FMLABEL id="HierarchyLabel" 
				style="Z-INDEX: 107; LEFT: 335px; POSITION: absolute; TOP: 40px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Hierarchy:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMLABEL id="Label1" style="Z-INDEX: 107; LEFT: 8px; POSITION: absolute; TOP: 40px" runat="server"
				BackColor="Transparent" CssClass="formfieldtitle">Company:</FMCONTROLS:FMLABEL>
			<FMCONTROLS:FMBUTTON id="Cancel" 
				style="Z-INDEX: 106; LEFT: 688px; POSITION: absolute; TOP: 468px;" tabIndex="12"
				runat="server" Width="67px" CssClass="formfieldtitle" Text="Cancel"></FMCONTROLS:FMBUTTON>
			<FMCONTROLS:FMBUTTON id="OK" 
				style="Z-INDEX: 105; LEFT: 608px; POSITION: absolute; TOP: 468px" tabIndex="11"
				runat="server" Width="67px" CssClass="formfieldtitle" Text="OK"></FMCONTROLS:FMBUTTON>
			<FMCONTROLS:FMLABEL id="Label10" 
				style="Z-INDEX: 103; LEFT: 600px; POSITION: absolute; TOP: 499px" runat="server"
				Width="144px" CssClass="formfieldtitle" Height="8px" ForeColor="Crimson">* Denotes Required Field</FMCONTROLS:FMLABEL><FMCONTROLS:FMLABEL id="Label5" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				BackColor="Transparent" Width="368px" CssClass="headline">Allocation Configuration</FMCONTROLS:FMLABEL>
			<TABLE id="Table1" style="Z-INDEX: 113; LEFT: 8px; WIDTH: 238px; POSITION: absolute; TOP: 185px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" width="238" border="0">
				<TR>
					<TD style="WIDTH: 732px; HEIGHT: 10px">
						<FMCONTROLS:FMDATAGRID id="LineItemsDataGrid" tabIndex="9" runat="server" 
							BackColor="White" Width="736px"
							CssClass="tabletext" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" 
							BorderWidth="1px" AllowSorting="True" BorderColor="White"
							CellPadding="3" AllowPaging="True" PageSize="8">
							<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
							<EditItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
								Font-Strikeout="False" Font-Underline="False" Wrap="False" />
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C" 
								Font-Italic="False" Font-Overline="False" Font-Strikeout="False" 
								Font-Underline="False" Wrap="False"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro" Font-Bold="False" 
								Font-Italic="False" Font-Overline="False" Font-Strikeout="False" 
								Font-Underline="False" Wrap="False"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE" Font-Bold="False" 
								Font-Italic="False" Font-Overline="False" Font-Strikeout="False" 
								Font-Underline="False" Wrap="False"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" />
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMUpdateLinkButton runat="server" />
										<FMControls:FMCancelLinkButton runat="server" />
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Index">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Type">
									<ItemTemplate>
										<FMControls:FMLabel width=.8in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Type") %>' ID="Label9">
										</FMControls:FMLabel>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMDropDownList width=.8in CssClass=tabletext runat="server" Enabled="True" ID="TypeDropDownList" DataSource="<%# EnumerateTypes()%>" DataTextField="Text" DataValueField="Value" AutoPostBack="True" OnSelectedIndexChanged="TypeDropDownList_SelectedIndexChanged">
										</FMControls:FMDropDownList>
									</EditItemTemplate>
									<HeaderStyle Width="0.5in" />
									<ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
										Font-Strikeout="False" Font-Underline="False" Wrap="False" />
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="ID">
									<ItemTemplate>
										<asp:Label width=.65in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ID="Label8">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:dropdownlist width=.65in CssClass=tabletext runat="server" Enabled="True" ID="IDDropDownList" DataSource="<%# EnumerateIDs()%>" DataTextField="Text" DataValueField="Value">
										</asp:dropdownlist>
									</EditItemTemplate>
									<HeaderStyle Width="2in" />
									<ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
										Font-Strikeout="False" Font-Underline="False" Wrap="False" />
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Limit">
									<ItemTemplate>
										<asp:Label width=.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Limit") %>' ID="Label13">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox width=.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Limit") %>' CssClass="tabletext" ID="LimitTextbox">
										</asp:TextBox>
									</EditItemTemplate>
									<ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
										Font-Strikeout="False" Font-Underline="False" Wrap="False" />
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Loaded">
									<ItemTemplate>
										<asp:Label width=.4in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Loaded") %>' ID="Label14">
										</asp:Label>
									</ItemTemplate>
									<ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
										Font-Strikeout="False" Font-Underline="False" Wrap="False" />
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Next">
									<ItemTemplate>
										<asp:Label width=.4in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Next") %>' ID="Label15">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox width=.4in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Next") %>' CssClass="tabletext" ID="NextTextbox">
										</asp:TextBox>
									</EditItemTemplate>
									<ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
										Font-Strikeout="False" Font-Underline="False" Wrap="False" />
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Period">
									<ItemTemplate>
										<FMControls:FMLabel width=.6in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ResetPeriod") %>' ID="Label11">
										</FMControls:FMLabel>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMDropDownList width=.6in CssClass=tabletext runat="server" Enabled="True" ID="ResetPeriodDropDownList" DataSource="<%# EnumerateResetPeriods()%>" DataTextField="Text" DataValueField="Value">
										</FMControls:FMDropDownList>
									</EditItemTemplate>
									<ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
										Font-Strikeout="False" Font-Underline="False" Wrap="False" />
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Multiple">
									<ItemTemplate>
										<asp:Label width=.4in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ResetMultiple") %>' ID="Label16">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox width=.4in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ResetMultiple") %>' CssClass="tabletext" ID="ResetMultipleTextbox">
										</asp:TextBox>
									</EditItemTemplate>
									<ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
										Font-Strikeout="False" Font-Underline="False" Wrap="False" />
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Method">
									<ItemTemplate>
										<FMControls:FMLabel width=.9in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ResetMethod") %>' ID="Label18" >
										</FMControls:FMLabel>
									</ItemTemplate>
									<EditItemTemplate>
										<FMcontrols:FMDropDownList width=.9in CssClass=tabletext runat="server" Enabled="True" ID="ResetMethodDropDownList" DataSource="<%# EnumerateResetMethods()%>" DataTextField="Text" DataValueField="Value" AutoPostBack="True" OnSelectedIndexChanged="MethodDropDownList_SelectedIndexChanged">
										</FMcontrols:FMDropDownList>
									</EditItemTemplate>
									<ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
										Font-Strikeout="False" Font-Underline="False" Wrap="False" />
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Reset Date">
									<ItemStyle Wrap="False"></ItemStyle>
									<ItemTemplate>
										<asp:Label width=.65in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ResetDate") %>' ID="ResetDateLabel">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.4in" />
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" CssClass="tablepager" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDATAGRID></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 732px; HEIGHT: 10px"><FMCONTROLS:FMBUTTON id="AddButton" Width="66px" tabIndex="10" runat="server" CssClass="formfieldtitle" Text="Add"></FMCONTROLS:FMBUTTON></TD>
				</TR>
			</TABLE>
			<script>
			    var okButton = document.getElementById("OK");
			    if (!okButton.disabled)
			        okButton.setActive();
			</script>
		</div>
</form>
	</body>
</HTML>
