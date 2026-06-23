redirectIfNotLoggedIn();

const params = new URLSearchParams(window.location.search);
const eventId = parseInt(params.get('id'));
const user = getUser();
let eventData = null;
let isManager = false;
let isHost = false;

async function init() {
    eventData = await apiFetch(`/events/${eventId}`);
    isManager = eventData.eventManagerId === user.id;
    isHost = eventData.eventHostId === user.id;

    const contextRole = isManager ? 'Manager' : isHost ? 'Host' : 'Participant';
    buildNavWithRole(contextRole);

    document.getElementById('event-title-section').innerHTML = `
        <h1 style="margin-top:8px">${eventData.title}</h1>
        <p style="color:var(--muted);font-size:14px">${formatDate(eventData.date)} &bull; ${eventData.location}
        ${isManager ? '<span class="badge badge-manager" style="margin-left:8px">Manager</span>' : isHost ? '<span class="badge badge-host" style="margin-left:8px">Host</span>' : '<span class="badge badge-participant" style="margin-left:8px">Participant</span>'}</p>
    `;

    let tabs;
    if (isManager) tabs = ['Details', 'Participants', 'Financial', 'Vendors', 'Polls'];
    else if (isHost) tabs = ['Details'];
    else tabs = ['Details', 'Polls', 'Attendance', 'Settings'];

    document.getElementById('event-tabs').innerHTML = tabs.map((t, i) =>
        `<button class="tab-btn ${i===0?'active':''}" onclick="loadTab('${t.toLowerCase()}')">${t}</button>`
    ).join('');

    loadTab('details');
}

function buildNavWithRole(contextRole) {
    const nav = document.getElementById('main-nav');
    if (!nav) return;
    nav.innerHTML = `
        <a href="/HtmlFiles/dashboard.html" class="logo-text">
            <img src="/gatherup-logo.png" alt="GatherUp" style="width:30px;height:30px;object-fit:contain;border-radius:6px">
            GatherUp
        </a>
        <div style="display:flex;align-items:center;gap:4px">
            <a href="/HtmlFiles/dashboard.html">My Events</a>
            <a href="/HtmlFiles/profile.html">Profile</a>
            <span class="nav-user-chip">${user.name} &bull; ${contextRole}</span>
            <button onclick="logout()" class="nav-logout-btn">Sign out</button>
        </div>
    `;
}

async function loadTab(tab) {
    document.querySelectorAll('.tab-btn').forEach(b =>
        b.classList.toggle('active', b.textContent.toLowerCase() === tab));

    const content = document.getElementById('tab-content');
    content.innerHTML = '<div id="loading">Loading...</div>';

    if (tab === 'details') renderDetails();
    else if (tab === 'participants') renderParticipants();
    else if (tab === 'financial') renderFinancial();
    else if (tab === 'vendors') renderVendors();
    else if (tab === 'polls') renderPolls();
    else if (tab === 'attendance') renderAttendance();
    else if (tab === 'settings') renderSettings();
}

init();
