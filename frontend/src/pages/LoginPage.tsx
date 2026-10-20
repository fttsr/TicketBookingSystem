import { useState, type FormEvent } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router'
import { authApi } from '../api/client'
import { useAuth } from '../auth/AuthContext'

function safeReturnTo(value: string | null) { return value && value.startsWith('/') && !value.startsWith('//') ? value : '/events' }
export default function LoginPage() {
  const [params] = useSearchParams()
  const navigate = useNavigate()
  const { signIn } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault(); setError(''); setBusy(true)
    try { signIn(await authApi.login({ email, password })); navigate(safeReturnTo(params.get('returnTo')), { replace: true }) }
    catch (err) { setError(err instanceof Error ? err.message : 'Не удалось войти.') }
    finally { setBusy(false) }
  }
  const returnTo = params.get('returnTo')
  const registerUrl = '/register' + (returnTo ? '?returnTo=' + encodeURIComponent(returnTo) : '')
  return <main className="auth-page page-shell"><div className="auth-layout">
    <section className="auth-card">
      <Link className="back-link auth-back" to="/events"><span>←</span> К афише</Link>
      <span className="eyebrow eyebrow-muted">РАДЫ ВАС ВИДЕТЬ</span><h1>С возвращением</h1><p className="auth-intro">Войдите в аккаунт, чтобы управлять бронированиями.</p>
      {error && <div className="form-alert" role="alert">{error}</div>}
      <form className="form-stack" onSubmit={submit}>
        <label className="field"><span>Email</span><input type="email" autoComplete="email" placeholder="you@example.com" value={email} onChange={(e) => setEmail(e.target.value)} required maxLength={256} /></label>
        <label className="field"><span>Пароль</span><input type="password" autoComplete="current-password" placeholder="Не короче 8 символов" value={password} onChange={(e) => setPassword(e.target.value)} required minLength={8} maxLength={128} /></label>
        <button className="button button-primary button-wide" disabled={busy}>{busy ? 'Входим…' : 'Войти в аккаунт'} <span>→</span></button>
      </form>
      <p className="auth-switch">Впервые здесь? <Link to={registerUrl}>Создать аккаунт</Link></p>
    </section>
    <aside className="auth-art auth-art-login"><span className="auth-art-orbit auth-orbit-a" /><span className="auth-art-orbit auth-orbit-b" /><span className="auth-art-kicker"></span><div className="auth-art-copy">Билеты на<br /><em>мероприятия</em><br />в одном месте.</div><span className="auth-art-spark">✳</span><span className="auth-art-caption"></span></aside>
  </div></main>
}
