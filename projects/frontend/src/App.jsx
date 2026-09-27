import {useEffect, useState} from 'react'
import {getCurrentUser, logout} from './api/auth'
import {startAttempt, chooseOption, sendTimeout, getResult} from './api/attempts'
import AuthArea from './components/auth/AuthArea'
import ResetPasswordScreen from './components/auth/ResetPasswordScreen'
import LegalScreen from './components/LegalScreen'
import StartScreen from './components/StartScreen'
import ScenarioScreen from './components/ScenarioScreen'
import ResultScreen from './components/ResultScreen'
import './App.css'

function App() {
    const [route] = useState(() => window.location.pathname)
    const isResetPasswordRoute = route === '/reset-password'
    const legalPage = route === '/terms' ? 'terms' : route === '/privacy' ? 'privacy' : null

    const [authStatus, setAuthStatus] = useState('checkingSession') // 'checkingSession' | 'unauthenticated' | 'authenticated'
    const [user, setUser] = useState(null)
    const [sessionError, setSessionError] = useState(null)

    const [screen, setScreen] = useState('start') // 'start' | 'scenario'
    const [attempt, setAttempt] = useState(null)
    const [result, setResult] = useState(null)
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState(null)

    useEffect(() => {
        if (isResetPasswordRoute || legalPage) return

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
    }, [isResetPasswordRoute, legalPage])

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

    function handleAuthenticated(authenticatedUser) {
        setUser(authenticatedUser)
        setAuthStatus('authenticated')
    }

    async function handleLogout() {
        try {
            await logout()
        } catch {
            //
        } finally {
            setUser(null)
            setAuthStatus('unauthenticated')
            handleRestart()
        }
    }

    if (isResetPasswordRoute) {
        return (
            <ResetPasswordScreen onDone={() => {
                window.location.href = '/?passwordReset=success'
            }}/>
        )
    }

    if (legalPage) {
        return <LegalScreen page={legalPage}/>
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
            <>
                {sessionError && (
                    <div className="screen">
                        <p className="error">{sessionError}</p>
                    </div>
                )}
                <AuthArea onAuthenticated={handleAuthenticated}/>
            </>
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
