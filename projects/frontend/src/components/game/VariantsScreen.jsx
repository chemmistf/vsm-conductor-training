import variantsBackground from '../../assets/game/variants-background.png'
import variantsOverlay from '../../assets/game/variants-overlay.png'
import radioIcon from '../../assets/game/variant-radio.svg'
import closeIcon from '../../assets/game/variants-close.svg'
import {GameMetrics, GameShell} from './GameShell'
import './game.css'

const defaultChoices = [
    {
        id: 'assess_and_report',
        text: 'Спокойно оценить состояние пассажира, понять есть ли агрессия или риск для окружающих.',
    },
    {
        id: 'restore_order_first',
        text: 'Сначала восстановить порядок в вагоне: попросить пассажира говорить тише и вернуться к своему месту.',
    },
    {
        id: 'observe_from_distance',
        text: 'Не вступать в прямой конфликт. Продолжить обслуживание других пассажиров и наблюдать с расстояния.',
    },
]

function VariantsScreen({
    step = 1,
    totalSteps = 10,
    title = 'Пассажир сидит у прохода, держит в руках бутылку',
    description = 'Он громко разговаривает с соседом. Несколько пассажиров уже обращают внимание на ситуацию',
    choices = defaultChoices,
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
                        <p>{description}</p>
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

