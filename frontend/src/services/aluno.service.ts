import api from "./api";
import type { 
  Evento, 
  Materia, 
  DesempenhoMateria, 
  NotaAluno, 
  MateriaNome, 
  Notificacao 
} from "../types/index";

export const fetchProximoEvento = async () => {
  const { data } = await api.get<Evento>("/Evento/proximoEvento");
  return data;
};

export const fetchAllEventos = async () => {
  const { data } = await api.get<Evento[]>("/Evento/getAllEvents");
  return data;
};

export const fetchMateriasAluno = async (cursoId: number) => {
  const { data } = await api.get<Materia[]>(`/Materia/getMateriasByCursoId/${cursoId}`);
  return data;
};

export const fetchDesempenhoAluno = async (alunoId: number) => {
  const { data } = await api.get<DesempenhoMateria[]>(
    `/Nota/aluno/${alunoId}/getDesempenho`
  );
  return data;
};

export const fetchAllNotas = async (alunoId: number) => {
  const { data } = await api.get<NotaAluno[]>(`/Nota/aluno/${alunoId}/getAllNotas`);
  return data;
};

export const fetchNomeMateria = async (materiaId: number) => {
  const { data } = await api.get<MateriaNome>(`/Materia/getNomeMateria/${materiaId}`);
  return data;
};

export const fetchConteudosMateria = async (materiaId: string | number, turmaId: number) => {
  const { data } = await api.get(`/Conteudo/getAllConteudos/${materiaId}/${turmaId}`);
  return data;
};

export const fetchNotificacoesByTurma = async (turmaId: number) => {
  const { data } = await api.get<Notificacao[]>(`/Notificacao/getNotificacoesByTurmaId/${turmaId}`);
  return data;
};
