<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaxMarkupDetailForm.aspx.cs" Inherits="FuelsManager.FinanceWebApp.TaxMarkupDetailForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
		<title></title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
		<meta name="CODE_LANGUAGE" content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
      <style type="text/css">
         .style1
         {
            width: 92px;
         }
      </style>
</head>
<body>
   <form id="TaxMarkupDetailForm" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
      <asp:image id="FadeImage" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				     ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent">
      </asp:image>
      <FMCONTROLS:FMLABEL id="MarkupDetailTitleLabel" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 8px"
				              runat="server" BackColor="Transparent" Width="400px" 
          CssClass="headline">Markup Configuration</FMCONTROLS:FMLABEL>
       <table style="Z-INDEX: 103; LEFT: 8px; WIDTH: 502px; POSITION: absolute; TOP: 48px; HEIGHT: 74px"
				 cellspacing="1" cellpadding="1" border="0">
          <tr>
             <td class="style1">
                <FMControls:FMLabel ID="PurchasingEntityLabel" runat="server" CssClass="formfieldtitle">Purchasing Unit</FMControls:FMLabel>
             </td>
             <td>
               <span style="COLOR: red;">*</span>
               <asp:TextBox ID="PurchasingEntityTextBox" runat="server"></asp:TextBox>
             </td>
          </tr>
          <tr>
             <td class="style1">
                <FMControls:FMLabel ID="MarkupRateLabel" runat="server" CssClass="formfieldtitle">Markup Rate</FMControls:FMLabel>
             </td>
             <td>
                <span style="COLOR: red;">*</span>
                <asp:TextBox ID="MarkupRateTextBox" runat="server"></asp:TextBox>
             </td>
          </tr>
          <tr>
             <td colspan="2">
               &nbsp;
             </td>
          </tr>
          <tr>
            <td colspan="2">
               <FMControls:FMLabel ID="AssignedCompanyLabel" runat="server" CssClass="formfieldtitle">Assigned Companies</FMControls:FMLabel>
            </td>
          </tr>
          <tr>
             <td colspan="2">
                <FMControls:FMButton ID="AddTopButton" runat="server" CssClass="formfieldtitle" 
                   Text="Add" Width="65px" onclick="AddTopButtonOnClick" />
                &nbsp;&nbsp;
                <FMCONTROLS:FMPAGESIZEDROPDOWN id="GridSizeDropDown" runat="server" 
                   onselectedindexchanged="GridSizeDropdownOnChange" />
             </td>
          </tr>
          <tr>
            <td colspan="2">           
               <FMControls:FMGrid id="DataGridAssignedCompanies" runat="server" 
                  BackColor="White" PageSize="10"
                                  CssClass="tabletext" Width="495px" 
                                  BorderWidth="1px" Gridlines="Vertical" AllowPaging="True" 
                  Cellpadding="3" BorderColor="White"
                                  AllowSorting="true" BorderStyle="None" 
                  AutoGenerateColumns="False">
                  <FooterStyle BackColor="<%$ AppSettings: ColorHeaderBlue %>" ForeColor="Black"></FooterStyle>
                  <SelectedItemStyle BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SelectedItemStyle>
                  <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                  <ItemStyle BackColor="#EEEEEE" ForeColor="Black"></ItemStyle>
                  <HeaderStyle CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>" ForeColor="White" Font-Bold="True"></HeaderStyle>
                  <Columns>
                     <asp:TemplateColumn>
	                     <HeaderStyle Width="55px"></HeaderStyle>
	                     <HeaderTemplate><FMControls:FMLabel ID="labDeleteHdr" runat="server" Text="Delete" /></HeaderTemplate>
	                     <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center"></ItemStyle>
	                     <ItemTemplate>
		                     <FMControls:FMDeleteLinkButton id="btnDeleteCompany" runat="server" Name="btnDelete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CompanyGuid")%>'>
		                     &nbsp;
		                     </FMControls:FMDeleteLinkButton>
	                     </ItemTemplate>
	                     <EditItemTemplate>
		                     <FMControls:FMUpdateLinkButton id="btnSaveCompany" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CompanyGuid") %>'>
			                     &nbsp;
			                     </FMControls:FMUpdateLinkButton>&nbsp;
		                     <FMControls:FMCancelLinkButton id="btnCancelCompany" runat="server"></FMControls:FMCancelLinkButton>
	                     </EditItemTemplate>
                     </asp:TemplateColumn>
                     <asp:TemplateColumn>
	                     <HeaderTemplate><FMControls:FMLabel ID="labCompanyHdr" runat="server" Text="Company" /><span style="COLOR: red;">*</span></HeaderTemplate>
	                     <ItemStyle VerticalAlign="Middle" HorizontalAlign="Left" Wrap="False"></ItemStyle>
	                     <ItemTemplate>
		                     <FMControls:FMLabel ID="labCompany" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CompanyID") %>' />
	                     </ItemTemplate>
	                     <EditItemTemplate>
		                     <FMControls:FMCompanyTextBox id="txtCompany" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CompanyID") %>' CssClass="tabletext">
		                     &nbsp;
		                     </FMControls:FMCompanyTextBox>
	                     </EditItemTemplate>
                     </asp:TemplateColumn>
                  </Columns>
                  <PagerStyle BackColor="<%$ AppSettings: ColorHeaderBlue %>" ForeColor="White" HorizontalAlign="Center" Mode="NumericPages"></PagerStyle>
               </FMControls:FMGrid>       
            </td>
          </tr>
          <tr>
             <td colspan="2">
                <FMControls:FMButton ID="AddBottomButton" runat="server" CssClass="formfieldtitle" 
                   Text="Add" Width="65px" onclick="AddBottomButtonOnClick" />
             </td>
          </tr>
          <tr>
            <td colspan="2"; align="right">
               <FMControls:FMButton ID="NewButton" runat="server" CssClass="formfieldtitle" 
                  Text="New" Width="65px" onclick="NewButtonOnClick" />
               &nbsp;&nbsp;
               <FMControls:FMButton ID="OkButton" runat="server" CssClass="formfieldtitle" 
                  Text="OK" Width="65px" onclick="OkButtonOnClick" />
               &nbsp;&nbsp;
               <FMControls:FMButton ID="CancelButton" runat="server" CssClass="formfieldtitle" 
                  Text="Cancel" Width="65px" onclick="CancelButtonOnClick" />
            </td>
          </tr>
       </table>
   </div>
</form>
</body>
   <script type="text/jscript">
	   function CompanySelect(Role,CompanyTextBoxID)
	   {
		   var sFeatures="dialogWidth: 855px; dialogHeight: 560px";
		   var CompanyTextBox = document.getElementById(CompanyTextBoxID);
		   var Result=window.showModalDialog("../FMWebApp/CompanySelectForm.aspx","",sFeatures);
		   if(Result != null)
		   {
			   CompanyTextBox.value=Result[0];
			   CompanyTextBox.title=Result[1];
		   }
	   }
   </script>
</html>
