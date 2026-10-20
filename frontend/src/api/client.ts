export type EventItem = {
  id: string
  title: string
  description: string
  date: string
  venue: string
  price: number
  availableTickets: number
}
export type BookingItem = {
  id: string
  userId: string
  eventId: string
  createdAt: string
  status: number | string
}
export type Credentials = { email: string; password: string }

const TOKEN_KEY = 'ticket-booking-token'
const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL ?? '').replace(/\/+$/, '')
export const tokenStorage = {
  get: () => window.sessionStorage.getItem(TOKEN_KEY),
  set: (token: string) => window.sessionStorage.setItem(TOKEN_KEY, token),
  clear: () => window.sessionStorage.removeItem(TOKEN_KEY),
}

function responseMessage(body: string, status: number) {
  if (!body) return 'Ошибка запроса (' + status + ')'
  try {
    const data = JSON.parse(body) as { detail?: string; title?: string; message?: string; errors?: Record<string, string[]> }
    const validation = data.errors ? Object.values(data.errors).flat()[0] : undefined
    return data.detail || data.message || validation || data.title || body
  } catch {
    return body
  }
}

export async function apiRequest<T>(path: string, init: RequestInit = {}): Promise<T> {
  const headers = new Headers(init.headers)
  const token = tokenStorage.get()
  if (token) headers.set('Authorization', 'Bearer ' + token)
  if (init.body && !headers.has('Content-Type')) headers.set('Content-Type', 'application/json')
  let response: Response
  try {
    response = await fetch(API_BASE_URL + path, { ...init, headers })
  } catch {
    throw new Error('Не удалось подключиться к API. Проверь, запущен ли backend.')
  }
  const body = await response.text()
  if (!response.ok) throw new Error(responseMessage(body, response.status))
  if (response.status === 204 || !body) return undefined as T
  return JSON.parse(body) as T
}

export const eventsApi = {
  getAll: () => apiRequest<EventItem[]>('/api/events'),
  getById: (id: string) => apiRequest<EventItem>('/api/events/' + id),
}
type AuthResponse = { token?: string; Token?: string }
async function authenticate(path: string, credentials: Credentials) {
  const result = await apiRequest<AuthResponse>(path, { method: 'POST', body: JSON.stringify(credentials) })
  const token = result.token || result.Token
  if (!token) throw new Error('Сервис авторизации не вернул токен.')
  return token
}
export const authApi = {
  login: (credentials: Credentials) => authenticate('/api/auth/login', credentials),
  register: (credentials: Credentials) => authenticate('/api/auth/register', credentials),
}
export const bookingsApi = {
  getMine: () => apiRequest<BookingItem[]>('/api/bookings'),
  create: (eventId: string) => apiRequest<BookingItem>('/api/bookings', {
    method: 'POST', body: JSON.stringify({ eventId }),
  }),
  cancel: (id: string) => apiRequest<BookingItem>('/api/bookings/' + id + '/cancel', { method: 'POST' }),
}
export function getBookingStatus(status: BookingItem['status']) {
  const value = String(status).toLowerCase()
  if (status === 1 || value === 'confirmed') return { key: 'confirmed' as const, label: 'Подтверждено' }
  if (status === 2 || value === 'cancelled' || value === 'canceled') return { key: 'cancelled' as const, label: 'Отменено' }
  return { key: 'pending' as const, label: 'В обработке' }
}
export function formatPrice(value: number) {
  return new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB', maximumFractionDigits: 0 }).format(value)
}
export function formatDateTime(value: string) {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return new Intl.DateTimeFormat('ru-RU', { day: 'numeric', month: 'long', year: 'numeric', hour: '2-digit', minute: '2-digit' }).format(date)
}
export function formatDate(value: string) {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return new Intl.DateTimeFormat('ru-RU', { weekday: 'short', day: 'numeric', month: 'short' }).format(date)
}
