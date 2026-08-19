<%@ Page language="c#" Codebehind="ImportResults.aspx.cs" AutoEventWireup="false" Inherits="StandardXMLImportExport.ImportResults" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
		<title>ImportResults</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="../css/FuelsManager.css" rel="stylesheet">
  </HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:image id="FadeImage" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				ImageUrl="../FMWebApp/images/Page_Fade_7.jpg" BackColor="Transparent"></asp:image>
			<asp:label id="ImportResultsLabel" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server">Import Results</asp:label>
			<asp:DataGrid id="resultsGrid" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 144px"
				runat="server" AllowPaging="True" PageSize="25" AutoGenerateColumns="False" CssClass="tabletext" BackColor="White" BorderStyle="None"
				GridLines="Vertical" BorderWidth="1px" BorderColor="#999999" CellPadding="1">
<FooterStyle ForeColor="Black" BackColor="#CCCCCC">
</FooterStyle>

<SelectedItemStyle Font-Bold="True" ForeColor="White" CssClass="tablelink" BackColor="#008A8C">
</SelectedItemStyle>

<AlternatingItemStyle CssClass="tabletext" BackColor="Gainsboro">
</AlternatingItemStyle>

<ItemStyle ForeColor="Black" CssClass=".tabletext" BackColor="#EEEEEE">
</ItemStyle>

<HeaderStyle Font-Bold="True" Wrap="False" ForeColor="White" CssClass="tablecolhead" BackColor="#333399">
</HeaderStyle>

<Columns>
<asp:BoundColumn DataField="Level" HeaderText="Level"></asp:BoundColumn>
<asp:BoundColumn DataField="Message" HeaderText="Message"></asp:BoundColumn>
<asp:BoundColumn DataField="TransID" HeaderText="Transaction ID"></asp:BoundColumn>
</Columns>

<PagerStyle HorizontalAlign="Center" ForeColor="White" BackColor="#333399" Mode="NumericPages">
</PagerStyle>
			</asp:DataGrid></form>
	</body>
</HTML>
