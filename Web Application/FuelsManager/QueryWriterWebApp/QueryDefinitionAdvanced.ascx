<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QueryDefinitionAdvanced.ascx.cs" Inherits="FuelsManager.QueryWriterWebApp.QueryDefinitionAdvanced" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>


	<head>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</head>
	    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
	        <ContentTemplate>
                <table style="z-index:110; left:8px; top: 10px; position:absolute; width:890px" role="presentation" aria-label="layout">
                    <tr>
                        <td style="width:1in">
                            <FMControls:FMLabel ID="TitleLabel" AssociatedControlID="TitleTextBox" runat="server" CssClass="formfieldtitle" Text="Query Title" />
                        </td>
                        <td style="width:400px">
                            <asp:TextBox id="TitleTextBox" runat="server" style="width:350px" />
                        </td>
                        <td>
                            <FMControls:FMLabel ID="PageSizeLabel" runat="server" Text="Initial Results Page Size" CssClass="formfieldtitle" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width:1in">
                            <FMControls:FMLabel ID="HeaderLabel" AssociatedControlID="HeaderTextBox" runat="server" CssClass="formfieldtitle" Text="Query Header" />
                        </td>
                        <td style="width:400px">
                            <asp:TextBox id="HeaderTextBox" runat="server" style="width:350px" />
                        </td>
                        <td>
                            <FMControls:FMPageSizeDropDown ID="PageSizeDropDown" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width:1in">
                            <FMControls:FMLabel ID="FooterLabel" AssociatedControlID="FooterTextBox" runat="server" CssClass="formfieldtitle" Text="Query Footer" />
                        </td>
                        <td colspan="2">
                            <asp:TextBox id="FooterTextBox" runat="server" style="width:350px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width:1in">
                            <FMControls:FMLabel ID="LocationLabel" AssociatedControlID="MenuLocationTextBox" runat="server" CssClass="formfieldtitle" Text="Menu Path" />
                        </td>
                        <td colspan="2">
                            <asp:TextBox id="MenuLocationTextBox" runat="server" style="width:350px" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <hr style="width:85%; color:Black; size:1pt; top:105px; position:absolute; left:8px"/>
        
	    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
	        <ContentTemplate>
                <table style="z-index:110; left:8px; top: 115px; position:absolute; width:890px" role="presentation" aria-label="layout">
                    <tr>
                        <td style="width:250px"><FMControls:FMCheckBox ID="TotalAllFields" runat="server" CssClass="formfieldtitle" Text="Total All Fields" AutoPostBack="True" /></td>
                        <td><FMControls:FMCheckBox ID="SummaryOnly" runat="server" CssClass="formfieldtitle" Text="Show Summary Lines Only" /></td>
                    </tr>
                    <tr>
                        <td><FMControls:FMCheckBox ID="LineNumbersCheckBox" runat="server" CssClass="formfieldtitle" Text="Include Line Numbers" /></td>
                        <td><FMControls:FMCheckBox ID="ArchiveQueryCheckBox" runat="server" CssClass="formfieldtitle" Text="Query on Archive Data" /></td>
                    </tr>
                    <tr>
                        <td><FMControls:FMCheckBox ID="PreventDeletionCheckBox" runat="server" CssClass="formfieldtitle" Text="Prevent deletion" /></td>
                        <td>&nbsp;</td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <hr style="width:85%; color:Black; size:1pt; top:180px; position:absolute; left:8px"/>
        
        <asp:UpdatePanel ID="GroupsPanel" runat="server">
            <ContentTemplate>
                <table style="z-index:110; left:8px; top: 190px; width:255px; position:absolute" role="presentation" aria-label="layout">
                    <tr>
                        <td style="width:200px">
                            <FMControls:FMLabel ID="AssignedGroupsLabel" AssociatedControlID="AssignedGroupsListBox" runat="server" CssClass="formfieldtitle" Text="Assigned User Groups" /><br />
                        </td>
                        <td style="width:50px">&nbsp;</td>
                        <td>
                            <FMControls:FMLabel ID="UnassignedGroupsLabel" AssociatedControlID="UnassignedGroupsListBox" runat="server" CssClass="formfieldtitle" Text="Unassigned User Groups" /><br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <FMControls:FMListBox ID="AssignedGroupsListBox" runat="server" CssClass="formfield" style="height:125px; width:175px" SelectionMode="Multiple" />
                        </td>
                        <td valign="top">
                            <br />
                            <FMControls:FMButton ID="AssignButton" runat="server" CssClass="formfieldtitle" Text="<<" /><br /><br />
                            <FMControls:FMButton ID="RemoveButton" runat="server" CssClass="formfieldtitle" Text=">>" />
                        </td>
                        <td align="left">
                            <FMControls:FMListBox ID="UnassignedGroupsListBox" runat="server" CssClass="formfield" style="height:125px; width:175px" SelectionMode="Multiple" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <hr style="width:85%; color:Black; size:1pt; top:350px; position:absolute; left:8px"/>
        
        <asp:UpdatePanel ID="GroupingPanel" runat="server">
            <ContentTemplate>
                <table id="grouptable" style="z-index:110; left:575px; top: 190px; position:absolute" role="presentation" aria-label="layout">
                    <tr>
                        <td>
                            <FMControls:FMLabel ID="Group1Label" AssociatedControlID="Group1DropDown" runat="server" CssClass="formfieldtitle" 
                                Text="1st Data Grouping" style="z-index:120" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <FMControls:FMDropDownList ID="Group1DropDown" runat="server" CssClass="formfield" style="width:185px" 
                                OnSelectedIndexChanged="Group1DropDownSelectedIndexChanged" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <FMControls:FMLabel ID="Group2Label" AssociatedControlID="Group2DropDown" runat="server" CssClass="formfieldtitle" Text="2nd Data Grouping" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <FMControls:FMDropDownList ID="Group2DropDown" runat="server" CssClass="formfield" style="width:185px" 
                                Enabled="false" OnSelectedIndexChanged="Group2DropDownSelectedIndexChanged" AutoPostBack="true"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <FMControls:FMLabel ID="Group3Label" AssociatedControlID="Group3DropDown" runat="server" CssClass="formfieldtitle" Text="3rd Data Grouping" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <FMControls:FMDropDownList ID="Group3DropDown" runat="server" CssClass="formfield" style="width:185px" 
                                Enabled="false" OnSelectedIndexChanged="Group3DropDownSelectedIndexChanged" AutoPostBack="true" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <table style="z-index:110; left:8px; top: 360px; width:710px; position:absolute" role="presentation" aria-label="layout">
                    <tr>
                        <td>
                            <FMControls:FMGridView ID="GroupFilterGrid" runat="server" Width="625px" Height="314px" RowHeaderColumn="Field"
                                AllowPaging="false" ShowFooter="true" EmptyDataText="" FooterStyle-CssClass="pgr" FixedHeaders="true" aria-label="Group Filter">
                                <Columns>
                                    <asp:TemplateField HeaderText="Field">
                                        <HeaderStyle Width="2.5in" HorizontalAlign="Left" />
                                        <ItemTemplate>
								            <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Field") %>' ID="FieldLabel"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="True" HeaderText="Filter" ItemStyle-HorizontalAlign="Center">
                                        <HeaderStyle Width="1in" HorizontalAlign="Center"/>
                                        <ItemTemplate>
                                            <FMControls:FMCheckBox ID="FilterCheckBox" runat="server" DataValueField="Filter" Checked='<%# DataBinder.Eval(Container, "DataItem.Filter") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="True" HeaderText="Default Filter Value">
                                        <HeaderStyle Width="2.5in" HorizontalAlign="Left"/>
                                        <ItemTemplate>
                                            <asp:TextBox id="ValueTextBox" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Value") %>' 
                                                style="width:250px" alt="Value"/>
                                            <asp:TextBox id="ValueTextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Value") %>' 
                                                style="width:120px" visible="false" alt="Value 2"/>
                                            <asp:TextBox id="ValueTextBox2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Value2") %>' 
                                                style="width:120px" visible="false" alt="Value 3"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
				            </FMControls:FMGridView>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
