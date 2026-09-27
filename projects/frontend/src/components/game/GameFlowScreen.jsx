import questionBackground from '../../assets/game/question-background.png'
import questionOverlay from '../../assets/game/question-overlay.png'
import questionClose from '../../assets/game/question-close.svg'
import variantsBackground from '../../assets/game/variants-background.png'
import variantsOverlay from '../../assets/game/variants-overlay.png'
import variantsClose from '../../assets/game/variants-close.svg'
import answerBackground from '../../assets/game/answer-background.png'
import answerOverlay from '../../assets/game/answer-overlay.png'
import answerClose from '../../assets/game/answer-close.svg'
import selectedRadio from '../../assets/game/answer-radio.svg'
import selectedDot from '../../assets/game/answer-radio-selected.svg'
import mutedRadio from '../../assets/game/answer-radio-muted.svg'
import {GameClose, GameMetrics, GameShell} from './GameShell'
import './game.css'

const screenAssets = {
    question: {
        background: questionBackground,
        overlay: questionOverlay,
        close: questionClose,
    },
    variants: {
        background: variantsBackground,
        overlay: variantsOverlay,
        close: variantsClose,
    },
    answer: {
        background: answerBackground,
        overlay: answerOverlay,
        close: answerClose,
    },
}

function GameRadio({selected}) {
    return (
        <span className="game__radio" aria-hidden="true">
            <img src={selected ? selectedRadio : mutedRadio} alt="" />
            {selected && <img className="game__radio-dot" src={selectedDot} alt="" />}
        </span>
    )
}

function GameOption({choice, selected, interactive, onSelect}) {
    const className = `game__option${selected ? ' game__option--selected' : ''}`

    if (!interactive) {
        return (
            <div className={className}>
                <GameRadio selected={selected} />
                <span className="game__option-label">{choice.text}</span>
            </div>
        )
    }

    return (
        <button
            type="button"
            className={className}
            onClick={() => onSelect?.(choice.id)}
            role="radio"
            aria-checked={selected}
        >
            <GameRadio selected={selected} />
            <span className="game__option-label">{choice.text}</span>
        </button>
    )
}

function GameFlowScreen({
    phase,
    step = 1,
    totalSteps = 10,
    title = 'Ситуация',
    description = '',
    choices = [],
    selectedChoiceId,
    safety = 49,
    loyalty = 49,
    loading = false,
    error,
    onSelect,
    onContinue,
    onClose,
}) {
    const assets = screenAssets[phase]
    const isQuestion = phase === 'question'
    const isVariants = phase === 'variants'
    const isAnswer = phase === 'answer'

    return (
        <GameShell
            className={`game--${phase}`}
            background={assets.background}
            overlay={assets.overlay}
            metrics={<GameMetrics safety={safety} loyalty={loyalty} />}
        >
            <section className="game__sheet" aria-labelledby={`${phase}-title`}>
                <GameClose icon={assets.close} onClose={onClose} />

                {isVariants && <div className="game__step">Шаг {step} из {totalSteps}</div>}

                <div className="game__sheet-content">
                    <div className="game__title-block">
                        {isAnswer && <div className="game__step">Шаг {step} из {totalSteps}</div>}
                        <h1 className="game__title" id={`${phase}-title`}>{title}</h1>
                        {description && <p className="game__description">{description}</p>}
                    </div>

                    {isQuestion && (
                        <button type="button" className="game__button" onClick={onContinue}>
                            Продолжить
                        </button>
                    )}

                    {isVariants && (
                        <div className="game__question">
                            <h2 className="game__question-title">Как вы поступите?</h2>
                            <div className="game__options" role="radiogroup" aria-label="Варианты ответа">
                                {choices.map((choice) => (
                                    <GameOption
                                        key={choice.id}
                                        choice={choice}
                                        selected={choice.id === selectedChoiceId}
                                        interactive
                                        onSelect={onSelect}
                                    />
                                ))}
                            </div>
                        </div>
                    )}

                    {isAnswer && (
                        <div className="game__question">
                            <h2 className="game__question-title">Как вы поступите?</h2>
                            <div className="game__options" role="list">
                                {choices.map((choice) => (
                                    <GameOption
                                        key={choice.id}
                                        choice={choice}
                                        selected={choice.id === selectedChoiceId}
                                    />
                                ))}
                            </div>
                        </div>
                    )}

                    {error && isAnswer && <p className="game__error" role="alert">{error}</p>}

                    {isVariants && (
                        <button
                            type="button"
                            className="game__button"
                            onClick={onContinue}
                            disabled={!selectedChoiceId}
                        >
                            Продолжить
                        </button>
                    )}

                    {isAnswer && (
                        <button
                            type="button"
                            className="game__button"
                            onClick={onContinue}
                            disabled={loading || !selectedChoiceId}
                        >
                            {loading ? 'Загрузка…' : 'Продолжить'}
                        </button>
                    )}
                </div>
            </section>
        </GameShell>
    )
}

export default GameFlowScreen
