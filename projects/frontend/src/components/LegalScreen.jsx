function LegalScreen({page}) {
    const isTerms = page === 'terms'

    return (
        <div className="screen">
            <h1>{isTerms ? 'Условия использования' : 'Политика конфиденциальности'}</h1>
            <p>
                {isTerms
                    ? 'Используйте тренажёр добросовестно и только для обучения. Это демонстрационная версия приложения.'
                    : 'Приложение использует email, имя и результаты попыток только для работы профиля и тренажёра.'}
            </p>
            <a href="/">Вернуться назад</a>
        </div>
    )
}

export default LegalScreen
