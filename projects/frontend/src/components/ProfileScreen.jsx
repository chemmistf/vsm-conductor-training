import avatar from '../assets/profile/avatar.png'
import badge from '../assets/profile/badge.png'
import achievementImage from '../assets/profile/achievement.png'
import bellIcon from '../assets/profile/bell.svg'
import chevronIcon from '../assets/profile/chevron.svg'
import divider from '../assets/profile/divider.svg'
import MobileBottomNav from './MobileBottomNav'
import './profile.css'

const profileMenu = ['Аналитика тренировок', 'Друзья', 'Настройки', 'Выйти']
const defaultAchievements = [
    {id: 'default-1', title: 'Быстрый старт'},
    {id: 'default-2', title: 'Быстрый старт'},
    {id: 'default-3', title: 'Быстрый старт'},
]

function ProfileScreen({profile, onHome, onStartGame, onLeaderboard, onLogout}) {
    if (!profile) {
        return <main className="profile-screen profile-screen--loading">Загрузка профиля…</main>
    }

    const achievements = profile.achievements?.length ? profile.achievements : defaultAchievements

    return (
        <main className="profile-screen">
            <section className="profile-screen__header">
                <div className="profile-screen__avatar">
                    <div className="profile-screen__avatar-mask">
                        <img src={avatar} alt="" />
                    </div>
                </div>
                <img className="profile-screen__badge" src={badge} alt="" aria-hidden="true" />
                <div className="profile-screen__identity">
                    <h1>{profile.name}</h1>
                    <p>{profile.serviceClass || 'Проводник ВСМ'}</p>
                </div>
                <button type="button" className="profile-screen__notifications" aria-label="Уведомления">
                    <img src={bellIcon} alt="" />
                </button>
            </section>

            <div className="profile-screen__content">
                <section className="profile-card profile-level-card">
                    <h2>Уровень {profile.level}</h2>
                    <div className="profile-level-card__progress">
                        <div className="profile-level-card__bar-group">
                            <strong>{profile.xp} XP</strong>
                            <div className="profile-level-card__track">
                                <span style={{width: `${profile.levelProgressPercent}%`}} />
                            </div>
                            <small>до {profile.level + 1} уровня: {profile.xpToNextLevel} XP</small>
                        </div>
                        <strong>{profile.levelProgressPercent}%</strong>
                    </div>
                </section>

                <section className="profile-card profile-achievements-card">
                    <div className="profile-card__heading">
                        <h2>Достижения</h2>
                        <button type="button">Все →</button>
                    </div>
                    {achievements.length > 0 ? (
                        <div className="profile-achievements">
                            {achievements.slice(0, 3).map((achievement) => (
                                <article key={achievement.id} className="profile-achievement">
                                    <img src={achievementImage} alt="" />
                                    <p>{achievement.title}</p>
                                </article>
                            ))}
                        </div>
                    ) : null}
                </section>

                <section className="profile-card profile-menu-card">
                    {profileMenu.map((item, index) => (
                        <div key={item}>
                            <button
                                type="button"
                                className={`profile-menu-card__item${item === 'Выйти' ? ' profile-menu-card__item--logout' : ''}`}
                                onClick={item === 'Выйти' ? onLogout : undefined}
                                disabled={item !== 'Выйти'}
                            >
                                <span>{item}</span>
                                <img src={chevronIcon} alt="" aria-hidden="true" />
                            </button>
                            {index < profileMenu.length - 1 && <img className="profile-menu-card__divider" src={divider} alt="" aria-hidden="true" />}
                        </div>
                    ))}
                </section>
            </div>

            <MobileBottomNav active="profile" onHome={onHome} onStartGame={onStartGame} onLeaderboard={onLeaderboard} />
        </main>
    )
}

export default ProfileScreen
