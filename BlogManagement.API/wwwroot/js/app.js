const App = {
    apiBase: "",
    
    getToken: function() {
        return localStorage.getItem('jwt_token');
    },
    
    setToken: function(token, user) {
        if (user) {
            user.id = user.id || user.userId;
            user.userId = user.userId || user.id;
            user.username = user.username || user.email || (user.firstName ? `${user.firstName} ${user.lastName}` : "User");
        }
        localStorage.setItem('jwt_token', token);
        localStorage.setItem('user', JSON.stringify(user));
    },
    
    clearToken: function() {
        localStorage.removeItem('jwt_token');
        localStorage.removeItem('user');
    },
    
    getUser: function() {
        const u = localStorage.getItem('user');
        if (!u) return null;
        try {
            const user = JSON.parse(u);
            if (user) {
                user.id = user.id || user.userId;
                user.userId = user.userId || user.id;
                user.username = user.username || user.email || (user.firstName ? `${user.firstName} ${user.lastName}` : "User");
            }
            return user;
        } catch (e) {
            return null;
        }
    },
    
    getCurrentUser: function() {
        return this.getUser();
    },
    
    isLoggedIn: function() {
        return !!this.getToken();
    },
    
    getUserRole: function() {
        const user = this.getUser();
        return user ? user.role : null;
    },
    
    ajax: function(options) {
        const token = this.getToken();
        const headers = options.headers || {};
        if (token) {
            headers['Authorization'] = 'Bearer ' + token;
        }
        
        return $.ajax({
            ...options,
            headers: headers
        });
    },

    showToast: function(message, type = 'success') {
        let container = $('#toast-container');
        if (!container.length) {
            container = $('<div id="toast-container" class="fixed top-5 right-5 z-[9999] space-y-2"></div>');
            $('body').append(container);
        }
        
        const bgColor = type === 'success' ? 'bg-primary text-white border-primary-container' : 'bg-red-600 text-white border-red-800';
        const icon = type === 'success' ? 'check_circle' : 'error';
        
        const toast = $(`
            <div class="flex items-center gap-sm px-md py-3 rounded-xl shadow-lg border animate-in fade-in slide-in-from-right-4 duration-300 ${bgColor}">
                <span class="material-symbols-outlined">${icon}</span>
                <span class="font-label-md text-label-md">${message}</span>
            </div>
        `);
        
        container.append(toast);
        setTimeout(() => {
            toast.addClass('animate-out fade-out slide-out-to-right-4 duration-300');
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    },

    getQueryParam: function(name) {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(name);
    },
    
    updateHeader: function () {
        console.trace('updateHeader called'); // shows full call stack
        const user = this.getUser();
        const authContainer = $('#auth-buttons-container');
        const navContainer = $('header nav');
        
        if (authContainer.length) {
            if (user) {
                let dashboardLink = '';
                if (user.role === 'Author') {
                    dashboardLink = `<a class="text-on-surface-variant hover:text-primary font-label-md text-label-md transition-colors duration-200 dynamic-nav" href="/Pages/dashboard.html">Dashboard</a>`;
                }
                
                let adminLink = '';
                if (user.role === 'Admin') {
                    adminLink = `<a class="text-on-surface-variant hover:text-primary font-label-md text-label-md transition-colors duration-200 dynamic-nav" href="/Pages/admin.html">Admin</a>`;
                }

                if (navContainer.length) {
                    navContainer.find('.dynamic-nav').remove();
                    if (dashboardLink) {
                        navContainer.append($(dashboardLink));
                    }
                    if (adminLink) {
                        navContainer.append($(adminLink));
                    }
                }

                authContainer.html(`
                    <div class="flex items-center gap-md">
                        <div class="flex flex-col items-end">
                            <span class="font-label-md text-label-md text-on-surface font-bold">${user.firstName} ${user.lastName}</span>
                            <span class="text-[10px] text-on-surface-variant capitalize font-semibold bg-surface-container-low px-sm py-[2px] rounded-full">${user.role}</span>
                        </div>
                        <button id="logout-btn" class="bg-surface-container border border-outline-variant text-on-surface font-label-md px-md py-base rounded-lg hover:bg-surface-container-high transition-all cursor-pointer">
                            Logout
                        </button>
                    </div>
                `);

                $('#logout-btn').on('click', () => {
                    this.logout();
                });
            } else {
                authContainer.html(`
                    <a href="/Pages/login.html" class="text-on-surface-variant font-label-md px-md py-base hover:bg-surface-container-low transition-all cursor-pointer">Login</a>
                    <a href="/Pages/register.html" class="bg-primary text-on-primary font-label-md px-md py-base rounded-lg hover:bg-primary-container transition-all cursor-pointer active:opacity-80">Register</a>
                `);
            }
        }
    },
    
    logout: function() {
        this.ajax({
            url: '/api/auth/logout',
            method: 'POST',
            success: () => {
                this.clearToken();
                this.showToast('Logged out successfully.');
                setTimeout(() => {
                    window.location.href = '/index.html';
                }, 1000);
            },
            error: () => {
                this.clearToken();
                window.location.href = '/index.html';
            }
        });
    },
    
    escapeHtml: function(text) {
        if (!text) return "";
        return text
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }
};

$(document).ready(function() {
    App.updateHeader();
});


