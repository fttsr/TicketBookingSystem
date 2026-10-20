import { useState, type FormEvent } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router'
import { authApi } from '../api/client'
import { useAuth } from '../auth/AuthContext'

function safeReturnTo(value: string | null) { return value && value.startsWith('/') && !value.startsWith('//') ? value : '/events' }
export default function RegisterPage() {
  const [params] = useSearchParams()
  const navigate = useNavigate()
  const { signIn } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirm, setConfirm] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault(); setError('')
    if (password !== confirm) { setError('Пароли не совпадают.'); return }
    setBusy(true)
    try { signIn(await authApi.register({ email, password })); navigate(safeReturnTo(params.get('returnTo')), { replace: true }) }
    catch (err) { setError(err instanceof Error ? err.message : 'Не удалось создать аккаунт.') }
    finally { setBusy(false) }
  }
  const returnTo = params.get('returnTo')
  const loginUrl = '/login' + (returnTo ? '?returnTo=' + encodeURIComponent(returnTo) : '')
  return <main className="auth-page page-shell"><div className="auth-layout">
    <section className="auth-card">
      <Link className="back-link auth-back" to="/events"><span>←</span> К афише</Link>
      <span className="eyebrow eyebrow-muted"></span><h1>Создать аккаунт</h1><p className="auth-intro">Создайте аккаунт, чтобы бронировать билеты и управлять ими.</p>
      {error && <div className="form-alert" role="alert">{error}</div>}
      <form className="form-stack" onSubmit={submit}>
        <label className="field"><span>Email</span><input type="email" autoComplete="email" placeholder="you@example.com" value={email} onChange={(e) => setEmail(e.target.value)} required maxLength={256} /></label>
        <label className="field"><span>Пароль</span><input type="password" autoComplete="new-password" placeholder="Не короче 8 символов" value={password} onChange={(e) => setPassword(e.target.value)} required minLength={8} maxLength={128} /></label>
        <label className="field"><span>Повторите пароль</span><input type="password" autoComplete="new-password" placeholder="Введите пароль ещё раз" value={confirm} onChange={(e) => setConfirm(e.target.value)} required minLength={8} maxLength={128} /></label>
        <button className="button button-primary button-wide" disabled={busy}>{busy ? 'Создаём аккаунт…' : 'Зарегистрироваться'} <span>→</span></button>
      </form>
      <p className="auth-switch">Уже есть аккаунт? <Link to={loginUrl}>Войти</Link></p>
    </section>
    <aside className="auth-art auth-art-register"><span className="auth-art-orbit auth-orbit-a" /><span className="auth-art-orbit auth-orbit-b" /><span className="auth-art-kicker"></span><div className="auth-art-copy">Создайте аккаунт<br />и бронируйте<br /><em>билеты онлайн.</em></div><span className="auth-art-spark">✦</span><span className="auth-art-caption"></span></aside>
  </div></main>
}
