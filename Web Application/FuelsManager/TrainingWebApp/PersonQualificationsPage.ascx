<%@ Control language="c#" Codebehind="PersonQualificationsPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.TrainingWebApp.PersonQualificationsPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<TABLE id="Table1" style="Z-INDEX: 102; LEFT: 0px; WIDTH: 238px; POSITION: absolute; TOP: 16px; HEIGHT: 10px"
		cellSpacing="0" cellPadding="1" width="238" border="0">
		<TBODY>
			<TR>
				<TD><FMControls:FMDataGrid id="QualificationsDataGrid" runat="server" CssClass="tabletext" BackColor="White" RowHeaderColumn="Qualification ID"
						Width="320px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
						BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="8">
						<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
						<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
						<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
						<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
						<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
						<Columns>
							<asp:TemplateColumn HeaderText="Edit">
								<HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
								<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								<ItemTemplate>
									<FMControls:FMEditLinkButton runat="server" />
								</ItemTemplate>
								<EditItemTemplate>
			                        <FMControls:FMUpdateLinkButton runat="server" />&nbsp;
                                    <FMControls:FMCancelLinkButton runat="server" />
                                </EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn Visible="False" HeaderText="Index">
								<ItemTemplate>
									<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Qualification ID">
								<HeaderStyle Wrap="False"></HeaderStyle>
								<ItemStyle Wrap="False"></ItemStyle>
								<ItemTemplate>
									<asp:Label Width=1.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.QualificationID") %>' ID="Label11">
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:dropdownlist CssClass=tabletext runat="server" Enabled="True" ID="QualificationsDropDownList" DataSource="<%# EnumerateQualifications()%>" DataTextField="Text" DataValueField="Value">
									</asp:dropdownlist>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Number">
								<HeaderStyle Wrap="False"></HeaderStyle>
								<ItemStyle Wrap="False"></ItemStyle>
								<ItemTemplate>
									<asp:Label Width=1.0in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' ID="Label1">
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox Width=.8in CssClass=tabletext runat="server" Enabled="True" ID="NumberTextBox" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' DataTextField="Text" DataValueField="Value" MaxLength=50>
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Date Completed">
								<HeaderStyle Wrap="False"></HeaderStyle>
								<ItemStyle Wrap="False"></ItemStyle>
								<ItemTemplate>
									<asp:Label Width=.8in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DateCompleted") %>' ID="Label3">
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<FMControls:FMDate ID="DateCompleted"  CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DateCompleted") %>'>
									</FMControls:FMDate>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Due Date">
								<HeaderStyle Wrap="False"></HeaderStyle>
								<ItemStyle Wrap="False"></ItemStyle>
								<ItemTemplate>
									<asp:Label Width=.8in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DateDue") %>' ID="Label4">
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Expiration Date">
								<HeaderStyle Wrap="False"></HeaderStyle>
								<ItemStyle Wrap="False"></ItemStyle>
								<ItemTemplate>
									<asp:Label Width=1.0in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ExpirationDate") %>' ID="Label15">
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Delete">
								<HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
								<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								<ItemTemplate>
									<FMControls:FMDeleteLinkButton runat="server" />
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
						<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
					</FMControls:FMDataGrid></TD>
			</TR>
			<TR>
				<TD height="21"><FMControls:FMButton id="AddButton" runat="server" CssClass="formfieldtitle" Text="Add"></FMControls:FMButton></TD>
			</TR>
		</TBODY>
	</TABLE>
