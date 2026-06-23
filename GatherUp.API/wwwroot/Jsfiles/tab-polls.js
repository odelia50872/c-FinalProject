let questionCount = 1;

async function renderPolls() {
    const event = await apiFetch(`/events/${eventId}`);
    const pollResults = await Promise.all(
        (event.pollIds || []).map(id => apiFetch(`/polls/${id}/results`).then(r => ({ id, r })))
    );

    document.getElementById('tab-content').innerHTML = `
        <div id="polls-alert" class="alert"></div>
        ${isManager ? `
        <div class="card">
            <h2>Create New Poll</h2>
            <div class="form-row"><label>Poll Name</label><input type="text" id="poll-name"></div>
            <div id="questions-container">
                <div class="question-block" data-index="0">
                    <label>Question 1</label>
                    <input type="text" class="q-text" placeholder="Question text" style="margin-bottom:6px">
                    <div class="q-options-list" style="margin-bottom:4px">
                        <input type="text" class="q-option-item" placeholder="Option 1" style="margin-bottom:4px">
                        <input type="text" class="q-option-item" placeholder="Option 2" style="margin-bottom:4px">
                    </div>
                    <button class="btn-secondary btn-sm" style="margin-bottom:8px" onclick="addOption(this)">+ Add Option</button>
                </div>
            </div>
            <div class="form-actions">
                <button class="btn-secondary btn-sm" onclick="addQuestion()">+ Add Question</button>
                <button class="btn-primary" onclick="createPoll()">Create Poll</button>
            </div>
        </div>` : ''}
        ${pollResults.map(({ id, r }) => renderPollCard(id, r)).join('') || '<div class="card"><p style="color:#999;text-align:center">No polls yet.</p></div>'}
    `;
}

function renderPollCard(pollId, results) {
    return `
        <div class="card">
            <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:14px">
                <h2>Poll #${pollId}</h2>
                ${!isManager ? `<button class="btn-primary btn-sm" onclick="openVote(${pollId})">Vote</button>` : ''}
            </div>
            ${results.map(r => `
                <div style="margin-bottom:16px">
                    <strong style="font-size:14px">${r.questionText}</strong>
                    ${r.results.map(o => `
                        <div style="margin-top:6px">
                            <div style="display:flex;justify-content:space-between;font-size:13px">
                                <span>${o.option}</span><span>${o.votes} votes (${o.percentage}%)</span>
                            </div>
                            <div class="poll-bar"><div class="poll-bar-fill" style="width:${o.percentage}%"></div></div>
                        </div>
                    `).join('')}
                </div>
            `).join('')}
        </div>
    `;
}

function addQuestion() {
    const c = document.getElementById('questions-container');
    const div = document.createElement('div');
    div.className = 'question-block';
    div.innerHTML = `
        <label>Question ${++questionCount}</label>
        <input type="text" class="q-text" placeholder="Question text" style="margin-bottom:6px">
        <div class="q-options-list" style="margin-bottom:4px">
            <input type="text" class="q-option-item" placeholder="Option 1" style="margin-bottom:4px">
            <input type="text" class="q-option-item" placeholder="Option 2" style="margin-bottom:4px">
        </div>
        <button class="btn-secondary btn-sm" style="margin-bottom:8px" onclick="addOption(this)">+ Add Option</button>
    `;
    c.appendChild(div);
}

function addOption(btn) {
    const list = btn.previousElementSibling;
    const input = document.createElement('input');
    input.type = 'text';
    input.className = 'q-option-item';
    input.placeholder = `Option ${list.querySelectorAll('.q-option-item').length + 1}`;
    input.style.marginBottom = '4px';
    list.appendChild(input);
}

async function createPoll() {
    const name = document.getElementById('poll-name').value;
    const questions = Array.from(document.querySelectorAll('.question-block')).map(b => ({
        questionText: b.querySelector('.q-text').value,
        options: Array.from(b.querySelectorAll('.q-option-item')).map(i => i.value.trim()).filter(Boolean)
    }));
    try {
        await apiFetch(`/polls/events/${eventId}`, 'POST', { name, questions });
        showAlert('polls-alert', 'Poll created!', 'success');
        questionCount = 1;
        renderPolls();
    } catch (e) { showAlert('polls-alert', e.message); }
}

function openVote(pollId) {
    window.location.href = `/HtmlFiles/vote.html?pollId=${pollId}&eventId=${eventId}`;
}
