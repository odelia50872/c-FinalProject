async function renderSettings() {
    const me = await apiFetch(`/participants/${user.id}`);
    const pref = me.mailingPreferences ?? 7;
    document.getElementById('tab-content').innerHTML = `
        <div class="card">
            <h2>Notification Preferences</h2>
            <div id="settings-alert" class="alert"></div>
            <p style="color:#666;margin-bottom:16px">Choose which emails you want to receive (all enabled by default):</p>
            <div class="form-row" style="display:flex;align-items:center;gap:10px">
                <input type="checkbox" id="pref-attendance" style="width:auto;margin:0" ${pref & 1 ? 'checked' : ''} disabled>
                <label style="margin:0">Attendance confirmation updates</label>
            </div>
            <div class="form-row" style="display:flex;align-items:center;gap:10px">
                <input type="checkbox" id="pref-payment" style="width:auto;margin:0" ${pref & 2 ? 'checked' : ''} disabled>
                <label style="margin:0">Payment confirmation updates</label>
            </div>
            <div class="form-row" style="display:flex;align-items:center;gap:10px">
                <input type="checkbox" id="pref-polls" style="width:auto;margin:0" ${pref & 4 ? 'checked' : ''} disabled>
                <label style="margin:0">New poll notifications</label>
            </div>
            <div class="form-actions">
                <button class="btn-secondary" onclick="enableSettingsEdit()">✏️ Change Preferences</button>
            </div>
        </div>
    `;
}

function enableSettingsEdit() {
    ['pref-attendance', 'pref-payment', 'pref-polls'].forEach(id =>
        document.getElementById(id).disabled = false
    );
    const btn = document.querySelector('#tab-content .btn-secondary');
    btn.className = 'btn-primary';
    btn.textContent = 'Save Preferences';
    btn.setAttribute('onclick', 'saveSettings()');
}

async function saveSettings() {
    const attendance = document.getElementById('pref-attendance').checked ? 1 : 0;
    const payment = document.getElementById('pref-payment').checked ? 2 : 0;
    const polls = document.getElementById('pref-polls').checked ? 4 : 0;
    try {
        await apiFetch(`/participants/${user.id}/mailing-preference`, 'PUT', { preference: attendance | payment | polls });
        showAlert('settings-alert', 'Preferences saved!', 'success');
        setTimeout(() => renderSettings(), 1000);
    } catch (e) { showAlert('settings-alert', e.message); }
}
