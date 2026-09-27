import {useState} from 'react'
import {register} from '../../api/auth'
import {isValidEmail} from '../../validation/email'
import {AuthBackButton, AuthField, AuthPage} from './AuthLayout'
import './auth.css'

function RegisterScreen({onSuccess, onBack, onGoToLogin, initialValues = {}, onDraftChange}) {
    const [form, setForm] = useState({
        name: initialValues.name ?? '',
        email: initialValues.email ?? '',
        password: initialValues.password ?? '',
        passwordConfirmation: initialValues.passwordConfirmation ?? '',
    })
    const [fieldErrors, setFieldErrors] = useState({})
    const [generalError, setGeneralError] = useState(null)
    const [loading, setLoading] = useState(false)

    function validate() {
        const trimmedName = form.name.trim()
        const trimmedEmail = form.email.trim().toLowerCase()
        const errors = {}

        if (trimmedName.length < 2 || trimmedName.length > 100) {
            errors.name = 'Имя должно содержать от 2 до 100 символов.'
        }
        if (!isValidEmail(trimmedEmail)) {
            errors.email = 'Введите корректный email.'
        }
        if (form.password.length < 8) {
            errors.password = 'Пароль должен содержать минимум 8 символов.'
        }
        if (form.passwordConfirmation !== form.password) {
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
            const response = await register({
                name: trimmedName,
                email: trimmedEmail,
                password: form.password,
                passwordConfirmation: form.passwordConfirmation,
            })
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

    function updateField(field, value) {
        const next = {...form, [field]: value}
        setForm(next)
        onDraftChange?.(next)
    }

    return (
        <AuthPage className="auth-page--signup">
            <form className="auth-content auth-content--wide" onSubmit={handleSubmit} noValidate>
                <AuthBackButton onClick={onBack}/>
                <div className="auth-content__body">
                    <div className="auth-title">
                        <h1>Создайте аккаунт</h1>
                        <p>Создайте аккаунт, чтобы сохранять прогресс и получать награды.</p>
                    </div>
                    <div className="auth-fields">
                        <AuthField label="Ваше Имя" value={form.name} onChange={(e) => updateField('name', e.target.value)} disabled={loading} error={fieldErrors.name}/>
                        <AuthField label="Ваша почта" type="email" value={form.email} onChange={(e) => updateField('email', e.target.value)} disabled={loading} error={fieldErrors.email}/>
                        <AuthField label="Пароль" type="password" value={form.password} onChange={(e) => updateField('password', e.target.value)} disabled={loading} error={fieldErrors.password}/>
                        <AuthField label="Подтверждение пароля" type="password" value={form.passwordConfirmation} onChange={(e) => updateField('passwordConfirmation', e.target.value)} disabled={loading} error={fieldErrors.passwordConfirmation}/>
                    </div>
                    {generalError && <p className="auth-general-error">{generalError}</p>}
                </div>
                <p className="auth-legal">
                    Нажимая «Создать аккаунт», вы подтверждаете согласие с{' '}
                    <a href="/terms" target="_blank" rel="noreferrer">Условиями использования</a> и{' '}
                    <a href="/privacy" target="_blank" rel="noreferrer">Политикой конфиденциальности</a>.
                </p>
                <div className="auth-dock">
                    <div className="auth-dock__primary">
                        <button type="submit" className="auth-primary" disabled={loading}>
                            {loading ? 'Создаём…' : 'Создать аккаунт'}
                        </button>
                    </div>
                    <div className="auth-dock__footer">
                        <span>Уже есть аккаунт?</span>
                        <button type="button" className="auth-link auth-link--strong" onClick={onGoToLogin}>Войти</button>
                    </div>
                </div>
            </form>
        </AuthPage>
    )
}

export default RegisterScreen
