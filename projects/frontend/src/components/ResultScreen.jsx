function ResultScreen({ result, onRestart }) {
    return (
        <div className="screen">
            <h2>Результат: {result.resultStatus}</h2>
            <p>{result.resultText}</p>

            <div className="xp-reward">
                <h3>Награда</h3>
                <p>+{result.earnedXp} XP</p>
                <p>Всего XP: {result.totalXp} · Уровень {result.level}</p>
            </div>

            <div className="scales">
                <p>Safety: {result.safety.initial} → {result.safety.final}</p>
                <p>Loyalty: {result.loyalty.initial} → {result.loyalty.final}</p>
            </div>

            {result.criticalErrors.length > 0 && (
                <div>
                    <h3>Критические ошибки</h3>
                    <ul>
                        {result.criticalErrors.map((item) => (
                            <li key={`${item.nodeId}-${item.choiceId}`}>
                                {item.choiceText} {item.code && `(${item.code})`}
                            </li>
                        ))}
                    </ul>
                </div>
            )}

            {result.competencies.length > 0 && (
                <div>
                    <h3>Результаты по компетенциям</h3>
                    <ul>
                        {result.competencies.map((item) => (
                            <li key={item.code}>
                                {item.code}: {item.score > 0 ? '+' : ''}{item.score} ({item.level})
                            </li>
                        ))}
                    </ul>
                </div>
            )}

            <div>
                <h3>Хронология</h3>
                <ol>
                    {result.timeline.map((entry, index) => (
                        <li key={index}>
                            {entry.nodeId} — {entry.choiceId ?? 'таймаут'}
                            {' '}(safety {entry.safetyDelta >= 0 ? '+' : ''}{entry.safetyDelta},
                            {' '}loyalty {entry.loyaltyDelta >= 0 ? '+' : ''}{entry.loyaltyDelta})
                            {entry.criticalError && ' — критическая ошибка'}
                        </li>
                    ))}
                </ol>
            </div>

            <button type="button" onClick={onRestart}>Пройти ещё раз</button>
        </div>
    )
}

export default ResultScreen
