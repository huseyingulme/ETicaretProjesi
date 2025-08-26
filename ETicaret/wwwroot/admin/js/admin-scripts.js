// ===== ADMIN PANEL SCRIPTS =====

document.addEventListener('DOMContentLoaded', function() {
    // Initialize admin panel functionality
    initAdminPanel();
    
    // Initialize DataTables
    initDataTables();
    
    // Initialize tooltips and popovers
    initBootstrapComponents();
    
    // Initialize image preview functionality
    initImagePreview();

    // Initialize image uploads
    initializeImageUploads();
});

// ===== ADMIN PANEL INITIALIZATION =====
function initAdminPanel() {
    // Sidebar toggle functionality
    const sidebarToggle = document.getElementById('sidebarToggle');
    const adminSidebar = document.querySelector('.admin-sidebar');
    const adminContainer = document.querySelector('.admin-container');
    
    // Create mobile menu toggle button
    createMobileMenuToggle();
    
    // Create sidebar overlay
    createSidebarOverlay();
    
    if (sidebarToggle && adminSidebar) {
        sidebarToggle.addEventListener('click', function() {
            adminSidebar.classList.toggle('show');
            
            // Update container margin on mobile
            if (window.innerWidth <= 1200) {
                if (adminSidebar.classList.contains('show')) {
                    adminContainer.style.marginLeft = '0';
                    showSidebarOverlay();
                } else {
                    adminContainer.style.marginLeft = '0';
                    hideSidebarOverlay();
                }
            }
        });
    }
    
    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function(e) {
        if (window.innerWidth <= 1200) {
            const mobileToggle = document.querySelector('.mobile-menu-toggle');
            if (!adminSidebar.contains(e.target) && 
                !sidebarToggle?.contains(e.target) && 
                !mobileToggle?.contains(e.target)) {
                adminSidebar.classList.remove('show');
                hideSidebarOverlay();
            }
        }
    });
    
    // Handle window resize
    window.addEventListener('resize', function() {
        if (window.innerWidth > 1200) {
            adminSidebar.classList.remove('show');
            adminContainer.style.marginLeft = '';
            hideSidebarOverlay();
        }
    });
    
    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            if (alert) {
                const bsAlert = new bootstrap.Alert(alert);
                bsAlert.close();
            }
        }, 5000);
    });
}

// ===== MOBILE MENU FUNCTIONS =====
function createMobileMenuToggle() {
    // Remove existing mobile toggle if any
    const existingToggle = document.querySelector('.mobile-menu-toggle');
    if (existingToggle) {
        existingToggle.remove();
    }
    
    // Create mobile menu toggle button
    const mobileToggle = document.createElement('button');
    mobileToggle.className = 'mobile-menu-toggle';
    mobileToggle.innerHTML = '<i class="fas fa-bars"></i>';
    mobileToggle.setAttribute('type', 'button');
    mobileToggle.setAttribute('aria-label', 'Menüyü Aç/Kapat');
    mobileToggle.setAttribute('title', 'Menüyü Aç/Kapat');
    
    // Add click event
    mobileToggle.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation();
        
        const adminSidebar = document.querySelector('.admin-sidebar');
        if (adminSidebar) {
            adminSidebar.classList.toggle('show');
            if (adminSidebar.classList.contains('show')) {
                showSidebarOverlay();
                // Change icon to close
                this.innerHTML = '<i class="fas fa-times"></i>';
            } else {
                hideSidebarOverlay();
                // Change icon back to menu
                this.innerHTML = '<i class="fas fa-bars"></i>';
            }
        }
    });
    
    // Add to body
    document.body.appendChild(mobileToggle);
    
    // Show/hide based on screen size
    updateMobileToggleVisibility();
}

function createSidebarOverlay() {
    // Remove existing overlay if any
    const existingOverlay = document.querySelector('.sidebar-overlay');
    if (existingOverlay) {
        existingOverlay.remove();
    }
    
    // Create sidebar overlay
    const overlay = document.createElement('div');
    overlay.className = 'sidebar-overlay';
    
    // Add click event to close sidebar
    overlay.addEventListener('click', function() {
        const adminSidebar = document.querySelector('.admin-sidebar');
        if (adminSidebar) {
            adminSidebar.classList.remove('show');
            hideSidebarOverlay();
        }
    });
    
    // Add to body
    document.body.appendChild(overlay);
}

