<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="WacSummary.aspx.cs" AutoEventWireup="True" Inherits="ADFWebApp.WacSummary" %>
<%@ Register TagPrefix="FM" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Weighted Average Cost Summary</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../FuelsManager.css" rel="stylesheet">
		<script>
		function EditWacItem(a_wacIndex)
        {
            self.location = "WacOverride.aspx?WacIndex=" + a_wacIndex;
        }
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<asp:image id="FadeImage" 
                style="Z-INDEX: -100; LEFT: 0px; POSITION: absolute; TOP: -1px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
			<FMCONTROLS:FMLABEL id="lblHeading" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				runat="server" BackColor="Transparent" CssClass="headline" Width="272px">WAC Summary</FMCONTROLS:FMLABEL>
			<br />
			<br />
			<table id="tblGrid" runat="server" style="z-index:100" cellpadding="2" 
                cellspacing="3">
				<tr>
					<td><FM:FMLABEL id="labStartDate" runat="server" CssClass="formfieldtitle">Start Date</FM:FMLABEL></td>
					<td><FM:FMDATE id="startDateControl" runat="server" CssClass="formfield" Width="136px"></FM:FMDATE></td>
					<td><FM:FMLABEL id="labEndDate" runat="server" CssClass="formfieldtitle">End Date</FM:FMLABEL></td>
					<td><FM:FMDATE id="endDateControl" runat="server" CssClass="formfield" Width="136px"></FM:FMDATE></td>
					<td>&nbsp;</td>
				</tr>			        
				<tr>
					<td><FM:FMLabel ID="labSite" runat="server" CssClass="formfieldtitle">Site</FM:FMLabel></td>
					<td><FM:FMDropDownList ID="ddlSite" runat="server" CssClass="formfield"></FM:FMDropDownList></td>
					<td><FM:FMLabel ID="labFuelType" runat="server" CssClass="formfieldtitle">Fuel Type</FM:FMLabel></td>
					<td><FM:FMDropDownList ID="ddlFuelType" runat="server" CssClass="formfield"></FM:FMDropDownList></td>
					<td>&nbsp;</td>
				</tr>				
				<tr>
					<td><FM:FMLabel ID="labShow" runat="server" CssClass="formfieldtitle">Show:</FM:FMLabel></td>
					<td>
                        <FMControls:FMRadioButtonList ID="radioListShow" runat="server" 
                            CssClass="formfield">
                            <asp:ListItem Selected="True">Changes</asp:ListItem>
                            <asp:ListItem>All</asp:ListItem>
                        </FMControls:FMRadioButtonList>
                    </td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
				</tr>				
				<tr>
					<td>
                        <br />
                    </td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
				</tr>				
                <tr>
                    <td><FM:FMButton id="btnAddTop" runat="server" CssClass="formfieldtitle" Text="Add" 
                            Width="100px" /></td>
	                <td><FM:FMPageSizeDropDown ID="ddlPageSize" runat="server" CssClass="formfield" 
                            onselectedindexchanged="ddlPageSize_SelectedIndexChanged"></FM:FMPageSizeDropDown></td>
                    <td colspan="3" align="right"><FM:FMButton id="btnRefresh1" runat="server" CssClass="formfieldtitle" 
                            Text="Refresh" onclick="btnRefresh1_Click" Width="64px" /></td>
                </tr>
				<tr>
				    <td colspan="5">
	                    <FM:FMDATAGRID id="WacGrid" tabIndex="5" 
                                runat="server" BackColor="White" CssClass="tabletext"
				                Width="736px" BorderStyle="None" AutoGenerateColumns="False" GridLines="Vertical" 
                                BorderWidth="1px" AllowSorting="True"
				                BorderColor="White" CellPadding="3" AllowPaging="True" PageSize="20">
			                <FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
			                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
			                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
			                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
			                <HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
			                <Columns>
			                    <asp:TemplateColumn HeaderText="View" ItemStyle-Width="100px">
									<HeaderStyle Width="0.5in"></HeaderStyle>
			                        <ItemTemplate>
						                <FMControls:FMEditLinkButton id="EditLinkButton" runat="server"></FMControls:FMEditLinkButton>
					                </ItemTemplate>
			                    </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Date">
                                    <ItemTemplate>
                                        <FM:FMLabel ID="lblWacDate" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Date") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Value">
                                    <ItemTemplate>
                                        <FM:FMLabel ID="lblWacValue" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Value") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Source">
                                    <ItemTemplate>
                                        <FM:FMLabel ID="lblWacSource" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Source") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
			                    <asp:TemplateColumn Visible="False"></asp:TemplateColumn>
			                    <asp:TemplateColumn Visible="False"></asp:TemplateColumn>
			                </Columns>
			                <PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
		                </FM:FMDATAGRID>
		            </td>
		        </tr>
		        <tr>
		            <td><FM:FMButton id="btnAddBottom" runat="server" CssClass="formfieldtitle" 
                            Text="Add" Width="100px" /></td>
		            <td>&nbsp;</td>
		            <td>&nbsp;</td>
		            <td>&nbsp;</td>
		            <td>&nbsp;</td>
		        </tr>
		    </table>
        </form>
				
	</body>
</HTML>
