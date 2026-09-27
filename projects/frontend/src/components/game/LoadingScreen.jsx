import loadingIllustration from '../../assets/game/loading-illustration.png'
import backArrow from '../../assets/game/back-arrow.svg'
import './game.css'

function LoadingScreen({onBack}) {
    return (
        <main className="game game--loading">
            <button type="button" className="game__back" onClick={onBack}>
                <img src={backArrow} alt="" aria-hidden="true" />
                <span>Назад</span>
            </button>
            <img className="loading__illustration" src={loadingIllustration} alt="" aria-hidden="true" />
            <div className="loading__copy">
                <h1 className="loading__title">Генерируем сценарий</h1>
                <p className="loading__description">Формируем уникальную ситуацию<br />на основе реальных кейсов ВСМ.<br />Это займет несколько секунд</p>
            </div>
        </main>
    )
}

export default LoadingScreen
