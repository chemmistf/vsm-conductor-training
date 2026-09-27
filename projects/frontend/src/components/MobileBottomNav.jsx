import homeIcon from '../assets/profile/nav-home.svg'
import chartIcon from '../assets/profile/nav-chart.svg'
import trainIcon from '../assets/profile/nav-train.svg'
import signalIcon from '../assets/profile/nav-signal.svg'
import userIcon from '../assets/profile/nav-user.svg'

const items = [
    ['home', 'Главная', homeIcon],
    ['skills', 'Компетенции', chartIcon],
    ['play', 'Играть', trainIcon],
    ['leaders', 'Доска лидеров', signalIcon],
    ['profile', 'Профиль', userIcon],
]

function MobileBottomNav({active, onHome, onStartGame, onLeaderboard, onProfile}) {
    return (
        <nav className="mobile-bottom-nav" aria-label="Основная навигация">
            {items.map(([id, label, icon]) => {
                const action = id === 'home'
                    ? onHome
                    : id === 'play'
                        ? onStartGame
                        : id === 'leaders'
                            ? onLeaderboard
                            : id === 'profile'
                                ? onProfile
                                : undefined
                return (
                    <button
                        key={id}
                        type="button"
                        className={`mobile-bottom-nav__item${active === id ? ' mobile-bottom-nav__item--active' : ''}`}
                        onClick={action}
                        disabled={!action}
                    >
                        <img src={icon} alt="" aria-hidden="true" />
                        <span>{label}</span>
                    </button>
                )
            })}
        </nav>
    )
}

export default MobileBottomNav
