import { renderHook } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { useErrorHandler, useNotification } from '@/hooks/useErrorHandler';

// Mock do react-hot-toast
vi.mock('react-hot-toast', () => ({
  default: {
    error: vi.fn(),
    success: vi.fn(),
    loading: vi.fn(),
    dismiss: vi.fn(),
  },
}));

describe('useErrorHandler', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('deve chamar handleError', () => {
    const { result } = renderHook(() => useErrorHandler());
    const error = new Error('Test error');
    
    expect(() => {
      result.current.handleError(error);
    }).not.toThrow();
  });
});

describe('useNotification', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('deve ter métodos de notificação', () => {
    const { result } = renderHook(() => useNotification());
    
    expect(result.current.success).toBeDefined();
    expect(result.current.error).toBeDefined();
    expect(result.current.loading).toBeDefined();
    expect(result.current.dismiss).toBeDefined();
  });
});
