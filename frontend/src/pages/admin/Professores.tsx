import { useEffect, useState } from "react";
import api from "../../services/api";
import { Modal } from "../../components/common/Modal";
import { fetchTodosProfessores } from "../../services/admin.service";
import type { ProfessorAdmin } from "../../types";

export default function Professores() {
  const [professores, setProfessores] = useState<ProfessorAdmin[]>([]);
  const [showNovo, setShowNovo] = useState(false);
  const [modal, setModal] = useState({ open: false, titulo: "", mensagem: "" });
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");
  const [cpf, setCpf] = useState("");
  const [status, setStatus] = useState("ATIVO");

  useEffect(() => { carregar(); }, []);

  async function carregar() {
    try {
      const data = await fetchTodosProfessores();
      setProfessores(data);
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Erro ao carregar professores." }); }
  }

  async function salvar() {
    if (!nome || !email || !cpf) return;
    try {
      await api.post("/Professor/criarProfessor", { nome, email, cpf, status });
      setShowNovo(false); setNome(""); setEmail(""); setCpf(""); setStatus("ATIVO");
      setModal({ open: true, titulo: "Sucesso", mensagem: "Professor criado com sucesso!" });
      carregar();
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Erro ao criar professor." }); }
  }

  return (
    <section className="prof-section">
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", width: "100%", marginBottom: "24px" }}>
        <h2><i className="bx bx-clipboard"></i> Gerenciamento de Professores</h2>
        <button className="btn-accent" onClick={() => setShowNovo(true)}>
          <i className="bx bx-plus-circle"></i> Novo Professor
        </button>
      </div>

      <div className="professores-grid" style={{ width: "100%" }}>
        {professores.map(p => (
          <div key={p.id} className="professor-card">
            <h3><i className="bx bx-user"></i> {p.nome}</h3>
            <p><i className="bx bx-envelope"></i> {p.email ?? "—"}</p>
            <p><i className="bx bx-registered"></i> RM: {p.rm}</p>
            <p><i className="bx bx-face"></i> CPF: {p.cpf}</p>
          </div>
        ))}
      </div>

      {showNovo && (
        <div className="popup-overlay" onClick={() => setShowNovo(false)}>
          <div className="popup-content" onClick={e => e.stopPropagation()}>
            <h3><i className="bx bx-user-plus"></i> Novo Professor</h3>
            <label>Nome</label>
            <input type="text" value={nome} onChange={e => setNome(e.target.value)} />
            <label>Email</label>
            <input type="email" value={email} onChange={e => setEmail(e.target.value)} />
            <label>CPF</label>
            <input type="text" value={cpf} onChange={e => setCpf(e.target.value)} />
            <label>Status</label>
            <select value={status} onChange={e => setStatus(e.target.value)}>
              <option value="ATIVO">Ativo</option>
              <option value="INATIVO">Inativo</option>
            </select>
            <div className="popup-actions">
              <button className="btn-cancelar" onClick={() => setShowNovo(false)}>Cancelar</button>
              <button className="btn-accent" onClick={salvar}>Salvar</button>
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