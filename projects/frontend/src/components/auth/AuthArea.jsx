import {useState} from 'react'
import AuthLandingScreen from './AuthLandingScreen'
import LoginScreen from './LoginScreen'
import RegisterScreen from './RegisterScreen'

function AuthArea({onAuthenticated}) {
    const [authScreen, setAuthScreen] = useState('landing') // 'landing' | 'login' | 'register' | 'forgot-placeholder'

    if (authScreen === 'login') {
        return (
            <LoginScreen
                onSuccess={onAuthenticated}
                onForgotPassword={() => setAuthScreen('forgot-placeholder')}
                onGoToRegister={() => setAuthScreen('register')}
            />
        )
    }

    if (authScreen === 'register') {
        return (
            <RegisterScreen
                onSuccess={onAuthenticated}
                onBack={() => setAuthScreen('landing')}
                onGoToLogin={() => setAuthScreen('login')}
            />
        )
    }

    if (authScreen === 'forgot-placeholder') {
        return (
            <div className="screen">
                <h1>Восстановление пароля</h1>
                <p>Этот экран появится на следующем этапе.</p>
                <button type="button" className="link-button" onClick={() => setAuthScreen('login')}>
                    Назад ко входу
                </button>
            </div>
        )
    }

    return (
        <AuthLandingScreen
            onGoToLogin={() => setAuthScreen('login')}
            onGoToRegister={() => setAuthScreen('register')}
        />
    )
}

export default AuthArea