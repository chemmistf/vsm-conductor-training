const BASE_URL = '/api/attempts'

async function handleResponse(response) {
    const data = await response.json().catch(() => null)

    if (!response.ok) {
        const message = data?.message ?? `Запрос не выполнен (код ${response.status})`
        throw new Error(message)
    }

    return data
}

export function startAttempt(scenarioId = null) {
    return fetch(BASE_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ scenarioId }),
    }).then(handleResponse)
}

export function chooseOption(attemptId, choiceId) {
    return fetch(`${BASE_URL}/${attemptId}/choice`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ choiceId }),
    }).then(handleResponse)
}

export function sendTimeout(attemptId) {
    return fetch(`${BASE_URL}/${attemptId}/timeout`, {
        method: 'POST',
    }).then(handleResponse)
}

export function getResult(attemptId) {
    return fetch(`${BASE_URL}/${attemptId}/result`).then(handleResponse)
}