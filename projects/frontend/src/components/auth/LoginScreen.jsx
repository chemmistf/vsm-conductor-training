import {useState} from 'react'
import {login} from '../../api/auth'
import {isValidEmail} from '../../validation/email'
import {AuthBackButton, AuthField, AuthPage} from './AuthLayout'
import checkIcon from '../../assets/auth/check.svg'
import './auth.css'

function LoginScreen({onSuccess, onForgotPassword, onGoToRegister, onBack, infoMessage}) {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [rememberMe, setRememberMe] = useState(true)
    const [fieldErrors, setFieldErrors] = useState({})
    const [generalError, setGeneralError] = useState(null)
    const [loading, setLoading] = useState(false)

    async function handleSubmit(e) {
        e.preventDefault()

        const trimmedEmail = email.trim().toLowerCase()
        const errors = {}
        if (trimmedEmail.length === 0) errors.email = 'Введите email.'
        else if (!isValidEmail(trimmedEmail)) errors.email = 'Введите корректный email.'
        if (password.length === 0) errors.password = 'Введите пароль.'

        setFieldErrors(errors)
        setGeneralError(null)
        if (Object.keys(errors).length > 0) return

        setLoading(true)
        try {
            const response = await login({email: trimmedEmail, password, rememberMe})
            onSuccess(response.user)
        } catch (err) {
            setGeneralError(err.code === 'invalid_credentials' ? 'Неверный email или пароль' : err.message)
        } finally {
            setLoading(false)
        }
    }

    return (
        <AuthPage className="auth-page--login">
            <form className="auth-content" onSubmit={handleSubmit} noValidate>
                <AuthBackButton onClick={onBack}/>
                <div className="auth-content__body">
                    <div className="auth-title">
                        <h1>Вход</h1>
                        <p>Войдите, чтобы продолжить с того места, где остановились.</p>
                    </div>
                    <div className="auth-form">
                        <div className="auth-fields">
                            <AuthField label="Ваша почта" type="email" value={email} onChange={(e) => setEmail(e.target.value)} disabled={loading} error={fieldErrors.email}/>
                            <AuthField label="Пароль" type="password" value={password} onChange={(e) => setPassword(e.target.value)} disabled={loading} error={fieldErrors.password}/>
                        </div>
                        <div className="auth-login-options">
                            <label className="auth-remember">
                                <span className={`auth-checkbox${rememberMe ? ' auth-checkbox--checked' : ''}`}>
                                    <input type="checkbox" checked={rememberMe} onChange={(e) => setRememberMe(e.target.checked)} disabled={loading}/>
                                    {rememberMe && <img src={checkIcon} width="14" height="14" alt="" aria-hidden="true"/>}
                                </span>
                                <span>Запомнить меня</span>
                            </label>
                            <button type="button" className="auth-link" onClick={onForgotPassword}>Забыли пароль ?</button>
                        </div>
                        {infoMessage && <p className="auth-info">{infoMessage}</p>}
                        {generalError && <p className="auth-general-error">{generalError}</p>}
                    </div>
                </div>
                <div className="auth-dock auth-dock--login">
                    <div className="auth-dock__primary">
                        <button type="submit" className="auth-primary" disabled={loading}>
                            {loading ? 'Входим…' : 'Войти'}
                        </button>
                    </div>
                    <div className="auth-dock__footer">
                        <span>Еще нет аккаунты?</span>
                        <button type="button" className="auth-link auth-link--strong" onClick={onGoToRegister}>Зарегистрироваться</button>
                    </div>
                </div>
            </form>
        </AuthPage>
    )
}

export default LoginScreen
