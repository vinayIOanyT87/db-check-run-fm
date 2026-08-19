<%@ Page language="c#" Codebehind="FieldLevelConfigForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.FieldLevelConfigForm" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="FMBusinessObjects.DataObjects" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<!DOCTYPE html>
<html>
	<head>
		<title></title>
	    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
	    <meta name="CODE_LANGUAGE" content="C#"/>
	    <meta name="vs_defaultClientScript" content="JavaScript" />
	    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
	    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
		<style type="text/css">
           .style1
           {
                width: 276px;
            }
           .style2
           {
              width: 234px;
           }
            .style6
            {
                width: 217px;
            }
            .style8
            {
                width: 217px;
                height: 20px;
            }
        </style>
	</head>
	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="FieldLevelConfigForm" method="post" encType="multipart/form-data"  runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
		      <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
		      <FMControls:FMLabel id="FieldLevelConfigTitleLabel" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
			      CssClass="headline" BackColor="Transparent" Text="Field Level Control Configuration" /> 			
		      <table style="z-index:125; top:39px; position:absolute; left:20px; width: 1302px;" role="presentation" aria-label="layout" >
		          <tr>
                      <td class="style6">
                          <FMCONTROLS:FMLABEL id="EntityTypeLabel" AssociatedControlID="EntityTypeDropdown" runat="server" BackColor="Transparent" CssClass="formfieldtitle" 
                               Text="Entity Type" />
                      </td>
                      <td class="style6">
                          <FMControls:FMLABEL ID="SiteGroupLabel" AssociatedControlID="SiteGroupDropdown" runat="server" BackColor="Transparent" 
                             CssClass="formfieldtitle" Text="Site Group" />
                      </td>
                       <td class="style6">
                          <FMControls:FMLABEL ID="FilterValueLabel" AssociatedControlID="FilterValueDropdown" runat="server" BackColor="Transparent" 
                               CssClass="formfieldtitle" Text="Filter Value" />
                      </td>
                      <td class="style6">
                          <FMControls:FMLABEL ID="TargetFieldLabel" AssociatedControlID="TargetFieldDropdown" runat="server" BackColor="Transparent" 
                               CssClass="formfieldtitle" Text="Target Field" />
                      </td>
                      <td class="style6">
                          <FMControls:FMLABEL ID="ControlModeLabel" AssociatedControlID="ControlModeDropdown" runat="server" BackColor="Transparent" 
                               CssClass="formfieldtitle" Text="Control Mode" />
                      </td>
                      <td class="style6">
                          &nbsp;</td>
                 </tr>
                  <tr>
                      <td class="style6">
                          <FMControls:FMDropDownList ID="EntityTypeDropdown" runat="server" 
                             CssClass="formfield" AutoPostBack="True" Width="200px" 
                              onselectedindexchanged="EntityTypeDropdownSelectedIndexChanged"  />
                      </td>
                      <td class="style6">
                          <asp:DropDownList ID="SiteGroupDropdown" runat="server" CssClass="formfield" 
                              Width="200px"  AutoPostBack="True" onselectedindexchanged="SiteGroupDropdownSelectedIndexChanged" 
                              />
                      </td>
                      <td class="style6">
                          <FMControls:FMDropDownList ID="FilterValueDropdown" runat="server" 
                             CssClass="formfield" AutoPostBack="true" Width="200px" onselectedindexchanged="FilterValueDropDownSelectedIndexChanged"
                              />
                      </td>
                      <td class="style6">
                          <FMControls:FMDropDownList ID="TargetFieldDropdown" runat="server" 
                             CssClass="formfield" AutoPostBack="true" Width="200px" onselectedindexchanged="TargetFieldDropDownSelectedIndexChanged" 
                              />
                      </td>
                      <td class="style6">
                          <FMControls:FMDropDownList ID="ControlModeDropdown" runat="server" 
                             CssClass="formfield" AutoPostBack="true" Width="200px" onselectedindexchanged="ControlModeDropDownSelectedIndexChanged"
                              />
                      </td>
                      <td class="style6">
                          &nbsp;</td>
                  </tr>
                  <tr>
                     <td class="style6">
                     </td>                     
                     <td class="style6">
                        <FMControls:FMCheckBox ID="IncludeMemberSiteGroupsCheckBox" runat="server" 
                           Text="Include member site groups" CssClass="formfieldtitle" 
                             AutoPostBack="True" oncheckedchanged="IncludeMemberSiteGroupsCheckBoxCheckedChanged" 
                           />
                     </td>
                     <td class="style6">
                     </td>
                     <td class="style6">
                     </td>
                     <td class="style6">
                     </td>
                     <td class="style6">
                     </td>
                  </tr>
                  <tr valign="bottom">
                      <td class="style6">
                          <FMControls:FMButton ID="TopApplyBtn" runat="server" CssClass="formfieldtitle" 
                                               Text="Apply" Width="73px" onclick="ApplyBtn_Onclick" />
                            <ajaxToolkit:ConfirmButtonExtender ID="cbeTopApply" runat="server" TargetControlID="TopApplyBtn" Enabled="true" 
                               ConfirmText="Field Level Control configuration changes impact Entity Record Versions. It can lead to entity record version data being overridden and/or record versions being deleted. Are you sure you want to proceed with the configuration changes?" />

                      </td>
                      <td colspan="2" class="style6">
                          <FMControls:FMButton ID="TopCheckAllButton" runat="server" CssClass="formfieldtitle" 
                                               Text="Check All" Width="90px" onclick="TopCheckAllButtonClick" 
                              />
                          &nbsp;&nbsp;
                          <FMControls:FMButton ID="TopUncheckAllButton" runat="server" CssClass="formfieldtitle" 
                                               Text="Uncheck All" Width="90px" onclick="TopUncheckAllButtonClick" 
                               />
                      </td>
                  </tr>
                  <tr>
                      <td colspan="6">
                          <FMCONTROLS:FMDATAGRIDFIXED id="FieldLevelConfigGrid" runat="server" 
                             BackColor="White" Width="1302px" CssClass="tabletext" CellPadding="3"  RowHeaderColumn="Entity Type"
                             BorderColor="White" AllowSorting="True" 
                             BorderWidth="1px" GridLines="Vertical"
			                  BorderStyle="None" AutoGenerateColumns="False" 
                             onsortcommand="FieldLevelConfigGridSortCommand" FixedHeaders="True" 
                              FixedHeight="550px" Height="550px" ShowFooter="True" 
                              onitemdatabound="FieldLevelConfigGridItemDataBound"
							      aria-label="Field Levels">
			                  <SelectedItemStyle Font-Bold="True" ForeColor="White" CssClass="tablelink" BackColor="#008A8C" />
			                  <AlternatingItemStyle BackColor="Gainsboro" />
			                  <ItemStyle ForeColor="Black" BackColor="#EEEEEE" />
			                  <Columns>
				                  <asp:TemplateColumn HeaderText="Entity Type" SortExpression="EntityTypeId">
				                      <HeaderStyle Width="290px" />
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
                                          <asp:Label ID="FieldLevelConfigMatrixIndexColLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.FieldLevelConfigMatrixIndex") %>' Visible="false" />
                                          <asp:Label ID="EntityTypeDisplayColLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.EntityTypeDisplayName") %>' />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
				                  <asp:TemplateColumn HeaderText="Site Group" SortExpression="SiteGroupId">
				                      <HeaderStyle Width="290px" />
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
				                          <asp:Label ID="SiteGroupIdColLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGroupId") %>' />
                                          <asp:Label ID="SiteGroupGuidColLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SiteGroupGuid") %>' Visible="false" />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
                                  <asp:TemplateColumn HeaderText="Filter" SortExpression="FilterDisplayName" Visible ="false">
				                      <HeaderStyle Width="215px" />
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
				                          <asp:Label ID="FilterColLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.FilterDisplayName") %>' />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
                                  <asp:TemplateColumn HeaderText="Filter Value" SortExpression="FilterValueName">
				                      <HeaderStyle Width="290px" />
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
				                          <asp:Label ID="FilterValueColLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.FilterValueName") %>' />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
                                  <asp:TemplateColumn HeaderText="Target Field DB" SortExpression="TargetField" Visible = "false">
				                      <HeaderStyle Width="215px" />
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
				                          <asp:Label ID="TargetFieldColLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.TargetField") %>' />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
                                       <asp:TemplateColumn HeaderText="Target Field" SortExpression="TargetFieldDisplay">
				                      <HeaderStyle Width="290px" />
				                      <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
				                      <ItemTemplate>
				                          <asp:Label ID="TargetFieldDisplayColLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.TargetFieldDisplay") %>' />
				                      </ItemTemplate>
				                  </asp:TemplateColumn>
                                  <asp:TemplateColumn HeaderText="Child Control" SortExpression="ForwardControlMode">
				                      <ItemTemplate>
                                          <FMControls:FMCheckBox ID="VerSpecificColCheckBox" runat="server" Checked='<%#Convert.ToBoolean(DataBinder.Eval(Container, "DataItem.IsFCMVerSpecific"))%>'/>
                                          <FMControls:FMCheckBox ID="OriginalVerSpecificColCheckBox" runat="server" Checked='<%#Convert.ToBoolean(DataBinder.Eval(Container, "DataItem.IsFCMVerSpecific"))%>' Visible="false" />
                                          <asp:Label ID="InheritedControlModeColLabel" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.InheritedControlMode") %>' Visible="false" />
                                      </ItemTemplate>
				                      <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
				                  </asp:TemplateColumn>
                                  <asp:TemplateColumn HeaderText="Global Control" SortExpression="ForwardControlMode">
				                      <ItemTemplate>
                                          <FMControls:FMCheckBox ID="GlobalSpecificColCheckBox" runat="server" Checked='<%#Convert.ToBoolean(DataBinder.Eval(Container, "DataItem.IsFCMGlobalSpecific"))%>'/>
                                          <FMControls:FMCheckBox ID="OriginalGlobalSpecificColCheckBox" runat="server" Checked='<%#Convert.ToBoolean(DataBinder.Eval(Container, "DataItem.IsFCMGlobalSpecific"))%>' Visible="false" />
                                      </ItemTemplate>
				                      <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
				                  </asp:TemplateColumn>
			                  </Columns>
		                  </FMCONTROLS:FMDATAGRIDFIXED>
                      </td>
                  </tr>
                  <tr>
                      <td colspan="3" class="style6">
                          <FMControls:FMButton ID="BottomApplyBtn" runat="server" CssClass="formfieldtitle" 
                                               Text="Apply" Width="73px" onclick="ApplyBtn_Onclick" />
                          <ajaxToolkit:ConfirmButtonExtender ID="cbeBottomApply" runat="server" TargetControlID="BottomApplyBtn" Enabled="true" 
                            ConfirmText="Field Level Control configuration changes impact Entity Record Versions. It can lead to entity record version data being overridden and/or record versions being deleted. Are you sure you want to proceed with the configuration changes?" />

                      </td>
                  </tr>
              </table>		
        </div>
        <asp:ObjectDataSource ID="odsFieldLevelConfig" runat="server">
        </asp:ObjectDataSource>
</form>
	</body>
</html>

