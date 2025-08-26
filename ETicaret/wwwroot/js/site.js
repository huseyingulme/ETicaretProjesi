document.addEventListener('DOMContentLoaded', function() {
    const anchorLinks = document.querySelectorAll('a[href^="#"]');
    anchorLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            const targetElement = document.querySelector(targetId);
            
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Search functionality
    const searchInput = document.querySelector('.search-box input');
    const searchTags = document.querySelectorAll('.search-tag');
    
    if (searchInput) {
        searchInput.addEventListener('focus', function() {
            this.parentElement.style.boxShadow = '0 0 0 0.2rem rgba(0, 123, 255, 0.25)';
        });
        
        searchInput.addEventListener('blur', function() {
            this.parentElement.style.boxShadow = 'none';
        });
    }
    
    // Search tag click functionality
    searchTags.forEach(tag => {
        tag.addEventListener('click', function() {
            const categoryName = this.getAttribute('data-category');
            if (searchInput) {
                searchInput.value = categoryName;
                searchInput.focus();
            }
        });
    });

    // Main search form functionality
    const mainSearchForm = document.getElementById('searchForm');
    if (mainSearchForm) {
        mainSearchForm.addEventListener('submit', function(e) {
            const searchInput = document.getElementById('searchInput');
            if (searchInput && searchInput.value.trim() === '') {
                e.preventDefault();
                showNotification('Lütfen arama yapmak istediğiniz kelimeyi girin.', 'error');
                searchInput.focus();
                }
            });
        }

    // Search box in navigation
    const navSearchInput = document.querySelector('.search-box input');
    const navSearchBtn = document.querySelector('.search-box button');
    
    if (navSearchInput && navSearchBtn) {
        navSearchBtn.addEventListener('click', function() {
            const query = navSearchInput.value.trim();
                if (query) {
                window.location.href = `/Home/Search?q=${encodeURIComponent(query)}`;
            } else {
                showNotification('Lütfen arama yapmak istediğiniz kelimeyi girin.', 'error');
                navSearchInput.focus();
            }
        });
        
        navSearchInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                navSearchBtn.click();
            }
        });
    }

    // Newsletter form functionality
    const newsletterForm = document.querySelector('.newsletter-form');
    if (newsletterForm) {
        const newsletterInput = newsletterForm.querySelector('input[type="email"]');
        const newsletterBtn = newsletterForm.querySelector('button');
        
        if (newsletterBtn && newsletterInput) {
            newsletterBtn.addEventListener('click', function() {
                const email = newsletterInput.value.trim();
                if (email && isValidEmail(email)) {
                    showNotification('E-posta adresiniz başarıyla eklendi!', 'success');
                    newsletterInput.value = '';
                } else {
                    showNotification('Lütfen geçerli bir e-posta adresi girin.', 'error');
                }
            });
        }
    }

    // Product card hover effects
    const productCards = document.querySelectorAll('.product-card');
    productCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-5px)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
        });
    });

    // Category card hover effects
    const categoryCards = document.querySelectorAll('.category-card');
    categoryCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-5px)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
        });
    });

    // Brand card hover effects
    const brandCards = document.querySelectorAll('.brand-card');
    brandCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-5px)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
        });
    });

    // Form validation enhancement
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        const inputs = form.querySelectorAll('input, textarea');
        inputs.forEach(input => {
            input.addEventListener('blur', function() {
                validateField(this);
            });
            
            input.addEventListener('input', function() {
                clearFieldError(this);
            });
        });
    });

    // Back to top functionality
    const backToTopBtn = document.createElement('button');
    backToTopBtn.innerHTML = '<i class="fas fa-arrow-up"></i>';
    backToTopBtn.className = 'back-to-top';
    backToTopBtn.style.cssText = `
        position: fixed;
        bottom: 30px;
        right: 30px;
        width: 50px;
        height: 50px;
        border-radius: 50%;
        background: #007bff;
        color: white;
        border: none;
        cursor: pointer;
        display: none;
        z-index: 1000;
        box-shadow: 0 4px 20px rgba(0, 0, 0, 0.2);
        transition: all 0.3s ease;
    `;
    
    document.body.appendChild(backToTopBtn);
    
    backToTopBtn.addEventListener('click', function() {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });

    window.addEventListener('scroll', function() {
        if (window.pageYOffset > 300) {
            backToTopBtn.style.display = 'block';
        } else {
            backToTopBtn.style.display = 'none';
        }
    });

    // Lazy loading for images
    const images = document.querySelectorAll('img[data-src]');
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                imageObserver.unobserve(img);
            }
        });
    });

    images.forEach(img => imageObserver.observe(img));
});

