const puppeteer = require('puppeteer');
const config_data = require('./config.json')
const path = require('path');

var myArgs = process.argv.slice(2);
//console.log('myArgs: ', myArgs);
if (myArgs.length < 5) {
    console.log("usage: node printpointgroup.js <userid> <password> <siteid> <operategroupname> <operategroupguid>")
    return;
}
var userid = myArgs[0];
var password = myArgs[1];
var siteid = myArgs[2];
var operategroupname = myArgs[3];
var operategroupguid = myArgs[4];

/* taken from https://github.com/rocklau/pkg-puppeteer */
const isPkg = typeof process.pkg !== 'undefined';

let chromiumExecutablePath = (isPkg ?
    puppeteer.executablePath().replace(
        /^.*?\/node_modules\/puppeteer\/\.local-chromium/,
        path.join(path.dirname(process.execPath), 'chromium')
    ) :
    puppeteer.executablePath()
);


(async(userid, password, siteid, operategroupname, operategroupguid) => {

    // node printpointgroup.js administrator marietta marathon Tanks C24EEAAB-3C35-431F-9252-56F1F933EDDA
    //const browser = await puppeteer.launch();
    const browser = await puppeteer.launch({ executablePath: chromiumExecutablePath, headless: true, devtools: false });
    try {

        const page = await browser.newPage();
        await page.goto(config_data.base_url);

        await page.evaluate((operategroupname, operategroupguid) => {
            sessionStorage.setItem('operateMode', 'printPointGroup');
            sessionStorage.setItem('printPointGroupName', operategroupname);
            sessionStorage.setItem('printPointGroupGuid', operategroupguid);
        }, operategroupname, operategroupguid);

        // login
        const frame = (await page.frames())[1];
        await frame.$eval('input[name=UserNameTextBox]', (el, userid) => el.value = userid, userid)
        await frame.$eval('input[name=PasswordTextBox]', (el, password) => el.value = password, password)
        await frame.$eval('input[name=SiteTextBox]', (el, siteid) => el.value = siteid, siteid)
        await frame.$eval('input[name=LoginButton]', form => form.click());

        // wait for main screen
        const mainpagemenu = await page.waitForSelector('#bdyMenuBarBody');

        // click for menu item
        await mainpagemenu.$eval("a[id='Operations_Inventory Management_Operate']", form => form.click());

        // Operate
        // get the iframe, MVC screens work within an itrame
        await page.waitForSelector("iframe");
        const iframeHandle = await page.$('iframe');
        const iframe = await iframeHandle.contentFrame();

        const operatepage = await iframe.waitForSelector('#mainTab');
        await page.waitFor(14000)

        // open the pointgroup menu for printing
        await iframe.$eval('div.tab-pane.active .tab-configuration', button => button.click());
        await iframe.waitFor('.popover-content')

        await iframe.$eval('.popover.fade.bottom.in button[name=configurationPointGroupAutoPrintHidden]', button => button.click());
        await page.waitFor(5000)

        // generate pdf by copying the table to print into another page
        let dom = await iframe.$eval('#pointgroupprint', (element) => {
                return element.innerHTML
            }) // Get DOM HTML
        dom = '<h4 class="printpointgroupheader" style="width: 100%; text-align: center">' + operategroupname + '</h4>' + dom;
        await page.setContent(dom) // HTML markup to assign to the page for generate pdf
            // path, can be relative or absolute path
        await page.addStyleTag({ url: config_data.base_url + '/Areas/Content/GeneralStyles.css' })
        await page.addStyleTag({ url: config_data.base_url + '/Content/BootStrap.css' })
        await page.addStyleTag({ url: config_data.base_url + '/Areas/Content/OperateIndex.css' })
        await page.addStyleTag({ url: config_data.base_url + '/Areas/Content/slick.grid.css' })

        await page.pdf({
            path: 'pointgroup.pdf',
            format: 'Letter',
            margin: { left: '1cm', top: '1cm', right: '1cm', bottom: '1cm' },
            displayHeaderFooter: false
        })

    } catch (error) {
        console.log(error)
    } finally {
        //await page.close
        await browser.close();
    }

})(userid, password, siteid, operategroupname, operategroupguid);