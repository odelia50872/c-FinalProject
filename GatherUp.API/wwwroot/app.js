const API = '/api';

function getToken() { return localStorage.getItem('token'); }
function getUser() { return JSON.parse(localStorage.getItem('user') || 'null'); }
function isLoggedIn() { return !!getToken(); }

function redirectIfNotLoggedIn() {
    if (!isLoggedIn()) { window.location.href = '/index.html'; }
}

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/index.html';
}

async function apiFetch(path, method = 'GET', body = null) {
    const headers = { 'Content-Type': 'application/json' };
    const token = getToken();
    if (token) headers['Authorization'] = `Bearer ${token}`;

    const res = await fetch(API + path, {
        method,
        headers,
        body: body ? JSON.stringify(body) : null
    });

    if (res.status === 401 && path !== '/auth/login') { logout(); return; }

    const text = await res.text();
    const data = text ? JSON.parse(text) : null;

    if (!res.ok) throw new Error(data?.error || `Error ${res.status}`);
    return data;
}

function showAlert(id, message, type = 'error') {
    const el = document.getElementById(id);
    if (el) {
        el.className = `alert alert-${type}`;
        el.textContent = message;
        el.style.display = 'block';
        setTimeout(() => el.style.display = 'none', 4000);
        return;
    }
    // fallback — toast שמופיע בכל דף גם בלי alert div
    showToast(message, type);
}

function showToast(message, type = 'error') {
    let toast = document.getElementById('global-toast');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'global-toast';
        toast.style.cssText = `
            position:fixed;bottom:24px;left:50%;transform:translateX(-50%);
            padding:12px 24px;border-radius:8px;font-size:14px;font-weight:500;
            z-index:9999;box-shadow:0 4px 12px rgba(0,0,0,0.15);max-width:400px;text-align:center;
        `;
        document.body.appendChild(toast);
    }
    toast.style.background = type === 'error' ? '#e74c3c' : '#27ae60';
    toast.style.color = 'white';
    toast.textContent = message;
    toast.style.display = 'block';
    clearTimeout(toast._timeout);
    toast._timeout = setTimeout(() => toast.style.display = 'none', 4000);
}

function formatDate(dateStr) {
    return new Date(dateStr).toLocaleDateString('en-GB');
}

function setNavUser() {
    const user = getUser();
    const el = document.getElementById('nav-user');
    if (el && user) el.textContent = `${user.name} (${user.role})`;
}

function buildNav(active) {
    const user = getUser();
    if (!user) return;
    const nav = document.getElementById('main-nav');
    if (!nav) return;
    nav.innerHTML = `
        <a href="/dashboard.html" class="logo-text">
            <img src="/gatherup-logo.png" alt="GatherUp" style="width:30px;height:30px;object-fit:contain;border-radius:6px">
            GatherUp
        </a>
        <div style="display:flex;align-items:center;gap:4px">
            <a href="/dashboard.html" ${active==='dashboard'?'class="nav-active"':''}>My Events</a>
            <a href="/profile.html" ${active==='profile'?'class="nav-active"':''}>Profile</a>
            <span class="nav-user-chip">${user.name}</span>
            <button onclick="logout()" class="nav-logout-btn">Sign out</button>
        </div>
    `;
}
