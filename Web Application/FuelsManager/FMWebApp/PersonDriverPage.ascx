<%@ Control language="c#" Codebehind="PersonDriverPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.PersonDriverPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<head>
		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
	</head>
<script>
    function EquipmentSelect(equipmentTextBoxId)
    {
        var equipmentTextBox = document.getElementById(equipmentTextBoxId);

        showModalDialogFrame({
			url: "../FMWebApp/EquipmentSelectForm.aspx?EquipmentTextBoxID=" + equipmentTextBoxId,
			width: 855,
			height: 560,
			title: "Equipment Select",
			onClose: function ()
			{
			    if (this.returnValue != null)
			    {
			        var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
			        var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

			        equipmentTextBox.value = asciiValue1;
			        equipmentTextBox.title = asciiValue2;
			    }
			}
		});
    }
</script>

	<body>
		<FMCONTROLS:FMLABEL id="FMLABEL3" AssociatedControlID="AssignedEquipmentTextBox"
         style="Z-INDEX: 116; LEFT: 7px; POSITION: absolute; TOP: 23px; height: 5px; width: 119px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle">Assigned Equipment:</FMCONTROLS:FMLABEL>
		<FMControls:FMEquipmentTextBox ID="AssignedEquipmentTextBox" runat="server" style="Z-INDEX: 146; LEFT: 131px; POSITION: absolute; TOP: 20px"
			tabIndex="22" Width="113px" CssClass="formfield" MaxLength="20" AutoPostBack="True"  Enabled="false"></FMControls:FMEquipmentTextBox>	
		<FMCONTROLS:FMLABEL id="FMLABEL4" AssociatedControlID="StatusDownList" 
         style="Z-INDEX: 116; LEFT: 7px; POSITION: absolute; TOP: 59px; height: 5px;" runat="server"
			BackColor="Transparent" CssClass="formfieldtitle" Width="70px">Status:</FMCONTROLS:FMLABEL>
		<FMControls:FMDropDownList ID="StatusDownList" runat="server" style="Z-INDEX: 105; LEFT: 131px; POSITION: absolute; TOP: 59px; right: 814px; width: 157px;"
			tabIndex="8" CssClass="formfield" Enabled="false">
		</FMControls:FMDropDownList>     
	</body>
	</HTML>
