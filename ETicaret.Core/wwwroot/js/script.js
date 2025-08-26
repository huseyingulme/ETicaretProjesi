// ===== TECHSTORE E-COMMERCE JAVASCRIPT =====
// Modern ES6+ syntax ile yazılmış, veritabanı entegrasyonu için hazır

class TechStore {
    constructor() {
        this.currentSlide = 0;
        this.slideInterval = null;
        this.cartItems = [];
        this.wishlistItems = [];
        this.currentFilter = 'all';
        this.productsLoaded = 0;
        this.productsPerPage = 8;
        
        this.init();
    }

    init() {
        this.hideLoadingScreen();
        this.initEventListeners();
        this.initSlider();
        this.initCountdown();
        this.loadCategories();
        this.loadProducts();
        this.loadBrands();
        this.loadDeals();
        this.initScrollEffects();
        this.initSearch();
        this.initCart();
        this.initWishlist();

        this.initScrollToTop();
    }

    // ===== LOADING SCREEN =====
    hideLoadingScreen() {
        setTimeout(() => {
            const loadingScreen = document.getElementById('loadingScreen');
            if (loadingScreen) {
                loadingScreen.classList.add('hidden');
                setTimeout(() => {
                    loadingScreen.style.display = 'none';
                }, 500);
            }
        }, 1500);
    }

    // ===== EVENT LISTENERS =====
    initEventListeners() {
        // Navigation
        document.querySelectorAll('.nav-link').forEach(link => {
            link.addEventListener('click', (e) => {
                if (link.getAttribute('href').startsWith('#')) {
                    e.preventDefault();
                    this.smoothScrollTo(link.getAttribute('href'));
                    this.updateActiveNav(link);
                }
            });
        });

        // Search
        const searchBtn = document.getElementById('searchBtn');
        const searchInput = document.getElementById('searchInput');
        
        if (searchBtn) {
            searchBtn.addEventListener('click', () => this.performSearch());
        }
        
        if (searchInput) {
            searchInput.addEventListener('input', (e) => this.handleSearchInput(e));
            searchInput.addEventListener('keypress', (e) => {
                if (e.key === 'Enter') this.performSearch();
            });
        }

        // Cart
        const cartBtn = document.getElementById('cartBtn');
        const closeCart = document.getElementById('closeCart');
        const cartOverlay = document.getElementById('cartOverlay');
        
        if (cartBtn) {
            cartBtn.addEventListener('click', () => this.toggleCart());
        }
        
        if (closeCart) {
            closeCart.addEventListener('click', () => this.toggleCart());
        }
        
        if (cartOverlay) {
            cartOverlay.addEventListener('click', () => this.toggleCart());
        }

        // Wishlist
        const wishlistBtn = document.getElementById('wishlistBtn');
        if (wishlistBtn) {
            wishlistBtn.addEventListener('click', () => this.toggleWishlist());
        }

        // Load more products
        const loadMoreBtn = document.getElementById('loadMoreBtn');
        if (loadMoreBtn) {
            loadMoreBtn.addEventListener('click', () => this.loadMoreProducts());
        }

        // Product filters
        document.querySelectorAll('.filter-btn').forEach(btn => {
            btn.addEventListener('click', (e) => this.filterProducts(e.target.dataset.filter));
        });


    }

    // ===== SLIDER FUNCTIONALITY =====
    initSlider() {
        const slides = document.querySelectorAll('.slide');
        const indicators = document.querySelectorAll('.indicator');
        const prevBtn = document.getElementById('prevSlide');
        const nextBtn = document.getElementById('nextSlide');

        if (slides.length === 0) return;

        // Auto-slide
        this.startAutoSlide();

        // Manual controls
        if (prevBtn) {
            prevBtn.addEventListener('click', () => this.prevSlide());
        }
        
        if (nextBtn) {
            nextBtn.addEventListener('click', () => this.nextSlide());
        }

        // Indicators
        indicators.forEach((indicator, index) => {
            indicator.addEventListener('click', () => this.goToSlide(index));
        });

        // Pause on hover
        const heroSection = document.querySelector('.hero-section');
        if (heroSection) {
            heroSection.addEventListener('mouseenter', () => this.pauseAutoSlide());
            heroSection.addEventListener('mouseleave', () => this.startAutoSlide());
        }
    }

