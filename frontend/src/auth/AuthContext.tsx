import { createContext, useContext, useMemo, useState, type ReactNode } from 'react'
import { tokenStorage } from '../api/client'

type AuthValue = { isAuthenticated: boolean; signIn: (token: string) => void; signOut: () => void }
const AuthContext = createContext<AuthValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState(() => tokenStorage.get())
  const value = useMemo<AuthValue>(() => ({
    isAuthenticated: Boolean(token),
    signIn: (nextToken) => { tokenStorage.set(nextToken); setToken(nextToken) },
    signOut: () => { tokenStorage.clear(); setToken(null) },
  }), [token])
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
export function useAuth() {
  const value = useContext(AuthContext)
  if (!value) throw new Error('useAuth должен использоваться внутри AuthProvider')
  return value
}
