import startIllustration from '../../assets/auth/start-illustration.png'
import {AuthPage} from './AuthLayout'
import './auth.css'

function AuthLandingScreen({onGoToLogin, onGoToRegister}) {
    return (
        <AuthPage className="auth-page--landing">
            <img className="landing-hero" src={startIllustration} alt=""/>
            <div className="landing-actions">
                <button type="button" className="landing-button landing-button--primary" onClick={onGoToLogin}>
                    Войти
                </button>
                <button type="button" className="landing-button landing-button--secondary" onClick={onGoToRegister}>
                    Зарегистрироваться
                </button>
            </div>
        </AuthPage>
    )
}

export default AuthLandingScreen
