import quickTrainingImage from '../assets/start-game/quick-training.png'
import selectScenarioImage from '../assets/start-game/select-scenario.png'
import cardArrow from '../assets/start-game/card-arrow.svg'
import backArrow from '../assets/start-game/back-arrow.svg'
import MobileBottomNav from './MobileBottomNav'
import './profile.css'
import './start-game.css'

function StartGameScreen({scenarios = [], onQuickStart, onSelectScenario, onHome, onProfile, onBack}) {
    const firstScenario = scenarios[0]

    return (
        <main className="start-game-screen">
            <div className="start-game-screen__scroll">
                <button type="button" className="start-game-screen__back" onClick={onBack}>
                    <img src={backArrow} alt="" aria-hidden="true" />
                    <span>Назад</span>
                </button>

                <header className="start-game-screen__heading">
                    <h1>Начать игру</h1>
                    <p>Выберите формат тренировки или проверьте свои навыки в экзамене.</p>
                </header>

                <section className="start-game-screen__cards">
                    <button type="button" className="start-game-card start-game-card--blue" onClick={onQuickStart}>
                        <img src={quickTrainingImage} alt="" aria-hidden="true" />
                        <span className="start-game-card__copy">
                            <strong>Быстрая тренировка</strong>
                            <span>Начните игру сразу. Сценарий будет выбран автоматически, чтобы вы могли быстро потренироваться без лишних настроек.</span>
                        </span>
                        <img className="start-game-card__arrow" src={cardArrow} alt="" aria-hidden="true" />
                    </button>

                    <button
                        type="button"
                        className="start-game-card start-game-card--blue"
                        onClick={() => firstScenario && onSelectScenario?.(firstScenario.id)}
                        disabled={!firstScenario}
                    >
                        <img src={selectScenarioImage} alt="" aria-hidden="true" />
                        <span className="start-game-card__copy">
                            <strong>Выбрать сценарий</strong>
                            <span>{firstScenario ? firstScenario.title : 'Сценарии загружаются из базы данных.'}</span>
                        </span>
                        <img className="start-game-card__arrow" src={cardArrow} alt="" aria-hidden="true" />
                    </button>
                </section>

                <h2 className="start-game-screen__section-title">Повышение квалификации</h2>
                <button type="button" className="start-game-card start-game-card--light" disabled>
                    <span className="start-game-card__copy">
                        <strong>Экзамен</strong>
                        <span>Раздел станет доступен после накопления тренировочного прогресса.</span>
                    </span>
                    <img className="start-game-card__arrow" src={cardArrow} alt="" aria-hidden="true" />
                </button>
            </div>
            <MobileBottomNav active="play" onHome={onHome} onProfile={onProfile} />
        </main>
    )
}

export default StartGameScreen
