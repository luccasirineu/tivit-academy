/**
 * Componente de Empty State
 */

interface EmptyStateProps {
  title?: string;
  message?: string;
  icon?: string;
  action?: {
    label: string;
    onClick: () => void;
  };
}

export function EmptyState({
  title = 'Nenhum resultado encontrado',
  message = 'Não há dados para exibir no momento',
  icon = 'fas fa-inbox',
  action,
}: EmptyStateProps) {
  return (
    <div className="empty-state">
      <i className={icon}></i>
      <h3>{title}</h3>
      <p>{message}</p>
      {action && (
        <button onClick={action.onClick} className="btn-action">
          {action.label}
        </button>
      )}
    </div>
  );
}

export default EmptyState;
