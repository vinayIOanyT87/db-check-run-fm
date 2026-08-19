<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TestSetResultGeneralPage.ascx.cs" Inherits="FuelsManager.QualityControlWebApp.TestSetResultGeneralPage" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<html>
	<head>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
		<style type="text/css">
			.style1
			{
				 width: 100px;
			}
			.style2
			{
				 width: 345px;
			}
			.style3
			{
				 width: 100px;
			}
			.style4
			{
				 width: 345px;
			}
		</style>
	</head>
	<body>
		<asp:UpdatePanel ID="UpdatePanel1" runat="server">
			<ContentTemplate>			
				<table style="Z-INDEX: 103; width:890px; LEFT: 5px; POSITION: absolute; TOP: 0px; height: 650px;">
					<tr style="height:30px">
						<td class="style1">
							<fmcontrols:fmlabel id="AssetTypeLabel" AssociatedControlID="AssetTypeDropDownList" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="80px">Asset Type:
							</fmcontrols:fmlabel>
						</td>
                        <td>
						</td>
						<td class="style2">
							<fmcontrols:fmdropdownlist Width="100px" CssClass="formfield" runat="server" AutoPostBack="True" Enabled="True" tabIndex="1" ID="AssetTypeDropDownList"
								OnSelectedIndexChanged="AssetTypeDropDownListSelectedIndexChanged">
							</fmcontrols:fmdropdownlist>
						</td>
						<td class="style3">
							<fmcontrols:fmlabel id="AssetLabel" AssociatedControlID="AssetDropDownList" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="80px">Asset:
							</fmcontrols:fmlabel>
						</td>
						<td>
                            <asp:Label style="Z-INDEX: 105; width: 1px; height: 15px;" runat="server" 
                                CssClass="formfieldtitle" ForeColor="Crimson" BackColor="Transparent" Text="*"/>
						</td>
						<td class="style4">
							<fmcontrols:fmdropdownlist id="AssetDropDownList" tabIndex="2" Width="200px" CssClass="formfield" runat="server" Enabled="True" aria-required="true"
								AutoPostBack="False">
							</fmcontrols:fmdropdownlist>
						</td>
					</tr>
					<tr style="height:30px">
						<td class="style1">
							<fmcontrols:fmlabel id="TestSetLabel" AssociatedControlID="TestSetDropDownList" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="80px">Test Set:
							</fmcontrols:fmlabel>
						</td>
						<td>
                            <asp:Label ID="Label1" style="Z-INDEX: 105; width: 1px; height: 15px;" runat="server" 
                                CssClass="formfieldtitle" ForeColor="Crimson" BackColor="Transparent" Text="*"/>
						</td>
                        <td class="style2">
							<fmcontrols:fmdropdownlist id="TestSetDropDownList" tabIndex="3" Width="200px" CssClass="formfield" runat="server" Enabled="True" aria-required="true"
								AutoPostBack="True" OnSelectedIndexChanged="TestSetDropDownListSelectedIndexChanged">
							</fmcontrols:fmdropdownlist>
						</td>
						<td class="style3">
								<fmcontrols:fmlabel id="TestDateLabel" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
									Width="80px">Test Date:
								</fmcontrols:fmlabel>
						</td>
						<td>
						</td>
						<td class="style4">
                		<fmcontrols:fmdatetime id="TestDate"  FormatInfo="<%# this.DateFormat %>" tabIndex="4" Width="280px" 
                			CssClass="formfield" runat="server">
                		</fmcontrols:fmdatetime>
						</td>
					</tr>
					<tr style="height:30px">
						<td class="style1">
							<fmcontrols:fmlabel id="OperatorLabel" AssociatedControlID="OperatorDropDownList" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="80px">Operator:
							</fmcontrols:fmlabel>
						</td>
						<td>
                            <asp:Label ID="Label2" style="Z-INDEX: 105; width: 1px; height: 15px;" runat="server" 
                                CssClass="formfieldtitle" ForeColor="Crimson" BackColor="Transparent" Text="*"/>
						</td>
						<td class="style2">
							<fmcontrols:fmdropdownlist id="OperatorDropDownList" tabIndex="5" Width="200px" CssClass="formfield" runat="server" Enabled="True" aria-required="true"
								AutoPostBack="False" OnSelectedIndexChanged="OperatorDropDownListSelectedIndexChanged">
							</fmcontrols:fmdropdownlist>
						</td>
						<td class="style3">
							<fmcontrols:fmlabel id="SupervisorLabel" AssociatedControlID="SupervisorDropDownList" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="80px">Supervisor:
							</fmcontrols:fmlabel>
						</td>
						<td>
						</td>
						<td class="style4">
							<fmcontrols:fmdropdownlist id="SupervisorDropDownList" tabIndex="6" Width="200px" CssClass="formfield" runat="server" Enabled="True" 
								AutoPostBack="False" OnSelectedIndexChanged="SupervisorDropDownListSelectedIndexChanged">
							</fmcontrols:fmdropdownlist>
						</td>
					</tr>
					<tr style="height:30px">
						<td class="style1">
							<fmcontrols:fmlabel id="SampleSizeLabel" AssociatedControlID="SampleSizeTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="80px">Sample Size:
							</fmcontrols:fmlabel>
						</td>
						<td>
						</td>
						<td class="style2">
                		<asp:textbox id="SampleSizeTextbox" tabIndex="7" Width="168px" CssClass="formfield" runat="server" MaxLength="32">
                		</asp:textbox>
						</td>
						<td class="style3">
							<fmcontrols:fmlabel id="SampleNumberLabel" AssociatedControlID="SampleNumberTextbox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="80px">Sample Number:
							</fmcontrols:fmlabel>
						</td>
						<td>
						</td>
						<td class="style4">
                		<asp:textbox id="SampleNumberTextbox" tabIndex="8" Width="168px" CssClass="formfield" runat="server" MaxLength="32">
                		</asp:textbox>
						</td>
					</tr>
					<tr style="height:30px">
						<td class="style1">
							<fmcontrols:fmlabel id="QuantityRepLabel" AssociatedControlID="QuantityRepTextbox" style="white-space: break-spaces;" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="80px">Quantity Represented:
							</fmcontrols:fmlabel>
						</td>
						<td>
						</td>
						<td class="style2">
                		<asp:textbox id="QuantityRepTextbox" tabIndex="9" Width="168px" CssClass="formfield" runat="server" MaxLength="32">
                		</asp:textbox>
						</td>
						<td class="style3">
							<fmcontrols:fmlabel id="PreviousSampleLabel" AssociatedControlID="PreviousSampleTextbox" style="white-space: break-spaces;"  CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="90px">Previous Sample Number:
							</fmcontrols:fmlabel>
						</td>
						<td>
						</td>
						<td class="style4">
                		<asp:textbox id="PreviousSampleTextbox" tabIndex="11" Width="168px" CssClass="formfield" runat="server" MaxLength="32">
                		</asp:textbox>
						</td>
					</tr>
					<tr style="height:30px">
						<td class="style1">
							<fmcontrols:fmcheckbox id="IsRetestCheckBox" tabIndex="10" TextAlign="Left" 
								Text="Is Retest:" Height="27px" 
								Width="88px" CssClass="formfieldtitle" runat="server" AutoPostBack="True" 
								oncheckedchanged="IsRestestCheckBoxCheckChanged" >
							</fmcontrols:fmcheckbox>
						</td>
						<td>
						</td>
						<td class="style2">
						  &nbsp;
						</td>
						<td class="style3">
							&nbsp;
						</td>
						<td>
						</td>
						<td class="style4">
						  &nbsp;
						</td>
					</tr>
					<tr style="height:30px">
						<td class="style1">
							<fmcontrols:fmlabel id="MemoLabel" AssociatedControlID="MemoTextBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="80px">Memo:
							</fmcontrols:fmlabel>
						</td>
						<td colspan="5">
							<asp:textbox id="MemoTextBox" tabIndex="12" runat="server" Width="500px" CssClass="formfield" 
								Height="50px" TextMode="MultiLine">
							</asp:textbox>						
						</td>
					</tr>
					<tr style="height:30px">
						<td class="style1">
							<fmcontrols:fmlabel id="StatusLabel" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="80px">Status:
							</fmcontrols:fmlabel>
						</td>
						<td>
						</td>
						<td class="style2">
							<fmcontrols:fmlabel id="StatusLabelBox" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="80px">
							</fmcontrols:fmlabel>
						</td>
						<td class="style3">
						  &nbsp;
						</td>
						<td>
						</td>
						<td class="style4">
						  &nbsp;
						</td>
					</tr>
					<tr style="height:30px">
						<td class="style1">
							<fmcontrols:fmlabel id="TestResultsLabel" CssClass="formfieldtitle" runat="server" BackColor="Transparent" 
								Width="80px">Test Results:
							</fmcontrols:fmlabel>
						</td>
						<td>
						</td>
						<td class="style2">
						  &nbsp;
						</td>
						<td class="style3">
						  &nbsp;
						</td>
						<td>
						</td>
						<td class="style4">
						  &nbsp;
						</td>
					</tr>
					<tr>
						<td colspan="6" style="vertical-align:top">
							<FMControls:FMGridView ID="TestResultsGridView" runat="server"  AutoGenerateColumns="False"  AllowSorting="true" RowHeaderColumn="Test"
								FixedHeaders="false" Width="890px" PagerStyle-CssClass="pgr"
								OnRowDataBound="TestResultsGridViewRowDataBound" 
								OnRowUpdating="TestResultsGridViewRowUpdating"
								OnPageIndexChanging="TestResultsGridViewPageIndexChanging">
								<Columns>
                           <asp:TemplateField HeaderText="Edit">
										<HeaderStyle Width="0.5in" />
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
										<ItemTemplate>
											<FMControls:FMEditLinkButton ID="EditButton" OnCommand="TestResultsGridViewRowEditing" runat="server" />
										</ItemTemplate>
										<EditItemTemplate>
					                  <FMControls:FMUpdateLinkButton runat="server" ID="TestResultsGridView_RowUpdating" />&nbsp;
						               <FMControls:FMCancelLinkButton runat="server" ID="CancelButton"  OnCommand="TestResultsGridViewRowCancelingEdit" />
										</EditItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="IdentityGuid" Visible="false">
                               <HeaderStyle Width="50px" />
                               <ItemTemplate>
									<asp:Label Width=.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdentityGuid") %>' ID="IdentityGuid">
									</asp:Label>
                               </ItemTemplate>
                            </asp:TemplateField>
							<asp:TemplateField HeaderText="CollIndex" Visible="false">
							   <HeaderStyle Width="50px" />
                               <ItemTemplate>
											<asp:Label Width=.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CollIndex") %>' ID="CollIndex">
											</asp:Label>
                               </ItemTemplate>
                           </asp:TemplateField>
									
									<asp:TemplateField HeaderText="Test">
                               <HeaderStyle Width="50px" />
                               <ItemTemplate>
											<asp:Label Width=.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Test") %>' ID="TestID">
											</asp:Label>
                               </ItemTemplate>
                           </asp:TemplateField>
									
									<asp:TemplateField HeaderText="Result">
                               <HeaderStyle Width="50px" />
                               <ItemTemplate>
											<asp:Label Width=.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Result") %>' ID="ResultID">
											</asp:Label>
                               </ItemTemplate>
                             <EditItemTemplate>
					            <asp:TextBox Width=.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Result") %>' ID="ResultID1"/>
                             </EditItemTemplate>
                           </asp:TemplateField>
									
									<asp:TemplateField HeaderText="Status">
                               <HeaderStyle Width="50px" />
                               <ItemTemplate>
											<asp:Label Width=.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Status") %>' ID="StatusID">
											</asp:Label>
                               </ItemTemplate>
                           </asp:TemplateField>
									
									<asp:TemplateField HeaderText="Passing Range">
                               <HeaderStyle Width="50px" />
                               <ItemTemplate>
											<asp:Label Width=.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Passing Range") %>' ID="PassingRangeID">
											</asp:Label>
                               </ItemTemplate>
                           </asp:TemplateField>
									
									<asp:TemplateField HeaderText="Test Date">
                               <HeaderStyle Width="150px" />
                               <ItemTemplate>
											<asp:Label Width=1.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Test Date") %>' ID="TestDateID">
											</asp:Label>
                               </ItemTemplate>
                             <EditItemTemplate>
					            <FMControls:FMDateTime Width=2.5in runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Test Date") %>' ID="TestDateID1"/>
                             </EditItemTemplate>
                           </asp:TemplateField>
                           
                       <asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Performed By" >
                             <ItemTemplate>
                                 <asp:Label ID="PerformedByID" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Performed By") %> '></asp:Label>
                             </ItemTemplate>
                             <EditItemTemplate>
                                 <FMControls:FMDropDownList ID="PerformedByID1" Font-Size="XX-Small" runat="server"   
                                 DataSource="<%# EnumerateOperatorNames() %>" />
                             </EditItemTemplate>
                       </asp:TemplateField>
                           
                       <asp:TemplateField ConvertEmptyStringToNull="False" HeaderText="Supervisor" >
                             <ItemTemplate>
                                 <asp:Label ID="SupervisorID" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Supervisor") %> '></asp:Label>
                             </ItemTemplate>
                             <EditItemTemplate>
                                 <FMControls:FMDropDownList ID="SupervisorID1" Font-Size="XX-Small" runat="server"   
                                 DataSource="<%# EnumerateSupervisorNames() %>" />
                             </EditItemTemplate>
                       </asp:TemplateField>
                           
									
								</Columns>
							</FMControls:FMGridView>
						</td>
					</tr>
				</table>
			</ContentTemplate>
		</asp:UpdatePanel>
	</body>
</html>