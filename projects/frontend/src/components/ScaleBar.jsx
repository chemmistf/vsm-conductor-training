function ScaleBar({ label, value }) {
    return (
        <div className="scale-bar">
            <div className="scale-bar__label">
                <span>{label}</span>
                <span>{value}</span>
            </div>
            <div className="scale-bar__track">
                <div className="scale-bar__fill" style={{ width: `${value}%` }} />
            </div>
        </div>
    )
}

export default ScaleBar