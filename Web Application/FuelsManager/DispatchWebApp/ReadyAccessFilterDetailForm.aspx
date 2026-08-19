<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReadyAccessFilterDetailForm.aspx.cs" Inherits="FuelsManager.DispatchWebApp.ReadyAccessFilterDetailForm" %>

<%@ Register Src="..\MenuBar\FMMenuBar.ascx" TagName="FMMenuBar" TagPrefix="FMMenuBar" %>
<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
	<LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
	    <FMMenuBar:FMMenuBar ID="ucFMMenuBar" runat="server" />
        <div id="pageContent">
            <div id="content" style="position: absolute">
		    <asp:Image ID="fadeImage" Style="z-index: 100; left: 0px; top: 0px; position: absolute;"
			    runat="server" ImageUrl="<%$ AppSettings: PageFadeImage %>"></asp:Image>
        
            <table style="z-index:110; left:32px; top: 10px; width:750px; position: absolute;" cellpadding="5" >
		        <tr>
		            <td colspan="2">
                        <FMControls:FMLabel id="FMLabel1" runat="server" CssClass="headline" Text="Ready Access Filter for Dispatch"/>
		            </td>
		        </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="FMLabel2" runat="server" CssClass="formfieldtitle" Text="Filter Name:"/>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="FilterNameTextBox" CssClass="formfield" style="width:200px"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="FMLabel3" runat="server" CssClass="formfieldtitle" Text="Filter Description:"/>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="FilterDescriptionTextBox" CssClass="formfield" style="width:375px"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="FMLabel4" runat="server" CssClass="formfieldtitle" Text="Customer:"/>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td><FMControls:FMLabel runat="server" CssClass="formfieldtitle" Text="Include:"/></td>
                                <td><asp:TextBox runat="server" ID="CustomerIncludeTextBox" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="CustomerIncludeAddButton" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                            <tr>
                                <td><FMControls:FMLabel runat="server" CssClass="formfieldtitle" Text="Exclude:"/></td>
                                <td><asp:TextBox runat="server" ID="CustomerExcludeTextBox" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="CustomerExcludeAddButton" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="FMLabel5" runat="server" CssClass="formfieldtitle" Text="Location Group:"/>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td><FMControls:FMLabel ID="FMLabel6" runat="server" CssClass="formfieldtitle" Text="Include:"/></td>
                                <td><asp:TextBox runat="server" ID="TextBox3" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="FMButton3" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                            <tr>
                                <td><FMControls:FMLabel ID="FMLabel7" runat="server" CssClass="formfieldtitle" Text="Exclude:"/></td>
                                <td><asp:TextBox runat="server" ID="TextBox4" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="FMButton4" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="FMLabel8" runat="server" CssClass="formfieldtitle" Text="Location:"/>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td><FMControls:FMLabel ID="FMLabel9" runat="server" CssClass="formfieldtitle" Text="Include:"/></td>
                                <td><asp:TextBox runat="server" ID="TextBox1" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="FMButton1" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                            <tr>
                                <td><FMControls:FMLabel ID="FMLabel10" runat="server" CssClass="formfieldtitle" Text="Exclude:"/></td>
                                <td><asp:TextBox runat="server" ID="TextBox2" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="FMButton2" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="FMLabel11" runat="server" CssClass="formfieldtitle" Text="Equipment Type:"/>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td><FMControls:FMLabel ID="FMLabel12" runat="server" CssClass="formfieldtitle" Text="Include:"/></td>
                                <td><asp:TextBox runat="server" ID="TextBox5" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="FMButton5" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                            <tr>
                                <td><FMControls:FMLabel ID="FMLabel13" runat="server" CssClass="formfieldtitle" Text="Exclude:"/></td>
                                <td><asp:TextBox runat="server" ID="TextBox6" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="FMButton6" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="FMLabel14" runat="server" CssClass="formfieldtitle" Text="Destination:"/>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td><FMControls:FMLabel ID="FMLabel15" runat="server" CssClass="formfieldtitle" Text="Include:"/></td>
                                <td><asp:TextBox runat="server" ID="TextBox7" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="FMButton7" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                            <tr>
                                <td><FMControls:FMLabel ID="FMLabel16" runat="server" CssClass="formfieldtitle" Text="Exclude:"/></td>
                                <td><asp:TextBox runat="server" ID="TextBox8" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="FMButton8" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="FMLabel17" runat="server" CssClass="formfieldtitle" Text="Depart Flight Number:"/>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td><FMControls:FMLabel ID="FMLabel18" runat="server" CssClass="formfieldtitle" Text="Include:"/></td>
                                <td><asp:TextBox runat="server" ID="TextBox9" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="FMButton9" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                            <tr>
                                <td><FMControls:FMLabel ID="FMLabel19" runat="server" CssClass="formfieldtitle" Text="Exclude:"/></td>
                                <td><asp:TextBox runat="server" ID="TextBox10" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="FMButton10" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <FMControls:FMLabel ID="FMLabel20" runat="server" CssClass="formfieldtitle" Text="Supplier:"/>
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td><FMControls:FMLabel ID="FMLabel21" runat="server" CssClass="formfieldtitle" Text="Include:"/></td>
                                <td><asp:TextBox runat="server" ID="TextBox11" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="FMButton11" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                            <tr>
                                <td><FMControls:FMLabel ID="FMLabel22" runat="server" CssClass="formfieldtitle" Text="Exclude:"/></td>
                                <td><asp:TextBox runat="server" ID="TextBox12" CssClass="formfield" style="width:525px"/></td>
                                <td><FMControls:FMButton ID="FMButton12" CssClass="formfieldtitle" runat="server" Text="Add"/></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:right">
                        <FMControls:FMButton runat="server" ID="OKButton" Text="OK" CssClass="formfieldtitle" width="75px"/> &nbsp;
                        <FMControls:FMButton runat="server" ID="CancelButton" CssClass="formfieldtitle" Text="Cancel" width="75px"/> &nbsp;
                    </td>
                </tr>
            </table>
        </div>
        </div>
    </form>
</body>
</html>
