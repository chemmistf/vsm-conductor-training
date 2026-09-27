import {useState} from 'react'
import {resetPassword} from '../../api/auth'
import {AuthBackButton, AuthField, AuthPage} from './AuthLayout'
import './auth.css'

function ResetPasswordScreen({onDone, onBack}) {
    const token = new URLSearchParams(window.location.search).get('token')

    const [password, setPassword] = useState('')
    const [passwordConfirmation, setPasswordConfirmation] = useState('')
    const [fieldErrors, setFieldErrors] = useState({})
    const [generalError, setGeneralError] = useState(null)
    const [tokenInvalid, setTokenInvalid] = useState(false)
    const [loading, setLoading] = useState(false)

    if (!token) {
        return (
            <AuthPage className="auth-page--new-password">
                <div className="auth-content auth-content--wide">
                    <AuthBackButton onClick={onBack}/>
                    <div className="auth-content__body">
                        <div className="auth-title">
                            <h1>Ссылка недействительна</h1>
                            <p>Ссылка для восстановления пароля отсутствует или повреждена.</p>
                        </div>
                    </div>
                </div>
            </AuthPage>
        )
    }

    async function handleSubmit(e) {
        e.preventDefault()

        const errors = {}
        if (password.length < 8) errors.password = 'Пароль должен содержать минимум 8 символов.'
        if (passwordConfirmation !== password) errors.passwordConfirmation = 'Пароли не совпадают.'

        setFieldErrors(errors)
        setGeneralError(null)
        if (Object.keys(errors).length > 0) return

        setLoading(true)
        try {
            await resetPassword({token, password, passwordConfirmation})
            onDone()
        } catch (err) {
            if (err.code === 'invalid_or_expired_reset_token') {
                setGeneralError('Ссылка для восстановления недействительна или уже использована.')
                setTokenInvalid(true)
            } else {
                setGeneralError(err.message)
            }
        } finally {
            setLoading(false)
        }
    }

    return (
        <AuthPage className="auth-page--new-password">
            <form className="auth-content auth-content--wide" onSubmit={handleSubmit} noValidate>
                <AuthBackButton onClick={onBack}/>
                <div className="auth-content__body">
                    <div className="auth-title">
                        <h1>Новый пароль</h1>
                        <p>Придумайте новый пароль.</p>
                    </div>
                    <div className="auth-fields">
                        <AuthField label="Новый пароль" type="password" value={password} onChange={(e) => setPassword(e.target.value)} disabled={loading} error={fieldErrors.password}/>
                        <AuthField label="Повторите пароль" type="password" value={passwordConfirmation} onChange={(e) => setPasswordConfirmation(e.target.value)} disabled={loading} error={fieldErrors.passwordConfirmation}/>
                    </div>
                    {generalError && <p className="auth-general-error">{generalError}</p>}
                    {tokenInvalid && <button type="button" className="auth-link" onClick={onBack}>Вернуться ко входу</button>}
                </div>
                <div className="auth-dock auth-dock--new-password">
                    <div className="auth-dock__primary">
                        <button type="submit" className="auth-primary" disabled={loading}>
                            {loading ? 'Сохраняем…' : 'Сохранить пароль'}
                        </button>
                    </div>
                </div>
            </form>
        </AuthPage>
    )
}

export default ResetPasswordScreen
