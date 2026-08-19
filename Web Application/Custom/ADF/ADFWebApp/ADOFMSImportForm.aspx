<%@ Register TagPrefix="fmcontrols" Namespace="FMControls" Assembly="FMControls" %>
<%@ Page language="c#" Codebehind="ADOFMSImportForm.aspx.cs" AutoEventWireup="false" Inherits="ADFWebApp.ADOFMSImportForm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>ADOFMSImportForm</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="../FuelsManager.css" rel="stylesheet" />
		<script language="javascript">
		
		function disableButtons()
		{
		    var uploadCtrl = document.getElementById("FileUpload");
		    var importCtrl = document.getElementById("ImportButton");
		    		    
		    if (uploadCtrl != null && importCtrl != null)
		    {
		        uploadCtrl.disabled = true;
		        importCtrl.disabled = true;
		    }
		    
		    Form1.submit();
		}
		
		</script>
	</head>
	<body MS_POSITIONING="GridLayout">
	    <FMCONTROLS:FMLABEL id="TitleLabel" runat="server" BackColor="Transparent" CssClass="headline" Width="456px">ADOFMS Import Facility</FMCONTROLS:FMLABEL>
		<form id="Form1" method="post" enctype="multipart/form-data" runat="server">
		
			<asp:image id="FadeImage" style="Z-INDEX: -1; LEFT: 0px; POSITION: absolute; TOP: 0px" runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:image>
			<table>
			<tr>
			    <td colspan="2"><FMCONTROLS:FMLABEL id="FindFileLabel" runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="112px">Enter or select file</FMCONTROLS:FMLABEL><br /></td>
			</tr>
			<tr>
              <td><asp:FileUpload  runat="server" id="FileUpload" size="60" name="FileUpload" /></td>
			  <td><FMCONTROLS:FMBUTTON id="ImportButton" runat="server" CssClass="formfieldtitle" 
                      Width="72px" Text="Import"></FMCONTROLS:FMBUTTON><br /></td>
			</tr>
			<tr>
			  <td colspan="2"><FMCONTROLS:FMLABEL id="FMLABEL1" runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="112px">Error messages:</FMCONTROLS:FMLABEL><br />
			  <asp:ScriptManager runat="server" id="ScriptManager1" AsyncPostBackTimeout="6000" ></asp:ScriptManager>
			  <asp:UpdatePanel ID="updatePanel_error" runat="server">
			  <ContentTemplate>
			    <asp:textbox id="ResultsTextBox_Error" runat="server" CssClass="formfield" Width="552px" Height="120px" TextMode="MultiLine" ReadOnly="True"></asp:textbox><br />
			  </ContentTemplate>
			  </asp:UpdatePanel>
			  </td>
			</tr>
			<tr>
			  <td colspan="2"><FMCONTROLS:FMLABEL id="FMLABEL2" runat="server" BackColor="Transparent" CssClass="formfieldtitle" Width="112px">Progress:</FMCONTROLS:FMLABEL><br />
			  <asp:UpdatePanel ID="updatePanel_progress" runat="server">
			  <ContentTemplate>
			    <asp:textbox id="ResultsTextBox_Progress" runat="server" CssClass="formfield" Width="552px" Height="120px" TextMode="MultiLine" ReadOnly="True"></asp:textbox><br /></td>
			  </ContentTemplate>
			  </asp:UpdatePanel>
			</tr>
			</table>
         <p>
               &nbsp;
            </p>
        </form>
	</body>
</html>