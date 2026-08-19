<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserPermissionAssignmentForm.aspx.cs" Inherits="FuelsManager.FMWebApp.UserPermissionAssignmentForm" %>
<%@ Register src="../MenuBar/FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css" runat="server">
       .style1
       {
           left: 25px;
           position: relative;
           text-align: left;
       }

       .style2
       {
           width: 234px;
       }

       .style3
       {
           width: 325px;
       }
       
       .VertiColumn 
       {
           padding: 10px 2px 10px 2px;
           white-space: nowrap;
           writing-mode: vertical-rl;
       }

       .comboBoxInPanel
       {
           position: relative;
       }

       .comboBoxInPanel .formfield .ajax__combobox_buttoncontainer button
       {
           height: 20px !important;
           width: 20px !important;
       }

       .comboBoxInPanel ul
       {
           position: absolute !important;
           left: 2px !important;
           top: 28px !important;
           height: auto !important;
           width: 235px !important;
       }

/*       .comboBoxInPanel1
       {
           position: relative;
           z-index: 2000 !important;
       }

      .comboBoxInPanel1 ul
      {
          position: absolute !important;
          left: 2px !important;
          top: 28px !important;
          z-index: 2010 !important;
      }
       
       .comboBoxInPanel1 .formfield .ajax__combobox_buttoncontainer button
       {
           height: 20px !important;
           width: 20px !important;
       }

      .comboBoxInPanel2
      {
          position: relative;
          z-index: 1000 !important;
      }

      .comboBoxInPanel2 ul
      {
          position: absolute !important;
          left: 2px !important;
          top: 28px !important;
          z-index: 1010 !important;
      }

       .comboBoxInPanel2 .formfield .ajax__combobox_buttoncontainer button
       {
           height: 20px !important;
           width: 20px !important;
       }
*/
      .GVFixedHeaderUP 
      {
          font-family: Arial, Helvetica, sans-serif;
          font-size: 12px;
          font-style: normal;
          line-height: normal;
          font-weight: bold;
          font-variant: normal;
          text-transform: none;
          color: #FFFFFF;
          text-align: left;
          vertical-align: middle;
          text-decoration: none;
          top: expression(document.getElementById("pnlContainer").scrollTop-2);
      }

      .GVFixedFooterUP 
      {
          font-family: Arial, Helvetica, sans-serif;
          font-size: 12px;
          font-style: normal;
          line-height: normal;
          font-weight: bold;
          font-variant: normal;
          text-transform: none;
          color: #FFFFFF;
          text-align: center;
          vertical-align: middle;
          text-decoration: none;
          border-left: none 0px white;
          border-right: none 0px white;
          border-top: none 0px white;
          border-bottom: none 0px white;
          bottom: expression(this.parentNode.parentNode.parentNode.scrollHeight - this.parentNode.parentNode.parentNode.scrollTop - this.parentNode.parentNode.parentNode.clientHeight);
      }
    </style>
    <script type="text/javascript">
        $('[type=checkbox]').ready(function () {
            setCheckBoxInitialState();
        });

        function setCheckBoxInitialState() {
            $("[type = 'checkbox']").each(function () {
                var ovAttr = $(this).parent().attr("ov");
                setTriStateInitialState(this, ovAttr);
            });
        }

        function setCheckBoxDisabled(checkBox) {
            checkBox.disabled = true;
            if (isManagedByActiveDirectory(checkBox)) {
                setHoverText(checkBox, "Please use Active Directory to assign this group membership.");
            }
            else {
                setHoverText(checkBox, "Unable to assign this group membership.");
            }
        }

        function setCheckBoxIndeterminate(checkBox) {
            if (isManagedByActiveDirectory(checkBox)) {
                var triStateInput = getTriStateControl(checkBox);
                setHiddenInputValue(triStateInput, "true");
            }
            checkBox.readOnly = checkBox.indeterminate = true;
            checkBox.className = "indeterminate";
            setHoverText(checkBox, "Active Directory has assigned this group membership, but a FuelsManager administrator has removed this group membership.\r\n\r\nClick to restore this group membership.");
        }

        function setCheckBoxChecked(checkBox) {
            if (isManagedByActiveDirectory(checkBox)) {
                var triStateInput = getTriStateControl(checkBox);
                setHiddenInputValue(triStateInput, "false");
                setHoverText(checkBox, "Active Directory has assigned this group membership.\r\n\r\nClick to remove group membership and prevent Active Directory from re-adding group membership.");
            }
            else {
                setHoverText(checkBox, "Click to unassign this group membership.");
            }
            checkBox.readOnly = false;
            checkBox.checked = true;
            checkBox.className = "determinate";
        }

        function setTriStateUnchecked(checkBox) {
            if (isManagedByActiveDirectory(checkBox)) {
                var triStateInput = getTriStateControl();
                setTriStateInputValue(triStateInput, "false");
            }
            checkBox.checked = checkBox.readOnly = false;
            checkBox.className = "determinate";
            setHoverText(checkBox, "Click to assign this group membership.");
        }

        function setTriStateInitialState(checkBox, ovAttr) {
            if (ovAttr === "2") {
                setCheckBoxIndeterminate(checkBox);
            }

            if (ovAttr === "1" || checkBox.checked) {
                setCheckBoxChecked(checkBox);
            }
            // if the user is managed by active directory and the group has not been assigned,
            // then disable the checkBox
            if (isManagedByActiveDirectory(checkBox) && ovAttr === "0") {
                setCheckBoxDisabled(checkBox);
            }

            if (ovAttr === "0" && !isManagedByActiveDirectory(checkBox)) {
                setCheckBoxUnchecked(checkBox);
            }
        }

        function setHiddenInputValue(theControl, newValue) {
            if (theControl !== null && theControl !== undefined) {
                theControl.value = newValue;
            }
        }

        function permissionCheckBoxClick(checkBox) {
            var managedByAD = isManagedByActiveDirectory(checkBox);
            // Implement two state as Check-ThirdState (no unchecked state) for AD users
            if (managedByAD) {
                if (checkBox.readOnly) {
                    setCheckBoxChecked(checkBox);
                }
                else {
                    setCheckBoxIndeterminate(checkBox);
                }
            }
            else {
                if (checkBox.checked) {
                    setCheckBoxChecked(checkBox);
                }
                else {
                    setTriStateUnchecked(checkBox);
                }
            }
        }

        function getTriStateControl(checkBox) {
            return checkBox.parentElement.parentElement.childNodes[1];
        }

        function isManagedByActiveDirectory(checkBox) {
            var triStateInput = getTriStateControl(checkBox);
            var managedByAD = triStateInput != undefined;
            return managedByAD;
        }

        function setHoverText(checkBox, hoverText) {
            checkBox.setAttribute("title", hoverText);
        }

        function setCheckBoxUnchecked(checkBox) {
            if (isManagedByActiveDirectory(checkBox)) {
                var triStateInput = getTriStateControl();
                setTriStateInputValue(triStateInput, "false");
            }
            checkBox.checked = checkBox.readOnly = false;
            checkBox.className = "determinate";
            setHoverText(checkBox, "Click to assign this group membership.");
        }

        function SelectAllCheckboxes(headerchk, headerText) {
            var gvcheck = document.getElementById('UPG');
            var colIndex = GetColumnIndex(gvcheck, headerText);

            for (var i = 3; i < gvcheck.rows.length -1; i++) {
                var inputs = gvcheck.rows[i].cells[colIndex].getElementsByTagName('input');

                if (inputs.length > 0) {
                    inputs[0].checked = headerchk.checked;
                }
            }
        }

        function GetColumnIndex(gridView, headerText) 
        {
            var toRet = 0;
            var curHeader;
            var headerRow = gridView.rows[0];
                
            for (var col = 0; col < headerRow.cells.length; col++) 
            {
                if (document.all) 
                {
                    curHeader = headerRow.cells[col].innerText;
                } 
                else
                {
                    curHeader = headerRow.cells[col].textContent;
                }
                    
                if (headerText == curHeader) {
                    toRet = col;
                    break;
                }
            }

            return toRet;
        }

    </script>
