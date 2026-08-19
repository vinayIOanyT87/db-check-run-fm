//KioskKeyRestrictions.js
/*
    this files contains the processing and remove of the key commands that we do not want the user
    to be able to do
*/

//this function will prevent the user from getting out of kiosk mode by checking for the deafualt commands
//and not processing them --%>
document.addEventListener('keydown', function (ev) {
    if (ev.altKey === true) {
        if (ev.which === 115) {  // f4
            ev.preventDefault();
        }
        else if (ev.which === 37) {  // left arrow
            ev.preventDefault();
        }
        else if (ev.which === 39) {  // right arrow
            ev.preventDefault();
        }
    }
    else if (ev.ctrlKey === true) {
        if (ev.which === 66) {  // b
            ev.preventDefault();
        }
        else if (ev.which === 72) {  // h
            ev.preventDefault();
        }
        else if (ev.which === 76) {  // l
            ev.preventDefault();
        }
        else if (ev.which === 78) {  // n
            ev.preventDefault();
        }
        else if (ev.which === 79) {  // o
            //debugger;
            ev.preventDefault();
        }
        else if (ev.which === 83) {  // s
            ev.preventDefault();
        }
        else if (ev.which === 87) {  // w
            ev.preventDefault();
        }
    }
    else if (ev.which === 27) {  // esc
        ev.preventDefault();
    }
    //ev.preventDefault();
});

function pageWidth() {
    return window.innerWidth != null? window.innerWidth: document.body != null? document.documentElement.clientWidth:null;
}
function pageHeight() {
    return window.innerHeight != null ? window.innerHeight : document.body != null ? document.documentElement.clientHeight : null;
}