// Utility functions
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.textContent = message;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 1rem 1.5rem;
        border-radius: 8px;
        color: white;
        font-weight: 600;
        z-index: 9999;
        transform: translateX(100%);
        transition: transform 0.3s ease;
        max-width: 300px;
    `;
    
    // Set background color based on type
    switch (type) {
        case 'success':
            notification.style.background = '#28a745';
            break;
        case 'error':
            notification.style.background = '#dc3545';
            break;
        case 'warning':
            notification.style.background = '#ffc107';
            notification.style.color = '#333';
            break;
        default:
            notification.style.background = '#17a2b8';
    }
    
    document.body.appendChild(notification);
    
    // Animate in
    setTimeout(() => {
        notification.style.transform = 'translateX(0)';
    }, 100);
    
    setTimeout(() => {
        notification.style.transform = 'translateX(100%)';
                setTimeout(() => {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }
        }, 300);
    }, 5000);
}

function validateField(field) {
    const value = field.value.trim();
    let isValid = true;
    let errorMessage = '';
    
    clearFieldError(field);
    
    if (field.hasAttribute('required') && !value) {
        isValid = false;
        errorMessage = 'Bu alan zorunludur.';
    }

    if (field.type === 'email' && value && !isValidEmail(value)) {
        isValid = false;
        errorMessage = 'Geçerli bir e-posta adresi girin.';
    }

    if (field.hasAttribute('minlength')) {
        const minLength = parseInt(field.getAttribute('minlength'));
        if (value.length < minLength) {
            isValid = false;
            errorMessage = `En az ${minLength} karakter girmelisiniz.`;
        }
    }
    
    if (!isValid) {
        showFieldError(field, errorMessage);
    }
    
    return isValid;
}

function showFieldError(field, message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'field-error';
    errorDiv.textContent = message;
    errorDiv.style.cssText = `
        color: #dc3545;
        font-size: 0.875rem;
        margin-top: 0.25rem;
        display: block;
    `;
    
    field.parentNode.appendChild(errorDiv);
    field.style.borderColor = '#dc3545';
}

function clearFieldError(field) {
    const existingError = field.parentNode.querySelector('.field-error');
    if (existingError) {
        existingError.remove();
    }
    field.style.borderColor = '#e9ecef';
}

function performSearch(query) {
    if (!query.trim()) return;

    showNotification(`"${query}" için arama yapılıyor...`, 'info');
}
function quickView(productId) {
    showNotification('Ürün detayları yükleniyor...', 'info');
}

function addToCart(productId) {
    showNotification('Ürün sepete eklendi!', 'success');
}

function updateFavoriteCount() {
    fetch('/Favorites/Count')
        .then(response => response.json())
        .then(data => {
            const favoriteBadge = document.querySelector('#favoriteCountBadge');
            if (favoriteBadge) {
                if (data.count > 0) {
                    favoriteBadge.textContent = data.count;
                    favoriteBadge.style.display = 'inline-block';
                } else {
                    favoriteBadge.style.display = 'none';
                }
            }
        })
        .catch(error => {
            console.error('Hata:', error);
        });
}

async function addToFavorites(productId) {
    try {
        const response = await fetch('/Favorites/Add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: 'productId=' + productId
        });

        const data = await response.json();
        
        if (data.success) {
            alert('Ürün favorilere eklendi!');
            location.reload();
        } else {
            if (data.message.includes('Giriş yapmalısınız')) {
                alert('Favori eklemek için giriş yapmalısınız');
                window.location.href = '/Account/Login';
            } else {
                alert(data.message);
            }
        }
    } catch (error) {
        console.error('Hata:', error);
        alert('Bir hata oluştu');
    }
}
// Remove from favorites
async function removeFromFavorites(productId) {
    try {
        const response = await fetch('/Favorites/Remove', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: 'productId=' + productId
        });

        const data = await response.json();
        
        if (data.success) {
            alert('Ürün favorilerden kaldırıldı!');
            location.reload();
        } else {
            if (data.message.includes('Giriş yapmalısınız')) {
                alert('Favori kaldırmak için giriş yapmalısınız');
                window.location.href = '/Account/Login';
            } else {
                alert(data.message);
            }
        }
    } catch (error) {
        console.error('Hata:', error);
        alert('Bir hata oluştu');
    }
}
// Update favorite button state
function updateFavoriteButton(button, isFavorite) {
    if (isFavorite) {
        button.classList.add('active');
        button.querySelector('i').className = 'fas fa-heart';
        button.title = 'Favorilerden Kaldır';
    } else {
        button.classList.remove('active');
        button.querySelector('i').className = 'far fa-heart';
        button.title = 'Favorilere Ekle';
    }
}

function addToWishlist(productId) {
        showNotification('Ürün favorilere eklendi!', 'success');
}

function subscribeNewsletter(email) {
    if (!isValidEmail(email)) {
        showNotification('Lütfen geçerli bir e-posta adresi girin.', 'error');
        return false;
    }
    showNotification('E-posta listesine başarıyla eklendiniz!', 'success');
    return true;
}

// Contact form submission
function submitContactForm(formData) {
    showNotification('Mesajınız başarıyla gönderildi!', 'success');
    return true;
}

// Category filter functionality
function filterByCategory(categoryId) {
    showNotification('Kategori filtreleniyor...', 'info');
}

// Brand filter functionality
function filterByBrand(brandId) {
    showNotification('Marka filtreleniyor...', 'info');
}

// Price range filter
function filterByPrice(minPrice, maxPrice) {
    showNotification(`Fiyat aralığı: ${minPrice} - ${maxPrice}`, 'info');
}

// Sort products
function sortProducts(sortBy) {
    showNotification(`Ürünler ${sortBy} göre sıralanıyor...`, 'info');
}

// Initialize tooltips
function initializeTooltips() {
    const tooltipElements = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltipElements.forEach(element => {
        new bootstrap.Tooltip(element);
    });
}

// Initialize popovers
function initializePopovers() {
    const popoverElements = document.querySelectorAll('[data-bs-toggle="popover"]');
    popoverElements.forEach(element => {
        new bootstrap.Popover(element);
    });
}

// Mobile menu toggle
function toggleMobileMenu() {
    const navbar = document.querySelector('.navbar-collapse');
    if (navbar) {
        navbar.classList.toggle('show');
    }
}

// Close mobile menu when clicking outside
document.addEventListener('click', function(e) {
    const navbar = document.querySelector('.navbar');
    const navbarToggle = document.querySelector('.navbar-toggler');
    
    if (navbar && navbarToggle && !navbar.contains(e.target)) {
        const navbarCollapse = navbar.querySelector('.navbar-collapse');
        if (navbarCollapse && navbarCollapse.classList.contains('show')) {
            navbarCollapse.classList.remove('show');
        }
    }
});

// Export functions for global use
window.siteFunctions = {
    performSearch,
    quickView,
    addToCart,
    addToWishlist,
    subscribeNewsletter,
    submitContactForm,
    filterByCategory,
    filterByBrand,
    filterByPrice,
    sortProducts,
    initializeTooltips,
    initializePopovers,
    toggleMobileMenu,
    updateFavoriteCount,
    updateFavoriteButton,
    addToFavorites,
    removeFromFavorites
};
