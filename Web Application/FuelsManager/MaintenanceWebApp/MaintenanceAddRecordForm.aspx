<%@ Page Language="c#" AutoEventWireup="True" CodeBehind="MaintenanceAddRecordForm.aspx.cs" Inherits="FuelsManager.MaintenanceWebApp.MaintenanceAddRecordForm" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<!DOCTYPE html >

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title></title>
	<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
	<meta content="C#" name="CODE_LANGUAGE">
	<meta content="JavaScript" name="vs_defaultClientScript">
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	<style type="text/css">
		/* Correct placement of Ajax combo box drop-down lists */
		#AssetTypeDropdown ul {
			position: absolute !important;
			left: 121px !important;
			top: 56px !important;
		}

		#AssetIDComboBox ul {
			position: absolute !important;
			left: 121px !important;
			top: 86px !important;
		}

		#PersonnelIDComboBox ul {
			position: absolute !important;
			left: 121px !important;
			top: 116px !important;
		}

		#MaintenanceReasonFMCombobox ul {
			position: absolute !important;
			left: 245px !important;
			top: 361px !important;
		}
	</style>
</head>
<body>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	<form id="MaintenanceAddRecordForm" method="post" runat="server">
		<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		<div id="pageContent" style="position: absolute">

			<!-- Top row -->
			<asp:ScriptManager ID="oScriptManager" runat="server" />

			<asp:Image ID="FadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>" 
				Style="z-index: -3; left: 0px; position: absolute; top: 0px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>

			<asp:Label ID="TitleLabel"
				Style="z-index: 105; left: 16px; position: absolute; top: 9px" runat="server"
				CssClass="headline">Add Maintenance Record</asp:Label>

			<div>

				<!-- Assets -->
				<asp:UpdatePanel ID="UpdatePanelAsset" runat="server" UpdateMode="Conditional">
					<ContentTemplate>

						<!-- Asset Type: EQUIPMENT or TANK -->
						<asp:Label ID="AssetLabel" AssociatedControlID="AssetTypeDropdown$AssetTypeDropdown_TextBox" Style="z-index: 105; left: 16px; position: absolute; top: 40px"
							runat="server" CssClass="formfieldtitle" BackColor="Transparent">Asset Type:</asp:Label>

						<FMControls:FMComboBox Style="visibility: hidden"
							ID="AssetTypeDropdown" TabIndex="14" runat="server" CssClass="formfield"
							Width="127px" AutoPostBack="True" EnableViewState="True" DropDownStyle="DropDownList"
							OnSelectedIndexChanged="AssetTypeDropdownSelectedIndexChanged">
						</FMControls:FMComboBox>
					</ContentTemplate>
				</asp:UpdatePanel>
				<!-- AssetID -->
				<asp:UpdatePanel ID="UpdatePanelAssetID" runat="server" UpdateMode="Conditional">
					<ContentTemplate>
						<!-- Equipment or Tank ID "serial number"-->
						<asp:Label ID="AssetIDLabel" AssociatedControlID="AssetIDComboBox$AssetIDComboBox_TextBox" Style="z-index: 105; left: 16px; position: absolute; top: 70px"
							runat="server" CssClass="formfieldtitle" BackColor="Transparent">ID:</asp:Label>

						<!-- POSITION must be static or inherit.  Also, must add pageLoad() function below. -->
						<!-- Initially invisible to avoid it appearing in wrong place. -->
						<FMControls:FMComboBox ID="AssetIDComboBox"
							runat="server" Style="visibility: hidden"
							Width="127px" MaxLength="50" AutoCompleteMode="SuggestAppend" CssClass="formfield" AutoPostBack="true"
							EnableViewState="true" DropDownStyle="DropDownList"
							OnSelectedIndexChanged="AssetIDComboBoxSelectedIndexChanged" TabIndex="10" />

						<!-- Equipment Attribute ("Type") -->
						<asp:Label ID="AssetTypeLabel" Style="z-index: 105; left: 309px; position: absolute; top: 70px; width: 73px;"
							runat="server" CssClass="formfieldtitle" BackColor="Transparent" EnableViewState="true">Type Class:</asp:Label>

						<asp:Label ID="AssetTypeValueLabel" Style="z-index: 105; left: 395px; position: absolute; top: 70px"
							runat="server" CssClass="formfield" BackColor="Transparent" EnableViewState="false"></asp:Label>
						<!-- Work Order -->
						<asp:Label ID="WorkorderLabel" AssociatedControlID="WorkOrderTextBox" Style="z-index: 105; left: 16px; position: absolute; top: 130px"
							runat="server" CssClass="formfieldtitle" BackColor="Transparent">Work Order:</asp:Label>

						<asp:TextBox ID="WorkOrderTextBox" Style="z-index: 104; left: 120px; position: absolute; top: 127px; width: 150px;"
							runat="server" CssClass="formfield" MaxLength="20" TextMode="SingleLine"></asp:TextBox>
					</ContentTemplate>
					<Triggers>
						<asp:AsyncPostBackTrigger ControlID="AssetTypeDropdown" EventName="SelectedIndexChanged" />
					</Triggers>
				</asp:UpdatePanel>

				<!-- Personnel -->
				<asp:UpdatePanel ID="UpdatePanelPersonnel" runat="server" UpdateMode="Conditional">
					<ContentTemplate>
						<asp:Label ID="PersonnelIDLabel" AssociatedControlID="PersonnelIDComboBox$PersonnelIDComboBox_TextBox" Style="z-index: 105; left: 16px; position: absolute; top: 100px"
							runat="server" CssClass="formfieldtitle" BackColor="Transparent">Operator:</asp:Label>

						<!-- POSITION must be static or inherit.  Also, must add pageLoad() function below. -->
						<!-- Initially invisible to avoid it appearing in wrong place. -->
						<FMControls:FMComboBox ID="PersonnelIDComboBox"
							runat="server" Style="visibility: hidden"
							Width="127px" MaxLength="50" AutoCompleteMode="SuggestAppend" CssClass="formfield" AutoPostBack="False"
							EnableViewState="True" DropDownStyle="DropDownList" />
					</ContentTemplate>
					<Triggers>
						<asp:AsyncPostBackTrigger ControlID="AssetTypeDropdown" EventName="SelectedIndexChanged" />
						<asp:AsyncPostBackTrigger ControlID="AssetIDComboBox" EventName="SelectedIndexChanged" />
					</Triggers>
				</asp:UpdatePanel>


				<!-- Hours Passed -->
				<asp:Label ID="HoursPassedLabel" Style="z-index: 100; left: 120px; position: absolute; top: 160px"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent">
					000 hour(s) have passed since the last change in maintenance status</asp:Label>

				<!-- Memo -->
				<asp:UpdatePanel ID="UpdatePanelMemo" runat="server" UpdateMode="Conditional">
					<ContentTemplate>
						<asp:Label ID="MemoLabel" AssociatedControlID="MemoTextBox" Style="z-index: 105; left: 16px; position: absolute; top: 188px"
							runat="server" CssClass="formfieldtitle" BackColor="Transparent">Memo:</asp:Label>

						<FMControls:FMTextBox ID="MemoTextBox"
							Style="z-index: 104; left: 16px; position: absolute; top: 206px; height: 90px; width: 503px;"
							runat="server" MaxLength="1000" TextMode="MultiLine" CssClass="formfield" EnableTheming="True"
							EnableViewState="False" />

						<ajaxToolkit:TextBoxWatermarkExtender ID="MemoTextBox_TextBoxWatermarkExtender"
							runat="server" TargetControlID="MemoTextBox" WatermarkText="Enter up to 1000 characters.">
						</ajaxToolkit:TextBoxWatermarkExtender>
					</ContentTemplate>
					<Triggers>
						<asp:AsyncPostBackTrigger ControlID="AssetIDComboBox" EventName="SelectedIndexChanged" />
						<asp:AsyncPostBackTrigger ControlID="AssetTypeDropdown" EventName="SelectedIndexChanged" />
					</Triggers>
				</asp:UpdatePanel>

				<!-- In Service -->
				<asp:UpdatePanel ID="UpdatePanelInService" runat="server" UpdateMode="Conditional">
					<ContentTemplate>

						<FMControls:FMCheckBox ID="InServiceFMCheckBox" Style="z-index: 105; left: 13px; position: absolute; top: 310px;"
							runat="server" CssClass="formfieldtitle" Text=" In Service"
							Checked="true" OnCheckedChanged="InServiceFMCheckBoxCheckedChanged" AutoPostBack="True" EnableViewState="True" />

						<hr style="z-index: 105; left: 100px; position: absolute; top: 322px; width: 422px; color: #114DFF; height: 1px;"
							align="right" />

						<!-- Maintenance Reason -->
						<asp:Label ID="MaintenanceReasonLabel" AssociatedControlID="MaintenanceReasonFMCombobox$MaintenanceReasonFMCombobox_TextBox" Style="z-index: 105; left: 64px; position: absolute; top: 344px; height: 15px; width: 154px;"
							runat="server" CssClass="formfieldtitle" AutoPostBack="false" BackColor="Transparent">Maintenance Reason:</asp:Label>
						<asp:Label ID="MaintenanceReasonStar" Style="z-index: 105; left: 235px; position: absolute; top: 344px; width: 4px; height: 15px;"
							runat="server" CssClass="formfieldtitle" ForeColor="Crimson"
							BackColor="Transparent">*</asp:Label>
						<!-- POSITION must be static or inherit.  Also, must add pageLoad() function below. -->
						<!-- Initially invisible to avoid it appearing in wrong place. -->
						<FMControls:FMComboBox ID="MaintenanceReasonFMCombobox"
							runat="server" Style="visibility: hidden"
							Width="250px" MaxLength="50" AutoCompleteMode="SuggestAppend" CssClass="formfield" AutoPostBack="True" aria-required="true"
							EnableViewState="True" DropDownStyle="DropDownList">
						</FMControls:FMComboBox>

						<!-- Estimated Return To Service -->
						<asp:Label ID="EstimatedReturnToServiceLabel" Style="z-index: 105; left: 64px; position: absolute; top: 380px"
							runat="server" CssClass="formfieldtitle" BackColor="Transparent">Estimated Return to Service:</asp:Label>

						<FMControls:FMDate ID="EstimatedReturnFMDATE" CssClass="formfield"
							Style="z-index: 200; left: 244px; position: absolute; top: 375px" runat="server"
							TabIndex="1" Width="160px" Height="25px"></FMControls:FMDate>

					</ContentTemplate>
					<Triggers>
						<asp:AsyncPostBackTrigger ControlID="AssetIDComboBox" EventName="SelectedIndexChanged" />
						<asp:AsyncPostBackTrigger ControlID="AssetTypeDropdown" EventName="SelectedIndexChanged" />
					</Triggers>
				</asp:UpdatePanel>

				<!-- Bottom row -->
				<asp:Label ID="DenotesLabel" Style="z-index: 105; left: 16px; position: absolute; top: 499px;"
					runat="server" CssClass="formfieldtitle" BackColor="Transparent" ForeColor="Red">
					* Denotes Required Field</asp:Label>


                <asp:UpdatePanel ID="UpdatePanelModalDialog" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="OkButton"
                            PopupControlID="ConfirmTankPanel" BackgroundCssClass="modalBackground" OkControlID="YesButton"
                            PopupDragHandleControlID="PopupHeader" Drag="true"
                            CancelControlID="NoButton" DropShadow="true" />
                        <asp:Panel ID="ConfirmTankPanel" Style="display: none; background-color: #EEEEEE; width: 350px; height: 100px"
                            runat="server">
                            <div>
                                <div id="PopupHeader" style="background-color: ActiveCaption; text-align: left; color: White">Please Confirm</div>
                                <div align="center">
                                    <p>Can the tank physically issue or receive fuel?</p>
                                </div>
                                <div align="right">
                                    <FMControls:FMButton ID="YesButton" runat="server" Text="Yes" Style="width: 80px; height: 25px" OnClick="YesButtonClick" />
                                    <FMControls:FMButton ID="NoButton" runat="server" Text="No" Style="width: 80px; height: 25px" OnClick="NoButtonClick" />
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:Button ID="OkButton" Style="z-index: 110; left: 342px; position: absolute; top: 495px;"
                            runat="server" Text="OK1" CssClass="formfieldtitle" TabIndex="5"
                            OnClick="OkButtonClick" Width="80px"></asp:Button>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:Button ID="CancelButton" Style="z-index: 110; left: 447px; position: absolute; top: 495px"
                    runat="server" Text="Cancel" CssClass="formfieldtitle" TabIndex="5"
                    OnClick="CancelButtonClick" Width="80px"></asp:Button>

            </div>

			<!-- ==================================================================== -->
			<!--                        Client side code                              -->
			<!-- ==================================================================== -->

			<script type="text/javascript">
			    
			    function fnModalBtnClick(sender, e) {
			        __doPostBack(sender, e);
			    }

			    if (document.getElementById("OkButton") != null) {
			        document.getElementById("OkButton").setActive();
			    }

                // ??	check if visible?		if (document.getElementById("AssetTypeDropdown") != null)  document.getElementById("AssetTypeDropdown").focus();

			    // Corrects MS bug in placement of AJAX comboboxes.
			    // http://forums.asp.net/p/1423235/3170064.aspx
			    // http://74.125.95.132/search?q=cache:DUimsBB1FH0J:forums.asp.net/ThreadNavigation.aspx%3FPostID%3D3227954%26NavType%3DPrevious+ajax+toolkit+combobox+wrong+position&cd=1&hl=en&ct=clnk&gl=us
			    function pageLoad()
			    {
			        // AssetType
			        var comboboxAssetType = $get('<%=AssetTypeDropdown.ClientID + "_" + AssetTypeDropdown.ClientID %>' + '_Table');
			        comboboxAssetType.style.position = "absolute";
			        comboboxAssetType.style.left = "120px";
			        comboboxAssetType.style.top = "36px";
			        comboboxAssetType.style.visibility = "visible";
			        comboboxAssetType.visible = "true";

			        // EquipmentID
			        var comboboxEquipmentID = $get('<%=AssetIDComboBox.ClientID + "_" + AssetIDComboBox.ClientID %>' + '_Table');
			        comboboxEquipmentID.style.position = "absolute";
			        comboboxEquipmentID.style.left = "120px";
			        comboboxEquipmentID.style.top = "66px";
			        comboboxEquipmentID.style.visibility = "visible";
			        comboboxEquipmentID.visible = "true";

			        // PersonnelID
			        var comboboxPersonnelID = $get('<%=PersonnelIDComboBox.ClientID + "_" + PersonnelIDComboBox.ClientID %>' + '_Table');
			        comboboxPersonnelID.style.position = "absolute";
			        comboboxPersonnelID.style.left = "120px";
			        comboboxPersonnelID.style.top = "96px";
			        comboboxPersonnelID.style.visibility = "visible";
			        comboboxPersonnelID.visible = "true";

			        // Maintenance Reason
			        var comboboxMaintenanceReasonID = $get('<%=MaintenanceReasonFMCombobox.ClientID + "_" + MaintenanceReasonFMCombobox.ClientID %>' + '_Table');
			        comboboxMaintenanceReasonID.style.position = "absolute";
			        comboboxMaintenanceReasonID.style.left = "244px";
			        comboboxMaintenanceReasonID.style.top = "341px";
			        comboboxMaintenanceReasonID.style.visibility = "visible";
			        comboboxMaintenanceReasonID.visible = "true";
			    };
			</script>

		</div>
	</form>
</body>
</html>