    startAutoSlide() {
        if (this.slideInterval) clearInterval(this.slideInterval);
        this.slideInterval = setInterval(() => this.nextSlide(), 5000);
    }

    pauseAutoSlide() {
        if (this.slideInterval) {
            clearInterval(this.slideInterval);
            this.slideInterval = null;
        }
    }

    nextSlide() {
        const slides = document.querySelectorAll('.slide');
        const indicators = document.querySelectorAll('.indicator');
        
        slides[this.currentSlide].classList.remove('active');
        indicators[this.currentSlide].classList.remove('active');
        
        this.currentSlide = (this.currentSlide + 1) % slides.length;
        
        slides[this.currentSlide].classList.add('active');
        indicators[this.currentSlide].classList.add('active');
    }

    prevSlide() {
        const slides = document.querySelectorAll('.slide');
        const indicators = document.querySelectorAll('.indicator');
        
        slides[this.currentSlide].classList.remove('active');
        indicators[this.currentSlide].classList.remove('active');
        
        this.currentSlide = (this.currentSlide - 1 + slides.length) % slides.length;
        
        slides[this.currentSlide].classList.add('active');
        indicators[this.currentSlide].classList.add('active');
    }

    goToSlide(index) {
        const slides = document.querySelectorAll('.slide');
        const indicators = document.querySelectorAll('.indicator');
        
        slides[this.currentSlide].classList.remove('active');
        indicators[this.currentSlide].classList.remove('active');
        
        this.currentSlide = index;
        
        slides[this.currentSlide].classList.add('active');
        indicators[this.currentSlide].classList.add('active');
    }

    // ===== COUNTDOWN TIMER =====
    initCountdown() {
        // Set target date (24 hours from now)
        const targetDate = new Date();
        targetDate.setHours(targetDate.getHours() + 24);
        
        const updateCountdown = () => {
            const now = new Date().getTime();
            const distance = targetDate.getTime() - now;
            
            if (distance < 0) {
                // Reset countdown
                targetDate.setHours(targetDate.getHours() + 24);
                return;
            }
            
            const hours = Math.floor(distance / (1000 * 60 * 60));
            const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            const seconds = Math.floor((distance % (1000 * 60)) / 1000);
            
            const hoursEl = document.getElementById('hours');
            const minutesEl = document.getElementById('minutes');
            const secondsEl = document.getElementById('seconds');
            
            if (hoursEl) hoursEl.textContent = hours.toString().padStart(2, '0');
            if (minutesEl) minutesEl.textContent = minutes.toString().padStart(2, '0');
            if (secondsEl) secondsEl.textContent = seconds.toString().padStart(2, '0');
        };
        
        updateCountdown();
        setInterval(updateCountdown, 1000);
    }

    // ===== DATA LOADING (Simulated API calls) =====
    async loadCategories() {
        // Simulate API call
        const categories = [
            { id: 1, name: 'Telefonlar', icon: 'fas fa-mobile-alt', count: 156, color: '#6366f1' },
            { id: 2, name: 'Laptoplar', icon: 'fas fa-laptop', count: 89, color: '#f59e0b' },
            { id: 3, name: 'Tabletler', icon: 'fas fa-tablet-alt', count: 67, color: '#10b981' },
            { id: 4, name: 'Kulaklıklar', icon: 'fas fa-headphones', count: 234, color: '#ef4444' },
            { id: 5, name: 'Akıllı Saatler', icon: 'fas fa-clock', count: 123, color: '#8b5cf6' },
            { id: 6, name: 'Gaming', icon: 'fas fa-gamepad', count: 78, color: '#06b6d4' }
        ];
        
        this.renderCategories(categories);
    }

