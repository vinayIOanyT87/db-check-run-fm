<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomizeEmailMessageForm.aspx.cs" Inherits="FuelsManager.FMWebApp.CustomizeEmailMessageForm" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ OutputCache Location="None" VaryByParam="None" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <title></title>
    <base target="_self">
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

</head>
<body>
    <link href="<%=HttpRuntime.AppDomainAppVirtualPath%>/CSS/FuelsManager.css" media="screen" rel="stylesheet" type="text/css" />
	<link href="<%=HttpRuntime.AppDomainAppVirtualPath%>/css/CFS.css"  media="screen" rel="stylesheet" type="text/css" />

    <script src="<%=HttpRuntime.AppDomainAppVirtualPath%>/javascripts/CFS.js" type="text/javascript"   defer="defer"></script>
    <script src="<%=HttpRuntime.AppDomainAppVirtualPath%>/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" type="text/javascript" language="javascript" ></script>
    <script src="<%=HttpRuntime.AppDomainAppVirtualPath%>/Javascripts/modalpopup.js" type="text/javascript" language="javascript" ></script>
    <script src="<%=HttpRuntime.AppDomainAppVirtualPath%>/Javascripts/customizeemailmessageform.js" type="text/javascript" language="javascript" ></script>

    <form id="form1" method="post" runat="server">
         <asp:Image ID="Image1" style="z-index: 200; left: 0px; position: absolute; top: 0px" runat="server"
            ImageUrl="<%$ AppSettings: PageFadeImage %>" BackColor="Transparent"></asp:Image>
        <div style="position: absolute; top: 10px; left:8px">
        <div style="z-index: 201; display:flex; flex-direction:column; ">
           <div>				
               <FMControls:FMLabel id="TitleLabel" style="z-index: 201; position:relative" runat="server"
					BackColor="Transparent" CssClass="headline"><%=alarmAndEventID %></FMControls:FMLabel>
            </div>
            <div style="height:20px"></div>
            <div>
                 <FMControls:FMLabel id="SubjectLabel" style="z-index: 201; position:relative" runat="server"
					BackColor="Transparent" CssClass="formfield">Subject: </FMControls:FMLabel>              
                <FMControls:FMTextBox id="SubjectTextBox"  runat="server" MaxLenght="1024"  TabIndex="1" Width="500px" Columns="40" style="z-index: 201; position:relative"></FMControls:FMTextBox>
            </div>
            <div style="height:10px"></div>
             <div>
                <FMControls:FMTextBox id="BodyTextBox"  TextMode="MultiLine"  runat="server" MaxLenght="4096" TabIndex="2" Rows="15" Columns="80"  style="z-index: 201; position:relative"></FMControls:FMTextBox>
            </div>
             <div style="height:20px"></div>
           <div style="z-index: 201; display:flex; flex-direction:row; position:relative">
                <FMControls:FMButton ID="SaveBtn" Style="z-index: 201; "
                    TabIndex="3" runat="server" Width="64px" CssClass="formfieldtitle" Text="Save" OnClientClick="Ok();"></FMControls:FMButton>
             <div style="width:20px"></div>
               <FMControls:FMButton ID="CancelBtn" Style="z-index: 201; position:relative" TabIndex="4"
                    runat="server" Width="64px" CssClass="formfieldtitle" Text="Cancel" OnClientClick="Cancel();"></FMControls:FMButton>
            </div>

        </div>

        </div>
    </form>
</body>
</html>
