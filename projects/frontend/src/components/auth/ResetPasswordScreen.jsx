import {useState} from 'react'
import {resetPassword} from '../../api/auth'
import FieldError from './FieldError'

function ResetPasswordScreen({onDone}) {
    const token = new URLSearchParams(window.location.search).get('token')

    const [password, setPassword] = useState('')
    const [passwordConfirmation, setPasswordConfirmation] = useState('')
    const [fieldErrors, setFieldErrors] = useState({})
    const [generalError, setGeneralError] = useState(null)
    const [tokenInvalid, setTokenInvalid] = useState(false)
    const [loading, setLoading] = useState(false)

    if (!token) {
        return (
            <div className="screen">
                <h1>Ссылка недействительна</h1>
                <p>Ссылка для восстановления пароля отсутствует или повреждена.</p>
                <a href="/">Вернуться ко входу</a>
            </div>
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
        <div className="screen">
            <h1>Новый пароль</h1>
            <form onSubmit={handleSubmit} noValidate>
                <label>
                    Новый пароль
                    <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} disabled={loading}/>
                    <FieldError message={fieldErrors.password}/>
                </label>
                <label>
                    Повтор нового пароля
                    <input
                        type="password"
                        value={passwordConfirmation}
                        onChange={(e) => setPasswordConfirmation(e.target.value)}
                        disabled={loading}
                    />
                    <FieldError message={fieldErrors.passwordConfirmation}/>
                </label>
                {generalError && <p className="error">{generalError}</p>}
                {tokenInvalid && <a href="/">Вернуться ко входу</a>}
                <button type="submit" disabled={loading}>
                    {loading ? 'Сохраняем…' : 'Сохранить пароль'}
                </button>
            </form>
        </div>
    )
}

export default ResetPasswordScreen