function showSidebarOverlay() {
    const overlay = document.querySelector('.sidebar-overlay');
    if (overlay) {
        overlay.classList.add('show');
    }
}

function hideSidebarOverlay() {
    const overlay = document.querySelector('.sidebar-overlay');
    if (overlay) {
        overlay.classList.remove('show');
    }
}

function updateMobileToggleVisibility() {
    const mobileToggle = document.querySelector('.mobile-menu-toggle');
    if (mobileToggle) {
        if (window.innerWidth <= 1200) {
            mobileToggle.style.display = 'block';
        } else {
            mobileToggle.style.display = 'none';
        }
    }
}

// Update mobile toggle visibility on window resize
window.addEventListener('resize', function() {
    updateMobileToggleVisibility();
});

// ===== DATATABLES INITIALIZATION =====
function initDataTables() {
    const tables = document.querySelectorAll('.datatable');
    
    tables.forEach(table => {
        if (table) {
            $(table).DataTable({
                language: {
                    url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/tr.json'
                },
                responsive: true,
                pageLength: 25,
                order: [[0, 'desc']],
                columnDefs: [
                    {
                        targets: -1,
                        orderable: false,
                        searchable: false
                    }
                ],
                dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
                     '<"row"<"col-sm-12"tr>>' +
                     '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
                lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Tümü"]],
                initComplete: function() {
                    // Add custom styling
                    this.api().columns().every(function() {
                        const column = this;
                        const title = column.header().textContent;
                        
                        // Add search input for each column
                        if (title !== 'İşlemler') {
                            const input = document.createElement('input');
                            input.placeholder = title + ' ara...';
                            input.className = 'form-control form-control-sm';
                            input.style.width = '100%';
                            
                            input.addEventListener('keyup', function() {
                                if (column.search() !== this.value) {
                                    column.search(this.value).draw();
                                }
                            });
                            
                            column.header().appendChild(input);
                        }
                    });
                }
            });
        }
    });
}

