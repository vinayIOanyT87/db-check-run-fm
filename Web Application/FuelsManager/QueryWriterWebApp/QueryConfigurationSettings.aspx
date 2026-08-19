<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryConfigurationSettings.aspx.cs" Inherits="FuelsManager.QueryWriterWebApp.QueryConfigurationSettings" %>

<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title />
    <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>
</head>
<body>
    <form id="form1" runat="server">
        <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent" style="position:absolute">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
		<FMControls:FMPageFadeImage runat="server"></FMControls:FMPageFadeImage>
			
        <table style="z-index:110; left:32px; top: 10px; width:575px; position:absolute" cellpadding="5" role="presentation" aria-label="layout">
		    <tr>
		        <td>
                    <FMControls:FMLabel id="TitleLabel" runat="server" CssClass="headline" Text="Query Settings" style="left:-24px; position:relative" />
		        </td>
		    </tr>
		    <tr>
		        <td>
		            <FMControls:FMLabel id="HeaderLabel" AssociatedControlID="HeaderTextBox" runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text="Header Line for All Queries" /><br />
		            <input type="text" id="HeaderTextBox" runat="server" class="formfield" style="width:500px" maxlength="100"/>
		        </td>
		    </tr>
		    <tr>
		        <td>
		            <FMControls:FMLabel id="FooterLabel" AssociatedControlID="FooterTextBox" runat="server" CssClass="formfieldtitle" BackColor="Transparent" Text="Footer Line for All Queries" /><br />
		            <input type="text" id="FooterTextBox" runat="server" class="formfield" style="width:500px" maxlength="100" />
		        </td>
		    </tr>
            <tr>
                <td>
                    <hr style="width:100%; color:Black; size:1pt"/>
                </td>
            </tr>
        </table>
        
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" >
            <ContentTemplate>
                <table style="z-index:110; left:50px; top: 180px; width:375px; height:200px; position:absolute" cellpadding="3" role="presentation" aria-label="layout">
                    <tr>
                        <td colspan="4" style="left:-18px;position:relative">
                            <FMControls:FMLabel ID="QueryTypeLabel" AssociatedControlID="QueryTypeDropDown" runat="server" CssClass="formfieldtitle" Text="Default Field Configuration for Query Type:" /><br />
                            <FMControls:FMDropDownList ID="QueryTypeDropDown" runat="server" style="width:200px" CssClass="formfield" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top" align="right">
                            <br />
                            <FMControls:FMUpLinkButton ID="MoveUpButton" runat="server"/>
                        </td>
                        <td rowspan="2" valign="top">
                            <FMControls:FMLabel ID="SelectedFieldsLabel" AssociatedControlID="SelectedFieldsList" runat="server" CssClass="formfieldtitle" Text="Selected Fields" /><br />
                            <FMControls:FMListBox ID="SelectedFieldsList" runat="server" CssClass="formfield" Width="185px" Height="175px" SelectionMode="Multiple" Sort="false" />
                        </td>
                        <td valign="middle">
                            <FMControls:FMButton ID="AssignButton" runat="server" CssClass="formfieldtitle" Text="<< Assign" Width="80px" />
                        </td>
                        <td rowspan="2" valign="top">
                            <FMControls:FMLabel ID="AvailableFieldsLabel" AssociatedControlID="AvailableFieldsList" runat="server" CssClass="formfieldtitle" Text="Available Fields" /><br />
                            <FMControls:FMListBox ID="AvailableFieldsList" runat="server" CssClass="formfield" Width="185px" Height="175px" SelectionMode="Multiple" />
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
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <table style="z-index:110; left:32px; top: 435px; width:575px; position:absolute" cellpadding="5" role="presentation" aria-label="layout">
            <tr>
                <td>
                    <hr style="width:100%; color:Black; size:1pt"/>
                </td>
            </tr>
            <tr>
                <td>
                    <FMControls:FMButton id="ApplyButton" runat="server" Text="Apply" CssClass="formfieldtitle" />
                </td>
            </tr>
        </table>
        
    </div>
</form>
</body>
</html>
