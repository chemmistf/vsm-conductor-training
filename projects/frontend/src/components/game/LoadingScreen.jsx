import loadingIllustration from '../../assets/game/loading-illustration.png'
import backArrow from '../../assets/game/back-arrow.svg'
import {HomeIndicator, StatusBar} from './GameShell'
import './game.css'

function LoadingScreen({onBack}) {
    return (
        <main className="game-screen game-screen--loading">
            <StatusBar />
            <button type="button" className="game-back" onClick={onBack}>
                <img src={backArrow} alt="" aria-hidden="true" />
                <span>Назад</span>
            </button>
            <img className="loading-screen__illustration" src={loadingIllustration} alt="" aria-hidden="true" />
            <div className="loading-screen__copy">
                <h1>Генерируем сценарий</h1>
                <p>Формируем уникальную ситуацию<br />на основе реальных кейсов ВСМ.<br />Это займет несколько секунд</p>
            </div>
            <HomeIndicator />
        </main>
    )
}

export default LoadingScreen

