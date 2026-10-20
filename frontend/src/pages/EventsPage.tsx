import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router'
import { eventsApi, formatDate, formatPrice, type EventItem } from '../api/client'

type Filter = 'all' | 'upcoming' | 'available'
function EventArtwork({ event, index }: { event: EventItem; index: number }) {
  return <div className={'event-art event-art-' + ((index % 4) + 1)} aria-hidden="true"><span className="art-date">{formatDate(event.date)}</span><span className="art-mark">TBS</span></div>
}

export default function EventsPage() {
  const [events, setEvents] = useState<EventItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [search, setSearch] = useState('')
  const [filter, setFilter] = useState<Filter>('all')
  useEffect(() => {
    let active = true
    eventsApi.getAll().then((items) => { if (active) setEvents(items) })
      .catch((err: Error) => { if (active) setError(err.message) })
      .finally(() => { if (active) setLoading(false) })
    return () => { active = false }
  }, [])
  const visible = useMemo(() => {
    const query = search.trim().toLocaleLowerCase('ru-RU')
    return events.filter((item) => {
      const match = !query || item.title.toLocaleLowerCase('ru-RU').includes(query) || item.venue.toLocaleLowerCase('ru-RU').includes(query)
      const time = new Date(item.date).getTime()
      const upcoming = !Number.isNaN(time) && time >= Date.now()
      const filtered = filter === 'all' || (filter === 'upcoming' && upcoming) || (filter === 'available' && item.availableTickets > 0)
      return match && filtered
    }).sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
  }, [events, filter, search])

  return <main className="page-shell">
    <section className="events-hero">
      <div><span className="eyebrow eyebrow-muted"><h1>TICKETBOOKINGSYSTEM</h1></span><p>Выберите мероприятие и забронируйте билет.</p></div>
      <span className="events-hero-mark" aria-hidden="true">T B S</span>
    </section>
    <section id="events" className="events-section">
      <div className="section-heading">
        <div><h2>Афиша</h2></div>
        <label className="search-box"><span className="search-icon" aria-hidden="true">⌕</span><span className="visually-hidden">Поиск мероприятия или места</span><input type="search" placeholder="Событие или место" value={search} onChange={(e) => setSearch(e.target.value)} /></label>
      </div>
      <div className="events-toolbar"><div className="filter-pills" aria-label="Фильтры мероприятий">
        <button className={filter === 'all' ? 'filter-pill active' : 'filter-pill'} onClick={() => setFilter('all')}>Все</button>
        <button className={filter === 'upcoming' ? 'filter-pill active' : 'filter-pill'} onClick={() => setFilter('upcoming')}>Предстоящие</button>
        <button className={filter === 'available' ? 'filter-pill active' : 'filter-pill'} onClick={() => setFilter('available')}>Есть билеты</button>
      </div></div>
      {loading && <div className="state-panel"><span className="loading-spinner" /><h3>Загружаем мероприятия</h3></div>}
      {!loading && error && <div className="state-panel state-panel-error"><span className="state-icon">!</span><h3>Не удалось загрузить мероприятия</h3><p>{error}</p><button className="button button-outline" onClick={() => window.location.reload()}>Попробовать снова</button></div>}
      {!loading && !error && visible.length === 0 && <div className="state-panel"><span className="state-icon">⌕</span><h3>{events.length ? 'Ничего не найдено' : 'Мероприятий пока нет'}</h3><p>{events.length ? 'Измените запрос или фильтр.' : 'Когда мероприятия появятся, они будут отображаться здесь.'}</p>{search && <button className="button button-outline" onClick={() => setSearch('')}>Сбросить поиск</button>}</div>}
      {!loading && !error && visible.length > 0 && <div className="event-grid">
        {visible.map((event, index) => <Link className="event-card" to={'/events/' + event.id} key={event.id}>
          <EventArtwork event={event} index={index} />
          <div className="event-card-body"><div className="event-card-meta"><span>{formatDate(event.date)}</span><span aria-hidden="true">·</span><span className="event-venue">{event.venue}</span></div>
            <h3>{event.title}</h3><p className="event-description">{event.description}</p>
            <div className="event-card-bottom"><strong>{formatPrice(event.price)}</strong><span className={event.availableTickets > 0 ? 'availability' : 'availability sold-out'}>{event.availableTickets > 0 ? 'Есть билеты' : 'Нет билетов'}</span></div>
          </div>
        </Link>)}
      </div>}
    </section>
  </main>
}
