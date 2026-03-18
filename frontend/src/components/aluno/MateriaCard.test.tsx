import { render, screen } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { vi, describe, it, expect } from 'vitest';
import MateriaCard from '@/components/aluno/MateriaCard';
import type { Materia } from '@/types';

describe('MateriaCard', () => {
  const mockMateria: Materia = {
    id: 1,
    nome: 'Matemática',
    descricao: 'Aula de matemática básica',
  };

  const renderComponent = (materia: Materia = mockMateria, onSelect?: (m: Materia) => void) => {
    return render(
      <BrowserRouter>
        <MateriaCard materia={materia} onSelect={onSelect} />
      </BrowserRouter>
    );
  };

  it('deve renderizar o nome da matéria', () => {
    renderComponent();
    expect(screen.getByText('Matemática')).toBeInTheDocument();
  });

  it('deve renderizar o ícone', () => {
    renderComponent();
    const icon = screen.getByRole('button').querySelector('i');
    expect(icon).toHaveClass('fas', 'fa-book');
  });

  it('deve ter atributo role button', () => {
    renderComponent();
    expect(screen.getByRole('button')).toBeInTheDocument();
  });
});
