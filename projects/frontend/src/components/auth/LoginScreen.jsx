import {useState} from 'react'
import {login} from '../../api/auth'
import FieldError from './FieldError'
import {isValidEmail} from '../../validation/email'

function LoginScreen({onSuccess, onForgotPassword, onGoToRegister, infoMessage}) {
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
        <div className="screen">
            <h1>Вход</h1>
            {infoMessage && <p className="info">{infoMessage}</p>}
            <form onSubmit={handleSubmit} noValidate>
                <label>
                    Email
                    <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} disabled={loading}/>
                    <FieldError message={fieldErrors.email}/>
                </label>
                <label>
                    Пароль
                    <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} disabled={loading}/>
                    <FieldError message={fieldErrors.password}/>
                </label>
                <label className="checkbox-field">
                    <input
                        type="checkbox"
                        checked={rememberMe}
                        onChange={(e) => setRememberMe(e.target.checked)}
                        disabled={loading}
                    />
                    Запомнить меня
                </label>
                {generalError && <p className="error">{generalError}</p>}
                <button type="submit" disabled={loading}>
                    {loading ? 'Входим…' : 'Войти'}
                </button>
            </form>
            <p>
                <button type="button" className="link-button" onClick={onForgotPassword}>
                    Забыли пароль?
                </button>
            </p>
            <p>
                Нет аккаунта?{' '}
                <button type="button" className="link-button" onClick={onGoToRegister}>
                    Зарегистрироваться
                </button>
            </p>
        </div>
    )
}

export default LoginScreen
