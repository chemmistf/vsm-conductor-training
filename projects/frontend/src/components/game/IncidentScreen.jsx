import incidentBackground from '../../assets/game/game-background.png'
import incidentOverlay from '../../assets/game/game-overlay.png'
import incidentIllustration from '../../assets/game/incident-illustration.png'
import closeIcon from '../../assets/game/incident-close.svg'
import {GameShell} from './GameShell'
import './game.css'

function IncidentScreen({onStart, onClose}) {
    return (
        <GameShell
            className="game--incident"
            background={incidentBackground}
            overlay={incidentOverlay}
            closeIcon={closeIcon}
            onClose={onClose}
        >
            <section className="incident" aria-labelledby="incident-title">
                <img className="incident__illustration" src={incidentIllustration} alt="" aria-hidden="true" />
                <div className="incident__body">
                    <div className="incident__copy">
                        <h1 className="incident__title" id="incident-title">Инцидент</h1>
                        <p className="incident__description">
                            Пассажир стал нетрезвым в пути.<br />
                            Он употребляет алкоголь не в бистро, громко разговаривает и мешает соседям.
                        </p>
                        <p className="incident__description">Вам необходимо урегулировать ситуацию, руководствуясь регламентом.</p>
                    </div>
                    <button type="button" className="game__button" onClick={onStart}>Начать</button>
                </div>
            </section>
        </GameShell>
    )
}

export default IncidentScreen
