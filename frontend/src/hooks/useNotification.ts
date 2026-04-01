/**
 * Custom hook para notificações toast
 */

import { useCallback } from 'react';
import toast from 'react-hot-toast';

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
