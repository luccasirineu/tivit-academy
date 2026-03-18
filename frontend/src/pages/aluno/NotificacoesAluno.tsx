import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import { fetchNotificacoesByTurma } from "../../services/aluno.service";
import type { Notificacao } from "../../types/index";

export default function NotificacoesAluno() {
  const { user } = useAuth();
  const [notificacoes, setNotificacoes] = useState<Notificacao[]>([]);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState("");

  useEffect(() => {
    if (!user?.turmaId) return;
    carregar();
  }, [user]);

  async function carregar() {
    try {
      setLoading(true);
      const data = await fetchNotificacoesByTurma(user!.turmaId);
      setNotificacoes(data);
    } catch {
      setErro("Não foi possível carregar as notificações.");
    } finally {
      setLoading(false);
    }
  }

  function formatarData(dataStr: string) {
  if (!dataStr) return "Data não informada";

  // força leitura como UTC adicionando Z se não tiver
  const dataUTC = new Date(dataStr.endsWith("Z") ? dataStr : dataStr + "Z");

  if (dataUTC.getFullYear() <= 1) return "Data não informada";

  // converte para horário de Brasília (UTC-3)
  return dataUTC.toLocaleString("pt-BR", {
    day: "2-digit",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    timeZone: "America/Sao_Paulo",
  });
}

  return (
    <section className="notificacoes-section">
      <div className="notificacoes-header">
        <h1><i className="bx bx-bell"></i> Notificações</h1>
        <p>Acompanhe os avisos e comunicados da sua turma.</p>
      </div>

      {loading && (
        <div className="notificacoes-loading">
          <i className="bx bx-loader-alt bx-spin"></i>
          <p>Carregando notificações...</p>
        </div>
      )}

      {erro && (
        <div className="notificacoes-erro">
          <i className="bx bx-error-circle"></i>
          <p>{erro}</p>
        </div>
      )}

      {!loading && !erro && notificacoes.length === 0 && (
        <div className="notificacoes-vazio">
          <i className="bx bx-bell-off"></i>
          <p>Nenhuma notificação por enquanto.</p>
        </div>
      )}

      {!loading && !erro && notificacoes.length > 0 && (
        <div className="notificacoes-lista">
          {notificacoes.map((n, i) => (
            <div key={i} className="notificacao-card">
              <div className="notificacao-icone">
                <i className="bx bx-bell"></i>
              </div>
              <div className="notificacao-body">
                <div className="notificacao-top">
                  <h3>{n.titulo}</h3>
                  <span className="notificacao-data">
                    <i className="bx bx-calendar"></i>
                    {formatarData(n.dataCriacao)}
                  </span>
                </div>
                <p>{n.descricao.trim()}</p>
              </div>
            </div>
          ))}
        </div>
      )}
    </section>
  );
}