// ========== Site-wide JavaScript ==========

// Document Ready
$(document).ready(function () {
    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();

    // Initialize popovers
    $('[data-toggle="popover"]').popover();

    // Auto-hide alerts after 5 seconds
    $('.alert:not(.alert-permanent)').delay(5000).fadeOut(500);

    // Handle AJAX loading spinner
    $(document).ajaxStart(function () {
        $('#loading-spinner').show();
    }).ajaxStop(function () {
        $('#loading-spinner').hide();
    });

    // Initialize date pickers
    $('.datepicker').datepicker({
        format: 'yyyy-mm-dd',
        autoclose: true,
        todayHighlight: true
    });

    // Initialize select2 for better dropdowns
    $('.select2').select2({
        placeholder: 'Select an option',
        allowClear: true
    });

    // Confirm delete actions
    $('.delete-confirm').click(function (e) {
        if (!confirm('Are you sure you want to delete this item? This action cannot be undone.')) {
            e.preventDefault();
            return false;
        }
    });

    // Toggle password visibility
    $('.toggle-password').click(function () {
        var input = $($(this).data('target'));
        if (input.attr('type') === 'password') {
            input.attr('type', 'text');
            $(this).find('i').removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            input.attr('type', 'password');
            $(this).find('i').removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    // Character counter for textareas
    $('.char-counter').each(function () {
        var maxLength = $(this).attr('maxlength');
        var counter = $('<span class="char-counter-display">' + maxLength + ' characters remaining</span>');
        $(this).after(counter);

        $(this).on('input', function () {
            var remaining = maxLength - $(this).val().length;
            counter.text(remaining + ' characters remaining');
            if (remaining < 10) {
                counter.css('color', '#fc8181');
            } else {
                counter.css('color', '#718096');
            }
        });
    });

    // Auto-resize textareas
    $('textarea.auto-resize').each(function () {
        this.style.height = 'auto';
        this.style.height = this.scrollHeight + 'px';
    }).on('input', function () {
        this.style.height = 'auto';
        this.style.height = this.scrollHeight + 'px';
    });

    // Handle form submission with loading state
    $('form[data-loading]').submit(function () {
        var btn = $(this).find('button[type="submit"]');
        var originalText = btn.html();
        btn.html('<i class="fas fa-spinner fa-spin"></i> Loading...');
        btn.prop('disabled', true);

        setTimeout(function () {
            btn.html(originalText);
            btn.prop('disabled', false);
        }, 3000);
    });

    // Handle keyboard shortcuts
    $(document).keydown(function (e) {
        // Ctrl + S to save
        if (e.ctrlKey && e.key === 's') {
            e.preventDefault();
            $('form[data-shortcut="save"]').submit();
        }
        // Escape to close modals
        if (e.key === 'Escape') {
            $('.modal.show').modal('hide');
        }
    });
});

// ========== Utility Functions ==========

// Format date
function formatDate(date) {
    if (!date) return '';
    var d = new Date(date);
    return d.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Format time ago
function timeAgo(date) {
    if (!date) return '';
    var now = new Date();
    var past = new Date(date);
    var diff = Math.floor((now - past) / 1000);

    if (diff < 60) return 'Just now';
    if (diff < 3600) return Math.floor(diff / 60) + ' minutes ago';
    if (diff < 86400) return Math.floor(diff / 3600) + ' hours ago';
    if (diff < 604800) return Math.floor(diff / 86400) + ' days ago';
    return formatDate(date);
}

// Truncate text
function truncateText(text, maxLength) {
    if (text.length <= maxLength) return text;
    return text.substring(0, maxLength) + '...';
}

// Generate random ID
function generateId() {
    return Math.random().toString(36).substr(2, 9);
}

// Get URL parameters
function getUrlParams() {
    var params = {};
    window.location.search.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (str, key, value) {
        params[key] = decodeURIComponent(value);
    });
    return params;
}

// Validate email
function isValidEmail(email) {
    var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

// Validate phone number
function isValidPhone(phone) {
    var re = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/;
    return re.test(phone);
}

// Show notification
function showNotification(message, type) {
    var types = {
        success: 'alert-success',
        error: 'alert-danger',
        warning: 'alert-warning',
        info: 'alert-info'
    };

    var alertClass = types[type] || 'alert-info';
    var html = '<div class="alert ' + alertClass + ' alert-dismissible fade show" role="alert">' +
        message +
        '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
        '</div>';

    var container = $('#notification-container');
    if (!container.length) {
        container = $('<div id="notification-container" style="position:fixed;top:80px;right:20px;z-index:9999;max-width:400px;"></div>');
        $('body').append(container);
    }

    container.prepend(html);

    setTimeout(function () {
        container.find('.alert').fadeOut(500, function () {
            $(this).remove();
        });
    }, 5000);
}

// ========== AJAX Helpers ==========

function ajaxRequest(url, method, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        method: method || 'GET',
        data: data,
        dataType: 'json',
        beforeSend: function () {
            $('#loading-spinner').show();
        },
        success: function (response) {
            if (response.success) {
                if (successCallback) successCallback(response);
                showNotification(response.message || 'Operation successful', 'success');
            } else {
                if (errorCallback) errorCallback(response);
                showNotification(response.message || 'An error occurred', 'error');
            }
        },
        error: function (xhr, status, error) {
            var message = 'An error occurred. Please try again.';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                message = xhr.responseJSON.message;
            }
            if (errorCallback) errorCallback({ message: message });
            showNotification(message, 'error');
        },
        complete: function () {
            $('#loading-spinner').hide();
        }
    });
}

// ========== Form Validation ==========

function validateForm(formId) {
    var isValid = true;
    var form = $('#' + formId);

    form.find('.required').each(function () {
        var input = $(this);
        var value = input.val().trim();

        if (!value) {
            input.addClass('is-invalid');
            isValid = false;
        } else {
            input.removeClass('is-invalid');
        }
    });

    form.find('.email-validate').each(function () {
        var input = $(this);
        var value = input.val().trim();

        if (value && !isValidEmail(value)) {
            input.addClass('is-invalid');
            isValid = false;
        } else {
            input.removeClass('is-invalid');
        }
    });

    form.find('.phone-validate').each(function () {
        var input = $(this);
        var value = input.val().trim();

        if (value && !isValidPhone(value)) {
            input.addClass('is-invalid');
            isValid = false;
        } else {
            input.removeClass('is-invalid');
        }
    });

    return isValid;
}

// ========== Export for use in other files ==========
window.RespondX = {
    formatDate: formatDate,
    timeAgo: timeAgo,
    truncateText: truncateText,
    generateId: generateId,
    getUrlParams: getUrlParams,
    isValidEmail: isValidEmail,
    isValidPhone: isValidPhone,
    showNotification: showNotification,
    ajaxRequest: ajaxRequest,
    validateForm: validateForm
};