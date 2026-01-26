var ps_floater = {"id":"9bda5de7-b4b8-497b-aa0c-589b06a9a45c","width":400,"height":225,"mobile":true,"float":"left","mobile_height":147,"mobile_width":226,"cross":false};

let floater_context = window.self !== window.top ? window.parent.document : document;
floater_context.ps_floater = ps_floater;
let outStream_floater = document.createElement("script");
outStream_floater.type = "text/javascript";
outStream_floater.src = "https://app.playstream.media/js/floater/floater.js";
floater_context.getElementsByTagName("head")[0].appendChild(outStream_floater);
