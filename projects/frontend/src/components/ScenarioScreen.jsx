import ScaleBar from './ScaleBar'

function ScenarioScreen({ attempt, onChoose, loading, error }) {
    const { node, safety, loyalty } = attempt

    return (
        <div className="screen">
            <div className="scales">
                <ScaleBar label="Safety" value={safety} />
                <ScaleBar label="Loyalty" value={loyalty} />
            </div>

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