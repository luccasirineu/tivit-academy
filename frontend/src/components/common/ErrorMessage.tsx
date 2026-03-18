/**
 * Componente de Erro
 */

interface ErrorMessageProps {
  message?: string;
  onRetry?: () => void;
  fullHeight?: boolean;
}

export function ErrorMessage({
  message = 'Ocorreu um erro ao carregar os dados',
  onRetry,
  fullHeight = false,
}: ErrorMessageProps) {
  const className = fullHeight ? 'error-container fullHeight' : 'error-container';

  return (
    <div className={className}>
      <i className="fas fa-exclamation-circle"></i>
      <p>{message}</p>
      {onRetry && (
        <button onClick={onRetry} className="btn-retry">
          <i className="fas fa-redo"></i>
          Tentar novamente
        </button>
      )}
    </div>
  );
}

export default ErrorMessage;
