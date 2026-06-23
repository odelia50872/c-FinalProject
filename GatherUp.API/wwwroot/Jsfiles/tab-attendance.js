async function renderAttendance() {
    const status = await apiFetch(`/participants/${user.id}/event/${eventId}/status`);
    const attending = status.isAttending;

    let statusHtml = '';
    if (attending === true) {
        statusHtml = `
            <div class="status-confirmed">
                <span style="font-size:20px">✓</span>
                <div>
                    <div>You have confirmed your attendance.</div>
                    <div style="font-size:12px;font-weight:400;margin-top:2px;opacity:0.8">Changed your mind?</div>
                </div>
            </div>
            <button class="btn-secondary btn-sm" onclick="changeAttendance()">↩ Change Response</button>`;
    } else if (attending === false) {
        statusHtml = `
            <div class="status-declined">
                <span style="font-size:20px">✗</span>
                <div>
                    <div>You have declined this event.</div>
                    <div style="font-size:12px;font-weight:400;margin-top:2px;opacity:0.8">Changed your mind?</div>
                </div>
            </div>
            <button class="btn-secondary btn-sm" onclick="changeAttendance()">↩ Change Response</button>`;
    } else {
        statusHtml = `
            <p style="color:var(--muted);margin-bottom:20px">Are you attending <strong>${eventData.title}</strong> on ${formatDate(eventData.date)} at ${eventData.location}?</p>
            <div class="form-actions" id="attend-btns">
                <button class="btn-success" onclick="confirmAttend(true, this)">✓ Yes, I'm attending</button>
                <button class="btn-danger" onclick="confirmAttend(false, this)">✗ No, I can't attend</button>
            </div>`;
    }

    document.getElementById('tab-content').innerHTML = `
        <div class="card">
            <h2>Your Attendance</h2>
            <div id="attend-alert" class="alert"></div>
            ${statusHtml}
        </div>
    `;
}

function changeAttendance() {
    document.querySelector('#tab-content .card').innerHTML = `
        <h2>Update Your Attendance</h2>
        <div id="attend-alert" class="alert"></div>
        <p style="color:var(--muted);margin-bottom:20px">Are you attending <strong>${eventData.title}</strong>?</p>
        <div class="form-actions" id="attend-btns">
            <button class="btn-success" onclick="confirmAttend(true, this)">✓ Yes, I'm attending</button>
            <button class="btn-danger" onclick="confirmAttend(false, this)">✗ No, I can't attend</button>
        </div>
    `;
}

async function confirmAttend(isAttending, clickedBtn) {
    document.querySelectorAll('#attend-btns button').forEach(b => {
        b.disabled = true;
        b.style.opacity = '0.6';
    });
    if (clickedBtn) clickedBtn.textContent = 'Saving...';
    try {
        await apiFetch(`/participants/${user.id}/confirm-attendance`, 'POST', { isAttending });
        showAlert('attend-alert', isAttending ? '✓ Attendance confirmed!' : 'Declined successfully.', 'success');
        setTimeout(() => renderAttendance(), 1200);
    } catch (e) {
        showAlert('attend-alert', e.message);
        document.querySelectorAll('#attend-btns button').forEach(b => {
            b.disabled = false;
            b.style.opacity = '1';
        });
        if (clickedBtn) clickedBtn.textContent = isAttending ? "✓ Yes, I'm attending" : "✗ No, I can't attend";
    }
}
