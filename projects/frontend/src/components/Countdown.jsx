import { useEffect, useRef, useState } from 'react'

function Countdown({ deadlineAt, onExpire }) {
    const [remainingMs, setRemainingMs] = useState(() => new Date(deadlineAt).getTime() - Date.now())
    const onExpireRef = useRef(onExpire)
    onExpireRef.current = onExpire

    useEffect(() => {
        let expired = false

        const tick = () => {
            const msLeft = new Date(deadlineAt).getTime() - Date.now()
            setRemainingMs(msLeft)

            if (msLeft <= 0 && !expired) {
                expired = true
                onExpireRef.current()
            }
        }

        tick()
        const intervalId = setInterval(tick, 250)
        return () => clearInterval(intervalId)
    }, [deadlineAt])

    const seconds = Math.max(0, Math.ceil(remainingMs / 1000))

    return <div className="countdown">Осталось: {seconds} с</div>
}

export default Countdown