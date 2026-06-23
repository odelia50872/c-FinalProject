async function renderFinancial() {
    const [summary, receipts, participants] = await Promise.all([
        apiFetch(`/financial/events/${eventId}/summary`),
        apiFetch(`/financial/events/${eventId}/receipts`),
        apiFetch(`/events/${eventId}/participants`)
    ]);
    const balanceClass = summary.balance >= 0 ? 'positive' : 'negative';
    const receiptRows = !receipts || receipts.length === 0
        ? '<tr><td colspan="2" style="color:#999;text-align:center">No receipts yet. Add from Vendors tab.</td></tr>'
        : receipts.map(r => `<tr><td>${r.receiptNumber || '—'}</td><td>₪${r.amount ?? 0}</td></tr>`).join('');
    const unpaidOptions = participants
        .filter(p => !p.hasPaid)
        .map(p => `<option value="${p.id}">${p.name}</option>`).join('');

    document.getElementById('tab-content').innerHTML = `
        <div class="card">
            <h2>Financial Summary</h2>
            <div id="fin-alert" class="alert"></div>
            <div class="summary-box">
                <div class="summary-item"><div class="value positive">₪${summary.totalIncome}</div><div class="label">Total Income</div></div>
                <div class="summary-item"><div class="value" style="color:var(--primary)">₪${summary.totalOutgoing}</div><div class="label">Total Outgoing</div></div>
                <div class="summary-item"><div class="value ${balanceClass}">₪${summary.balance}</div><div class="label">Balance</div></div>
            </div>
            <div style="margin-bottom:16px">
                <button class="btn-secondary btn-sm" onclick="sendPaymentReminders()">Send Payment Reminders</button>
            </div>
            <h2>Register Payment</h2>
            <div style="display:flex;gap:8px;margin-bottom:20px">
                <select id="pay-pid" style="margin:0;flex:1">
                    <option value="">-- Select unpaid participant --</option>
                    ${unpaidOptions || '<option disabled>All participants have paid</option>'}
                </select>
                <input type="number" id="pay-amount" placeholder="Amount ₪" style="margin:0;width:120px">
                <button class="btn-success btn-sm" onclick="registerPayment()">Confirm</button>
            </div>
            <h2>Receipts <span style="font-size:12px;color:#888;font-weight:400">(add via Vendors tab → Add Receipt)</span></h2>
            <table><tr><th>Receipt Number</th><th>Amount</th></tr>${receiptRows}</table>
        </div>
    `;
}

async function registerPayment() {
    const pid = document.getElementById('pay-pid').value;
    const amount = parseFloat(document.getElementById('pay-amount').value);
    if (!pid) { showAlert('fin-alert', 'Please select a participant.'); return; }
    if (!amount || amount <= 0) { showAlert('fin-alert', 'Please enter a valid amount.'); return; }
    const btn = document.querySelector('#tab-content .btn-success.btn-sm');
    if (btn) { btn.disabled = true; btn.textContent = 'Saving...'; }
    try {
        await apiFetch(`/financial/participants/${pid}/payment`, 'POST', { amount });
        await renderFinancial();
        showAlert('fin-alert', `✓ Payment of ₪${amount} registered successfully!`, 'success');
    } catch (e) {
        showAlert('fin-alert', e.message);
        if (btn) { btn.disabled = false; btn.textContent = 'Confirm'; }
    }
}

async function sendPaymentReminders() {
    try {
        const res = await apiFetch(`/financial/events/${eventId}/send-payment-reminders`, 'POST');
        showAlert('fin-alert', res.message, 'success');
    } catch (e) { showAlert('fin-alert', e.message); }
}
