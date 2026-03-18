import { useEffect, useState } from "react";
import api from "../../services/api";
import { Modal } from "../../components/common/Modal";
import { fetchMatriculasPendentes, aprovarMatricula, recusarMatricula } from "../../services/admin.service";
import type { MatriculaPendente } from "../../types";

export default function Solicitacoes() {
  const [matriculas, setMatriculas] = useState<MatriculaPendente[]>([]);
  const [cursoNomes, setCursoNomes] = useState<Record<number, string>>({});
  const [modal, setModal] = useState({ open: false, titulo: "", mensagem: "" });

  useEffect(() => { carregar(); }, []);

  async function carregar() {
    try {
      const dados = await fetchMatriculasPendentes();
      setMatriculas(dados);
      const nomes: Record<number, string> = {};
      await Promise.all(dados.map(async m => {
        try {
          const r = await api.get(`/Curso/${m.cursoId}`);
          nomes[m.cursoId] = r.data.nome;
        } catch { nomes[m.cursoId] = "Curso não encontrado"; }
      }));
      setCursoNomes(nomes);
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Erro ao carregar solicitações." }); }
  }

  async function aprovar(id: string) {
    try {
      await aprovarMatricula(id);
      carregar();
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Erro ao aprovar matrícula." }); }
  }

  async function reprovar(id: string) {
    try {
      await recusarMatricula(id);
      carregar();
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Erro ao reprovar matrícula." }); }
  }

  return (
    <section className="prof-section">
      <h2><i className="bx bx-user-check"></i> Solicitações de Matrícula Pendentes</h2>
      <p style={{ opacity: 0.7, marginBottom: "24px" }}>Gerencie as matrículas aguardando aprovação.</p>

      {matriculas.length === 0
        ? <p>Nenhuma matrícula pendente.</p>
        : (
          <div className="matriculas-grid">
            {matriculas.map(m => (
              <div key={m.id} className="matricula-card">
                <h3>{m.nome}</h3>
                <div className="matricula-info">
                  <p><strong>E-mail:</strong> {m.email}</p>
                  <p><strong>CPF:</strong> {m.cpf}</p>
                  <p><strong>Curso:</strong> {cursoNomes[m.cursoId] ?? "..."}</p>
                </div>
                <div className="card-actions">
                  <button className="btn-aprovar" onClick={() => aprovar(m.id)}>Aprovar</button>
                  <button className="btn-reprovar" onClick={() => reprovar(m.id)}>Reprovar</button>
                </div>
              </div>
            ))}
          </div>
        )
      }

      <Modal isOpen={modal.open} onClose={() => setModal({ open: false, titulo: "", mensagem: "" })} title={modal.titulo}>
        <p>{modal.mensagem}</p>
        <button className="btn-primary" style={{ width: "100%", marginTop: "12px" }} onClick={() => setModal({ open: false, titulo: "", mensagem: "" })}>OK</button>
      </Modal>
    </section>
  );
}