</head>
<body ms_positioning="GridLayout">
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
    <script type="text/javascript" language="javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
    <form id="form1" runat="server" method="post" DefaultButton="FindBtn">
      <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
      <div id="pageContent" style="position:absolute">
      <asp:ScriptManager ID="ScriptManager" runat="server" />

             <FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
    		      <FMControls:FMLabel id="Label1" 
                 style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
			      CssClass="headline" BackColor="Transparent" Text="User Permission Assignment" />
		      <table style="z-index:125; top:48px; position:absolute; left:32px" role="presentation" aria-label="layout" >
		          <tr style="height:20px">
                      <td class="style3">
                          <FMCONTROLS:FMLABEL id="SiteGroupLabel" AssociatedControlID="SiteGroupDropDown$SiteGroupDropDown_TextBox" runat="server" BackColor="Transparent" CssClass="formfieldtitle" 
                              style="Z-INDEX: 123;" Text="Site Groups:" />
                      </td>
                      <td class="style1">
                          <FMControls:FMLABEL ID="UserLabel" AssociatedControlID="UserDropdown$UserDropdown_TextBox" runat="server" BackColor="Transparent" CssClass="formfieldtitle" Text="User:" />
                      </td>
                  </tr>
                  <tr style="height:20px">
                      <td class="style3">
                        <asp:Panel ID="SiteGroupComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                           <FMControls:FMComboBox ID="SiteGroupDropDown" runat="server" 
                             CssClass="formfield" AutoPostBack="true" Width="200px" 
                             onselectedindexchanged="SiteGroupSelectionChange" 
                               DropDownStyle="DropDownList" AutoCompleteMode="SuggestAppend"/>
      			        </asp:Panel>
                      </td>
                      <td class = "style1">
                        <asp:Panel ID="UserComboBoxPanel" runat="server" CssClass="comboBoxInPanel" Height="33px">
                          <FMControls:FMComboBox ID="UserDropdown" runat="server" 
                             CssClass="formfield" AutoPostBack="true" Width="200px" 
                             onselectedindexchanged="UserSelectionChange" DropDownStyle="DropDownList" AutoCompleteMode="SuggestAppend" />
      			        </asp:Panel>
                      </td>
                  </tr>
                  <tr>
                      <td class="style3">
                          <FMControls:FMLABEL ID="SitesLabel" AssociatedControlID="SiteDropDown$SiteDropDown_TextBox" runat="server" BackColor="Transparent" 
                            CssClass="formfieldtitle" Text="Sites:" />
                      </td>
                      <td class="style1">
                          <FMControls:FMLABEL ID="FindLabel" AssociatedControlID="FindTextBox" runat="server" BackColor="Transparent" 
                             CssClass="formfieldtitle" Text="Find String:" />
                      </td>
                  </tr>
                  <tr>
                     <td class="style3">
                        <asp:Panel ID="Panel1" runat="server" CssClass="comboBoxInPanel" Height="33px">
                          <FMControls:FMComboBox ID="SiteDropDown" title="Select site" runat="server" 
                           CssClass="formfield" Width="200px" AutoPostBack="True" 
                             onselectedindexchanged="SiteDropDownSelectChange" DropDownStyle="DropDownList" AutoCompleteMode="SuggestAppend"/>
      			        </asp:Panel>
                      </td>
                     <td class="style1">
                          <asp:TextBox ID="FindTextBox" runat="server" Width="314px" MaxLength="100" valign="center"></asp:TextBox>
                      </td>
                  </tr>
                  <tr>
                     <td class="style3">
                     </td>
                     <td class="style1">
                          <FMControls:FMButton ID="FindBtn" runat="server" CssClass="formfieldtitle" 
                             Text="Find" Width="66px" onclick="FindBtn_OnClick" />&nbsp;&nbsp;
                          <FMControls:FMButton ID="ShowAllBtn" runat="server" CssClass="formfieldtitle" 
                             Text="Show All" Width="66px" onclick="ShowAllBtn_OnClick" />
                      </td>
                  </tr>
                  <tr valign="bottom">
                      <td colspan="2" class="style2">
                          <FMControls:FMButton ID="TopApplyButton" runat="server" CssClass="formfieldtitle" 
                                               Text="Save" Width="73px" onclick="ApplyBtn_Onclick" />
                          <FMControls:FMButton ID="TopCloseBtn" runat="server" CssClass="formfieldtitle"
                                                Text="Close" Width="73px" OnClick="CloseBtn_Onclick" />      
                          <FMControls:FMPageSizeDropDown ID="CompanySummaryPageSizeDropDown" ToolTip="Page size" runat="server" onselectedindexchanged="PageSizeDropDownSelectedIndexChanged" />                                                                                 
                      </td>
                  </tr>
                  <tr>
                      <td colspan="2" style="padding-top: 4px; padding-bottom: 4px">
                          <FMCONTROLS:FMDATAGRIDFixedPaging id="UPG" runat="server" 
                             BackColor="White" Width="100%" CssClass="tabletext" CellPadding="3" 
                             BorderColor="White" AllowSorting="True" 
                             BorderWidth="1px" GridLines="Vertical"
			                  BorderStyle="None" AllowPaging="True" PageSize="50" AutoGenerateColumns="False" 
                              EmptyDataText="No records found" ShowFooter="False"
								ShowHeaderWhenEmpty="True"
								ShowFooterWhenEmpty="False" HeaderStyle-CssClass="GVFixedHeaderUP"
                             onsortcommand="UserPermissionGridSortCommand"
							  aria-label="User Permission Assignments">
                              <AlternatingItemStyle BackColor="Gainsboro"/>
			                  <ItemStyle ForeColor="Black" BackColor="#EEEEEE" />
                              <PagerStyle CssClass="GVFixedFooterUP" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages" />
			                  <Columns>
			                      <asp:BoundColumn DataField="SiteID" SortExpression="SiteID" HeaderText="Sites">
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
			                      </asp:BoundColumn>
			                      <asp:BoundColumn DataField="UserID" SortExpression="UserID" HeaderText="User">
                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
			                      </asp:BoundColumn>
			                      <asp:BoundColumn DataField="SiteGuid" Visible="false" />
			                      <asp:BoundColumn DataField="UserGuid" Visible="false" />
			                  </Columns>
                              <HeaderStyle CssClass="GVHeaderFixed" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" />
                              <PagerStyle CssClass="GVFixedFooter" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages" />
		                  </FMCONTROLS:FMDATAGRIDFixedPaging>
                          <FMControls:FMLabel runat="server" ID="noResultsLabel" CssClass="formfieldtitle" >No results found.   Please make sure you have permissions to modify users at selected site(s).</FMControls:FMLabel>
                      </td>
                  </tr>
                  <tr>
                     <td class="style3">
                        <FMControls:FMButton ID="BottomApplyButton" runat="server" CssClass="formfieldtitle" 
                        Text="Save" Width="73px" onclick="ApplyBtn_Onclick" />
                          <FMControls:FMButton ID="BottomCloseBtn" runat="server" CssClass="formfieldtitle"
                                                Text="Close" Width="73px" OnClick="CloseBtn_Onclick" />                        
                     </td>
                  </tr>
              </table>
	</div>
   </form>
</body>
</html>
