<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EquipmentCompartmentsPage.ascx.cs" Inherits="FuelsManager.FMWebApp.EquipmentCompartmentsPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

<TABLE id=Table1 style="Z-INDEX: 101; LEFT: 0px; WIDTH: 238px; POSITION: absolute; TOP: 40px; HEIGHT: 10px"
	cellSpacing=0 cellPadding=1 width=238 border=0>
	<TBODY>
		<TR>
			<TD><FMControls:FMDataGrid id=CompartmentsDataGrid runat="server" CssClass="tabletext" BackColor="White" PageSize="5" RowHeaderColumn="Number"
					AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical"
					AutoGenerateColumns="False" BorderStyle="None" Width="392px">
					<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
					<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
					<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
					<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
					<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
					<Columns>
					   <asp:TemplateColumn HeaderText="Edit">
						   <HeaderStyle Wrap="False" Width="0.5in"></HeaderStyle>
							<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<FMControls:FMEditLinkButton runat="server" ID="EditButton" />
							</ItemTemplate>
							<EditItemTemplate>
			               <FMControls:FMUpdateLinkButton runat="server" ID="Fmupdatelinkbutton1" />&nbsp;
                        <FMControls:FMCancelLinkButton runat="server" ID="Fmcancellinkbutton1" />
                      </EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn Visible="False" HeaderText="Index">
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
								</asp:Label>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Number">
							<HeaderTemplate>
								Number <span style="color:Red">*</span>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Number") %>' ID="NumberLabel" />
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Number") %>' ID="txtNumber" CssClass="tabletext" aria-required="true"/>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Capacity">
							<ItemTemplate>
								<asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Capacity") %>' ID="Label1" NAME="Label1">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox ID="CapacityTextBox" CssClass="tabletext" Width= ".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Capacity") %>'>
								</asp:TextBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Safe Fill">
							<ItemTemplate>
								<asp:Label Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SafeFill") %>' ID="Label2" NAME="Label2">
								</asp:Label>
							</ItemTemplate>
							<EditItemTemplate>
								<asp:TextBox ID="SafeFillTextBox" CssClass=tabletext Width=".5in" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SafeFill") %>'>
								</asp:TextBox>
							</EditItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Delete">
							<HeaderStyle Width="0.5in"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
							<ItemTemplate>
								<FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" 
								Text="&lt;img src=Images/Delete.gif border=0 align=absmiddle alt='Delete this item'&gt;"  />
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
					<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
				</FMControls:FMDataGrid></TD>
		</TR>
		<TR>
			<TD height=21><FMControls:FMButton id=AddButton runat="server" CssClass="formfieldtitle" Text="Add"></FMControls:FMButton></TD>
		</TR>
	</TBODY>
</TABLE>

