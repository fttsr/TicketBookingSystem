import { Link } from 'react-router'

export default function NotFoundPage() {
  return <main className="page-shell">
    <div className="state-panel not-found-panel">
      <span className="not-found-code">404</span>
      <h1>Страница не найдена</h1>
      <p>Проверьте адрес или перейдите к списку мероприятий.</p>
      <Link className="button button-primary" to="/events">К мероприятиям</Link>
    </div>
  </main>
}