    async loadProducts() {
        // Simulate API call
        const products = [
            {
                id: 1, name: 'iPhone 15 Pro', price: 89999, oldPrice: 99999, 
                image: 'https://images.unsplash.com/photo-1592750475338-74b7b21085ab?w=400',
                category: 'phones', rating: 4.8, reviews: 1247, discount: 10
            },
            {
                id: 2, name: 'MacBook Air M2', price: 45999, oldPrice: 52999,
                image: 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400',
                category: 'laptops', rating: 4.9, reviews: 892, discount: 13
            },
            {
                id: 3, name: 'AirPods Pro 2', price: 7999, oldPrice: 8999,
                image: 'https://images.unsplash.com/photo-1606220945770-b5b6c2c55bf1?w=400',
                category: 'accessories', rating: 4.7, reviews: 2156, discount: 11
            },
            {
                id: 4, name: 'Samsung Galaxy S24', price: 34999, oldPrice: 39999,
                image: 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400',
                category: 'phones', rating: 4.6, reviews: 987, discount: 12
            },
            {
                id: 5, name: 'Dell XPS 13', price: 38999, oldPrice: 44999,
                image: 'https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?w=400',
                category: 'laptops', rating: 4.5, reviews: 654, discount: 13
            },
            {
                id: 6, name: 'Sony WH-1000XM5', price: 12999, oldPrice: 14999,
                image: 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400',
                category: 'accessories', rating: 4.9, reviews: 1876, discount: 13
            },
            {
                id: 7, name: 'Apple Watch Series 9', price: 18999, oldPrice: 21999,
                image: 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400',
                category: 'accessories', rating: 4.8, reviews: 1432, discount: 14
            },
            {
                id: 8, name: 'PlayStation 5', price: 24999, oldPrice: 27999,
                image: 'https://images.unsplash.com/photo-1606813907291-d86efa9b94db?w=400',
                category: 'gaming', rating: 4.9, reviews: 2341, discount: 11
            }
        ];
        
        this.renderProducts(products);
    }

    async loadBrands() {
        // Simulate API call
        const brands = [
            { id: 1, name: 'Apple', logo: 'https://via.placeholder.com/150x80/ffffff/666666?text=Apple', url: '#' },
            { id: 2, name: 'Samsung', logo: 'https://via.placeholder.com/150x80/ffffff/666666?text=Samsung', url: '#' },
            { id: 3, name: 'Sony', logo: 'https://via.placeholder.com/150x80/ffffff/666666?text=Sony', url: '#' },
            { id: 4, name: 'Dell', logo: 'https://via.placeholder.com/150x80/ffffff/666666?text=Dell', url: '#' },
            { id: 5, name: 'HP', logo: 'https://via.placeholder.com/150x80/ffffff/666666?text=HP', url: '#' },
            { id: 6, name: 'Lenovo', logo: 'https://via.placeholder.com/150x80/ffffff/666666?text=Lenovo', url: '#' }
        ];
        
        this.renderBrands(brands);
    }

    async loadDeals() {
        // Simulate API call
        const deals = [
            {
                id: 1, name: 'iPhone 15 Pro Max', price: 109999, oldPrice: 129999,
                image: 'https://images.unsplash.com/photo-1592750475338-74b7b21085ab?w=400',
                discount: 15, timeLeft: '23:59:59'
            },
            {
                id: 2, name: 'MacBook Pro M3', price: 89999, oldPrice: 109999,
                image: 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400',
                discount: 18, timeLeft: '23:59:59'
            },
            {
                id: 3, name: 'iPad Pro 12.9', price: 39999, oldPrice: 49999,
                image: 'https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400',
                discount: 20, timeLeft: '23:59:59'
            }
        ];
        
        this.renderDeals(deals);
    }

    // ===== RENDERING FUNCTIONS =====
    renderCategories(categories) {
        const container = document.getElementById('categoriesGrid');
        if (!container) return;

        const html = categories.map(category => `
            <div class="category-card" data-category-id="${category.id}">
                <div class="category-icon" style="background: ${category.color}">
                    <i class="${category.icon}"></i>
                </div>
                <div class="category-content">
                    <h3>${category.name}</h3>
                    <p>${category.count} ürün</p>
                </div>
            </div>
        `).join('');

        container.innerHTML = html;

        // Add click events
        container.querySelectorAll('.category-card').forEach(card => {
            card.addEventListener('click', () => {
                const categoryId = card.dataset.categoryId;
                this.filterProducts(categoryId);
            });
        });
    }

