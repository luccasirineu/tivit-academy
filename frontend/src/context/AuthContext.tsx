import React, { createContext, useState, useContext, useEffect, ReactNode } from 'react';
import { login as apiLogin } from '../services/auth.service';
import type { LoginResponse } from '../types/index';

interface AuthContextType {
  user: LoginResponse | null;
  token: string | null;
  isLoading: boolean;
  login: (credentials: any) => Promise<void>;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<LoginResponse | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check for saved user data in localStorage on initial load
    try {
      const savedUser = localStorage.getItem('usuarioLogado');
      const savedToken = localStorage.getItem('token');
      if (savedUser && savedToken) {
        setUser(JSON.parse(savedUser));
        setToken(savedToken);
      }
    } catch (error) {
      console.error("Failed to parse auth data from localStorage", error);
    } finally {
      setIsLoading(false);
    }
  }, []);

  const login = async (credentials: any) => {
    const data = await apiLogin(credentials);
    console.log(data);
    localStorage.setItem('usuarioLogado', JSON.stringify({
      id: data.id,
      nome: data.nome,
      tipo: data.tipo,
      cursosIds: data.cursosIds,
      turmaId : data.turmaId
    }));
    localStorage.setItem('token', data.token);
    setUser({
      id: data.id,
      nome: data.nome,
      tipo: data.tipo,
      token: data.token,
      cursosIds: data.cursosIds, 
      turmaId: data.turmaId,
    });
    setToken(data.token);
  };

  const logout = () => {
    localStorage.removeItem('usuarioLogado');
    localStorage.removeItem('token');
    setUser(null);
    setToken(null);
  };

  const value = {
    user,
    token,
    isLoading,
    login,
    logout,
  };

  return (
    <AuthContext.Provider value={value}>
      {!isLoading && children}
    </AuthContext.Provider>
  );
}
