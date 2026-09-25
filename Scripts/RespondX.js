/*
    RespondX shared client behaviour: modal dialogs, toast notifications,
    a top loading bar, and smooth partial postbacks (UpdatePanel) for CRUD pages.
*/
(function (window, document, $) {
    'use strict';

    // ========== Modal dialogs (Bootstrap-compatible $.fn.modal subset) ==========
    if ($ && !$.fn.modal) {
        $.fn.modal = function (action) {
            return this.each(function () {
                var modal = this;
                var shouldShow = action === 'show' || (action === 'toggle' && !modal.classList.contains('show'));

                if (shouldShow) {
                    modal.__returnFocus = document.activeElement;
                    modal.style.display = 'block';
                    modal.classList.add('show');
                    modal.removeAttribute('aria-hidden');
                    modal.setAttribute('aria-modal', 'true');
                    document.body.classList.add('modal-open');
                    var firstField = modal.querySelector('input:not([type="hidden"]):not([disabled]), textarea, select');
                    if (firstField) firstField.focus();
                } else {
                    modal.classList.remove('show');
                    modal.style.display = 'none';
                    modal.setAttribute('aria-hidden', 'true');
                    modal.removeAttribute('aria-modal');
                    syncModalState();
                    if (modal.__returnFocus && modal.__returnFocus.focus && document.body.contains(modal.__returnFocus))
                        modal.__returnFocus.focus();
                }
            });
        };

        $(document).on('click', '[data-dismiss="modal"]', function (event) {
            event.preventDefault();
            $(this).closest('.modal').modal('hide');
        });
        $(document).on('mousedown', '.modal', function (event) {
            if (event.target === this) $(this).modal('hide');
        });
        $(document).on('keydown', function (event) {
            if (event.key === 'Escape') $('.modal.show').last().modal('hide');
        });
    }

    function syncModalState() {
        if (!document.querySelector('.modal.show'))
            document.body.classList.remove('modal-open');
    }

    // ========== Toast notifications ==========
    var toastIcons = {
        success: 'fa-check-circle',
        error: 'fa-triangle-exclamation',
        danger: 'fa-triangle-exclamation',
        warning: 'fa-exclamation-triangle',
        info: 'fa-info-circle'
    };

    function getToastContainer() {
        var container = document.getElementById('rx-toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'rx-toast-container';
            container.setAttribute('role', 'status');
            container.setAttribute('aria-live', 'polite');
            document.body.appendChild(container);
        }
        return container;
    }

    function showNotification(message, type) {
        if (!message) return;
        type = toastIcons.hasOwnProperty(type) ? type : 'info';
        if (type === 'danger') type = 'error';

        var toast = document.createElement('div');
        toast.className = 'rx-toast rx-toast-' + type;

        var icon = document.createElement('i');
        icon.className = 'fas ' + toastIcons[type];
        icon.setAttribute('aria-hidden', 'true');

        var text = document.createElement('span');
        text.className = 'rx-toast-text';
        text.textContent = message;

        var close = document.createElement('button');
        close.type = 'button';
        close.className = 'rx-toast-close';
        close.setAttribute('aria-label', 'Dismiss');
        close.innerHTML = '&times;';
        close.addEventListener('click', function () { dismiss(); });

        toast.appendChild(icon);
        toast.appendChild(text);
        toast.appendChild(close);
        getToastContainer().appendChild(toast);

        // Next frame so the CSS transition runs.
        window.requestAnimationFrame(function () { toast.classList.add('show'); });

        var timer = window.setTimeout(dismiss, type === 'error' ? 7000 : 4000);
        function dismiss() {
            window.clearTimeout(timer);
            toast.classList.remove('show');
            window.setTimeout(function () {
                if (toast.parentNode) toast.parentNode.removeChild(toast);
            }, 250);
        }
    }

    // ========== Top loading bar ==========
    var progressTimer = null;

    function startProgress() {
        var bar = document.getElementById('rx-progress');
        if (!bar) return;
        window.clearTimeout(progressTimer);
        bar.classList.remove('done');
        bar.classList.add('active');
        document.documentElement.classList.add('rx-busy');
    }

    function stopProgress() {
        var bar = document.getElementById('rx-progress');
        document.documentElement.classList.remove('rx-busy');
        if (!bar) return;
        bar.classList.add('done');
        progressTimer = window.setTimeout(function () {
            bar.classList.remove('active', 'done');
        }, 300);
    }

    // Converts server alert panels (.alert-success / .alert-danger) inside an
    // updated region into toasts, so feedback is visible even when the page
    // is scrolled down to the item that was edited.
    function announceAlerts(root) {
        (root || document).querySelectorAll('.alert.alert-success, .alert.alert-danger').forEach(function (alert) {
            if (alert.closest('.modal') || alert.getAttribute('data-announced')) return;
            var message = (alert.textContent || '').trim();
            if (!message) return;
            alert.setAttribute('data-announced', 'true');
            if (alert.classList.contains('alert-success')) {
                // Success is shown once, as a toast; errors stay inline next to the form as well.
                alert.style.display = 'none';
                showNotification(message, 'success');
            } else {
                showNotification(message, 'error');
            }
        });
    }

    // ========== Partial postbacks (ASP.NET AJAX UpdatePanel) ==========
    function initAjax() {
        if (!window.Sys || !Sys.WebForms || !Sys.WebForms.PageRequestManager) return;

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var disabledElement = null;

        prm.add_beginRequest(function (sender, args) {
            startProgress();
            var element = args.get_postBackElement && args.get_postBackElement();
            if (element && (element.tagName === 'INPUT' || element.tagName === 'BUTTON' || element.tagName === 'A')) {
                disabledElement = element;
                element.classList.add('is-loading');
                element.setAttribute('aria-busy', 'true');
            }
        });

        prm.add_endRequest(function (sender, args) {
            stopProgress();
            if (disabledElement) {
                disabledElement.classList.remove('is-loading');
                disabledElement.removeAttribute('aria-busy');
                disabledElement = null;
            }
            syncModalState();

            if (args.get_error && args.get_error()) {
                args.set_errorHandled(true);
                showNotification('Something went wrong while saving. Please try again.', 'error');
            }
        });

        prm.add_pageLoaded(function (sender, args) {
            var panels = args.get_panelsUpdated ? args.get_panelsUpdated() : [];
            for (var i = 0; i < panels.length; i++) announceAlerts(panels[i]);
        });
    }

    // Full (non-AJAX) form submits and link navigations also show the loading bar.
    // Checked after the event finishes so submits cancelled by client-side validation are ignored.
    document.addEventListener('submit', function (event) {
        window.setTimeout(function () {
            if (!event.defaultPrevented) startProgress();
        }, 0);
    });
    window.addEventListener('pageshow', function () { stopProgress(); });

    document.addEventListener('DOMContentLoaded', function () {
        announceAlerts(document);
    });

    window.showNotification = showNotification;
    window.RespondX = {
        initAjax: initAjax,
        notify: showNotification,
        startProgress: startProgress,
        stopProgress: stopProgress
    };
})(window, document, window.jQuery);
