import {useEffect, useState} from 'react'
import {getCurrentUser, logout} from './api/auth'
import {startAttempt, chooseOption, sendTimeout, getResult} from './api/attempts'
import StartScreen from './components/StartScreen'
import ScenarioScreen from './components/ScenarioScreen'
import ResultScreen from './components/ResultScreen'
import './App.css'

function App() {
    const [authStatus, setAuthStatus] = useState('checkingSession') // 'checkingSession' | 'unauthenticated' | 'authenticated'
    const [user, setUser] = useState(null)
    const [sessionError, setSessionError] = useState(null)

    const [screen, setScreen] = useState('start') // 'start' | 'scenario'
    const [attempt, setAttempt] = useState(null)
    const [result, setResult] = useState(null)
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState(null)

    useEffect(() => {
        let cancelled = false

        getCurrentUser()
            .then((currentUser) => {
                if (cancelled) return
                setUser(currentUser)
                setAuthStatus('authenticated')
            })
            .catch((err) => {
                if (cancelled) return
                if (err.status !== 401) {
                    setSessionError(err.message)
                }
                setAuthStatus('unauthenticated')
            })

        return () => {
            cancelled = true
        }
    }, [])

    async function applyAttemptState(nextState) {
        setAttempt(nextState)
        if (nextState.finished) {
            const data = await getResult(nextState.attemptId)
            setResult(data)
        }
    }

    async function handleStart() {
        setLoading(true)
        setError(null)
        try {
            const state = await startAttempt()
            await applyAttemptState(state)
            setScreen('scenario')
        } catch (err) {
            setError(err.message)
        } finally {
            setLoading(false)
        }
    }

    async function handleChoose(choiceId) {
        setLoading(true)
        setError(null)
        try {
            const state = await chooseOption(attempt.attemptId, choiceId)
            await applyAttemptState(state)
        } catch (err) {
            setError(err.message)
        } finally {
            setLoading(false)
        }
    }

    async function handleTimeout() {
        if (!attempt || attempt.finished) return
        setLoading(true)
        setError(null)
        try {
            const state = await sendTimeout(attempt.attemptId)
            await applyAttemptState(state)
        } catch (err) {
            setError(err.message)
        } finally {
            setLoading(false)
        }
    }

    function handleRestart() {
        setScreen('start')
        setAttempt(null)
        setResult(null)
        setError(null)
    }

    async function handleLogout() {
        try {
            await logout()
        } catch {
            // Кука чистится на сервере в любом случае — локально сбрасываем состояние независимо от результата запроса.
        } finally {
            setUser(null)
            setAuthStatus('unauthenticated')
            handleRestart()
        }
    }

    if (authStatus === 'checkingSession') {
        return (
            <div className="screen">
                <p>Загрузка…</p>
            </div>
        )
    }

    if (authStatus === 'unauthenticated') {
        return (
            <div className="screen">
                <h1>VSM Training</h1>
                <p>Экраны входа и регистрации появятся на следующем этапе.</p>
                {sessionError && <p className="error">{sessionError}</p>}
            </div>
        )
    }

    let content

    if (screen === 'start') {
        content = <StartScreen onStart={handleStart} loading={loading} error={error}/>
    } else if (attempt.finished) {
        content = result
            ? <ResultScreen result={result} onRestart={handleRestart}/>
            : (
                <div className="screen">
                    <h2>Попытка завершена</h2>
                    <p>Загружаю результат…</p>
                    {error && <p className="error">{error}</p>}
                </div>
            )
    } else {
        content = (
            <ScenarioScreen
                attempt={attempt}
                onChoose={handleChoose}
                onTimeout={handleTimeout}
                loading={loading}
                error={error}
            />
        )
    }

    return (
        <>
            <header className="app-header">
                <span className="app-header__user">{user?.name}</span>
                <button type="button" className="app-header__logout" onClick={handleLogout}>
                    Выйти
                </button>
            </header>
            {content}
        </>
    )
}

export default App