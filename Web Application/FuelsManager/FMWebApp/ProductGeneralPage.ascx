<%@ Control language="c#" Codebehind="ProductGeneralPage.ascx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.ProductGeneralPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
	<HEAD>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
		<SCRIPT>
		    function ProductSelect(productTextBoxId)
		    {
				var productTextBox = document.getElementById(productTextBoxId);

				showModalDialogFrame({
				    url: "../FMWebApp/ProductSelectForm.aspx?Type=BlendProduct&Map=MAX_MAP&None=true",
					width: 855,
					height: 560,
                    title: "Product Select",
                    onClose: function ()
                    {
					    if (this.returnValue != null)
					    {
					        var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
					        var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

					        productTextBox.value = asciiValue1;
					        productTextBox.title = asciiValue2;
						}
					}
				});
			}
		</SCRIPT>
	</HEAD>
	<body>
        <table style="width: 850px">
            <tr>
                <td>
                    <FMControls:FMLabel ID="ProductIdLabel" Style="z-index: 102;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server">Product ID:</FMControls:FMLabel>
                    <FMControls:FMLabel ID="ProdIDAskLabel" Style="z-index: 109; float: right;" runat="server"
                        Width="8px" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="IDTextbox" Style="z-index: 103;" aria-required="true"
                        TabIndex="1" CssClass="formfield" runat="server" MaxLength="30" Width="208px"></asp:TextBox>
                </td>
                <td>
                    <FMControls:FMLabel ID="PidxCodeLabel" Style="z-index: 134;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server">PIDX Code:</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="PIDXCodeTextbox" Style="z-index: 135;"
                        TabIndex="2" CssClass="formfield" runat="server" Width="48px" MaxLength="3"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="ProductCodeLabel" Style="z-index: 102;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server">Product Code:</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="CodeTextbox" Style="z-index: 103;"
                        TabIndex="1" CssClass="formfield" runat="server" MaxLength="15" Width="208px"></asp:TextBox>
                </td>
                <td>
                    <FMControls:FMLabel ID="PidxFamilyCodeLabel" Style="z-index: 134;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server">PIDX Family Code:</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="PIDXFamilyCodeTextbox" Style="z-index: 135;"
                        TabIndex="2" CssClass="formfield" runat="server" Width="48px" MaxLength="3"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="DescriptionLabel" Style="z-index: 104;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server">Description:</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="DescriptionTextbox" Style="z-index: 105;"
                        TabIndex="1" CssClass="formfield" runat="server" MaxLength="50" Width="208px"></asp:TextBox>
                </td>
                <td>
                    <FMControls:FMLabel ID="ContaminationLabel" Style="z-index: 134;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server">Contamination Prompt Load Rack Text:</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="ContaminationPromptLoadRackTextTextBox" Style="z-index: 135;"
                        TabIndex="2" CssClass="formfield" runat="server" Width="72px" MaxLength="10"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="TypeLabel" Style="z-index: 106;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server">Type:</FMControls:FMLabel>
                </td>
                <td>
                    <FMControls:FMDropDownList ID="ProductTypeDropDownList" Style="z-index: 107;"
                        TabIndex="1" CssClass="formfield" runat="server" Width="114px" AutoPostBack="True" OnSelectedIndexChanged="ProductTypeDropDownListSelectedIndexChanged">
                    </FMControls:FMDropDownList>
                </td>
                <td>
                    <FMControls:FMLabel ID="TrackingProdLabel" Style="z-index: 141;"
                        CssClass="formfieldtitle" BackColor="Transparent" runat="server" Width="104px">Tracking Product:</FMControls:FMLabel>
                </td>
                <td>
                    <FMControls:FMProductTextBox ID="TrackingProductTextBox" Style="z-index: 113;"
                        TabIndex="2" runat="server" AutoPostBack="True" CssClass="formfield" Width="200px"></FMControls:FMProductTextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="OctaneLabel" Style="z-index: 108;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server">Octane:</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="OctaneTextbox" Style="z-index: 110;"
                        TabIndex="1" CssClass="formfield" runat="server" Width="66px"></asp:TextBox>
                </td>
                <td>
                    <FMControls:FMLabel ID="ReidVaporLabel" Style="z-index: 111;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server">Reid Vapor Pressure:</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="ReidVaporPressureTextbox" Style="z-index: 112;"
                        TabIndex="2" CssClass="formfield" runat="server" Width="66px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="LRDisplayLabel" Style="z-index: 114;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server">Load Rack Display Text:</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="LoadRackDisplayTextbox" Style="z-index: 115;"
                        TabIndex="1" CssClass="formfield" runat="server" MaxLength="10" Width="90px" OnTextChanged="LoadRackDisplayTextboxTextChanged"></asp:TextBox>
                </td>
                <td>
                    <FMControls:FMLabel ID="PriceLabel" runat="server" CssClass="formfieldtitle" BackColor="Transparent"
                        Style="z-index: 1;">Price:</FMControls:FMLabel>
                </td>
                <td>
                    <FMControls:FMTextBox ID="PriceTextBox" runat="server" CssClass="formfield"
                        TabIndex="2" Style="z-index: 1; width: 106px;" Height="21px"></FMControls:FMTextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="TaxCodeLabel" runat="server" CssClass="formfieldtitle"
                        Style="z-index: 1;">Tax Code:</FMControls:FMLabel>
                </td>
                <td>
                    <FMControls:FMTextBox ID="TaxCodeTextBox" runat="server" CssClass="formfield"
                        TabIndex="1" Style="z-index: 1; width: 108px"></FMControls:FMTextBox>
                </td>
                <td>
                    <FMControls:FMLabel ID="ProdClassLabel" Style="z-index: 141;"
                        CssClass="formfieldtitle" BackColor="Transparent" runat="server" Width="130px">Product Classification:</FMControls:FMLabel>
                </td>
                <td>
                    <FMControls:FMRadioButtonList ID="ProductClassification" runat="server" Style="z-index: 1;"
                        TabIndex="2" CssClass="formfield" Width="200px" RepeatDirection="Horizontal">
                        <asp:ListItem Selected="True">None</asp:ListItem>
                        <asp:ListItem>Aviation</asp:ListItem>
                        <asp:ListItem>Ground</asp:ListItem>
                    </FMControls:FMRadioButtonList>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="VarianceTolLabel" Style="z-index: 134;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server">Variance Tolerance:</FMControls:FMLabel>
                    <FMControls:FMLabel ID="VarianceTolAskLabel" Style="z-index: 109; float: right" runat="server"
                        Width="8px" Height="8px" ForeColor="Crimson">*</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="VarianceToleranceTextbox" Style="z-index: 135;" aria-required="true"
                        TabIndex="1" CssClass="formfield" runat="server" Width="66px"></asp:TextBox>
                    <asp:Label ID="VarianceTolPercentLabel" Style="z-index: 134;"
                        runat="server" CssClass="formfieldtitle" BackColor="Transparent">%</asp:Label>
                </td>
                <td>
                    <FMControls:FMLabel ID="DielectricTolLabel" Style="z-index: 134;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server">Dielectric Tolerance:</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="DielectricToleranceTextbox" Style="z-index: 115;"
                        TabIndex="2" CssClass="formfield" runat="server" MaxLength="10" Width="90px"></asp:TextBox>
                    <asp:Label ID="DielectricTolPercentLabel" Style="z-index: 134;"
                        runat="server" CssClass="formfieldtitle" BackColor="Transparent">%</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMCheckBox ID="VaporRecoveryCheckBox" Style="z-index: 117;"
                        TabIndex="1" CssClass="formfieldtitle" runat="server" TextAlign="Left" Text="Vapor Recovery"></FMControls:FMCheckBox>
                </td>
                <td></td>
                <td>
                    <FMControls:FMCheckBox ID="LoadByWeightCheckBox" Style="z-index: 117;"
                        TabIndex="2" CssClass="formfieldtitle" runat="server" TextAlign="Left" Text="Load By Weight"></FMControls:FMCheckBox>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMCheckBox ID="HazardousMaterialCheckBox" Style="z-index: 117;"
                        TabIndex="1" CssClass="formfieldtitle" runat="server" TextAlign="Left" Text="Hazardous Material"></FMControls:FMCheckBox>
                </td>
                <td></td>
                <td>
                    <FMControls:FMCheckBox ID="InhibitAccountingCheckBox" Style="z-index: 117;"
                        TabIndex="2" CssClass="formfieldtitle" runat="server" TextAlign="Left" Text="Inhibit Accounting"></FMControls:FMCheckBox>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMCheckBox ID="HiddenCheckBox" Style="z-index: 117;"
                        TabIndex="1" CssClass="formfieldtitle" runat="server" TextAlign="Left" Text="Hidden"></FMControls:FMCheckBox>
                </td>
                <td></td>
                <td>
                    <FMControls:FMCheckBox ID="AutomaticCloseoutCheckBox" Style="z-index: 117;"
                        TabIndex="2" CssClass="formfieldtitle" runat="server" TextAlign="Left" Text="Automatic Closeout"></FMControls:FMCheckBox>
                </td>
                <td></td>
            </tr>
			   <tr>
                <td>
                    <FMControls:FMCheckBox ID="IsEthanolCheckBox" Style="z-index: 138;"
                        TabIndex="2" CssClass="formfieldtitle" runat="server" Width="120px" AutoPostBack="False" TextAlign="Left"
                        Text="Ethanol" ></FMControls:FMCheckBox>

                </td>
                <td></td>
                <td></td>
                <td></td>
			   </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="LockedOutDateLabel" Style="z-index: 141;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server" Width="116px">Locked Out Date:</FMControls:FMLabel>
                </td>
                <td>
                    <asp:TextBox ID="LockedOutDateTextbox" Style="z-index: 142;"
                        TabIndex="1" CssClass="formfield" runat="server" MaxLength="20" Width="96px" ReadOnly="True"
                        Enabled="False"></asp:TextBox>
                </td>
                <td>
                    <FMControls:FMCheckBox ID="LockedOutCheckBox" Style="z-index: 138;"
                        TabIndex="2" CssClass="formfieldtitle" runat="server" Width="120px" AutoPostBack="True" TextAlign="Left"
                        Text="Locked Out" OnCheckedChanged="LockedOutCheckBoxCheckedChanged"></FMControls:FMCheckBox>
                </td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMLabel ID="LockedReasonLabel" Style="z-index: 139;" CssClass="formfieldtitle"
                        BackColor="Transparent" runat="server" Width="116px">Locked Out Reason:</FMControls:FMLabel>
                </td>
                <td>
                    <FMControls:FMTextBox ID="LockedOutReasonTextbox" Style="z-index: 140;"
                        TabIndex="1" CssClass="formfield" runat="server" MaxLength="80" Width="202px" Height="75px"
                        TextMode="MultiLine" />
                </td>
               <td></td>
            </tr>
        </table>
	</body>
</HTML>
