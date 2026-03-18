import { Routes, Route, Navigate } from "react-router-dom";
import { useContext } from "react";
import { AuthContext } from "./context/AuthContext";

import LoginPage from "./pages/LoginPage";
import MatriculaPage from "./pages/MatriculaPage";

import AlunoLayout from "./layout/AlunoLayout";
import DashboardAluno from "./pages/aluno/DashboardAluno";
import Calendario from "./pages/aluno/Calendario";
import Desempenho from "./pages/aluno/Desempenho";
import Materias from "./pages/aluno/Materias";
import Relatorio from "./pages/aluno/Relatorio";
import MateriaDetalhes from "./pages/aluno/MateriaDetalhes";
import NotificacoesAluno from "./pages/aluno/NotificacoesAluno";

import ProfessorLayout from "./layout/ProfessorLayout";
import DashboardProfessor from "./pages/professor/DashboardProfessor";
import CalendarioProfessor from "./pages/professor/CalendarioProfessor";
import Chamada from "./pages/professor/Chamada";
import Boletins from "./pages/professor/Boletins";
import Conteudo from "./pages/professor/Conteudo";
import Notas from "./pages/professor/Notas";

import AdminLayout from "./layout/AdminLayout";
import DashboardAdmin from "./pages/admin/DashboardAdmin";
import Solicitacoes from "./pages/admin/Solicitacoes";
import Usuarios from "./pages/admin/Usuarios";
import Cursos from "./pages/admin/Cursos";
import Turmas from "./pages/admin/Turmas";
import Professores from "./pages/admin/Professores";
import Notificacoes from "./pages/admin/Notificacoes";
import Alunos from "./pages/admin/Alunos";

function PrivateRoute({ children }: { children: JSX.Element }) {
  const { user, isLoading } = useContext(AuthContext);
  if (isLoading) return null;
  return user ? children : <Navigate to="/login" replace />;
}

function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/matricula" replace />} />

      <Route path="/login" element={<LoginPage />} />
      <Route path="/matricula" element={<MatriculaPage />} />

      {/* Aluno */}
      <Route path="/aluno" element={<PrivateRoute><AlunoLayout /></PrivateRoute>}>
        <Route index element={<DashboardAluno />} />
        <Route path="dashboard" element={<DashboardAluno />} />
        <Route path="calendario" element={<Calendario />} />
        <Route path="desempenho" element={<Desempenho />} />
        <Route path="materias" element={<Materias />} />
        <Route path="materias/:materiaId" element={<MateriaDetalhes />} />
        <Route path="relatorio" element={<Relatorio />} />
        <Route path="notificacoes" element={<NotificacoesAluno />} />

      </Route>

      {/* Professor */}
      <Route path="/professor" element={<PrivateRoute><ProfessorLayout /></PrivateRoute>}>
        <Route index element={<DashboardProfessor />} />
        <Route path="dashboardProfessor" element={<DashboardProfessor />} />
        <Route path="calendario" element={<CalendarioProfessor />} />
        <Route path="boletim" element={<Boletins />} />
        <Route path="chamada" element={<Chamada />} />
        <Route path="conteudo" element={<Conteudo />} />
        <Route path="notas" element={<Notas />} />
      </Route>

      {/* Admin */}
      <Route path="/admin" element={<PrivateRoute><AdminLayout /></PrivateRoute>}>
        <Route index element={<DashboardAdmin />} />
        <Route path="dashboard" element={<DashboardAdmin />} />
        <Route path="solicitacoes" element={<Solicitacoes />} />
        <Route path="usuarios" element={<Usuarios />} />
        <Route path="cursos" element={<Cursos />} />
        <Route path="turmas" element={<Turmas />} />
        <Route path="professores" element={<Professores />} />
        <Route path="notificacoes" element={<Notificacoes />} />
        <Route path="alunos" element={<Alunos />} />

      </Route>

      <Route path="*" element={<Navigate to="/matricula" replace />} />
    </Routes>
  );
}

export default App;