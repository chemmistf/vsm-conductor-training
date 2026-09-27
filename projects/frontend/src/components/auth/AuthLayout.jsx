import backArrow from '../../assets/auth/back-arrow.svg'
import inputField from '../../assets/auth/input-field.svg'

export function AuthBackButton({onClick}) {
    return (
        <button type="button" className="auth-back" onClick={onClick}>
            <img src={backArrow} width="8" height="14" alt="" aria-hidden="true"/>
            <span>Назад</span>
        </button>
    )
}

export function AuthField({label, type = 'text', value, onChange, disabled, error}) {
    return (
        <label className={`auth-field${error ? ' auth-field--error' : ''}`}>
            <span className="sr-only">{label}</span>
            <img className="auth-field__background" src={inputField} width="335" height="50" alt="" aria-hidden="true"/>
            <input
                type={type}
                value={value}
                onChange={onChange}
                placeholder={label}
                disabled={disabled}
                autoComplete="off"
            />
            {error && <span className="auth-field__error">{error}</span>}
        </label>
    )
}

export function AuthPage({className = '', children}) {
    return <main className={`auth-page ${className}`}>{children}</main>
}
