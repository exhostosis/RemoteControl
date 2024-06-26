import sendRequest from './sendRequest.js';
import initElements from './initElements.js';

document.addEventListener('DOMContentLoaded', async () => {
  let pageInPortraitMode;

  function setAttribute() {
    pageInPortraitMode = window.innerHeight > window.innerWidth;
    document.querySelector('meta[name=viewport]').setAttribute('content', `width=${window.innerWidth}, height=${window.innerHeight}, initial-scale=1.0, maximum-scale=1.0, user-scalable=0`);
  }

  setAttribute();

  window.addEventListener('resize', () => {
    if (((pageInPortraitMode === true)
        && (window.innerHeight < window.innerWidth))
        || ((pageInPortraitMode === false)
        && (window.innerHeight > window.innerWidth))) {
      setAttribute();
    }
  });

  const devices = JSON.parse(await (await sendRequest('audio', 'getdevices')).text());
  const volume = await (await sendRequest('audio', 'getvolume')).text();

  initElements(volume, devices);
});
