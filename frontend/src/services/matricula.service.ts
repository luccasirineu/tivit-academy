import api from "./api";
import type { EnrollmentData, EnrollmentResponse } from "../types/index";

export const startEnrollment = async (payload: EnrollmentData) => {
  const { data } = await api.post<EnrollmentResponse>("/Matricula", payload);
  return data;
};

export const uploadPayment = async (matriculaId: number, file: File) => {
  const formData = new FormData();
  formData.append("arquivo", file);
  const { data } = await api.post(
    `/Matricula/${matriculaId}/pagamento`,
    formData,
    { headers: { "Content-Type": "multipart/form-data" } }
  );
  return data;
};

export const uploadDocuments = async (
  matriculaId: number,
  historico: File,
  cpf: File
) => {
  const formData = new FormData();
  formData.append("documentoHistorico", historico);
  formData.append("documentoCpf", cpf);
  const { data } = await api.post(
    `/Matricula/${matriculaId}/documentos`,
    formData,
    { headers: { "Content-Type": "multipart/form-data" } }
  );
  return data;
};
