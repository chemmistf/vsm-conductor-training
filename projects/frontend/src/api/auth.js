const BASE_URL = '/api/auth'

async function handleResponse(response) {
    if (response.status === 204) {
        return null
    }

    const data = await response.json().catch(() => null)

    if (!response.ok) {
        const message = data?.message ?? `Запрос не выполнен (код ${response.status})`
        const error = new Error(message)
        error.code = data?.code ?? null
        error.status = response.status
        throw error
    }

    return data
}

export function getCurrentUser() {
    return fetch(`${BASE_URL}/me`, {
        method: 'GET',
        credentials: 'include',
    }).then(handleResponse)
}

export function logout() {
    return fetch(`${BASE_URL}/logout`, {
        method: 'POST',
        credentials: 'include',
    }).then(handleResponse)
}