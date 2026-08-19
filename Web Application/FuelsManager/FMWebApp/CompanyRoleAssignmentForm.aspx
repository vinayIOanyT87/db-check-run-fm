<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyRoleAssignmentForm.aspx.cs" Inherits="FuelsManager.FMWebApp.CompanyRoleAssignmentForm"  %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	<script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
    <style type="text/css">
       .style1
       {
          width: 537px;
       }
       .style2
       {
          width: 234px;
       }
       .style3
       {
          width: 325px;
       }
    </style>
</head>
<body MS_POSITIONING="GridLayout">
    <form id="form1" runat="server" method="post" DefaultButton="FindBtn">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
      <asp:ScriptManager ID="ScriptManager" runat="server" />
      <asp:UpdatePanel ID="UpdatePanel1" runat="server">
         <ContentTemplate>
         	<SCRIPT>
         	   function CompanySelect(role, companyTextBoxId)
         	   {
         	      var companyTextBox = document.getElementById(companyTextBoxId);

         	      showModalDialogFrame({
         	          url: "../FMWebApp/CompanySelectForm.aspx?All=true",
         	          width: 855,
         	          height: 690,
         	          title: "Select Company",
         	          onClose: function ()
         	          {
         	              if (this.returnValue != null && this.returnValue.length > 0)
         	              {
         	                  var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
         	                  companyTextBox.value = asciiValue1;

         	                  if (this.returnValue.length === 2)
         	                  {
         	                      var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);
         	                      companyTextBox.value += "|" + asciiValue2;
         	                  }

         	                  companyTextBox.onchange();
         	              }
         	          }
         	      });

         	   }
            </SCRIPT>
		      <asp:image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
			      ImageUrl="<%$ AppSettings: PageFadeImage %>" CssClass="formfieldtitle" />
		      <FMControls:FMLabel id="Label1" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
			      CssClass="headline" BackColor="Transparent" Text="Company Role Assignment" />
      			
		      <table style="z-index:125; top:40px; position:absolute; left:20px" >
		          <tr style="height:20px">
                      <td class="style3">
                          <FMCONTROLS:FMLABEL id="CompanyLable" AssociatedControlID="CompanyTextBox" runat="server" BackColor="Transparent" CssClass="formfieldtitle" 
                              style="Z-INDEX: 123;" Text="Company:" />
                      </td>
                      <td class="style1">
                          <FMControls:FMLABEL ID="FindLabel" AssociatedControlID="FindTextBox" runat="server" BackColor="Transparent" 
                             CssClass="formfieldtitle" Text="Find String:" />
                      </td>
                  </tr>
                  <tr style="height:20px">
                      <td class="style3">
                          <FMControls:FMCompanyTextBox ID="CompanyTextBox" ToolTip="Company" runat="server" 
                             CssClass="formfield" AutoPostBack="True" Width="200px" 
                             ontextchanged="CompanySelectionOnTextChange" />
                      </td>
                      <td class="style1">
                          <asp:TextBox ID="FindTextBox" runat="server" Width="314px" MaxLength="100"></asp:TextBox>&nbsp;&nbsp;
                          <FMControls:FMButton ID="FindBtn" runat="server" CssClass="formfieldtitle" 
                             Text="Find" Width="66px" onclick="FindBtn_OnClick" />&nbsp;&nbsp;
                          <FMControls:FMButton ID="ShowAllBtn" runat="server" CssClass="formfieldtitle" 
                             Text="Show All" Width="66px" onclick="ShowAllBtn_OnClick" />
                      </td>
                  </tr>
                  <tr>
                      <td class="style3">
                          <FMControls:FMLABEL ID="RoleLabel" AssociatedControlID="RoleDropDown" runat="server" BackColor="Transparent" 
                            CssClass="formfieldtitle" Text="Company Roles:" />
                      </td>
                      <td class="style1">
                          <FMControls:FMLABEL ID="SiteLabel" AssociatedControlID="SiteDropDown" runat="server" BackColor="Transparent" CssClass="formfieldtitle" Text="Site:" />
                      </td>
                  </tr>
                  <tr>
                     <td class="style3">
                          <FMControls:FMDropDownList ID="RoleDropDown" runat="server" 
                           CssClass="formfield" Width="200px" AutoPostBack="True" 
                             onselectedindexchanged="CompanyRoleSelectChange" />
                      </td>
                     <td class="style1">
                          <FMControls:FMDropDownList ID="SiteDropDown" runat="server" 
                             CssClass="formfield" AutoPostBack="true" Width="200px" 
                             onselectedindexchanged="SiteSelectionChange" />
                      </td>
                  </tr>
                  <tr>
                     <td class="style3">
                     </td>
                     <td class="style1">
                        <FMControls:FMCheckBox ID="IncludeMemberSitesCheckBox" runat="server" 
                           Text="Include member sites" CssClass="formfieldtitle" AutoPostBack="True" 
                           oncheckedchanged="IncludeMemberSiteChange"/>
                     </td>
                  </tr>
                  <tr valign="bottom">
                      <td colspan="2" class="style2">
                          <FMControls:FMButton ID="TopApplyButton" runat="server" CssClass="formfieldtitle" 
                                               Text="Apply" Width="73px" onclick="ApplyBtn_Onclick" />
                      </td>
                  </tr>
                  <tr>
                      <td colspan="2">
                          <FMCONTROLS:FMDATAGRIDFIXED id="CompanyRolesGrid" runat="server" RowHeaderColumn="ID,Site"
                             BackColor="White" Width="934px" CssClass="tabletext" CellPadding="3" 
                             BorderColor="White" AllowSorting="True" 
                             BorderWidth="1px" GridLines="Vertical"
			                  BorderStyle="None" AllowPaging="True" AutoGenerateColumns="False" 
                             onsortcommand="CompanyRolesGridSortCommand">
			                  <SelectedItemStyle Font-Bold="True" ForeColor="White" CssClass="tablelink" BackColor="#008A8C" />
			                  <AlternatingItemStyle BackColor="Gainsboro" />
			                  <ItemStyle ForeColor="Black" BackColor="#EEEEEE" />
			                  <Columns>
				                  <asp:TemplateColumn HeaderText="ID" SortExpression="ID">
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
				                          <FMControls:FMLabel ID="IDLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CompanyID") %>'/>
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
				                  <asp:TemplateColumn HeaderText="Name" SortExpression="Name">
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
				                          <FMControls:FMLabel ID="NameLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CompanyName") %>'/>
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
				                  <asp:TemplateColumn HeaderText="Site" SortExpression="Site">
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
				                          <FMControls:FMLabel ID="SiteLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "SiteID") %>'/>
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
				                  <asp:TemplateColumn HeaderText="Manager" SortExpression="Manager">
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
						                  <FMControls:FMCheckBox id="ManagerCheckbox" ToolTip="Manager" runat="server" Checked="false" 
                                        Enabled="true" />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
				                  <asp:TemplateColumn HeaderText="Owner" SortExpression="Owner">
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
						                  <FMControls:FMCheckBox id="OwnerCheckbox" ToolTip="Owner" runat="server" Checked="false" 
                                        Enabled="true" />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
				                  <asp:TemplateColumn HeaderText="Shipper" SortExpression="Shipper">
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
						                  <FMControls:FMCheckBox id="ShipperCheckbox" ToolTip="Shipper" runat="server" Checked="false" 
                                        Enabled="true" />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
				                  <asp:TemplateColumn HeaderText="Bill To" SortExpression="BillTo">
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
						                  <FMControls:FMCheckBox id="BillToCheckbox" ToolTip="Bill to" runat="server" Checked="false" 
                                        Enabled="true" />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
				                  <asp:TemplateColumn HeaderText="Ship To" SortExpression="ShipTo">
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
						                  <FMControls:FMCheckBox id="ShipToCheckbox" ToolTip="Ship to" runat="server" Checked="false" 
                                        Enabled="true" />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
				                  <asp:TemplateColumn HeaderText="Carrier" SortExpression="Carrier">
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
						                  <FMControls:FMCheckBox id="CarrierCheckbox" ToolTip="Carrier" runat="server" Checked="false" 
                                        Enabled="true" />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
				                  <asp:TemplateColumn HeaderText="Supplier" SortExpression="Supplier">
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
						                  <FMControls:FMCheckBox id="SupplierCheckbox" ToolTip="Supplier" runat="server" Checked="false" 
                                        Enabled="true" />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
			                  </Columns>
		                  </FMCONTROLS:FMDATAGRIDFIXED>
                      </td>
                  </tr>
                  <tr>
                     <td class="style3">
                        <FMControls:FMButton ID="BottomApplyButton" runat="server" CssClass="formfieldtitle" 
                        Text="Apply" Width="73px" onclick="ApplyBottomBtn_OnClick" />
                     </td>
                  </tr>
              </table>
         </ContentTemplate>
      </asp:UpdatePanel>
    </div>
</form>
</body>
</html>
