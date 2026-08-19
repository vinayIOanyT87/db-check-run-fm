<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="IATACodeSelectForm.aspx.cs" AutoEventWireup="True" Inherits="FuelsManager.FMWebApp.IATACodeSelectForm"%>
<%@ OutputCache Location="None" VaryByParam="None" %>
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

		<script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/javascripts/CFS.js" %>" defer="defer"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<SCRIPT>
			function Select(IATAID, Title, IATAName)
			{
				var Result=new Array();
				Result[0] = IATAID;
				Result[1] = Title;
				Result[2] = IATAName;
				window.returnValue=Result;
				window.close();
			}

			function MultipleSelect()
			{
				var Result=new Array();
				var IATACodeTable = document.getElementById("IATACodesDataGrid");
				if (IATACodeTable != null)
				{
					var resultIndex=0;
					for (index = 0; index < IATACodeTable.rows.length; index++)
					{					
					    if (IATACodeTable.rows(index).className == "GVFixedFooter" ||
					        IATACodeTable.rows(index).className == "GVFixedHeader")
					    {
					        continue;
					    }
					    
					    if (IATACodeTable.rows(index).cells(0).childNodes[0].checked)
						{
					        Result[resultIndex] = IATACodeTable.rows(index).cells(3).innerText;
							resultIndex++;
						}
					}
				}
				window.returnValue=Result;
				window.close();
			}

			function NoSelect()
			{
				var Result=new Array();
				window.returnValue=Result;
				window.close();
			}
		</SCRIPT>
		<form id="Form1" method="post" runat="server">
			<asp:image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server"
				ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
			<asp:textbox id="FindTextBox" alt="Find" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 14px" tabIndex="2"
				runat="server" Width="300px" CssClass="formfield"></asp:textbox>
			<FMCONTROLS:FMBUTTON id="ShowAllBtn" style="Z-INDEX: 104; LEFT: 408px; POSITION: absolute; TOP: 8px"
				tabIndex="4" runat="server" Width="64px" CssClass="formfieldtitle" Text="Show All" onclick="FindAllBtn_OnClick"></FMCONTROLS:FMBUTTON>
			<FMCONTROLS:FMBUTTON id="FindBtn" style="Z-INDEX: 103; LEFT: 328px; POSITION: absolute; TOP: 8px" tabIndex="3"
				runat="server" Width="64px" CssClass="formfieldtitle" Text="Find" onclick="FindBtn_OnClick"></FMCONTROLS:FMBUTTON>
			<TABLE id="Table1" style="Z-INDEX: 101; LEFT: 8px; WIDTH: 600px; POSITION: absolute; TOP: 45px; HEIGHT: 10px"
				cellSpacing="0" cellPadding="1" border="0">
				<tr>
					<TD width="350" height="36" vAlign="middle">
						<FMCONTROLS:FMBUTTON id="AddButton1" tabIndex="3" runat="server" CssClass="formfieldtitle" Width="100px"
							Text="Add"></FMCONTROLS:FMBUTTON> <FMControls:FMLabel Width="500px" ID="lblWarning" runat="server" 
						    CssClass="formfield" Text="abc" Visible="false" ForeColor="Red" />
					</TD>
				</tr>
				<tr>
					<td>
						<FMCONTROLS:FMDATAGRIDFIXED id="IATACodesDataGrid" tabIndex="5" runat="server" 
                            BackColor="White" Width="700px"
							CssClass="tabletext" CellPadding="3" BorderColor="White" AllowSorting="True" BorderWidth="1px" GridLines="Vertical"
							AutoGenerateColumns="False" BorderStyle="None" Height="450px" FixedHeaders="True" 
                            FixedHeight="450px" ShowFooter="True">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<Columns>
								<asp:TemplateColumn>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate></ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Edit">
									<HeaderStyle Width="55px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMEditLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn Visible="False" DataField="SiteGuid" HeaderText="SiteGuid"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="IdentityGuid" 
                                    HeaderText="IdentityGuid"></asp:BoundColumn>
								<asp:BoundColumn DataField="ID" HeaderText="ID">
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Code" HeaderText="Code"></asp:BoundColumn>
								<asp:BoundColumn DataField="Name" HeaderText="Name"></asp:BoundColumn>
								<asp:BoundColumn DataField="Address1" HeaderText="Address"></asp:BoundColumn>
								<asp:BoundColumn DataField="City" HeaderText="City"></asp:BoundColumn>
								<asp:BoundColumn DataField="State" HeaderText="State"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Delete">
									<HeaderStyle Width="0.5in"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<FMControls:FMDeleteLinkButton runat="server" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</FMCONTROLS:FMDATAGRIDFIXED>
					</td>
				</tr>
				<tr>
					<TD width="350" height="36" vAlign="middle">
						<FMCONTROLS:FMBUTTON id="AddButton2" tabIndex="3" runat="server" CssClass="formfieldtitle" Width="100px"
							Text="Add"></FMCONTROLS:FMBUTTON>
					</TD>
				</tr>
			</TABLE>
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
			    catch (err){}
			}
		    // Set the Find Button to be activated by the enter key.
			document.addEventListener('keydown', function(ev) {
			    if (ev.keyCode == 13) {
			        ev.returnValue = false;
			        ev.cancel = true;
			        document.all("FindBtn").click();
			    }});
		</script>
	</body>
</HTML>
