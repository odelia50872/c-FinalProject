async function renderVendors() {
    const vendors = await apiFetch(`/vendors/events/${eventId}`);
    document.getElementById('tab-content').innerHTML = `
        <div class="card">
            <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:14px">
                <h2>Vendors</h2>
                <button class="btn-success btn-sm" onclick="toggleAddVendor()">+ Add Vendor</button>
            </div>
            <div id="vendor-alert" class="alert"></div>
            <div id="add-vendor-form" class="hidden" style="margin-bottom:16px">
                <div class="grid-2">
                    <div><label>Name</label><input type="text" id="v-name"></div>
                    <div><label>Amount Owed ₪</label><input type="number" id="v-amount"></div>
                </div>
                <button class="btn-primary btn-sm" onclick="addVendor()">Add</button>
            </div>
            <table>
                <tr><th>Name</th><th>Amount Owed</th><th>Has Receipt</th><th>Actions</th></tr>
                ${vendors.map(v => `
                    <tr>
                        <td>${v.name}</td>
                        <td>₪${v.amountOwed}</td>
                        <td>${v.hasReceipt ? '✓' : '—'}</td>
                        <td>
                            <button class="btn-secondary btn-sm" onclick="showSetAmount(${v.id})">Set Amount</button>
                            <button class="btn-primary btn-sm" style="margin-left:4px" onclick="showAddReceipt(${v.id})">Add Receipt</button>
                        </td>
                    </tr>
                    <tr id="amount-form-${v.id}" class="hidden">
                        <td colspan="4" style="background:#f8f9fa;padding:10px">
                            <input type="number" id="amt-${v.id}" placeholder="New amount" style="width:140px;display:inline-block;margin:0 8px 0 0">
                            <button class="btn-primary btn-sm" onclick="setAmount(${v.id})">Save</button>
                        </td>
                    </tr>
                    <tr id="receipt-form-${v.id}" class="hidden">
                        <td colspan="4" style="background:#f8f9fa;padding:10px">
                            <small style="color:#888;display:block;margin-bottom:6px">Receipt number will be generated automatically</small>
                            <input type="number" id="ramt-${v.id}" placeholder="Amount" style="width:100px;display:inline-block;margin:0 6px 0 0">
                            <input type="date" id="rdate-${v.id}" style="width:140px;display:inline-block;margin:0 6px 0 0">
                            <button class="btn-primary btn-sm" onclick="addReceipt(${v.id})">Add</button>
                        </td>
                    </tr>
                `).join('')}
            </table>
        </div>
    `;
}

function toggleAddVendor() {
    document.getElementById('add-vendor-form').classList.toggle('hidden');
}

async function addVendor() {
    try {
        await apiFetch(`/vendors/events/${eventId}`, 'POST', {
            name: document.getElementById('v-name').value,
            amountOwed: parseFloat(document.getElementById('v-amount').value)
        });
        showAlert('vendor-alert', 'Vendor added!', 'success');
        renderVendors();
    } catch (e) { showAlert('vendor-alert', e.message); }
}

function showSetAmount(vid) {
    document.getElementById(`amount-form-${vid}`).classList.toggle('hidden');
}

async function setAmount(vid) {
    try {
        await apiFetch(`/financial/events/${eventId}/vendors/${vid}/amount`, 'PUT', {
            amount: parseFloat(document.getElementById(`amt-${vid}`).value)
        });
        renderVendors();
    } catch (e) { showAlert('vendor-alert', e.message); }
}

function showAddReceipt(vid) {
    document.getElementById(`receipt-form-${vid}`).classList.toggle('hidden');
}

async function addReceipt(vid) {
    const amount = parseFloat(document.getElementById(`ramt-${vid}`).value);
    const date = document.getElementById(`rdate-${vid}`).value;
    if (!amount || amount <= 0) { showAlert('vendor-alert', 'Please enter a valid amount.'); return; }
    if (!date) { showAlert('vendor-alert', 'Please select a date.'); return; }
    try {
        await apiFetch(`/vendors/events/${eventId}/${vid}/receipts`, 'POST', { amount, date });
        showAlert('vendor-alert', 'Receipt added!', 'success');
        renderVendors();
    } catch (e) { showAlert('vendor-alert', e.message); }
}
