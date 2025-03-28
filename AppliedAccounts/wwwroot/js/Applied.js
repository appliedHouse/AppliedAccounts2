// Download Report File in Cleint


const screenHeight = screen.height;
document.documentElement.style.setProperty('--hight-screen', `${screenHeight}px`)


window.downloadFile = function (fileUrl) {
    const link = document.createElement('a');
    link.href = fileUrl;
    link.download = fileUrl.split('/').pop(); // Set download attribute to the filename
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function downloadPDF(fileName, byteArray) {
    const blob = new Blob([byteArray], { type: 'application/pdf' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
    URL.revokeObjectURL(link.href); // Clean up the object URL
}

// display a PDF File in Browser
window.displayPDF = function (fileUrl) {
    const link = document.createElement('a');
    link.href = fileUrl;
    link.target = "_blank"
    link.click();
}


// Show Bootstrap Modol Class.
function showModol(ModalID) {
    var myModal = new bootstrap.Modal(document.getElementById(ModalID));
    myModal.show();
}

function closeModal(ModalID) {
    var modalElement = document.getElementById(ModalID);
    if (modalElement) {
        var modalInstance = bootstrap.Modal.getInstance(modalElement);

        if (modalInstance) {
            modalInstance.hide();
        } else {
            modalInstance = new bootstrap.Modal(modalElement);
            modalInstance.hide();
        }

        // Manually remove the backdrop if it persists
        var backdrop = document.querySelector('.modal-backdrop');
        if (backdrop) {
            backdrop.remove();
        }
    } else {
        console.error("Modal not found: " + ModalID);
    }
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

window.showBlazorToast = () => {
    var toastEl = document.getElementById('blazorToast');
    toastEl.style.display = "block"; // Ensure it's visible
    var toast = new bootstrap.Toast(toastEl, { delay: 5000 }); // Auto-hide after 5 seconds
    toast.show();
};

// Trigger for click of Upload Excel file in Import Sale Data
window.triggerFileUpload = function () {
    document.getElementById("inputFile").click();
};


// Table List Height Auto Adjust
function adjustTableHeight() {
    const tableContainer = document.getElementById("tableContainer");
    if (tableContainer != null) {
        const windowHeight = window.innerHeight;
        const containerTop = tableContainer.getBoundingClientRect().top;
        const maxHeight = windowHeight - containerTop - 20; // 20px less than bottom
        tableContainer.style.maxHeight = maxHeight + "px";
    }
}

// Adjust height on page load and window resize
window.addEventListener("load", adjustTableHeight);
window.addEventListener("resize", adjustTableHeight);