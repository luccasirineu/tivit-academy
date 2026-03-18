import { useEffect, useState } from "react";
import api from "../../services/api";
import { Modal } from "../../components/common/Modal";
import { fetchTodasTurmas, fetchTodosCursos } from "../../services/admin.service";
import type { TurmaAdmin, CursoAdmin } from "../../types";

export default function Turmas() {
  const [turmas, setTurmas] = useState<TurmaAdmin[]>([]);
  const [cursos, setCursos] = useState<CursoAdmin[]>([]);
  const [cursoNomes, setCursoNomes] = useState<Record<number, string>>({});
  const [modal, setModal] = useState({ open: false, titulo: "", mensagem: "" });
  const [showNova, setShowNova] = useState(false);
  const [showEditar, setShowEditar] = useState(false);
  const [turmaEditando, setTurmaEditando] = useState<TurmaAdmin | null>(null);
  const [novoNome, setNovoNome] = useState("");
  const [novoCursoId, setNovoCursoId] = useState("");
  const [novoStatus, setNovoStatus] = useState("ATIVO");
  const [editNome, setEditNome] = useState("");
  const [editCursoId, setEditCursoId] = useState("");
  const [editStatus, setEditStatus] = useState("ATIVO");

  useEffect(() => { carregar(); }, []);

  async function carregar() {
    try {
      const [turmasData, cursosData] = await Promise.all([
        fetchTodasTurmas(),
        fetchTodosCursos(),
      ]);
      setTurmas(turmasData);
      setCursos(cursosData);
      const nomes: Record<number, string> = {};
      cursosData.forEach((c) => { nomes[c.id] = c.nome; });
      setCursoNomes(nomes);
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Erro ao carregar turmas." }); }
  }

  async function salvarNova() {
    if (!novoNome || !novoCursoId) return;
    try {
      await api.post("/Turma/criarTurma", { nome: novoNome, cursoId: Number(novoCursoId), status: novoStatus });
      setShowNova(false); setNovoNome(""); setNovoCursoId(""); setNovoStatus("ATIVO");
      setModal({ open: true, titulo: "Sucesso", mensagem: "Turma criada com sucesso!" });
      carregar();
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Erro ao criar turma." }); }
  }

  async function salvarEdicao() {
    if (!turmaEditando) return;
    try {
      await api.put("/Turma/atualizarTurma", { id: turmaEditando.id, nome: editNome, cursoId: Number(editCursoId), status: editStatus });
      setShowEditar(false);
      setModal({ open: true, titulo: "Sucesso", mensagem: "Turma atualizada!" });
      carregar();
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Erro ao atualizar turma." }); }
  }

  return (
    <section className="prof-section">
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", width: "100%", marginBottom: "24px" }}>
        <h2><i className="bx bx-group"></i> Gerenciamento de Turmas</h2>
        <button className="btn-accent" onClick={() => setShowNova(true)}>
          <i className="bx bx-plus-circle"></i> Nova Turma
        </button>
      </div>

      <div className="cursos-grid" style={{ width: "100%" }}>
        {turmas.map(t => (
          <div key={t.id} className="turmas-card">
            <div className="turma-info">
              <h3>Turma: {t.nome}</h3>
              <p><i className="bx bx-book"></i> Curso: {cursoNomes[t.cursoId] ?? "—"}</p>
            </div>
            <div className={`turma-status ${t.status.toLowerCase()}`}>{t.status}</div>
            <button className="btn-edit" onClick={() => {
              setTurmaEditando(t); setEditNome(t.nome);
              setEditCursoId(String(t.cursoId)); setEditStatus(t.status);
              setShowEditar(true);
            }}><i className="bx bx-edit-alt"></i></button>
          </div>
        ))}
      </div>

      {showNova && (
        <div className="popup-overlay" onClick={() => setShowNova(false)}>
          <div className="popup-content" onClick={e => e.stopPropagation()}>
            <h3><i className="bx bx-plus-circle"></i> Nova Turma</h3>
            <label>Nome da Turma</label>
            <input type="text" value={novoNome} onChange={e => setNovoNome(e.target.value)} placeholder="Ex: Turma FullStack 2025" />
            <label>Curso Vinculado</label>
            <select value={novoCursoId} onChange={e => setNovoCursoId(e.target.value)}>
              <option value="">Selecione um curso</option>
              {cursos.filter(c => c.status === "ATIVO").map(c => <option key={c.id} value={c.id}>{c.nome}</option>)}
            </select>
            <label>Status</label>
            <select value={novoStatus} onChange={e => setNovoStatus(e.target.value)}>
              <option value="ATIVO">ATIVA</option>
              <option value="INATIVO">DESATIVADA</option>
            </select>
            <div className="popup-actions">
              <button className="btn-cancelar" onClick={() => setShowNova(false)}>Cancelar</button>
              <button className="btn-accent" onClick={salvarNova}>Salvar</button>
            </div>
          </div>
        </div>
      )}

      {showEditar && (
        <div className="popup-overlay" onClick={() => setShowEditar(false)}>
          <div className="popup-content" onClick={e => e.stopPropagation()}>
            <h3><i className="bx bx-edit"></i> Editar Turma</h3>
            <label>Nome</label>
            <input type="text" value={editNome} onChange={e => setEditNome(e.target.value)} />
            <label>Curso</label>
            <select value={editCursoId} onChange={e => setEditCursoId(e.target.value)}>
              {cursos.map(c => <option key={c.id} value={c.id}>{c.nome}</option>)}
            </select>
            <label>Status</label>
            <select value={editStatus} onChange={e => setEditStatus(e.target.value)}>
              <option value="ATIVO">ATIVA</option>
              <option value="INATIVO">DESATIVADA</option>
            </select>
            <div className="popup-actions">
              <button className="btn-cancelar" onClick={() => setShowEditar(false)}>Cancelar</button>
              <button className="btn-accent" onClick={salvarEdicao}>Salvar</button>
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