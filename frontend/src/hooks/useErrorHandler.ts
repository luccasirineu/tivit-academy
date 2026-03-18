/**
 * Custom hook para tratamento de erros
 */

import { useCallback } from 'react';
import toast from 'react-hot-toast';

export function useErrorHandler() {
  const handleError = useCallback((error: Error | unknown, customMessage?: string) => {
    const message = customMessage || 
      (error instanceof Error ? error.message : 'Erro desconhecido');
    
    toast.error(message);
    console.error('[Error]', error);
  }, []);

  return { handleError };
}

/**
 * Hook para validar e exibir notificações
 */
export function useNotification() {
  const success = useCallback((message: string) => {
    toast.success(message);
  }, []);

  const error = useCallback((message: string) => {
    toast.error(message);
  }, []);

  const loading = useCallback((message: string) => {
    return toast.loading(message);
  }, []);

  const dismiss = useCallback((toastId: string) => {
    toast.dismiss(toastId);
  }, []);

  return { success, error, loading, dismiss };
}
