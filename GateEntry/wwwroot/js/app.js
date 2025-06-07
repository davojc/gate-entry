// wwwroot/js/app.js
document.addEventListener('DOMContentLoaded', () => {
    const pinScreen = document.getElementById('pin-screen');
    const appScreen = document.getElementById('app');
    const pinInput = document.getElementById('pin-input');
    const pinSubmitBtn = document.getElementById('pin-submit');
    const pinError = document.getElementById('pin-error');

    const addPlateForm = document.getElementById('add-plate-form');
    const plateNumberInput = document.getElementById('plate-number-input');
    const platesTableBody = document.querySelector('#plates-table tbody');
    const formError = document.getElementById('form-error');

    // Use sessionStorage to keep the token for the duration of the tab session.
    let authToken = sessionStorage.getItem('authToken');

    // --- Core App Logic ---
    const initializeApp = () => {
        if (authToken) {
            // If a token exists, try to fetch data immediately.
            fetchPlates();
        } else {
            // Otherwise, show the login screen.
            pinScreen.style.display = 'block';
            appScreen.style.display = 'none';
        }
    };

    const login = async (pin) => {
        hideErrors();
        try {
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ pin })
            });

            if (!response.ok) {
                showPinError('Invalid PIN provided.');
                return;
            }

            const data = await response.json();
            authToken = data.token;
            sessionStorage.setItem('authToken', authToken); // Store token

            // Successfully logged in, now fetch the data.
            await fetchPlates();

        } catch (error) {
            console.error('Login failed:', error);
            showPinError('An error occurred during login.');
        }
    };

    // --- PIN Handling ---
    pinSubmitBtn.addEventListener('click', () => {
        const pin = pinInput.value;
        if (pin) {
            login(pin);
        }
    });

    // --- API Calls ---
    const getApiHeaders = () => ({
        'Content-Type': 'application/json',
        // Send the JWT as a Bearer token.
        'Authorization': `Bearer ${authToken}`
    });

    const fetchPlates = async () => {
        try {
            const response = await fetch('/api/plates', {
                method: 'GET',
                headers: getApiHeaders()
            });

            if (response.status === 401) {
                // Token is invalid or expired. Clear it and force re-login.
                sessionStorage.removeItem('authToken');
                authToken = null;
                initializeApp(); // This will show the PIN screen
                return;
            }
            if (!response.ok) throw new Error('Failed to fetch plates.');

            const plates = await response.json();
            renderPlates(plates);

            // Show the main app screen
            pinScreen.style.display = 'none';
            appScreen.style.display = 'block';

        } catch (error) {
            console.error('Error fetching plates:', error);
        }
    };

    addPlateForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        hideErrors();
        const number = plateNumberInput.value.trim();
        if (!number) return;

        try {
            const response = await fetch('/api/plates', {
                method: 'POST',
                headers: getApiHeaders(),
                body: JSON.stringify({ number })
            });

            if (!response.ok) {
                const errorData = await response.json();
                showFormError(errorData.message || 'Failed to add plate.');
                return;
            }

            plateNumberInput.value = '';
            fetchPlates();
        } catch (error) {
            console.error('Error adding plate:', error);
            showFormError('An unexpected error occurred.');
        }
    });

    const updatePlateStatus = async (number, newStatus) => {
        await fetch(`/api/plates/${number}/status`, {
            method: 'PUT',
            headers: getApiHeaders(),
            body: JSON.stringify(newStatus)
        });
        fetchPlates();
    };

    const deletePlate = async (number) => {
        if (!confirm(`Are you sure you want to delete ${number}?`)) return;
        await fetch(`/api/plates/${number}`, {
            method: 'DELETE',
            headers: getApiHeaders()
        });
        fetchPlates();
    };

    // --- UI Rendering & Helpers (no changes needed from here down) ---
    const renderPlates = (plates) => {
        platesTableBody.innerHTML = '';
        if (plates.length === 0) {
            platesTableBody.innerHTML = '<tr><td colspan="4">No number plates found.</td></tr>';
            return;
        }

        plates.forEach(plate => {
            const row = document.createElement('tr');
            const statusText = plate.enabled ? 'Enabled' : 'Disabled';
            const toggleButtonText = plate.enabled ? 'Disable' : 'Enable';
            const toggleButtonClass = plate.enabled ? '' : 'disabled';

            row.innerHTML = `
                <td>${plate.number}</td>
                <td>${new Date(plate.added).toLocaleString()}</td>
                <td>${plate.seen ? new Date(plate.seen).toLocaleString() : 'Never'}</td>
                <td>${statusText}</td>
                <td class="actions">
                    <button class="toggle-status ${toggleButtonClass}">${toggleButtonText}</button>
                    <button class="delete">Delete</button>
                </td>
            `;
            row.querySelector('.toggle-status').addEventListener('click', () => updatePlateStatus(plate.number, !plate.enabled));
            row.querySelector('.delete').addEventListener('click', () => deletePlate(plate.number));
            platesTableBody.appendChild(row);
        });
    };

    const showPinError = (message) => {
        pinError.textContent = message;
        pinError.style.display = 'block';
    };
    const showFormError = (message) => {
        formError.textContent = message;
        formError.style.display = 'block';
    };
    const hideErrors = () => {
        pinError.style.display = 'none';
        formError.style.display = 'none';
    };

    // --- Start the app ---
    initializeApp();
});