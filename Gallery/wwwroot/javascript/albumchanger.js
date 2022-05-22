function KeyListen() {
    document.addEventListener('keydown', logKey);

    function logKey(e) {
        if (e.keyCode == '65') {
            var log = document.getElementsByClassName('rz-paginator-prev')[0];
            log.click();
        }
        if (e.keyCode == '68') {
            var log = document.getElementsByClassName('rz-paginator-next')[0];
            log.click();
        }
    }

}
