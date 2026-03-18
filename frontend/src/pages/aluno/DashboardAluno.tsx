import { useEffect, useState } from "react";
import { fetchProximoEvento } from "../../services/aluno.service";
import type { Evento } from "../../types/index";

export default function DashboardAluno() {
  const [evento, setEvento] = useState<Evento | null>(null);

  useEffect(() => {
    async function carregarProximoEvento() {
      try {
        const data = await fetchProximoEvento();
        setEvento(data);
      } catch (err) {
        console.error(err);
      }
    }

    carregarProximoEvento();
  }, []);

  if (!evento) return <h2>Carregando evento...</h2>;

  const data = new Date(evento.horario);

  return (
    <section className="dashboard-content">
      <div className="desempenho-header">
        <h1>Bem-vindo, Aluno</h1>
        <p>Acompanhe seu desempenho acadêmico e próximos eventos.</p>
      </div>

      <div className="ultimo-evento">
        <h2>Próximo Evento</h2>

        <div className="evento-card">
          <div className="evento-info">
            <div className="evento-data">
              <span className="dia">{data.getDate()}</span>
              <span className="mes">
                {data.toLocaleString("pt-BR", { month: "short" })}
              </span>
            </div>

            <div className="evento-detalhes">
              <h3>{evento.titulo}</h3>
              <p>{evento.descricao}</p>
              <p className="descricao">
                <i className="fas fa-clock"></i> {data.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" })}
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}