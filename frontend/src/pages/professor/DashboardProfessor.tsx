import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import api from "../../services/api";

interface Props { abrirModal?: (titulo: string, mensagem: string) => void; }
interface ProximoEvento { titulo: string; horario: string; descricao: string; }

export default function DashboardProfessor({ abrirModal }: Props) {
  const { user } = useAuth();
  const [qtdAlunos, setQtdAlunos] = useState(0);
  const [qtdTurmas, setQtdTurmas] = useState(0);
  const [qtdEventos, setQtdEventos] = useState(0);
  const [evento, setEvento] = useState<ProximoEvento | null>(null);

 useEffect(() => {
  if (!user?.id) return;
  console.log("Carregando dashboard para professor ID:", user.id);

  Promise.all([
    api.get(`/Matricula/getAlunosAtivosProfessor/${user.id}`)
      .then(r => { console.log("Alunos ativos:", r.data); setQtdAlunos(r.data); }),

    api.get(`/Curso/getQntdCursosProf/${user.id}`)
      .then(r => { console.log("Qtd turmas:", r.data); setQtdTurmas(r.data); }),

    api.get(`/Evento/getNextWeekEvents`)
      .then(r => { console.log("Eventos próxima semana:", r.data); setQtdEventos(r.data); }),

    api.get(`/Evento/proximoEvento`)
      .then(r => { console.log("Próximo evento:", r.data); setEvento(r.data); })
      .catch((err) => { console.warn("Erro ao buscar próximo evento:", err); }),

  ]).catch(console.error);
}, [user]);

  const d = evento ? new Date(evento.horario) : null;
  const dia = d?.getDate() ?? "--";
  const mes = d ? d.toLocaleString("pt-BR", { month: "short" }).toUpperCase() : "---";
  const horario = d ? d.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" }) : "--:--";

  return (
    <section>
      <div className="dashboard-header">
        <h1>Bem-vindo, Professor!</h1>
        <p>Gerencie as informações e acompanhe os últimos eventos da <strong>TIVIT Academy</strong>.</p>
      </div>
      <div className="resumo-grid">
        <div className="resumo-card"><i className="bx bx-group"></i><div><h3>{qtdAlunos}</h3><p>Alunos Ativos</p></div></div>
        <div className="resumo-card"><i className="bx bx-book"></i><div><h3>{qtdTurmas}</h3><p>Cursos</p></div></div>
        <div className="resumo-card"><i className="bx bx-time"></i><div><h3>{qtdEventos}</h3><p>Eventos nos próximos 7 dias</p></div></div>
      </div>
      <div className="ultimo-evento">
        <h2><i className="bx bx-calendar"></i> Próximo Evento</h2>
        <div className="evento-card">
          <div className="evento-info">
            <div className="evento-data">
              <span className="dia">{dia}</span>
              <span className="mes">{mes}</span>
            </div>
            <div className="evento-detalhes">
              <h3>{evento?.titulo ?? "Nenhum evento encontrado"}</h3>
              <p><strong>Horário:</strong> {horario}</p>
              <p className="descricao">{evento?.descricao}</p>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}