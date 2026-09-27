function FieldError({message}) {
    if (!message) return null
    return <span className="field-error">{message}</span>
}

export default FieldError