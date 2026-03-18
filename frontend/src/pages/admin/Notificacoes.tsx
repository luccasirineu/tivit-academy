import { useEffect, useState } from "react";
import { Modal } from "../../components/common/Modal";
import { criarNotificacao, fetchTodasTurmas, fetchTodosCursos } from "../../services/admin.service"; 
import type { TurmaAdmin, CursoAdmin } from "../../types/index";

export default function Notificacoes() {
  const [turmas, setTurmas] = useState<TurmaAdmin[]>([]);
  const [cursoNomes, setCursoNomes] = useState<Record<number, string>>({});
  const [turmasSelecionadas, setTurmasSelecionadas] = useState<number[]>([]);
  const [titulo, setTitulo] = useState("");
  const [descricao, setDescricao] = useState("");
  const [modal, setModal] = useState({ open: false, titulo: "", mensagem: "" });

  useEffect(() => { carregar(); }, []);

  async function carregar() {
    try {
      const [turmasData, cursosData] = await Promise.all([
        fetchTodasTurmas(),
        fetchTodosCursos(),
      ]);
      const ativas = turmasData.filter((t) => t.status === "ATIVO");
      setTurmas(ativas);
      const nomes: Record<number, string> = {};
      cursosData.forEach((c) => { nomes[c.id] = c.nome; });
      setCursoNomes(nomes);
    } catch { console.error("Erro ao carregar turmas"); }
  }

  function toggleTurma(id: number) {
    setTurmasSelecionadas(prev =>
      prev.includes(id) ? prev.filter(t => t !== id) : [...prev, id]
    );
  }

  async function publicar() {
    if (!titulo || !descricao) {
      setModal({ open: true, titulo: "Atenção", mensagem: "Preencha título e descrição." });
      return;
    }
    if (!turmasSelecionadas.length) {
      setModal({ open: true, titulo: "Atenção", mensagem: "Selecione pelo menos uma turma." });
      return;
    }

    try {
      await criarNotificacao({ titulo, descricao, turmasIds: turmasSelecionadas });
      setModal({ open: true, titulo: "Sucesso", mensagem: "Notificação criada com sucesso!" });
      setTitulo("");
      setDescricao("");
      setTurmasSelecionadas([]);
    } catch (err: any) {
      const mensagem = err.response?.data?.message || "Erro ao enviar notificação.";
      setModal({ open: true, titulo: "Erro", mensagem });
    }
  }

  return (
    <section className="prof-section">
      <h2><i className="bx bx-bell"></i> Notificações</h2>

      <div className="notificacao-container" style={{ width: "100%" }}>
        <div className="notificacao-form">
          <input
            type="text"
            placeholder="Título da notificação"
            value={titulo}
            onChange={e => setTitulo(e.target.value)}
          />
          <textarea
            placeholder="Descrição da notificação"
            rows={4}
            value={descricao}
            onChange={e => setDescricao(e.target.value)}
          />
          <label>Selecionar Turmas</label>
          <div className="turmas-container">
            {turmas.map(t => (
              <label key={t.id} className="turma-item">
                <input
                  type="checkbox"
                  checked={turmasSelecionadas.includes(t.id)}
                  onChange={() => toggleTurma(t.id)}
                />
                <span>{t.nome} - {cursoNomes[t.cursoId] ?? "—"}</span>
              </label>
            ))}
          </div>
          <button className="btn-accent" onClick={publicar}>Publicar Notificação</button>
        </div>
      </div>

      <Modal isOpen={modal.open} onClose={() => setModal({ open: false, titulo: "", mensagem: "" })} title={modal.titulo}>
        <p>{modal.mensagem}</p>
        <button className="btn-primary" style={{ width: "100%", marginTop: "12px" }} onClick={() => setModal({ open: false, titulo: "", mensagem: "" })}>OK</button>
      </Modal>
    </section>
  );
}