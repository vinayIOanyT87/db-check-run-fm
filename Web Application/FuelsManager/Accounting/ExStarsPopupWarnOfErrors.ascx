<%@ Control  Language="C#" AutoEventWireup="true" CodeBehind="ExStarsPopupWarnOfErrors.ascx.cs" Inherits="FuelsManager.Accounting.ExStarsPopupWarnOfErrors" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
    <head>
        <title>ExSTARS Warning</title>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
	</HEAD>
	<body >
	   <asp:Panel ID="Panel1" Visible="True" BorderStyle="Solid"  runat="server" Style=" background-color:#EEEEEE; Z-INDEX: 600; TOP: 33px;  LEFT: 63px; width: 1077px; bottom: 428px;  POSITION: absolute; width:999px; height:300px">
	      <table>
	        <tr >
	          <td colspan="2">
	              <FMControls:FMLabel runat="server" CssClass="popupHeader" style="Z-INDEX: 602; position:absolute; vertical-align: middle;  top: 1px; left: 1px; height: 29px; width: 996px;">Error Found When Creating Report</FMControls:FMLabel>
             </td>
           </tr>
	       <tr >
             <td colspan="2">
                <asp:TextBox ID="TextBox1" runat="server" BorderStyle="None" CssClass="popupText" TextMode="MultiLine" ValidateRequestMode="Disabled" Rows="5" 
                    style="Z-INDEX: 505; POSITION: absolute; overflow:auto;  top: 63px; left: 25px; height: 143px; width: 956px;"
                     Font-Size="16" Wrap="True" ReadOnly="True" MaxLength="10000">
	           </asp:TextBox>
             </td>
           </tr>
	       <tr >
  	         <td>
               <FMControls:FMButton runat="server" ID="Button1" Text="OK" style="Z-INDEX: 502; POSITION: absolute; top: 243px; left: 383px; height: 41px; width: 202px;" OnClick="Button1_Click" />
             </td>
           </tr>
        </table>
	  </asp:Panel>        
    
    </body> 
</HTML>

