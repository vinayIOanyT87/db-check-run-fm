<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="InvoiceQueriesForm.aspx.cs" AutoEventWireup="True" Inherits="ADFWebApp.InvoiceQueriesForm" ValidateRequest=false %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>FuelsManager - Invoice Queries Form</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="../FuelsManager.css" rel="stylesheet" />
	    <style type="text/css">
            #Text1
            {
                width: 261px;
            }
            #btn
            {
                width: 364px;
            }
            #txtSearchText
            {
                width: 294px;
            }
        </style>
        <script type="text/javascript" language="javascript">
        window.name = "invoiceQuery"
        function Select(QueryIndex, Description)
			{
				var Result = new Array();
				Result[0] = QueryIndex;
				Result[1] = Description;
				
				window.returnValue=Result;
				
				window.close();
			}
        </script>
	</head>
	<body MS_POSITIONING="GridLayout">
		<form id="InvoiceQueriesForm" method="post" runat="server" target="invoiceQuery">
			<asp:image id="FadeImage" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image>
            <FMCONTROLS:FMLABEL id="lblHeading" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" BackColor="Transparent" CssClass="headline" Width="272px">Invoice Query Numbers</FMCONTROLS:FMLABEL>
			<table id="Table1" style="Z-INDEX: 103; LEFT: 8px; WIDTH: 737px; POSITION: absolute; TOP: 48px; HEIGHT: 74px"
				cellSpacing="3" cellPadding="2" width="737" border="0">
				<tr>
					<td colspan="2" nowrap="nowrap">
                        <input id="txtSearchText" type="text" runat="server"  />&nbsp;
                        <FMCONTROLS:FMBUTTON id="btnRefresh" tabIndex="1" runat="server" 
                            CssClass="formfieldtitle" width="64px"
							Text="Find" onclick="btnRefresh_Click"></FMCONTROLS:FMBUTTON>&nbsp;
                        <FMCONTROLS:FMBUTTON id="btnShowAll" tabIndex="1" runat="server" 
                            CssClass="formfieldtitle" width="64px"
							Text="Show All" onclick="btnShowAll_Click"></FMCONTROLS:FMBUTTON></td>
				</tr>
				<tr>
					<td><FMCONTROLS:FMBUTTON id="btnAddTop" tabIndex="1" runat="server" 
                            CssClass="formfieldtitle" width="100px"
							Text="Add" onclick="btnAddTop_Click"></FMCONTROLS:FMBUTTON>&nbsp;&nbsp; <FMCONTROLS:FMPAGESIZEDROPDOWN id="ddlPageSize" tabIndex="7" runat="server" 
                            Width="96px" onselectedindexchanged="ddlPageSize_SelectedIndexChanged" ></FMCONTROLS:FMPAGESIZEDROPDOWN></td>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td colspan="2">
					   <FMCONTROLS:FMDATAGRID id="InvoiceQueriesDataGrid" tabIndex="5" runat="server" 
                            BackColor="White" CssClass="tabletext"
							                    Width="736px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" 
                                         BorderWidth="1px" AllowSorting="True" BorderColor="White" 
                            CellPadding="3" AllowPaging="True" 
                                         PageSize="20" FixedHeaders="True" FixedHeight="">
							<FooterStyle ForeColor="Black" BackColor="#333399"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#333399"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Select">
									<HeaderStyle Width="0.5in"></HeaderStyle>
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" 
                                        VerticalAlign="Middle" />
                                </asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle Wrap="False" HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton id="EditLinkButton" runat="server"></FMControls:FMEditLinkButton>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMUpdateLinkButton id="UpdateLinkButton" runat="server"></FMControls:FMUpdateLinkButton>
										<FMControls:FMCancelLinkButton id="CancelLinkButton" runat="server"></FMControls:FMCancelLinkButton>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="ID">
									<ItemTemplate>
										<asp:Label id="lblGridID" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="txtEditQueryID" readonly="true" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.ID") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Description">
									<ItemTemplate>
										<asp:Label id="lblGridDescription" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="txtEditDescription" runat="server" CssClass="tabletext" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>'>
										</asp:TextBox>
									</EditItemTemplate>
							    </asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#333399" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDATAGRID></td>
				</tr>
				<tr>
					<td><FMCONTROLS:FMBUTTON id="btnAddBottom" tabIndex="1" runat="server" 
                            CssClass="formfieldtitle" width="100px"
							Text="Add" onclick="btnAddBottom_Click"></FMCONTROLS:FMBUTTON></td>
				</tr>
			</table>
		</form>
	</body>
</html>
