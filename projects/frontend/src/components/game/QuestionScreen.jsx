import questionBackground from '../../assets/game/question-background.png'
import questionOverlay from '../../assets/game/question-overlay.png'
import closeIcon from '../../assets/game/question-close.svg'
import {GameClose, GameMetrics, GameShell} from './GameShell'
import './game.css'

function QuestionScreen({
    step = 1,
    totalSteps = 10,
    title = 'Ситуация',
    description = '',
    safety = 49,
    loyalty = 49,
    onContinue,
    onClose,
}) {
    return (
        <GameShell
            className="game--question"
            background={questionBackground}
            overlay={questionOverlay}
            metrics={<GameMetrics safety={safety} loyalty={loyalty} />}
        >
            <section className="game__sheet" aria-labelledby="question-title">
                <GameClose icon={closeIcon} onClose={onClose} />
                <div className="game__step">Шаг {step} из {totalSteps}</div>
                <div className="game__sheet-content">
                    <div className="game__title-block">
                        <h1 className="game__title" id="question-title">{title}</h1>
                        {description && <p className="game__description">{description}</p>}
                    </div>
                    <button type="button" className="game__button" onClick={onContinue}>Продолжить</button>
                </div>
            </section>
        </GameShell>
    )
}

export default QuestionScreen
