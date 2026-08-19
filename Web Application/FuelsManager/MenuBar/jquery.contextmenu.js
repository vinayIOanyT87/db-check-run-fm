/**
 * jQuery plugin for Pretty looking right click context menu.
 *
 * Requires popup.js and popup.css to be included in your page. And jQuery, obviously.
 *
 * Usage:
 *
 *   $('.something').contextPopup({
 *     title: 'Some title',
 *     items: [
 *       {label:'My Item', icon:'/some/icon1.png', action:function() { alert('hi'); }},
 *       {label:'Item #2', icon:'/some/icon2.png', action:function() { alert('yo'); }},
 *       null, // divider
 *       {label:'Blahhhh', icon:'/some/icon3.png', action:function() { alert('bye'); }, isEnabled: function() { return false; }},
 *     ]
 *   });
 *
 * Icon needs to be 16x16. I recommend the Fugue icon set from: http://p.yusukekamiyamane.com/ 
 *
 * - Joe Walnes, 2011 http://joewalnes.com/
 *   https://github.com/joewalnes/jquery-simple-context-menu
 *
 * MIT License: https://github.com/joewalnes/jquery-simple-context-menu/blob/master/LICENSE.txt
 */

GenerateContextMenuData = function () {
    var queryArr = [];
    
    var menuDataEmpty = {
        "items": queryArr
    };

    var myList = document.getElementById("quickLinksBar");
    if(myList == null)
    {
        return menuDataEmpty;
    }
    var items = myList.getElementsByTagName("li");

    if( items == null || items.lenth == 0)
    {
        return menuDataEmpty
    }

    for (var i = 0; i < items.length; ++i) {
        var item = items[i];
        var link = item.getElementsByTagName("a");
        if (link) {
            var isVisible = (item.offsetTop <= 80);
            if (!isVisible) {
                var action = link[0].onclick;
                var menuItem = {
                    "label": item.textContent,
                    "action": action,
                    "isEnabled": FMMenuBarLib.AlwaysTrue
                };
                queryArr.push(menuItem);
            }
        }
    }
    var menuData = {
        "items": queryArr
    };

    return menuData;
}

// Build popup menu HTML
function createMenu(e) {

    // Define default settings
var settings = {
    contextMenuClass: 'contextMenuPlugin',
    gutterLineClass: 'gutterLine',
    headerClass: 'header',
    seperatorClass: 'divider',
    title: '',
    items: []
};

var menuData = GenerateContextMenuData();

if (menuData.items.length == 0) return;

// merge them
$.extend(settings, menuData);


    var menu = $('<ul id="fmContextMenu" class="' + settings.contextMenuClass + '"><div class="' + settings.gutterLineClass + '"></div></ul>')
        .appendTo(document.body);
    if (settings.title) {
        $('<li class="' + settings.headerClass + '"></li>').text(settings.title).appendTo(menu);
    }
    settings.items.forEach(function (item) {
        if (item) {
            var rowCode = '<li><a href="#"><span></span></a></li>';
            // if(item.icon)
            //   rowCode += '<img>';
            // rowCode +=  '<span></span></a></li>';
            var row = $(rowCode).appendTo(menu);
            if (item.icon) {
                var icon = $('<img>');
                icon.attr('src', item.icon);
                icon.insertBefore(row.find('span'));
            }
            row.find('span').text(item.label);

            if (item.isEnabled != undefined && !item.isEnabled()) {
                row.addClass('disabled');
            } else if (item.action) {
                row.find('a').click(function () { item.action(e); });
            }

        } else {
            $('<li class="' + settings.seperatorClass + '"></li>').appendTo(menu);
        }
    });
    menu.find('.' + settings.headerClass).text(settings.title);
       
    menu.show();

    //Ensure that context menu always appears below quick link expansion button
    var quickLinksIcon = document.getElementById("quickLinksShowExtra");
    var left = 0;
    var top = 0;
    var quickLinkOffsetElement = quickLinksIcon;
    while (quickLinkOffsetElement != null)
    {
        left += quickLinkOffsetElement.offsetLeft;
        top += quickLinkOffsetElement.offsetTop;
        quickLinkOffsetElement = quickLinkOffsetElement.offsetParent;

    }
    left -= menu.width();
    top += quickLinksIcon.clientHeight + 8;

    // Create and show menu
    menu.css({ zIndex: 1000001, left: left, top: top })
        .bind('click', function () { return false; });

    // Cover rest of page with invisible div that when clicked will cancel the popup.
    var bg = $('<div></div>')
        .css({ left: 0, top: 0, width: '100%', height: '100%', position: 'absolute', zIndex: 1000000 })
        .appendTo(document.body)
        .bind('click', function () {
            // If click or right click anywhere else on page: remove clean up.
            bg.remove();
            menu.remove();
            return false;
        });

    // When clicking on a link in menu: clean up (in addition to handlers on link already)
    menu.find('a').click(function () {
        bg.remove();
        menu.remove();
    });

    // Cancel event, so real browser popup doesn't appear.
    return false;
}

   

