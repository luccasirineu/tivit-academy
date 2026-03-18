import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import api from "../../services/api";
import { Modal } from "../../components/common/modal";

interface Curso { id: number; nome: string; }
interface Aluno { matriculaId: number; nome: string; email: string; cpf: string; }
interface Materia { id: number; nome: string; }

export default function Notas() {
  const { user } = useAuth();
  const [cursos, setCursos] = useState<Curso[]>([]);
  const [alunos, setAlunos] = useState<Aluno[]>([]);
  const [materias, setMaterias] = useState<Materia[]>([]);
  const [cursoId, setCursoId] = useState("");
  const [alunoSelecionado, setAlunoSelecionado] = useState<number | null>(null);
  const [materiaId, setMateriaId] = useState("");
  const [nota1, setNota1] = useState("");
  const [nota2, setNota2] = useState("");
  const [modal, setModal] = useState({ open: false, titulo: "", mensagem: "" });

  function abrirModal(titulo: string, mensagem: string) {
    setModal({ open: true, titulo, mensagem });
  }

  useEffect(() => {
    if (!user?.id) return;
    api.get(`/Curso/getAllCursosProf/${user.id}`).then(r => setCursos(r.data)).catch(console.error);
  }, [user]);

  async function handleCursoChange(id: string) {
    setCursoId(id);
    setAlunos([]);
    setAlunoSelecionado(null);
    if (!id) return;
    const res = await api.get(`/Aluno/getAllAlunosByCurso/${id}`);
    setAlunos(res.data);
  }

  async function selecionarAluno(matriculaId: number) {
    setAlunoSelecionado(matriculaId);
    const res = await api.get(`/Materia/getMateriasByCursoId/${cursoId}`);
    setMaterias(res.data);
  }

  async function salvarNota() {
    if (!materiaId) { abrirModal("Atenção", "Selecione uma matéria."); return; }
    try {
      const alunoRes = await api.get(`/Aluno/getAlunoByMatriculaId/${alunoSelecionado}`);
      await api.post("/Nota/adicionarNota", {
        alunoId: alunoRes.data.alunoId,
        materiaId: Number(materiaId),
        nota1: Number(nota1),
        nota2: Number(nota2),
      });
      abrirModal("Sucesso", "Nota lançada com sucesso!");
      setAlunoSelecionado(null);
    } catch { abrirModal("Erro", "Não foi possível lançar a nota."); }
  }

  return (
    <section className="prof-section">
      <h1><i className="bx bx-calculator"></i> Notas</h1>

      <div className="calculo-container" style={{ width: "100%" }}>
        <div className="select-box">
          <label>Selecione o curso</label>
          <select value={cursoId} onChange={(e) => handleCursoChange(e.target.value)}>
            <option value="">-- Selecione um curso --</option>
            {cursos.map(c => <option key={c.id} value={c.id}>{c.nome}</option>)}
          </select>
        </div>

        {alunos.length > 0 && (
          <div className="alunos-lista">
            {alunos.map(a => (
              <div
                key={a.matriculaId}
                className={`aluno-card ${alunoSelecionado === a.matriculaId ? "selecionado" : ""}`}
              >
                <div>
                  <h4>{a.nome}</h4>
                  <p>Email: {a.email}</p>
                  <p>Matrícula: {a.matriculaId}</p>
                </div>
                <button className="btn-lancar" onClick={() => selecionarAluno(a.matriculaId)}>
                  Lançar Nota
                </button>
              </div>
            ))}
          </div>
        )}

        {alunoSelecionado && (
          <div className="form-nota">
            <h2>Lançar Nota</h2>
            <div className="grid">
              <div>
                <label>Matéria</label>
                <select value={materiaId} onChange={(e) => setMateriaId(e.target.value)}>
                  <option value="">Selecione a matéria</option>
                  {materias.map(m => <option key={m.id} value={m.id}>{m.nome}</option>)}
                </select>
              </div>
              <div>
                <label>Nota 1</label>
                <input type="number" step="0.1" value={nota1} onChange={(e) => setNota1(e.target.value)} />
              </div>
              <div>
                <label>Nota 2</label>
                <input type="number" step="0.1" value={nota2} onChange={(e) => setNota2(e.target.value)} />
              </div>
            </div>
            <button className="btn-salvar" onClick={salvarNota}>Salvar Nota</button>
          </div>
        )}
      </div>

      <Modal
        isOpen={modal.open}
        onClose={() => setModal({ open: false, titulo: "", mensagem: "" })}
        title={modal.titulo}
      >
        <p>{modal.mensagem}</p>
        <button
          className="btn-primary"
          style={{ width: "100%", justifyContent: "center", marginTop: "12px" }}
          onClick={() => setModal({ open: false, titulo: "", mensagem: "" })}
        >
          OK
        </button>
      </Modal>
    </section>
  );
}