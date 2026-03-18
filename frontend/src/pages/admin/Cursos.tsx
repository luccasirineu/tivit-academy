import { useEffect, useState } from "react";
import api from "../../services/api";
import { Modal } from "../../components/common/Modal";
import type { CursoAdmin, ProfessorAdmin } from "../../types";

export default function Cursos() {
  const [cursos, setCursos] = useState<CursoAdmin[]>([]);
  const [professores, setProfessores] = useState<ProfessorAdmin[]>([]);
  const [profNomes, setProfNomes] = useState<Record<number, string>>({});
  const [qtdAlunos, setQtdAlunos] = useState<Record<number, number>>({});
  const [modal, setModal] = useState({ open: false, titulo: "", mensagem: "" });
  const [showNovo, setShowNovo] = useState(false);
  const [showEditar, setShowEditar] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [confirmMsg, setConfirmMsg] = useState("");
  const [confirmAcao, setConfirmAcao] = useState<() => void>(() => {});
  const [cursoEditando, setCursoEditando] = useState<CursoAdmin | null>(null);
  const [novoNome, setNovoNome] = useState("");
  const [novoDesc, setNovoDesc] = useState("");
  const [novoProf, setNovoProf] = useState("");
  const [editNome, setEditNome] = useState("");
  const [editDesc, setEditDesc] = useState("");
  const [editProf, setEditProf] = useState("");

  useEffect(() => { carregar(); }, []);

  async function carregar() {
    try {
      const res = await api.get("/Curso");
      const lista: CursoAdmin[] = res.data;
      setCursos(lista);
      const nomes: Record<number, string> = {};
      const qtd: Record<number, number> = {};
      await Promise.all(lista.map(async c => {
        try { const r = await api.get(`/Professor/getProfessorById/${c.profResponsavel}`); nomes[c.profResponsavel] = r.data.nome; } catch { nomes[c.profResponsavel] = "—"; }
        try { const r = await api.get(`/Curso/getQntdAlunosByCursoId/${c.id}`); qtd[c.id] = r.data; } catch { qtd[c.id] = 0; }
      }));
      setProfNomes(nomes);
      setQtdAlunos(qtd);
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Erro ao carregar cursos." }); }
  }

  async function carregarProfessores() {
    const res = await api.get("/Professor/getAllProfessoresAtivos");
    setProfessores(res.data);
  }

  async function salvarNovo() {
    if (!novoNome || !novoProf) return;
    try {
      await api.post("/Curso/criarCurso", { nome: novoNome, descricao: novoDesc, profResponsavel: Number(novoProf) });
      setShowNovo(false); setNovoNome(""); setNovoDesc(""); setNovoProf("");
      carregar();
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Erro ao criar curso." }); }
  }

  async function salvarEdicao() {
    if (!cursoEditando || !editNome || !editProf) return;
    try {
      await api.put("/Curso/atualizarCurso", { id: cursoEditando.id, nome: editNome, descricao: editDesc, profResponsavel: Number(editProf) });
      setShowEditar(false);
      setModal({ open: true, titulo: "Sucesso", mensagem: "Curso atualizado!" });
      carregar();
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Erro ao atualizar curso." }); }
  }

  function confirmar(msg: string, acao: () => void) {
    setConfirmMsg(msg); setConfirmAcao(() => acao); setShowConfirm(true);
  }

  async function toggleStatus(curso: CursoAdmin) {
    const url = curso.status === "ATIVO" ? `/Curso/desativarCurso/${curso.id}` : `/Curso/ativarCurso/${curso.id}`;
    try {
      await api.put(url);
      setModal({ open: true, titulo: "Sucesso", mensagem: `Curso ${curso.status === "ATIVO" ? "desativado" : "ativado"}!` });
      carregar();
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Erro ao alterar status." }); }
  }

  return (
    <section className="prof-section">
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", width: "100%", marginBottom: "24px" }}>
        <h2><i className="bx bx-book"></i> Gerenciar Cursos</h2>
        <button className="btn-accent" onClick={() => { setShowNovo(true); carregarProfessores(); }}>
          <i className="bx bx-plus-circle"></i> Novo Curso
        </button>
      </div>

      <div className="cursos-grid" style={{ width: "100%" }}>
        {cursos.map(c => (
          <div key={c.id} className="cursos-card">
            <h3>{c.nome}</h3>
            <p><i className="bx bx-user"></i> Professor: {profNomes[c.profResponsavel] ?? "—"}</p>
            <p><i className="bx bx-group"></i> Alunos: {qtdAlunos[c.id] ?? 0}</p>
            <p><i className="bx bx-radio-circle"></i> Status: {c.status}</p>
            <div className="card-actions">
              <button className="btn-edit" onClick={() => {
                setCursoEditando(c); setEditNome(c.nome); setEditDesc(c.descricao);
                carregarProfessores().then(() => setEditProf(String(c.profResponsavel)));
                setShowEditar(true);
              }}><i className="bx bx-edit"></i></button>
              <button className="btn-toggle-status" onClick={() => confirmar(
                c.status === "ATIVO" ? "Deseja desativar este curso?" : "Deseja ativar este curso?",
                () => { toggleStatus(c); setShowConfirm(false); }
              )}>
                <i className={`bx ${c.status === "ATIVO" ? "bx-x-circle" : "bx-check-circle"}`}></i>
              </button>
            </div>
          </div>
        ))}
      </div>

      {/* Popup Novo Curso */}
      {showNovo && (
        <div className="popup-overlay" onClick={() => setShowNovo(false)}>
          <div className="popup-content" onClick={e => e.stopPropagation()}>
            <h3><i className="bx bx-plus-circle"></i> Novo Curso</h3>
            <label>Nome do Curso</label>
            <input type="text" value={novoNome} onChange={e => setNovoNome(e.target.value)} placeholder="Ex: Desenvolvimento Web" />
            <label>Descrição</label>
            <input type="text" value={novoDesc} onChange={e => setNovoDesc(e.target.value)} />
            <label>Professor Responsável</label>
            <select value={novoProf} onChange={e => setNovoProf(e.target.value)}>
              <option value="">Selecione um professor</option>
              {professores.map(p => <option key={p.id} value={p.id}>{p.nome}</option>)}
            </select>
            <div className="popup-actions">
              <button className="btn-cancelar" onClick={() => setShowNovo(false)}>Cancelar</button>
              <button className="btn-accent" onClick={salvarNovo}>Salvar</button>
            </div>
          </div>
        </div>
      )}

      {/* Popup Editar Curso */}
      {showEditar && (
        <div className="popup-overlay" onClick={() => setShowEditar(false)}>
          <div className="popup-content" onClick={e => e.stopPropagation()}>
            <h3><i className="bx bx-edit"></i> Editar Curso</h3>
            <label>Nome</label>
            <input type="text" value={editNome} onChange={e => setEditNome(e.target.value)} />
            <label>Descrição</label>
            <input type="text" value={editDesc} onChange={e => setEditDesc(e.target.value)} />
            <label>Professor</label>
            <select value={editProf} onChange={e => setEditProf(e.target.value)}>
              {professores.map(p => <option key={p.id} value={p.id}>{p.nome}</option>)}
            </select>
            <div className="popup-actions">
              <button className="btn-cancelar" onClick={() => setShowEditar(false)}>Cancelar</button>
              <button className="btn-accent" onClick={salvarEdicao}>Salvar</button>
            </div>
          </div>
        </div>
      )}

      {/* Popup Confirmação */}
      {showConfirm && (
        <div className="popup-overlay" onClick={() => setShowConfirm(false)}>
          <div className="popup-content" onClick={e => e.stopPropagation()}>
            <h3>Confirmação</h3>
            <p>{confirmMsg}</p>
            <div className="popup-actions">
              <button className="btn-cancelar" onClick={() => setShowConfirm(false)}>Cancelar</button>
              <button className="btn-accent" onClick={confirmAcao}>OK</button>
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