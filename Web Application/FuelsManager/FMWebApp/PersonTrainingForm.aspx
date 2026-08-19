<%@ Page Language="c#" CodeBehind="PersonTrainingForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.PersonTrainingForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
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
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 99; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label3" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="450px" BackColor="Transparent">Personnel Training Configuration</FMControls:FMLabel>
                <table id="Table1" style="z-index: 100; left: 32px; width: 856px; position: absolute; top: 48px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
				<tr>
                    <td valign="middle" style="width: 856px; height: 36px">
                        <FMControls:FMButton ID="AddButton2" TabIndex="6" runat="server" CssClass="formfieldtitle" Text="Add"
                            Width="100px"></FMControls:FMButton>&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="PersonTrainingFormPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged"></FMControls:FMPageSizeDropDown></td>
                </tr>
				<TR>
					<TD style="WIDTH: 856px; HEIGHT: 36px"><FMCONTROLS:FMDATAGRID id="TrainingDataGrid" style="LEFT: 1px; TOP: 0px" runat="server" CssClass="tabletext" RowHeaderColumn="Training ID"
							PageSize="16" AllowPaging="True" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" Width="856px" GridLines="Vertical" AutoGenerateColumns="False"
							BackColor="White" BorderStyle="None">
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
										<FMControls:FMUpdateLinkButton runat="server"/>&nbsp;
										<FMControls:FMCancelLinkButton runat="server"/>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Index">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>' ID="IndexLabel">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Training ID">
									<HeaderStyle Width="2in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' width="2in">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="IDTextBox" ToolTip="Personnel training ID" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>' width="2in" MaxLength="50" aria-required="true">
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
										<asp:TextBox width=3in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' CssClass="tabletext" ID="DescriptionTextBox" ToolTip="Description" MaxLength="50">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Duration (hrs)">
									<HeaderStyle Width="1in"></HeaderStyle>
									<ItemTemplate>
										<asp:Label width=1in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Duration") %>' ID="Label2">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox width=1in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Duration") %>' CssClass="tabletext" ID="DurationTextbox" ToolTip="Duration in hours" MaxLength="5">
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
										<asp:TextBox width=1in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Reoccurrence") %>' CssClass="tabletext" ID="ReoccurrenceTextbox" ToolTip="Reoccurrence in days" MaxLength="5">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server"/>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDATAGRID></TD>
				</TR>
				<TR>
					<TD style="WIDTH: 856px; HEIGHT: 36px" vAlign="middle"><FMCONTROLS:FMBUTTON id="AddButton" runat="server" CssClass="formfieldtitle" Text="Add" Width="98px"></FMCONTROLS:FMBUTTON></TD>
				</TR>
			</TABLE>
        </div>
        </form>
    </body>
</html>
