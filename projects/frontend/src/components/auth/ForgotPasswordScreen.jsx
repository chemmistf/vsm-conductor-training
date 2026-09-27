import {useState} from 'react'
import {requestPasswordReset} from '../../api/auth'
import {isValidEmail} from '../../validation/email'
import {AuthBackButton, AuthField, AuthPage} from './AuthLayout'
import './auth.css'

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
        if (!isValidEmail(trimmedEmail)) {
            setFieldError('Введите корректный email.')
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
            <AuthPage className="auth-page--password-reset">
                <div className="auth-content">
                    <AuthBackButton onClick={onBackToLogin}/>
                    <div className="auth-content__body">
                        <div className="auth-title">
                            <h1>Проверьте почту</h1>
                            <p>{result.message}</p>
                        </div>
                        {result.debugResetUrl && (
                            <p className="auth-info">
                                Только для разработки: <a href={result.debugResetUrl}>{result.debugResetUrl}</a>
                            </p>
                        )}
                    </div>
                </div>
                <div className="auth-dock">
                    <div className="auth-dock__primary">
                        <button type="button" className="auth-primary" onClick={onBackToLogin}>Вернуться ко Входу</button>
                    </div>
                </div>
            </AuthPage>
        )
    }

    return (
        <AuthPage className="auth-page--password-reset">
            <form className="auth-content" onSubmit={handleSubmit} noValidate>
                <AuthBackButton onClick={onBackToLogin}/>
                <div className="auth-content__body">
                    <div className="auth-title">
                        <h1>Забыли пароль?</h1>
                        <p>Введите email, который вы использовали при регистрации. Мы отправим ссылку для создания нового пароля.</p>
                    </div>
                    <AuthField label="Ваша почта" type="email" value={email} onChange={(e) => setEmail(e.target.value)} disabled={loading} error={fieldError}/>
                    {generalError && <p className="auth-general-error">{generalError}</p>}
                </div>
                <div className="auth-dock">
                    <div className="auth-dock__primary">
                        <button type="submit" className="auth-primary" disabled={loading}>
                            {loading ? 'Отправляем…' : 'Отправить код'}
                        </button>
                    </div>
                    <div className="auth-dock__footer">
                        <span>Вернуться ко </span>
                        <button type="button" className="auth-link auth-link--strong" onClick={onBackToLogin}>Входу</button>
                    </div>
                </div>
            </form>
        </AuthPage>
    )
}

export default ForgotPasswordScreen
