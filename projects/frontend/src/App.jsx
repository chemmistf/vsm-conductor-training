import { useEffect, useState } from 'react'
import { startAttempt, chooseOption, sendTimeout, getResult } from './api/attempts'
import StartScreen from './components/StartScreen'
import ScenarioScreen from './components/ScenarioScreen'
import ResultScreen from './components/ResultScreen'
import './App.css'

function App() {
  const [screen, setScreen] = useState('start') // 'start' | 'scenario'
  const [attempt, setAttempt] = useState(null)
  const [result, setResult] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)

  useEffect(() => {
    if (!attempt?.finished) {
      return undefined
    }

    let cancelled = false
    setLoading(true)
    setError(null)

    getResult(attempt.attemptId)
        .then((data) => {
          if (!cancelled) setResult(data)
        })
        .catch((err) => {
          if (!cancelled) setError(err.message)
        })
        .finally(() => {
          if (!cancelled) setLoading(false)
        })

    return () => {
      cancelled = true
    }
  }, [attempt?.finished, attempt?.attemptId])

  async function handleStart() {
    setLoading(true)
    setError(null)
    try {
      const state = await startAttempt()
      setAttempt(state)
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
      setAttempt(state)
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
      setAttempt(state)
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

  if (screen === 'start') {
    return <StartScreen onStart={handleStart} loading={loading} error={error} />
  }

  if (attempt.finished) {
    if (result) {
      return <ResultScreen result={result} onRestart={handleRestart} />
    }

    return (
        <div className="screen">
          <h2>Попытка завершена</h2>
          <p>Загружаю результат…</p>
          {error && <p className="error">{error}</p>}
        </div>
    )
  }

  return (
      <ScenarioScreen
          attempt={attempt}
          onChoose={handleChoose}
          onTimeout={handleTimeout}
          loading={loading}
          error={error}
      />
  )
}

export default App