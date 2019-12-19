function downloadURI(element) {
    document.getElementById(element).click();
}
function downloadURL2(url) {
    alert('Hi');
    location = url;
}
function downloadURI(url) {
    var ifrm = document.getElementById("dliframe");
    ifrm.src = url;
}


