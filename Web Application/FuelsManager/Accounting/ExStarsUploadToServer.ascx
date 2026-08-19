<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExStarsUploadToServer.ascx.cs" Inherits="FuelsManager.Accounting.ExStarsUploadToServer" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
    <head>
        <title>Create ExSTARS Upload To Server</title>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body >
	    <table>
	        <tr>
	            <td>
                   <FMControls:FMLabel runat="server" Text="Manager" CssClass="formfield" style="Z-INDEX: 101; LEFT: 1px; POSITION: absolute; TOP: 10px; width: 68px; margin-bottom: 0px; " ID="FMLabel1"></FMControls:FMLabel>
	            </td>
	            <td>
	               <FMControls:FMDropDownList runat="server" ID="ddManager" CssClass="formfield" style="Z-INDEX: 121; LEFT: 65px; POSITION: absolute; TOP: 10px; width: 180px; bottom: 330px; right: 839px; height: 20px;"  TabIndex="1"/>	    	
	            </td>
	            <td>
	                <FMControls:FMLabel runat="server" Text="Upload File To Server" style="Z-INDEX: 161; POSITION: absolute; TOP: 10px; left: 270px; width: 171px; bottom: 756px;" CssClass="formfield"/>        
	            </td>
	        </tr>
            <tr>
                <td colspan="2"></td>
                <td rowspan="2">
                    <asp:RadioButtonList ID="RadioReportType" runat="server" 
                         style="Z-INDEX: 13; LEFT: 270px; POSITION: absolute; TOP: 28px; right: 491px; width: 240px; height: 43px;"
                         CssClass="formfield" TextAlign="Right">   
                        <asp:ListItem Value="Acknowledgement" Selected="True">151 Acknowledgement</asp:ListItem>                      
                    </asp:RadioButtonList>        
                </td>
            </tr>
            <tr>
                <td colspan="3"></td>
                <td>
                    <asp:FileUpload ID="FileUpload1" runat="server" style="Z-INDEX: 161; POSITION: absolute; TOP: 79px; left: 5px; width: 1081px; bottom: 720px;" height="21px" />	   
                </td>
                <td>
                    <FMControls:FMButton runat="server" ID="btnUpLoad" Text="Upload" style="Z-INDEX: 161; LEFT: 1105px; POSITION: absolute; TOP: 79px; width: 93px; bottom: 716px;" OnClick="btnUpLoad_Click" Height="21"/>                    
                </td>
            </tr>
	    </table>
	    <FMControls:FMLabel runat="server" Text="Errors and Warnings" CssClass="formfield" style="Z-INDEX: 161; LEFT: 4px; POSITION: absolute; TOP: 180px; width: 141px; bottom: 158px;"/>
	    <FMControls:FMTextBox runat="server" ID="tbErrorsAndWarnings" ValidateRequestMode="Disabled"  ReadOnly="True" CssClass="formfield" TextMode="MultiLine" style="Z-INDEX: 171; TOP: 124px;  LEFT: 1px; width: 1390px; bottom: 326px;  POSITION: absolute;" Rows="25"/>                           	    
    </body> 
</HTML>
