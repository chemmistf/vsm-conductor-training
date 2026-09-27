import {useEffect, useState} from 'react'
import {getCurrentUser, logout} from './api/auth'
import {startAttempt, chooseOption, getResult, sendTimeout} from './api/attempts'
import {getProfile, getScenarios} from './api/profile'
import AuthArea from './components/auth/AuthArea'
import ResetPasswordScreen from './components/auth/ResetPasswordScreen'
import LegalScreen from './components/LegalScreen'
import ResultScreen from './components/ResultScreen'
import FinishScreenDetail from './components/FinishScreenDetail'
import MainScreen from './components/MainScreen'
import StartGameScreen from './components/StartGameScreen'
import ProfileScreen from './components/ProfileScreen'
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

    const [screen, setScreen] = useState('main')
    const [attempt, setAttempt] = useState(null)
    const [result, setResult] = useState(null)
    const [selectedChoiceId, setSelectedChoiceId] = useState(null)
    const [profile, setProfile] = useState(null)
    const [scenarios, setScenarios] = useState([])
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

    useEffect(() => {
        if (authStatus !== 'authenticated') return

        let cancelled = false
        Promise.all([getProfile(), getScenarios()])
            .then(([nextProfile, nextScenarios]) => {
                if (cancelled) return
                setProfile(nextProfile)
                setScenarios(nextScenarios)
            })
            .catch((err) => {
                if (!cancelled) setError(err.message)
            })

        return () => {
            cancelled = true
        }
    }, [authStatus])

    async function applyAttemptState(nextState) {
        setAttempt(nextState)
        if (nextState.finished) {
            const data = await getResult(nextState.attemptId)
            setResult(data)
            getProfile().then(setProfile).catch(() => null)
        }
    }

    async function handleStart(scenarioId = null) {
        setScreen('loading')
        setLoading(true)
        setError(null)
        try {
            const state = await startAttempt(scenarioId)
            await applyAttemptState(state)
            setSelectedChoiceId(null)
            setScreen('incident')
        } catch (err) {
            setError(err.message)
            setScreen('startGame')
        } finally {
            setLoading(false)
        }
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
            setScreen(state.finished ? 'scenario' : 'question')
        } catch (err) {
            setError(err.message)
        } finally {
            setLoading(false)
        }
    }

    async function handleTimeout() {
        if (!attempt) return

        setLoading(true)
        setError(null)
        try {
            const state = await sendTimeout(attempt.attemptId)
            await applyAttemptState(state)
            setSelectedChoiceId(null)
            setScreen(state.finished ? 'scenario' : 'question')
        } catch (err) {
            if (err.message !== 'Timer has not expired yet.') {
                setError(err.message)
            }
        } finally {
            setLoading(false)
        }
    }

    function handleRestart() {
        setScreen('main')
        setAttempt(null)
        setResult(null)
        setSelectedChoiceId(null)
        setError(null)
    }

    function handleAuthenticated(authenticatedUser) {
        setUser(authenticatedUser)
        setAuthStatus('authenticated')
        setScreen('main')
    }

    async function handleLogout() {
        try {
            await logout()
        } catch {
            //
        } finally {
            setUser(null)
            setAuthStatus('unauthenticated')
            setProfile(null)
            setScenarios([])
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

    if (screen === 'main') {
        content = (
            <MainScreen
                profile={profile}
                onPlay={() => setScreen('startGame')}
                onProfile={() => setScreen('profile')}
            />
        )
    } else if (screen === 'startGame') {
        content = (
            <StartGameScreen
                scenarios={scenarios}
                onQuickStart={() => handleStart()}
                onSelectScenario={(scenarioId) => handleStart(scenarioId)}
                onHome={() => setScreen('main')}
                onProfile={() => setScreen('profile')}
                onBack={() => setScreen('main')}
            />
        )
    } else if (screen === 'profile') {
        content = (
            <ProfileScreen
                profile={profile}
                onHome={() => setScreen('main')}
                onStartGame={() => setScreen('startGame')}
                onLogout={handleLogout}
            />
        )
    } else if (screen === 'loading') {
        content = <LoadingScreen onBack={handleRestart}/>
    } else if (screen === 'incident') {
        content = <IncidentScreen onStart={handleIncidentStart} onClose={handleRestart}/>
    } else if (screen === 'question') {
        content = (
            <GameFlowScreen
                phase="question"
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
                title={nodeCopy.title}
                description={nodeCopy.description}
                choices={attempt?.node?.choices}
                selectedChoiceId={selectedChoiceId}
                safety={attempt?.safety}
                loyalty={attempt?.loyalty}
                deadlineAt={attempt?.node?.deadlineAt}
                timerSeconds={attempt?.node?.timerSeconds}
                onTimeout={handleTimeout}
                onSelect={setSelectedChoiceId}
                onContinue={handleVariantsContinue}
                onClose={handleRestart}
            />
        )
    } else if (screen === 'answer') {
        content = (
            <GameFlowScreen
                phase="answer"
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
    } else if (screen === 'resultDetail') {
        content = result
            ? <FinishScreenDetail result={result} onBack={() => setScreen('scenario')} onFinish={handleRestart}/>
            : <div className="screen"><p>Детали результата недоступны.</p></div>
    } else if (attempt.finished) {
        content = result
            ? <ResultScreen
                result={result}
                onRestart={handleRestart}
                onDetails={() => setScreen('resultDetail')}
            />
            : (
                <div className="screen">
                    <h2>Попытка завершена</h2>
                    <p>Загружаю результат…</p>
                    {error && <p className="error">{error}</p>}
                </div>
            )
    } else {
        content = <div className="screen"><p>Экран загружается…</p></div>
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
