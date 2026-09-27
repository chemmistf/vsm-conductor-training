import answerBackground from '../../assets/game/answer-background.png'
import answerOverlay from '../../assets/game/answer-overlay.png'
import selectedRadio from '../../assets/game/answer-radio.svg'
import selectedDot from '../../assets/game/answer-radio-selected.svg'
import mutedRadio from '../../assets/game/answer-radio-muted.svg'
import closeIcon from '../../assets/game/answer-close.svg'
import {GameMetrics, GameShell} from './GameShell'
import './game.css'

function AnswerRadio({selected}) {
    return (
        <span className="answer-radio" aria-hidden="true">
            <img src={selected ? selectedRadio : mutedRadio} alt="" />
            {selected && <img className="answer-radio__dot" src={selectedDot} alt="" />}
        </span>
    )
}

function AnswerScreen({
    step = 1,
    totalSteps = 10,
    title = 'Ситуация',
    description = '',
    choices = [],
    selectedChoiceId,
    safety = 49,
    loyalty = 49,
    onContinue,
    onClose,
    loading = false,
    error,
}) {
    return (
        <GameShell
            className="game-screen--answer"
            background={answerBackground}
            overlay={answerOverlay}
            closeIcon={closeIcon}
            onClose={onClose}
            metrics={<GameMetrics safety={safety} loyalty={loyalty} />}
        >
            <section className="game-sheet game-sheet--answer" aria-labelledby="answer-title">
                <div className="game-sheet__content">
                    <div className="game-sheet__title-block">
                        <div className="game-sheet__step">Шаг {step} из {totalSteps}</div>
                        <h1 id="answer-title">{title}</h1>
                        {description && <p>{description}</p>}
                    </div>
                    <div className="variants-question">
                        <h2>Как вы поступите?</h2>
                        <div className="variants-list" role="list">
                            {choices.map((choice) => {
                                const selected = choice.id === selectedChoiceId
                                return (
                                    <div key={choice.id} className={`variant-option${selected ? ' variant-option--selected' : ''}`}>
                                        <AnswerRadio selected={selected} />
                                        <span>{choice.text}</span>
                                    </div>
                                )
                            })}
                        </div>
                    </div>
                    {error && <p className="game-error" role="alert">{error}</p>}
                    <button
                        type="button"
                        className="game-primary-button"
                        onClick={onContinue}
                        disabled={loading || !selectedChoiceId}
                    >
                        {loading ? 'Загрузка…' : 'Продолжить'}
                    </button>
                </div>
            </section>
        </GameShell>
    )
}

export default AnswerScreen
