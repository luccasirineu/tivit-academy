import axios, { AxiosError } from "axios";
import { PUBLIC_ROUTES, API_BASE_URL, API_TIMEOUT } from "@/constants/api";
import type { ApiError } from "@/types";

/* =====================================
   CONFIGURAÇÃO BASE DO AXIOS
===================================== */
const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: API_TIMEOUT,
  headers: {
    "Content-Type": "application/json",
  },
});

/* =====================================
   INTERCEPTOR DE TOKEN AUTOMÁTICO
===================================== */
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

/* =====================================
   INTERCEPTOR DE ERRO GLOBAL
===================================== */
api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    const requestUrl = error.config?.url || "";
    const isPublicRoute = PUBLIC_ROUTES.some(route => 
      requestUrl.includes(route)
    );

    // Tratamento de erro 401 (Unauthorized)
    if (error.response?.status === 401 && !isPublicRoute) {
      localStorage.removeItem("token");
      localStorage.removeItem("usuarioLogado");
      window.location.href = "/login";
    }

    // Criar objeto de erro estruturado
    const apiError: ApiError = {
      status: error.response?.status || 500,
      message: error.response?.statusText || error.message || "Erro na requisição",
      data: error.response?.data,
    };

    return Promise.reject(apiError);
  }
);

export default api;
