let allSystemParticipants = [];

async function renderParticipants() {
    const [participants, allUsers] = await Promise.all([
        apiFetch(`/events/${eventId}/participants`),
        apiFetch(`/events/all-users`)
    ]);
    const host = allUsers.find(u => u.id === eventData.eventHostId);
    const content = document.getElementById('tab-content');
    content.innerHTML = `
        <div class="card">
            <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:14px">
                <h2>Participants (${participants.length})</h2>
                <div>
                    <button class="btn-secondary btn-sm" onclick="sendInvitations()">Send Reminders</button>
                    <button class="btn-success btn-sm" style="margin-left:8px" onclick="showAddParticipant()">+ Add</button>
                </div>
            </div>
            <div id="participants-alert" class="alert"></div>
            ${host ? `
            <div class="host-banner">
                <div class="host-banner-avatar">${host.name.charAt(0).toUpperCase()}</div>
                <div class="host-banner-info">
                    <div class="host-banner-label">🏠 Event Host</div>
                    <div class="host-banner-name">${host.name}</div>
                    <div class="host-banner-sub">${host.email}</div>
                </div>
                <div class="host-banner-actions">
                    <div class="host-banner-badge">Host</div>
                    <button class="host-banner-btn" onclick="sendHostInvitation()">✉️ Send Invitation</button>
                </div>
            </div>` : ''}
            <div id="add-participant-form" class="hidden" style="margin-bottom:14px">
                <div style="display:flex;gap:8px;margin-bottom:6px">
                    <input type="text" id="add-p-search" placeholder="Search by name or email..." style="margin:0;flex:1" oninput="searchParticipant(this.value)">
                </div>
                <div id="participant-results" style="border:1px solid #ddd;border-radius:6px;display:none;max-height:150px;overflow-y:auto;background:white;margin-bottom:6px"></div>
                <input type="hidden" id="add-p-id">
                <small id="participant-selected" style="color:#27ae60;font-size:12px"></small>
                <div style="margin-top:8px">
                    <button class="btn-primary btn-sm" onclick="addParticipant()">Add Selected</button>
                </div>
            </div>
            <table>
                <tr><th>Name</th><th>Email</th><th>Attendance</th><th>Paid</th><th>Amount</th></tr>
                ${participants.map(p => `
                    <tr>
                        <td>${p.name}</td>
                        <td>${p.email}</td>
                        <td class="${p.isAttending === true ? 'tag-confirmed' : p.isAttending === false ? 'tag-declined' : 'tag-pending'}">
                            ${p.isAttending === true ? '✓ Confirmed' : p.isAttending === false ? '✗ Declined' : '? Pending'}
                        </td>
                        <td class="${p.hasPaid ? 'tag-paid' : 'tag-unpaid'}">${p.hasPaid ? 'Paid' : 'Unpaid'}</td>
                        <td>₪${p.amountContributed}</td>
                    </tr>
                `).join('')}
            </table>
        </div>
    `;
}

async function sendHostInvitation() {
    try {
        await apiFetch(`/events/${eventId}/send-host-invitation`, 'POST');
        showAlert('participants-alert', 'Host invitation sent!', 'success');
    } catch (e) { showAlert('participants-alert', e.message); }
}

function showAddParticipant() {
    const f = document.getElementById('add-participant-form');
    f.classList.toggle('hidden');
    f.style.display = f.classList.contains('hidden') ? 'none' : 'flex';
}

async function searchParticipant(query) {
    const resultsEl = document.getElementById('participant-results');
    if (query.length < 2) { resultsEl.style.display = 'none'; return; }

    if (allSystemParticipants.length === 0)
        allSystemParticipants = await apiFetch(`/events/${eventId}/participants/all`);

    const filtered = allSystemParticipants.filter(p =>
        p.name.toLowerCase().includes(query.toLowerCase()) || p.email.toLowerCase().includes(query.toLowerCase())
    );

    if (filtered.length === 0) {
        resultsEl.innerHTML = '<div style="padding:10px;color:#999;font-size:13px">No participants found</div>';
        resultsEl.style.display = 'block';
        return;
    }

    resultsEl.innerHTML = filtered.map(p => `
        <div onclick="selectParticipant(${p.id}, '${p.name}')"
             style="padding:10px;cursor:pointer;font-size:13px;border-bottom:1px solid #f0f0f0"
             onmouseover="this.style.background='#f0f7ff'" onmouseout="this.style.background='white'">
            ${p.name} <span style="color:#999;font-size:12px">${p.email}</span>
        </div>
    `).join('');
    resultsEl.style.display = 'block';
}

function selectParticipant(id, name) {
    document.getElementById('add-p-id').value = id;
    document.getElementById('add-p-search').value = name;
    document.getElementById('participant-selected').textContent = `✓ Selected: ${name}`;
    document.getElementById('participant-results').style.display = 'none';
}

async function addParticipant() {
    const pid = document.getElementById('add-p-id').value;
    if (!pid) { showAlert('participants-alert', 'Please select a participant first.'); return; }
    try {
        await apiFetch(`/events/${eventId}/participants/${pid}`, 'POST');
        showAlert('participants-alert', 'Participant added!', 'success');
        allSystemParticipants = [];
        renderParticipants();
    } catch (e) { showAlert('participants-alert', e.message); }
}

async function sendInvitations() {
    try {
        await apiFetch(`/participants/events/${eventId}/send-invitations`, 'POST');
        showAlert('participants-alert', 'Reminders sent!', 'success');
    } catch (e) { showAlert('participants-alert', e.message); }
}
