import createVolumeBar from './slider.js';
import { createTouch, touchHide, touchShow } from './touch.js';
import createKeys from './keys.js';
import createTextInput from './text.js';

const Modes = {
    Audio: 'audio',
    Mouse: 'mouse',
    Keyboard: 'keyboard',
    Text: 'text',
};

let socket;

function sendRequest(method, param) {
    socket.send(`/${method}/${param}`);
}

function initElements(volume) {
    const slider = document.getElementById('slider');

    if (slider) {
        createVolumeBar(slider, 100, volume, true);
        slider.addEventListener('volumechanged', e => sendRequest(Modes.Audio, e.value));
    }

    const touch = document.getElementById('touch');

    if (touch) {
        createTouch(touch);
        touch.addEventListener('touchpanelclick', e => sendRequest(Modes.Mouse, e.touches));
        touch.addEventListener('touchpanelmove', e => { if (e.position) sendRequest(Modes.Mouse, e.position); });
        touch.addEventListener('touchpanelscroll', e => { sendRequest(Modes.Mouse, e.direction); });
        touch.addEventListener('touchpaneldrag', e => { sendRequest(Modes.Mouse, e.start ? 'dragstart' : 'dragstop'); });
    }

    const buttons = document.getElementById('buttons');

    if (buttons) {
        createKeys(buttons);
        buttons.addEventListener('keyclick', e => sendRequest(Modes.Keyboard, e.value));
    }

    const textInput = document.getElementById('text-input');

    if (textInput) {
        createTextInput(textInput);
        textInput.addEventListener('textinput', e => sendRequest(Modes.Text, e.text));
        textInput.addEventListener('focus', () => touchHide());
        textInput.addEventListener('blur', e => {
            e.target.value = '';
            touchShow();
        });
    }

    const loader = document.getElementById('loader');

    if (loader) {
        document.getElementById('loader').style.display = 'none';
    }
}

function messageReceived(e) {
    initElements(e.data);
}

document.addEventListener('DOMContentLoaded', () => {
    socket = new WebSocket(`ws://${window.location.hostname}:${(Number)(window.location.port) + 1}`);
    socket.onmessage = messageReceived;
    socket.onopen = () => {
        sendRequest(Modes.Audio, 'init');
    };
});

document.addEventListener('beforeunload', () => {
    socket.Close();
});
