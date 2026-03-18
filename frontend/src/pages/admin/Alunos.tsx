import { useEffect, useState } from "react";
import type { AlunoAdmin, TurmaAdmin } from "../../types/index";
import { Modal } from "../../components/common/Modal";
import { fetchTodosAlunos, fetchTurmasPorCurso, atualizarTurmaAluno, fetchTodosCursos } from "../../services/admin.service";

export default function Alunos() {
  const [alunos, setAlunos] = useState<AlunoAdmin[]>([]);
  const [turmasDisponiveis, setTurmasDisponiveis] = useState<TurmaAdmin[]>([]);
  const [loading, setLoading] = useState(true);
  const [modal, setModal] = useState({ open: false, titulo: "", mensagem: "" });
  const [showEditar, setShowEditar] = useState(false);
  const [alunoEditando, setAlunoEditando] = useState<AlunoAdmin | null>(null);
  const [turmaSelecionada, setTurmaSelecionada] = useState("");

  useEffect(() => { carregar(); }, []);

  async function carregar() {
  try {
    setLoading(true);
    const alunosData = await fetchTodosAlunos();
    setAlunos(alunosData);
  } catch {
    setModal({ open: true, titulo: "Erro", mensagem: "Erro ao carregar alunos." });
  } finally {
    setLoading(false);
  }
}

  async function abrirEditar(aluno: AlunoAdmin) {
  setAlunoEditando(aluno);
  setTurmaSelecionada(aluno.turmaId ? String(aluno.turmaId) : "");
  setTurmasDisponiveis([]);
  try {
    // busca o cursoId pelo nome do curso via lista de cursos
    const cursosData = await fetchTodosCursos();
    const curso = cursosData.find(c => c.nome === aluno.cursoNome);
    if (curso) {
      const turmasData = await fetchTurmasPorCurso(curso.id);
      setTurmasDisponiveis(turmasData.filter(t => t.status === "ATIVO"));
    }
  } catch {
    setModal({ open: true, titulo: "Erro", mensagem: "Erro ao carregar turmas do curso." });
  }
  setShowEditar(true);
}

  async function salvarTurma() {
    if (!alunoEditando || !turmaSelecionada) return;
    try {
      await atualizarTurmaAluno(alunoEditando.alunoId, Number(turmaSelecionada));
      setShowEditar(false);
      setModal({ open: true, titulo: "Sucesso", mensagem: "Turma atualizada com sucesso!" });
      carregar();
    } catch {
      setModal({ open: true, titulo: "Erro", mensagem: "Erro ao atualizar turma do aluno." });
    }
  }

  return (
    <section className="prof-section">
      <h2><i className="fas fa-user-graduate"></i> Alunos</h2>

      {loading
        ? <p style={{ opacity: 0.6 }}>Carregando alunos...</p>
        : (
          <div className="alunos-admin-grid">
            {alunos.map(a => (
              <div key={a.alunoId} className="aluno-admin-card">
                <div className="aluno-admin-header">
                  <i className="bx bx-user-circle" style={{ fontSize: "32px", color: "var(--accent)" }}></i>
                  <div>
                    <h3>{a.nome}</h3>
                    <span className="aluno-matricula">Matrícula: {a.matriculaId}</span>
                  </div>
                  <button className="btn-edit-turma" title="Alterar turma" onClick={() => abrirEditar(a)}>
                    <i className="bx bx-transfer-alt"></i>
                  </button>
                </div>
                <div className="aluno-admin-info">
                  <p><i className="bx bx-envelope"></i> {a.email}</p>
                  <p><i className="bx bx-id-card"></i> CPF: {a.cpf}</p>
                  <p><i className="bx bx-book"></i> Curso: {a.cursoNome || "—"}</p>
                  <p><i className="bx bx-group"></i> Turma: {a.turmaNome || "Sem turma"}</p>
                </div>
              </div>
            ))}
          </div>
        )
      }

      {showEditar && alunoEditando && (
        <div className="popup-overlay" onClick={() => setShowEditar(false)}>
          <div className="popup-content" onClick={e => e.stopPropagation()}>
            <h3><i className="bx bx-transfer-alt"></i> Alterar Turma</h3>
            <div className="aluno-editar-info">
              <p><strong>{alunoEditando.nome}</strong></p>
              <p style={{ opacity: 0.6, fontSize: "13px" }}>Turma atual: {alunoEditando.turmaNome || "Sem turma"}</p>
            </div>
            <label>Nova Turma</label>
            <select value={turmaSelecionada} onChange={e => setTurmaSelecionada(e.target.value)}>
              <option value="">-- Selecione uma turma --</option>
              {turmasDisponiveis.map(t => <option key={t.id} value={t.id}>{t.nome}</option>)}
            </select>
            <div className="popup-actions">
              <button className="btn-cancelar" onClick={() => setShowEditar(false)}>Cancelar</button>
              <button className="btn-accent" onClick={salvarTurma} disabled={!turmaSelecionada}>
                <i className="bx bx-save"></i> Salvar
              </button>
            </div>
          </div>
        </div>
      )}

      <Modal isOpen={modal.open} onClose={() => setModal({ open: false, titulo: "", mensagem: "" })} title={modal.titulo}>
        <p>{modal.mensagem}</p>
        <button className="btn-primary" style={{ width: "100%", marginTop: "12px" }} onClick={() => setModal({ open: false, titulo: "", mensagem: "" })}>OK</button>
      </Modal>
    </section>
  );
}