// ===== BOOTSTRAP COMPONENTS INITIALIZATION =====
function initBootstrapComponents() {
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    // Initialize popovers
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function(popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
}

// ===== IMAGE PREVIEW FUNCTIONALITY =====
function initImagePreview() {
    // File input change event for image preview
    document.addEventListener('change', function(e) {
        if (e.target.type === 'file' && e.target.accept && e.target.accept.includes('image')) {
            const file = e.target.files[0];
            const previewContainer = e.target.parentNode.querySelector('.image-preview');
            
            if (file && previewContainer) {
                showImagePreview(file, previewContainer);
            }
        }
    });
}

function showImagePreview(file, container) {
    if (file) {
        // File validation
        if (file.size > 5 * 1024 * 1024) {
            showAlert('Dosya boyutu 5MB\'dan büyük olamaz!', 'danger');
            return;
        }
        
        if (!file.type.match('image.*')) {
            showAlert('Lütfen sadece resim dosyası seçin!', 'danger');
            return;
        }
        
        const reader = new FileReader();
        reader.onload = function(e) {
            // Create preview with proper structure
            container.innerHTML = `
                <div class="preview-header">
                    <span class="preview-title">Önizleme</span>
                    <button type="button" class="btn btn-sm btn-outline-danger" onclick="clearImagePreview(this)">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
                <img src="${e.target.result}" alt="Logo önizleme" class="preview-img" />
                <div class="preview-info">
                    <div class="file-name">${file.name}</div>
                    <div class="file-size">${formatFileSize(file.size)}</div>
                </div>
            `;
            container.style.display = 'block';
            
            // Auto-adjust image size for optimal display
            const previewImg = container.querySelector('.preview-img');
            if (previewImg) {
                previewImg.onload = function() {
                    // Ensure image fits properly in preview area
                    this.style.maxWidth = '100%';
                    this.style.height = 'auto';
                    this.style.objectFit = 'contain';
                    
                    // Auto-resize based on image dimensions
                    const img = this;
                    const container = img.closest('.image-preview');
                    if (container) {
                        // Get image natural dimensions
                        const naturalWidth = img.naturalWidth;
                        const naturalHeight = img.naturalHeight;
                        
                        // Calculate optimal display size
                        const maxWidth = 500; // Max width for preview
                        const maxHeight = 500; // Max height for preview
                        
                        let displayWidth = naturalWidth;
                        let displayHeight = naturalHeight;
                        
                        // Scale down if image is too large
                        if (naturalWidth > maxWidth || naturalHeight > maxHeight) {
                            const ratio = Math.min(maxWidth / naturalWidth, maxHeight / naturalHeight);
                            displayWidth = naturalWidth * ratio;
                            displayHeight = naturalHeight * ratio;
                        }
                        
                        // Apply calculated dimensions
                        img.style.width = displayWidth + 'px';
                        img.style.height = displayHeight + 'px';
                        img.style.maxWidth = '100%';
                        img.style.maxHeight = maxHeight + 'px';
                    }
                };
            }
        };
        reader.readAsDataURL(file);
    }
}

// ===== IMAGE PREVIEW UTILITIES =====
function clearImagePreview(button) {
    const previewContainer = button.closest('.image-preview');
    if (previewContainer) {
        previewContainer.style.display = 'none';
        previewContainer.innerHTML = '';
        
        // Clear the file input
        const fileInput = previewContainer.parentNode.querySelector('input[type="file"]');
        if (fileInput) {
            fileInput.value = '';
        }
    }
}

// ===== UTILITY FUNCTIONS =====
function formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

function showAlert(message, type = 'info') {
    const alertContainer = document.createElement('div');
    alertContainer.className = `alert alert-${type} alert-dismissible fade show`;
    alertContainer.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    // Insert at the top of the page content
    const pageContent = document.querySelector('.page-content');
    if (pageContent) {
        pageContent.insertBefore(alertContainer, pageContent.firstChild);
        
        // Auto-remove after 5 seconds
        setTimeout(() => {
            if (alertContainer) {
                const bsAlert = new bootstrap.Alert(alertContainer);
                bsAlert.close();
            }
        }, 5000);
    }
}

// ===== IMAGE MANAGEMENT FUNCTIONS =====
function changeLogo(brandId) {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    input.style.display = 'none';
    
    input.addEventListener('change', function(e) {
        const file = e.target.files[0];
        if (file) {
            uploadNewLogo(brandId, file);
        }
    });
    
    input.click();
}

function uploadNewLogo(brandId, file) {
    const formData = new FormData();
    formData.append('newLogo', file);
    
    // Show loading state
    showAlert('Logo yükleniyor...', 'info');
    
    fetch(`/Admin/Brands/ChangeLogo/${brandId}`, {
        method: 'POST',
        body: formData,
        headers: {
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            showAlert(data.message, 'success');
            // Update the logo display
            const logoImg = document.querySelector(`#brand-${brandId} .brand-logo`);
            if (logoImg) {
                logoImg.src = `/Img/Brands/${data.logoPath}`;
            }
        } else {
            showAlert(data.message, 'danger');
        }
    })
    .catch(error => {
        showAlert('Logo güncellenirken hata oluştu: ' + error.message, 'danger');
    });
}

function removeLogo(brandId) {
    if (confirm('Bu markanın logosunu kaldırmak istediğinizden emin misiniz?')) {
        // Send request to remove logo
        fetch(`/Admin/Brands/Edit/${brandId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({
                removeLogo: true
            })
        })
        .then(response => {
            if (response.ok) {
                showAlert('Logo başarıyla kaldırıldı!', 'success');
                // Update the logo display
                const logoImg = document.querySelector(`#brand-${brandId} .brand-logo`);
                if (logoImg) {
                    logoImg.src = '/admin/img/no-image.png';
                }
            } else {
                showAlert('Logo kaldırılırken hata oluştu!', 'danger');
            }
        })
        .catch(error => {
            showAlert('Logo kaldırılırken hata oluştu: ' + error.message, 'danger');
        });
    }
}

