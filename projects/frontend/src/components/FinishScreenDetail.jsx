import backArrow from '../assets/start-game/back-arrow.svg'
import './result.css'

function Metric({label, before, after, delta}) {
    const percent = Math.min(100, Math.max(0, after))
    const valueClass = delta > 0 ? 'is-positive' : delta < 0 ? 'is-negative' : 'is-neutral'

    return (
        <div className="detail-metric">
            <div className="detail-metric__heading">
                <span>{label}</span>
                <strong className={valueClass}>{before} → {after}</strong>
            </div>
            <div className="detail-metric__track"><span style={{width: `${percent}%`}} /></div>
        </div>
    )
}

function DetailCard({entry, index, result}) {
    const sceneLabel = entry.nodeId?.startsWith('scene_')
        ? `Сцена ${entry.nodeId.replace('scene_', '')}`
        : `Этап ${index + 1}`
    const safetyBefore = entry.safetyBefore ?? result.safety.initial
    const safetyAfter = entry.safetyAfter ?? safetyBefore + (entry.safetyDelta ?? 0)
    const loyaltyBefore = entry.loyaltyBefore ?? result.loyalty.initial
    const loyaltyAfter = entry.loyaltyAfter ?? loyaltyBefore + (entry.loyaltyDelta ?? 0)

    return (
        <article className="detail-card">
            <div className="detail-card__intro">
                <h2>{sceneLabel}</h2>
                <p>{entry.nodeText || 'Ситуация'}</p>
            </div>
            <div className="detail-card__body">
                <section>
                    <h3>Ваше решение</h3>
                    <p>{entry.choiceText || (entry.choiceId ? `Выбор: ${entry.choiceId}` : 'Время на решение истекло.')}</p>
                </section>
                <section>
                    <h3>Результат</h3>
                    <div className="detail-metrics">
                        <Metric label="Безопасность" before={safetyBefore} after={safetyAfter} delta={entry.safetyDelta ?? 0}/>
                        <Metric label="Лояльность" before={loyaltyBefore} after={loyaltyAfter} delta={entry.loyaltyDelta ?? 0}/>
                    </div>
                </section>
                <section>
                    <h3>К чему это привело</h3>
                    <p>{entry.outcomeText || result.resultText}</p>
                </section>
                {entry.criticalError && (
                    <p className="detail-card__error">Критическая ошибка{entry.criticalErrorCode ? `: ${entry.criticalErrorCode}` : ''}</p>
                )}
            </div>
        </article>
    )
}

function FinishScreenDetail({result, onBack}) {
    const timeline = result?.timeline ?? []

    return (
        <main className="result-detail-screen">
            <header className="result-detail-header">
                <button type="button" className="result-back-button" onClick={onBack}>
                    <img src={backArrow} alt="" />
                    <span>Назад</span>
                </button>
                <div>
                    <h1>Как развивалась ситуация</h1>
                    <p>Вы приняли {timeline.length} решений. Посмотрите, к чему они привели.</p>
                </div>
            </header>
            <div className="result-detail-list">
                {timeline.map((entry, index) => <DetailCard key={`${entry.nodeId}-${index}`} entry={entry} index={index} result={result}/>) }
                {!timeline.length && <p className="result-empty">История решений пока недоступна.</p>}
            </div>
        </main>
    )
}

export default FinishScreenDetail
