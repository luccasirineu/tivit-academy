import api from "./api";
import type { LoginCredentials, LoginResponse } from "../types/index";

export const login = async (credentials: LoginCredentials) => {
  const { data } = await api.post<LoginResponse>("/Login", credentials);
  localStorage.setItem("token", data.token);
  return data;
};

export const logout = () => {
  localStorage.removeItem("token");
  window.location.href = "/login";
};
