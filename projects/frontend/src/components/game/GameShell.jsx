export function GameMetric({label, value = 49}) {
    const boundedValue = Math.max(0, Math.min(100, value))

    return (
        <div className="game__metric">
            <div className="game__metric-label">
                <span>{label}</span>
                <strong>{boundedValue}/100</strong>
            </div>
            <div className="game__metric-track">
                <span style={{width: `${boundedValue}%`}} />
            </div>
        </div>
    )
}

export function GameMetrics({safety, loyalty, className = ''}) {
    return (
        <div className={`game__metrics ${className}`}>
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
        <main className={`game ${className}`}>
            {background && <img className="game__background" src={background} alt="" aria-hidden="true" />}
            {overlay && <img className="game__overlay" src={overlay} alt="" aria-hidden="true" />}
            <div className="game__shade" aria-hidden="true" />
            {metrics}
            {closeIcon && (
                <button type="button" className="game__close" onClick={onClose} aria-label="Закрыть">
                    <img src={closeIcon} alt="" />
                </button>
            )}
            <div className="game__content">{children}</div>
        </main>
    )
}
