<%@ Page language="c#" Codebehind="PermissivesForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.PermissivesForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
		<title></title>
		<base target="_self">
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
		<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" defer="defer"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
        <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        </HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<FMCONTROLS:FMLABEL id="ConfigurationLabel" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" Width="456px" CssClass="headline" BackColor="Transparent">Permissives Configuration</FMCONTROLS:FMLABEL><asp:image id="Image1" style="Z-INDEX: 99; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				CssClass="formfieldtitle" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:image>
			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 24px; WIDTH: 39.37%; POSITION: absolute; TOP: 48px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<td><FMCONTROLS:FMLABEL id="Fmlabel1" runat="server" Width="80px" CssClass="formfieldtitle" BackColor="Transparent"
							Height="20px">Outputs:</FMCONTROLS:FMLABEL></td>
				</tr>
				<tr>
					<TD width="710" height="10"><FMCONTROLS:FMDATAGRID id="OutputPermissivesDataGrid" tabIndex="1" runat="server" Width="520px" CssClass="tabletext"
							BackColor="White" Height="10px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
							BorderColor="White" CellPadding="3" PageSize="5" AllowPaging="True">
<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>">
</FooterStyle>

<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C">
</SelectedItemStyle>

<EditItemStyle Wrap="False">
</EditItemStyle>

<AlternatingItemStyle Wrap="False" BackColor="Gainsboro">
</AlternatingItemStyle>

<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE">
</ItemStyle>

<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>">
</HeaderStyle>

<Columns>
<asp:TemplateColumn HeaderText="Edit">
<HeaderStyle Width="0.5in">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle">
</ItemStyle>

<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" ID="Fmeditlinkbutton1" NAME="Fmeditlinkbutton1" />
									
</ItemTemplate>
</asp:TemplateColumn>
<asp:TemplateColumn Visible="False" HeaderText="Index">
<ItemTemplate>
										<asp:Label ID="OutputPermissiveIndexLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'>
										</asp:Label>
									
</ItemTemplate>
</asp:TemplateColumn>
<asp:BoundColumn DataField="Host" HeaderText="System">
<HeaderStyle Width="0.5in">
</HeaderStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="OPCServerID" HeaderText="OPC Server">
<HeaderStyle Width="1.5in">
</HeaderStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="OPCItemID" HeaderText="OPC Item ID">
<HeaderStyle Width="3in">
</HeaderStyle>
</asp:BoundColumn>
<asp:TemplateColumn HeaderText="Delete">
<HeaderStyle Width="0.5in">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle">
</ItemStyle>

<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" ID="Fmdeletelinkbutton1" NAME="Fmdeletelinkbutton1" />
									
</ItemTemplate>
</asp:TemplateColumn>
</Columns>

<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages">
</PagerStyle>
						</FMCONTROLS:FMDATAGRID></TD>
				</tr>
				<tr>
					<td height="35"><FMCONTROLS:FMBUTTON id="AddOutputPermissiveButton" tabIndex="2" runat="server" Text="Add" Width="67px"
							CssClass="formfieldtitle"></FMCONTROLS:FMBUTTON></td>
				</tr>
				<tr>
					<td><FMCONTROLS:FMLABEL id="Fmlabel2" runat="server" Width="80px" CssClass="formfieldtitle" BackColor="Transparent"
							Height="20px">Inputs:</FMCONTROLS:FMLABEL></td>
				</tr>
				<tr>
					<TD width="710" height="10"><FMCONTROLS:FMDATAGRID id="InputPermissivesDataGrid" tabIndex="1" runat="server" Width="680px" CssClass="tabletext"
							BackColor="White" Height="10px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px" AllowSorting="True"
							BorderColor="White" CellPadding="3" PageSize="5" AllowPaging="True">
<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>">
</FooterStyle>

<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C">
</SelectedItemStyle>

<EditItemStyle Wrap="False">
</EditItemStyle>

<AlternatingItemStyle Wrap="False" BackColor="Gainsboro">
</AlternatingItemStyle>

<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE">
</ItemStyle>

<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>">
</HeaderStyle>

<Columns>
<asp:TemplateColumn HeaderText="Edit">
<HeaderStyle Width="0.5in">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle">
</ItemStyle>

<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" ID="Fmeditlinkbutton2" NAME="Fmeditlinkbutton1" />
									
</ItemTemplate>
</asp:TemplateColumn>
<asp:TemplateColumn Visible="False" HeaderText="Index">
<ItemTemplate>
										<asp:Label ID="InputPermissiveIndexLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Index") %>'>
										</asp:Label>
									
</ItemTemplate>
</asp:TemplateColumn>
<asp:BoundColumn DataField="Host" HeaderText="System">
<HeaderStyle Width="0.5in">
</HeaderStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="OPCServerID" HeaderText="OPC Server">
<HeaderStyle Width="1.5in">
</HeaderStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="OPCItemID" HeaderText="OPC Item ID">
<HeaderStyle Width="3in">
</HeaderStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="MessageID" HeaderText="Message ID">
<HeaderStyle Width="2in">
</HeaderStyle>
</asp:BoundColumn>
<asp:TemplateColumn HeaderText="Delete">
<HeaderStyle Width="0.5in">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle">
</ItemStyle>

<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" ID="Fmdeletelinkbutton2" NAME="Fmdeletelinkbutton1" />
									
</ItemTemplate>
</asp:TemplateColumn>
</Columns>

<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages">
</PagerStyle>
						</FMCONTROLS:FMDATAGRID></TD>
				</tr>
				<tr>
					<td height="35"><FMCONTROLS:FMBUTTON id="AddInputPermissiveButton" tabIndex="2" runat="server" Text="Add" Width="67px"
							CssClass="formfieldtitle"></FMCONTROLS:FMBUTTON></td>
				</tr>
			</TABLE>
		</form>
	</body>
</HTML>
