import scene2 from '../../assets/game/scenarios/scene2.png'
import scene3a from '../../assets/game/scenarios/scene3a.jpeg'
import scene3b from '../../assets/game/scenarios/scene3b.png'
import scene3c from '../../assets/game/scenarios/scene3c.png'
import scene4 from '../../assets/game/scenarios/scene4.png'
import scene5 from '../../assets/game/scenarios/scene5.png'
import criticalPath from '../../assets/game/scenarios/critical-path.png'

const scenarioImages = {
    scene_2: scene2,
    scene_3a: scene3a,
    scene_3b: scene3b,
    scene_3c: scene3c,
    scene_4: scene4,
    scene_5: scene5,
    safety_critical: criticalPath,
    critical_failure: criticalPath,
}

export function getScenarioImage(nodeId) {
    return scenarioImages[nodeId] ?? null
}

export {criticalPath}
