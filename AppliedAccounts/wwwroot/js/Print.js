
function printer(rptBytes64) {
    // This function will open the printer dialoge box
    console.log("Report Array:", rptBytes64);
    const byteCharacters = atob(rptBytes64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: 'application/pdf' });
    const blobUrl = URL.createObjectURL(blob);
    const iframe = document.createElement('iframe');
    iframe.style.display = 'none';
    iframe.src = blobUrl;
    document.body.appendChild(iframe);
    iframe.onload = function () {
        setTimeout(() => {
            iframe.contentWindow.focus();
            iframe.contentWindow.print();
        }, 500); // short delay to ensure PDF is rendered
    };
}


// Download from binary Data file PDF, Excel, Word, HTML, Image
function downloadPDF(fileName, byteArray) {
    const blob = new Blob([byteArray], { type: 'application/pdf' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
    URL.revokeObjectURL(link.href); // Clean up the object URL
}

function downloadFile(fileName, byteArray, mimeType) {
    const blob = new Blob([byteArray], { type: mimeType });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
    URL.revokeObjectURL(link.href); // Clean up the object URL
}


function DisplayPDF(byteArray) {
    const bytes = new Uint8Array(byteArray);
    const blob = new Blob([bytes], { type: "application/pdf" });

    // Create a blob URL
    const blobUrl = URL.createObjectURL(blob);

    // Open in new tab
    window.open(blobUrl, '_blank');
}


// Download a  existed file PDF, Excel, Word, HTML, Image
window.OpenLink = function (fileUrl) {
    const link = document.createElement('a');
    link.href = fileUrl;
    link.download = fileUrl.split('/').pop(); // Set download attribute to the filename
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

// display a PDF File in Browser
window.displayPDF = function (fileUrl) {
    const link = document.createElement('a');
    link.href = fileUrl;
    link.target = "_blank"
    link.click();
}