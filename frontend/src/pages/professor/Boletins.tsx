import { useState } from "react";
import api from "../../services/api";
import { Modal } from "../../components/common/modal";

interface Nota { alunoId: number; materiaId: number; media: number; qtdFaltas: number; }
interface AlunoInfo { nome: string; cursoNome: string; matriculaId: number; }
interface AlunoAgrupado { info: AlunoInfo; notas: (Nota & { nomeMateria: string })[]; }

export default function Boletins() {
  const [busca, setBusca] = useState("");
  const [resultados, setResultados] = useState<AlunoAgrupado[]>([]);
  const [loading, setLoading] = useState(false);
  const [modal, setModal] = useState({ open: false, titulo: "", mensagem: "" });

  function abrirModal(titulo: string, mensagem: string) {
    setModal({ open: true, titulo, mensagem });
  }

  async function buscarBoletim() {
    if (!busca.trim()) return;
    setLoading(true);
    try {
      const isMatricula = /^\d+$/.test(busca.trim());
      const url = isMatricula
        ? `/Nota/aluno/${busca.trim()}/getAllNotasByMatriculaId`
        : `/Nota/aluno/getAllNotasByNome?nome=${encodeURIComponent(busca.trim())}`;

      const res = await api.get(url);
      const notas: Nota[] = res.data;
      if (!notas?.length) { abrirModal("Atenção", "Nenhum aluno encontrado."); return; }

      const agrupado: Record<number, Nota[]> = notas.reduce((acc, n) => {
        if (!acc[n.alunoId]) acc[n.alunoId] = [];
        acc[n.alunoId].push(n);
        return acc;
      }, {} as Record<number, Nota[]>);

      const resultados: AlunoAgrupado[] = await Promise.all(
        Object.entries(agrupado).map(async ([alunoId, notasAluno]) => {
          const [infoRes, notasComNome] = await Promise.all([
            api.get(`/Aluno/contextMe/${alunoId}`),
            Promise.all(notasAluno.map(async (n) => {
              try {
                const m = await api.get(`/Materia/getNomeMateria/${n.materiaId}`);
                return { ...n, nomeMateria: m.data.materiaNome };
              } catch { return { ...n, nomeMateria: `Matéria ${n.materiaId}` }; }
            })),
          ]);
          return { info: infoRes.data, notas: notasComNome };
        })
      );
      setResultados(resultados);
    } catch (err) {
      abrirModal("Erro", "Aluno não encontrado.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className="prof-section">
      <h2><i className="bx bx-file"></i> Boletins</h2>

      <div className="boletim-container">
        <div className="search-box">
          <input
            type="text"
            placeholder="Pesquise o aluno pelo nome ou matrícula"
            value={busca}
            onChange={(e) => setBusca(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && buscarBoletim()}
          />
          <button onClick={buscarBoletim}>
            <i className="bx bx-search"></i> Buscar
          </button>
        </div>

        {loading && <p style={{ textAlign: "center", opacity: 0.7 }}>Buscando...</p>}

        {resultados.map((aluno, idx) => (
          <div key={idx} className="resultado-box boletim-aluno">
            <h3>Informações do aluno</h3>
            <p><strong>Nome:</strong> {aluno.info.nome}</p>
            <p><strong>Curso:</strong> {aluno.info.cursoNome}</p>
            <p><strong>Matrícula:</strong> {aluno.info.matriculaId}</p>
            <table className="tabela-notas">
              <thead>
                <tr><th>Disciplina</th><th>Média Final</th><th>Frequência</th></tr>
              </thead>
              <tbody>
                {aluno.notas.map((n, i) => {
                  const freq = Math.max(0, Math.round(((70 - n.qtdFaltas) / 70) * 100));
                  return (
                    <tr key={i}>
                      <td>{n.nomeMateria}</td>
                      <td>{n.media.toFixed(1)}</td>
                      <td>{freq}%</td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
            <hr />
          </div>
        ))}
      </div>

      <Modal
        isOpen={modal.open}
        onClose={() => setModal({ open: false, titulo: "", mensagem: "" })}
        title={modal.titulo}
      >
        <p>{modal.mensagem}</p>
        <button
          className="btn-primary"
          style={{ width: "100%", justifyContent: "center", marginTop: "12px" }}
          onClick={() => setModal({ open: false, titulo: "", mensagem: "" })}
        >
          OK
        </button>
      </Modal>
    </section>
  );
}