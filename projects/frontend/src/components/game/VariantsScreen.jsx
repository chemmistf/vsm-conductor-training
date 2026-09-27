import variantsBackground from '../../assets/game/variants-background.png'
import variantsOverlay from '../../assets/game/variants-overlay.png'
import radioIcon from '../../assets/game/variant-radio.svg'
import closeIcon from '../../assets/game/variants-close.svg'
import {GameMetrics, GameShell} from './GameShell'
import './game.css'

function VariantsScreen({
    step = 1,
    totalSteps = 10,
    title = 'Ситуация',
    description = '',
    choices = [],
    selectedChoiceId,
    safety = 49,
    loyalty = 49,
    onSelect,
    onContinue,
    onClose,
}) {
    return (
        <GameShell
            className="game--variants"
            background={variantsBackground}
            overlay={variantsOverlay}
            closeIcon={closeIcon}
            onClose={onClose}
            metrics={<GameMetrics safety={safety} loyalty={loyalty} />}
        >
            <section className="game__sheet" aria-labelledby="variants-title">
                <div className="game__step">Шаг {step} из {totalSteps}</div>
                <div className="game__sheet-content">
                    <div className="game__title-block">
                        <h1 className="game__title">{title}</h1>
                        {description && <p className="game__description">{description}</p>}
                    </div>
                    <div className="game__question">
                        <h2 className="game__question-title" id="variants-title">Как вы поступите?</h2>
                        <div className="game__options" role="radiogroup" aria-labelledby="variants-title">
                            {choices.map((choice) => {
                                const selected = choice.id === selectedChoiceId
                                return (
                                    <button
                                        key={choice.id}
                                        type="button"
                                        className={`game__option${selected ? ' game__option--selected' : ''}`}
                                        onClick={() => onSelect?.(choice.id)}
                                        role="radio"
                                        aria-checked={selected}
                                    >
                                        <img src={radioIcon} alt="" aria-hidden="true" />
                                        <span className="game__option-label">{choice.text}</span>
                                    </button>
                                )
                            })}
                        </div>
                    </div>
                    <button
                        type="button"
                        className="game__button"
                        onClick={onContinue}
                        disabled={!selectedChoiceId}
                    >
                        Продолжить
                    </button>
                </div>
            </section>
        </GameShell>
    )
}

export default VariantsScreen
