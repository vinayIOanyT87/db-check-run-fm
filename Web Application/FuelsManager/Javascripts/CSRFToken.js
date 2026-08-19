if (typeof(rndTokenStr) == "undefined") {
	rndTokenStr = new Object();
	rndTokenStr = "";
}

function AddCSRFTokenToUrl(u) {

	if (rndTokenStr == "") {
		return u;
	}

	var u1 = u;
	if (u != '' && u.charAt(u.length - 1) != '/') {
		var p = u1.indexOf('?');
		if (p > -1)
			u1 += "&CSRFToken=" + rndTokenStr ;
		else
			u1 += "?CSRFToken=" + rndTokenStr ;
	}
	return u1;
}

window_locatio = function(u)
{
	var u1 = AddCSRFTokenToUrl(u);
	window.location=u1;
};

window_open = function(u, n, f, r)
{
	var u1 = AddCSRFTokenToUrl(u);
	if (r != undefined) return window.open(u1, n, f, r);
	else if (f != undefined) return window.open(u1, n, f);
	else if (n != undefined) return window.open(u1, n);
	return window.open(u1);
};

window_location_replace = function(u)
{
	var u1 = AddCSRFTokenToUrl(u);
	return window.location.replace(u1);
};

window_location_assign = function(u)
{
    var u1 = AddCSRFTokenToUrl(u);
    return window.location.assign(u1);
};


window_showModalDialog = function(u, a, f)
{
	var u1 = AddCSRFTokenToUrl(u);
	if (f != undefined) return window.showModalDialog(u1, a, f);
	else if (a != undefined) return window.showModalDialog(u1, a);
	return window.showModalDialog(u1);
};


window_top_locatio = function(u)
{
	var u1 = AddCSRFTokenToUrl(u);
	window.top.location=u1;
};

window_top_open = function(u, n, f, r)
{
	var u1 = AddCSRFTokenToUrl(u);
	if (r != undefined) return window.top.open(u1, n, f, r);
	else if (f != undefined) return window.top.open(u1, n, f);
	else if (n != undefined) return window.top.open(u1, n);
	return window.top.open(u1);
};

window_top_showModalDialog = function(u, a, f)
{
	var u1 = AddCSRFTokenToUrl(u);
	if (f != undefined) return window.top.showModalDialog(u1, a, f);
	else if (a != undefined) return window.top.showModalDialog(u1, a);
	return window.top.showModalDialog(u1);
};
window_self_locatio = function(u) {
	var u1 = AddCSRFTokenToUrl(u);
	window.self.location = u1;
};

window_self_open = function(u, n, f, r) {
	var u1 = AddCSRFTokenToUrl(u);
	if (r != undefined) return window.self.open(u1, n, f, r);
	else if (f != undefined) return window.self.open(u1, n, f);
	else if (n != undefined) return window.self.open(u1, n);
	return window.self.open(u1);
};

window_self_showModalDialog = function(u, a, f) {
	var u1 = AddCSRFTokenToUrl(u);
	if (f != undefined) return window.self.showModalDialog(u1, a, f);
	else if (a != undefined) return window.self.showModalDialog(u1, a);
	return window.self.showModalDialog(u1);
};
