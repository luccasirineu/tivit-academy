import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Cell,
} from "recharts";

const STATUS_COR = {
  APROVADO:    "#4caf50",
  REPROVADO:   "#e53935",
};

export default function GraficoEvolucao({ notas = [] }) {
  const data = notas.map((n, index) => ({
    label: n.nomeMateria,
    materiaId: n.materiaId,
    nota1: Number(n.nota1.toFixed(1)),
    nota2: Number(n.nota2.toFixed(1)),
    media: Number(n.media.toFixed(1)),
    status: n.status,
  }));

  const CustomTooltip = ({ active, payload }: any) => {
    if (!active || !payload?.length) return null;
    const item = payload[0].payload;
    return (
      <div style={{
        background: "var(--bg-secondary)",
        border: "1px solid var(--accent)",
        borderRadius: "8px",
        padding: "10px 14px",
        fontSize: "0.85rem",
        color: "var(--text)",
      }}>
        <p style={{ fontWeight: "bold", marginBottom: "6px" }}>Matéria ID: {item.materiaId}</p>
        <p>Nota 1: <strong>{item.nota1}</strong></p>
        <p>Nota 2: <strong>{item.nota2}</strong></p>
        <p>Média: <strong>{item.media}</strong></p>
        <p>Status: <strong style={{ color: STATUS_COR[item.status] || "var(--accent)" }}>{item.status}</strong></p>
      </div>
    );
  };

  if (!notas.length) {
    return (
      <div style={{
        background: "var(--bg-secondary)",
        padding: "20px 24px",
        borderRadius: "16px",
        boxShadow: "0 6px 18px var(--shadow)",
        textAlign: "center",
        opacity: 0.7,
      }}>
        <h2 style={{ color: "var(--accent)", marginBottom: "16px" }}>Notas por Matéria</h2>
        <p>Nenhuma nota encontrada.</p>
      </div>
    );
  }

  return (
    <div style={{
      width: "100%",
      background: "var(--bg-secondary)",
      padding: "20px 24px",
      borderRadius: "16px",
      boxShadow: "0 6px 18px var(--shadow)",
    }}>
      <h2 style={{ marginBottom: "24px", color: "var(--accent)" }}>
        Notas por Matéria
      </h2>

      <ResponsiveContainer width="100%" height={data.length * 60 + 40}>
        <BarChart
          data={data}
          layout="vertical"
          margin={{ top: 0, right: 50, left: 10, bottom: 0 }}
        >
          <CartesianGrid strokeDasharray="3 3" horizontal={false} stroke="rgba(255,255,255,0.08)" />
          <XAxis
            type="number"
            domain={[0, 10]}
            tickCount={6}
            tick={{ fill: "var(--text)", fontSize: 12 }}
          />
          <YAxis
            type="category"
            dataKey="label"
            width={160}
            tick={{ fill: "var(--text)", fontSize: 12 }}
          />
          <Tooltip content={<CustomTooltip active={false} payload={[]} />} cursor={{ fill: "rgba(255,255,255,0.05)" }} />
          <Bar dataKey="media" radius={[0, 8, 8, 0]} barSize={22} name="Média">
            {data.map((entry, index) => (
              <Cell key={index} fill={STATUS_COR[entry.status] || "#ff0054"} />
            ))}
          </Bar>
        </BarChart>
      </ResponsiveContainer>

      {/* Legenda */}
      <div style={{ display: "flex", gap: "20px", justifyContent: "center", marginTop: "16px", flexWrap: "wrap" }}>
        {Object.entries(STATUS_COR).map(([status, cor]) => (
          <span key={status} style={{ display: "flex", alignItems: "center", gap: "6px", fontSize: "0.8rem", opacity: 0.9 }}>
            <span style={{ width: 12, height: 12, borderRadius: "3px", background: cor, display: "inline-block" }} />
            {status}
          </span>
        ))}
      </div>
    </div>
  );
}