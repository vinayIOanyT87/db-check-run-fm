<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VRUThresholdForm.aspx.cs" Inherits="FuelsManager.FMWebApp.VruThresholdForm" ValidateRequest=false%>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">     
    <title>Throughput Monitor</title>
    <script>
    function doHourglass()
    {
        document.body.style.cursor = 'wait';
        disablecontrols();
    }

    function disablecontrols() 
    {
        document.getElementById("AddButton2").disabled = true;
        document.getElementById("AddButton").disabled = true;
        document.getElementById("RefreshButton").disabled = true;
        document.getElementById("AssignProductsButton").disabled = true;
        document.getElementById("UnassignProductsButton").disabled = true;
    }
    </script>
</head>
<body onbeforeunload="doHourglass();" onunload="doHourglass();">
    <form id="VruThresholdForm" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
            <asp:image id="Image1" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
    				        BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>" alt="<%$ AppSettings: PageFadeImageAlt %>"></asp:image>
            <FMCONTROLS:FMLABEL id="VruThresholdLabel" 
                style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px; width: 1062px;" runat="server"
                BackColor="Transparent" CssClass="headline">Throughput Monitor</FMCONTROLS:FMLABEL>        
			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 32px; POSITION: absolute; TOP: 72px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<TD height="36" vAlign="middle">
						<FMControls:FMButton width="100px" id="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle" />
						&nbsp;&nbsp;
						<FMControls:FMPageSizeDropDown ID="VRUTrackingFormPageSizeDropDown" runat="server" onselectedindexchanged="PageSizeDropDown_SelectedIndexChanged" />
					</TD>
				<TR>
					<TD style="HEIGHT: 10px" >
					    <FMCONTROLS:FMDATAGRID id="VRUConfigurationDataGrid" 
                            style="LEFT: 1px; TOP: 0px; Width:835px;" tabIndex="3" runat="server" PageSize="12"
							BorderStyle="None" BackColor="White" AutoGenerateColumns="False" GridLines="Vertical" 
                            BorderWidth="1px" AllowSorting="True" BorderColor="White"
							CellPadding="3" AllowPaging="True" CssClass="tabletext" >
							<FooterStyle ForeColor="Black" BackColor="#0d246a"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="#0d246a"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="60px" HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton ID="FMEditLinkButton1" runat="server" />
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMUpdateLinkButton ID="FMUpdateLinkButton1" runat="server" ValidationGroup="DatagridValidation"/>&nbsp;
<FMControls:FMCancelLinkButton ID="FMCancelLinkButton1" runat="server" />
									
