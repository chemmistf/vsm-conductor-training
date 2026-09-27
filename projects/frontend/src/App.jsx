import {useEffect, useState} from 'react'
import {getCurrentUser, logout} from './api/auth'
import {startAttempt, chooseOption, sendTimeout, getResult} from './api/attempts'
import AuthArea from './components/auth/AuthArea'
import ResetPasswordScreen from './components/auth/ResetPasswordScreen'
import LegalScreen from './components/LegalScreen'
import StartScreen from './components/StartScreen'
import ScenarioScreen from './components/ScenarioScreen'
import ResultScreen from './components/ResultScreen'
import LoadingScreen from './components/game/LoadingScreen'
import IncidentScreen from './components/game/IncidentScreen'
import GameFlowScreen from './components/game/GameFlowScreen'
import {getNodeCopy} from './components/game/nodeCopy'
import './App.css'

function App() {
    const [route] = useState(() => window.location.pathname)
    const isResetPasswordRoute = route === '/reset-password'
    const legalPage = route === '/terms' ? 'terms' : route === '/privacy' ? 'privacy' : null

    const [authStatus, setAuthStatus] = useState('checkingSession') // 'checkingSession' | 'unauthenticated' | 'authenticated'
    const [user, setUser] = useState(null)
    const [sessionError, setSessionError] = useState(null)

    const [screen, setScreen] = useState('start')
    const [attempt, setAttempt] = useState(null)
    const [result, setResult] = useState(null)
    const [selectedChoiceId, setSelectedChoiceId] = useState(null)
    const [step, setStep] = useState(1)
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
        setScreen('loading')
        setLoading(true)
        setError(null)
        try {
            const state = await startAttempt()
            await applyAttemptState(state)
            setSelectedChoiceId(null)
            setStep(1)
            setScreen('incident')
        } catch (err) {
            setError(err.message)
            setScreen('start')
        } finally {
            setLoading(false)
        }
    }

    function handleChoose(choiceId) {
        setSelectedChoiceId(choiceId)
    }

    function handleIncidentStart() {
        setScreen('question')
    }

    function handleQuestionContinue() {
        setScreen('variants')
    }

    function handleVariantsContinue() {
        if (selectedChoiceId) handleAnswerContinue()
    }

    async function handleAnswerContinue() {
        if (!attempt || !selectedChoiceId) return

        setLoading(true)
        setError(null)
        try {
            const state = await chooseOption(attempt.attemptId, selectedChoiceId)
            await applyAttemptState(state)
            setSelectedChoiceId(null)
            if (!state.finished) setStep((currentStep) => currentStep + 1)
            setScreen(state.finished ? 'scenario' : 'question')
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
        setSelectedChoiceId(null)
        setStep(1)
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
            <ResetPasswordScreen onBack={() => {
                window.location.href = '/'
            }} onDone={() => {
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
    const nodeCopy = getNodeCopy(attempt?.node)

    if (screen === 'start') {
        content = <StartScreen onStart={handleStart} loading={loading} error={error}/>
    } else if (screen === 'loading') {
        content = <LoadingScreen onBack={handleRestart}/>
    } else if (screen === 'incident') {
        content = <IncidentScreen onStart={handleIncidentStart} onClose={handleRestart}/>
    } else if (screen === 'question') {
        content = (
            <GameFlowScreen
                phase="question"
                step={step}
                title={nodeCopy.title}
                description={nodeCopy.description}
                safety={attempt?.safety}
                loyalty={attempt?.loyalty}
                onContinue={handleQuestionContinue}
                onClose={handleRestart}
            />
        )
    } else if (screen === 'variants') {
        content = (
            <GameFlowScreen
                phase="variants"
                step={step}
                title={nodeCopy.title}
                description={nodeCopy.description}
                choices={attempt?.node?.choices}
                selectedChoiceId={selectedChoiceId}
                safety={attempt?.safety}
                loyalty={attempt?.loyalty}
                onSelect={setSelectedChoiceId}
                onContinue={handleVariantsContinue}
                onClose={handleRestart}
            />
        )
    } else if (screen === 'answer') {
        content = (
            <GameFlowScreen
                phase="answer"
                step={step}
                title={nodeCopy.title}
                description={nodeCopy.description}
                choices={attempt?.node?.choices}
                selectedChoiceId={selectedChoiceId}
                safety={attempt?.safety}
                loyalty={attempt?.loyalty}
                onContinue={handleAnswerContinue}
                onClose={handleRestart}
                loading={loading}
                error={error}
            />
        )
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
            {screen === 'start' && (
                <header className="app-header">
                    <span className="app-header__user">{user?.name}</span>
                    <button type="button" className="app-header__logout" onClick={handleLogout}>
                        Выйти
                    </button>
                </header>
            )}
            {content}
        </>
    )
}

export default App
