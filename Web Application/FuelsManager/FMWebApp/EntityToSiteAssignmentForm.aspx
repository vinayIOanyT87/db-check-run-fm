<%@ Page language="c#" Codebehind="EntityToSiteAssignmentForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EntityToSiteAssignmentForm" %>
<%@ Import Namespace="FMBusinessObjects.DataObjects" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>

<!DOCTYPE html>
<html>
	<head>
		<title></title>
	    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
	    <meta name="CODE_LANGUAGE" content="C#"/>
	    <meta name="vs_defaultClientScript" content="JavaScript"/>
	    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
 		<script type="text/javascript">    	rndTokenStr = '<%= Security.CSRFToken%>';    </script>
        <script type="text/javascript" src='../Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js'></script>
	    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
		<style type="text/css" runat="server">
           .style1 {
                width: 276px;
            }
           .style2 {
              width: 234px;
           }
            .style6 {
                width: 217px;
            }
            .style7 {
                width: 373px;
            }
        </style>
	</head>
    <script type="text/javascript">
        function ShowConfirmationDialogAndClickButton(confirmMessage, controlName) {
            // Without setTimeout(), the controls on the page are not rendered before the dialog displays. 
            // In other words, the dialog pops up over an empty form.
            setTimeout(function () {
                // If the user says OK, then click the Button
                if (confirm(confirmMessage)) {
                    document.getElementById(controlName).click();
                }
            }, 0);
        }

        function SetCheckboxSelection(value)
        {
            $("#Grid").find("[type='checkbox']").each(function ()
            {
                if ($(this).prop("disabled") === false)
                {
                    $(this).prop('checked', value);
                }                
            });
        }

    </script>

	<body MS_POSITIONING="GridLayout" tabindex="-1">
		<form id="EntityAssignmentForm" method="post" encType="multipart/form-data"  runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />		
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
			<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			<FMControls:FMLabel ID="EntityAssignmentTitleLabel" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
				CssClass="headline" BackColor="Transparent" Text="Entity Assignment Configuration" />
			<table style="z-index: 125; top: 39px; position: absolute; left: 20px; width: 812px;" role="presentation" aria-label="layout">
				<tr style="height: 20px">
					<td class="style6">
						<FMControls:FMLabel ID="EntityTypeLabel" AssociatedControlID="EntityTypeDropdown" runat="server" BackColor="Transparent" CssClass="formfieldtitle"
							Style="z-index: 123;" Text="Entity Type" />
					</td>
					<td class="style7">
						<FMControls:FMLabel ID="EntityLabel" AssociatedControlID="EntityDropdown" runat="server" BackColor="Transparent"
							CssClass="formfieldtitle" Text="Entity" />
					</td>
					<td class="style1">
						<FMControls:FMLabel ID="SiteLabel" AssociatedControlID="SiteDropDown" runat="server" BackColor="Transparent"
							CssClass="formfieldtitle" Text="Site Assigning To" />
					</td>
				</tr>
				<tr style="height: 20px">
					<td class="style6">
						<FMControls:FMDropDownList ID="EntityTypeDropdown" runat="server"
							CssClass="formfield" AutoPostBack="True" Width="200px"
							OnSelectedIndexChanged="EntityTypeSelectChange" />
					</td>
					<td class="style7">
						<asp:DropDownList ID="EntityDropdown" runat="server" CssClass="formfield"
							Width="270px" AutoPostBack="True"
							OnSelectedIndexChanged="EntitySelectChange" />
					</td>
					<td class="style1">
						<FMControls:FMDropDownList ID="SiteDropDown" runat="server"
							CssClass="formfield" AutoPostBack="true" Width="253px"
							OnSelectedIndexChanged="SiteSelectionChange" AppendDataBoundItems="true" />

					</td>
				</tr>
				<tr>
					<td class="style6"></td>
					<td class="style7"></td>
					<td class="style1">
						<FMControls:FMCheckBox ID="IncludeMemberSitesCheckBox" runat="server"
							Text="Include member sites" CssClass="formfieldtitle" AutoPostBack="True"
							OnCheckedChanged="IncludeMemberSiteChange" />
					</td>
				</tr>
				<tr valign="bottom">
					<td class="style6">
						<FMControls:FMButton ID="TopApplyBtn" runat="server" CssClass="formfieldtitle"
							Text="Apply" Width="73px" OnClick="ApplyBtn_Onclick" />
						<FMControls:FMButton ID="TopCloseBtn" runat="server" CssClass="formfieldtitle"
							Text="Close" Width="73px" OnClick="CloseBtn_Onclick" />

                      </td>
                      <td colspan="2" class="style2">
                          <FMControls:FMButton ID="TopAssignAllButton" runat="server" CssClass="formfieldtitle" 
                            Text="Assign All" Width="90px" OnClientClick="SetCheckboxSelection(true); return false;"/>
                          &nbsp;&nbsp;
                          <FMControls:FMButton ID="TopUnassignAllButton" runat="server" CssClass="formfieldtitle" 
                            Text="Unassign All" Width="90px" OnClientClick="SetCheckboxSelection(false); return false;" />
                      </td>
                  </tr>
                  <tr>
                      <td colspan="3">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                              <FMCONTROLS:FMGridView id="Grid" runat="server"
                                 BackColor="White" Width="809px" CssClass="tabletext" CellPadding="3" RowHeaderColumn="Entity,Assigned To Site"
                                 BorderColor="White" AllowSorting="True" AllowPaging="False"
                                 BorderWidth="1px" GridLines="Vertical"
                                 BorderStyle="None" AutoGenerateColumns="False" FixedHeaders="true" 
                                 FixedHeight="550px" Height="550px" ShowFooter="True" aria-label="Entity to Site Assignments">
			                      <Columns>
  				                      <asp:TemplateField HeaderText="Assigned">                                                                     
   				                          <ItemTemplate>
                                              <asp:CheckBox ID="ACB" runat="server" Checked='<%# ((EntityToSiteMapClass)Container.DataItem).IsAssigned %>'/>
                                          </ItemTemplate>
				                          <HeaderStyle Width="70px" />
				                      </asp:TemplateField>
  				                      <asp:TemplateField HeaderText="Entity" SortExpression="Entity">                                                                     
   				                          <ItemTemplate>
   				                              <asp:Label CssClass="formfield" runat="server" Text="<%# ((EntityToSiteMapClass)Container.DataItem).ID %>"></asp:Label>
                                          </ItemTemplate>
				                          <HeaderStyle Width="250px" />
				                      </asp:TemplateField>
  				                      <asp:TemplateField HeaderText="Assigned To Site" SortExpression="Site">                                                                     
   				                          <ItemTemplate>
   				                              <asp:Label CssClass="formfield" runat="server" Text="<%# ((EntityToSiteMapClass)Container.DataItem).SiteID %>"></asp:Label>
                                          </ItemTemplate>
				                          <HeaderStyle Width="240px" />
				                      </asp:TemplateField>
  				                      <asp:TemplateField HeaderText="Assigned From Site" SortExpression="AssignedFromSite">                                                                     
   				                          <ItemTemplate>
   				                              <asp:Label CssClass="formfield" runat="server" Text="<%# ((EntityToSiteMapClass)Container.DataItem).AssignedFromSiteId %>"></asp:Label>
                                          </ItemTemplate>
				                          <HeaderStyle Width="240px" />
				                      </asp:TemplateField>
  				                      <asp:TemplateField Visible="False">                                                                     
   				                          <ItemTemplate>
   				                              <asp:CheckBox ID="DisableSelectionCheckbox" Visible="False" runat="server" Checked="<%# ((EntityToSiteMapClass)Container.DataItem).DisableSelection %>"></asp:CheckBox>
                                          </ItemTemplate>
				                      </asp:TemplateField>
			                      </Columns>
		                      </FMCONTROLS:FMGridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                      </td>
                  </tr>
                  <tr>
                      <td colspan="4" class="style2">                          
                          <FMControls:FMButton ID="BottomApplyBtn" runat="server" CssClass="formfieldtitle" 
                                               Text="Apply" Width="73px" 
                              onclick="ApplyBtn_Onclick" />
                          <FMControls:FMButton ID="HiddenApplyButton" Style="display: none"
                            runat="server" OnClick="HiddenApplyBtnClick" />
                          <FMControls:FMButton ID="BottomCloseBtn" runat="server" CssClass="formfieldtitle"
                                                Text="Close" Width="73px" OnClick="CloseBtn_Onclick" />
                      </td>
                  </tr>
              </table>	
        </div>
    </form>
	</body>
</html>

