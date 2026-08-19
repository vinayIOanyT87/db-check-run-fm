<%@ Control language="c#" Codebehind="AccessibilityGeneralPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.AccessibilityGeneralPage"  TargetSchema="http://schemas.microsoft.com/intellisense/ie5"  %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
	    <div style="Z-INDEX: 103; width:700px; LEFT: 5px; POSITION: absolute; TOP: 5px; height: 300px;">
	
			<FMCONTROLS:FMDataGrid id="AccessibilityDataGrid" AllowPaging="True" PageSize="5"   
				runat="server" CssClass="tabletext" CellPadding="3" RowHeaderColumn="Feature"
				BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical" 
				AutoGenerateColumns="False" BackColor="White"
				BorderStyle="None" Height="350px" Width="790px" ShowFooter="false" aria-label="Accessibility settings">
	                   
				<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
 			
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C" Wrap="false" HorizontalAlign="Left"></SelectedItemStyle>
				
				<AlternatingItemStyle BackColor="Gainsboro" Wrap="false" HorizontalAlign="Left"></AlternatingItemStyle>
				
				<ItemStyle ForeColor="Black" BackColor="#EEEEEE" Wrap="false" HorizontalAlign="Left"></ItemStyle>

				<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>			
				<Columns>
					<asp:TemplateColumn HeaderText="Edit">
						<HeaderStyle  Width="10px"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						<ItemTemplate>
							<FMControls:FMEditLinkButton runat="server" />
						</ItemTemplate>
						<EditItemTemplate>
							<FMControls:FMUpdateLinkButton runat="server" />
							&nbsp;
							<FMControls:FMCancelLinkButton runat="server" />
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn Visible="False" HeaderText="UserGuid">
						<HeaderStyle></HeaderStyle>
						<ItemStyle></ItemStyle>
						<ItemTemplate>
							<asp:Label id="UserGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.UserGuid") %>'>
							</asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn Visible="False" HeaderText="AccessibilityConfigurationSettingGuid">
						<HeaderStyle></HeaderStyle>
						<ItemStyle></ItemStyle>
						<ItemTemplate>
							<asp:Label id="AccessibilitySettingGuidLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %>'>
							</asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Feature">
						<HeaderStyle Width="10px"></HeaderStyle>
						<ItemStyle></ItemStyle>
						<ItemTemplate>
							<asp:Label id="DisplayNameLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DisplayName") %>'></asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Value">
						<HeaderStyle Width="10px"></HeaderStyle>
						<ItemStyle></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container, "DataItem.SettingValue") %>
						</ItemTemplate>
						<EditItemTemplate>
							<FMControls:FMDropDownList id=SettingValueDropDown runat="server" Width="1in" CssClass="formfield" DataSource="<%# PopulateList()%>"  >
							</FMControls:FMDropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Description" ItemStyle-Wrap="true" ItemStyle-Width="40%" >
						<HeaderStyle Width="40%"></HeaderStyle>
						<ItemStyle Wrap="true" Width="40%"></ItemStyle>
						<ItemTemplate>
							<%# DataBinder.Eval(Container, "DataItem.Description") %>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
                   <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>"
                        Mode="NumericPages"></PagerStyle>
			</FMCONTROLS:FMDataGrid>
        </div>		

