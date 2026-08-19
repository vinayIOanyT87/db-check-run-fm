<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="IATACodesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.IATACodesForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
  <HEAD>
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1" />
		<meta name="CODE_LANGUAGE" Content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
  </HEAD>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="Form1" method="post" enctype="multipart/form-data" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 32px; WIDTH: 600px; POSITION: absolute; TOP: 96px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<TD width="350" height="36" vAlign="middle">
						<FMControls:FMButton width="100px" id="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
							tabIndex="6" />
						<FMControls:FMPageSizeDropDown ID="IATACodeSummaryPageSizeDropDown" alt="Page size" runat="server" onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />
						<FMControls:FMLabel Width="500px" ID="lblWarning" runat="server" CssClass="formfield" Text="abc" Visible="false" ForeColor="Red" />
					</TD>
				</tr>
				<TR>
					<TD style="WIDTH: 500px; HEIGHT: 10px">
						<FMCONTROLS:FMDataGridFixedPaging ID="IATACodesDataGrid" 
                            style="LEFT: 1px; TOP: 0px" runat="server"
								AutoGenerateColumns="False"
								DataKeyNames="SiteGuid, IdentityGuid"
								BorderStyle="Solid" 
								BackColor="White" 
								GridLines="Vertical"
								Width="648px"
								BorderWidth="1px"
								AllowSorting="True"
								CellPadding="3"
								AllowPaging="True"
								CssClass="tabletext"
								EmptyDataText="No records found"
								PageSize="12"
							BorderColor="White"
							tabIndex="7"
								ShowHeaderWhenEmpty="True"
								ShowFooterWhenEmpty="False"
								FixedHeaders="True"
								GroupColumnOffset="0"
								GroupingDepth="0" FixedHeight="450px" Height="450px">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>

							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="IATAGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
								<asp:BoundColumn DataField="IATAID" HeaderText="IATA ID" SortExpression="ID"></asp:BoundColumn>
								<asp:BoundColumn DataField="Name" HeaderText="Name" SortExpression="Name"></asp:BoundColumn>
								<asp:BoundColumn DataField="CountryID" HeaderText="Country" SortExpression="Country"></asp:BoundColumn>
								<asp:BoundColumn DataField="TimeZone" HeaderText="TimeZone" SortExpression="TimeZone"></asp:BoundColumn>
								<asp:BoundColumn DataField="Latitude" HeaderText="Latitude" SortExpression="Latitude"></asp:BoundColumn>
								<asp:BoundColumn DataField="Longitude" HeaderText="Longitude" SortExpression="Longitude"></asp:BoundColumn>
								<asp:BoundColumn DataField="Zoom" HeaderText="Zoom" SortExpression="Zoom"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="GVFixedFooter" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages" />
						</FMControls:FMDataGridFixedPaging></TD>
				</TR>
				<tr>
					<TD style="WIDTH: 350px; HEIGHT: 36px" vAlign="middle">
                        <FMControls:FMButton id="AddButton" runat="server" Width="98px" Text="Add" CssClass="formfieldtitle"
							tabIndex="8"></FMControls:FMButton></TD>
				</tr>
			</TABLE>
			<FMControls:FMLabel id="FindLabel" AssociatedControlID="FindTextBox" style="Z-INDEX: 111; LEFT: 224px; POSITION: absolute; TOP: 40px"
				runat="server" CssClass="formfieldtitle" BackColor="Transparent">Find String:</FMControls:FMLabel>
			<FMCONTROLS:FMBUTTON id="ShowAllBtn" style="Z-INDEX: 110; LEFT: 616px; POSITION: absolute; TOP: 64px"
				runat="server" CssClass="formfieldtitle" Width="64px" Text="Show All" tabIndex="4" onclick="FindAllBtn_OnClick"></FMCONTROLS:FMBUTTON>
			<FMCONTROLS:FMBUTTON id="FindBtn" style="Z-INDEX: 109; LEFT: 536px; POSITION: absolute; TOP: 64px" runat="server"
				CssClass="formfieldtitle" Text="Find" Width="64px" tabIndex="3" onclick="FindBtn_OnClick"></FMCONTROLS:FMBUTTON>
			<asp:Image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
			<FMControls:FMLabel id="Label2" style="Z-INDEX: 103; LEFT: 32px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="272px" BackColor="Transparent">Delivery Location Configuration</FMControls:FMLabel>
			<asp:TextBox id="FindTextBox" style="Z-INDEX: 108; LEFT: 224px; POSITION: absolute; TOP: 64px"
				runat="server" CssClass="formfield" Width="300px" tabIndex="2"></asp:TextBox>
		</div>
</form>
		<script language="jscript">
			var findBtn = document.getElementById("FindBtn");
			var findTbBtn = document.getElementById("FindTextBox");
			
			if (findBtn != null && findTbBtn != null)
			{
			    try
			    {
			        findBtn.setActive();
			        findTbBtn.focus();
			    }
			    catch (err){}
			}

			// Set the Find Button to be activated by the enter key.
			document.addEventListener('keydown', function (ev) {
			    if (ev.keyCode == 13) {
			        ev.returnValue = false;
			        ev.cancel = true;
			        document.all("FindBtn").click();
			    }
			});
		</script>
	</body>
</HTML>
