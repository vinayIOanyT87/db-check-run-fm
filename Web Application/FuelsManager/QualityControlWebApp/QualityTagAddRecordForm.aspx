<%@ Page language="c#" AutoEventWireup="True" Codebehind="QualityTagAddRecordForm.aspx.cs" Inherits="FuelsManager.QualityControlWebApp.QualityTagAddRecordForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="FMMenuBar" TagName="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" %>

<!DOCTYPE html >
<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
	<head id="Head1" runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="generator" />
		<meta content="C#" name="code_language" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
        <style type="text/css"> 
            /* Correct placement of Ajax combo box drop-down lists */
            #AssetIDComboBox ul 
            { 
                position: absolute !important; 
                left: 121px !important; 
                top: 86px !important; 
            }
            #QualityTagNameFMCombobox ul 
            { 
                position: absolute !important; 
                left: 121px !important; 
                top: 116px !important; 
            }
        </style>
	</head>
	<body>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
        <form id="QualityTagAddRecordForm" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">

                <!-- Top row -->
                <asp:ScriptManager ID="oScriptManager" runat="server" />

                <asp:Image ID="FadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>"
                    Style="z-index: -3; left: 0px; position: absolute; top: 0px" runat="server"
                    ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>

                <FMControls:FMLabel ID="TitleLabel"
                    Style="z-index: 105; left: 16px; position: absolute; top: 9px" runat="server"
                    CssClass="headline">Add Quality Tag Record</FMControls:FMLabel>

                <div>

                    <!-- Assets -->
                    <asp:UpdatePanel ID="UpdatePanelAsset" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <!-- Asset Type: EQUIPMENT or TANK -->
                            <FMControls:FMLabel ID="AssetLabel" AssociatedControlID="AssetTypeDropdown" Style="z-index: 105; left: 16px; position: absolute; top: 40px"
                                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Asset Type:</FMControls:FMLabel>

                            <FMControls:FMDropDownList Style="z-index: 105; left: 120px; position: absolute; top: 37px"
                                ID="AssetTypeDropdown" runat="server" CssClass="formfield"
                                Width="154px" AutoPostBack="True" EnableViewState="True"
                                OnSelectedIndexChanged="AssetTypeDropdownSelectedIndexChanged">
                            </FMControls:FMDropDownList>


                            <!-- AssetID -->
                            <asp:UpdatePanel ID="UpdatePanelAssetID" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <!-- Equipment or Tank ID "serial number"-->
                                    <FMControls:FMLabel ID="AssetIDLabel" AssociatedControlID="AssetIDComboBox$AssetIDComboBox_TextBox" Style="z-index: 105; left: 16px; position: absolute; top: 70px"
                                        runat="server" CssClass="formfieldtitle" BackColor="Transparent">ID:</FMControls:FMLabel>
                                    <asp:Label ID="Label1" Style="z-index: 105; left: 110px; position: absolute; top: 70px"
                                        runat="server" CssClass="formfieldtitle" ForeColor="Red" BackColor="Transparent">*</asp:Label>

                                    <!-- POSITION must be static or inherit.  Also, must add pageLoad() function below. -->
                                    <!-- Initially invisible to avoid it appearing in wrong place. -->
                                    <FMControls:FMComboBox ID="AssetIDComboBox" runat="server" Style="visibility: hidden" aria-required="true"
                                        Width="127px" MaxLength="50" AutoCompleteMode="SuggestAppend" CssClass="formfield" AutoPostBack="true"
                                        EnableViewState="true" DropDownStyle="DropDownList"
                                        OnSelectedIndexChanged="AssetIDComboBoxSelectedIndexChanged" />

                                    <!-- Equipment Attribute ("Type") -->
                                    <FMControls:FMLabel ID="AssetTypeLabel" AssociatedControlID="AssetTypeTextBox" Style="z-index: 105; left: 325px; position: absolute; top: 70px;"
                                        runat="server" CssClass="formfieldtitle" BackColor="Transparent" EnableViewState="true">Type:</FMControls:FMLabel>

                                    <asp:TextBox runat="server" ID="AssetTypeTextBox" CssClass="formfield" Enabled="false"
                                        Style="z-index: 105; position: absolute; left: 420px; top: 70px; width: 150px" />

                                    <!-- QualityTag -->
                                    <asp:UpdatePanel ID="UpdatePanelQualityTag" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>

                                            <!-- QualityTag Label -->
                                            <FMControls:FMLabel ID="QualityTagNameLabel" AssociatedControlID="QualityTagNameFMCombobox$QualityTagNameFMCombobox_TextBox" Style="z-index: 105; left: 16px; position: absolute; top: 100px"
                                                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Quality Tag:</FMControls:FMLabel>
                                            <asp:Label ID="Label2" Style="z-index: 105; left: 110px; position: absolute; top: 100px"
                                                runat="server" CssClass="formfieldtitle" ForeColor="Red" BackColor="Transparent">*</asp:Label>

                                            <!-- POSITION must be static or inherit.  Also, must add pageLoad() function below. -->
                                            <!-- Initially invisible to avoid it appearing in wrong place. -->
                                            <FMControls:FMComboBox ID="QualityTagNameFMCombobox" runat="server" Style="visibility: hidden" aria-required="true"
                                                Width="127px" MaxLength="50" AutoCompleteMode="SuggestAppend" CssClass="formfield" AutoPostBack="true"
                                                EnableViewState="True" DropDownStyle="DropDownList" OnSelectedIndexChanged="QualityTagNameFMComboboxOnSelectedIndexChanged">
                                            </FMControls:FMComboBox>

                                        </ContentTemplate>
                                    </asp:UpdatePanel>

                                    <!-- Tag Number -->
                                    <FMControls:FMLabel runat="server" ID="TagLabel" AssociatedControlID="TagTextBox" CssClass="formfieldtitle" Text="Tag Number:"
                                        Style="z-index: 105; position: absolute; top: 100px; left: 325px;" />

                                    <asp:Label ID="Label3" Style="z-index: 105; left: 410px; position: absolute; top: 102px"
                                        runat="server" CssClass="formfieldtitle" ForeColor="Red" BackColor="Transparent" Text="*" />

                                    <asp:TextBox runat="server" ID="TagTextBox" CssClass="formfield" aria-required="true"
                                        Style="z-index: 105; position: absolute; top: 100px; left: 420px; width: 150px;" />

                                    <!-- Memo -->
                                    <asp:UpdatePanel ID="UpdatePanelMemo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                        <ContentTemplate>

                                            <FMControls:FMLabel ID="MemoLabel" AssociatedControlID="MemoTextBox" Style="z-index: 105; left: 16px; position: absolute; top: 148px"
                                                runat="server" CssClass="formfieldtitle" BackColor="Transparent">Memo:</FMControls:FMLabel>

                                            <asp:Label ID="MemoStar" Style="z-index: 105; left: 60px; position: absolute; top: 148px"
                                                runat="server" CssClass="formfieldtitle" ForeColor="Red" BackColor="Transparent">*</asp:Label>

                                            <FMControls:FMTextBox ID="MemoTextBox"
                                                Style="z-index: 104; left: 16px; position: absolute; top: 166px; height: 90px; width: 553px;" aria-required="true"
                                                runat="server" MaxLength="1000" TextMode="MultiLine" CssClass="formfield" EnableTheming="True"
                                                EnableViewState="False" />

                                            <ajaxToolkit:TextBoxWatermarkExtender ID="MemoTextBox_TextBoxWatermarkExtender"
                                                runat="server" TargetControlID="MemoTextBox" WatermarkText="Enter up to 1000 characters.">
                                            </ajaxToolkit:TextBoxWatermarkExtender>

                                        </ContentTemplate>
                                    </asp:UpdatePanel>



                                    <!-- Tagged Date -->
                                    <FMControls:FMLabel ID="TaggedDateLabel" Style="z-index: 105; left: 16px; position: absolute; top: 280px"
                                        runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text="Tagged Date:"
                                        EnableViewState="false"></FMControls:FMLabel>

                                    <asp:Label ID="TaggedDateValue" Style="z-index: 105; left: 116px; position: absolute; top: 280px; width: 150px;"
                                        runat="server" CssClass="formfield" BackColor="Transparent" Text=""
                                        EnableViewState="false" Enabled="true"></asp:Label>

                                    <!-- Tagged By -->
                                    <FMControls:FMLabel ID="TaggedByLabel" Style="z-index: 105; left: 287px; position: absolute; top: 280px"
                                        runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text="Tagged By:"
                                        EnableViewState="False"></FMControls:FMLabel>

                                    <asp:Label ID="TaggedByValue" Style="z-index: 105; left: 384px; position: absolute; top: 280px"
                                        runat="server" CssClass="formfield" BackColor="Transparent" Text=""
                                        EnableViewState="false" Enabled="true"></asp:Label>


                                    <!-- Removed Date -->
                                    <FMControls:FMLabel ID="RemovedDateLabel" Style="z-index: 105; left: 16px; position: absolute; top: 310px;"
                                        runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text="Removed Date:"
                                        EnableViewState="false"></FMControls:FMLabel>

                                    <asp:Label ID="RemovedDateValue" Style="z-index: 105; left: 116px; position: absolute; top: 310px; width: 150px;"
                                        runat="server" CssClass="formfield" BackColor="Transparent" Text=""
                                        EnableViewState="false" Enabled="true"></asp:Label>

                                    <!-- Removed By -->
                                    <FMControls:FMLabel ID="RemovedByLabel" Style="z-index: 105; left: 287px; position: absolute; top: 310px"
                                        runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text="Removed By:"
                                        EnableViewState="False"></FMControls:FMLabel>

                                    <asp:Label ID="RemovedByValue" Style="z-index: 105; left: 384px; position: absolute; top: 310px"
                                        runat="server" CssClass="formfield" BackColor="Transparent" Text=""
                                        EnableViewState="false" Enabled="true"></asp:Label>

                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <!-- Bottom row -->
                    <FMControls:FMLabel ID="DenotesLabel" Style="z-index: 105; left: 16px; position: absolute; top: 370px;"
                        runat="server" CssClass="formfieldtitle" BackColor="Transparent" ForeColor="Red">* Denotes Required Field</FMControls:FMLabel>

                    <asp:UpdatePanel ID="UpdatePanelOKButton" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <FMControls:FMButton ID="ApplyOrRemoveButton" runat="server"
                                Style="z-index: 110; left: 395px; top: 366px; position: absolute" Text="OK"
                                Width="80px" CssClass="formfieldtitle" OnClick="ApplyOrRemoveButtonClick" OnClientClick="if (!RemoveButtonClick(this.value)) return false;" />
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <FMControls:FMButton ID="CancelButton" Style="z-index: 110; left: 495px; position: absolute; top: 366px"
                        runat="server" Text="Cancel" CssClass="formfieldtitle" 
                        OnClick="CancelButtonClick" Width="80px"></FMControls:FMButton>
                </div>

                <input type="hidden" name="__MYEVENTTARGET" />
                <input type="hidden" name="__MYEVENTARGUMENT" />

                <!-- ==================================================================== -->
			<!--                        Client side code                              -->
			<!-- ==================================================================== -->

			<script type="text/javascript">

				function RemoveButtonClick(val) {
					var removeButtonValue = '<%=this.GetTranslatedText("Remove") %>';
					if (val != removeButtonValue)
					    //If not remove button than handle onclick directly on server side. Same button can be either Remove or Save/OK.
					    return true;
					return confirm('Click OK to remove tag.');
				}

				if (document.getElementById("ApplyOrRemoveButton") != null)
					document.getElementById("ApplyOrRemoveButton").focus();

				if (document.getElementById("AssetTypeDropdown") != null &&
				    document.getElementById("AssetTypeDropdown").getAttribute("Visible") == true)
				{
					document.getElementById("AssetTypeDropdown").focus();
				}
				
				function __mydoPostBack(eventTarget, eventArgument) {
				   var theform;
				   if (window.navigator.appName.toLowerCase().indexOf("microsoft") > -1) {
				      theform = document.QualityTagAddRecordForm;
				   }
				   else {
				      theform = document.forms["QualityTagAddRecordForm"];
				   }
				   theform.__MYEVENTTARGET.value = eventTarget;
				   theform.__MYEVENTARGUMENT.value = eventArgument;

				}
				// Corrects MS bug in placement of AJAX comboboxes.
				// http://forums.asp.net/p/1423235/3170064.aspx
				// http://74.125.95.132/search?q=cache:DUimsBB1FH0J:forums.asp.net/ThreadNavigation.aspx%3FPostID%3D3227954%26NavType%3DPrevious+ajax+toolkit+combobox+wrong+position&cd=1&hl=en&ct=clnk&gl=us
				function pageLoad()
				{
                    // EquipmentID
                    var comboboxAssetID = $get('<%=AssetIDComboBox.ClientID  + "_" + AssetIDComboBox.ClientID  %>' + '_Table');
                    comboboxAssetID.style.position = "absolute";
                    comboboxAssetID.style.left = "120px";
                    comboboxAssetID.style.top = "66px";
                    comboboxAssetID.style.visibility = "visible";
                    comboboxAssetID.visible = "true";

                    // Quality Tag Reason
                    var comboboxQualityTagNameID = $get('<%=QualityTagNameFMCombobox.ClientID  + "_" + QualityTagNameFMCombobox.ClientID  %>' + '_Table');
                    comboboxQualityTagNameID.style.position = "absolute";
                    comboboxQualityTagNameID.style.left = "120px";
                    comboboxQualityTagNameID.style.top = "96px";
                    comboboxQualityTagNameID.style.visibility = "visible";
                    comboboxQualityTagNameID.visible = "true";
                }
            </script>

            </div>
        </form>
    </body>
</html>
