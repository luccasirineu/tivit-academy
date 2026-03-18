export default function EventoModal({ date, eventos = [], onClose }) {
  const formattedDate = new Date(date).toLocaleDateString("pt-BR", {
    weekday: "long",
    day: "2-digit",
    month: "long",
    year: "numeric",
  });

  return (
    <div className="modal" style={{ display: "flex" }}>
      <div className="modal-content">
        <span className="close" onClick={onClose}>
          &times;
        </span>

        <h2 style={{ marginBottom: "6px" }}>Eventos do dia</h2>
        <p
          style={{
            marginBottom: "20px",
            textTransform: "capitalize",
            opacity: 0.7,
            fontSize: "0.9rem",
          }}
        >
          {formattedDate}
        </p>

        {eventos.length === 0 ? (
          <p style={{ opacity: 0.6, textAlign: "center", padding: "20px 0" }}>
            Nenhum evento neste dia.
          </p>
        ) : (
          <div style={{ display: "flex", flexDirection: "column", gap: "12px" }}>
            {eventos.map((evento) => {
              const hora = new Date(evento.horario).toLocaleTimeString("pt-BR", {
                hour: "2-digit",
                minute: "2-digit",
              });
              return (
                <div
                  key={evento.id}
                  style={{
                    background: "var(--bg)",
                    borderRadius: "10px",
                    padding: "14px 16px",
                    borderLeft: "4px solid var(--accent)",
                    textAlign: "left",
                  }}
                >
                  <strong style={{ fontSize: "1rem" }}>{evento.titulo}</strong>
                  <p style={{ margin: "6px 0 4px", opacity: 0.85, fontSize: "0.9rem" }}>
                    {evento.descricao}
                  </p>
                  <span style={{ fontSize: "0.8rem", opacity: 0.65 }}>
                    <i className="fas fa-clock" style={{ marginRight: "5px" }} />
                    {hora}
                  </span>
                </div>
              );
            })}
          </div>
        )}

        <button
          onClick={onClose}
          style={{
            marginTop: "20px",
            background: "var(--accent)",
            color: "#fff",
            border: "none",
            padding: "10px 24px",
            borderRadius: "8px",
            cursor: "pointer",
            width: "100%",
          }}
        >
          Fechar
        </button>
      </div>
    </div>
  );
}