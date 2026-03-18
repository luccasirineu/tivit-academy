/**
 * Tipos de Props dos componentes Aluno
 */

import type { Materia, DesempenhoMateria, Evento } from './index';

export interface MateriaCardProps {
  materia: Materia;
  onSelect?: (materia: Materia) => void;
}

export interface CalendarioGridProps {
  eventos: Evento[];
  onEventClick?: (evento: Evento) => void;
}

export interface GraficoEvolucaoProps {
  desempenho: DesempenhoMateria[];
  className?: string;
}

export interface EventoModalProps {
  isOpen: boolean;
  evento: Evento | null;
  onClose: () => void;
}

export interface SidebarAlunoProps {
  collapsed: boolean;
  toggleSidebar: () => void;
  lightTheme: boolean;
  toggleTheme: () => void;
}
