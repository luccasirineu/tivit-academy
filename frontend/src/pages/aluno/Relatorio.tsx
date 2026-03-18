import { useEffect, useState } from "react";
import api from "../../services/api";
import { fetchAllNotas, fetchNomeMateria } from "../../services/aluno.service";
import type { NotaAluno } from "../../types/index";
import { useAuth } from "../../context/AuthContext";

export default function Relatorio() {
  const { user } = useAuth();
  const [dados, setDados] = useState<any[]>([]);
  const [mediaGeral, setMediaGeral] = useState<string | number>(0);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (user?.id) carregarRelatorio();
  }, [user]);

  async function carregarRelatorio() {
    try {
      const notas = await fetchAllNotas(user!.id);

      // busca o nome de cada matéria em paralelo
      const dadosComNome = await Promise.all(
        notas.map(async (item) => {
          try {
            const { materiaNome } = await fetchNomeMateria(item.materiaId);
            return { ...item, materia: materiaNome };
          } catch {
            return { ...item, materia: `Matéria ${item.materiaId}` };
          }
        })
      );

      setDados(dadosComNome);

      const soma = dadosComNome.reduce((acc, item) => acc + item.media, 0);
      setMediaGeral((soma / dadosComNome.length).toFixed(2));
    } catch (error) {
      console.error("Erro ao carregar relatório:", error);
    } finally {
      setLoading(false);
    }
  }

  async function gerarPDF() {
    try {
      const response = await api.get(`/Nota/aluno/${user.id}/exportarRelatorio`, {
        responseType: "blob",
      });

      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `relatorio-aluno-${user.id}.pdf`);
      document.body.appendChild(link);
      link.click();
      link.remove();
    } catch (error) {
      console.error("Erro ao exportar relatório:", error);
    }
  }

  return (
    <section className="relatorio-section">
      <h1 className="titulo-relatorio">Relatório Acadêmico</h1>
      <p className="subtitulo-relatorio">Confira suas notas e média final por matéria.</p>

      <div className="relatorio-container">
        <div className="materia-card">
          {loading ? (
            <p style={{ textAlign: "center", opacity: 0.7 }}>Carregando...</p>
          ) : (
            <table className="tabela-notas">
              <thead>
                <tr>
                  <th>Matéria</th>
                  <th>Nota 1</th>
                  <th>Nota 2</th>
                  <th>Média</th>
                  <th>Faltas</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {dados.map((item, index) => (
                  <tr key={index}>
                    <td>{item.materia}</td>
                    <td>{item.nota1}</td>
                    <td>{item.nota2}</td>
                    <td>{item.media}</td>
                    <td>{item.qtdFaltas}</td>
                    <td>
                      <span className={item.status === "APROVADO" ? "status-aprovado" : "status-recuperacao"}>
                        {item.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        <div className="media-geral-container">
          <h3>Média Geral: <span className="media-geral">{mediaGeral}</span></h3>
        </div>

        <button className="btn-gerar-pdf" onClick={gerarPDF}>
          <i className="fas fa-file-pdf"></i> Exportar PDF
        </button>
      </div>
    </section>
  );
}