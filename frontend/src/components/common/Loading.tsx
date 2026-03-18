/**
 * Componente de Loading
 */

interface LoadingProps {
  message?: string;
  fullHeight?: boolean;
}

export function Loading({ message = 'Carregando...', fullHeight = false }: LoadingProps) {
  const className = fullHeight ? 'loading-container fullHeight' : 'loading-container';

  return (
    <div className={className}>
      <div className="spinner"></div>
      <p>{message}</p>
    </div>
  );
}

export default Loading;
