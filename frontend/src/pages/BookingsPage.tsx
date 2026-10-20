import { useEffect, useMemo, useState } from 'react'
import { Link, useLocation } from 'react-router'
import { bookingsApi, eventsApi, formatDateTime, formatPrice, getBookingStatus, type BookingItem, type EventItem } from '../api/client'

export default function BookingsPage() {
  const location = useLocation()
  const [bookings, setBookings] = useState<BookingItem[]>([])
  const [events, setEvents] = useState<EventItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [cancelId, setCancelId] = useState('')
  const [busyId, setBusyId] = useState('')
  const state = location.state as { notice?: string } | null

  useEffect(() => {
    let active = true
    Promise.all([bookingsApi.getMine(), eventsApi.getAll()])
      .then(([items, allEvents]) => {
        if (!active) return
        setBookings(items.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()))
        setEvents(allEvents)
      })
      .catch((err: Error) => { if (active) setError(err.message) })
      .finally(() => { if (active) setLoading(false) })
    return () => { active = false }
  }, [])
  const eventById = useMemo(() => new Map(events.map((item) => [item.id, item])), [events])

  async function cancel(id: string) {
    setBusyId(id); setError('')
    try {
      const updated = await bookingsApi.cancel(id)
      setBookings((items) => items.map((item) => item.id === id ? updated : item))
      setCancelId('')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Не удалось отменить бронирование.')
    } finally { setBusyId('') }
  }

  return <main className="page-shell bookings-page">
    <div className="page-heading page-heading-row"><div><span className="eyebrow eyebrow-muted"></span><h1>Мои бронирования</h1></div><Link className="button button-primary" to="/events">Найти событие <span>→</span></Link></div>
    {state?.notice && <div className="success-banner"><span>✓</span>{state.notice}</div>}
    {error && <div className="form-alert booking-alert" role="alert">{error}</div>}
    {loading && <div className="state-panel"><span className="loading-spinner" /><h3>Загружаем ваши бронирования</h3></div>}
    {!loading && !error && bookings.length === 0 && <div className="empty-bookings"><div className="empty-ticket">✳</div><span className="eyebrow eyebrow-muted">ПОКА ПУСТО</span><h2>Бронирований пока нет</h2><p>Выберите мероприятие и забронируйте первый билет.</p><Link className="button button-primary" to="/events">Посмотреть афишу <span>→</span></Link></div>}
    {!loading && bookings.length > 0 && <div className="booking-list">
      {bookings.map((booking) => {
        const event = eventById.get(booking.eventId)
        const status = getBookingStatus(booking.status)
        const date = event ? new Date(event.date) : null
        return <article className="my-booking-card" key={booking.id}>
          <div className="booking-date-block"><span className="booking-date-month">{date && !Number.isNaN(date.getTime()) ? date.toLocaleDateString('ru-RU', { month: 'short' }) : 'БИЛЕТ'}</span><strong>{date && !Number.isNaN(date.getTime()) ? date.getDate() : '—'}</strong><span>{date && !Number.isNaN(date.getTime()) ? date.toLocaleDateString('ru-RU', { weekday: 'short' }) : ''}</span></div>
          <div className="my-booking-main">
            <div className="booking-title-row"><div><span className="eyebrow eyebrow-muted">БРОНЬ · {booking.id.slice(0, 8).toUpperCase()}</span><h2>{event?.title || 'Мероприятие'}</h2></div><span className={'status-pill status-' + status.key}><i />{status.label}</span></div>
            <div className="my-booking-meta"><span><i>◷</i>{event ? formatDateTime(event.date) : 'Дата недоступна'}</span><span><i>⌖</i>{event?.venue || 'Место не указано'}</span><span><i>₽</i>{event ? formatPrice(event.price) : 'Цена не указана'}</span></div>
            <div className="my-booking-actions"><span>Создано {formatDateTime(booking.createdAt)}</span>
              {status.key !== 'cancelled' && (cancelId === booking.id ? <div className="cancel-confirm"><span>Точно отменить?</span><button className="button button-danger button-small" disabled={busyId === booking.id} onClick={() => cancel(booking.id)}>{busyId === booking.id ? 'Отменяем…' : 'Да, отменить'}</button><button className="button button-quiet button-small" onClick={() => setCancelId('')}>Оставить</button></div> : <button className="cancel-link" onClick={() => setCancelId(booking.id)}>Отменить бронь</button>)}
            </div>
          </div>
        </article>
      })}
    </div>}
  </main>
}
