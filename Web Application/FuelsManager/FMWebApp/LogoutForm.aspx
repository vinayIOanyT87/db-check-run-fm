<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogoutForm.aspx.cs" Inherits="FuelsManager.FMWebApp.LogoutForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
     <link href="<%= HttpRuntime.AppDomainAppVirtualPath + "/css/CFS.css" %>" media="screen" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        function checkForCrossFrameScripting() {
            if (window.cfsChecked) {
                return;
            }
            var cfsDetected = true;
            //The following STYLE and script are for preventing Cross-Frame Scripting attack on browsers not supporting x-frame-options:sameorigin header
            let selfLocation = self.location.host.toUpperCase();
            try {
                let topLocation = top.location.host.toUpperCase();
                if (topLocation === selfLocation) {
                    let p1 = top.location.pathname.split("/", 5);
                    let p2 = self.location.pathname.split("/", 5);
                    if (p1.length > 1 && p2.length > 1 && p1[1] === p2[1]) {
                        window.cfsChecked = true;
                        cfsDetected = false;
                    }
                }
            }
            catch (e) { ; }

            if (cfsDetected) {
                try {
                    top.location = self.location;
                }
                catch (e) {
                    ;
                }
			  }
			  else {
				  document.body.style.display = 'block';
			  }
       }

       window.addEventListener("load", function (e) {
            checkForCrossFrameScripting();
        });

	 </script>
    <style type="text/css">
.center {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 200px;
  color:darkblue;
}
    </style>
    <script type="text/javascript">
        if (<%=SessionTimedOut%>) {
            alert("Session timed out");
        }
        if (<%=InvalidSession%>) {
            alert("Invalid Session");
        }
        if (<%= commercialReturnToLoginStr%>) {
            window.location = "../";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<p class="center"><span>You have successfully logged out.</span><span>You may close your browser.</span></p>
    </div>
    </form>
</body>
</html>
