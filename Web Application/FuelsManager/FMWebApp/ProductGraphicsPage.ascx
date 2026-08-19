<%@ Register TagPrefix="FMControls" Namespace="FMControls" Assembly="FMControls" %>
<%@ Control Language="c#" AutoEventWireup="true" CodeBehind="ProductGraphicsPage.ascx.cs" Inherits="FuelsManager.FMWebApp.ProductGraphicsPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<html>
<head>
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Areas/lib/spectrum.js" %>"></script>
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Areas/Scripts/DrawPatternPalette.js" %>"></script>
    <script src="<%= HttpRuntime.AppDomainAppVirtualPath + "/Areas/Scripts/DrawPropertyMenu.js" %>"></script>
    <link rel='stylesheet' href="<%= HttpRuntime.AppDomainAppVirtualPath + "/Areas/Content/spectrum.css" %>" />
    <LINK href="<%= HttpRuntime.AppDomainAppVirtualPath + "/CSS/FuelsManager.css" %>" rel="stylesheet" />
    
    <style type="text/css">
        .canvasClass {
            height: 19px;
            float: right;
            border: 1px solid #3b5780;
        }

        .tableColumn1 {
            width: 40%;
        }

        .tableColumn2 {
            width: 40%;
        }
    </style>
</head>
<body>
    <table>
        <tr>
            <td class="tableColumn1">
                <FMControls:FMLabel ID="FillColorLabel" Style="z-index: 110;" runat="server"
                    CssClass="formfieldtitle" BackColor="Transparent">Fill Color:</FMControls:FMLabel>
            </td>
            <td class="tableColumn2">
                <FMControls:FMLabel ID="PatternColorLabel" Style="z-index: 110;" runat="server"
                    CssClass="formfieldtitle" BackColor="Transparent">Pattern Color:</FMControls:FMLabel>
            </td>
            <td>
                <FMControls:FMLabel ID="PatternPickerLabel" Style="z-index: 110;" runat="server"
                    CssClass="formfieldtitle" BackColor="Transparent">Pattern:</FMControls:FMLabel>
            </td>
        </tr>
        <tr>
            <td>
                <input id="FillColorSpectrumTextBoxId" type="text" />
            </td>
            <td>
                <input id="PatternColorSpectrumTextBoxId" type="text" />
            </td>
            <td>
                <table>
                    <tr>
                        <td>
                            <span onclick="PatternOnClick('1')">
							    <canvas id="CanvasTag1Id" width="80" height="15" class="canvasClass"></canvas>
						    </span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span onclick="PatternOnClick('2')">
							    <canvas id="CanvasTag2Id" width="80" height="15" class="canvasClass"></canvas>
						    </span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span onclick="PatternOnClick('3')">
							    <canvas id="CanvasTag3Id" width="80" height="15" class="canvasClass"></canvas>
						    </span>
                        </td>
                     </tr>
                    <tr>
                        <td>
                            <span onclick="PatternOnClick('4')">
							    <canvas id="CanvasTag4Id" width="80" height="15" class="canvasClass"></canvas>
						    </span>
                        </td>
                     </tr>
                    <tr>
                        <td>
                            <span onclick="PatternOnClick('5')">
							    <canvas id="CanvasTag5Id" width="80" height="15" class="canvasClass"></canvas>
						    </span>
                        </td>
                     </tr>
                    <tr>
                        <td>
                            <span onclick="PatternOnClick('6')">
							    <canvas id="CanvasTag6Id" width="80" height="15" class="canvasClass"></canvas>
						    </span>
                        </td>
                     </tr>
                    <tr>
                        <td>
                            <span onclick="PatternOnClick('7')">
							    <canvas id="CanvasTag7Id" width="80" height="15" class="canvasClass"></canvas>
						    </span>
                        </td>
                     </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td></td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td>
                <FMControls:FMLabel ID="FMLabel1" Style="z-index: 110;" runat="server"
                    CssClass="formfieldtitle" BackColor="Transparent">Selections:</FMControls:FMLabel>
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td>
                <input id="FillColorManualColorTextBoxId" type="text" placeholder="#" style="font-size: 9px; width: 100px; height: 15px; border: 1px solid rgb(86, 126, 185);" oninput="FillColorManualColorChange();" onfocus="FillColorManualColorChange();"/>
				&nbsp;
				<input id="FillColorManualColorSamplerTextBoxId" type="text" style="font-size: 9px; width: 60px; height: 15px; border: 1px solid rgb(86, 126, 185);" readonly="readonly" />
            </td>
            <td>
                <input id="PatternColorManualColorTextBoxId" type="text" placeholder="#" style="font-size: 9px; width: 100px; height: 15px; border: 1px solid rgb(86, 126, 185);" oninput="PatternColorManualColorChange();" onfocus="PatternColorManualColorChange();"/>
				&nbsp;
				<input id="PatternColorManualColorSamplerTextBoxId" type="text" style="font-size: 9px; width: 60px; height: 15px; border: 1px solid rgb(86, 126, 185);" readonly="readonly" />
            </td>
            <td>
                <canvas id="CanvasTagSelectedId" width="80" height="15" class="canvasClass"></canvas>
            </td>
        </tr>
    </table>
    
    <asp:HiddenField ID="FillColorHexValue" runat="server"/>
    <asp:HiddenField ID="PatternColorHexValue" runat="server"/>
    <asp:HiddenField ID="SelectedPatternNumber" runat="server"/>

    <asp:HiddenField ID="IsFillColorHexValueEnabled" runat="server"/>
    <asp:HiddenField ID="IsPatternColorHexValueEnabled" runat="server"/>
    <asp:HiddenField ID="IsSelectedPatternNumberEnabled" runat="server"/>

    <script>
        
        var currentSelectedPattern = '1';
        var hiddenFillColorFieldId = "<%= FillColorHexValue.ClientID %>";
        var hiddenPatternColorFieldId = "<%= PatternColorHexValue.ClientID %>";
        var hiddenSelectedPatternFieldId = "<%= SelectedPatternNumber.ClientID %>";

        var isFillColorHexValueEnabled = "<%= IsFillColorHexValueEnabled.ClientID %>";
        var isPatternColorHexValueEnabled = "<%= IsPatternColorHexValueEnabled.ClientID %>";
        var isSelectedPatternNumberEnabled = "<%= IsSelectedPatternNumberEnabled.ClientID %>";
        
        $(document).ready(function ()
        {
            // Get the hex color value from the ASP hidden fields.
            
            var fillColorHexHiddenField = document.getElementById(hiddenFillColorFieldId);
            var patternColorHexHiddenField = document.getElementById(hiddenPatternColorFieldId);
            var selectedPatternNumberHiddenField = document.getElementById(hiddenSelectedPatternFieldId);

            var fillColorHexValueEnabled = document.getElementById(isFillColorHexValueEnabled);
            var patternColorHexValueEnabled = document.getElementById(isPatternColorHexValueEnabled);
            var selectedPatternNumberEnabled = document.getElementById(isSelectedPatternNumberEnabled);

            var fillColor = "#99ccff";
            var patternColor = "#ffffff";
            var selectedPattern = '1';

            if (fillColorHexHiddenField.value !== null && fillColorHexHiddenField.value !== "")
            {
                fillColor = fillColorHexHiddenField.value;
            }

            if (patternColorHexHiddenField.value !== null && patternColorHexHiddenField.value !== "")
            {
                patternColor = patternColorHexHiddenField.value;
            }

            if (selectedPatternNumberHiddenField.value !== null && selectedPatternNumberHiddenField.value !== "")
            {
                selectedPattern = selectedPatternNumberHiddenField.value;
            }

            currentSelectedPattern = selectedPattern;

            $("#FillColorSpectrumTextBoxId").spectrum({
                flat: true,
                showInput: false,
                showButtons: false,
                color: fillColor,
                move: function (color) { SetColorHexValue(color, "#FillColorManualColorTextBoxId", "#FillColorManualColorSamplerTextBoxId"); }
            });

            $("#PatternColorSpectrumTextBoxId").spectrum({
                flat: true,
                showInput: false,
                showButtons: false,
                color: patternColor,
                move: function (color) { SetColorHexValue(color, "#PatternColorManualColorTextBoxId", "#PatternColorManualColorSamplerTextBoxId"); }
            });

            $("#FillColorSpectrumTextBoxId").spectrum('set', fillColor);
            $("#PatternColorSpectrumTextBoxId").spectrum('set', patternColor);

            $("#FillColorManualColorTextBoxId").val(fillColor);
            $("#PatternColorManualColorTextBoxId").val(patternColor);

            $("#FillColorManualColorSamplerTextBoxId").css('background-color', fillColor);
            $("#PatternColorManualColorSamplerTextBoxId").css('background-color', patternColor);

            // Create the pattern pickers.
            CreatePatterns();

            // Set the selected pattern.
            PatternOnClick(selectedPattern);
            
            //Set Field Accessibility For Child Record Version
            if (fillColorHexValueEnabled.value === "False")
            {
                $("#FillColorManualColorTextBoxId").attr("disabled", "disabled");

                $("#FillColorSpectrumTextBoxId").spectrum({
                    flat: true,
                    showInput: false,
                    showButtons: false,
                    move: function () { }
                });
            }
            if (patternColorHexValueEnabled.value === "False")
            {
                $("#PatternColorManualColorTextBoxId").attr("disabled", "disabled");
                $("#PatternColorSpectrumTextBoxId").spectrum({
                    flat: true,
                    showInput: false,
                    showButtons: false,
                    move: function () { }
                });
            }
            if (selectedPatternNumberEnabled.value === "False")
            {
                $("#CanvasTag1Id").css('pointer-events', 'none');
                $("#CanvasTag2Id").css('pointer-events', 'none');
                $("#CanvasTag3Id").css('pointer-events', 'none');
                $("#CanvasTag4Id").css('pointer-events', 'none');
                $("#CanvasTag5Id").css('pointer-events', 'none');
                $("#CanvasTag6Id").css('pointer-events', 'none');
                $("#CanvasTag7Id").css('pointer-events', 'none');
            }
        });

        //=======================================================================
        // This function creates the pattern picker.
        //=======================================================================
        function CreatePatterns()
        {
            var fillColor = $("#FillColorManualColorTextBoxId").val();
            var patternColor = $("#PatternColorManualColorTextBoxId").val();

            if ( fillColor === null || fillColor === "" )
            {
                fillColor = "#99ccff";
            }

            if (patternColor === null || patternColor === "")
            {
                patternColor = "#ffffff";
            }

            FMDrawPatternPalette.MakePattern1("CanvasTag1Id");
            FMDrawPatternPalette.MakePattern2("CanvasTag2Id", fillColor, patternColor);
            FMDrawPatternPalette.MakePattern3("CanvasTag3Id", fillColor, patternColor);
            FMDrawPatternPalette.MakePattern4("CanvasTag4Id", fillColor, patternColor);
            FMDrawPatternPalette.MakePattern5("CanvasTag5Id", fillColor, patternColor);
            FMDrawPatternPalette.MakePattern6("CanvasTag6Id", fillColor, patternColor);
            FMDrawPatternPalette.MakePattern7("CanvasTag7Id", fillColor, patternColor);
        }

        //===================================================================
        // This function will handle the pattern select event.
        //===================================================================
        function PatternOnClick(patternNumberStr)
        {
            var canvasId = "CanvasTagSelectedId";
            var selectedPatternNumberHiddenField = document.getElementById(hiddenSelectedPatternFieldId);
            selectedPatternNumberHiddenField.value = patternNumberStr;
            currentSelectedPattern = patternNumberStr;

            var fillColor = $("#FillColorManualColorTextBoxId").val();
            var patternColor = $("#PatternColorManualColorTextBoxId").val();

            if (fillColor === null || fillColor === "")
            {
                fillColor = "#99ccff";
            }

            if (patternColor === null || patternColor === "")
            {
                patternColor = "#ffffff";
            }

            switch (patternNumberStr)
            {
            case '1':
                FMDrawPatternPalette.MakePattern1(canvasId);
                break;
            case '2':
                FMDrawPatternPalette.MakePattern2(canvasId, fillColor, patternColor);
                break;
            case '3':
                FMDrawPatternPalette.MakePattern3(canvasId, fillColor, patternColor);
                break;
            case '4':
                FMDrawPatternPalette.MakePattern4(canvasId, fillColor, patternColor);
                break;
            case '5':
                FMDrawPatternPalette.MakePattern5(canvasId, fillColor, patternColor);
                break;
            case '6':
                FMDrawPatternPalette.MakePattern6(canvasId, fillColor, patternColor);
                break;
            case '7':
                FMDrawPatternPalette.MakePattern7(canvasId, fillColor, patternColor);
                break;
            default:
                FMDrawPatternPalette.MakePattern1(canvasId);
                break;
            }
        }

        //==================================================================
        // This method handles the palette change event.
        //==================================================================
        function SetColorHexValue(color, manualColorTextboxId, manualColorSamplerTextboxId)
        {
            $(manualColorTextboxId).val(color.toHexString());
            $(manualColorSamplerTextboxId).css('background-color', color.toHexString());

            // Recreate the patterns with the new colors.
            CreatePatterns();
            PatternOnClick(currentSelectedPattern);

            if (manualColorTextboxId === "#FillColorManualColorTextBoxId")
            {
                var fillColorHexHiddenField = document.getElementById(hiddenFillColorFieldId);
                fillColorHexHiddenField.value = color.toHexString();
            }

            if ( manualColorTextboxId === "#PatternColorManualColorTextBoxId" )
            {
                var patternColorHexHiddenField = document.getElementById(hiddenPatternColorFieldId);
                patternColorHexHiddenField.value = color.toHexString();
            }
        }

        //==================================================================
        // This method handles the fill color manual change event.
        //==================================================================
        function FillColorManualColorChange()
        {
            ManualColorSetting("#FillColorManualColorTextBoxId", "#FillColorSpectrumTextBoxId", "#FillColorManualColorSamplerTextBoxId");
        }

        //==================================================================
        // This method handles the pattern color manual change event.
        //==================================================================
        function PatternColorManualColorChange()
        {
            ManualColorSetting("#PatternColorManualColorTextBoxId", "#PatternColorSpectrumTextBoxId", "#PatternColorManualColorSamplerTextBoxId");
        }

        //==================================================================
        // This method will take the manual color hex input and set the 
        // palette.
        //==================================================================
        function ManualColorSetting(manualColorTextboxId, spectrumTextboxId, manualColorSamplerTextboxId)
        {
            var newHexValue = $(manualColorTextboxId).val();

            if (newHexValue == null)
            {
                return;
            }

            newHexValue = newHexValue.trim();

            if (newHexValue.length < 7)
            {
                return;
            }

            if (newHexValue.length === 7 && ValidateColorHexString(newHexValue))
            {
                // The Reflow must be called if the spectrum was hidden in order
                // to move the selection dot to the correct position.
                $(spectrumTextboxId).spectrum('reflow');

                $(spectrumTextboxId).spectrum('set', newHexValue);
                $(manualColorSamplerTextboxId).css('background-color', newHexValue);

                // Recreate the patterns with the new colors.
                CreatePatterns();
                PatternOnClick(currentSelectedPattern);

                if (manualColorTextboxId === "#FillColorManualColorTextBoxId")
                {
                    var fillColorHexHiddenField = document.getElementById(hiddenFillColorFieldId);
                    fillColorHexHiddenField.value = newHexValue;
                }

                if (manualColorTextboxId === "#PatternColorManualColorTextBoxId")
                {
                    var patternColorHexHiddenField = document.getElementById(hiddenPatternColorFieldId);
                    patternColorHexHiddenField.value = newHexValue;
                }
            }
            else
            {
                var errMsg = 'Invalid HEX color value.';
                alert(errMsg);
            }
        }

        //========================================================================
        // This function will validate the user HEX color value input.  It must
        // be a HEX value of six characters and prefixed by a # symbol.
        //========================================================================
        function ValidateColorHexString(colorHexValueStr)
        {
            if (colorHexValueStr == null)
            {
                return false;
            }

            var rx = /#[a-f0-9]{6}/i;
            var found = colorHexValueStr.search(rx);

            if (found === -1)
            {
                return false;
            }

            return true;
        };
    </script>
</body>
</html>
