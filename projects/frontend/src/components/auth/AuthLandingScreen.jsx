function AuthLandingScreen({onGoToLogin, onGoToRegister}) {
    return (
        <div className="screen">
            <h1>VSM Training</h1>
            <p>Тренажёр для отработки рабочих сценариев.</p>
            <button type="button" onClick={onGoToLogin}>Войти</button>
            <button type="button" onClick={onGoToRegister}>Зарегистрироваться</button>
        </div>
    )
}

export default AuthLandingScreen