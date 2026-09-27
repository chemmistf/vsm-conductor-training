import {Fragment, useEffect, useState} from 'react'
import avatar from '../assets/profile/avatar.png'
import backArrow from '../assets/leaderboard/back-arrow.svg'
import bronzeMedal from '../assets/leaderboard/bronze-medal.png'
import divider from '../assets/leaderboard/divider.svg'
import goldMedal from '../assets/leaderboard/gold-medal.png'
import hero from '../assets/leaderboard/hero.png'
import silverMedal from '../assets/leaderboard/silver-medal.png'
import MobileBottomNav from './MobileBottomNav'
import {getLeaderboard} from '../api/profile'
import './leaderboard.css'

const periodFilters = [
    ['week', 'Неделя'],
    ['company', 'Компания'],
    ['all_time', 'Все время'],
]

const scopeFilters = [
    ['brigade', 'Бригада'],
    ['depot', 'Депо'],
    ['company', 'Компания'],
    ['friends', 'Друзья'],
]

const medalImages = [goldMedal, silverMedal, bronzeMedal]

function LeaderboardFilter({filters, value, onChange, className = ''}) {
    return (
        <div className={`leaderboard-filter ${className}`.trim()} role="group">
            {filters.map(([id, label], index) => (
                <Fragment key={id}>
                    <button
                        type="button"
                        className={`leaderboard-filter__item${value === id ? ' leaderboard-filter__item--active' : ''}`}
                        aria-pressed={value === id}
                        onClick={() => onChange(id)}
                    >
                        {label}
                    </button>
                    {index < filters.length - 1 && (
                        <span className="leaderboard-filter__divider" aria-hidden="true">
                            <img src={divider} alt="" />
                        </span>
                    )}
                </Fragment>
            ))}
        </div>
    )
}

function LeaderboardRow({entry}) {
    const medal = entry.rank <= medalImages.length ? medalImages[entry.rank - 1] : null

    return (
        <article className={`leaderboard-row${entry.isCurrentUser ? ' leaderboard-row--current' : ''}`}>
            <div className="leaderboard-row__rank">
                {medal ? <img src={medal} alt={`Место ${entry.rank}`} /> : entry.rank}
            </div>
            <img className="leaderboard-row__avatar" src={avatar} alt="" aria-hidden="true" />
            <div className="leaderboard-row__details">
                <div className="leaderboard-row__topline">
                    <div className="leaderboard-row__identity">
                        <strong>{entry.name}</strong>
                        <span>{entry.serviceClass}</span>
                    </div>
                    <strong className="leaderboard-row__xp">{entry.xp} XP</strong>
                </div>
                <div className="leaderboard-row__progress">
                    <span style={{width: `${entry.levelProgressPercent}%`}} />
                </div>
            </div>
        </article>
    )
}

function LeaderboardScreen({onHome, onStartGame, onProfile, onBack}) {
    const [period, setPeriod] = useState('week')
    const [scope, setScope] = useState('brigade')
    const [leaderboard, setLeaderboard] = useState(null)
    const [error, setError] = useState(null)

    useEffect(() => {
        let cancelled = false

        getLeaderboard({period, scope})
            .then((data) => {
                if (cancelled) return
                setError(null)
                setLeaderboard(data)
            })
            .catch((err) => {
                if (!cancelled) setError(err.message)
            })

        return () => {
            cancelled = true
        }
    }, [period, scope])

    const entries = leaderboard?.entries ?? []

    return (
        <main className="leaderboard-screen">
            <div className="leaderboard-screen__scroll">
                <img className="leaderboard-screen__hero" src={hero} alt="" aria-hidden="true" />

                <header className="leaderboard-screen__header">
                    <button type="button" className="leaderboard-screen__back" onClick={onBack}>
                        <span className="leaderboard-screen__back-icon">
                            <img src={backArrow} alt="" aria-hidden="true" />
                        </span>
                        <span>Назад</span>
                    </button>
                    <div className="leaderboard-screen__heading">
                        <h1>Доска лидеров</h1>
                        <p>Соревнуйтесь с коллегами, развивайтесь и занимайте новые вершины!</p>
                    </div>
                </header>

                <section className="leaderboard-screen__content" aria-label="Рейтинг сотрудников">
                    <LeaderboardFilter filters={periodFilters} value={period} onChange={setPeriod} />
                    <LeaderboardFilter
                        className="leaderboard-filter--scope"
                        filters={scopeFilters}
                        value={scope}
                        onChange={setScope}
                    />

                    <section className="leaderboard-card" aria-live="polite">
                        {error && <p className="leaderboard-card__message leaderboard-card__message--error">{error}</p>}
                        {!error && !leaderboard && <p className="leaderboard-card__message">Загружаем рейтинг…</p>}
                        {!error && leaderboard && entries.length === 0 && (
                            <p className="leaderboard-card__message">В этом разделе пока нет участников.</p>
                        )}
                        {entries.map((entry) => <LeaderboardRow key={entry.userId} entry={entry} />)}
                    </section>
                </section>
            </div>

            <MobileBottomNav
                active="leaders"
                onHome={onHome}
                onStartGame={onStartGame}
                onProfile={onProfile}
            />
        </main>
    )
}

export default LeaderboardScreen
