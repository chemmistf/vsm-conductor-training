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
            className="game-screen--variants"
            background={variantsBackground}
            overlay={variantsOverlay}
            closeIcon={closeIcon}
            onClose={onClose}
            metrics={<GameMetrics safety={safety} loyalty={loyalty} />}
        >
            <section className="game-sheet game-sheet--variants" aria-labelledby="variants-title">
                <div className="game-sheet__step">Шаг {step} из {totalSteps}</div>
                <div className="game-sheet__content">
                    <div className="game-sheet__title-block">
                        <h1>{title}</h1>
                        {description && <p>{description}</p>}
                    </div>
                    <div className="variants-question">
                        <h2 id="variants-title">Как вы поступите?</h2>
                        <div className="variants-list" role="radiogroup" aria-labelledby="variants-title">
                            {choices.map((choice) => {
                                const selected = choice.id === selectedChoiceId
                                return (
                                    <button
                                        key={choice.id}
                                        type="button"
                                        className={`variant-option${selected ? ' variant-option--selected' : ''}`}
                                        onClick={() => onSelect?.(choice.id)}
                                        role="radio"
                                        aria-checked={selected}
                                    >
                                        <img src={radioIcon} alt="" aria-hidden="true" />
                                        <span>{choice.text}</span>
                                    </button>
                                )
                            })}
                        </div>
                    </div>
                    <button
                        type="button"
                        className="game-primary-button"
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
