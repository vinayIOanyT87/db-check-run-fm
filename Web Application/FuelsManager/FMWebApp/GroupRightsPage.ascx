<%@ Control Language="c#" AutoEventWireup="True" Codebehind="GroupRightsPage.ascx.cs" Inherits="FuelsManager.FMWebApp.GroupRightsPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body>
		<FMControls:FMTextbox ID="FilterBox" runat="server" style="Z-INDEX: 122; LEFT: 32px; POSITION: absolute; top: 3px;"></FMControls:FMTextbox>
		<img alt="Filter Rights" title="Search" src="../fmwebapp/images/Search Icon.png" style="Z-INDEX: 122; LEFT: 8px; POSITION: absolute; top: 6px;">
			
        <table style="position: absolute; top: 40px;">
            <tr>
                <td>
                    <FMControls:FMLabel id="AssignedLbl" AssociatedControlID="AssignedRightsListBox" 
                        style="Z-INDEX: 122;" runat="server" CssClass="formfieldtitle">Assigned Rights:</FMControls:FMLabel>
                </td>
                <td></td>
                <td>
                    <FMControls:FMLabel id="Label4" AssociatedControlID="UnassignedRightsListBox" 
                        style="Z-INDEX: 113;" runat="server" CssClass="formfieldtitle" Width="120px">Unassigned Rights:</FMControls:FMLabel>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMListBox id="AssignedRightsListBox" style="Z-INDEX: 108;"
			            runat="server" Width="270px" Height="256px" SelectionMode="Multiple" CssClass="formfield" tabIndex="1">
                    </FMControls:FMListBox>
                </td>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:button id="AssignRightsButton" style="Z-INDEX: 115;"
			                        runat="server" Text="<<" CssClass="formfieldtitle" tabIndex="2" Height="25px">
                                </asp:button>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp</td>
                        </tr>
                        <tr>
                            <td>
		                        <asp:button id="UnassignRightsButton" style="Z-INDEX: 118;"
			                        runat="server" Text=">>" CssClass="formfieldtitle" tabIndex="3" Height="25px">
		                        </asp:button>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <FMControls:FMListBox id="UnassignedRightsListBox" style="Z-INDEX: 110;"
			            runat="server" Width="270px" Height="256px" SelectionMode="Multiple" CssClass="formfield" tabIndex="4">
                    </FMControls:FMListBox>
                </td>
            </tr>
        </table>

	</body>

   <script type="text/javascript">

		//global variables
    	var keysAssigned = [];
    	var valuesAssigned = [];
    	var keysUnAssigned = [];
    	var valuesUnAssigned = [];
    	var filter = $('#<% = FilterBox.ClientID %>').val();
    	var options = $('#<% = AssignedRightsListBox.ClientID %> option');

		//filter method
    	function DoListBoxFilter(listBoxSelector, filter, keys, values) {
    		var list = $(listBoxSelector);
    		list.empty();
    		for (i = 0; i < values.length; ++i) {
    			var value = values[i];
    			if (value == "" || value.toLowerCase().indexOf(filter.toLowerCase()) >= 0) {
    				var temp = '<option value="' + keys[i] + '">' + value + '</option>';
    				list.append(temp);
    			}
    		}
    	}

		//listener
   	$('#<% = FilterBox.ClientID %>').on('input', function () {
   		var filter = $(this).val();
    		DoListBoxFilter('#<% = AssignedRightsListBox.ClientID %>', filter, keysAssigned, valuesAssigned);
         DoListBoxFilter('#<% = UnassignedRightsListBox.ClientID %>', filter, keysUnAssigned, valuesUnAssigned);
   	});

   	document.onload = initializeFilter();

   	//initialization and first time filter
   	function initializeFilter() {
   		$.each(options, function (index, item) {
   			keysAssigned.push(item.value);
   			valuesAssigned.push(item.innerHTML);
   		});
   		var options2 = $('#<% = UnassignedRightsListBox.ClientID %> option');
        	$.each(options2, function (index, item) {
        		keysUnAssigned.push(item.value);
        		valuesUnAssigned.push(item.innerHTML);
        	});
        	DoListBoxFilter('#<% = AssignedRightsListBox.ClientID %>', filter, keysAssigned, valuesAssigned);
	    	DoListBoxFilter('#<% = UnassignedRightsListBox.ClientID %>', filter, keysUnAssigned, valuesUnAssigned);
    	}

</script>
</HTML>
