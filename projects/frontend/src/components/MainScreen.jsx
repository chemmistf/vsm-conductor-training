import avatarImage from '../assets/main/avatar.png'
import levelBadge from '../assets/main/level-badge.png'
import progressHero from '../assets/main/progress-hero.png'
import assessmentIcon from '../assets/main/competencies/assessment.svg'
import prioritizationIcon from '../assets/main/competencies/prioritization.svg'
import safetyIcon from '../assets/main/competencies/safety.svg'
import communicationIcon from '../assets/main/competencies/communication.svg'
import timeLimitIcon from '../assets/main/competencies/time-limit.svg'
import bellIcon from '../assets/profile/bell.svg'
import MobileBottomNav from './MobileBottomNav'
import './main.css'

const competencies = [
    ['Оценка ситуации', 'Умеет замечать важные детали и быстро определять риски.', assessmentIcon],
    ['Приоритизация', 'Выбирает правильный порядок действий в сложной ситуации.', prioritizationIcon],
    ['Безопасность', 'Соблюдает правила и заботится о пассажирах.', safetyIcon],
    ['Коммуникация', 'Спокойно объясняет решения и работает с людьми.', communicationIcon],
    ['Соблюдение времени', 'Укладывается в нормативы и сохраняет спокойствие.', timeLimitIcon],
]

function MainScreen({profile, onPlay, onProfile}) {
    const level = profile?.level ?? 8
    const xp = profile?.xp ?? 2480
    const progress = profile?.levelProgressPercent ?? 78
    const name = profile?.name || 'Анна Сергеева'

    return (
        <main className="main-screen">
            <div className="main-screen__scroll">
                <header className="main-screen__header">
                    <button type="button" className="main-screen__identity" onClick={onProfile}>
                        <img src={avatarImage} alt="" />
                        <span>
                            <small>Добрый день,</small>
                            <strong>{name}!</strong>
                        </span>
                    </button>
                    <button type="button" className="main-screen__notifications" aria-label="Уведомления">
                        <img src={bellIcon} alt="" />
                    </button>
                </header>

                <div className="main-screen__content">
                    <section className="main-screen__progress" aria-labelledby="main-progress-title">
                        <h1 id="main-progress-title">Ваш прогресс</h1>
                        <div className="main-screen__level-row">
                            <img src={levelBadge} alt="" aria-hidden="true" />
                            <strong>Уровень {level}</strong>
                            <span>{xp} XP</span>
                        </div>
                        <div className="main-screen__progress-track">
                            <span style={{width: `${progress}%`}} />
                        </div>
                    </section>

                    <button type="button" className="main-screen__scenario-card" onClick={onPlay}>
                        <img src={progressHero} alt="" aria-hidden="true" />
                        <span className="main-screen__scenario-copy">
                            <strong>Новая ситуация уже ждёт</strong>
                            <span>Погрузитесь в ситуацию, принимайте решения и смотрите, к чему приведёт ваш выбор.</span>
                        </span>
                        <span className="main-screen__scenario-action">Играть</span>
                    </button>

                    <section className="main-screen__competencies" aria-labelledby="main-competencies-title">
                        <div className="main-screen__section-heading">
                            <h2 id="main-competencies-title">Мои текущие компетенции</h2>
                            <button type="button" disabled>Динамика →</button>
                        </div>
                        <div className="main-screen__competency-list">
                            {competencies.map(([title, description, icon]) => (
                                <article className="main-screen__competency" key={title}>
                                    <span className="main-screen__competency-icon">
                                        <img src={icon} alt="" aria-hidden="true" />
                                    </span>
                                    <span>
                                        <strong>{title}</strong>
                                        <small>{description}</small>
                                    </span>
                                </article>
                            ))}
                        </div>
                    </section>
                </div>
            </div>
            <MobileBottomNav active="home" onStartGame={onPlay} onProfile={onProfile} onHome={() => window.scrollTo(0, 0)} />
        </main>
    )
}

export default MainScreen
