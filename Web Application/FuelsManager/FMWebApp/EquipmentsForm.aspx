<%@ Page Language="c#" CodeBehind="EquipmentsForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.EquipmentsForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <meta http-equiv="cache-control" content="private, max-age=0" />
        <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        <style>
            #pnlContainer {
                display: table;
            }
        </style>
    </HEAD>
	<body MS_POSITIONING="GridLayout">
        <form id="Form1" method="post" enctype="multipart/form-data" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent" style="position: absolute">
                <asp:Image ID="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" Style="z-index: 100; left: 0px; position: absolute; top: 0px" runat="server"
                    BackColor="Transparent" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
                <FMControls:FMLabel ID="Label2" Style="z-index: 103; left: 8px; position: absolute; top: 8px" runat="server"
                    CssClass="headline" Width="264px" BackColor="Transparent">Equipment Configuration</FMControls:FMLabel>
                <FMControls:FMLabel ID="FindStringLabel" AssociatedControlID="FindTextBox" Style="z-index: 106; left: 32px; position: absolute; top: 48px"
                    runat="server" BackColor="Transparent" CssClass="formfieldtitle">Find String:</FMControls:FMLabel>
                <asp:TextBox ID="FindTextBox" Style="z-index: 107; left: 32px; position: absolute; top: 64px"
                    runat="server" Width="308px" MaxLength="30"></asp:TextBox>

                <FMControls:FMButton ID="SearchEnterpriseBtn" Style="z-index: 108; left: 350px; position: absolute; top: 58px"
                    runat="server" Width="64px" CssClass="formfieldtitle" Text="Search" OnClick="SearchEnterpriseBtnOnClick"></FMControls:FMButton>

                <FMControls:FMButton ID="FindBtn" Style="z-index: 108; left: 434px; position: absolute; top: 58px"
                    runat="server" Width="64px" CssClass="formfieldtitle" Text="Filter" OnClick="FindBtnOnClick"></FMControls:FMButton>

                <FMControls:FMButton ID="ShowAllButton" Style="z-index: 109; left: 518px; position: absolute; top: 58px" runat="server"
                    Width="64px" CssClass="formfieldtitle" Text="Show All" OnClick="ShowAllBtnOnClick"></FMControls:FMButton>

                <FMControls:FMLabel ID="Label" AssociatedControlID="EquipmentTypeDropDownList" Style="z-index: 104; left: 32px; position: absolute; top: 107px" runat="server"
                    CssClass="formfieldtitle" BackColor="Transparent">Type:</FMControls:FMLabel>
                <FMControls:FMDropDownList ID="EquipmentTypeDropDownList" Style="z-index: 105; left: 32px; position: absolute; top: 123px" Height="20"
                    runat="server" CssClass="formfield" Width="112px" AutoPostBack="True" OnSelectedIndexChanged="EquipmentTypeDropDownListSelectedIndexChanged">
                </FMControls:FMDropDownList>

                <FMControls:FMLabel ID="FMLABEL1" AssociatedControlID="EquipmentTypeClassDropDownList"
                    Style="z-index: 104; left: 149px; position: absolute; top: 107px" runat="server"
                    CssClass="formfieldtitle" BackColor="Transparent">Equipment Type:</FMControls:FMLabel>
                <FMControls:FMDropDownList ID="EquipmentTypeClassDropDownList" Style="z-index: 105; left: 149px; position: absolute; top: 123px"
                    runat="server" CssClass="formfield" Width="112px" AutoPostBack="True" Height="20"
                    OnSelectedIndexChanged="EquipmentTypeClassDropDownListSelectedIndexChanged">
                </FMControls:FMDropDownList>

                <FMControls:FMLabel ID="TrailerIDLabel" AssociatedControlID="TrailerIDSearchBox" Style="z-index: 106; left: 266px; position: absolute; top: 107px"
                    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Visible="false">Trailer ID:</FMControls:FMLabel>
                <asp:TextBox ID="TrailerIDSearchBox" Style="height:20px;z-index: 107; left: 266px; position: absolute; top: 123px;padding: 0px 2px;"
                    runat="server" Width="105px" MaxLength="100" Visible="false"></asp:TextBox>

                <FMControls:FMLabel ID="ProductLabel" AssociatedControlID="ProductSearchBox" Style="z-index: 106; left: 384px; position: absolute; top: 107px"
                    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Visible="false">Product:</FMControls:FMLabel>
                <asp:TextBox ID="ProductSearchBox" Style="height:20px;z-index: 107; left: 384px; position: absolute; top: 123px;padding: 0px 2px;"
                    runat="server" Width="105px" MaxLength="100" Visible="false"></asp:TextBox>

                <FMControls:FMLabel ID="CompanyLabel" AssociatedControlID="CompanySearchBox" Style="z-index: 106; left: 502px; position: absolute; top: 107px"
                    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Visible="false">Company:</FMControls:FMLabel>
                <asp:TextBox ID="CompanySearchBox" Style="height:20px;z-index: 107; left: 502px; position: absolute; top: 123px;padding: 0px 2px;"
                    runat="server" Width="105px" MaxLength="100" Visible="false"></asp:TextBox>

                <FMControls:FMLabel ID="CompanyEquipmentIDLabel" AssociatedControlID="CompanyEquipmentIDSearchBox" Style="z-index: 106; left: 620px; position: absolute; top: 107px"
                    runat="server" BackColor="Transparent" CssClass="formfieldtitle" Visible="false">Company Equipment ID:</FMControls:FMLabel>
                <asp:TextBox ID="CompanyEquipmentIDSearchBox" Style="height:20px;z-index: 107; left: 620px; position: absolute; top: 123px;padding: 0px 2px;"
                    runat="server" Width="138px" MaxLength="100" Visible="false"></asp:TextBox>

                <FMControls:FMCheckBox ID="ShowHiddenCheckBox" Style="z-index: 110; left: 499px; position: absolute; top: 150px"
                    CssClass="formfieldtitle" runat="server" Text="Show Hidden" AutoPostBack="True"
                    OnCheckedChanged="ShowHiddenCheckBox_OnCheckedChanged" />

                <FMControls:FMCheckBox ID="ManagedEquipmentCheckBox" Style="z-index: 105; left: 499px; position: absolute; top: 170px; width: 239px;"
                    runat="server" CssClass="formfieldtitle" AutoPostBack="True"
                    Text="Show Managed Equipment only" Checked="true"
                    OnCheckedChanged="ManagedEquipmentCheckBoxCheckedChanged" />

                <FMControls:FMCheckBox ID="SecondaryStorageCheckBox" Style="z-index: 105; left: 499px; position: absolute; top: 190px; width: 304px; right: 321px;"
                    runat="server" CssClass="formfieldtitle" AutoPostBack="True"
                    Text="Show Secondary Storage only"
                    OnCheckedChanged="SecondaryStorageCheckBoxCheckedChanged" />
                <table id="Table1" style="z-index: 101; left: 32px; width: 760px; position: absolute; top: 200px; height: 10px"
                    cellspacing="0" cellpadding="1" border="0">
                    <tr>
                        <td width="750" height="36" valign="middle">
                            <FMControls:FMButton Width="100px" ID="AddButton2" runat="server" Text="Add" CssClass="formfieldtitle" />
                            <FMControls:FMPageSizeDropDown ID="EquipmentSummaryPageSizeDropDown" ToolTip="Page size" runat="server" OnSelectedIndexChanged="PageSizeDropDownSelectedIndexChanged" />
                            <FMControls:FMLabel Width="500px" ID="lblWarning" runat="server" CssClass="formfield" Text="abc" Visible="false" ForeColor="Red" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 550px; height: 10px">
                            <!-- In the below grid, including "Remote" a second time because the disabled checkbox will not report its status correctly -->
                            <FMControls:FMDataGridFixedPaging ID="EquipmentsDataGrid" Style="left: 1px; top: 0px" runat="server" PageSize="12"
                                AutoGenerateColumns="False" Width="100%"
                                DataKeyNames="SiteGuid, IdentityGuid" AllowSorting="True"
                                ShowHeaderWhenEmpty="True" FixedHeaders="True" UseAccessibleHeader="False"
                                AllowPaging="True" ShowFooter="False" ShowFooterWhenEmpty="False"
                                BackColor="White" BorderStyle="Solid" BorderWidth="1px"
                                CellPadding="3" CssClass="tabletext" EmptyDataText="No records found"
                                GridLines="Vertical" GroupColumnOffset="0" GroupingDepth="0">
                                <AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
                                <ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
                                <Columns>
                                    <asp:TemplateColumn HeaderText="Assign">
                                        <HeaderStyle Width="0.5in" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <FMControls:FMSelectLinkButton ID="AssignButton" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Edit">
                                        <HeaderStyle Width="0.5in" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <FMControls:FMEditLinkButton ID="EditButton" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid" />
                                    <asp:BoundColumn Visible="False" DataField="EquipmentGuid" HeaderText="IdentityGuid" />
                                    <asp:BoundColumn DataField="ID" HeaderText="Equipment ID" SortExpression="ID" />
                                    <asp:BoundColumn DataField="Volume" HeaderText="Volume" SortExpression="Volume" />
                                    <asp:BoundColumn DataField="QCDate" HeaderText="QC Due Date" SortExpression="QCDate" />
                                    <asp:BoundColumn DataField="ReturnToServiceDate" HeaderText="Return To Service Date" SortExpression="ReturnToServiceDate" />
                                    <asp:BoundColumn DataField="InServiceFlag" HeaderText="In Service" SortExpression="InServiceFlag" />
                                    <asp:BoundColumn DataField="LockedOut" HeaderText="Locked Out" SortExpression="LockedOut" />
                                    <asp:BoundColumn DataField="Capacity" HeaderText="Capacity" SortExpression="Capacity" />
                                    <asp:BoundColumn DataField="VolumeUnit" HeaderText="Units" SortExpression="VolumeUnit" />
                                    <asp:BoundColumn DataField="CompanyEquipmentID" HeaderText="Company Equipment ID" SortExpression="CompanyEquipmentID" />
                                    <asp:BoundColumn DataField="Company" HeaderText="Company" SortExpression="Company" />
                                    <asp:BoundColumn DataField="ProductID" HeaderText="Product ID" SortExpression="ProductID" />
                                    <asp:BoundColumn DataField="Description" HeaderText="Description" SortExpression="Description" />
                                    <asp:BoundColumn DataField="EqTypeName" HeaderText="Type" SortExpression="EqTypeName" />
                                    <asp:BoundColumn DataField="Make" HeaderText="Make" SortExpression="Make" />
                                    <asp:BoundColumn DataField="Model" HeaderText="Model" SortExpression="Model" />
                                    <asp:BoundColumn DataField="Year" HeaderText="Year" SortExpression="Year" />
                                    <asp:BoundColumn DataField="SerialNumber" HeaderText="Serial Number" SortExpression="SerialNumber" />
                                    <asp:BoundColumn DataField="FuelCardID" HeaderText="Fuel Card ID" SortExpression="ID" />
                                    <asp:TemplateColumn HeaderText="Locked Out">
                                        <HeaderStyle Width="70px" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <FMControls:FMCheckBox ID="GlobalLockedOut" runat="server" Enabled="false" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Delete">
                                        <HeaderStyle Width="0.5in" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <FMControls:FMDeleteLinkButton ID="DeleteButton" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Enterprise Only" SortExpression="Remote">
                                        <HeaderStyle Width="80px" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <FMControls:FMCheckBox ID="RemoteCheckBox" runat="server" Enabled="false" />
                                        </ItemTemplate>
                                    </asp:TemplateColumn>
                                    <asp:BoundColumn Visible="False" DataField="_MasterRecordGuid" HeaderText="MasterRecordGuid" />
                                    <asp:BoundColumn Visible="False" DataField="Remote" HeaderText="Remote" />
                                </Columns>
                                <PagerStyle CssClass="GVFixedFooter" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages" />
                            </FMControls:FMDataGridFixedPaging></td>
                    </tr>
                    <tr>
                        <td style="width: 163px; height: 36px" valign="middle" width="163">
                            <FMControls:FMButton ID="AddButton" runat="server" CssClass="formfieldtitle" Width="98px" Text="Add" ></FMControls:FMButton></td>
                    </tr>
                </table>

            </div>
        </form>
		<script language="jscript">
		   var findBtn = document.getElementById("FindBtn");
			var findTbBtn = document.getElementById("FindTextBox");
			
			if (findBtn != null && findTbBtn != null)
			{
				try
				{
					findBtn.focus();
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
