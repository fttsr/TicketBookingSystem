import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router'
import { bookingsApi, eventsApi, formatDateTime, formatPrice, type EventItem } from '../api/client'
import { useAuth } from '../auth/AuthContext'

export default function EventPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { isAuthenticated } = useAuth()
  const [event, setEvent] = useState<EventItem | null>(null)
  const [loading, setLoading] = useState(true)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState(false)
  useEffect(() => {
    let active = true
    if (!id) { setError('В адресе страницы не указан номер мероприятия.'); setLoading(false); return }
    eventsApi.getById(id).then((item) => { if (active) setEvent(item) })
      .catch((err: Error) => { if (active) setError(err.message) })
      .finally(() => { if (active) setLoading(false) })
    return () => { active = false }
  }, [id])

  async function book() {
    if (!event) return
    if (!isAuthenticated) { navigate('/login?returnTo=' + encodeURIComponent('/events/' + event.id)); return }
    setBusy(true); setError('')
    try { await bookingsApi.create(event.id); setEvent((current) => current ? { ...current, availableTickets: Math.max(0, current.availableTickets - 1) } : current); setSuccess(true) }
    catch (err) { setError(err instanceof Error ? err.message : 'Не удалось создать бронирование.') }
    finally { setBusy(false) }
  }
  if (loading) return <main className="page-shell"><div className="state-panel"><span className="loading-spinner" /><h3>Загружаем мероприятие</h3></div></main>
  if (error && !event) return <main className="page-shell"><div className="state-panel state-panel-error"><span className="state-icon">!</span><h3>Мероприятие не найдено</h3><p>{error}</p><Link className="button button-outline" to="/events">Вернуться к афише</Link></div></main>
  if (!event) return null

  return <main className="page-shell detail-page">
    <Link className="back-link" to="/events"><span>←</span> Все мероприятия</Link>
    {event.isMock && <p className="demo-data-notice" role="status">Бронирование временно недоступно.</p>}
    <div className="detail-layout">
      <article className="detail-main">
        <div className="detail-cover"><span>МЕРОПРИЯТИЕ TBS</span><strong>{event.title}</strong><small>{formatDateTime(event.date)}</small></div>
        <div className="detail-content"><span className="eyebrow eyebrow-muted">ИНФОРМАЦИЯ</span><h1>{event.title}</h1>
          <p className="detail-lead">{event.description || 'Подробная информация о мероприятии.'}</p>
          <div className="detail-info-grid">
            <div className="info-tile"><span className="info-icon">◷</span><div><span className="info-label">Дата и время</span><strong>{formatDateTime(event.date)}</strong></div></div>
            <div className="info-tile"><span className="info-icon">⌖</span><div><span className="info-label">Место</span><strong>{event.venue}</strong></div></div>
            <div className="info-tile"><span className="info-icon">▣</span><div><span className="info-label">Свободно мест</span><strong>{event.availableTickets}</strong></div></div>
          </div>
        </div>
      </article>
      <aside className="booking-panel"><span className="eyebrow eyebrow-muted"></span><h2>Забронировать билет</h2>
        <div className="booking-price">{formatPrice(event.price)} <span>/ билет</span></div><div className="booking-divider" />
        <div className="booking-summary-row"><span>Мероприятие</span><strong>{event.title}</strong></div>
        <div className="booking-summary-row"><span>Место</span><strong>{event.venue}</strong></div>
        {error && <div className="form-alert" role="alert">{error}</div>}
        {event.isMock ? <div className="demo-data-notice booking-demo-notice"></div> : success ? <div className="booking-success"><span>✓</span><strong>Билет забронирован</strong><Link to="/bookings">Открыть мои бронирования →</Link></div> :
          <button className="button button-primary button-wide booking-cta" disabled={busy || event.availableTickets <= 0} onClick={book}>{busy ? 'Бронируем…' : event.availableTickets > 0 ? 'Забронировать билет' : 'Билетов нет'}</button>}
        <p className="booking-footnote">Оплата сейчас не требуется. Бронь появится в вашем списке.</p>
      </aside>
    </div>
  </main>
}
