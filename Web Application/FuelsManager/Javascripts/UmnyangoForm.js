document.addEventListener("DOMContentLoaded", function (event)
{
    jQuery.fn.loginCenter = function ()
    {
        this.css("position", "absolute");
        this.css("top", Math.max(0, (($(window).height() - $(this).outerHeight()) / 2) +
            $(window).scrollTop()) + "px");
        this.css("left", Math.max(0, (($(window).width() - $(this).outerWidth()) / 2) +
            $(window).scrollLeft()) + "px");
        return this;
    };

    if (typeof JSON == 'undefined')
    {
        $("#JSONWarningText").text('Error: Current Browser configuration does not support JSON objects.  This problem is often caused when the browser is run in Compatibility View mode(s).  JSON support is required by the FuelsManager application.  Please correct this problem and restart your browser.');
        $("#JSONWarningLabel").css("visibility", "visible");
        $("#JSONWarningLabel").loginCenter();
        $("#splashDiv").css("visibility", "hidden");

        $(window).resize(function ()
        {
            $("#JSONWarningLabel").loginCenter();
        });
    }

    //Script for password hint bubble *Umnyango.aspx*.
    var isButtonVisible = $("#PasswordHintButton").is(":visible");
    var isButtonHidden = $("#PasswordHintButton").is(":hidden");

    if (isButtonHidden || isButtonVisible)
    {
        $('#PasswordHintButton').tooltip(
        {
            position:
            {
                my: "center bottom-20",
                at: "center top",
                using: function(position, feedback) {
                    $(this).css(position);
                    $("<div>")
                        .addClass("arrow")
                        .appendTo(this);
                }
            }
        });
    }

    ////Script for Password policy bubble *UserForm.aspx*.
    //$('#PasswordPopupBubbleLabel').tooltip(
    //{
    //    position:
    //    {
    //        my: "center bottom-20",
    //        at: "center top",
    //        using: function(position, feedback) {
    //            $(this).css(position);
    //            $("<div>")
    //                .addClass("arrow")
    //                .appendTo(this);
    //        }
    //    }
    //});

    //    var id = document.getElementById("LoginButton").setActive();
    //if(id){
    //    if (id.setActive) { 
    //        id.setActive() 
    //    } else if (id.focus) { 
    //        id.focus 
    //    };

    //    var oUserNameTextBox = document.getElementById("UserNameTextBox");
    //    if (oUserNameTextBox != null) {
    //        oUserNameTextBox.focus();
    //    }
						   
    //    var oInitialPasswordTextBox = document.getElementById("InitialPasswordTextBox");
    //    var oPasswordTextBox = document.getElementById("PasswordTextBox");
    //    if (oInitialPasswordTextBox != null
    //        && oPasswordTextBox != null) 
    //    {
    //        oPasswordTextBox.value = oInitialPasswordTextBox.value;
    //        if ( typeof oPasswordTextBox.addEventListener != "undefined" ) {
    //            oPasswordTextBox.addEventListener("onactivate", PasswordActive);
    //        }
    //    }
    //}
    //function PasswordActive() 
    //{
    //    var oPasswordTextBox = document.getElementById("PasswordTextBox");
    //    oPasswordTextBox.select();
    //}

        
    //This code is the above code minified by Google Closure Compiler Service on Advanced setting
    var id = document.getElementById("LoginButton");
    if (id)
    {
        if (id.setActive)
        {
            id.setActive();
        }
        else if (id.focus)
        {
            id.focus();
        };

        var a = document.getElementById("UserNameTextBox");
        null != a && a.focus();
        var b = document.getElementById("InitialPasswordTextBox"), c = document.getElementById("PasswordTextBox");
        null != b && null != c && (c.value = b.value, "undefined" != typeof c.addEventListener && c.addEventListener("onactivate", d));
    }

    //Code taken from UserForm.aspx
    //var oUserID = document.getElementById("Name");

    //if (!oUserID.disabled)
    //{
    //    oUserID.focus();
    //    oUserID.setActive();
    //}
});

//Code taken from UmnyangoForm.aspx
function d()
{
    document.getElementById("PasswordTextBox").select();
}

function openPrivacyPolicy()
{
    FMMenuBarLib.openPrivacyPolicy();
}

//Code taken from UserForm.aspx
function RemoveSpecialChar(txtVal)
{
    if (txtVal.value != '' && txtVal.value.match(/^[\w ]+$/) == null)
    {
        txtVal.value = txtVal.value.replace(/\'/ig, '');
    }
}