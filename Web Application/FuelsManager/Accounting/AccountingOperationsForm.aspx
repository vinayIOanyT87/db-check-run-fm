<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly = "FMControls" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountingOperationsForm.aspx.cs" Inherits="FuelsManager.Accounting.AccountingOperationsForm" %>
<%@ Register src="..\MenuBar\FMMenuBar.ascx" tagname="FMMenuBar" tagprefix="FMMenuBar" %>
<!DOCTYPE html>
<HTML>
<head>
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet">
        <link rel="stylesheet" href="<%= HttpRuntime.AppDomainAppVirtualPath + "/DispatchWebApp/css/jquery-ui-1.8.17.custom.css" %>" type="text/css" />
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js" %>"></script>
	    <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/modalpopup.js" %>"></script>
        <script type="text/javascript" src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Javascripts/CSRFToken.js" %>"></script>
		<script type="text/javascript">
			function CompanySelect(role, companyTextBoxId)
			{
			    var companyTextBox = document.getElementById(companyTextBoxId);

			    showModalDialogFrame({
			        url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "",
			        width: 855,
			        height: 560,
			        onClose: function ()
			        {
			            if (this.returnValue != null)
			            {
			                var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
			                var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

			                companyTextBox.value = asciiValue1;
			                companyTextBox.title = asciiValue2;
                            $(companyTextBox).change();
			            }
			        }
			    });
            }

            function ProductSelect(productTextBoxId)
			{
				var productTextBox = document.getElementById(productTextBoxId);
				showModalDialogFrame({
				    url: '../FMWebApp/ProductSelectForm.aspx?Type=MaxProduct&Map=MAX_MAP&All=true',
				    width: 855,
				    height: 560,
				    onClose: function ()
				    {
				        if (this.returnValue != null)
				        {
				            var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
				            var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

				            productTextBox.value = asciiValue1;
				            productTextBox.title = asciiValue2;
                            $(productTextBox).change();
				        }
				    }
				});
			}

			function ConfirmProcess( oConfirmText )
			{
				if(confirm(oConfirmText))
				{
					document.getElementById("HiddenButton").click();
				}
			}
		</script>

        <style>
            .formContainer {
                margin: 10px;
                border: #486899 1px solid;
                width:800px;
                padding: 15px;
            }
            .operationsForm tr td {
                vertical-align: top;
                text-align: left;
                padding: 10px;
            }
            .FMCompanyTextBox {
                display: inline-block;
            }
            .inputLoading {
              background-image: url("data:image/gif;base64,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");
              background-position: right center;
              background-repeat: no-repeat;
              background-size: 16px 16px;
            }

            progress[value]::-webkit-progress-value::before {
              content: '80%';
              position: absolute;
              right: 0;
              top: -125%;
            }
        
        </style>
    </head>
    <body>
		<form id="Form1" method="post" runat="server">
            <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
            <div id="pageContent">
                <div class="formContainer">
                    <div class="headline">Accounting Operations</div>
                
                    <table class="postTransactionsToEnterpriseForm">
                        <tr>
                            <td>
                                <FMCONTROLS:FMDATE id="CloseoutDate" runat="server" CssClass="formfield"></FMCONTROLS:FMDATE>
                            </td>
                            <td>
                                <FMCONTROLS:FMLABEL id="managerLabel" AssociatedControlID="managerTextBox" runat="server" 
                                        CssClass="formfieldtitle" Width="72px">Manager:</FMCONTROLS:FMLABEL>
                                <FMCONTROLS:FMCompanyTextBox id="managerTextBox" ToolTip="Manager" runat="server" 
                                        CssClass="formfield" Width="264px" Role="MANAGER"></FMCONTROLS:FMCompanyTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <FMCONTROLS:FMBUTTON id="CloseoutAllButton" runat="server" Width="136px" CssClass="formfieldtitle" 
                                        Text="Closeout All Products"></FMCONTROLS:FMBUTTON>
                                <asp:button id="HiddenButton" onclick="HiddenButtonClick" style="display:none"
                                        runat="server" Text="Button" CausesValidation="False"></asp:button>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </div>
                <div class="formContainer">
                    <div class="headline">Post Transactions To Enterprise</div>
                
                    <table id="postTransactionsToEnterpriseForm">
                        <tr>
                            <td style="width: 50%;" colspan="2">
                                <div>
                                    <FMCONTROLS:FMLABEL id="specifyStartDateLabel" runat="server" BackColor="Transparent" 
                                            CssClass="formfieldtitle"><%= SpecifyStartDate %></FMCONTROLS:FMLABEL>
                                    <input id="specifyStartDate" type="checkbox" checked/>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <FMCONTROLS:FMLABEL id="postToEnterpriseStartDateLabel" runat="server" BackColor="Transparent" 
                                            CssClass="formfieldtitle" Width="72px"><%= StartDateText %></FMCONTROLS:FMLABEL>
                                    <input id="postToEnterpriseStartDate" autocomplete="off"/>
                                </div>
                            </td>
                            <td>
                                <div>
                                    <FMCONTROLS:FMLABEL id="productTextBoxLabel" AssociatedControlID="productTextBox" runat="server" 
                                            BackColor="Transparent" CssClass="formfieldtitle" Width="72px">Product:</FMCONTROLS:FMLABEL>
                                    <FMCONTROLS:FMPRODUCTTEXTBOX id="productTextBox" runat="server" CssClass="formfield" Width="169px" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <FMCONTROLS:FMLABEL id="postToEnterpriseStopDateLabel" runat="server" BackColor="Transparent" 
                                            CssClass="formfieldtitle" Width="72px"><%= StopDateText %></FMCONTROLS:FMLABEL>
                                    <input id="postToEnterpriseStopDate" autocomplete="off"/>
                                </div>
                            </td>
                            <td>
                                <div>
                                    <FMCONTROLS:FMLABEL id="managerTextBoxLabel2" AssociatedControlID="managerTextBox2" runat="server" 
                                            BackColor="Transparent" CssClass="formfieldtitle" Width="72px">Manager: </FMCONTROLS:FMLABEL>
                                    <FMCONTROLS:FMCompanyTextBox id="managerTextBox2"  runat="server" CssClass="formfield" Width="169px" Role="MANAGER" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="height: 25px; vertical-align: bottom;">
                                <div id="before-update"><%= NumberOfTransactions %>
                                    <input type="text" id="count" size="6" disabled>
                                    <span id="loading" style="display:inline-block; height: 16px; width: 16px" class="inputLoading"></span>
                                </div>
                                <div id="after-update"></div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2"><input id="postToEnterpriseButton" class="formfieldtitle" type="button" value="Post to Enterprise" /></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                &nbsp;
                                <table style="width: 100%">
                                    <tr class="progressRow" style="display:none;">
                                        <td colspan="2">
                                            Transactions remaining: <span id="toGo"></span>
                                            <span id="complete" style="display:none; font-weight: bold;">
                                                &nbsp;&nbsp;
                                            </span>
                                        </td>
                                    </tr>
                                    <tr class="progressRow" style="display: none;">
                                        <td style="width: 93%">
                                            <div id="progressbar"></div>
                                        </td>
                                        <td style="width: 7%">
                                            <div id="percentComplete"></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">&nbsp;</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
		</form>
    <script>
        function GetTransactionsAffected() {
            var product = $('#productTextBox').val();
            var manager = $('#managerTextBox2').val();
            var start = $('#postToEnterpriseStartDate').val();
            var stop = $('#postToEnterpriseStopDate').val();
            $('#count').val("");
            $('#loading').show();
            $(".progressRow").hide();
            $('#complete').hide();
            $.getJSON('<%=Request.ApplicationPath%><%= InlineSessionID %>api/AccountingOperations/TransactionCountToUpdateToEnterprise',
                    { product: encodeURI(product), manager: encodeURI(manager), start: encodeURI(start), stop: encodeURI(stop) })
                .done(function(data) {
                    $('#count').val(formatNumber(data));
                    $('#loading').hide();
                })
                .fail(function(error) {
                    $('#count').val("");
                    $('#loading').hide();
                });
        }

        function PostToEnterprise() {
            var product = $('#productTextBox').val();
            var manager = $('#managerTextBox2').val();
            var start = $('#postToEnterpriseStartDate').val();
            var stop = $('#postToEnterpriseStopDate').val();
            updatePercentageComplete(null, true);

            $('#postToEnterpriseButton').prop('disabled', true);
            $.ajax({
                method: 'POST',
                url: '<%=Request.ApplicationPath%><%= InlineSessionID %>api/AccountingOperations/StartUpdateTransactionsToEnterprise?'
                    + $.param({
                        product: encodeURI(product),
                        manager: encodeURI(manager),
                        start: encodeURI(start),
                        stop: encodeURI(stop)
                    }),
                dataType: "json"
            })
                .done(function (data) {
                    window.jobID = data.JobIdentifier;
                    window.totalTransactionsToUpdate = data.TotalTransactionsToUpdate;
                    updatePercentageComplete(data);
                    setTimeout(CheckStatusOnUpdate, 1000);
                    console.log(data);
                })
                .fail(function(error) {
                    console.log(error);
                });
        }

        function updatePercentageComplete(data, loading) {
            $(".progressRow").show();
            $("#percentComplete").html('');
            $("#toGo").html('');
            if (loading) {
                $("#progressbar").progressbar({ value: 0 })
                return;
            }

            var percentage = 0;
            if (data.TotalTransactionsToUpdate === 0) {
                percentage = 100;
            }
            else {
                percentage = data.TotalTransactionsUpdated / data.TotalTransactionsToUpdate * 100;
            }

            $("#toGo").html(formatNumber(data.TotalTransactionsToUpdate - data.TotalTransactionsUpdated));
            $("#percentComplete").html(parseInt(percentage) + "%");
            $("#progressbar").progressbar({value: percentage})
        }

        function CheckStatusOnUpdate() {
            $.getJSON('<%=Request.ApplicationPath%><%= InlineSessionID %>api/AccountingOperations/CheckUpdateTransactionsToEnterprise',
                    { jobID: encodeURI(window.jobID)})
                .done(function (data) {
                    if (data.Complete) {
                        updatePercentageComplete(data);
                        $('#complete').show().html("&nbsp;<%= Complete %> " + $("#count").val() + " transactions posted to Enterprise.");
                        $('#postToEnterpriseButton').prop('disabled', false);
                        $('#count').val(0);
                        console.log('Complete!');
                    }
                    else {
                        updatePercentageComplete(data);
                        console.log(data);
                        setTimeout(CheckStatusOnUpdate, 1000);
                    }
                })
                .fail(function(error) {
                    console.log(error);
                });
        }

        function formatNumber(num) {
            return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
        }

        function appendLeadingZeroes(n) {
            if(n <= 9) {
                return "0" + n;
            }
            return n
        }

        document.addEventListener("DOMContentLoaded", function () {
            var date = new Date();
            date.setDate(date.getDate() - 1);
            var today = appendLeadingZeroes(date.getMonth() + 1) + '/' + appendLeadingZeroes(date.getDate()) + '/' + date.getFullYear();
            $('#postToEnterpriseStartDate').datepicker();
            $('#postToEnterpriseStopDate').datepicker();
            $('#postToEnterpriseStartDate').val(today);
            $('#postToEnterpriseStopDate').val(today);
            $('#specifyStartDate').change(function() {
                if (this.checked) {
                    $('#postToEnterpriseStartDate').attr('disabled', false);
                }
                else {
                    $('#postToEnterpriseStartDate').attr('disabled', true);
                    $('#postToEnterpriseStartDate').val(null);
                    GetTransactionsAffected();
                }
            });
            $('#postToEnterpriseStartDate').change(function () { GetTransactionsAffected(); });
            $('#postToEnterpriseStopDate').change(function () { GetTransactionsAffected(); });
            $('#managerTextBox2').change(function () { GetTransactionsAffected(); });
            $('#productTextBox').change(function () { GetTransactionsAffected(); });

            $('#postToEnterpriseButton').click(PostToEnterprise);

            GetTransactionsAffected();

            $('#loading').hide();
        });
    </script>
</body>
</html>
