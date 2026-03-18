import { describe, it, expect } from 'vitest';
import {
  LoginCredentialsSchema,
  LoginResponseSchema,
  CourseSchema,
  MateriaSchema,
  DesempenhoMateriaSchema,
} from '@/schemas';

describe('Schemas de Validação', () => {
  describe('LoginCredentialsSchema', () => {
    it('deve validar credenciais corretas', () => {
      const validCredentials = {
        Tipo: 'aluno' as const,
        Cpf: '12345678901',
        Senha: 'password123',
      };

      const result = LoginCredentialsSchema.safeParse(validCredentials);
      expect(result.success).toBe(true);
    });

    it('deve rejeitar tipo inválido', () => {
      const invalidCredentials = {
        Tipo: 'invalid-type',
        Cpf: '12345678901',
        Senha: 'password123',
      };

      const result = LoginCredentialsSchema.safeParse(invalidCredentials);
      expect(result.success).toBe(false);
    });
  });

  describe('CourseSchema', () => {
    it('deve validar curso correto', () => {
      const validCourse = {
        id: 1,
        nome: 'Curso de React',
        descricao: 'Um ótimo curso',
      };

      const result = CourseSchema.safeParse(validCourse);
      expect(result.success).toBe(true);
    });

    it('deve rejeitar curso sem nome', () => {
      const invalidCourse = {
        id: 1,
        nome: '',
        descricao: 'Um ótimo curso',
      };

      const result = CourseSchema.safeParse(invalidCourse);
      expect(result.success).toBe(false);
    });
  });

  describe('MateriaSchema', () => {
    it('deve validar matéria correta', () => {
      const validMateria = {
        id: 1,
        nome: 'Matemática',
        descricao: 'Aula de mate',
      };

      const result = MateriaSchema.safeParse(validMateria);
      expect(result.success).toBe(true);
    });
  });

  describe('DesempenhoMateriaSchema', () => {
    it('deve validar desempenho correto', () => {
      const validDesempenho = {
        nomeMateria: 'Matemática',
        media: 7.5,
        qtdFaltas: 2,
        nivel: 'OURO' as const,
      };

      const result = DesempenhoMateriaSchema.safeParse(validDesempenho);
      expect(result.success).toBe(true);
    });

    it('deve rejeitar média acima de 10', () => {
      const invalidDesempenho = {
        nomeMateria: 'Matemática',
        media: 11,
        qtdFaltas: 2,
        nivel: 'OURO' as const,
      };

      const result = DesempenhoMateriaSchema.safeParse(invalidDesempenho);
      expect(result.success).toBe(false);
    });
  });
});
