/**
 * Toyz Shop Admin Panel JavaScript
 * Modern, şık ve sade admin panel fonksiyonları
 */

(function() {
    'use strict';

    // DOM Ready
    document.addEventListener('DOMContentLoaded', function() {
        initializeAdminPanel();
    });

    /**
     * Admin Panel Ana Başlatıcı
     */
    function initializeAdminPanel() {
        initializeSidebar();
        initializeTopbar();
        initializeTooltips();
        initializeDataTables();
        initializeCharts();
        initializeSearch();
        initializeNotifications();
        initializeUserDropdown();
        initializeFormValidation();
        initializeImageUpload();
        initializeTableActions();
        initializeDataExport();
        initializeCustomAlerts();
        
        // Sayfa yüklendiğinde animasyon
        animatePageLoad();
    }

    /**
     * Sidebar Yönetimi
     */
    function initializeSidebar() {
        const sidebar = document.querySelector('.admin-sidebar');
        const container = document.querySelector('.admin-container');
        const toggleBtn = document.querySelector('.sidebar-toggle');
        const collapseBtn = document.querySelector('.sidebar-collapse-btn');

        if (!sidebar || !container) return;

        // Sidebar toggle
        if (toggleBtn) {
            toggleBtn.addEventListener('click', function() {
                sidebar.classList.toggle('show');
                document.body.classList.toggle('sidebar-open');
            });
        }

        // Sidebar collapse
        if (collapseBtn) {
            collapseBtn.addEventListener('click', function() {
                sidebar.classList.toggle('collapsed');
                container.classList.toggle('expanded');
                
                // Local storage'a kaydet
                const isCollapsed = sidebar.classList.contains('collapsed');
                localStorage.setItem('adminSidebarCollapsed', isCollapsed);
            });
        }

        // Local storage'dan durumu yükle
        const isCollapsed = localStorage.getItem('adminSidebarCollapsed') === 'true';
        if (isCollapsed) {
            sidebar.classList.add('collapsed');
            container.classList.add('expanded');
        }

        // Sidebar link aktif durumu
        const sidebarLinks = sidebar.querySelectorAll('.nav-link');
        sidebarLinks.forEach(link => {
            if (link.href === window.location.href) {
                link.classList.add('active');
            }
        });

        // Mobile overlay
        const overlay = document.createElement('div');
        overlay.className = 'sidebar-overlay';
        overlay.addEventListener('click', function() {
            sidebar.classList.remove('show');
            document.body.classList.remove('sidebar-open');
        });
        document.body.appendChild(overlay);
    }

    /**
     * Topbar Yönetimi
     */
    function initializeTopbar() {
        const topbar = document.querySelector('.admin-topbar');
        if (!topbar) return;

        // Scroll efekti
        let lastScrollTop = 0;
        window.addEventListener('scroll', function() {
            const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
            
            if (scrollTop > lastScrollTop && scrollTop > 100) {
                topbar.style.transform = 'translateY(-100%)';
            } else {
                topbar.style.transform = 'translateY(0)';
            }
            
            lastScrollTop = scrollTop;
        });

        // Search form
        const searchForm = document.querySelector('.search-form');
        if (searchForm) {
            searchForm.addEventListener('submit', function(e) {
                e.preventDefault();
                const searchInput = this.querySelector('.search-input');
                if (searchInput.value.trim()) {
                    performSearch(searchInput.value.trim());
                }
            });
        }
    }

    /**
     * Tooltip Başlatıcı
     */
    function initializeTooltips() {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    /**
     * DataTables Başlatıcı
     */
    function initializeDataTables() {
        const tables = document.querySelectorAll('.data-table');
        tables.forEach(table => {
            if (table.classList.contains('initialized')) return;
            
            const dataTable = new DataTable(table, {
                language: {
                    url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/tr.json'
                },
                responsive: true,
                pageLength: 25,
                order: [[0, 'desc']],
                dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
                      '<"row"<"col-sm-12"tr>>' +
                      '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
                lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
                columnDefs: [
                    {
                        targets: -1,
                        orderable: false,
                        searchable: false
                    }
                ]
            });
            
            table.classList.add('initialized');
        });
    }

    /**
     * Chart.js Grafikleri
     */
    function initializeCharts() {
        // Satış Grafiği
        const salesChart = document.getElementById('salesChart');
        if (salesChart) {
            const ctx = salesChart.getContext('2d');
            new Chart(ctx, {
                type: 'line',
                data: {
                    labels: ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran'],
                    datasets: [{
                        label: 'Satışlar',
                        data: [12, 19, 3, 5, 2, 3],
                        borderColor: '#6366f1',
                        backgroundColor: 'rgba(99, 102, 241, 0.1)',
                        tension: 0.4,
                        fill: true
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top',
                        },
                        title: {
                            display: true,
                            text: 'Aylık Satış Grafiği'
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            });
        }

        // Kategori Dağılımı
        const categoryChart = document.getElementById('categoryChart');
        if (categoryChart) {
            const ctx = categoryChart.getContext('2d');
            new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ['Oyuncaklar', 'Elektronik', 'Kitaplar', 'Spor', 'Diğer'],
                    datasets: [{
                        data: [300, 150, 100, 80, 70],
                        backgroundColor: [
                            '#6366f1',
                            '#f59e0b',
                            '#10b981',
                            '#ef4444',
                            '#06b6d4'
                        ]
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'bottom',
                        },
                        title: {
                            display: true,
                            text: 'Kategori Dağılımı'
                        }
                    }
                }
            });
        }
    }

    /**
     * Arama Fonksiyonu
     */
    function initializeSearch() {
        const searchInput = document.querySelector('.search-input');
        if (!searchInput) return;

        let searchTimeout;
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                const query = this.value.trim();
                if (query.length >= 2) {
                    performSearch(query);
                }
            }, 300);
        });
    }

    /**
     * Arama İşlemi
     */
    function performSearch(query) {
        // Burada gerçek arama API'si çağrılabilir
        console.log('Arama yapılıyor:', query);
        
        // Örnek arama sonuçları
        showCustomAlert('Arama sonuçları', `"${query}" için arama yapıldı`, 'info');
    }

    /**
     * Bildirim Yönetimi
     */
    function initializeNotifications() {
        const notificationBtn = document.querySelector('.notification-icon');
        if (!notificationBtn) return;

        // Bildirim sayısı güncelleme
        updateNotificationCount();

        // Bildirim dropdown
        const dropdown = notificationBtn.nextElementSibling;
        if (dropdown) {
            dropdown.addEventListener('show.bs.dropdown', function() {
                loadNotifications();
            });
        }
    }

    /**
     * Bildirim Sayısını Güncelle
     */
    function updateNotificationCount() {
        const badge = document.querySelector('.notification-icon .badge');
        if (badge) {
            // Gerçek uygulamada API'den alınacak
            const count = Math.floor(Math.random() * 10);
            badge.textContent = count;
            badge.style.display = count > 0 ? 'block' : 'none';
        }
    }

    /**
     * Bildirimleri Yükle
     */
    function loadNotifications() {
        const notificationList = document.querySelector('.notification-dropdown .dropdown-menu');
        if (!notificationList) return;

        // Örnek bildirimler
        const notifications = [
            {
                type: 'success',
                title: 'Yeni Sipariş',
                text: 'Yeni bir sipariş alındı',
                time: '2 dakika önce'
            },
            {
                type: 'info',
                title: 'Stok Uyarısı',
                text: 'Bazı ürünlerde stok azalıyor',
                time: '1 saat önce'
            }
        ];

        notificationList.innerHTML = notifications.map(notification => `
            <div class="dropdown-item">
                <div class="d-flex align-items-center">
                    <div class="notification-icon bg-${notification.type} me-3">
                        <i class="fas fa-${notification.type === 'success' ? 'check' : 'info'}"></i>
                    </div>
                    <div class="flex-grow-1">
                        <div class="notification-title">${notification.title}</div>
                        <div class="notification-text">${notification.text}</div>
                        <div class="notification-time">${notification.time}</div>
                    </div>
                </div>
            </div>
        `).join('');
    }

    /**
     * Kullanıcı Dropdown
     */
    function initializeUserDropdown() {
        const userBtn = document.querySelector('.user-avatar');
        if (!userBtn) return;

        // Kullanıcı bilgilerini güncelle
        updateUserInfo();
    }

    /**
     * Kullanıcı Bilgilerini Güncelle
     */
    function updateUserInfo() {
        const userName = document.querySelector('.user-name');
        const userRole = document.querySelector('.user-role');
        
        if (userName) {
            userName.textContent = 'Admin User';
        }
        
        if (userRole) {
            userRole.textContent = 'Sistem Yöneticisi';
        }
    }

    /**
     * Form Validasyonu
     */
    function initializeFormValidation() {
        const forms = document.querySelectorAll('.needs-validation');
        forms.forEach(form => {
            form.addEventListener('submit', function(event) {
                if (!form.checkValidity()) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            });
        });

        // Real-time validation
        const inputs = document.querySelectorAll('.form-control');
        inputs.forEach(input => {
            input.addEventListener('blur', function() {
                validateField(this);
            });
        });
    }

    /**
     * Alan Validasyonu
     */
    function validateField(field) {
        const value = field.value.trim();
        const fieldName = field.name;
        let isValid = true;
        let errorMessage = '';

        // Basit validasyon kuralları
        if (field.hasAttribute('required') && !value) {
            isValid = false;
            errorMessage = 'Bu alan zorunludur';
        } else if (fieldName === 'email' && value && !isValidEmail(value)) {
            isValid = false;
            errorMessage = 'Geçerli bir email adresi giriniz';
        } else if (fieldName === 'phone' && value && !isValidPhone(value)) {
            isValid = false;
            errorMessage = 'Geçerli bir telefon numarası giriniz';
        }

        // Hata mesajını göster/gizle
        const errorElement = field.parentNode.querySelector('.invalid-feedback');
        if (errorElement) {
            if (!isValid) {
                errorElement.textContent = errorMessage;
                field.classList.add('is-invalid');
            } else {
                errorElement.textContent = '';
                field.classList.remove('is-invalid');
                field.classList.add('is-valid');
            }
        }
    }

    /**
     * Email Validasyonu
     */
    function isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    /**
     * Telefon Validasyonu
     */
    function isValidPhone(phone) {
        const phoneRegex = /^[\+]?[0-9\s\-\(\)]{10,}$/;
        return phoneRegex.test(phone);
    }

    /**
     * Resim Yükleme
     */
    function initializeImageUpload() {
        const uploadAreas = document.querySelectorAll('.image-upload-area');
        uploadAreas.forEach(area => {
            const input = area.querySelector('input[type="file"]');
            if (!input) return;

            // Drag & Drop
            area.addEventListener('dragover', function(e) {
                e.preventDefault();
                this.classList.add('dragover');
            });

            area.addEventListener('dragleave', function(e) {
                e.preventDefault();
                this.classList.remove('dragover');
            });

            area.addEventListener('drop', function(e) {
                e.preventDefault();
                this.classList.remove('dragover');
                
                const files = e.dataTransfer.files;
                if (files.length > 0) {
                    handleFileUpload(files[0], input);
                }
            });

            // Click to upload
            area.addEventListener('click', function() {
                input.click();
            });

            // File input change
            input.addEventListener('change', function() {
                if (this.files.length > 0) {
                    handleFileUpload(this.files[0], this);
                }
            });
        });
    }

    /**
     * Dosya Yükleme İşlemi
     */
    function handleFileUpload(file, input) {
        // Dosya türü kontrolü
        if (!file.type.startsWith('image/')) {
            showCustomAlert('Hata', 'Lütfen geçerli bir resim dosyası seçiniz', 'error');
            return;
        }

        // Dosya boyutu kontrolü (5MB)
        if (file.size > 5 * 1024 * 1024) {
            showCustomAlert('Hata', 'Dosya boyutu 5MB\'dan küçük olmalıdır', 'error');
            return;
        }

        // Preview göster
        showImagePreview(file, input);

        // Form data hazırla
        const formData = new FormData();
        formData.append('image', file);

        // Upload işlemi (gerçek uygulamada API'ye gönderilecek)
        console.log('Resim yükleniyor:', file.name);
    }

    /**
     * Resim Preview
     */
    function showImagePreview(file, input) {
        const reader = new FileReader();
        reader.onload = function(e) {
            const preview = input.parentNode.querySelector('.image-preview');
            if (preview) {
                preview.innerHTML = `
                    <img src="${e.target.result}" alt="Preview" class="img-fluid rounded" style="max-height: 200px;">
                    <div class="mt-2">
                        <small class="text-muted">${file.name} (${(file.size / 1024 / 1024).toFixed(2)} MB)</small>
                    </div>
                `;
            }
        };
        reader.readAsDataURL(file);
    }

    /**
     * Tablo Aksiyonları
     */
    function initializeTableActions() {
        // Silme işlemi
        document.addEventListener('click', function(e) {
            if (e.target.classList.contains('btn-delete')) {
                e.preventDefault();
                const itemId = e.target.dataset.id;
                const itemName = e.target.dataset.name;
                
                if (confirm(`${itemName} öğesini silmek istediğinizden emin misiniz?`)) {
                    deleteItem(itemId);
                }
            }
        });

        // Toplu işlemler
        const selectAllCheckbox = document.querySelector('.select-all');
        if (selectAllCheckbox) {
            selectAllCheckbox.addEventListener('change', function() {
                const checkboxes = document.querySelectorAll('.row-checkbox');
                checkboxes.forEach(checkbox => {
                    checkbox.checked = this.checked;
                });
                updateBulkActions();
            });
        }

        // Satır seçimi
        document.addEventListener('change', function(e) {
            if (e.target.classList.contains('row-checkbox')) {
                updateBulkActions();
            }
        });
    }

    /**
     * Öğe Silme
     */
    function deleteItem(itemId) {
        // Gerçek uygulamada API'ye DELETE isteği gönderilecek
        console.log('Öğe siliniyor:', itemId);
        
        // UI'dan kaldır
        const row = document.querySelector(`[data-id="${itemId}"]`).closest('tr');
        if (row) {
            row.remove();
            showCustomAlert('Başarılı', 'Öğe başarıyla silindi', 'success');
        }
    }

    /**
     * Toplu Aksiyonları Güncelle
     */
    function updateBulkActions() {
        const selectedCheckboxes = document.querySelectorAll('.row-checkbox:checked');
        const bulkActions = document.querySelector('.bulk-actions');
        
        if (bulkActions) {
            if (selectedCheckboxes.length > 0) {
                bulkActions.style.display = 'block';
                bulkActions.querySelector('.selected-count').textContent = selectedCheckboxes.length;
            } else {
                bulkActions.style.display = 'none';
            }
        }
    }

    /**
     * Veri Dışa Aktarma
     */
    function initializeDataExport() {
        const exportBtns = document.querySelectorAll('.btn-export');
        exportBtns.forEach(btn => {
            btn.addEventListener('click', function(e) {
                e.preventDefault();
                const format = this.dataset.format || 'csv';
                exportData(format);
            });
        });
    }

    /**
     * Veri Dışa Aktarma İşlemi
     */
    function exportData(format) {
        const table = document.querySelector('.data-table');
        if (!table) return;

        let data = [];
        const headers = [];
        const rows = table.querySelectorAll('tbody tr');

        // Başlıkları al
        table.querySelectorAll('thead th').forEach(th => {
            if (!th.classList.contains('no-export')) {
                headers.push(th.textContent.trim());
            }
        });

        // Satırları al
        rows.forEach(row => {
            const rowData = [];
            row.querySelectorAll('td').forEach((td, index) => {
                if (!table.querySelector(`thead th:nth-child(${index + 1})`).classList.contains('no-export')) {
                    rowData.push(td.textContent.trim());
                }
            });
            data.push(rowData);
        });

        // Format'a göre dışa aktar
        switch (format) {
            case 'csv':
                exportToCSV(headers, data);
                break;
            case 'excel':
                exportToExcel(headers, data);
                break;
            case 'pdf':
                exportToPDF(headers, data);
                break;
        }
    }

    /**
     * CSV Dışa Aktarma
     */
    function exportToCSV(headers, data) {
        let csvContent = headers.join(',') + '\n';
        data.forEach(row => {
            csvContent += row.join(',') + '\n';
        });

        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = 'data.csv';
        link.click();
    }

    /**
     * Excel Dışa Aktarma
     */
    function exportToExcel(headers, data) {
        // Basit Excel export (gerçek uygulamada xlsx.js kullanılabilir)
        let excelContent = headers.join('\t') + '\n';
        data.forEach(row => {
            excelContent += row.join('\t') + '\n';
        });

        const blob = new Blob([excelContent], { type: 'application/vnd.ms-excel' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = 'data.xls';
        link.click();
    }

    /**
     * PDF Dışa Aktarma
     */
    function exportToPDF(headers, data) {
        // Basit PDF export (gerçek uygulamada jsPDF kullanılabilir)
        showCustomAlert('Bilgi', 'PDF dışa aktarma özelliği yakında eklenecek', 'info');
    }

    /**
     * Özel Alert Sistemi
     */
    function initializeCustomAlerts() {
        // Global alert container oluştur
        if (!document.querySelector('.custom-alert-container')) {
            const alertContainer = document.createElement('div');
            alertContainer.className = 'custom-alert-container';
            alertContainer.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 9999;
                max-width: 400px;
            `;
            document.body.appendChild(alertContainer);
        }
    }

    /**
     * Özel Alert Göster
     */
    function showCustomAlert(title, message, type = 'info') {
        const alertContainer = document.querySelector('.custom-alert-container');
        if (!alertContainer) return;

        const alertId = 'alert-' + Date.now();
        const alertElement = document.createElement('div');
        alertElement.id = alertId;
        alertElement.className = `alert alert-${type} alert-dismissible fade show`;
        alertElement.innerHTML = `
            <strong>${title}</strong><br>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

        alertContainer.appendChild(alertElement);

        // Otomatik kapatma
        setTimeout(() => {
            const alert = document.getElementById(alertId);
            if (alert) {
                alert.remove();
            }
        }, 5000);
    }

    /**
     * Sayfa Yükleme Animasyonu
     */
    function animatePageLoad() {
        const elements = document.querySelectorAll('.fade-in');
        elements.forEach((element, index) => {
            setTimeout(() => {
                element.style.opacity = '1';
                element.style.transform = 'translateY(0)';
            }, index * 100);
        });
    }

    /**
     * Utility Fonksiyonlar
     */
    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    function formatCurrency(amount, currency = '₺') {
        return currency + parseFloat(amount).toLocaleString('tr-TR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    }

    function formatDate(date) {
        return new Date(date).toLocaleDateString('tr-TR', {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
    }

    // Global fonksiyonları expose et
    window.ToyzShopAdmin = {
        showAlert: showCustomAlert,
        formatCurrency: formatCurrency,
        formatDate: formatDate,
        exportData: exportData
    };
})();