// ===== FORM VALIDATION =====
function validateBrandForm(form) {
    const nameInput = form.querySelector('input[name="Name"]');
    const logoInput = form.querySelector('input[type="file"]');
    
    let isValid = true;
    
    // Clear previous error states
    form.querySelectorAll('.is-invalid').forEach(el => {
        el.classList.remove('is-invalid');
    });
    
    // Validate name
    if (!nameInput.value.trim()) {
        nameInput.classList.add('is-invalid');
        showAlert('Marka adı zorunludur!', 'danger');
        isValid = false;
    }
    
    // Validate logo (only for create form)
    if (form.action.includes('Create') && (!logoInput.files || logoInput.files.length === 0)) {
        logoInput.classList.add('is-invalid');
        showAlert('Logo dosyası zorunludur!', 'danger');
        isValid = false;
    }
    
    return isValid;
}

// ===== CONFIRMATION DIALOGS =====
function confirmDelete(message = 'Bu öğeyi silmek istediğinizden emin misiniz?') {
    return confirm(message);
}

// ===== AJAX HELPERS =====
function makeAjaxRequest(url, method = 'GET', data = null) {
    const options = {
        method: method,
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    };
    
    if (data && method !== 'GET') {
        options.body = JSON.stringify(data);
    }
    
    return fetch(url, options);
}

// ===== NOTIFICATION SYSTEM =====
function showNotification(message, type = 'info', duration = 5000) {
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.innerHTML = `
        <div class="notification-content">
            <i class="fas fa-${getNotificationIcon(type)}"></i>
            <span>${message}</span>
        </div>
        <button class="notification-close">&times;</button>
    `;
    
    // Add to page
    document.body.appendChild(notification);
    
    // Show notification
    setTimeout(() => {
        notification.classList.add('show');
    }, 100);
    
    // Auto-remove
    setTimeout(() => {
        hideNotification(notification);
    }, duration);
    
    // Close button
    notification.querySelector('.notification-close').addEventListener('click', () => {
        hideNotification(notification);
    });
}

function hideNotification(notification) {
    notification.classList.remove('show');
    setTimeout(() => {
        if (notification.parentNode) {
            notification.parentNode.removeChild(notification);
        }
    }, 300);
}

function getNotificationIcon(type) {
    const icons = {
        success: 'check-circle',
        error: 'exclamation-circle',
        warning: 'exclamation-triangle',
        info: 'info-circle'
    };
    return icons[type] || 'info-circle';
}

// ===== EXPORT FUNCTIONS =====
function exportToExcel(tableId, filename = 'export') {
    const table = document.getElementById(tableId);
    if (!table) return;
    
    const wb = XLSX.utils.table_to_book(table, { sheet: "Sheet1" });
    XLSX.writeFile(wb, `${filename}.xlsx`);
}

function exportToPDF(tableId, filename = 'export') {
    const table = document.getElementById(tableId);
    if (!table) return;
    
    const opt = {
        margin: 1,
        filename: `${filename}.pdf`,
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 2 },
        jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
    };
    
    html2pdf().set(opt).from(table).save();
}

// ===== IMAGE UPLOAD FUNCTIONALITY =====
function initializeImageUploads() {
    const imageUploadAreas = document.querySelectorAll('.image-upload-area');
    
    imageUploadAreas.forEach(area => {
        const fileInput = area.querySelector('input[type="file"]');
        const previewContainer = area.querySelector('.image-preview-container');
        const currentImage = area.querySelector('.current-image');
        const uploadIcon = area.querySelector('.upload-icon');
        const uploadTitle = area.querySelector('.upload-title');
        const uploadSubtitle = area.querySelector('.upload-subtitle');
        
        if (fileInput) {
            // File input change event
            fileInput.addEventListener('change', function(e) {
                handleFileSelect(e.target.files[0], area, previewContainer, currentImage, uploadIcon, uploadTitle, uploadSubtitle);
            });
            
            // Drag and drop events
            area.addEventListener('dragover', function(e) {
                e.preventDefault();
                area.classList.add('dragover');
            });
            
            area.addEventListener('dragleave', function(e) {
                e.preventDefault();
                area.classList.remove('dragover');
            });
            
            area.addEventListener('drop', function(e) {
                e.preventDefault();
                area.classList.remove('dragover');
                const files = e.dataTransfer.files;
                if (files.length > 0) {
                    handleFileSelect(files[0], area, previewContainer, currentImage, uploadIcon, uploadTitle, uploadSubtitle);
                    fileInput.files = files;
                }
            });
            
            // Click to upload
            area.addEventListener('click', function() {
                fileInput.click();
            });
        }
    });
}

