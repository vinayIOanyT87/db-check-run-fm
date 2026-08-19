<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaxRateMarkupSummaryForm.aspx.cs" Inherits="FuelsManager.FinanceWebApp.TaxRateMarkupSummaryForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="TaxRateMarkupSummaryHeader" runat="server">
		<title></title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
		<meta name="CODE_LANGUAGE" content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
   <form id="TaxRateMarkupSummaryForm" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
      <asp:image id="FadeImageMarkup" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				     ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent">
      </asp:image>
      <FMCONTROLS:FMLABEL id="MarkupConfigurationTitleLabel" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				              runat="server" BackColor="Transparent" Width="272px" CssClass="headline">Markups Configuration</FMCONTROLS:FMLABEL>
      <table style="Z-INDEX: 103; LEFT: 8px; WIDTH: 737px; POSITION: absolute; TOP: 48px; HEIGHT: 74px"
				 cellspacing="1" cellpadding="1" width="737" border="0">
         <tr>
            <td>
               <FMControls:FMButton ID="AddTopButton" Runat="server" Text="Add" Width="67px" 
                  onclick="AddButtonTop_Click" CssClass="formfieldtitle"></FMControls:FMButton>
                  &nbsp;&nbsp;
               <FMCONTROLS:FMPAGESIZEDROPDOWN id="GridSizeDropDown" runat="server" 
                  onselectedindexchanged="GridSizeDropdownOnChange" />
            </td>
         </tr>
         <tr>
            <td>
               <FMControls:FMDataGrid id="MarkupDataGrid" runat="server" backColor="White" cssClass="tabletext" width="400px" pageSize="10"
                                      allowPaging="True" cellPadding="3" borderColor="White" allowSorting="True" borderWidth="1px" 
                                      gridLines="Vertical" autoGenerateColumns="False" borderStyle="None">
                  <FOOTERSTYLE BackColor="<%$ AppSettings: ColorHeaderBlue %>" foreColor="Black"></FOOTERSTYLE>
                  <SELECTEDITEMSTYLE BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SELECTEDITEMSTYLE>
                  <ALTERNATINGITEMSTYLE BackColor="Gainsboro"></ALTERNATINGITEMSTYLE>
                  <ITEMSTYLE BackColor="#EEEEEE" ForeColor="Black"></ITEMSTYLE>
                  <HEADERSTYLE CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>" ForeColor="White" Font-Bold="True"></HEADERSTYLE>
                  <COLUMNS>
                     <ASP:TEMPLATECOLUMN HeaderText="Edit">
                        <HeaderStyle Width="55px"></HeaderStyle>
                        <ITEMSTYLE VerticalAlign="Middle" HorizontalAlign="Center"></ITEMSTYLE>
                        <ITEMTEMPLATE>
                           <FMCONTROLS:FMEDITLINKBUTTON id="btnEdit" runat="server" Name="btnEdit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdentityGuid") %>' >
                           </FMCONTROLS:FMEDITLINKBUTTON>
                        </ITEMTEMPLATE>
                     </ASP:TEMPLATECOLUMN>
                     <ASP:TEMPLATECOLUMN>
                        <HeaderTemplate>
                           <FMControls:FMLabel ID="labPurchasingUnitHdr" runat="server" Text="Purchasing Unit" />
                        </HeaderTemplate>
                        <ITEMSTYLE VerticalAlign="Middle" HorizontalAlign="Left"></ITEMSTYLE>
                        <ITEMTEMPLATE>
                           <%# DataBinder.Eval(Container.DataItem, "PurchasingEntity") %>
                        </ITEMTEMPLATE>
                        <EDITITEMTEMPLATE>
                           <asp:TextBox id="txtPurchasingUnit" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PurchasingEntity")%>'>
                           </asp:TextBox>
                        </EDITITEMTEMPLATE>
                     </ASP:TEMPLATECOLUMN>
                     <ASP:TEMPLATECOLUMN>
                        <HeaderTemplate>
                           <FMControls:FMLabel ID="labMarkupValueHdr" runat="server" Text="Markup Percentage" />
                        </HeaderTemplate>
                        <ITEMSTYLE VerticalAlign="Middle" HorizontalAlign="Left"></ITEMSTYLE>
                        <ITEMTEMPLATE>
                           <%# DataBinder.Eval(Container.DataItem, "MarkupRate") %>
                        </ITEMTEMPLATE>
                        <EDITITEMTEMPLATE>
                           <asp:TextBox id="txtMarkupValue" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "MarkupRate")%>'>
                           </asp:TextBox>
                        </EDITITEMTEMPLATE>
	                  </ASP:TEMPLATECOLUMN>
                     <ASP:TEMPLATECOLUMN HeaderText="Delete">
                        <HEADERSTYLE width="0.5in"></HEADERSTYLE>
                        <ITEMSTYLE VerticalAlign="Middle" HorizontalAlign="Center"></ITEMSTYLE>
                        <ITEMTEMPLATE>
                           <FMCONTROLS:FMDELETELINKBUTTON id="btnDelete" runat="server" Name="btnDelete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdentityGuid") %>'>
                           </FMCONTROLS:FMDELETELINKBUTTON>
                        </ITEMTEMPLATE>
                     </ASP:TEMPLATECOLUMN>
                  </COLUMNS>
                  <PAGERSTYLE BackColor="<%$ AppSettings: ColorHeaderBlue %>" ForeColor="White" HorizontalAlign="Center" Mode="NumericPages"></PAGERSTYLE>
               </FMControls:FMDataGrid>
            </td>
         </tr>
         <tr>
            <td>
               <FMControls:FMButton ID="AddBottomButton" Runat="server" Text="Add" Width="67px" 
                  onclick="AddButtonBottom_Click" CssClass="formfieldtitle"></FMControls:FMButton>
            </td>
         </tr>
      </table>
   </div>
</form>
</body>
</html>
