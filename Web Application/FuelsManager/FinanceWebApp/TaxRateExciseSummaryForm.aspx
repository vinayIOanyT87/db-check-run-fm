<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaxRateExciseSummaryForm.aspx.cs" Inherits="FinanceWebApp.TaxRateExciseSummaryForm"  %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
		<title></title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
		<meta name="CODE_LANGUAGE" content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
      <style type="text/css">
         .style1
         {
            width: 417px;
         }
         .style2
         {
            width: 76px;
         }
      </style>
</head>
<body>
    <form id="TaxRateExciseSummaryForm" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
      <asp:image id="FadeImage" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				     ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent">
      </asp:image>
      <FMCONTROLS:FMLABEL id="ExciseSummaryTitleLabel" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				              runat="server" BackColor="Transparent" Width="272px" CssClass="headline">Excise Taxes Configuration</FMCONTROLS:FMLABEL>
         <table style="Z-INDEX: 103; LEFT: 8px; WIDTH: 498px; POSITION: absolute; TOP: 48px; HEIGHT: 74px"
				 cellspacing="1" cellpadding="1" border="0">
				 <tr>
				    <td class="style2">
				      <FMControls:FMLabel ID="FuelTypeLabel" runat="server" Text="Product:" 
                      CssClass="formfieldtitle" />
				    </td>
				    <td class="style1">
				       <FMControls:FMProductTextBox ID="ProductSelectControl" runat="server" CssClass="formfield" AutoPostBack="False" Width="200px"></FMControls:FMProductTextBox>
                     &nbsp;&nbsp;
                   <FMControls:FMButton ID="RefreshButton" runat="server" 
                      CssClass="formfieldtitle" onclick="RefreshButtonOnClick" Text="Refresh" 
                      Width="77px" />
				    </td>
				 </tr>
				 <tr>
				    <td class="style2">
                  <FMControls:FMLabel ID="StartDateLabel" runat="server" Text="Start Date:" CssClass="formfieldtitle" />
				    </td>
				    <td class="style1">
				       <FMControls:FMDate ID="StartDateControl" runat="server" />
				    </td>
				 </tr>
				 <tr>
				    <td class="style2">
                  <FMControls:FMLabel ID="EndDateLabel" runat="server" Text="End Date:" CssClass="formfieldtitle" />
				    </td>
				    <td class="style1">
				       <FMControls:FMDate ID="EndDateControl" runat="server" />
				    </td>
				 </tr>
				 <tr>
				   <td>&nbsp;</td>
				   <td>&nbsp;</td>
				 </tr>
				 <tr>
				   <td colspan="2">
                  <table cellspacing="1" cellpadding="1" border="0">
                      <tr>
                         <td>
                            <FMControls:FMButton ID="AddTopButton" runat="server" 
                               CssClass="formfieldtitle"  Text="Add" 
                               Width="77px" onclick="AddTopButtonOnClick" />
                            &nbsp;&nbsp;
                            <FMCONTROLS:FMPAGESIZEDROPDOWN id="GridSizeDropDown" runat="server" />

                         </td>
                      </tr>
                      <tr>
                        <td>
                           <FMCONTROLS:FMDATAGRID id="ExciseDataGrid" runat="server" BackColor="White" CssClass="tabletext" Width="400px"
                                                  PageSize="10" BorderWidth="1px" Gridlines="Vertical" AllowPaging="True" CellPadding="3" BorderColor="White"
                                                  AllowSorting="True" BorderStyle="None" AutoGenerateColumns="False">
                              <FooterStyle BackColor="<%$ AppSettings: ColorHeaderBlue %>" ForeColor="Black"></FooterStyle>
                              <SelectedItemStyle BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SelectedItemStyle>
                              <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                              <ItemStyle BackColor="#EEEEEE" ForeColor="Black"></ItemStyle>
                              <HeaderStyle CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>" ForeColor="White" Font-Bold="True"></HeaderStyle>
                              <Columns>
                                 <asp:TemplateColumn HeaderText="Edit">
                                    <HeaderStyle Width="55px"></HeaderStyle>
                                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
                                    <ItemTemplate>
                                       <FMControls:FMEditLinkButton id="btnEdit" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdentityGuid")%>'>
                                          &nbsp;
                                       </FMControls:FMEditLinkButton>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                       <FMControls:FMUpdateLinkButton id="btnUpdate" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdentityGuid")%>'>
                                          &nbsp;
                                       </FMControls:FMUpdateLinkButton>&nbsp;
                                       <FMControls:FMCancelLinkButton id="btnCancel" runat="server"></FMControls:FMCancelLinkButton>
                                    </EditItemTemplate>
                                 </asp:TemplateColumn>
                                 <asp:TemplateColumn>
                                    <HeaderTemplate><FMControls:FMLabel ID="labProductTypeHdr" runat="server" Text="Product Type" /></HeaderTemplate>
                                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Left" Wrap="False"></ItemStyle>
                                    <ItemTemplate>
                                       <%# DataBinder.Eval(Container.DataItem, "Product")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                       <FMControls:FMProductTextBox id="txtGridProduct" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Product")%>' CssClass="tabletext">
                                          &nbsp;
                                       </FMControls:FMProductTextBox>
                                    </EditItemTemplate>
                                 </asp:TemplateColumn>
                                 <asp:TemplateColumn>
                                    <HeaderTemplate><FMControls:FMLabel ID="labExciseDateHdr" runat="server" Text="Excise Date" /></HeaderTemplate>
                                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Left"></ItemStyle>
                                    <ItemTemplate>
                                       <%# DataBinder.Eval(Container.DataItem, "ExciseDate", DateFormat)%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                       <FMControls:FMDate ID="dtExciseDate" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ExciseDate", DateFormat)%>'>
                                       </FMControls:FMDate>
                                    </EditItemTemplate>
                                 </asp:TemplateColumn>
                                 <asp:TemplateColumn>
                                    <HeaderTemplate><FMControls:FMLabel ID="labExciseRateHdr" runat="server" Text="Excise Percentage" /></HeaderTemplate>
                                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Left"></ItemStyle>
                                    <ItemTemplate>
                                       <%# DataBinder.Eval(Container.DataItem, "ExciseRateStr")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                       <asp:TextBox ID="txtRate" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ExciseRateStr")%>' CssClass="tabletext">
                                       </asp:TextBox>
                                    </EditItemTemplate>
                                 </asp:TemplateColumn>
                                 <asp:TemplateColumn HeaderText="Delete">
                                    <HeaderStyle width="0.5in"></HeaderStyle>
                                    <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
                                    <ItemTemplate>
                                       <FMControls:FMDeleteLinkButton id="btnDelete" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdentityGuid")%>'>
                                          &nbsp;
                                       </FMControls:FMDeleteLinkButton>
                                    </ItemTemplate>
                                 </asp:TemplateColumn>
                              </Columns>
                              <PagerStyle BackColor="<%$ AppSettings: ColorHeaderBlue %>" ForeColor="White" HorizontalAlign="Center" Mode="NumericPages"></PagerStyle>
                           </FMCONTROLS:FMDATAGRID>
                         </td>
                      </tr>
                      <tr>
                         <td>
                            <FMControls:FMButton ID="AddBottomButton" runat="server" 
                               CssClass="formfieldtitle"  Text="Add" 
                               Width="77px" onclick="AddBottomButtonOnClick" />
                         </td>
                      </tr>
                  </table>
				   </td>
				 </tr>
         </table>
    </div>
</form>
</body>
<script type="text/javascript">
	function ProductSelect(productTextBoxID)
	{
		var sFeatures="dialogWidth: 8.81in; dialogHeight: 6in";
		var productTextBox        = document.getElementById(productTextBoxID);
		var result                = null;
		var companyID             = "";
					
		result = window.showModalDialog("../FMWebApp/ProductSelectForm.aspx?All=true&Type=MaxProduct" + 
					                    "&Map=MAX_MAP" + "&IDLink=" + encodeURIComponent(companyID), "", sFeatures);
 					                        
		if (result != null)
		{
			productTextBox.value = result[0];
			productTextBox.title = result[1];
		}
	}
</script>
</html>