function handleFileSelect(file, area, previewContainer, currentImage, uploadIcon, uploadTitle, uploadSubtitle) {
    if (file && file.type.startsWith('image/')) {
        const reader = new FileReader();
        
        reader.onload = function(e) {
            // Hide upload elements
            uploadIcon.style.display = 'none';
            uploadTitle.style.display = 'none';
            uploadSubtitle.style.display = 'none';
            
            // Show preview
            if (currentImage) {
                currentImage.src = e.target.result;
                currentImage.style.display = 'block';
            } else {
                // Create new preview image
                const newImage = document.createElement('img');
                newImage.src = e.target.result;
                newImage.className = 'current-image';
                newImage.style.display = 'block';
                previewContainer.appendChild(newImage);
            }
            
            // Show image actions
            showImageActions(area, previewContainer);
        };
        
        reader.readAsDataURL(file);
    } else {
        alert('Lütfen geçerli bir resim dosyası seçin.');
    }
}

function showImageActions(area, previewContainer) {
    // Remove existing actions
    const existingActions = previewContainer.querySelector('.image-actions');
    if (existingActions) {
        existingActions.remove();
    }
    
    // Create action buttons
    const actionsDiv = document.createElement('div');
    actionsDiv.className = 'image-actions';
    
    const changeBtn = document.createElement('button');
    changeBtn.type = 'button';
    changeBtn.className = 'btn-image-action btn-change-image';
    changeBtn.innerHTML = '<i class="fas fa-edit"></i> Resmi Değiştir';
    changeBtn.onclick = () => resetImageUpload(area, previewContainer);
    
    const removeBtn = document.createElement('button');
    removeBtn.type = 'button';
    removeBtn.className = 'btn-image-action btn-remove-image';
    removeBtn.innerHTML = '<i class="fas fa-trash"></i> Resmi Kaldır';
    removeBtn.onclick = () => removeImage(area, previewContainer);
    
    actionsDiv.appendChild(changeBtn);
    actionsDiv.appendChild(removeBtn);
}

function resetImageUpload(area, previewContainer) {
    const fileInput = area.querySelector('input[type="file"]');
    const uploadIcon = area.querySelector('.upload-icon');
    const uploadTitle = area.querySelector('.upload-title');
    const uploadSubtitle = area.querySelector('.upload-subtitle');
    const currentImage = area.querySelector('.current-image');
    
    // Reset file input
    fileInput.value = '';
    
    // Show upload elements
    uploadIcon.style.display = 'block';
    uploadTitle.style.display = 'block';
    uploadSubtitle.style.display = 'block';
    
    // Hide preview and actions
    if (currentImage) {
        currentImage.style.display = 'none';
    }
    
    const actionsDiv = previewContainer.querySelector('.image-actions');
    if (actionsDiv) {
        actionsDiv.remove();
    }
}

function removeImage(area, previewContainer) {
    const fileInput = area.querySelector('input[type="file"]');
    const currentImage = area.querySelector('.current-image');
    
    // Reset file input
    fileInput.value = '';
    
    // Hide preview
    if (currentImage) {
        currentImage.style.display = 'none';
    }
    
    // Remove actions
    const actionsDiv = previewContainer.querySelector('.image-actions');
    if (actionsDiv) {
        actionsDiv.remove();
    }
    
    // Reset to upload state
    resetImageUpload(area, previewContainer);
}
