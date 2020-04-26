﻿using HtmlParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RemoteControlTests
{
    [TestClass]
    public class HtmlParserTests
    {
        readonly string input = @"
<html>

<head>
    <title>
        Громкость
    </title>
    <meta name=""viewport""
          content=""width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no, minimum-scale=1""
          charset=""utf8"" />

    <script type = ""text/javascript"" >
        let Events = {
            Move: ""touchmove"",
            Touch: ""touchclick"",
            Click: ""buttonclick"",
            Drag: ""touchdrag"",
            Scroll: ""touchscroll"",
            ValueChanged: ""slidervaluechanged""
        }
    let Modes = {
            Audio: ""audio"",
            Mouse: ""mouse"",
            Keyboard: ""keyboard"",
            Text: ""text"",
            Wheel: ""wheel""
        }

        let SLIDERSCALE = 0;
    const HANDLECLASS = ""volume-handle"";
    const COLOREDCLASS = ""volume-colored"";
    const BARID = ""volume-bar"";
    const NUMERICID = ""volume-numeric"";

    let volumeBar;
    let volumeBarColored;
    let volumeBarHandle;
    let sliderElement;

    let volumeBarNumericElement;

    let volumeNumericValue = 0;

    let numericEnabled = false;

    let drag = false;

    let changeEvent = new Event(Events.ValueChanged);

    function setVolumeBar(value)
    {
        let correction = volumeBarHandle.offsetWidth / 2;

        let pixelValue = value - volumeBar.offsetLeft;
        pixelValue = pixelValue <= volumeBar.offsetWidth - correction ? pixelValue : volumeBar.offsetWidth - correction;
        pixelValue = pixelValue >= correction ? pixelValue : correction;

        volumeBarHandle.style.marginLeft = pixelValue - correction;
        volumeBarColored.style.width = pixelValue;

        return pixelValue;
    }

    function setVolumeBarNumeric(value)
    {
        volumeBarNumericElement.innerText = value;
    }

    function toPixels(value)
    {
        return (volumeBar.offsetWidth - volumeBarHandle.offsetWidth) / SLIDERSCALE * value + (volumeBarHandle.offsetWidth / 2) + volumeBar.offsetLeft;
    }

    function toNumeric(value)
    {
        return Math.round((value - volumeBarHandle.offsetWidth / 2) * SLIDERSCALE / (volumeBar.offsetWidth - volumeBarHandle.offsetWidth));
    }

    function checkSendRequest(value)
    {
        let newNumeric = toNumeric(setVolumeBar(value));

        if (newNumeric != volumeNumericValue)
        {
            volumeNumericValue = newNumeric;

            changeEvent.value = newNumeric;
            sliderElement.dispatchEvent(changeEvent);
            if (numericEnabled)
            {
                setVolumeBarNumeric(newNumeric);
            }
        }
    }

    function wheel(e)
    {
        checkSendRequest(toPixels(e > 0 ? volumeNumericValue + 2 : volumeNumericValue - 2));
    }

    function init(initialValue)
    {
        volumeNumericValue = Number(initialValue);

        if (!Number.isNaN(volumeNumericValue))
        {
            setVolumeBar(toPixels(initialValue));

            if (numericEnabled)
            {
                setVolumeBarNumeric(initialValue);
            }
        }
    }

    function createVolumeBar(slider, scale, initValue = 0, numeric = false)
    {
        SLIDERSCALE = scale;
        numericEnabled = numeric;

        sliderElement = slider;

        var html = `< div id = '${BARID}' ></ div >`;
        html = numericEnabled ? `< div id = '${NUMERICID}' > 0 </ div >\n` +html : html;

        sliderElement.innerHTML = html;

        volumeBar = sliderElement.querySelector(`#${BARID}`);

            if (numeric)
        {
            volumeBarNumericElement = sliderElement.querySelector(`#${NUMERICID}`);
            }

        volumeBar.innerHTML = `
            < div class=""${COLOREDCLASS}""></div>
            <div class=""${HANDLECLASS}""></div>
        `;

            volumeBarHandle = volumeBar.querySelector(`.${HANDLECLASS
}`);
            volumeBarColored = volumeBar.querySelector(`.${COLOREDCLASS}`);

            volumeBarHandle.addEventListener(""touchmove"", e => { e.preventDefault(); checkSendRequest(e.targetTouches[0].clientX); });
            volumeBar.addEventListener(""click"", e => { e.preventDefault(); checkSendRequest(e.clientX); });
            volumeBar.addEventListener(""wheel"", e => { e.preventDefault(); wheel(e.wheelDeltaY) });

            volumeBarHandle.addEventListener(""mousedown"", () => drag = true);
            volumeBarHandle.addEventListener(""mouseup"", () => drag = false);
            volumeBarHandle.addEventListener(""mousemove"", e => { e.preventDefault(); if (drag) checkSendRequest(e.clientX); });

            init(initValue);
        }

        let keyRepeadIntervalId;
let keyRepeatTimerId;

let clickEvent = new Event(Events.Click);

function start(value)
{
    clickEvent.value = value;

    keyRepeatTimerId = setTimeout(() => {
        keyRepeadIntervalId = setInterval(() => {
            buttons.dispatchEvent(clickEvent);
        }, 100)
            }, 1000);
}

function end()
{
    clearInterval(keyRepeatTimerId);
    clearTimeout(keyRepeadIntervalId);
}

function createKeys(buttons)
{

    let[buttonBack, buttonPause, buttonForth] = new DOMParser().parseFromString(`

    < button > &#60;&#60;</button>
            < button > &#10074;&#10074;</button>
            < button > &#62;&#62;</button>`, 'text/html').body.getElementsByTagName(""button"");

            buttons.append(buttonBack, buttonPause, buttonForth);

    buttonBack.addEventListener(""touchstart"", () => start(""back""));
    buttonBack.addEventListener(""touchend"", end);
    buttonBack.addEventListener(""click"", () => { clickEvent.value = ""back""; buttons.dispatchEvent(clickEvent); });
    buttonBack.addEventListener(""touchcancel"", end);

    buttonForth.addEventListener(""touchstart"", () => start(""forth""));
    buttonForth.addEventListener(""touchend"", end);
    buttonForth.addEventListener(""click"", () => { clickEvent.value = ""forth""; buttons.dispatchEvent(clickEvent); });
    buttonForth.addEventListener(""touchcancel"", end);

    buttonPause.addEventListener(""click"", e => { clickEvent.value = ""pause""; buttons.dispatchEvent(clickEvent); });
}

async function sendRequest(mode, value)
{
    return (await fetch(`/ api ? mode =${ mode}
    &value =${ value}`)).text();
}

function processText(e)
{
    if (e.key === ""Enter"")
    {
        if (e.target.value)
        {
            sendRequest(Modes.Text, encodeURIComponent(e.target.value));
        }
        e.target.blur();
    }
}

document.addEventListener(""DOMContentLoaded"", async() => {
            let slider = document.getElementById(""slider"");
createVolumeBar(slider, 100, await sendRequest(Modes.Audio, ""init""), true);
            slider.addEventListener(Events.ValueChanged, e => sendRequest(Modes.Audio, e.value));

            let touch = document.getElementById(""touch"");
createTouch(touch);
touch.addEventListener(Events.Touch, e => sendRequest(Modes.Mouse, e.touches));
            touch.addEventListener(Events.Move, e => sendRequest(Modes.Mouse, e.position));
            touch.addEventListener(Events.Scroll, e => sendRequest(Modes.Wheel, e.direction));
            touch.addEventListener(Events.Drag, e => sendRequest(Modes.Mouse, `drag${ e.start ? 'start' : 'stop'}`));

            let buttons = document.getElementById(""buttons"");
createKeys(buttons);
buttons.addEventListener(Events.Click, e => sendRequest(Modes.Keyboard, e.value));

            let prev = """";
let prevBorder = """";

document.getElementById(""text-input"").addEventListener(""keyup"", processText);
document.getElementById(""text-input"").addEventListener(""focus"", () => {
    prev = touch.style.display;
    prevBorder = touch.style.border;
    touch.style.border = ""none"";
    touch.style.display = ""none"";
})
            document.getElementById(""text-input"").addEventListener(""blur"", e => {
    e.target.value = """";
    touch.style.display = prev;
    setTimeout(() => touch.style.border = prevBorder, 200);
});

            document.getElementById(""loader"").style.display = ""none"";
        });

    </script>

    <style type = ""text/css"" >
        body {
            background: #2a2a2a;
        }

        #outer-container {
            display: block;
            height: 100%;
        }

        #volume-bar {
            display: block;
            margin: 0 auto;
            height: 60px;
            width: 100%;
            background: #555;
            border-radius: 15px;
            margin-bottom: 10px;
        }

            #volume-bar .volume-colored {
                height: 60px;
                position: absolute;
                background: #2ecc71;
                border: none;
                border-radius: 15px;
                outline: none;
            }

            #volume-bar .volume-handle {
                width: 70px;
                height: 70px;
                border-radius: 20px;
                background: #FFF;
                position: absolute;
                margin-top: -5px;
                cursor: pointer;
                outline: none;
            }

        button {
            cursor: pointer;
            font-weight: bold;
            text-align: center;
            color: #fff;
            border-radius: 30px;
            background-color: #538fbe;
            border: 1px solid #2d6898;
            text-shadow: 0px 0px 6px rgba(0,0,0,.8);
display: block;
            width: 32%;
            float: left;
            padding: 5px;
            padding-top: 8px;
            margin: 2px;
            text-decoration: none;
            font-size: 50pt;
            -webkit-touch-callout: none;
            -webkit-user-select: none;
            -moz-user-select: none;
            -ms-user-select: none;
            user-select: none;
            line-height: 0px;
            height: 115px;
            margin-bottom: 12px;
        }

            button:active {
                background-color: #73afde;
                box-shadow: 0 3px 0 #2b638f;
                transform: translateY(5px);
            }

        #volume-numeric {
            text-align: center;
            font-size: 70pt;
            color: white;
        }
    </style>

    <link rel = ""shortcut icon"" href=""favicon.ico"" type=""image/x-icon"">

</head>

<body>
    <div id = ""outer-container"" >
        < div id=""slider""></div>
        <div id = ""buttons"" >asdf</div>
    </div>
</body>

</html>
";

        [TestMethod]
        public void PaserTest()
        {
            var a1 = Parser.GetTree(input);

            Assert.IsNotNull(a1);
            Assert.AreEqual(a1.Count, 11);
        }
    }
}
