<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="EquipmentSelectForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EquipmentSelectForm" %>
<!DOCTYPE html>
<HTML>
	<HEAD>
        <title></title>
        <base target="_self">
        <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
        <meta content="C#" name="CODE_LANGUAGE">
        <meta content="JavaScript" name="vs_defaultClientScript">
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
<%=  Global.LinkAccessibilityCssUrl(Session) %>

        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>"  defer="defer"></script>
        <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
        <script type="text/javascript">
            function Select(EquipmentID, Title)
			{
                var Result = new Array();
                Result[0] = EquipmentID;
                Result[1] = Title;
                setWindowReturnValue(Result);
                closeDialogWindow();
			}

            function MultipleSelect()
			{
                var Result = new Array();
                var EquipmentTable = document.getElementById("EquipmentDataGrid");
                if (EquipmentTable != null)
				{
                    var resultIndex = 0;
					for (index = 0; index < EquipmentTable.rows.length; index++)
					{
					
                        if (EquipmentTable.rows[index].className == "GVFixedFooter" ||
                            EquipmentTable.rows[index].className == "GVFixedHeader")
					    {
                            continue;
					    }

					
                        if (EquipmentTable.rows[index].cells[0].childNodes[0].checked)
						{
                            Result[resultIndex] = EquipmentTable.rows[index].cells[2].innerText;
                            resultIndex++;
						}
					}
				}
                setWindowReturnValue(Result);
                closeDialogWindow();
			}

            function NoSelect()
			{
                var Result = new Array();
                setWindowReturnValue(Result);
                closeDialogWindow();
			}
        </script>
        <form id="Form1" method="post" runat="server">
            <asp:Image ID="Image1" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
            <asp:TextBox ID="FindTextBox" Style="z-index: 101; left: 8px; position: absolute; top: 14px" TabIndex="2"
                runat="server" Width="300px" CssClass="formfield" MaxLength="30"></asp:TextBox>
            <FMControls:FMButton ID="ShowAllBtn" Style="z-index: 104; left: 408px; position: absolute; top: 8px"
                TabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All" OnClick="FindAllBtn_OnClick"></FMControls:FMButton>
            <FMControls:FMButton ID="FindBtn" Style="z-index: 103; left: 328px; position: absolute; top: 8px" TabIndex="3"
                runat="server" Width="64px" CssClass="formfieldtitle" Text="Find" OnClick="FindBtn_OnClick"></FMControls:FMButton>
            <table id="Table1" style="z-index: 101; left: 8px; width: 50%; position: absolute; top: 45px; height: 10px"
                cellspacing="0" cellpadding="1" border="0">
                <tr>
                    <td width="350" height="36" valign="middle">
                        <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle"
                            TabIndex="6" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 549px; height: 10px" width="549">
                        <FMControls:FMDataGridFixed ID="EquipmentDataGrid" TabIndex="5" runat="server" BackColor="White" Width="800px" RowHeaderColumn="Equipment ID"
                            CssClass="tabletext" PageSize="12" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
                            GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None" Height="380px">
                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
                            <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                            <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                            <Columns>
                                <asp:TemplateColumn HeaderText="">
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate></ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Edit">
                                    <HeaderStyle Width="55px"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMEditLinkButton runat="server" ID="Fmeditlinkbutton1" NAME="Fmeditlinkbutton1" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="IdentityGuid" HeaderText="IdentityGuid"></asp:BoundColumn>
                                <asp:BoundColumn DataField="ID" HeaderText="Equipment ID">
                                    <HeaderStyle Width="2in"></HeaderStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Description" HeaderText="Description">
                                    <HeaderStyle Width="2in"></HeaderStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Make" HeaderText="Make">
                                    <HeaderStyle Width="1in"></HeaderStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Model" HeaderText="Model">
                                    <HeaderStyle Width="1in"></HeaderStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Year" HeaderText="Year">
                                    <HeaderStyle Width="0.5in"></HeaderStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="SerialNumber" HeaderText="SerialNumber">
                                    <HeaderStyle Width="0.5in"></HeaderStyle>
                                </asp:BoundColumn>
                                <asp:BoundColumn Visible="False" DataField="MasterRecordGuid" HeaderText="MasterRecordGuid"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Delete">
                                    <HeaderStyle Width="0.5in"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <FMControls:FMDeleteLinkButton runat="server" ID="Fmdeletelinkbutton1" NAME="Fmdeletelinkbutton1" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                        </FMControls:FMDataGridFixed></td>
                    <tr>
                        <td width="350" height="36" valign="middle">
                            <FMControls:FMButton ID="AddButton1" TabIndex="3" runat="server" CssClass="formfieldtitle" Width="100px"
                                Text="Add"></FMControls:FMButton>
                        </td>
                    </tr>
            </table>
        </form>
		<script language="jscript">
            var findBtn = document.getElementById("FindBtn");
            var findTbBtn = document.getElementById("FindTextBox");

            if (findBtn != null && findTbBtn != null)
			{
			    try
			    {
                    findBtn.setActive();
                    findTbBtn.focus();
			    }
                catch (err) { }
			}
			
			// Set the Find Button to be activated by the enter key.
            document.addEventListener('keydown', function (ev)
            {
                if (ev.keyCode == 13)
                {
                    ev.returnValue = false;
                    ev.cancel = true;
                    document.all("FindBtn").click();
			    }
			});
        </script>
	</body>
</HTML>
