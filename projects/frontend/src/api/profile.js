async function handleResponse(response) {
    const data = await response.json().catch(() => null)

    if (!response.ok) {
        const message = data?.message ?? `Запрос не выполнен (код ${response.status})`
        const error = new Error(message)
        error.status = response.status
        throw error
    }

    return data
}

export function getProfile() {
    return fetch('/api/users/me/profile', {
        credentials: 'include',
    }).then(handleResponse)
}

export function getScenarios() {
    return fetch('/api/scenarios', {
        credentials: 'include',
    }).then(handleResponse)
}

