// app.js
    const PORT = 5063;
    const API = `http://localhost:${PORT}/api`;
    let isRegistering = false;
    let allTodos = [];

    // Theme
    function toggleTheme() {
        document.body.classList.toggle('dark-mode');
        const isDark = document.body.classList.contains('dark-mode');
        document.getElementById('theme-btn').innerText = isDark ? '☀️' : '🌙';
        localStorage.setItem('theme', isDark ? 'dark' : 'light');
    }

    if (localStorage.getItem('theme') === 'dark') {
        document.body.classList.add('dark-mode');
        document.getElementById('theme-btn').innerText = '☀️';
    }

    // Security
    function escapeHtml(text) {
        if (!text) return text;
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Auth
    window.onload = () => {
        if (localStorage.getItem("token")) showApp();
    };

    function toggleAuthMode() {
        isRegistering = !isRegistering;
        document.getElementById("auth-title").innerText = isRegistering ? "Create Account" : "Welcome Back";
        document.querySelector(".subtitle").innerText = isRegistering ? "Join us to get started" : "Sign in to manage your tasks";
        document.getElementById("auth-btn").innerText = isRegistering ? "Register" : "Login";
        document.getElementById("auth-toggle").innerText = isRegistering ? "Have an account? Login" : "Need an account? Register";
        document.getElementById("auth-error").style.display = "none";
    }

    async function handleAuth() {
        const u = document.getElementById("username").value;
        const p = document.getElementById("password").value;
        const endpoint = isRegistering ? "register" : "login";
        
        try {
            const res = await fetch(`${API}/Auth/${endpoint}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username: u, password: p })
            });
            
            const data = await res.json();
            if (!res.ok) throw new Error(data.message || "Request failed");

            if (isRegistering) {
                alert("Registration successful! Please login.");
                toggleAuthMode();
            } else {
                localStorage.setItem("token", data.token);
                showApp();
            }
        } catch (err) {
            const errDiv = document.getElementById("auth-error");
            errDiv.innerText = err.message;
            errDiv.style.display = "block";
        }
    }

    function logout() {
        localStorage.removeItem("token");
        location.reload();
    }

    function showApp() {
        document.getElementById("auth-section").style.display = "none";
        document.getElementById("app-section").style.display = "block";
        loadTodos();
    }

    // Todos
    async function loadTodos() {
        const res = await fetch(`${API}/Todos`, {
            headers: { 'Authorization': `Bearer ${localStorage.getItem("token")}` }
        });
        if (res.status === 401) logout();
        allTodos = await res.json();
        renderTodos(allTodos);
    }

    function renderTodos(todos) {
        const list = document.getElementById("todo-list");
        if (todos.length === 0) {
            list.innerHTML = `
                <div class="empty-state">
                    <div class="empty-state-icon">✨</div>
                    <div class="empty-state-text">No tasks yet</div>
                    <div class="empty-state-subtext">Add your first task to get started</div>
                </div>
            `;
            return;
        }
        
        list.innerHTML = todos.map(t => `
            <div class="todo-item">
                <div class="todo-left">
                    <input type="checkbox" ${t.isComplete ? 'checked' : ''} onchange="toggleTask(${t.id})">
                    <span class="${t.isComplete ? 'completed' : ''}">${escapeHtml(t.name)}</span>
                </div>
                <button class="btn-danger" onclick="deleteTask(${t.id})">🗑️</button>
            </div>
        `).join('');
    }

    async function addTask() {
        const input = document.getElementById("new-task");
        if (!input.value.trim()) return;

        // UPDATED: Now sending JSON body
        await fetch(`${API}/Todos`, {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem("token")}` 
            },
            body: JSON.stringify({ name: input.value.trim() })
        });
        input.value = "";
        loadTodos();
    }

    async function toggleTask(id) {
        await fetch(`${API}/Todos/${id}/toggle`, {
            method: 'PUT',
            headers: { 'Authorization': `Bearer ${localStorage.getItem("token")}` }
        });
        loadTodos();
    }

    async function deleteTask(id) {
        if(!confirm("Are you sure you want to delete this task?")) return;
        await fetch(`${API}/Todos/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${localStorage.getItem("token")}` }
        });
        loadTodos();
    }

    function filterTasks() {
        const query = document.getElementById("search").value.toLowerCase();
        const filtered = allTodos.filter(t => t.name.toLowerCase().includes(query));
        renderTodos(filtered);
    }

    // Enter key support
    document.addEventListener('DOMContentLoaded', () => {
        const newTaskInput = document.getElementById('new-task');
        if (newTaskInput) {
            newTaskInput.addEventListener('keypress', (e) => {
                if (e.key === 'Enter') addTask();
            });
        }
    });
