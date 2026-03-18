import { useEffect, useState } from "react";
import MateriaCard from "@/components/aluno/MateriaCard";
import { fetchMateriasAluno } from "@/services/aluno.service";
import { useAuth } from "@/context/AuthContext";
import type { Materia } from "@/types";
import { useNotification } from "@/hooks";

export default function Materias() {
  const [materias, setMaterias] = useState<Materia[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { user } = useAuth();
  const { error: showError } = useNotification();

  useEffect(() => {
    async function carregar() {
      try {
        const cursoId = user?.cursosIds?.[0];

        if (!cursoId) {
          setError("Usuário sem curso vinculado");
          return;
        }

        setLoading(true);
        setError(null);
        const data = await fetchMateriasAluno(cursoId);
        setMaterias(data);
      } catch (err) {
        const message = err instanceof Error ? err.message : "Erro ao carregar matérias";
        setError(message);
        showError(message);
        setMaterias([]);
      } finally {
        setLoading(false);
      }
    }

    carregar();
  }, [user?.cursosIds, showError]);

  if (!user?.cursosIds?.[0]) {
    return (
      <section className="materias-section">
        <h1 className="titulo-materias">Minhas Matérias</h1>
        <div className="error-message">Usuário sem curso vinculado</div>
      </section>
    );
  }

  if (loading) {
    return (
      <section className="materias-section">
        <h1 className="titulo-materias">Minhas Matérias</h1>
        <div className="loading-skeleton">Carregando matérias...</div>
      </section>
    );
  }

  if (error || !materias || materias.length === 0) {
    return (
      <section className="materias-section">
        <h1 className="titulo-materias">Minhas Matérias</h1>
        <p className="subtitulo-materias">
          {error || "Nenhuma matéria foi encontrada."}
        </p>
      </section>
    );
  }

  return (
    <section className="materias-section">
      <h1 className="titulo-materias">Minhas Matérias</h1>
      <p className="subtitulo-materias">
        Explore os conteúdos e materiais de cada disciplina.
      </p>
      <div className="materias-cards">
        {materias.map((m: Materia) => (
          <MateriaCard key={m.id} materia={m} />
        ))}
      </div>
    </section>
  );
}