import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import { fetchDesempenhoAluno, fetchAllNotas, fetchNomeMateria } from "../../services/aluno.service";
import type { DesempenhoMateria, NotaAluno } from "../../types/index";
import GraficoEvolucao from "../../components/aluno/GraficoEvolucao";

const NIVEL_CONFIG: Record<string, { label: string; css: string }> = {
  OURO:   { label: "Ouro 🏆",   css: "ouro" },
  PRATA:  { label: "Prata 🥈",  css: "prata" },
  BRONZE: { label: "Bronze 🥉", css: "bronze" },
};

export default function Desempenho() {
  const { user } = useAuth();
  const [desempenho, setDesempenho] = useState<DesempenhoMateria[]>([]);
  const [notas, setNotas] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  const alunoId = user?.id;

  useEffect(() => {
    if (!alunoId) return;

    async function carregar() {
      setLoading(true);
      setErro(null);
      try {
        const [resDesempenho, resNotas] = await Promise.all([
          fetchDesempenhoAluno(alunoId),
          fetchAllNotas(alunoId),
        ]);

        setDesempenho(Array.isArray(resDesempenho) ? resDesempenho : [resDesempenho]);

        const notasComNome = await Promise.all(
          resNotas.map(async (nota) => {
            const data = await fetchNomeMateria(nota.materiaId);
            return { ...nota, nomeMateria: data.materiaNome };
          })
        );

        setNotas(notasComNome);
        
      } catch (err) {
        console.error("Erro ao carregar desempenho:", err);
        setErro("Não foi possível carregar o desempenho.");
      } finally {
        setLoading(false);
      }
    }

    carregar();
  }, [alunoId]);

  if (loading) {
    return (
      <section id="desempenho-aluno" className="active">
        <div className="desempenho-header"><h1>Meu Desempenho</h1></div>
        <p style={{ textAlign: "center", padding: "40px", opacity: 0.7 }}>
          Carregando desempenho...
        </p>
      </section>
    );
  }

  if (erro) {
    return (
      <section id="desempenho-aluno" className="active">
        <div className="desempenho-header"><h1>Meu Desempenho</h1></div>
        <p style={{ textAlign: "center", padding: "40px", color: "var(--accent)" }}>
          {erro}
        </p>
      </section>
    );
  }

  if (!desempenho.length) {
    return (
      <section id="desempenho-aluno" className="active">
        <div className="desempenho-header"><h1>Meu Desempenho</h1></div>
        <p style={{ textAlign: "center", padding: "40px", opacity: 0.7 }}>
          Nenhum dado de desempenho encontrado.
        </p>
      </section>
    );
  }
  
  const melhorMateria = desempenho.reduce((melhor, d) =>
    d.media > melhor.media ? d : melhor,
    desempenho[0]
  );
  const mediaGeral = (
    desempenho.reduce((acc, d) => acc + d.media, 0) / desempenho.length
  ).toFixed(1);

  const totalFaltas = desempenho.reduce((acc, d) => acc + d.qtdFaltas, 0);

  const ordemNivel = { OURO: 3, PRATA: 2, BRONZE: 1 };
  const melhorNivel = desempenho.reduce((melhor, d) =>
    (ordemNivel[d.nivel] || 0) > (ordemNivel[melhor] || 0) ? d.nivel : melhor,
    desempenho[0].nivel
  );
  const nivelConfig = NIVEL_CONFIG[melhorNivel] || { label: melhorNivel, css: "" };

  return (
    <section id="desempenho-aluno" className="active">
      <div className="desempenho-header">
        <h1>Meu Desempenho</h1>
        <p>Acompanhe suas notas, média e nível acadêmico.</p>
      </div>

      <div className="desempenho-cards">
        <div className="card">
          <h3>Melhor Matéria</h3>
          <p className="valor">{melhorMateria.nomeMateria}</p>
        </div>
        <div className="card">
          <h3>Média Geral</h3>
          <p className="valor destaque">{mediaGeral}</p>
        </div>
        <div className="card">
          <h3>Total de Faltas</h3>
          <p className="valor">{totalFaltas}</p>
        </div>
        <div className={`card nivel-card ${nivelConfig.css}`}>
          <h3>Seu Nível</h3>
          <p className="nivel-label">{nivelConfig.label}</p>
        </div>
      </div>

      <div className="graficos-container">
        <div className="grafico">
          <GraficoEvolucao notas={notas} />
        </div>
      </div>
    </section>
  );
}