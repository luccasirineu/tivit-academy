import { useNavigate } from "react-router-dom";
import type { MateriaCardProps } from "@/types/components";

/**
 * Componente que exibe um card com informações de uma matéria
 * @param materia - Dados da matéria
 * @param onSelect - Callback opcional quando a matéria é selecionada
 */
export default function MateriaCard({ materia, onSelect }: MateriaCardProps) {
  const navigate = useNavigate();

  function handleClick() {
    onSelect?.(materia);
    navigate(`/aluno/materias/${materia.id}`);
  }

  return (
    <div className="materia" onClick={handleClick} role="button" tabIndex={0}>
      <i className="fas fa-book"></i>
      <h3>{materia.nome}</h3>
    </div>
  );
}