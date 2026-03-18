/**
 * Configurações e constantes da API
 */

export const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5027/api';

export const PUBLIC_ROUTES = [
  '/Curso/GetAllCursosAtivos',
  '/Matricula',
  '/Login',
];

export const API_TIMEOUT = 30000; // 30 segundos

export const QUERY_CONFIG = {
  queries: {
    staleTime: 5 * 60 * 1000, // 5 minutos
    gcTime: 10 * 60 * 1000, // 10 minutos (antes: cacheTime)
    retry: 1,
    retryDelay: 1000,
  },
};

export const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  SERVER_ERROR: 500,
} as const;
