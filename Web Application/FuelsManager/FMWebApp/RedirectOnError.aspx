<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RedirectOnError.aspx.cs" Inherits="FuelsManager.WebForm1" %>

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
	<script type="text/javascript" defer="defer">

        window.addEventListener("load", function (e) {
            transferPage();
        });

        function transferPage() {
            alert('Request failed. Please see the Event Logs for details.');
            console.log("RedirectOnError.aspx transferToPage=" + '<%= transferToPage %>');
                top.location = '<%= transferToPage %>';
        }

    </script>
        
</head>
<body>
    <form id="form1" runat="server">
        <div class="center">
            <p>Request failed. Please see the Event Logs for details.</p>
        </div>
    </form>
</body>
</html>
