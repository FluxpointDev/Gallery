function writeToClipboardOnPermission(text) {
    try {
         writeToClipboard(text);
    } catch (error) {
        console.error(error);
        navigator.permissions.query({ name: 'clipboard-write' })
            .then(
                result => {
                    if (result.state == 'granted' || result.state == 'prompt') {
                        writeToClipboard(text);
                    }
                    else {
                        console.log("Don't have permissions to use clipboard", result.state);
                    }
                }
            )
            .catch(
                err => {
                    console.log("Error! Reqeusting permission", err)
                }
            )
    }
}

function writeToClipboard(text) {
    navigator.clipboard.writeText(text).then(
        result => {
            console.log("Successfully copied to clipboard", result)
        }
    )
        .catch(
            err => {
                console.log("Error! could not copy text", err)
            })
}