// Vendor Portal - Main JavaScript File

// Document Ready
$(document).ready(function () {
    console.log("Vendor Portal initialized");

    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Confirm delete actions
    $('.btn-delete').on('click', function (e) {
        if (!confirm('Bu kaydı silmek istediğinizden emin misiniz?')) {
            e.preventDefault();
        }
    });

    // Form validation
    $('form').on('submit', function (e) {
        var form = $(this);
        if (!form[0].checkValidity()) {
            e.preventDefault();
            e.stopPropagation();
        }
        form.addClass('was-validated');
    });
});

// API Helper Functions
const VendorPortalAPI = {
    baseUrl: '/api',
    token: null,

    setToken: function (token) {
        this.token = token;
        localStorage.setItem('vendorPortalToken', token);
    },

    getToken: function () {
        if (!this.token) {
            this.token = localStorage.getItem('vendorPortalToken');
        }
        return this.token;
    },

    makeRequest: function (method, endpoint, data = null) {
        const url = `${this.baseUrl}${endpoint}`;
        const headers = {
            'Content-Type': 'application/json'
        };

        const token = this.getToken();
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }

        const config = {
            method: method,
            headers: headers
        };

        if (data && (method === 'POST' || method === 'PUT')) {
            config.body = JSON.stringify(data);
        }

        return fetch(url, config)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .catch(error => {
                console.error('API Error:', error);
                throw error;
            });
    },

    // Login
    login: function (email, password) {
        return this.makeRequest('POST', '/AccountApi/Login', { email, password })
            .then(response => {
                if (response.token) {
                    this.setToken(response.token);
                }
                return response;
            });
    },

    // Sipariş işlemleri
    getSiparisler: function (firmaId, isMusteriView = true) {
        return this.makeRequest('GET', `/SiparisApi/GetSiparislerByFirma/${firmaId}?isMusteriView=${isMusteriView}`);
    },

    createSiparis: function (siparisData) {
        return this.makeRequest('POST', '/SiparisApi/CreateSiparis', siparisData);
    },

    bulkCreateSiparisler: function (siparislerData) {
        return this.makeRequest('POST', '/SiparisApi/PostSiparisler', siparislerData);
    }
};

// Utility Functions
function showNotification(message, type = 'info') {
    const alertClass = `alert-${type}`;
    const icon = type === 'success' ? 'check-circle' : 'exclamation-circle';
    
    const alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            <i class="fas fa-${icon}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    
    $('.container main').prepend(alertHtml);
    
    setTimeout(function() {
        $('.alert').fadeOut('slow', function() {
            $(this).remove();
        });
    }, 5000);
}

function formatCurrency(amount, currency = 'TRY') {
    const formatter = new Intl.NumberFormat('tr-TR', {
        style: 'currency',
        currency: currency
    });
    return formatter.format(amount);
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('tr-TR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });
}

function formatDateTime(dateString) {
    const date = new Date(dateString);
    return date.toLocaleString('tr-TR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Export for use in other files
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { VendorPortalAPI, showNotification, formatCurrency, formatDate, formatDateTime };
}
