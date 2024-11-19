// Download Report File in Cleint 
window.downloadFile = function (fileUrl) {
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


// Show Bootstrap Modol Class.
function showModol(ModolID) {
    var myModal = new bootstrap.Modal(document.getElementById(ModolID));
    myModal.show();
}


function showAcordion() {
    var collapseElementList = [].slice.call(document.querySelectorAll('.collapse'));
    collapseElementList.forEach(function (collapseEl) {
        var collapseInstance = bootstrap.Collapse.getInstance(collapseEl);
        if (!collapseInstance) {
            new bootstrap.Collapse(collapseEl);
        } else if (!collapseEl.classList.contains('show')) {
            collapseInstance.show();
        }
    });
}