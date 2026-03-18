import { useState, useEffect } from "react";
import CalendarioGrid from "../../components/aluno/CalendarioGrid";
import EventoModal from "../../components/aluno/EventoModal";
import api from "../../services/api";

export default function CalendarioProfessor() {
  const [selectedDate, setSelectedDate] = useState(null);
  const [eventos, setEventos] = useState([]);
  const [showFormEvento, setShowFormEvento] = useState(false);
  const [novoEvento, setNovoEvento] = useState({ titulo: "", descricao: "", horario: "" });
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState("");

  useEffect(() => {
    carregarEventos();
  }, []);

  async function carregarEventos() {
    try {
      const response = await api.get("/Evento/getAllEvents");
      setEventos(response.data);
    } catch (err) {
      console.error("Erro ao carregar eventos:", err);
    }
  }

  async function adicionarEvento() {
    if (!novoEvento.titulo || !novoEvento.horario) {
      setErro("Título e horário são obrigatórios.");
      return;
    }
    setSalvando(true);
    setErro("");
    try {
      await api.post("/Evento/adicionarEvento", {
        titulo: novoEvento.titulo,
        descricao: novoEvento.descricao,
        horario: novoEvento.horario,
      });
      setNovoEvento({ titulo: "", descricao: "", horario: "" });
      setShowFormEvento(false);
      await carregarEventos();
    } catch (err) {
      setErro("Erro ao adicionar evento. Tente novamente.");
      console.error(err);
    } finally {
      setSalvando(false);
    }
  }

  const eventosDoDia = selectedDate
    ? eventos.filter((evento) => {
        const dataEvento = new Date(evento.horario);
        return (
          dataEvento.getDate() === selectedDate.getDate() &&
          dataEvento.getMonth() === selectedDate.getMonth() &&
          dataEvento.getFullYear() === selectedDate.getFullYear()
        );
      })
    : [];

  return (
    <section id="calendario" className="active">
      <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", marginBottom: "30px" }}>
        <h1 style={{ textAlign: "center", flex: 1 }}>Calendário de Eventos</h1>
        <button
          onClick={() => setShowFormEvento((v) => !v)}
          style={{
            whiteSpace: "nowrap",
            background: "var(--accent)",
            color: "#fff",
            border: "none",
            padding: "10px 18px",
            borderRadius: "8px",
            cursor: "pointer",
            fontSize: "14px",
          }}
        >
          <i className="bx bx-plus"></i> Novo Evento
        </button>
      </div>

      {showFormEvento && (
        <div className="popup-overlay" onClick={() => { setShowFormEvento(false); setErro(""); }}>
          <div className="form-novo-evento" onClick={(e) => e.stopPropagation()}>
            <h3><i className="bx bx-calendar-plus"></i> Adicionar Evento</h3>

            <label>Título *</label>
            <input
              type="text"
              placeholder="Título do evento"
              value={novoEvento.titulo}
              onChange={(e) => setNovoEvento({ ...novoEvento, titulo: e.target.value })}
            />

            <label>Descrição</label>
            <input
              type="text"
              placeholder="Descrição"
              value={novoEvento.descricao}
              onChange={(e) => setNovoEvento({ ...novoEvento, descricao: e.target.value })}
            />

            <label>Data e Horário *</label>
            <input
              type="datetime-local"
              value={novoEvento.horario}
              onChange={(e) => setNovoEvento({ ...novoEvento, horario: e.target.value })}
            />

            {erro && <p style={{ color: "var(--accent)", fontSize: "14px", marginTop: "8px" }}>{erro}</p>}

            <div style={{ display: "flex", gap: "10px", marginTop: "16px" }}>
              <button
                onClick={adicionarEvento}
                disabled={salvando}
                style={{
                  flex: 1,
                  background: "var(--accent)",
                  color: "#fff",
                  border: "none",
                  padding: "10px 18px",
                  borderRadius: "8px",
                  cursor: "pointer",
                  fontSize: "14px",
                }}
              >
                {salvando ? "Salvando..." : "Salvar Evento"}
              </button>
              <button
                onClick={() => { setShowFormEvento(false); setErro(""); }}
                style={{
                  flex: 1,
                  background: "transparent",
                  color: "var(--text)",
                  border: "1px solid rgba(255,255,255,0.2)",
                  padding: "10px 18px",
                  borderRadius: "8px",
                  cursor: "pointer",
                  fontSize: "14px",
                }}
              >
                Cancelar
              </button>
            </div>
          </div>
        </div>
      )}

      <div className="calendar-container">
        <CalendarioGrid onSelectDate={setSelectedDate} eventos={eventos} />
      </div>

      {selectedDate && (
        <EventoModal
          date={selectedDate}
          eventos={eventosDoDia}
          onClose={() => setSelectedDate(null)}
        />
      )}
    </section>
  );
}