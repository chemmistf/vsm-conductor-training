function StartScreen({ onStart, loading, error }) {
    return (
        <div className="screen">
            <h1>VSM Training — демо</h1>
            <p>Тренировочный сценарий: нетрезвый пассажир в вагоне.</p>
            <button type="button" onClick={onStart} disabled={loading}>
                {loading ? 'Загрузка…' : 'Начать'}
            </button>
            {error && <p className="error">{error}</p>}
        </div>
    )
}

export default StartScreen