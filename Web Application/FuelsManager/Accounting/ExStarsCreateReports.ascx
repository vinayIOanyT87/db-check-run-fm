<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExStarsCreateReports.ascx.cs" Inherits="FuelsManager.Accounting.ExStarsCreateReports" %>
<%@ Register TagPrefix="Accounting" TagName="ExStarsPopupWarnOfErrors" Src="ExStarsPopupWarnOfErrors.ascx" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<HTML>
    <head>
        <title>Create ExSTARS Reports</title>
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
    </HEAD>
	<body>
        <FMControls:FMTextBox runat="server" ID="tbDescription" BorderStyle="None" ReadOnly="True" Rows="3"  CssClass="formfield" Wrap="True" MaxLength="10000" style="Z-INDEX: 101; background-color: transparent; overflow:auto; font-weight: bold; LEFT: 1px; POSITION: absolute; TOP: 3px; width: 900px; bottom: 711px; height: 45px;" TextMode="MultiLine" />
	    	
        <FMControls:FMLabel runat="server" Text="Manager" CssClass="formfield" style="Z-INDEX: 101; LEFT: 9px; POSITION: absolute; TOP: 69px; width: 102px; margin-bottom: 0px; "></FMControls:FMLabel>
        <FMControls:FMLabel runat="server" Text="File Mode" CssClass="formfield" style="Z-INDEX: 104; LEFT: 9px; POSITION: absolute; top:146px; width: 95px; bottom: 631px; height: 17px;"></FMControls:FMLabel>
        <FMControls:FMLabel runat="server" Text="Report Type" CssClass="formfield" style="Z-INDEX: 112; top: 106px; LEFT: 9px; POSITION: absolute; width: 95px; bottom: 664px; height: 21px;"></FMControls:FMLabel>
    
        <FMControls:FMDropDownList runat="server" ID="ddManager" CssClass="formfield" style="Z-INDEX: 121; LEFT: 99px; POSITION: absolute; TOP: 68px; width: 172px; bottom: 422px; height: 20px;"  TabIndex="1"/>
        <FMControls:FMDropDownList runat="server" ID="ddReportType" CssClass="formfield" AutoPostBack="True" onselectedindexchanged="ddReportType_SelectedIndexChanged" style="Z-INDEX: 124; LEFT: 99px; POSITION: absolute; TOP: 106px; width: 172px; bottom: 384px; height: 20px;" TabIndex="2" Sort="False"/>
	    <FMControls:FMDropDownList runat="server" ID="ddModifier" CssClass="formfield" style="Z-INDEX: 126; POSITION: absolute; TOP: 144px; width: 172px; bottom: 375px; left: 99px; height: 20px;" TabIndex="3" Sort="False"/>
    
        <FMControls:FMLabel runat="server" Text="Reporting Date" CssClass="formfield" style="Z-INDEX: 131; LEFT: 9px; POSITION: absolute; TOP: 189px; width: 103px; bottom: 604px;"></FMControls:FMLabel>
        <asp:Panel ID="panTurnOver" style="Z-INDEX: 142; LEFT: 4px; POSITION: absolute; TOP: 222px; width: 266px; height: 24px;" CssClass="formfield" runat="server">
            <FMControls:FMLabel runat="server" Text="Turnover Date" ID="lblTurnOverDate" CssClass="formfield" style="Z-INDEX: 152; LEFT: 3px; POSITION: absolute; TOP: 3px; width: 104px; bottom: 504px;"/>                                    
            <FMControls:FMTextBox runat="server" ID="tbTurnOverMonth" ReadOnly="True"  style="Z-INDEX: 152; LEFT: 96px; POSITION: absolute; TOP: 1px; width: 64px; height: 17px;" CssClass="formfield" />
            <FMControls:FMLabel runat="server"  Text="/" CssClass="formfield" style="font-size:large; Z-INDEX: 152; LEFT: 170px; POSITION: absolute; TOP: 2px; width: 5px; bottom: 2px; right: 99px;"/>                                    
            <FMControls:FMTextBox runat="server" ID="tbTurnOverDay"  OnTextChanged="SelectedDateChanged" style="Z-INDEX: 152; LEFT: 185px; POSITION: absolute; TOP: 1px; width: 19px; height: 17px;" CssClass="formfield"  TabIndex="6"/>
            <FMControls:FMLabel runat="server"   Text="/" CssClass="formfield" style="font-size:large; Z-INDEX: 152; LEFT: 213px; POSITION: absolute; TOP: 2px; width: 5px; bottom: 22px;"/>                                    
            <FMControls:FMTextBox runat="server" ID="tbTurnOverYear" ReadOnly="True"  style="Z-INDEX: 152; LEFT: 225px; POSITION: absolute; TOP: 1px; width: 35px; height: 17px;" CssClass="formfield" />

        </asp:Panel>

        <FMControls:FMDropDownList ID="MonthDropDownLst" runat="server" AutoPostBack="True" OnTextChanged="SelectedDateChanged" style="Z-INDEX: 145; LEFT: 99px; POSITION: absolute; TOP: 185px; width: 99px; height: 20px;" CssClass="formfield" tabIndex="7" Sort="False"/>
	    <FMControls:FMDropDownList ID="YearDropDownList" runat="server" AutoPostBack="True" onselectedindexchanged="SelectedDateChanged"  style="Z-INDEX: 146; POSITION: absolute; TOP: 185px; LEFT: 217px; width: 54px; height: 20px;"
	                                   CssClass="formfield" tabIndex="8" Sort="False"/>
        
        <FMControls:FMCheckBox runat="server" Text="Create Test File" ID="chkTest" Checked="False" CssClass="formfield" style="Z-INDEX: 120; POSITION: absolute; TOP: 267px; LEFT: 9px; width: 163px; bottom: 465px; height: 19px;" TabIndex="9"/>    
        <FMControls:FMButton runat="server" ID="btnCreateReport" OnClick="btnCreateReportStdMonthly_Click" Text="Create Report" style="Z-INDEX: 151; LEFT: 9px; POSITION: absolute;  TOP: 315px; width: 256px; bottom: 146px; right: 1493px;" height="29px" CssClass="formfield"  TabIndex="10" />
        <FMControls:FMButton runat="server" ID="btnDownLoadEDI" Text="Download Raw Report to PC" style="Z-INDEX: 152; LEFT: 9px; POSITION: absolute; top:370px; width: 256px; bottom: 91px;" height="29" CssClass="formfield"  TabIndex="11" OnClick="btnDownLoadEDI_Click"/>                        
        <FMControls:FMButton runat="server" ID="btnDownLoadEasyRead" Text="Download Easy-Read to PC" style="Z-INDEX: 154; LEFT: 9px; POSITION: absolute; top:422px;  width: 256px; bottom: 48px;" height="29px" CssClass="formfield"  TabIndex="12" OnClick="btnDownLoadEasyRead_Click"/>                        
        
        <FMControls:FMLabel runat="server" Text="Errors and Warnings" CssClass="formfield" style="Z-INDEX: 161; LEFT: 328px; POSITION: absolute; TOP: 55px; width: 141px; bottom: 312px;"></FMControls:FMLabel>
    
    
	    <FMControls:FMTextBox runat="server" ID="tbErrorsAndWarnings" ValidateRequestMode="Disabled"  MaxLength="10000" ReadOnly="False" CssClass="formfield" TextMode="MultiLine" Wrap="True" style="Z-INDEX: 171; TOP: 83px;  LEFT: 326px; width: 1077px; bottom: 0px;  POSITION: absolute;" Rows="30"/>
	    <asp:Panel ID="Panel1" Visible="True" BorderStyle="Solid"  runat="server" Style=" background-color:#EEEEEE; Z-INDEX: 600; TOP: 48px;  LEFT: 118px; width: 1077px; bottom: 405px;  POSITION: absolute; width:999px; height:300px">
	      <table>
	        <tr >
	          <td colspan="2">
	              <FMControls:FMLabel runat="server" CssClass="popupHeader" style="Z-INDEX: 602; position:absolute; vertical-align: middle;  top: 1px; left: 1px; height: 29px; width: 996px;">Confirm Selection</FMControls:FMLabel>
             </td>
           </tr>
	       <tr >
             <td colspan="2">
                   <FMControls:FMTextBox ID="tbReportTypeWarning" ValidateRequestMode="Disabled" CssClass="popupText" style=" Z-INDEX: 602; position:absolute; overflow:auto; top: 31px; left: 25px; height: 140px; width: 939px;" Rows="3" Wrap="True" TextMode="MultiLine" runat="server" />            
             </td>
           </tr>
	       <tr >
  	           <td>
	             <FMControls:FMButton runat="server" ID="btnInOutOk" OnClick="btnCreateReportInOutMgr_Click" Text="Yes" style="Z-INDEX: 602; LEFT: 521px; POSITION: absolute;  TOP: 215px; width: 138px; bottom: 54px; right: 340px; height: 31px;"  TabIndex="10" />
               </td>
               <td>
                 <FMControls:FMButton runat="server" ID="btnInOutProhbited" OnClick="btnInOutProhbited_Click" Text="NO" style="Z-INDEX: 602; LEFT: 250px; POSITION: absolute;  TOP: 218px; width: 169px; bottom: 53px; right: 580px;" height="29px" CssClass="formfield"  TabIndex="10"/>
               </td>
           </tr>
        </table>
	  </asp:Panel>

        <Accounting:ExStarsPopupWarnOfErrors ID="warningpopup" Visible="False" style="Z-INDEX: 500; POSITION: absolute; vertical-align: middle;  top: 400px; left: 200px; background-color: white;" runat="server"/>

          
    </body>
</HTML>