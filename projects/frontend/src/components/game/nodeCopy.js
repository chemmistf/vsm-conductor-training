export function getNodeCopy(node) {
    const text = node?.text?.trim() ?? ''
    if (!text) {
        return {title: 'Ситуация', description: ''}
    }

    const firstSentence = text.match(/^(.+?[.!?])(?:\s+|$)/)
    if (!firstSentence || firstSentence[1].length > 110) {
        return {title: 'Ситуация', description: text}
    }

    return {
        title: firstSentence[1],
        description: text.slice(firstSentence[0].length).trim(),
    }
}

