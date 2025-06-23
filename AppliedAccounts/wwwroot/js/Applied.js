// Download Report File in Cleint

const screenHeight = screen.height;
document.documentElement.style.setProperty('--hight-screen', `${screenHeight}px`)


// Show Bootstrap Modol Class.
function showModol(ModalID) {
    var myModal = new bootstrap.Modal(document.getElementById(ModalID));
    myModal.show();
}

// Close Bootstrap Modal Class.
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

// Show Toast Notification
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

function printPage() {
    // Print the entire page
    window.print();
}
function printDiv(divId) {
    var content = document.getElementById(divId).innerHTML;
    var originalContent = document.body.innerHTML;
    document.body.innerHTML = content;
    window.print();
    document.body.innerHTML = originalContent;
    location.reload(); // Restore the page
}
function printPDF(pdfUrl) {
    // Open PDF File in New Browser Tab and print it
    var win = window.open(pdfUrl, '_blank');
    win.onload = function () {
        win.print();
    };
}



