<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrainingAssignments.aspx.cs" Inherits="FuelsManager.TrainingWebApp.TrainingAssignments" %>

<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Register TagPrefix="FMMenuBar" Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
</head>
<body>
    <form id="form1" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position: absolute">
            <asp:Image ID="FadeImage" alt="<%$ AppSettings: PageFadeImageAlt %>"
                Style="z-index: -2; left: 0px; position: absolute; top: 0px" runat="server"
                ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent" />

            <FMControls:FMLabel ID="FMLabel1" runat="server" CssClass="headline"
                Text="Training Assignments" Style="left: 8px; top: 8px; position: absolute" BackColor="Transparent"
                Font-Bold="True" />
		<FMControls:FMLabel id="Label1" AssociatedControlID="TrainingDropDownList" style="Z-INDEX: 102; LEFT: 50px; POSITION: absolute; TOP: 40px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Training Item:</FMControls:FMLabel>
		<asp:dropdownlist id="TrainingDropDownList" style="Z-INDEX: 106; LEFT: 150px; POSITION: absolute; TOP: 40px"
			runat="server" CssClass="formfield" Width="170px" AutoPostBack="true" 
			 tabIndex="1" onselectedindexchanged="OnTrainingItemSelectedIndexChanged" ></asp:dropdownlist>
		<FMControls:FMLabel id="TrainingType" AssociatedControlID="TrainingTypeDropdownlist" style="Z-INDEX: 102; LEFT: 50px; POSITION: absolute; TOP: 70px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Training Type:</FMControls:FMLabel>
		<asp:dropdownlist id="TrainingTypeDropdownlist" style="Z-INDEX: 106; LEFT: 150px; POSITION: absolute; TOP: 70px"
			runat="server" CssClass="formfield" Width="170px" AutoPostBack="true" 
			 tabIndex="2" onselectedindexchanged="OnTrainingTypeSelectedIndexChanged" ></asp:dropdownlist>
		<FMControls:FMLabel id="FMLabel2" AssociatedControlID="HullNoTextBox" style="Z-INDEX: 102; LEFT: 50px; POSITION: absolute; TOP: 100px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Number:</FMControls:FMLabel>
		<asp:textbox id="HullNoTextBox" style="Z-INDEX: 103; LEFT: 150px; POSITION: absolute; TOP: 100px;"
			runat="server" CssClass="formfield" BackColor="White" Width="170px" Enabled="true" tabIndex="3" MaxLength="50"></asp:textbox>	
			 	
		<FMCONTROLS:FMBUTTON id="CalculateDates" 
			 style="Z-INDEX: 103; LEFT: 150px; POSITION: absolute; TOP: 125px" tabIndex="4" 
			 runat="server" CssClass="formfieldtitle"
			 Width="100px" Text="Calculate Dates" CommandName="Apply" 
			 onclick="OnCalculateDatesClick"></FMCONTROLS:FMBUTTON>

            <FMControls:FMLabel ID="FMLabel3" Style="z-index: 102; left: 50px; position: absolute; top: 165px" runat="server"
                CssClass="formfieldtitle" BackColor="Transparent">Completion Date:</FMControls:FMLabel>
            <FMControls:FMDate ID="CompletionDate" Style="z-index: 201; left: 150px; position: absolute; top: 165px"
                TabIndex="5" runat="server" Width="160px" CssClass="formfield" Height="25px"></FMControls:FMDate>

            <FMControls:FMLabel ID="FMLabel4" Style="z-index: 102; left: 50px; position: absolute; top: 195px" runat="server"
                CssClass="formfieldtitle" BackColor="Transparent">Due Date:</FMControls:FMLabel>
            <FMControls:FMDate ID="DueDate" Style="z-index: 201; left: 150px; position: absolute; top: 195px"
                TabIndex="6" runat="server" Width="160px" CssClass="formfield" Height="25px"></FMControls:FMDate>

            <FMControls:FMLabel ID="FMLabel5" Style="z-index: 102; left: 50px; position: absolute; top: 225px" runat="server"
                CssClass="formfieldtitle" BackColor="Transparent">Expiration Date:</FMControls:FMLabel>
            <FMControls:FMDate ID="ExpirationDate" Style="z-index: 201; left: 150px; position: absolute; top: 225px"
                TabIndex="7" runat="server" Width="160px" CssClass="formfield" Height="25px"></FMControls:FMDate>
			
		<FMControls:FMLabel id="FMLabel6" AssociatedControlID="InstructorTextbox" style="Z-INDEX: 102; LEFT: 50px; POSITION: absolute; TOP: 255px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Instructor:</FMControls:FMLabel>
		<asp:textbox id="InstructorTextbox" style="Z-INDEX: 103; LEFT: 150px; POSITION: absolute; TOP: 255px"
			runat="server" CssClass="formfield" BackColor="White" Width="170px" Enabled="true" 
			 tabIndex="8" MaxLength="50"></asp:textbox>		
		<FMControls:FMLabel id="FMLabel7" AssociatedControlID="RatingTextbox" style="Z-INDEX: 102; LEFT: 50px; POSITION: absolute; TOP: 285px" runat="server"
			CssClass="formfieldtitle" BackColor="Transparent">Rating:</FMControls:FMLabel>
		<asp:textbox id="RatingTextbox" style="Z-INDEX: 103; LEFT: 150px; POSITION: absolute; TOP: 285px"
			runat="server" CssClass="formfield" BackColor="White" Width="170px" Enabled="true" 
			 tabIndex="9" MaxLength="20"></asp:textbox>		

			<table cellpadding="1" cellspacing="1" border="0" style="Z-INDEX: 102; LEFT: 350px; POSITION: absolute; TOP: 40px">
				<tr>
					<td>
						<FMControls:FMLabel runat="server" ID="PersonsAssigned" AssociatedControlID="lbxAssigned" Text="Assigned Personnel" CssClass="formfieldtitle" />
					</td>
					<td>&nbsp;</td>
					<td>
						<FMControls:FMLabel runat="server" ID="PersonsAvailable" AssociatedControlID="lbxAvailable" Text="Available Personnel" CssClass="formfieldtitle" />
					</td>
				</tr>
				<tr>
					<td>
						<asp:ListBox Runat="server" ID="lbxAssigned" CssClass="formfield" Width="200px" Height="250px"
							SelectionMode="Multiple" TabIndex="10"></asp:ListBox>
					</td>
					<td align="center" valign="middle">
						<asp:Button Runat="server" ID="btnAssign" Text="<<" CssClass="formfield" 
							onclick="BtnAssignClick" Width="20px" TabIndex="12"></asp:Button>
						<div style ="height:10px;"></div>
						<asp:Button Runat="server" ID="btnUnassign" Text=">>" CssClass="formfield" 
							onclick="BtnUnassignClick" Width="20px" TabIndex="13"></asp:Button>
					</td>
					<td>
						<asp:ListBox Runat="server" ID="lbxAvailable" CssClass="formfield" 
							Width="200px" Height="250px"
							SelectionMode="Multiple" TabIndex="11"></asp:ListBox>
					</td>
				</tr>
            </table>
            <FMControls:FMButton ID="Apply"
                Style="z-index: 103; left: 350px; position: absolute; top: 350px" TabIndex="14"
                runat="server" CssClass="formfieldtitle"
                Width="100px" Text="Apply" CommandName="Apply" OnClick="OnButtonApplyClick"></FMControls:FMButton>

        </div>
    </form>
</body>
</html>
