import answerBackground from '../../assets/game/answer-background.png'
import answerOverlay from '../../assets/game/answer-overlay.png'
import selectedRadio from '../../assets/game/answer-radio.svg'
import selectedDot from '../../assets/game/answer-radio-selected.svg'
import mutedRadio from '../../assets/game/answer-radio-muted.svg'
import closeIcon from '../../assets/game/answer-close.svg'
import {GameClose, GameMetrics, GameShell} from './GameShell'
import './game.css'

function AnswerRadio({selected}) {
    return (
        <span className="game__radio" aria-hidden="true">
            <img src={selected ? selectedRadio : mutedRadio} alt="" />
            {selected && <img className="game__radio-dot" src={selectedDot} alt="" />}
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
            className="game--answer"
            background={answerBackground}
            overlay={answerOverlay}
            metrics={<GameMetrics safety={safety} loyalty={loyalty} />}
        >
            <section className="game__sheet" aria-labelledby="answer-title">
                <GameClose icon={closeIcon} onClose={onClose} />
                <div className="game__sheet-content">
                    <div className="game__title-block">
                        <div className="game__step">Шаг {step} из {totalSteps}</div>
                        <h1 className="game__title" id="answer-title">{title}</h1>
                        {description && <p className="game__description">{description}</p>}
                    </div>
                    <div className="game__question">
                        <h2 className="game__question-title">Как вы поступите?</h2>
                        <div className="game__options" role="list">
                            {choices.map((choice) => {
                                const selected = choice.id === selectedChoiceId
                                return (
                                    <div key={choice.id} className={`game__option${selected ? ' game__option--selected' : ''}`}>
                                        <AnswerRadio selected={selected} />
                                        <span>{choice.text}</span>
                                    </div>
                                )
                            })}
                        </div>
                    </div>
                    {error && <p className="game__error" role="alert">{error}</p>}
                    <button
                        type="button"
                        className="game__button"
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
