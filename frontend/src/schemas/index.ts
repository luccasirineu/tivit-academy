/**
 * Schemas de validação com Zod
 */

import { z } from 'zod';

/* Autenticação */
export const LoginCredentialsSchema = z.object({
  Tipo: z.enum(['aluno', 'professor', 'administrador']),
  Cpf: z.string().min(11),
  Senha: z.string().min(1),
});

export const LoginResponseSchema = z.object({
  id: z.number(),
  nome: z.string(),
  tipo: z.enum(['aluno', 'professor', 'administrador']),
  token: z.string(),
  cursosIds: z.array(z.number()),
  turmaId: z.number(),
});

/* Cursos e Matrículas */
export const CourseSchema = z.object({
  id: z.number(),
  nome: z.string().min(1),
  descricao: z.string(),
});

export const EnrollmentDataSchema = z.object({
  nome: z.string().min(1),
  email: z.string().email(),
  cpf: z.string().min(11),
  cursoId: z.number(),
});

/* Matérias */
export const MateriaSchema = z.object({
  id: z.number(),
  nome: z.string().min(1),
  descricao: z.string(),
});

/* Eventos */
export const EventoSchema = z.object({
  id: z.number(),
  titulo: z.string().min(1),
  descricao: z.string(),
  horario: z.string(),
});

/* Desempenho */
export const DesempenhoMateriaSchema = z.object({
  nomeMateria: z.string(),
  media: z.number().min(0).max(10),
  qtdFaltas: z.number().min(0),
  nivel: z.enum(['OURO', 'PRATA', 'BRONZE']).or(z.string()),
});

/* Tipos derivados dos schemas */
export type LoginCredentials = z.infer<typeof LoginCredentialsSchema>;
export type LoginResponse = z.infer<typeof LoginResponseSchema>;
export type Course = z.infer<typeof CourseSchema>;
export type EnrollmentData = z.infer<typeof EnrollmentDataSchema>;
export type Materia = z.infer<typeof MateriaSchema>;
export type Evento = z.infer<typeof EventoSchema>;
export type DesempenhoMateria = z.infer<typeof DesempenhoMateriaSchema>;
