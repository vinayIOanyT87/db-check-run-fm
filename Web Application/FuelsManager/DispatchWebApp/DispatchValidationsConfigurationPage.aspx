<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DispatchValidationsConfigurationPage.aspx.cs"
	Inherits="FuelsManager.DispatchWebApp.DispatchValidationsConfigurationPage" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
	<form runat="server">
	<FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
	<div id="pageContent">
		<div id="content" style="position: absolute">
		<asp:Image ID="fadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; top: 0px; position: absolute;"
			runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
		<FMControls:FMLabel ID="titleLabel" Style="z-index: 118; left: 8px; top: 8px; position: absolute; width: 800px"
			runat="server" BackColor="Transparent" CssClass="headline">Dispatch Validations Configuration</FMControls:FMLabel>
		<FMControls:FMLabel ID="selectValidationsLabel" Style="z-index: 118; left: 32px;
			position: absolute; top: 65px;" runat="server" BackColor="Transparent" Text="Select Validations to Perform"
			Width="200px" CssClass="formfieldtitle" />
		<asp:Panel ID="selectValidationsPanel" Style="z-index: 103; left: 32px; position: absolute;
			top: 85px; width: 756px; height: 357px;" runat="server" BorderColor="LightSteelBlue"
			BorderStyle="Solid" BorderWidth="1px" />
		<FMControls:FMCheckBox ID="chkQuantityNotZero" TabIndex="1" runat="server" CssClass="formfieldtitle"
			Style="z-index: 118; left: 45px; position: absolute; top: 100px; width: 176px;"
			Text="Quantity Not Zero" Height="20px"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkExactlyOneManager" TabIndex="2" runat="server" CssClass="formfieldtitle"
			Style="z-index: 118; left: 255px; position: absolute; top: 100px; width: 176px"
			Text="Exactly One Manager" Height="20px"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkExactlyOneOwner" TabIndex="3" runat="server" CssClass="formfieldtitle"
			Style="z-index: 118; left: 525px; position: absolute; top: 100px; width: 176px;
			right: 166px;" Text="Exactly One Owner" Height="20px"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkFastLogFuelAdditiveFlag" TabIndex="4" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 45px; position: absolute;
			top: 135px; width: 176px; height: 20px" Text="Fast Log Fuel Additive Flag"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkFillstandVolumeWithinTolerance" TabIndex="5" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 255px; position: absolute;
			top: 135px; width: 216px; height: 20px" Text="Fillstand Volume Within Tolerance">
		</FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkReturnToBulkVolumeWithinTolerance" TabIndex="6" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 525px; position: absolute;
			top: 135px; width: 256px; height: 20px" Text="Return To Bulk Volume Within Tolerance">
		</FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkDispatchFuelAdditiveFlag" TabIndex="7" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 45px; position: absolute;
			top: 170px; width: 176px; height: 20px" Text="Dispatch Fuel Additive Flag"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkOperatorIsIn" TabIndex="8" runat="server" CssClass="formfieldtitle"
			Style="z-index: 118; left: 255px; position: absolute; top: 170px; width: 176px;
			height: 20px" Text="Operator Is In"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkRecirculationVolumesGreaterThanZero" TabIndex="9" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 525px; position: absolute;
			top: 170px; width: 256px; height: 20px" Text="Recirculation Volumes Greater Than Zero">
		</FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkOperatorNotAssigned" TabIndex="10" runat="server" CssClass="formfieldtitle"
			Style="z-index: 118; left: 45px; position: absolute; top: 205px; width: 176px;
			height: 20px" Text="Operator Not Assigned"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkOperatorHasRequiredTraining" TabIndex="11" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 255px; position: absolute;
			top: 205px; width: 206px; height: 20px" Text="Operator Has Required Training">
		</FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkOperatorTrainingNotExpired" TabIndex="12" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 525px; position: absolute;
			top: 205px; width: 196px; height: 20px" Text="Operator Training Not Expired">
		</FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkOperatorNotLockedOut" TabIndex="13" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 45px; position: absolute;
			top: 240px; width: 176px; height: 20px" Text="Operator Not Locked Out"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkOperatorHasRequiredQualifications" TabIndex="14" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 255px; position: absolute;
			top: 240px; width: 236px; height: 20px" Text="Operator Has Required Qualifications">
		</FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkOperatorQualificationsNotExpired" TabIndex="15" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 525px; position: absolute;
			top: 240px; width: 226px; height: 20px" Text="Operator Qualifications Not Expired">
		</FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkDefuelStatusCheck" TabIndex="16" runat="server" CssClass="formfieldtitle"
			Style="z-index: 118; left: 45px; position: absolute; top: 275px; width: 176px;
			height: 20px" Text="Defuel Status Check"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkRefuelStatusCheck" TabIndex="17" runat="server" CssClass="formfieldtitle"
			Style="z-index: 118; left: 255px; position: absolute; top: 275px; width: 176px;
			height: 20px" Text="Refuel Status Check"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkEquipmentFuelGrade" TabIndex="18" runat="server" CssClass="formfieldtitle"
			Style="z-index: 118; left: 525px; position: absolute; top: 275px; width: 176px;
			height: 20px" Text="Equipment Fuel Grade"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkEquipmentNotLockedOut" TabIndex="19" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 45px; position: absolute;
			top: 310px; width: 176px; height: 20px" Text="Equipment Not Locked Out"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkEquipmentNotAssigned" TabIndex="20" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 255px; position: absolute;
			top: 310px; width: 176px; height: 20px" Text="Equipment Not Assigned"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkEquipmentInService" TabIndex="21" runat="server" CssClass="formfieldtitle"
			Style="z-index: 118; left: 525px; position: absolute; top: 310px; width: 176px;
			height: 20px" Text="Equipment In Service"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkTagLicenseNotExpired" TabIndex="22" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 45px; position: absolute;
			top: 345px; width: 176px; height: 20px" Text="Tag/License Not Expired"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkTestInspectionNotExpired" TabIndex="23" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 255px; position: absolute;
			top: 345px; width: 176px; height: 20px" Text="Test/Inspection Not Expired"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkQualityControlCheckupDate" TabIndex="24" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 525px; position: absolute;
			top: 345px; width: 196px; height: 20px" Text="Quality Control Checkup Date">
		</FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkCautionQualityTagCheck" TabIndex="25" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 45px; position: absolute;
			top: 380px; width: 176px; height: 20px" Text="Caution Quality Tag Check"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkWarningQualityTagCheck" TabIndex="26" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 255px; position: absolute;
			top: 380px; width: 176px; height: 20px" Text="Warning Quality Tag Check"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkDangerQualityTagCheck" TabIndex="27" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 525px; position: absolute;
			top: 380px; width: 176px; height: 20px" Text="Danger Quality Tag Check"></FMControls:FMCheckBox>

		<FMControls:FMCheckBox ID="chkEquipmentRequired" TabIndex="28" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 45px; position: absolute;
			top: 415px; width: 176px; height: 20px" Text="Equipment Required"></FMControls:FMCheckBox>
		<FMControls:FMCheckBox ID="chkPersonnelRequired" TabIndex="28" runat="server"
			CssClass="formfieldtitle" Style="z-index: 118; left: 255px; position: absolute;
			top: 415px; width: 176px; height: 20px" Text="Personnel Required"></FMControls:FMCheckBox>

		<asp:LinkButton ID="checkAllButton" Style="z-index: 118; left: 255px; position: absolute;
			top: 63px; width: 100px" TabIndex="28" runat="server" CssClass="formfieldtitle"
			OnClick="CheckAllButtonOnClick"><%=GetTranslatedText("Check All")%></asp:LinkButton>
		<asp:LinkButton ID="clearAllButton" Style="z-index: 118; left: 335px; position: absolute;
			top: 63px; width: 100px" TabIndex="29" runat="server" CssClass="formfieldtitle"
			OnClick="ClearAllButtonOnClick"><%=GetTranslatedText("Clear All")%></asp:LinkButton>
		<FMControls:FMButton ID="applyButton" Style="z-index: 118; left: 718px; position: absolute;
			top: 452px" TabIndex="30" runat="server" CssClass="formfieldtitle" Text="Apply"
			Width="72px" OnClick="ApplyButtonOnClick"></FMControls:FMButton>
	</div>
	</div>
	</form>
</body>
</html>
