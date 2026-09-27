export function GameMetric({label, value = 49}) {
    const boundedValue = Math.max(0, Math.min(100, value))

    return (
        <div className="game-metric">
            <div className="game-metric__label">
                <span>{label}</span>
                <strong>{boundedValue}/100</strong>
            </div>
            <div className="game-metric__track">
                <span style={{width: `${boundedValue}%`}} />
            </div>
        </div>
    )
}

export function GameMetrics({safety, loyalty, className = ''}) {
    return (
        <div className={`game-metrics ${className}`}>
            <GameMetric label="Безопасность" value={safety} />
            <GameMetric label="Лояльность" value={loyalty} />
        </div>
    )
}

export function GameShell({
    children,
    background,
    overlay,
    closeIcon,
    className = '',
    onClose,
    metrics,
}) {
    return (
        <main className={`game-screen ${className}`}>
            {background && <img className="game-screen__background" src={background} alt="" aria-hidden="true" />}
            {overlay && <img className="game-screen__overlay" src={overlay} alt="" aria-hidden="true" />}
            <div className="game-screen__shade" aria-hidden="true" />
            {metrics}
            {closeIcon && (
                <button type="button" className="game-close" onClick={onClose} aria-label="Закрыть">
                    <img src={closeIcon} alt="" />
                </button>
            )}
            <div className="game-screen__content">{children}</div>
        </main>
    )
}
