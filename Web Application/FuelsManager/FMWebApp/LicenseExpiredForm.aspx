<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LicenseExpiredForm.aspx.cs" Inherits="FuelsManager.FMWebApp.LicenseExpiredForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<style>
		.licenseInfoLink {
			text-decoration: underline;
		}
		.licenseInfoLink:link {
			color: mediumblue
		}	
		.licenseInfoLink:active {
			color: mediumblue
		}		
		.licenseInfoLink:visited {
			color: mediumblue
		}
		a.licenseInfoLink:hover {
			color: orangered;
		}

		.parent {
			display: grid;
			  grid-template-columns:1fr 1fr;
			  grid-gap:20px;
					  height: 150px;
		}
		.child {
		 /* display: inline-block;*/
		  border: 1px solid black;
		/*  padding: 1rem 1rem;*/
		  vertical-align: middle;
		  border-radius: 10px;
/*		  height: 150px;
		  width: 400px;*/
		  text-align: center;
		}
		.child-left{
			margin-right:10px;
		}
		.child-right{
			margin-left: 10px;
		}
		.thank-you-box {
			width: 380px;
			height: 65px;
			background-color: #f0f0f0;
			font-family: arial;
			font-size: 12px;
			text-align: center;
			padding: 20px;
			margin: 20px;
			border-radius: 10px;
			margin-right: 10px; /* Add some spacing between the boxes */
			margin-left: 0px;
			float: left; /* Float the boxes side by side */
			box-shadow: 5px 5px 10px rgba(0, 0, 0, 0.3);
		}
	</style>
	<link type="text/css" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet"/>

</head>
<body>
	<form id="form1" runat="server">
		   <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
		   <div id="pageContent" style="position:absolute">
			<asp:Image id="Image1" alt="<%$ AppSettings: PageFadeImageAlt %>" style="Z-INDEX: 101; LEFT: 0px; POSITION: absolute; TOP: 0px"
				runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
			<asp:Label id="TitleLabel" style="Z-INDEX: 103; LEFT: 8px; POSITION: absolute; TOP: 8px" runat="server"
				CssClass="headline" Width="136px" BackColor="Transparent" enableviewstate="false"><%=AppName%> License Has Expired</asp:Label>
			   
			<div class="formfieldtitle" style="position:relative; margin-top:50px; margin-left:80px">

				<p  style='margin-bottom:0in;line-height:normal'><span>&nbsp</span></p>

				<p  style='margin-bottom:0in;line-height:normal'>
					<span>We hope you have been enjoying your experience with <span><%=AppName%></span>.</span>
				</p>

				

				<p>
					<span>Unfortunately, your license for <%=AppName%> has expired as of <%=this.licenseStatusText%>.</span>
				</p>

				<p  style='margin-bottom:0in;line-height:normal'>
					<span>To regain access to all the features and benefits you have come to rely on, please renew your subscription by contacting Varec Sales.</span>
				</p>	
				
				
				

				<p  style='margin-bottom:0in;line-height:normal'>
					<span>Thank you for being a valued customer of <span><%=AppName%></span>!</span>

				</p>

				
				<p  style='margin-bottom:0in;line-height:normal'><span>&nbsp</span></p>
				<p  style='margin-bottom:0in;line-height:normal'><span>&nbsp</span></p>

			</div>
			<div class="formfieldtitle" style="position:relative; margin-top:0px; margin-left:10px">
				<div class="parent">
					<div class="thank-you-box">
						To renew your subscription simply contact Varec Sales:<br />
						Email  <a href="mailto:sales@varec.com" target="_blank">Sales@Varec.com</a> <br />
						Web  <a href="https://www.varec.com/contact/sales-support" target="_blank">www.varec.com/contact/sales-support</a><br />
						Phone  +1 770-447-9202 (US) or +1 866-698-2732 (internationally)<br />
					</div>
						<div class="thank-you-box">
						If you have questions or need assistance, contact Varec Support: <br />
								Email  <a href="mailto:support@varec.com" target="_blank">Support@Varec.com</a> <br />
						Web  <a href="https://www.varec.com/contact/technical-support" target="_blank">www.varec.com/contact/technical-support</a><br />
						Phone  +1 770-446-0818 (US) or +1 800-999-6708 (internationally)<br />
					</div>
					<div style="clear: both;"></div> <!-- Clear the float after the boxes -->
				</div>

			</div>
		</div>

	</form>
		<script type="text/javascript">
			addEventListener("DOMContentLoaded", (event) => { 
			let controls = document.querySelectorAll("li a");
			for (var c of controls) {
				if (c.id != "FMM_menuLogout"
					&& c.id != "FMM_About"
					&& c.id != "FMM_Help"
				) {
					c.href = "#";
					c.onclick = "";
					c.style.opacity = "0.5";
				}
				//alert(c);
			}
			//let siteSelect = document.getElementById("mhbSiteDropDown");
			//if (siteSelect) {
			//	siteSelect.style.opacity = "0.5";
   //             siteSelect.disabled = true;
			//	siteSelect = document.getElementById("SiteSelect");
			//	if (siteSelect) {
   //                 alert(siteSelect.options.length);
   //                 let i, L = siteSelect.options.length - 1;
   //                 for (i = L; i >= 0; i--) {
   //                     siteSelect.remove(i);
   //                 }
			//	}

			//	}
			});
		</script>
</body>
</html>
