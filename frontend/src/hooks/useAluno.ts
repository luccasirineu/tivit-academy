/**
 * Custom hooks para dados de aluno
 */

import { useQuery } from '@tanstack/react-query';
import { fetchMateriasAluno, fetchDesempenhoAluno, fetchAllNotas } from '@/services/aluno.service';
import type { Materia, DesempenhoMateria, NotaAluno } from '@/types';

/**
 * Hook para carregar as matérias de um aluno
 * @param cursoId - ID do curso
 * @returns Dados de matérias, loading e error states
 */
export function useMateriasAluno(cursoId: number | undefined) {
  return useQuery({
    queryKey: ['materias', cursoId],
    queryFn: () => {
      if (!cursoId) throw new Error('Course ID is required');
      return fetchMateriasAluno(cursoId);
    },
    enabled: Boolean(cursoId),
  });
}

/**
 * Hook para carregar o desempenho de um aluno
 * @param alunoId - ID do aluno
 * @returns Dados de desempenho, loading e error states
 */
export function useDesempenhoAluno(alunoId: number | undefined) {
  return useQuery({
    queryKey: ['desempenho', alunoId],
    queryFn: () => {
      if (!alunoId) throw new Error('Aluno ID is required');
      return fetchDesempenhoAluno(alunoId);
    },
    enabled: Boolean(alunoId),
  });
}

/**
 * Hook para carregar todas as notas de um aluno
 * @param alunoId - ID do aluno
 * @returns Dados de notas, loading e error states
 */
export function useNotasAluno(alunoId: number | undefined) {
  return useQuery({
    queryKey: ['notas', alunoId],
    queryFn: () => {
      if (!alunoId) throw new Error('Aluno ID is required');
      return fetchAllNotas(alunoId);
    },
    enabled: Boolean(alunoId),
  });
}
