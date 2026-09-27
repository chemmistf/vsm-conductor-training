import {useState} from 'react'
import {requestPasswordReset} from '../../api/auth'
import FieldError from './FieldError'

function ForgotPasswordScreen({onBackToLogin}) {
    const [email, setEmail] = useState('')
    const [fieldError, setFieldError] = useState(null)
    const [generalError, setGeneralError] = useState(null)
    const [loading, setLoading] = useState(false)
    const [result, setResult] = useState(null)

    async function handleSubmit(e) {
        e.preventDefault()

        const trimmedEmail = email.trim().toLowerCase()
        if (trimmedEmail.length === 0) {
            setFieldError('Введите email.')
            return
        }

        setFieldError(null)
        setGeneralError(null)
        setLoading(true)
        try {
            const response = await requestPasswordReset(trimmedEmail)
            setResult(response)
        } catch (err) {
            setGeneralError(err.message)
        } finally {
            setLoading(false)
        }
    }

    if (result) {
        return (
            <div className="screen">
                <h1>Восстановление пароля</h1>
                <p>{result.message}</p>
                {result.debugResetUrl && (
                    <p className="hint">
                        Только для разработки: <a href={result.debugResetUrl}>{result.debugResetUrl}</a>
                    </p>
                )}
                <button type="button" className="link-button" onClick={onBackToLogin}>Назад ко входу</button>
            </div>
        )
    }

    return (
        <div className="screen">
            <h1>Забыли пароль?</h1>
            <form onSubmit={handleSubmit} noValidate>
                <label>
                    Email
                    <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} disabled={loading}/>
                    <FieldError message={fieldError}/>
                </label>
                {generalError && <p className="error">{generalError}</p>}
                <button type="submit" disabled={loading}>
                    {loading ? 'Отправляем…' : 'Отправить код'}
                </button>
            </form>
            <button type="button" className="link-button" onClick={onBackToLogin}>Назад ко входу</button>
        </div>
    )
}

export default ForgotPasswordScreen