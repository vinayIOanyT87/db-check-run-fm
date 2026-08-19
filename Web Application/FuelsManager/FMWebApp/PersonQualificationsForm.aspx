<%@ Page language="c#" Codebehind="PersonQualificationsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.PersonQualificationsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<html>
<head>
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body ms_positioning="GridLayout" tabindex="-1">
    <form id="Form1" method="post" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div style="position: absolute">
            <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
                BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
            <FMControls:FMLabel ID="Label3" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                CssClass="headline" Width="344px" BackColor="Transparent">Personnel Qualifications Configuration</FMControls:FMLabel>
            <table id="Table1" style="z-index: 100; left: 32px; width: 745px; position: absolute; top: 48px; height: 10px"
                cellspacing="0" cellpadding="1" border="0">
                <tr>
                    <td style="width: 745px; height: 36px" valign="middle">
                        <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
                            TabIndex="6" />
                        &nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="PersonQualificationsFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
                    </td>
                </tr>
                <TR>
					<TD style="WIDTH: 745px; HEIGHT: 10px" >
						<FMControls:FMDataGrid id="QualificationsDataGrid" runat="server" BorderStyle="None" BackColor="White" RowHeaderColumn="Qualification ID"
							AutoGenerateColumns="False" GridLines="Vertical" Width="745px" BorderWidth="1px" AllowSorting="True"
							BorderColor="White" CellPadding="3" AllowPaging="True" CssClass="tabletext" style="LEFT: 1px; TOP: 0px"
							PageSize="16">
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
                                    <EditItemTemplate>
                                        <FMControls:FMUpdateLinkButton runat="server" />&nbsp;
                                        <FMControls:FMCancelLinkButton runat="server" />
                                    </EditItemTemplate>
                                </asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Index">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Qualification ID">
									<HeaderStyle Width="2in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' width="2in">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="IDTextBox" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' width="2in" MaxLength="50" ToolTip="Personnel Qualifications ID" aria-required="true">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Description">
									<HeaderStyle Width="3in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label width=3in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' ID="Label4">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox width=3in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' CssClass="tabletext" ToolTip="Description" ID="DescriptionTextBox" MaxLength="50">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Recurrence (Days)">
									<HeaderStyle Width="1in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label width=1in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Reoccurrence") %>' ID="Label5">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox width=1in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Reoccurrence") %>' CssClass="tabletext" ToolTip="Recurrence in days" ID="ReoccurrenceTextbox" MaxLength="5">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMControls:FMDataGrid></TD>
				</TR>
				<TR>
                    <td style="width: 745px; height: 36px" valign="middle">
                        <FMControls:FMButton ID="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"></FMControls:FMButton></td>
                </TR>
			</TABLE>
		</div>
</form>
	</body>
</HTML>
