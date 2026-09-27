import assessmentIcon from '../assets/main/competencies/assessment.svg'
import communicationIcon from '../assets/main/competencies/communication.svg'
import prioritizationIcon from '../assets/main/competencies/prioritization.svg'
import safetyIcon from '../assets/main/competencies/safety.svg'
import timeLimitIcon from '../assets/main/competencies/time-limit.svg'
import finishIllustration from '../assets/result/finish-illustration.png'
import closeIcon from '../assets/game/answer-close.svg'
import {criticalPath} from './game/scenarioAssets'
import './result.css'

const competencyMeta = {
    situation_assessment: {name: 'Оценка ситуации', icon: assessmentIcon},
    prioritization: {name: 'Приоритизация', icon: prioritizationIcon},
    safety_compliance: {name: 'Безопасность и регламент', icon: safetyIcon},
    communication: {name: 'Коммуникация', icon: communicationIcon},
    time_management: {name: 'Оперативность', icon: timeLimitIcon},
}

const competencyOrder = Object.keys(competencyMeta)

function getCompetencyMeta(item) {
    return competencyMeta[item.code] ?? {name: item.name ?? item.code, icon: null}
}

function getResultTitle() {
    return 'Сценарий завершен'
}

function getResultDescription(status) {
    if (status === 'success') return 'Вы успешно решили инцедент.'
    if (status === 'critical_failure') return 'Критическая ошибка! Ваше решение категорически запрещено!'
    return 'Сценарий провален, вы допустили ошибки'
}

function formatScore(score) {
    return score > 0 ? `+${score}` : String(score)
}

function getProgress(totalXp, level) {
    const levelStart = Math.max(0, (level - 1) * 500)
    const progress = Math.min(100, Math.max(0, ((totalXp - levelStart) / 500) * 100))
    return {percent: progress, xpToNext: Math.max(0, level * 500 - totalXp)}
}

export function CompetencyRows({competencies = [], compact = false, includeEmpty = false}) {
    const competencyByCode = new Map(competencies.map((item) => [item.code, item]))
    const rows = includeEmpty
        ? competencyOrder.map((code) => ({
            code,
            score: competencyByCode.get(code)?.score ?? 0,
        }))
        : competencies
    const sorted = [...rows].sort((a, b) => {
        const aIndex = competencyOrder.indexOf(a.code)
        const bIndex = competencyOrder.indexOf(b.code)
        return (aIndex < 0 ? 99 : aIndex) - (bIndex < 0 ? 99 : bIndex)
    })

    return (
        <div className={`result-competency-list${compact ? ' result-competency-list--compact' : ''}`}>
            {sorted.map((item) => {
                const meta = getCompetencyMeta(item)
                const scoreClass = item.score > 0 ? 'is-positive' : item.score < 0 ? 'is-negative' : 'is-neutral'

                return (
                    <div className="result-competency-row" key={item.code}>
                        <div className="result-competency-name">
                            {meta.icon && <img src={meta.icon} alt="" />}
                            <span>{meta.name}</span>
                        </div>
                        <strong className={scoreClass}>{formatScore(item.score)}</strong>
                    </div>
                )
            })}
        </div>
    )
}

function XpCard({result}) {
    const {percent, xpToNext} = getProgress(result.totalXp, result.level)

    return (
        <section className="result-card result-xp-card" aria-labelledby="result-xp-title">
            <div className="result-card__inner">
                <div className="result-card__heading">
                    <h2 id="result-xp-title">Получено очков</h2>
                    <span className="result-level">Уровень {result.level}</span>
                </div>
                <div className="result-xp-summary">
                    <strong>+ {result.earnedXp} <small>XP</small></strong>
                    <span>Всего: {result.totalXp} XP</span>
                </div>
                <div className="result-level-track" aria-label={`Прогресс до уровня ${result.level + 1}`}>
                    <span style={{width: `${percent}%`}} />
                </div>
                <p className="result-xp-hint">до {result.level + 1} уровня: {xpToNext} XP</p>
            </div>
        </section>
    )
}

function ResultScreen({result, onRestart, onDetails}) {
    const status = result?.resultStatus ?? 'success'
    const isCritical = status === 'critical_failure'

    return (
        <main className="result-screen">
            <section className={`result-hero${isCritical ? ' result-hero--critical' : ''}`}>
                <div className="result-hero__background" style={isCritical ? {backgroundImage: `url(${criticalPath})`} : undefined} />
                <img className="result-hero__illustration" src={finishIllustration} alt="" />
                <button type="button" className="result-hero__close" aria-label="Закрыть результат" onClick={onRestart}>
                    <img src={closeIcon} alt="" />
                </button>
                <div className="result-hero__copy">
                    <h1>{getResultTitle()}</h1>
                    <p>{getResultDescription(status)}</p>
                </div>
            </section>

            <div className="result-content">
                <XpCard result={result} />
                <section className="result-card result-competencies-card" aria-labelledby="result-competencies-title">
                    <h2 id="result-competencies-title">Результаты по компетенциям</h2>
                    <CompetencyRows competencies={result.competencies}/>
                </section>
                <button type="button" className="result-primary-button" onClick={onDetails}>Посмотреть детали</button>
            </div>
        </main>
    )
}

export default ResultScreen
