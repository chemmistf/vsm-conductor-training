import {useState} from 'react'
import {register} from '../../api/auth'
import FieldError from './FieldError'

const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

function RegisterScreen({onSuccess, onBack, onGoToLogin}) {
    const [name, setName] = useState('')
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [passwordConfirmation, setPasswordConfirmation] = useState('')
    const [fieldErrors, setFieldErrors] = useState({})
    const [generalError, setGeneralError] = useState(null)
    const [loading, setLoading] = useState(false)

    function validate() {
        const trimmedName = name.trim()
        const trimmedEmail = email.trim().toLowerCase()
        const errors = {}

        if (trimmedName.length < 2 || trimmedName.length > 100) {
            errors.name = 'Имя должно содержать от 2 до 100 символов.'
        }
        if (!EMAIL_PATTERN.test(trimmedEmail)) {
            errors.email = 'Введите корректный email.'
        }
        if (password.length < 8) {
            errors.password = 'Пароль должен содержать минимум 8 символов.'
        }
        if (passwordConfirmation !== password) {
            errors.passwordConfirmation = 'Пароли не совпадают.'
        }

        return {trimmedName, trimmedEmail, errors}
    }

    async function handleSubmit(e) {
        e.preventDefault()

        const {trimmedName, trimmedEmail, errors} = validate()
        setFieldErrors(errors)
        setGeneralError(null)
        if (Object.keys(errors).length > 0) return

        setLoading(true)
        try {
            const response = await register({name: trimmedName, email: trimmedEmail, password, passwordConfirmation})
            onSuccess(response.user)
        } catch (err) {
            if (err.code === 'email_already_exists') {
                setFieldErrors({email: 'Пользователь с таким email уже зарегистрирован.'})
            } else {
                setGeneralError(err.message)
            }
        } finally {
            setLoading(false)
        }
    }

    return (
        <div className="screen">
            <h1>Создание аккаунта</h1>
            <form onSubmit={handleSubmit} noValidate>
                <label>
                    Имя
                    <input type="text" value={name} onChange={(e) => setName(e.target.value)} disabled={loading}/>
                    <FieldError message={fieldErrors.name}/>
                </label>
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
                <label>
                    Подтверждение пароля
                    <input
                        type="password"
                        value={passwordConfirmation}
                        onChange={(e) => setPasswordConfirmation(e.target.value)}
                        disabled={loading}
                    />
                    <FieldError message={fieldErrors.passwordConfirmation}/>
                </label>
                {generalError && <p className="error">{generalError}</p>}
                <div className="form-actions">
                    <button type="button" onClick={onBack} disabled={loading}>Назад</button>
                    <button type="submit" disabled={loading}>
                        {loading ? 'Создаём…' : 'Создать аккаунт'}
                    </button>
                </div>
            </form>
            <p className="legal-text">
                Продолжая, вы принимаете{' '}
                <a href="/terms" target="_blank" rel="noreferrer">условия использования</a>{' '}
                и{' '}
                <a href="/privacy" target="_blank" rel="noreferrer">политику конфиденциальности</a>.
            </p>
            <p>
                Уже есть аккаунт?{' '}
                <button type="button" className="link-button" onClick={onGoToLogin}>Войти</button>
            </p>
        </div>
    )
}

export default RegisterScreen