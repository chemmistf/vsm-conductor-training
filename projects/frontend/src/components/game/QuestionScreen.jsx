import questionBackground from '../../assets/game/question-background.png'
import questionOverlay from '../../assets/game/question-overlay.png'
import closeIcon from '../../assets/game/question-close.svg'
import {GameMetrics, GameShell} from './GameShell'
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
            className="game-screen--question"
            background={questionBackground}
            overlay={questionOverlay}
            closeIcon={closeIcon}
            onClose={onClose}
            metrics={<GameMetrics safety={safety} loyalty={loyalty} />}
        >
            <section className="game-sheet game-sheet--question" aria-labelledby="question-title">
                <div className="game-sheet__step">Шаг {step} из {totalSteps}</div>
                <div className="game-sheet__content">
                    <div className="game-sheet__title-block">
                        <h1 id="question-title">{title}</h1>
                        {description && <p>{description}</p>}
                    </div>
                    <button type="button" className="game-primary-button" onClick={onContinue}>Продолжить</button>
                </div>
            </section>
        </GameShell>
    )
}

export default QuestionScreen
