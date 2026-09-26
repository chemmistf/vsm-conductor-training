import ScaleBar from './ScaleBar'
import Countdown from './Countdown'

function ScenarioScreen({ attempt, onChoose, onTimeout, loading, error }) {
    const { node, safety, loyalty } = attempt

    return (
        <div className="screen">
            <div className="scales">
                <ScaleBar label="Safety" value={safety} />
                <ScaleBar label="Loyalty" value={loyalty} />
            </div>

            {node.deadlineAt && <Countdown deadlineAt={node.deadlineAt} onExpire={onTimeout} />}

            <p className="scenario-text">{node.text}</p>

            <div className="choices">
                {node.choices.map((choice) => (
                    <button
                        key={choice.id}
                        type="button"
                        onClick={() => onChoose(choice.id)}
                        disabled={loading}
                    >
                        {choice.text}
                    </button>
                ))}
            </div>

            {error && <p className="error">{error}</p>}
        </div>
    )
}

export default ScenarioScreen