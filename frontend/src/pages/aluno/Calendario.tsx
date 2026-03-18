import { useState, useEffect } from "react";
import CalendarioGrid from "../../components/aluno/CalendarioGrid";
import EventoModal from "../../components/aluno/EventoModal";
import { fetchAllEventos } from "../../services/aluno.service";
import type { Evento } from "../../types/index";

export default function Calendario() {
  const [selectedDate, setSelectedDate] = useState<Date | null>(null);
  const [eventos, setEventos] = useState<Evento[]>([]);

  useEffect(() => {
    async function carregarEventos() {
      try {
        const data = await fetchAllEventos();
        setEventos(data);
      } catch (err) {
        console.error("Erro ao carregar eventos:", err);
      }
    }
    carregarEventos();
  }, []);

  // Filtra os eventos do dia selecionado
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
      <h1 style={{ textAlign: "center", marginBottom: "30px" }}>
        Calendário de Eventos
      </h1>
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