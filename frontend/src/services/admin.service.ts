import api from "./api";
import type { 
  AlunoAdmin, 
  TurmaAdmin, 
  CursoAdmin, 
  ProfessorAdmin, 
  MatriculaPendente, 
  UsuarioAdmin, 
  NotificacaoPayload 
} from "../types/index";

export const fetchTodosAlunos = async () => {
  const { data } = await api.get<AlunoAdmin[]>("/Aluno/getAllAlunos");
  return data;
};

export const atualizarTurmaAluno = async (alunoId: number, turmaId: number) => {
  const { data } = await api.patch(`/Aluno/${alunoId}/turma`, { turmaId });
  return data;
};

export const fetchTodasTurmas = async () => {
  const { data } = await api.get<TurmaAdmin[]>("/Turma/getAllTurmas");
  return data;
};

export const fetchTurmasPorCurso = async (cursoId: number) => {
  const { data } = await api.get<TurmaAdmin[]>(`/Turma/getTurmasByCursoId/${cursoId}`);
  return data;
};

export const fetchTodosCursos = async () => {
  const { data } = await api.get<CursoAdmin[]>("/Curso");
  return data;
};

export const fetchTodosProfessores = async () => {
  const { data } = await api.get<ProfessorAdmin[]>("/Professor/getAllProfessores");
  return data;
};

export const fetchMatriculasPendentes = async () => {
  const { data } = await api.get<MatriculaPendente[]>("/Matricula/getAllMatriculasPendentes");
  return data;
};

export const aprovarMatricula = async (id: string) => {
  const { data } = await api.post(`/Matricula/aprovar/${id}`);
  return data;
};

export const recusarMatricula = async (id: string) => {
  const { data } = await api.post(`/Matricula/recusar/${id}`);
  return data;
};

export const buscarUsuarioPorCpf = async (cpf: string) => {
  const { data } = await api.get<UsuarioAdmin>(`/User/getUserByCpf/?cpf=${cpf}`);
  return Array.isArray(data) ? data : [data];
};

export const buscarUsuariosPorNome = async (nome: string) => {
  const { data } = await api.get<UsuarioAdmin[]>(`/User/getUsersByNome/?nome=${encodeURIComponent(nome)}`);
  return Array.isArray(data) ? data : [data];
};

export const desativarUsuario = async (cpf: string, tipo: string) => {
  const { data } = await api.put(`/User/desativar?cpf=${cpf}&tipo=${tipo}`);
  return data;
};

export const ativarUsuario = async (cpf: string, tipo: string) => {
  const { data } = await api.put(`/User/ativar?cpf=${cpf}&tipo=${tipo}`);
  return data;
};

export const criarNotificacao = async (payload: NotificacaoPayload) => {
  const { data } = await api.post("/Notificacao/criarNotificacao", payload);
  return data;
};

export const getQtdAlunosAtivos = async () => {
  const { data } = await api.get<number>("/Aluno/getQntdAlunosAtivos");
  return data;
};

export const getQtdProfessoresAtivos = async () => {
  const { data } = await api.get<number>("/Professor/getQntdProfessoresAtivos");
  return data;
};

export const getQtdTurmasAtivas = async () => {
  const { data } = await api.get<number>("/Turma/getQntdTurmasAtivas");
  return data;
};
