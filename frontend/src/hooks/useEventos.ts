/**
 * Custom hooks para eventos
 */

import { useQuery } from '@tanstack/react-query';
import { fetchProximoEvento, fetchAllEventos } from '@/services/aluno.service';
import type { Evento } from '@/types';

/**
 * Hook para carregar o próximo evento
 * @returns Dados do próximo evento, loading e error states
 */
export function useProximoEvento() {
  return useQuery({
    queryKey: ['proximoEvento'],
    queryFn: fetchProximoEvento,
  });
}

/**
 * Hook para carregar todos os eventos
 * @returns Dados de eventos, loading e error states
 */
export function useTodosEventos() {
  return useQuery({
    queryKey: ['todosEventos'],
    queryFn: fetchAllEventos,
  });
}
