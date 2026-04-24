-- =============================================
-- Script de População Inicial do Banco TivitDB
-- =============================================
-- Senhas padrão para todos os usuários: "Senha@123"
-- Hash BCrypt (workFactor 12): $2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe

USE TivitDB;
GO

-- =============================================
-- 1. ADMINISTRADORES
-- =============================================
SET IDENTITY_INSERT Administradores ON;

INSERT INTO Administradores (Id, Nome, Email, Cpf, Senha, Status)
VALUES 
    (1, 'Admin Sistema', 'admin@tivit.com', '11122233344', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 'ATIVO'),
    (2, 'Maria Silva', 'maria.silva@tivit.com', '22233344455', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 'ATIVO'),
    (3, 'João Santos', 'joao.santos@tivit.com', '33344455566', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 'ATIVO');

SET IDENTITY_INSERT Administradores OFF;
GO

-- =============================================
-- 2. PROFESSORES
-- =============================================
SET IDENTITY_INSERT Professores ON;

INSERT INTO Professores (Id, Nome, Email, Senha, Rm, Cpf, Status)
VALUES 
    (1, 'Prof. Carlos Eduardo', 'carlos.eduardo@tivit.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 'RM001', '44455566677', 'ATIVO'),
    (2, 'Prof. Ana Paula', 'ana.paula@tivit.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 'RM002', '55566677788', 'ATIVO'),
    (3, 'Prof. Roberto Lima', 'roberto.lima@tivit.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 'RM003', '66677788899', 'ATIVO'),
    (4, 'Prof. Fernanda Costa', 'fernanda.costa@tivit.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 'RM004', '77788899900', 'ATIVO'),
    (5, 'Prof. Ricardo Alves', 'ricardo.alves@tivit.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 'RM005', '88899900011', 'ATIVO');

SET IDENTITY_INSERT Professores OFF;
GO

-- =============================================
-- 3. CURSOS
-- =============================================
SET IDENTITY_INSERT Cursos ON;

INSERT INTO Cursos (Id, Nome, Descricao, ProfResponsavel, Status)
VALUES 
    (1, 'Desenvolvimento Full Stack', 'Curso completo de desenvolvimento web com foco em tecnologias modernas como React, Node.js e bancos de dados relacionais.', 1, 'ATIVO'),
    (2, 'Ciência de Dados', 'Aprenda análise de dados, machine learning e visualização de dados com Python, Pandas e bibliotecas de ML.', 2, 'ATIVO'),
    (3, 'DevOps e Cloud Computing', 'Domine práticas DevOps, CI/CD, Docker, Kubernetes e serviços de nuvem AWS/Azure.', 3, 'ATIVO'),
    (4, 'Segurança da Informação', 'Curso focado em segurança cibernética, ethical hacking, criptografia e proteção de sistemas.', 4, 'ATIVO'),
    (5, 'Mobile Development', 'Desenvolvimento de aplicativos móveis nativos e híbridos para Android e iOS.', 5, 'ATIVO');

SET IDENTITY_INSERT Cursos OFF;
GO

-- =============================================
-- 4. MATÉRIAS - Desenvolvimento Full Stack
-- =============================================
SET IDENTITY_INSERT Materias ON;

-- Matérias do Curso 1: Desenvolvimento Full Stack
INSERT INTO Materias (Id, Nome, Descricao, CursoId)
VALUES 
    (1, 'HTML e CSS Fundamentals', 'Fundamentos de HTML5 e CSS3, layouts responsivos e boas práticas de desenvolvimento front-end.', 1),
    (2, 'JavaScript Avançado', 'JavaScript ES6+, programação assíncrona, promises, async/await e manipulação do DOM.', 1),
    (3, 'React.js', 'Desenvolvimento de interfaces modernas com React, hooks, context API e gerenciamento de estado.', 1),
    (4, 'Node.js e Express', 'Desenvolvimento de APIs RESTful com Node.js, Express, autenticação e autorização.', 1),
    (5, 'Banco de Dados SQL', 'Modelagem de dados, SQL Server, queries avançadas e otimização de performance.', 1);

-- Matérias do Curso 2: Ciência de Dados
INSERT INTO Materias (Id, Nome, Descricao, CursoId)
VALUES 
    (6, 'Python para Análise de Dados', 'Fundamentos de Python, NumPy, Pandas e manipulação de datasets.', 2),
    (7, 'Estatística Aplicada', 'Estatística descritiva e inferencial aplicada à análise de dados.', 2),
    (8, 'Machine Learning', 'Algoritmos de aprendizado supervisionado e não supervisionado com Scikit-learn.', 2),
    (9, 'Visualização de Dados', 'Criação de dashboards e visualizações com Matplotlib, Seaborn e Plotly.', 2),
    (10, 'Big Data e Spark', 'Processamento de grandes volumes de dados com Apache Spark e PySpark.', 2);

-- Matérias do Curso 3: DevOps e Cloud Computing
INSERT INTO Materias (Id, Nome, Descricao, CursoId)
VALUES 
    (11, 'Linux e Shell Scripting', 'Administração de sistemas Linux e automação com shell scripts.', 3),
    (12, 'Docker e Containers', 'Containerização de aplicações, Docker Compose e boas práticas.', 3),
    (13, 'Kubernetes', 'Orquestração de containers, deployments, services e escalabilidade.', 3),
    (14, 'CI/CD com GitHub Actions', 'Integração e entrega contínua, pipelines automatizados e testes.', 3),
    (15, 'AWS Cloud Services', 'Serviços AWS: EC2, S3, RDS, Lambda e arquitetura serverless.', 3);

-- Matérias do Curso 4: Segurança da Informação
INSERT INTO Materias (Id, Nome, Descricao, CursoId)
VALUES 
    (16, 'Fundamentos de Segurança', 'Conceitos básicos de segurança da informação, ameaças e vulnerabilidades.', 4),
    (17, 'Ethical Hacking', 'Técnicas de penetration testing e identificação de vulnerabilidades.', 4),
    (18, 'Criptografia', 'Algoritmos de criptografia simétrica e assimétrica, certificados digitais.', 4),
    (19, 'Segurança em Aplicações Web', 'OWASP Top 10, prevenção de ataques XSS, SQL Injection e CSRF.', 4),
    (20, 'Gestão de Riscos', 'Análise e gestão de riscos de segurança em ambientes corporativos.', 4);

-- Matérias do Curso 5: Mobile Development
INSERT INTO Materias (Id, Nome, Descricao, CursoId)
VALUES 
    (21, 'Fundamentos Mobile', 'Conceitos de desenvolvimento mobile, UX/UI para dispositivos móveis.', 5),
    (22, 'React Native', 'Desenvolvimento cross-platform com React Native e Expo.', 5),
    (23, 'Android Nativo', 'Desenvolvimento Android com Kotlin, Jetpack Compose e arquitetura MVVM.', 5),
    (24, 'iOS Nativo', 'Desenvolvimento iOS com Swift, SwiftUI e padrões de design.', 5),
    (25, 'APIs e Integração', 'Consumo de APIs REST, autenticação OAuth e sincronização de dados.', 5);

SET IDENTITY_INSERT Materias OFF;
GO

-- =============================================
-- 5. TURMAS
-- =============================================
SET IDENTITY_INSERT Turmas ON;

INSERT INTO Turmas (Id, Nome, CursoId, Status)
VALUES 
    (1, 'Full Stack - Turma 2026.1', 1, 'ATIVA'),
    (2, 'Full Stack - Turma 2026.2', 1, 'ATIVA'),
    (3, 'Ciência de Dados - Turma 2026.1', 2, 'ATIVA'),
    (4, 'DevOps - Turma 2026.1', 3, 'ATIVA'),
    (5, 'Segurança - Turma 2026.1', 4, 'ATIVA'),
    (6, 'Mobile - Turma 2026.1', 5, 'ATIVA');

SET IDENTITY_INSERT Turmas OFF;
GO

-- =============================================
-- 6. MATRÍCULAS (Enrollment Requests)
-- =============================================
SET IDENTITY_INSERT Matriculas ON;

INSERT INTO Matriculas (Id, Nome, Email, Cpf, CursoId, Status)
VALUES 
    -- Matrículas para Full Stack
    (1, 'Lucas Oliveira', 'lucas.oliveira@aluno.tivit.com', '12345678901', 1, 'APROVADO'),
    (2, 'Juliana Souza', 'juliana.souza@aluno.tivit.com', '23456789012', 1, 'APROVADO'),
    (3, 'Pedro Henrique', 'pedro.henrique@aluno.tivit.com', '34567890123', 1, 'APROVADO'),
    
    -- Matrículas para Ciência de Dados
    (4, 'Camila Santos', 'camila.santos@aluno.tivit.com', '45678901234', 2, 'APROVADO'),
    (5, 'Rafael Costa', 'rafael.costa@aluno.tivit.com', '56789012345', 2, 'APROVADO'),
    
    -- Matrículas para DevOps
    (6, 'Beatriz Lima', 'beatriz.lima@aluno.tivit.com', '67890123456', 3, 'APROVADO'),
    (7, 'Gabriel Alves', 'gabriel.alves@aluno.tivit.com', '78901234567', 3, 'APROVADO'),
    
    -- Matrículas para Segurança
    (8, 'Amanda Silva', 'amanda.silva@aluno.tivit.com', '89012345678', 4, 'APROVADO'),
    
    -- Matrículas para Mobile
    (9, 'Thiago Martins', 'thiago.martins@aluno.tivit.com', '90123456789', 5, 'APROVADO'),
    (10, 'Larissa Rocha', 'larissa.rocha@aluno.tivit.com', '01234567890', 5, 'APROVADO');

SET IDENTITY_INSERT Matriculas OFF;
GO

-- =============================================
-- 7. ALUNOS (Students linked to Matriculas and Turmas)
-- =============================================
SET IDENTITY_INSERT Alunos ON;

INSERT INTO Alunos (Id, Nome, Email, Cpf, Senha, MatriculaId, TurmaId, Status)
VALUES 
    (1, 'Lucas Oliveira', 'lucas.oliveira@aluno.tivit.com', '12345678901', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 1, 1, 'ATIVO'),
    (2, 'Juliana Souza', 'juliana.souza@aluno.tivit.com', '23456789012', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 2, 1, 'ATIVO'),
    (3, 'Pedro Henrique', 'pedro.henrique@aluno.tivit.com', '34567890123', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 3, 2, 'ATIVO'),
    (4, 'Camila Santos', 'camila.santos@aluno.tivit.com', '45678901234', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 4, 3, 'ATIVO'),
    (5, 'Rafael Costa', 'rafael.costa@aluno.tivit.com', '56789012345', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 5, 3, 'ATIVO'),
    (6, 'Beatriz Lima', 'beatriz.lima@aluno.tivit.com', '67890123456', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 6, 4, 'ATIVO'),
    (7, 'Gabriel Alves', 'gabriel.alves@aluno.tivit.com', '78901234567', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 7, 4, 'ATIVO'),
    (8, 'Amanda Silva', 'amanda.silva@aluno.tivit.com', '89012345678', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 8, 5, 'ATIVO'),
    (9, 'Thiago Martins', 'thiago.martins@aluno.tivit.com', '90123456789', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 9, 6, 'ATIVO'),
    (10, 'Larissa Rocha', 'larissa.rocha@aluno.tivit.com', '01234567890', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr9qP8fKe', 10, 6, 'ATIVO');

SET IDENTITY_INSERT Alunos OFF;
GO

PRINT '✓ Dados iniciais inseridos com sucesso!';
PRINT '';
PRINT '==============================================';
PRINT 'CREDENCIAIS DE ACESSO';
PRINT '==============================================';
PRINT 'Senha padrão para todos os usuários: Senha@123';
PRINT '';
PRINT 'ADMINISTRADORES:';
PRINT '  - admin@tivit.com';
PRINT '  - maria.silva@tivit.com';
PRINT '  - joao.santos@tivit.com';
PRINT '';
PRINT 'PROFESSORES:';
PRINT '  - carlos.eduardo@tivit.com (Full Stack)';
PRINT '  - ana.paula@tivit.com (Ciência de Dados)';
PRINT '  - roberto.lima@tivit.com (DevOps)';
PRINT '  - fernanda.costa@tivit.com (Segurança)';
PRINT '  - ricardo.alves@tivit.com (Mobile)';
PRINT '';
PRINT 'ALUNOS:';
PRINT '  - lucas.oliveira@aluno.tivit.com';
PRINT '  - juliana.souza@aluno.tivit.com';
PRINT '  - ... (e outros 8 alunos)';
PRINT '';
PRINT 'Total: 3 Admins, 5 Professores, 5 Cursos, 25 Matérias, 6 Turmas, 10 Matrículas, 10 Alunos';
PRINT '==============================================';
GO