    renderProducts(products) {
        const container = document.getElementById('productsGrid');
        if (!container) return;

        const html = products.map(product => `
            <div class="product-card" data-category="${product.category}" data-product-id="${product.id}">
                <div class="product-image">
                    <img src="${product.image}" alt="${product.name}">
                    ${product.discount ? `<span class="discount-badge">%${product.discount}</span>` : ''}
                    <div class="product-actions">
                        <button class="action-btn wishlist-btn" data-product-id="${product.id}">
                            <i class="fas fa-heart"></i>
                        </button>
                        <button class="action-btn quick-view-btn" data-product-id="${product.id}">
                            <i class="fas fa-eye"></i>
                        </button>
                    </div>
                </div>
                <div class="product-info">
                    <h3>${product.name}</h3>
                    <div class="product-rating">
                        <div class="stars">
                            ${this.generateStars(product.rating)}
                        </div>
                        <span class="rating-text">${product.rating} (${product.reviews})</span>
                    </div>
                    <div class="product-price">
                        <span class="current-price">₺${product.price.toLocaleString()}</span>
                        ${product.oldPrice ? `<span class="old-price">₺${product.oldPrice.toLocaleString()}</span>` : ''}
                    </div>
                    <button class="btn-add-cart" data-product-id="${product.id}">
                        <i class="fas fa-shopping-cart"></i>
                        Sepete Ekle
                    </button>
                </div>
            </div>
        `).join('');

        container.innerHTML = html;
        this.productsLoaded = products.length;

        // Add event listeners
        this.addProductEventListeners();
    }

    renderBrands(brands) {
        const container = document.getElementById('brandsGrid');
        if (!container) return;

        const html = brands.map(brand => `
            <div class="brand-card">
                <a href="${brand.url}">
                    <img src="${brand.logo}" alt="${brand.name}">
                </a>
            </div>
        `).join('');

        container.innerHTML = html;
    }

    renderDeals(deals) {
        const container = document.getElementById('dealsGrid');
        if (!container) return;

        const html = deals.map(deal => `
            <div class="deal-card">
                <div class="deal-image">
                    <img src="${deal.image}" alt="${deal.name}">
                    <span class="deal-discount">%${deal.discount}</span>
                </div>
                <div class="deal-info">
                    <h3>${deal.name}</h3>
                    <div class="deal-price">
                        <span class="current-price">₺${deal.price.toLocaleString()}</span>
                        <span class="old-price">₺${deal.oldPrice.toLocaleString()}</span>
                    </div>
                    <div class="deal-timer">
                        <span class="timer-label">Kalan Süre:</span>
                        <span class="timer-value">${deal.timeLeft}</span>
                    </div>
                    <button class="btn-primary">Hemen Satın Al</button>
                </div>
            </div>
        `).join('');

        container.innerHTML = html;
    }

    // ===== UTILITY FUNCTIONS =====
    generateStars(rating) {
        const fullStars = Math.floor(rating);
        const hasHalfStar = rating % 1 !== 0;
        const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);

        let stars = '';
        for (let i = 0; i < fullStars; i++) {
            stars += '<i class="fas fa-star"></i>';
        }
        if (hasHalfStar) {
            stars += '<i class="fas fa-star-half-alt"></i>';
        }
        for (let i = 0; i < emptyStars; i++) {
            stars += '<i class="far fa-star"></i>';
        }

