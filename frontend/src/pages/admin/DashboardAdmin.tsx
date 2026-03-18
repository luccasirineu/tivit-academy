import { useEffect, useState } from "react";
import { getQtdAlunosAtivos, getQtdProfessoresAtivos, getQtdTurmasAtivas } from "../../services/admin.service";

export default function DashboardAdmin() {
  const [qtdAlunos, setQtdAlunos]         = useState(0);
  const [qtdProfessores, setQtdProfessores] = useState(0);
  const [qtdTurmas, setQtdTurmas]         = useState(0);

  useEffect(() => {
    Promise.all([
      getQtdAlunosAtivos().then(setQtdAlunos),
      getQtdProfessoresAtivos().then(setQtdProfessores),
      getQtdTurmasAtivas().then(setQtdTurmas),
    ]).catch(console.error);
  }, []);

  return (
    <section className="prof-section">
      <div className="dashboard-header">
        <h1>Bem-vindo, Administrador!</h1>
        <p>Gerencie as informações e usuários da <strong>TIVIT Academy</strong>.</p>
      </div>
      <div className="resumo-grid">
        <div className="resumo-card">
          <i className="bx bx-user"></i>
          <div><h3>{qtdAlunos}</h3><p>Alunos Ativos</p></div>
        </div>
        <div className="resumo-card">
          <i className="bx bx-book"></i>
          <div><h3>{qtdProfessores}</h3><p>Professores Ativos</p></div>
        </div>
        <div className="resumo-card">
          <i className="bx bx-group"></i>
          <div><h3>{qtdTurmas}</h3><p>Turmas</p></div>
        </div>
      </div>
    </section>
  );
}