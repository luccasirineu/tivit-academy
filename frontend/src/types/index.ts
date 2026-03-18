/* =============================
   TIPAGENS GERAIS
============================= */
export interface Course {
  id: number;
  nome: string;
  descricao: string;
}

export interface EnrollmentData {
  nome: string;
  email: string;
  cpf: string;
  cursoId: number;
}

export interface EnrollmentResponse {
  matriculaId: number;
}

export interface LoginCredentials {
  Tipo: string;
  Cpf: string;
  Senha: string;
}

export interface LoginResponse {
  id: number;
  nome: string;
  tipo: "aluno" | "professor" | "administrador";
  token: string;
  cursosIds: number[]; 
  turmaId: number;
}

export interface Evento {
  id: number;
  titulo: string;
  descricao: string;
  horario: string;
}

export interface Materia {
  id: number;
  nome: string;
  descricao: string;
}

export interface DesempenhoMateria {
  nomeMateria: string;
  media: number;
  qtdFaltas: number;
  nivel: "OURO" | "PRATA" | "BRONZE" | string;
}

export interface NotaAluno {
  alunoId: number;
  materiaId: number;
  nota1: number;
  nota2: number;
  media: number;
  qtdFaltas: number;
  status: string;
}

export interface MateriaNome {
  materiaNome: string;
}

/* =============================
   TIPAGENS - ADMIN
============================= */
export interface AlunoAdmin {
  alunoId: number;
  nome: string;
  email: string;
  cpf: string;
  matriculaId: number;
  turmaId: number;
  cursoNome: string;
  turmaNome: string;
}

export interface TurmaAdmin {
  id: number;
  nome: string;
  cursoId: number;
  status: string;
}

export interface CursoAdmin {
  id: number;
  nome: string;
  descricao: string;
  profResponsavel: number;
  status: string;
}

export interface ProfessorAdmin {
  id: number;
  nome: string;
  email: string;
  rm: string;
  cpf: string;
  status: string;
}

export interface MatriculaPendente {
  id: string;
  nome: string;
  email: string;
  cpf: string;
  cursoId: number;
}

export interface UsuarioAdmin {
  nome: string;
  email: string;
  cpf: string;
  tipo: string;
  status: string;
}

export interface NotificacaoPayload {
  titulo: string;
  descricao: string;
  turmasIds: number[];
}

export interface Notificacao {
  titulo: string;
  descricao: string;
  dataCriacao: string;
}
/* =============================
   TIPAGENS - ERROS E UTILITÁRIOS
============================= */
export interface ApiError {
  status: number;
  message: string;
  data?: unknown;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
}