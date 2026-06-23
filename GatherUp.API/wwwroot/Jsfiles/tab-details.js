function renderDetails() {
    const content = document.getElementById('tab-content');
    const viewMode = isManager ? `
        <div class="form-row"><label>Title</label><input type="text" id="d-title" value="${eventData.title}" disabled></div>
        <div class="form-row"><label>Date</label><input type="date" id="d-date" value="${eventData.date?.split('T')[0]}" disabled></div>
        <div class="form-row"><label>Location</label><input type="text" id="d-location" value="${eventData.location}" disabled></div>
        <button class="btn-secondary" onclick="enableDetailsEdit()">✏️ Change Details</button>
    ` : `
        <div class="form-row"><label>Title</label><input type="text" value="${eventData.title}" disabled></div>
        <div class="form-row"><label>Date</label><input type="date" value="${eventData.date?.split('T')[0]}" disabled></div>
        <div class="form-row"><label>Location</label><input type="text" value="${eventData.location}" disabled></div>
    `;
    content.innerHTML = `
        <div class="card">
            <h2>Event Details</h2>
            <div id="details-alert" class="alert"></div>
            ${viewMode}
        </div>
    `;
}

function enableDetailsEdit() {
    document.getElementById('d-title').disabled = false;
    document.getElementById('d-date').disabled = false;
    document.getElementById('d-location').disabled = false;
    const btn = document.querySelector('#tab-content .btn-secondary');
    btn.className = 'btn-primary';
    btn.textContent = 'Save Changes';
    btn.setAttribute('onclick', 'updateDetails()');
}

async function updateDetails() {
    try {
        await apiFetch(`/events/${eventId}`, 'PUT', {
            title: document.getElementById('d-title').value,
            date: document.getElementById('d-date').value,
            location: document.getElementById('d-location').value
        });
        showAlert('details-alert', 'Event updated successfully!', 'success');
        eventData = await apiFetch(`/events/${eventId}`);
        setTimeout(() => renderDetails(), 1000);
    } catch (e) { showAlert('details-alert', e.message); }
}
