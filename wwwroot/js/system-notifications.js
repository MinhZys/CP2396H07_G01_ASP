/**
 * System Notification & Confirmation Helper
 * Requires: SweetAlert2
 */
const SystemNotification = {
    /**
     * Show distinct success/error messages if they exist
     * @param {string} successMsg - Success message text
     * @param {string} errorMsg - Error message text
     */
    showMessages: function (successMsg, errorMsg) {
        if (successMsg) {
            Swal.fire({
                icon: 'success',
                title: 'Success!',
                text: successMsg,
                showConfirmButton: false,
                timer: 1500
            });
        }

        if (errorMsg) {
            Swal.fire({
                icon: 'error',
                title: 'Error!',
                text: errorMsg
            });
        }
    },

    /**
     * Setup global event listeners for delete confirmations
     * Targets: 
     *  - forms with action containing "Delete"
     *  - buttons with onclick="return confirm(...)"
     */
    setupDeleteConfirmations: function () {
        // 1. Handle Forms
        document.querySelectorAll('form[asp-action="Delete"], form[action*="Delete"]').forEach(form => {
            // Remove existing listeners to avoid duplicates if called multiple times (optional)
            form.removeEventListener('submit', this.handleFormSubmit);
            form.addEventListener('submit', this.handleFormSubmit);
        });

        // 2. Handle simple buttons (convert return confirm to SweetAlert) OR buttons with class .btn-delete
        document.querySelectorAll('button[onclick*="confirm"], .btn-delete').forEach(btn => {
            btn.removeAttribute('onclick'); // For confirms
            // Remove existing to prevent duplicates
            btn.removeEventListener('click', this.handleButtonClick);
            btn.addEventListener('click', this.generateHandleButtonClick(this));
        });
    },

    // Fix: separating handler to bind context easier or use arrow function in setup
    generateHandleButtonClick: function (context) {
        return function (e) {
            context.handleButtonClick(e);
        }
    },

    handleFormSubmit: function (e) {
        e.preventDefault();
        const form = e.target;

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                form.submit();
            }
        });
    },

    handleButtonClick: function (e) {
        e.preventDefault();
        const btn = e.target.closest('button'); // Ensure we get the button if icon clicked
        const form = btn.closest('form');
        const href = btn.getAttribute('formaction') || btn.getAttribute('href');

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, do it!',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                if (form) form.submit();
                else if (href) window.location.href = href;
            }
        });
    }
};
