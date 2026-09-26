import { useState } from 'react'
import { startAttempt, chooseOption } from './api/attempts'
import StartScreen from './components/StartScreen'
import ScenarioScreen from './components/ScenarioScreen'
import './App.css'

function App() {
  const [screen, setScreen] = useState('start') // 'start' | 'scenario'
  const [attempt, setAttempt] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)

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

  if (screen === 'start') {
    return <StartScreen onStart={handleStart} loading={loading} error={error} />
  }

  if (attempt.finished) {
    return (
        <div className="screen">
          <h2>Попытка завершена</h2>
          <p>Результат: {attempt.resultStatus}</p>
          <p className="hint">Экран результата появится на следующем шаге.</p>
        </div>
    )
  }

  return (
      <ScenarioScreen
          attempt={attempt}
          onChoose={handleChoose}
          loading={loading}
          error={error}
      />
  )
}

export default App