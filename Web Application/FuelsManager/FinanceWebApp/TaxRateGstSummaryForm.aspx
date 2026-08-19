<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaxRateGstSummaryForm.aspx.cs" Inherits="FuelsManager.FinanceWebApp.TaxRateGstSummaryForm" %>
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
</head>
<body>
   <form id="TaxRateGstSummaryForm" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
      <asp:image id="FadeImage" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				     ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent">
      </asp:image>
      <FMCONTROLS:FMLABEL id="GSTSummaryTitleLabel" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				              runat="server" BackColor="Transparent" Width="272px" CssClass="headline">GST Taxes Configuration</FMCONTROLS:FMLABEL>
      <table style="Z-INDEX: 103; LEFT: 8px; WIDTH: 737px; POSITION: absolute; TOP: 48px; HEIGHT: 74px"
				 cellspacing="1" cellpadding="1" width="737" border="0">
         <tr>
            <td>
               <FMControls:FMButton ID="AddButtonTop" Runat="server" Text="Add" Width="67px" 
                  onclick="AddButtonTop_Click" CssClass="formfieldtitle"></FMControls:FMButton>
               &nbsp;&nbsp;
               <FMCONTROLS:FMPAGESIZEDROPDOWN id="GridSizeDropDown" runat="server" 
                  onselectedindexchanged="GridSizeDropdownOnChange" />
            </td>
         </tr>
         <tr>
            <td>
               <FMControls:FMDataGrid id="GSTDataGrid" runat="server" backColor="White" cssClass="tabletext" width="400px" pageSize="10"
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
                           <FMControls:FMLabel ID="labGSTCodeHdr" runat="server" Text="GST Code" />
                        </HeaderTemplate>
                        <ITEMSTYLE VerticalAlign="Middle" HorizontalAlign="Left"></ITEMSTYLE>
                        <ITEMTEMPLATE>
                           <%# DataBinder.Eval(Container.DataItem, "GstCode") %>
                        </ITEMTEMPLATE>
                        <EDITITEMTEMPLATE>
                           <asp:TextBox id="txtGstCode" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "GstCode")%>'>
                           </asp:TextBox>
                        </EDITITEMTEMPLATE>
                     </ASP:TEMPLATECOLUMN>
                     <asp:TemplateColumn>
                        <HeaderTemplate>
                           <FMControls:FMLabel ID="labGSTDateHdr" runat="server" Text="GST Date" />
                        </HeaderTemplate>
                        <ItemStyle VerticalAlign="Middle" HorizontalAlign="Left"></ItemStyle>
                        <ItemTemplate>
                           <%# DataBinder.Eval(Container.DataItem, "GSTDate", DateFormat)%>
                        </ItemTemplate>
                        <EditItemTemplate>
                           <FMControls:FMDate ID="dtGSTDate" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "GSTDate", DateFormat)%>'>
                           </FMControls:FMDate>
                        </EditItemTemplate>
                     </asp:TemplateColumn>
                     <ASP:TEMPLATECOLUMN>
                        <HeaderTemplate>
                           <FMControls:FMLabel ID="labGSTValueHdr" runat="server" Text="GST Percentage" />
                        </HeaderTemplate>
                        <ITEMSTYLE VerticalAlign="Middle" HorizontalAlign="Left"></ITEMSTYLE>
                        <ITEMTEMPLATE>
                           <%# DataBinder.Eval(Container.DataItem, "GstValue") %>
                        </ITEMTEMPLATE>
                        <EDITITEMTEMPLATE>
                           <asp:TextBox id="txtGstValue" CssClass="tabletext" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "GstValue")%>'>
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
               <FMControls:FMButton ID="AddButtonBottom" Runat="server" Text="Add" Width="67px" 
                  onclick="AddButtonBottom_Click" CssClass="formfieldtitle"></FMControls:FMButton>
            </td>
         </tr>
      </table>
   </div>
</form>
</body>
</html>