</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Interval" >
									<HeaderStyle Width="60px" HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Right"></ItemStyle>
									<ItemTemplate>
										<asp:Label Width="60px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Interval") %>' ID="LabelInterval">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Width="43px" Text='<%# DataBinder.Eval(Container, "DataItem.Interval") %>' CssClass="tabletext" ID="IntervalTextBox" MaxLength=8 >
										</asp:TextBox>
									    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="IntervalTextBox"
                                            ValidationGroup="DatagridValidation" Width="15px" Text="*"></asp:RequiredFieldValidator>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Interval Type">
									<HeaderStyle Width="90px" HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label Width="90px" runat="server" ID="IntervalTypeLabel" ></asp:Label>
									</ItemTemplate> 
									<EditItemTemplate>
										<asp:DropDownList Width="90px" CssClass=tabletext runat="server" Enabled="True" ID="DropDownTypeList" DataSource="<%# EnumerateIntervals()%>" DataTextField="Text" DataValueField="Value">
										</asp:DropDownList>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Current Value">
									<HeaderStyle Width="100px" HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Right"></ItemStyle>
									<ItemTemplate>
										<asp:Label Width="100px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CurrentValue") %>' ID="LabelCurrent">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:Label Width="100px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CurrentValue") %>' CssClass="tabletext" ID="CurrentTextBox" MaxLength=25>
										</asp:Label>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Limit Value">
									<HeaderStyle Width="90px" HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Right"></ItemStyle>
									<ItemTemplate>
										<asp:Label Width="90px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Limit") %>' ID="LabelLimit">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox Width="70px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Limit") %>' CssClass="tabletext" ID="LimitTextBox" MaxLength=25>
										</asp:TextBox>
									    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="LimitTextBox"
                                            ValidationGroup="DatagridValidation" Width="20px" Text="*"></asp:RequiredFieldValidator>
									</EditItemTemplate>
								</asp:TemplateColumn>					
								<asp:TemplateColumn HeaderText="Tolerance %">
									<HeaderStyle Width="80px" HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Right"></ItemStyle>
									<ItemTemplate>
										<asp:Label Width="80px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tolerance") %>' ID="LabelTolerance">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Width="60px" Text='<%# DataBinder.Eval(Container, "DataItem.Tolerance") %>' CssClass="tabletext" ID="ToleranceTextBox" MaxLength=3>
										</asp:TextBox>
									    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"  ControlToValidate="ToleranceTextBox"
                                            ValidationGroup="DatagridValidation" Width="20px" Text="*"></asp:RequiredFieldValidator>
									</EditItemTemplate>
								</asp:TemplateColumn>								
								<asp:TemplateColumn HeaderText="Enabled">
									<HeaderStyle Width="50px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMCheckBox ID="EnabledCheckbox" runat="server" Enabled="False" Checked='<%#Eval("Enabled") %>'/>
									</ItemTemplate>
									<EditItemTemplate>
										<FMControls:FMCheckBox ID="EnabledEditCheckbox" runat="server" Enabled="True" Checked='<%#Eval("Enabled") %>'/>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Reset Date">
									<HeaderStyle Width="120px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:Label Width="120px" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ResetDate") %>' ID="LabelResetDate">
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>								
								<asp:TemplateColumn >
									<HeaderStyle></HeaderStyle>
									<ItemTemplate>
										<FMControls:FMButton runat="server" Text='Reset' ID="ButtonResetDate" Width="80%" CssClass="tabletext" CommandName="ResetDate" CommandArgument='<%# Eval("IdentityGuid") %>'>
                                        </FMControls:FMButton>
									</ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
								</asp:TemplateColumn>								
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="20px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton ID="FMDeleteLinkButton1" runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="#0d246a" Mode="NumericPages"></PagerStyle>
						</FMCONTROLS:FMDATAGRID>
					</TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 26px" vAlign="middle" ><FMCONTROLS:FMBUTTON id="AddButton" tabIndex="4" runat="server" Width="98px" CssClass="formfieldtitle"
							Text="Add"></FMCONTROLS:FMBUTTON>
					<FMCONTROLS:FMBUTTON id="RefreshButton" tabIndex="5" runat="server" Width="98px" CssClass="formfieldtitle"
							Text="Refresh"></FMCONTROLS:FMBUTTON>
                    <FMControls:FMLabel id="LabelEmpty"  runat="server" CssClass="formfieldtitle" Width="300px" Height="26px"> </FMControls:FMLabel>
					</TD>
				</TR>					
				<tr style="height: 15px"></tr>
				<tr>
				    <td>
				        <table id="Table2" style="Z-INDEX: 101" cellSpacing="0" cellPadding="1" border="0">
				        <tr>
				            <td >
                                <FMCONTROLS:FMLABEL id="LabelAssigned" style="Z-INDEX: 105" runat="server" CssClass="formfieldtitle">Assigned Products:</FMCONTROLS:FMLABEL>							                
				            </td>
				            <td></td>
				            <td >
				                <FMCONTROLS:FMLABEL id="LabelUnassigned" style="Z-INDEX: 106" runat="server" CssClass="formfieldtitle">Unassigned Products:</FMCONTROLS:FMLABEL>
				            </td>
				           <td style="width:450px" ></td>
				        </tr>
				        <tr>
				            <td style="width: 150px">
				                <asp:listbox id="AssignedProductsListBox" style="Z-INDEX: 107; height: 150px; width: 100% " runat="server" BackColor="White" CssClass="formfield" SelectionMode="Multiple" tabIndex="2"></asp:listbox>
				            </td>
				            <td>
				                <div style="position: relative;padding-left: 18px;">
                                <asp:button id="AssignProductsButton" style="position: relative; width:20px;margin-bottom: 10px;"
				                runat="server" CssClass="formfieldtitle" Text="<<" ToolTip="Assign" tabIndex="6"></asp:button>
				                <asp:button id="UnassignProductsButton" style="position: relative;width:20px"
				                runat="server" CssClass="formfieldtitle" Text=">>" ToolTip="Unassign" tabIndex="7"></asp:button>
				                </div>
				            </td>
				            <td  style="width: 150px">
				   				<asp:listbox id="UnassignedProductsListBox" style="Z-INDEX: 110; height: 150px; width: 100%" runat="server" BackColor="White" CssClass="formfield" SelectionMode="Multiple" tabIndex="5"></asp:listbox>
				            </td>
				           <td></td>
				        </tr>
				        </table>
				</td></tr>
				<tr style="height: 15px"></tr>
				<tr>
				    <td>
                    <FMControls:FMLabel runat="server" ID="FMLabellastupdated" CssClass="formfieldtitle" >Last updated:</FMControls:FMLabel>
                    <FMControls:FMLabel runat="server" ID="FMLabelupdatedDate" CssClass="formfieldtitle"></FMControls:FMLabel>
                    <FMControls:FMLabel runat="server" ID="FMtab">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</FMControls:FMLabel>
                    <FMControls:FMLabel runat="server" ID="FMLabelAutoInterval" CssClass="formfieldtitle">Calculation Frequency in minutes:</FMControls:FMLabel>
                    <FMControls:FMLabel runat="server" ID="FMLabelAutoIntervalMinutes" CssClass="formfieldtitle"></FMControls:FMLabel>
                    </td>
				</tr>
			</TABLE>
			
				
    </div>
    </form>
</body>
</html>
