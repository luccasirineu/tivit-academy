import { useState } from "react";

export default function CalendarioGrid({ onSelectDate, eventos = [] }) {
  const today = new Date();
  const [currentMonth, setCurrentMonth] = useState(today);

  const year = currentMonth.getFullYear();
  const month = currentMonth.getMonth();
  const firstDayOfMonth = new Date(year, month, 1).getDay();
  const daysInMonth = new Date(year, month + 1, 0).getDate();

  const days = [];
  for (let i = 0; i < firstDayOfMonth; i++) {
    days.push(null);
  }
  for (let i = 1; i <= daysInMonth; i++) {
    days.push(new Date(year, month, i));
  }

  const handlePrevMonth = () => {
    setCurrentMonth(new Date(year, month - 1, 1));
  };
  const handleNextMonth = () => {
    setCurrentMonth(new Date(year, month + 1, 1));
  };

  // Verifica se determinado dia tem algum evento
  const temEvento = (date) => {
    if (!date) return false;
    return eventos.some((evento) => {
      const dataEvento = new Date(evento.horario);
      return (
        dataEvento.getDate() === date.getDate() &&
        dataEvento.getMonth() === date.getMonth() &&
        dataEvento.getFullYear() === date.getFullYear()
      );
    });
  };

  const isToday = (date) => {
    if (!date) return false;
    return (
      date.getDate() === today.getDate() &&
      date.getMonth() === today.getMonth() &&
      date.getFullYear() === today.getFullYear()
    );
  };

  return (
    <>
      <div className="calendar-header">
        <button onClick={handlePrevMonth}>◀</button>
        <h2>
          {currentMonth.toLocaleString("pt-BR", { month: "long" })} {year}
        </h2>
        <button onClick={handleNextMonth}>▶</button>
      </div>
      <div className="calendar-days">
        {["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"].map((day) => (
          <div key={day}>{day}</div>
        ))}
      </div>
      <div className="calendar-grid">
        {days.map((date, index) => (
          <div
            key={index}
            className={`calendar-day${isToday(date) ? " today" : ""}`}
            onClick={() => date && onSelectDate(date)}
            style={{ cursor: date ? "pointer" : "default" }}
          >
            {date ? date.getDate() : ""}
            {temEvento(date) && <span className="event-dot" />}
          </div>
        ))}
      </div>
    </>
  );
}