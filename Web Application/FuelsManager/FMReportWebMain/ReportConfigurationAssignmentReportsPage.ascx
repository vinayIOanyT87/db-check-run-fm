<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportConfigurationAssignmentReportsPage.ascx.cs" Inherits="FuelsManager.FMReportWebMain.ReportConfigurationAssignmentReportsPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html >
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	    <style type="text/css">
            .style1
            {
                width: 78px;
            }
            .style2
            {
                width: 177px;
            }
            .style3
            {
                width: 160px;
            }
        </style>
	</HEAD>
	<body>
			<TABLE id="AssignmentTable" style="Z-INDEX: 101; LEFT: 16px; WIDTH: 17.66%; TOP: 56px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0" role="presentation" aria-label="layout">
				<tr>
					<TD width="646" height="36" vAlign="middle">
						<FMCONTROLS:FMButton id="AddReportButton2" runat="server" CssClass="formfieldtitle" Style="min-width:100px;" Text="Add Report" onclick="AddReportButtonOnClick" />
						&nbsp;&nbsp;
						<FMCONTROLS:FMButton id="AddGroupButton2" runat="server" CssClass="formfieldtitle" Style="min-width:120px;" Text="Add/Modify Groups" onclick="AddGroupButtonOnClick" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="ReportGroupsFormPageSizeDropDown" ToolTip="Page size" Style="float:right;" runat="server" tabIndex="7" onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />
					</TD>
				</tr>
				<TR>
					<TD style="WIDTH: 646px; HEIGHT: 10px" width="646">
						<FMCONTROLS:FMBaseDataGrid id="AssignmentDataGrid" runat="server" 
                            BackColor="White" Width="568px" CssClass="tabletext"
							AllowPaging="True" AllowSorting="True" BorderColor="White" BorderWidth="1px" CellPadding="3"
							Height="10px" GridLines="Vertical" PageSize="5" AutoGenerateColumns="False" 
                            onselectedindexchanged="GridMoveItemCommand" 
                            onitemdatabound="AssignmentDataGridItemDataBound" aria-label="Group Assignment Data">
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
										<FMControls:FMEditLinkButton runat="server"/>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMUpdateLinkButton runat="server"/>&nbsp;<FMControls:FMCancelLinkButton ID="FMCancelLinkButton1" runat="server"/>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="ReportGuid" HeaderText="ReportGuid"></asp:BoundColumn>
								<asp:BoundColumn DataField="ReportName" HeaderText="Reports">
									<HeaderStyle Font-Bold="True"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="GroupName" HeaderText="Groups">
									<HeaderStyle Font-Bold="True"></HeaderStyle>
								</asp:BoundColumn>
								<asp:ButtonColumn Text="&lt;img src=../FMWebApp/images/Up.gif border=0 align=absmiddle alt='Move this item'&gt;"
									HeaderText="Order" CommandName="Select">
									<HeaderStyle Font-Bold="True" Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								</asp:ButtonColumn>
                                <asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server"/>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMBaseDataGrid></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 646px; HEIGHT: 38px" vAlign="middle" width="646">
						<FMCONTROLS:FMButton id="AddReportButton" runat="server" Style="min-width:100px;"
                            CssClass="formfieldtitle" Text="Add Report" onclick="AddReportButtonOnClick"/>&nbsp;&nbsp;
						<FMCONTROLS:FMButton id="AddGroupButton" runat="server" CssClass="formfieldtitle" Style="min-width:120px;" Text="Add/Modify Groups" onclick="AddGroupButtonOnClick" />
						<FMControls:FMButton ID="CreateDefaultReportsAssignmentButton" Style="min-width:170px; float: right"
									runat="server" CssClass="formfieldtitle" Text="Create Default Assignments" OnClick="CreateDefaultReportsAssignmentButtonOnClick"></FMControls:FMButton>

					</TD>
				</TR>
			</TABLE>
    </body>
</HTML>