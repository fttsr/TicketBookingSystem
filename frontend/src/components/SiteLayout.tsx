import { Link, NavLink, Outlet, useNavigate } from 'react-router'
import { useAuth } from '../auth/AuthContext'

export default function SiteLayout() {
  const { isAuthenticated, signOut } = useAuth()
  const navigate = useNavigate()
  function logout() { signOut(); navigate('/events') }

  return <div className="site">
    <header className="site-header"><div className="header-inner">
      <Link className="brand" to="/events" aria-label="TBS — мероприятия"><span className="brand-mark">TBS</span><span className="brand-name">TicketBookingSystem</span></Link>
      <nav className="main-nav" aria-label="Главная навигация">
        <NavLink to="/events" className={({ isActive }) => 'nav-link' + (isActive ? ' is-active' : '')}>Мероприятия</NavLink>
        {isAuthenticated && <NavLink to="/bookings" className={({ isActive }) => 'nav-link' + (isActive ? ' is-active' : '')}>Мои бронирования</NavLink>}
      </nav>
      <div className="header-actions">{isAuthenticated ? <>
        <span className="signed-in-label">Вы вошли</span><button className="button button-quiet button-small" onClick={logout}>Выйти</button>
      </> : <>
        <Link className="header-login" to="/login">Войти</Link><Link className="button button-primary button-small" to="/register">Регистрация</Link>
      </>}</div>
    </div></header>
    <div className="site-content"><Outlet /></div>
    <footer className="site-footer"><div className="footer-inner">
      <Link className="brand brand-footer" to="/events"><span className="brand-mark">TBS</span><span className="brand-name">TicketBookingSystem</span></Link>
      <span>© 2026 TicketBookingSystem</span>
    </div></footer>
  </div>
}
