import { Navigate, Outlet, useLocation } from 'react-router'
import { useAuth } from '../auth/AuthContext'

export default function ProtectedRoute() {
  const { isAuthenticated } = useAuth()
  const location = useLocation()
  if (!isAuthenticated) {
    const returnTo = location.pathname + location.search
    return <Navigate to={'/login?returnTo=' + encodeURIComponent(returnTo)} replace />
  }
  return <Outlet />
}
