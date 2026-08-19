<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnhandledException.aspx.cs" Inherits="FuelsManager.UnhandledException" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        .center {
          display: flex;
          justify-content: center;
          align-items: center;
          height: 200px;
          color:darkblue;
    }
    </style>  
</head>
<body>
    <form id="form1" runat="server">
        <div>
              <p class="center">Unable to process the request. Please see the Event Logs for details.</p>
        </div>
    </form>
</body>
</html>
