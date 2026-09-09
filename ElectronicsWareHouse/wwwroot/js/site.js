/**
 * Electronics Warehouse - Client Application Engine
 * Handles Themes, Collapsible Navigation, Mobile Drawer,
 * Dynamic Calculations, Delete Confirmation Modals, and Toasts.
 */

(function () {
    'use strict';

    // ==========================================
    // 1. THEME ENGINE (DARK / LIGHT MODE)
    // ==========================================
    const THEME_STORAGE_KEY = 'ew-theme-preference';

    function getPreferredTheme() {
        const storedTheme = localStorage.getItem(THEME_STORAGE_KEY);
        if (storedTheme) {
            return storedTheme;
        }
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    function applyTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        document.documentElement.setAttribute('data-bs-theme', theme);
        localStorage.setItem(THEME_STORAGE_KEY, theme);

        // Notify charts and dynamic elements of theme switch
        window.dispatchEvent(new CustomEvent('ewThemeChanged', { detail: { theme } }));
    }

    // Initialize Theme immediately
    applyTheme(getPreferredTheme());

    document.addEventListener('DOMContentLoaded', () => {
        const themeToggleBtn = document.getElementById('themeToggleBtn');
        if (themeToggleBtn) {
            themeToggleBtn.addEventListener('click', () => {
                const currentTheme = document.documentElement.getAttribute('data-theme') || 'light';
                const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
                applyTheme(newTheme);
            });
        }

        // ==========================================
        // 2. SIDEBAR COLLAPSE & MOBILE DRAWER
        // ==========================================
        const SIDEBAR_COLLAPSED_KEY = 'ew-sidebar-collapsed';
        const sidebarToggleBtn = document.getElementById('sidebarToggleBtn');
        const mobileMenuBtn = document.getElementById('mobileMenuBtn');
        const sidebarBackdrop = document.getElementById('sidebarBackdrop');

        // Restore desktop collapse state
        if (localStorage.getItem(SIDEBAR_COLLAPSED_KEY) === 'true') {
            document.body.classList.add('sidebar-collapsed');
        }

        if (sidebarToggleBtn) {
            sidebarToggleBtn.addEventListener('click', () => {
                document.body.classList.toggle('sidebar-collapsed');
                const isCollapsed = document.body.classList.contains('sidebar-collapsed');
                localStorage.setItem(SIDEBAR_COLLAPSED_KEY, isCollapsed);
            });
        }

        if (mobileMenuBtn) {
            mobileMenuBtn.addEventListener('click', () => {
                document.body.classList.toggle('sidebar-mobile-open');
            });
        }

        if (sidebarBackdrop) {
            sidebarBackdrop.addEventListener('click', () => {
                document.body.classList.remove('sidebar-mobile-open');
            });
        }

        // ==========================================
        // 3. TOAST NOTIFICATIONS
        // ==========================================
        const toastContainer = document.getElementById('ewToastContainer');

        window.showToast = function (type, title, message) {
            if (!toastContainer) return;

            const toast = document.createElement('div');
            toast.className = 'ew-toast';

            let iconClass = 'bi-info-circle-fill text-primary';
            if (type === 'success') iconClass = 'bi-check-circle-fill text-success';
            if (type === 'error' || type === 'danger') iconClass = 'bi-x-circle-fill text-danger';
            if (type === 'warning') iconClass = 'bi-exclamation-triangle-fill text-warning';

            toast.innerHTML = `
                <i class="bi ${iconClass} toast-icon"></i>
                <div class="toast-content">
                    <div class="toast-title">${title}</div>
                    <p class="toast-message">${message}</p>
                </div>
                <button type="button" class="toast-close" aria-label="Close">&times;</button>
                <div class="toast-progress"></div>
            `;

            toastContainer.appendChild(toast);

            const closeBtn = toast.querySelector('.toast-close');
            closeBtn.addEventListener('click', () => toast.remove());

            // Auto dismiss after 4 seconds
            setTimeout(() => {
                toast.style.transition = 'opacity 0.3s, transform 0.3s';
                toast.style.opacity = '0';
                toast.style.transform = 'translateX(100%)';
                setTimeout(() => toast.remove(), 300);
            }, 4000);
        };

        // Check for server-side TempData flash messages
        const flashData = document.getElementById('ewFlashMessages');
        if (flashData) {
            const success = flashData.getAttribute('data-success');
            const error = flashData.getAttribute('data-error');
            const warning = flashData.getAttribute('data-warning');

            if (success) showToast('success', 'Success', success);
            if (error) showToast('danger', 'Notice', error);
            if (warning) showToast('warning', 'Warning', warning);
        }

        // ==========================================
        // 4. GLOBAL SEARCH (Ctrl + K shortcut)
        // ==========================================
        const globalSearch = document.getElementById('globalSearchInput');
        document.addEventListener('keydown', (e) => {
            if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
                e.preventDefault();
                if (globalSearch) {
                    globalSearch.focus();
                }
            }
        });

        // ==========================================
        // 5. DELETE CONFIRMATION MODAL INTERCEPTOR
        // ==========================================
        const confirmDeleteModal = document.getElementById('ewDeleteModal');
        if (confirmDeleteModal) {
            const deleteForm = document.getElementById('ewDeleteModalForm');
            const deleteItemTitle = document.getElementById('ewDeleteItemTitle');
            const deleteItemDetails = document.getElementById('ewDeleteItemDetails');

            document.querySelectorAll('[data-ew-delete-trigger]').forEach(btn => {
                btn.addEventListener('click', (e) => {
                    e.preventDefault();
                    const actionUrl = btn.getAttribute('data-action-url');
                    const title = btn.getAttribute('data-item-title') || 'this item';
                    const details = btn.getAttribute('data-item-details') || '';

                    if (deleteForm) deleteForm.setAttribute('action', actionUrl);
                    if (deleteItemTitle) deleteItemTitle.textContent = title;
                    if (deleteItemDetails) deleteItemDetails.textContent = details;

                    const bsModal = new bootstrap.Modal(confirmDeleteModal);
                    bsModal.show();
                });
            });
        }
    });
})();

