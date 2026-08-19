<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="CompartmentSelectForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.CompartmentSelectForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
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
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<script type="text/javascript">
			function Select(ID,Title)
			{
				var Result=new Array();
				Result[0]=ID;
				Result[1]=Title;
				window.returnValue=Result;
				window.close();
                setWindowReturnValue(Result);
                closeDialogWindow();
			}
			
			function MultipleSelect()
			{
				var Result=new Array();
				var EquipmentTable=document.getElementById("ComparmentDataGrid");
				if(EquipmentTable != null)
				{
					var resultIndex=0;
					for(index=0;index < EquipmentTable.rows.length;index++)
					{										
					    if (EquipmentTable.rows(index).className == "GVFixedFooter" ||
					        EquipmentTable.rows(index).className == "GVFixedHeader")
					    {
					        continue;
					    }
					    
					    if (EquipmentTable.rows(index).cells(0).childNodes[0].checked)
						{
							Result[resultIndex]=EquipmentTable.rows(index).cells(1).innerText;
							resultIndex++;
						}
					}
				}
				window.returnValue=Result;
				window.close();
                setWindowReturnValue(Result);
                closeDialogWindow();
			}
        </script>
		<form id="Form1" method="post" runat="server">
			<asp:image id="Image1" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server" RowHeaderColumn="ID"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image><FMCONTROLS:FMDATAGRID id="CompartmentDataGrid" style="Z-INDEX: 102; LEFT: 8px; POSITION: absolute; TOP: 16px"
				tabIndex="5" runat="server" BackColor="White" Width="8.5in" CssClass="tabletext" PageSize="12" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px"
				GridLines="Vertical" AutoGenerateColumns="False" BorderStyle="None">
				<FooterStyle ForeColor="Black" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></FooterStyle>
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
				<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				<HeaderStyle Font-Bold="True" ForeColor="White" CssClass="tablecolhead" BackColor="<%$ AppSettings: ColorHeaderBlue %>"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<HeaderStyle Width="0.5in"></HeaderStyle>
						<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
						<ItemTemplate></ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="EquipmentSequence" HeaderText="ID">
						<HeaderStyle Width="2in"></HeaderStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="Capacity" HeaderText="Capacity">
						<HeaderStyle Width="2in"></HeaderStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="SafeFill" HeaderText="Safe Fill">
						<HeaderStyle Width="1in"></HeaderStyle>
					</asp:BoundColumn>
				</Columns>
				<PagerStyle CssClass="tablepager" ForeColor="White" BackColor="<%$ AppSettings: ColorHeaderBlue %>" Mode="NumericPages"></PagerStyle>
			</FMCONTROLS:FMDATAGRID></form>
	</body>
</HTML>
