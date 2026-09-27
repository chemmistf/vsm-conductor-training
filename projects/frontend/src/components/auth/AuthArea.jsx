import {useState} from 'react'
import AuthLandingScreen from './AuthLandingScreen'
import LoginScreen from './LoginScreen'
import RegisterScreen from './RegisterScreen'
import ForgotPasswordScreen from './ForgotPasswordScreen'

function AuthArea({onAuthenticated}) {
    const passwordWasReset = new URLSearchParams(window.location.search).get('passwordReset') === 'success'

    const [authScreen, setAuthScreen] = useState(passwordWasReset ? 'login' : 'landing')
    const [registerDraft, setRegisterDraft] = useState({
        name: '',
        email: '',
        password: '',
        passwordConfirmation: '',
    })
    const [infoMessage, setInfoMessage] = useState(
        passwordWasReset ? 'Пароль изменён. Войдите с новым паролем.' : null
    )

    if (authScreen === 'login') {
        return (
            <LoginScreen
                infoMessage={infoMessage}
                onSuccess={onAuthenticated}
                onBack={() => setAuthScreen('landing')}
                onForgotPassword={() => {
                    setInfoMessage(null)
                    setAuthScreen('forgot')
                }}
                onGoToRegister={() => {
                    setInfoMessage(null)
                    setAuthScreen('register')
                }}
            />
        )
    }

    if (authScreen === 'register') {
        return (
            <RegisterScreen
                onSuccess={onAuthenticated}
                initialValues={registerDraft}
                onDraftChange={setRegisterDraft}
                onBack={() => setAuthScreen('landing')}
                onGoToLogin={() => setAuthScreen('login')}
            />
        )
    }

    if (authScreen === 'forgot') {
        return <ForgotPasswordScreen onBackToLogin={() => setAuthScreen('login')}/>
    }

    return (
        <AuthLandingScreen
            onGoToLogin={() => setAuthScreen('login')}
            onGoToRegister={() => setAuthScreen('register')}
        />
    )
}

export default AuthArea
