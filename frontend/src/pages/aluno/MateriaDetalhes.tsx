import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api from "../../services/api";
import { fetchConteudosMateria } from "../../services/aluno.service";
import { useAuth } from "../../context/AuthContext";
import ChatConteudo from "../../components/aluno/ChatConteudo";
import taiIcon from "../../assets/TAI.png";

export default function MateriaDetalhes() {
  const { materiaId } = useParams();
  const { user } = useAuth();
  const navigate = useNavigate();
  const [conteudos, setConteudos] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [chatAberto, setChatAberto] = useState<{ conteudoId: number; titulo: string } | null>(null);
  const turmaId = user?.turmaId;
  const BASE_URL = api.defaults.baseURL?.replace("/api", "") ?? "http://localhost:5027";

  useEffect(() => {
    async function carregar() {
      try {
        if (!materiaId || !turmaId) return;
        const data = await fetchConteudosMateria(materiaId, turmaId);
        setConteudos(Array.isArray(data.conteudos) ? data.conteudos : []);
      } catch (error) {
        console.error("Erro ao carregar conteúdos:", error);
        setConteudos([]);
      } finally {
        setLoading(false);
      }
    }

    carregar();
  }, [materiaId, turmaId]);

  return (
    <section className="materias-section">
      <button onClick={() => navigate("/aluno/materias")} className="btn-voltar">
        ← Voltar
      </button>

      <h1 className="titulo-materias">Conteúdos da Matéria</h1>

      {loading && <p style={{ textAlign: "center", opacity: 0.7 }}>Carregando...</p>}

      {!loading && conteudos.length === 0 && (
        <p style={{ textAlign: "center", opacity: 0.7 }}>
          Nenhum conteúdo disponível para esta matéria.
        </p>
      )}

      <div className="materias-cards">
        {conteudos.map((c) => (
          <div key={c.id} className="materia conteudo-card" style={{ position: "relative", minHeight: "200px", display: "flex", flexDirection: "column", justifyContent: "center", alignItems: "center" }}>
            <h3>{c.titulo}</h3>

            {c.caminhoOuUrl && (
              <a
                href={c.tipo === "pdf" ? `${BASE_URL}${c.caminhoOuUrl}` : c.caminhoOuUrl}

                target="_blank"
                rel="noreferrer"
                style={{ color: "var(--accent)", fontSize: "14px", marginTop: "8px", display: "block" }}
              >
                Acessar material →
              </a>
            )}
            {c.tipo === "pdf" && (
              <button
                onClick={() => setChatAberto({ conteudoId: c.id, titulo: c.titulo })}
                style={{
                  position: "absolute",
                  bottom: "10px",
                  left: "10px",
                  background: "none",
                  border: "none",
                  cursor: "pointer",
                  padding: "0",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  transition: "transform 0.2s",
                }}
                onMouseEnter={(e) => (e.currentTarget.style.transform = "scale(1.1)")}
                onMouseLeave={(e) => (e.currentTarget.style.transform = "scale(1)")}
                title="Perguntar sobre este conteúdo"
              >
                <img src={taiIcon} alt="TAI" style={{ width: "58px", height: "48px" }} />
              </button>
            )}
          </div>
        ))}
      </div>

      {chatAberto && (
        <ChatConteudo
          conteudoId={chatAberto.conteudoId}
          tituloConteudo={chatAberto.titulo}
          isOpen={!!chatAberto}
          onClose={() => setChatAberto(null)}
        />
      )}
    </section>
  );
}