        return stars;
    }

    // ===== PRODUCT FILTERING =====
    filterProducts(category) {
        this.currentFilter = category;
        
        // Update filter buttons
        document.querySelectorAll('.filter-btn').forEach(btn => {
            btn.classList.toggle('active', btn.dataset.filter === category);
        });

        // Filter products
        const products = document.querySelectorAll('.product-card');
        products.forEach(product => {
            const productCategory = product.dataset.category;
            const shouldShow = category === 'all' || productCategory === category;
            product.style.display = shouldShow ? 'block' : 'none';
        });
    }

    // ===== SEARCH FUNCTIONALITY =====
    initSearch() {
        // Search suggestions would be implemented here
        // For now, just handle basic search
    }

    handleSearchInput(e) {
        const query = e.target.value.toLowerCase();
        // Implement search suggestions here
    }

    performSearch() {
        const searchInput = document.getElementById('searchInput');
        const searchCategory = document.getElementById('searchCategory');
        
        if (!searchInput || !searchInput.value.trim()) return;

        const query = searchInput.value.trim();
        const category = searchCategory.value;
        
        // Implement search functionality here
        console.log(`Searching for "${query}" in category: ${category}`);
        
        // Show search results or redirect to search page
        this.showNotification(`"${query}" için arama yapılıyor...`, 'info');
    }

    // ===== CART FUNCTIONALITY =====
    initCart() {
        // Load cart from localStorage
        this.cartItems = JSON.parse(localStorage.getItem('cartItems')) || [];
        this.updateCartCount();
    }

    addToCart(productId) {
        // Find product details
        const product = this.findProductById(productId);
        if (!product) return;

        // Check if already in cart
        const existingItem = this.cartItems.find(item => item.id === productId);
        if (existingItem) {
            existingItem.quantity += 1;
        } else {
            this.cartItems.push({
                id: product.id,
                name: product.name,
                price: product.price,
                image: product.image,
                quantity: 1
            });
        }

        // Save to localStorage
        localStorage.setItem('cartItems', JSON.stringify(this.cartItems));
        
        // Update UI
        this.updateCartCount();
        this.renderCartItems();
        
        // Show notification
        this.showNotification(`${product.name} sepete eklendi!`, 'success');
    }

    removeFromCart(productId) {
        this.cartItems = this.cartItems.filter(item => item.id !== productId);
        localStorage.setItem('cartItems', JSON.stringify(this.cartItems));
        this.updateCartCount();
        this.renderCartItems();
    }

    updateCartQuantity(productId, quantity) {
        const item = this.cartItems.find(item => item.id === productId);
        if (item) {
            item.quantity = Math.max(1, quantity);
            localStorage.setItem('cartItems', JSON.stringify(this.cartItems));
            this.renderCartItems();
        }
    }

    updateCartCount() {
        const cartCount = document.getElementById('cartCount');
        if (cartCount) {
            const totalItems = this.cartItems.reduce((sum, item) => sum + item.quantity, 0);
            cartCount.textContent = totalItems;
        }
    }

    renderCartItems() {
        const container = document.getElementById('cartItems');
        if (!container) return;

        if (this.cartItems.length === 0) {
            container.innerHTML = '<div class="empty-cart">Sepetiniz boş</div>';
            return;
        }

        const html = this.cartItems.map(item => `
            <div class="cart-item">
                <img src="${item.image}" alt="${item.name}">
                <div class="cart-item-info">
                    <h4>${item.name}</h4>
                    <div class="cart-item-price">₺${item.price.toLocaleString()}</div>
                    <div class="cart-item-quantity">
                        <button class="qty-btn" onclick="techStore.updateCartQuantity(${item.id}, ${item.quantity - 1})">-</button>
                        <span>${item.quantity}</span>
                        <button class="qty-btn" onclick="techStore.updateCartQuantity(${item.id}, ${item.quantity + 1})">+</button>
                    </div>
                </div>
                <button class="remove-item" onclick="techStore.removeFromCart(${item.id})">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `).join('');

        container.innerHTML = html;

        // Update total
        this.updateCartTotal();
    }

    updateCartTotal() {
        const totalElement = document.getElementById('cartTotal');
        if (totalElement) {
            const total = this.cartItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
            totalElement.textContent = `₺${total.toLocaleString()}`;
        }
    }

    toggleCart() {
        const cartSidebar = document.getElementById('cartSidebar');
        const cartOverlay = document.getElementById('cartOverlay');
        
        if (cartSidebar && cartOverlay) {
            cartSidebar.classList.toggle('active');
            cartOverlay.classList.toggle('active');
            
            if (cartSidebar.classList.contains('active')) {
                this.renderCartItems();
            }
        }
    }

    // ===== WISHLIST FUNCTIONALITY =====
    initWishlist() {
        this.wishlistItems = JSON.parse(localStorage.getItem('wishlistItems')) || [];
        this.updateWishlistCount();
    }

    toggleWishlist() {
        // Implement wishlist functionality
        this.showNotification('Favoriler sayfası yakında açılacak!', 'info');
    }

    updateWishlistCount() {
        const wishlistCount = document.getElementById('wishlistCount');
        if (wishlistCount) {
            wishlistCount.textContent = this.wishlistItems.length;
        }
    }



    // ===== SCROLL EFFECTS =====
    initScrollEffects() {
        // Header scroll effect
        window.addEventListener('scroll', () => {
            const header = document.getElementById('header');
            if (header) {
                if (window.scrollY > 100) {
                    header.classList.add('scrolled');
                } else {
                    header.classList.remove('scrolled');
                }
            }
        });

        // Smooth scroll for navigation
        this.initSmoothScroll();
    }

    initSmoothScroll() {
        // Smooth scroll implementation
    }

    smoothScrollTo(targetId) {
        const target = document.querySelector(targetId);
        if (target) {
            const headerHeight = document.querySelector('.header').offsetHeight;
            const targetPosition = target.offsetTop - headerHeight;
            
            window.scrollTo({
                top: targetPosition,
                behavior: 'smooth'
            });
        }
    }

    updateActiveNav(clickedLink) {
        document.querySelectorAll('.nav-link').forEach(link => {
            link.classList.remove('active');
        });
        clickedLink.classList.add('active');
    }

    // ===== SCROLL TO TOP =====
    initScrollToTop() {
        const scrollToTopBtn = document.getElementById('scrollToTop');
        if (!scrollToTopBtn) return;

        window.addEventListener('scroll', () => {
            if (window.scrollY > 300) {
                scrollToTopBtn.classList.add('visible');
            } else {
                scrollToTopBtn.classList.remove('visible');
            }
        });

        scrollToTopBtn.addEventListener('click', () => {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }

    // ===== PRODUCT EVENT LISTENERS =====
    addProductEventListeners() {
        // Add to cart buttons
        document.querySelectorAll('.btn-add-cart').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const productId = parseInt(e.target.dataset.productId);
                this.addToCart(productId);
            });
        });

        // Wishlist buttons
        document.querySelectorAll('.wishlist-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const productId = parseInt(e.target.dataset.productId);
                this.toggleWishlistItem(productId);
            });
        });

        // Quick view buttons
        document.querySelectorAll('.quick-view-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const productId = parseInt(e.target.dataset.productId);
                this.showQuickView(productId);
            });
        });
    }

    toggleWishlistItem(productId) {
        const index = this.wishlistItems.indexOf(productId);
        if (index > -1) {
            this.wishlistItems.splice(index, 1);
            this.showNotification('Favorilerden kaldırıldı', 'info');
        } else {
            this.wishlistItems.push(productId);
            this.showNotification('Favorilere eklendi', 'success');
        }
        
        localStorage.setItem('wishlistItems', JSON.stringify(this.wishlistItems));
        this.updateWishlistCount();
    }

    showQuickView(productId) {
        const product = this.findProductById(productId);
        if (!product) return;

        // Implement quick view modal
        this.showNotification(`${product.name} hızlı görünümü yakında!`, 'info');
    }

    findProductById(productId) {
        // This would normally come from an API or data store
        // For now, return a mock product
        return {
            id: productId,
            name: 'Ürün ' + productId,
            price: 9999,
            image: 'https://via.placeholder.com/300x300'
        };
    }

    // ===== LOAD MORE PRODUCTS =====
    loadMoreProducts() {
        // Implement load more functionality
        this.showNotification('Daha fazla ürün yükleniyor...', 'info');
    }

    // ===== NOTIFICATIONS =====
    showNotification(message, type = 'info') {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;
        notification.innerHTML = `
            <div class="notification-content">
                <span class="notification-message">${message}</span>
                <button class="notification-close">&times;</button>
            </div>
        `;

        // Add styles
        notification.style.cssText = `
            position: fixed;
            top: 100px;
            right: 20px;
            background: ${this.getNotificationColor(type)};
            color: white;
            padding: 15px 20px;
            border-radius: 10px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.2);
            z-index: 10000;
            transform: translateX(400px);
            transition: transform 0.3s ease;
            max-width: 300px;
        `;

        // Add to page
        document.body.appendChild(notification);

        // Animate in
        setTimeout(() => {
            notification.style.transform = 'translateX(0)';
        }, 100);

        // Close button
        const closeBtn = notification.querySelector('.notification-close');
        closeBtn.addEventListener('click', () => {
            this.closeNotification(notification);
        });

        // Auto close
        setTimeout(() => {
            this.closeNotification(notification);
        }, 5000);
    }

    getNotificationColor(type) {
        const colors = {
            success: '#10b981',
            error: '#ef4444',
            warning: '#f59e0b',
            info: '#3b82f6'
        };
        return colors[type] || colors.info;
    }

    closeNotification(notification) {
        notification.style.transform = 'translateX(400px)';
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 300);
    }
}

// ===== INITIALIZATION =====
let techStore;

document.addEventListener('DOMContentLoaded', () => {
    techStore = new TechStore();
});

// ===== GLOBAL FUNCTIONS =====
// These functions are called from HTML onclick attributes
window.addToCart = (productId) => {
    if (techStore) techStore.addToCart(productId);
};

window.removeFromCart = (productId) => {
    if (techStore) techStore.removeFromCart(productId);
};

window.updateCartQuantity = (productId, quantity) => {
    if (techStore) techStore.updateCartQuantity(productId, quantity);
};
