<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QueryDefinitionBasic.ascx.cs" Inherits="FuelsManager.QueryWriterWebApp.QueryDefinitionBasic" EnableViewState="true" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>


	<head>
	    <title></title>
		<link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
        <style type="text/css"> 
            /* Correct placement of Ajax combo box drop-down lists */
            .comboBoxInGrid
            { 
                position: relative; 
            } 
            .comboBoxInGrid ul 
            { 
                position: absolute !important; 
                left: 2px !important; 
                top: 22px !important; 
            }
            .centericon {
                text-align: center;
            }
        </style>
	</head>

        <script type="text/javascript" language="javascript">
            // Get a PageRequestManager reference.
            var prm = Sys.WebForms.PageRequestManager.getInstance();

            // Hook the _initializeRequest event and add our own handler.
            prm.add_initializeRequest(InitializeRequest);

            function InitializeRequest(sender, args) {
                // Check to be sure this async postback is actually
                //   requesting the file download.

                if (sender._postBackSettings.sourceElement.id == "QueryDefinitionBasicPage1_ExportButton") {
                    // Create an IFRAME.
                    var iframe = document.createElement("iframe");

                    // Point the IFRAME to GenerateFile, with the
                    //   desired region as a querystring argument.
                    // The "query" function will append the CSRF token to the string.
                    iframe.src = AddCSRFTokenToUrl("GenerateFile.aspx?Mode=Single");

                    // This makes the IFRAME invisible to the user.
                    iframe.style.display = "none";

                    // Add the IFRAME to the page.  This will trigger
                    //   a request to GenerateFile now.
                    document.body.appendChild(iframe);
                }
            }
        </script>
        
        <asp:button id="HiddenButton" runat="server" CausesValidation="False" OnClick="HiddenButtonClick"
		    style="DISPLAY: none; POSITION: static" Text="Button" />
		    
        <table style="z-index:110; left:8px; top: 10px; width:890px; position:absolute" role="presentation" aria-label="layout">
            <tr>
                <td style="width:1in; vertical-align:top">
                    <table border="0" cellspacing="0" cellpadding="0" role="presentation" aria-label="layout">
                        <tr>
                            <td style="width:85px; background-image:url(../FMWebApp/images/back_grid.gif); background-color:Transparent;">
                                <img src="../FMWebApp/images/Bullet_grid.jpg" alt="Decorative bullet graphic" background="../FMWebApp/images/Back_grid.gif" width="20px" height="21px" align="absmiddle">
                                <FMControls:FMLabel ID="Step1Label" runat="server" CssClass="ehsubhead" Text="Step 1" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="width:1.5in">    
                    <FMControls:FMLabel ID="SelectQueryTypeLabel" runat="server" CssClass="formfieldtitle" Text="Select Query Type" />
                </td>
                <td style="vertical-align:top" rowspan="2">
                    <FMControls:FMLabel ID="QueryTypeLabel" AssociatedControlID="QueryTypeDropDown" runat="server" cssclass="formfieldtitle" Text="Query Type" />
                    <FMControls:FMDropDownList ID="QueryTypeDropDown" runat="server" style="width:150px" AutoPostBack="true"/>
                    
                    &nbsp;&nbsp;&nbsp;
                    <FMControls:FMLabel ID="FMLabelTransactionType" AssociatedControlID="FMDropDownListTransactionAliasTypes" runat="server" CssClass="formfieldtitle"
                        Text="Transaction Type:" />
                    <FMControls:FMListBox ID="FMDropDownListTransactionAliasTypes" OnSelectedIndexChanged="FMDropDownListTransactionAliasTypesSelectedIndexChanged" runat="server" CssClass="formfield" Width="150px" Height="75px" SelectionMode="Multiple" AutoPostBack="true" />
                            
                </td>
            </tr>
            <tr>
                <td colspan="2" style="vertical-align:top; width:200px">
                    <ul style="list-style-type:decimal; list-style-position:outside" class="parabullets">
                        <li><FMControls:FMLabel ID="Step1Text" runat="server">Select the type of query you wish to create.</FMControls:FMLabel></li>
                    </ul>
                </td>
            </tr>
            <tr><td colspan="3"><hr style="width:100%; color:Black; size:1pt"/></td></tr>
            <tr>
                <td style="width:1in; vertical-align:top">
                    <table border="0" cellspacing="0" cellpadding="0" role="presentation" aria-label="layout">
                        <tr>
                            <td style="width:85px; background-image:url(../FMWebApp/images/back_grid.gif); background-color:Transparent;">
                                <img src="../FMWebApp/images/Bullet_grid.jpg" alt="Decorative bullet graphic" background="../FMWebApp/images/Back_grid.gif" width="20px" height="21px" align="absmiddle">
                                <FMControls:FMLabel ID="Step2Label" runat="server" CssClass="ehsubhead" Text="Step 2" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="width:1.5in">
                    <FMControls:FMLabel ID="FMLabel2" runat="server" CssClass="formfieldtitle" Text="Select Query Results" />
                </td>
                <td style="vertical-align:top" rowspan="2">
                    <table role="presentation" aria-label="layout">
                        <tr>
                            <td>&nbsp;</td>
                            <td class="formfieldtitle" style="width:2in" colspan="2">
                                <FMControls:FMLabel ID="SelectedFieldsLabel" AssociatedControlID="SelectedFieldsList" runat="server" CssClass="formfieldtitle" Text="Selected Fields" />
                            </td>
                            <td class="formfieldtitle" style="width:2in">
                                <FMControls:FMLabel ID="AvailableFieldsLabel" AssociatedControlID="AvailableFieldsList" runat="server" CssClass="formfieldtitle" Text="Available Fields" />
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" align="right">
                                <FMControls:FMUpLinkButton ID="MoveUpButton" runat="server"/>
                            </td>
                            <td rowspan="2">
                                <FMControls:FMListBox ID="SelectedFieldsList" runat="server" CssClass="formfield" Width="215px" Height="157px" Sort="false" SelectionMode="Multiple"/>
                            </td>
                            <td valign="middle">
                                <FMControls:FMButton ID="AssignButton" runat="server" CssClass="formfieldtitle" Text="<< Assign" Width="80px" />
                            </td>
                            <td rowspan="2">
                                <FMControls:FMListBox ID="AvailableFieldsList" runat="server" CssClass="formfield" Width="215px" Height="157px" SelectionMode="Multiple" />
                            </td>
                        </tr>
                        <tr>
                            <td valign="bottom" align="right">
                                <FMControls:FMDownLinkButton ID="MoveDownButton" runat="server" />
                            </td>
                            <td valign="top">
                                <FMControls:FMButton ID="RemoveButton" runat="server" CssClass="formfieldtitle" Text="Remove >>" Width="80px" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="vertical-align:top; width:2in">
                    <ul style="list-style-type:decimal; list-style-position:outside;" class="parabullets">
                        <li><FMControls:FMLabel ID="Step2A" runat="server">Select the items you want to appear in your results in the Available Fields list.</FMControls:FMLabel></li>
                        <li><FMControls:FMLabel ID="Step2B" runat="server">Click Assign to add the item to the Selected Fields list.</FMControls:FMLabel></li>
                        <li><FMControls:FMLabel ID="Step2C" runat="server">Select a field or fields in the Selected Fields list.</FMControls:FMLabel></li>
                        <li><FMControls:FMLabel ID="Step2D" runat="server">Use the up and down arrows to set the order of the results.</FMControls:FMLabel></li>
                    </ul>
                </td>
            </tr>
            <tr><td colspan="3"><hr style="width:100%; color:Black; size:1pt"/></td></tr>
            <tr>
                <td style="width:1in; vertical-align:top">
                    <table border="0" cellspacing="0" cellpadding="0" role="presentation" aria-label="layout">
                        <tr>
                            <td style="width:85px; background-image:url(../FMWebApp/images/back_grid.gif); background-color:Transparent;">
                                <img src="../FMWebApp/images/Bullet_grid.jpg" alt="Decorative bullet graphic" background="../FMWebApp/images/Back_grid.gif" width="20px" height="21px" align="absmiddle">
                                <FMControls:FMLabel ID="Step3Label" runat="server" CssClass="ehsubhead" Text="Step 3" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="width:1.5in">
                    <FMControls:FMLabel ID="FMLabel1" runat="server" CssClass="formfieldtitle" Text="Criteria" />
                </td>
                <td>
                    <FMControls:FMButton ID="AddPhraseButton" style="min-width:100px" runat="server" CssClass="formfieldtitle" Text="Add Phrase" />&nbsp;&nbsp;
                    <FMControls:FMButton ID="AddPhraseGroupButton" style="min-width:120px" runat="server" CssClass="formfieldtitle" Text="Add Phrase Group" />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="vertical-align:top; width:2in;">
                    <ul style="list-style-type:decimal; list-style-position:outside" class="parabullets">
                        <li><FMControls:FMLabel ID="Step3A" runat="server">Click Add Phrase</FMControls:FMLabel></li>
                        <li><FMControls:FMLabel ID="Step3B" runat="server">Select a field from the list box.</FMControls:FMLabel></li>
                        <li><FMControls:FMLabel ID="Step3C" runat="server">Select an operator from the drop down.</FMControls:FMLabel></li>
                        <li><FMControls:FMLabel ID="Step3D" runat="server">Enter a value, either numbers or text, the filter will search for.</FMControls:FMLabel></li>
                        <li><FMControls:FMLabel ID="Step3E" runat="server">You can add a criteria group by selecting the Add Group button.</FMControls:FMLabel></li>
                        <li ID="Step3FListItem" runat="server" Visible="false"><FMControls:FMLabel ID="Step3F" runat="server">Add Inventory Date and Product to the criteria for the best results when querying transactions.</FMControls:FMLabel></li>
                    </ul>
                </td>
                <td style="vertical-align:top; height:200px">
                    <br />
                    <FMControls:FMGridView ID="QueryCriteriaGrid" runat="server" FixedHeaders="false" Width="500px" 
                        ShowFooterWhenEmpty="false" EmptyDataText="No Criteria Defined" PagerStyle-CssClass="pgr" AllowPaging="false" aria-label="Query Criteria">
                        <Columns>
                            <asp:TemplateField HeaderText="Select">
                                <ItemTemplate >
                                    <FMControls:FMSelectLinkButton runat="server" CommandName="Select" />
                                </ItemTemplate>
                                <ItemStyle CssClass='centericon' />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Field">
                                <ItemTemplate>
                                    <asp:Panel ID="pnlComboBox" runat="server" CssClass="comboBoxInGrid">
                                        <FMControls:FMDropDownList runat="server" ID="FieldList" DataValueField="Field" Width="150px" alt="Field"/> 
                                    </asp:Panel>
                                    <FMControls:FMLabel ID="GroupLabel" runat="server" Text="" CssClass="formfield" Visible="false" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Operator">
                                <ItemTemplate>
                                    <FMControls:FMDropDownList ID="OperatorList" runat="server" DataValueField="Operator" style="width:140px" AutoPostBack="true"
                                     OnSelectedIndexChanged="OperatorListSelectedIndexChanged" alt="Operator">
                                        <asp:ListItem Text="= Equals" Value="Equals" />
                                        <asp:ListItem Text="&lt; Less Than" Value="LessThan" />
                                        <asp:ListItem Text="&gt; Greater Than" Value="GreaterThan" />
                                        <asp:ListItem Text="&gt;= Greater Than Equal" Value="GreaterThanEqual" />
                                        <asp:ListItem Text="&lt;= Less Than Equal" Value="LessThanEqual" />
                                        <asp:ListItem Text="&lt;&gt; Not Equal" Value="NotEqual" />
                                        <asp:ListItem Text="Like" Value="Like" />
                                        <asp:ListItem Text="Not Like" Value="NotLike" />
                                        <asp:ListItem Text="Contains" Value="Contains" />
                                        <asp:ListItem Text="IN" Value="IN" />
                                        <asp:ListItem Text="Empty" Value="NullOrEmpty" />
                                        <asp:ListItem Text="Not Empty" Value="NotNullOrEmpty" />
                                    </FMControls:FMDropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Value">
                                <ItemTemplate>
                                    <asp:TextBox ID="ValueTextBox" runat="server" style="width:180px" alt="Value"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="And/Or">
                                <ItemTemplate>
                                    <FMControls:FMDropDownList ID="AndOrDropDown" runat="server" DataValueField="AndOr" alt="And/Or">
                                        <asp:ListItem Text="AND" Value="AND" />
                                        <asp:ListItem Text="OR" Value="OR" />
                                    </FMControls:FMDropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <FMControls:FMDeleteLinkButton runat="server" CommandName="Delete" />
                                </ItemTemplate>
                                <ItemStyle CssClass='centericon' />
                            </asp:TemplateField>
                        </Columns>
                    </FMControls:FMGridView>
                </td>
            </tr>
            <tr><td colspan="3"><hr style="width:100%; color:Black; size:1pt"/></td></tr>
            <tr>
                <td style="width:1in; vertical-align:top">
                    <table border="0" cellspacing="0" cellpadding="0" role="presentation" aria-label="layout">
                        <tr>
                            <td style="width:85px; background-image:url(../FMWebApp/images/back_grid.gif); background-color:Transparent;">
                                <img src="../FMWebApp/images/Bullet_grid.jpg" alt="Decorative bullet graphic" background="../FMWebApp/images/Back_grid.gif" width="20px" height="21px" align="absmiddle">
                                <FMControls:FMLabel ID="Step4Label" runat="server" CssClass="ehsubhead" Text="Step 4" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="width:1.5in">
                    <FMControls:FMLabel ID="FMLabel4" runat="server" CssClass="formfieldtitle" Text="Submit, Save or Export" />
                </td>
                <td style="vertical-align:top" rowspan="2">
                    <asp:UpdatePanel ID="UpdatePanelStep4" runat="server">
                        <ContentTemplate>
                            <FMControls:FMButton ID="SubmitButton" runat="server" CssClass="formfieldtitle" Text="Submit" Width="125px" />&nbsp;&nbsp;
                            <FMControls:FMButton ID="SaveButton" runat="server" CssClass="formfieldtitle" Text="Save" Width="75px" />&nbsp;
                            <FMControls:FMButton ID="ExportButton" runat="server" CssClass="formfieldtitle" Text="Export" Width="75px" />
                            &nbsp;&nbsp;<FMControls:FMButton ID="ManageQueriesButton" runat="server" CssClass="formfieldtitle" Text="Manage Queries" />
                            
                            <asp:Panel ID="Panel1" runat="server" Style="display:none; background-color:#EEEEEE; width:455px; height:300px">
                                <table role="presentation" aria-label="layout">
                                    <tr>
                                        <td colspan="2">
                                            <asp:Panel ID="SaveChangesHandle" runat="server" CssClass="formfieldtitle" BackColor="<%$ AppSettings: ColorHeaderBlue %>" style="text-align:center; width:450px; color:White">
                                                Save Changes
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="NameLabel" AssociatedControlID="NameTextBox" runat="server" CssClass="formfieldtitle" Text="Name" style="position: relative; left:5px" />
                                        </td>
                                        <td>
                                            <FMControls:FMButton ID="ApplyButton" runat="server" CssClass="formfieldtitle" Text="Apply" 
                                                style="width:80px; height:25px" TabIndex="3" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="NameTextBox" runat="server" CssClass="formfield" style="width:330px; position:relative; left:5px" TabIndex="1" />
                                        </td>
                                        <td>
                                            <FMControls:FMButton ID="CancelButton" runat="server" CssClass="formfieldtitle" Text="Cancel" style="width:80px" TabIndex="4" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMLabel ID="DescriptionLabel" AssociatedControlID="DescriptionTextBox" runat="server" CssClass="formfieldtitle" Text="Description" style="position:relative; left:5px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <FMControls:FMTextBox ID="DescriptionTextBox" runat="server" CssClass="formfield" style="width:330px; height:160px; position:relative; left:5px" TextMode="MultiLine" TabIndex="2" MaxLength="500"/>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" 
                                TargetControlID="SaveButton"
                                PopupControlID="Panel1" 
                                BackgroundCssClass="modalBackground" 
                                OkControlID="ApplyButton"
                                CancelControlID="CancelButton" 
                                DropShadow="true" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="vertical-align:top; width:2in;">
                    <ul style="list-style-type:decimal; list-style-position:outside" class="parabullets">
                        <li><FMControls:FMLabel ID="Step4A" runat="server">Submit, save, or export the query by selecting the desired button.</FMControls:FMLabel></li>
                    </ul>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <span id="QueryDefinitionBasicPage1_FMLabel4" class="formfieldtitle">*Query Writer should be used for small scale data, if you are experiencing timeouts try to narrow your query parameters</span>
                </td>
            </tr>
        </table>
        
        <script type="text/javascript">
            function fnClickOK(sender, e) {
                __doPostBack(sender, e);
            }

		    function ConfirmProcess()
		    {
			    if(confirm("Query already exists. Do you want to overwrite?")) {
				    document.getElementById("QueryDefinitionBasicPage1_HiddenButton").click();
			    }
		    }
        </script>