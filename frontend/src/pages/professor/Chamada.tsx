import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import api from "../../services/api";
import { Modal } from "../../components/common/Modal";

interface Curso { id: number; nome: string; }
interface Turma { id: number; nome: string; }
interface Materia { id: number; nome: string; }
interface Aluno { nome: string; matriculaId: number; }
interface ChamadaItem { matriculaId: number; materiaId: number; turmaId: number; faltou: boolean; }

export default function Chamada() {
  const { user } = useAuth();
  const [cursos, setCursos] = useState<Curso[]>([]);
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [materias, setMaterias] = useState<Materia[]>([]);
  const [alunos, setAlunos] = useState<Aluno[]>([]);
  const [cursoId, setCursoId] = useState("");
  const [turmaId, setTurmaId] = useState("");
  const [materiaId, setMateriaId] = useState("");
  const [faltas, setFaltas] = useState<Record<number, boolean>>({});
  const [sucesso, setSucesso] = useState(false);
  const [bannerMsg, setBannerMsg] = useState("");
  const [showBanner, setShowBanner] = useState(false);
  const [modal, setModal] = useState({ open: false, titulo: "", mensagem: "" });

  function abrirModal(titulo: string, mensagem: string) {
    setModal({ open: true, titulo, mensagem });
  }

  useEffect(() => {
    if (!user?.id) return;
    api.get(`/Curso/getAllCursosProf/${user.id}`).then(r => setCursos(r.data)).catch(console.error);
  }, [user]);

  async function handleCursoChange(id: string) {
    setCursoId(id); setTurmaId(""); setMateriaId(""); setAlunos([]);
    setTurmas([]); setMaterias([]);
    if (!id) return;
    const res = await api.get(`/Turma/getTurmasByCursoId/${id}`);
    setTurmas(res.data);
  }

  async function handleTurmaChange(id: string) {
    setTurmaId(id); setMateriaId(""); setAlunos([]);
    if (!id) return;
    const res = await api.get(`/Materia/getMateriasByCursoId/${cursoId}`);
    setMaterias(res.data);
  }

  async function handleMateriaChange(id: string) {
    setMateriaId(id);
    if (!id || !turmaId) return;
    const res = await api.get(`/Aluno/getAllAlunosByTurmaId/${turmaId}`);
    setAlunos(res.data);
    setFaltas({});
  }

  async function salvarChamada() {
    const payload: ChamadaItem[] = alunos.map(a => ({
      matriculaId: a.matriculaId,
      materiaId: Number(materiaId),
      turmaId: Number(turmaId),
      faltou: faltas[a.matriculaId] ?? false,
    }));
    try {
      const res = await api.post("/Chamada/realizarChamada", payload);
      if (res.status === 204) { setSucesso(true); return; }
    } catch (err: any) {
      if (err.response?.status === 409 && err.response.data?.tipo === "CHAMADA_JA_REALIZADA") {
        setBannerMsg(err.response.data.mensagem);
        setShowBanner(true);
        return;
      }
      abrirModal("Erro", "Erro ao salvar chamada.");
    }
  }

  async function substituirChamada() {
    setShowBanner(false);
    const payload = alunos.map(a => ({
      matriculaId: a.matriculaId,
      turmaId: Number(turmaId),
      materiaId: Number(materiaId),
      faltou: faltas[a.matriculaId] ?? false,
      horarioChamada: new Date().toISOString(),
    }));
    try {
      await api.put("/Chamada/atualizarChamada", payload);
      setSucesso(true);
    } catch { abrirModal("Erro", "Erro ao substituir chamada."); }
  }

  function novaChamada() {
    setSucesso(false); setMateriaId(""); setAlunos([]); setFaltas({});
  }

  return (
    <section className="prof-section">
      <h1><i className="bx bx-check-square"></i> Chamada</h1>

      <div className="select-box">
        <label><strong>Selecione o curso</strong></label>
        <select value={cursoId} onChange={(e) => handleCursoChange(e.target.value)}>
          <option value="">-- Selecione um curso --</option>
          {cursos.map(c => <option key={c.id} value={c.id}>{c.nome}</option>)}
        </select>
      </div>

      {turmas.length > 0 && (
        <div className="select-box">
          <label><strong>Selecione a turma</strong></label>
          <select value={turmaId} onChange={(e) => handleTurmaChange(e.target.value)}>
            <option value="">-- Selecione a turma --</option>
            {turmas.map(t => <option key={t.id} value={t.id}>{t.nome}</option>)}
          </select>
        </div>
      )}

      {materias.length > 0 && (
        <div className="select-box">
          <label><strong>Selecione a matéria</strong></label>
          <select value={materiaId} onChange={(e) => handleMateriaChange(e.target.value)}>
            <option value="">-- Selecione a matéria --</option>
            {materias.map(m => <option key={m.id} value={m.id}>{m.nome}</option>)}
          </select>
        </div>
      )}

      {alunos.length > 0 && !sucesso && (
        <div style={{ width: "100%" }}>
          <h3>Lista de Alunos</h3>
          <table className="tabela-notas">
            <thead><tr><th>Aluno</th><th>Matrícula</th><th>Faltou?</th></tr></thead>
            <tbody>
              {alunos.map(a => (
                <tr key={a.matriculaId}>
                  <td>{a.nome}</td>
                  <td>{a.matriculaId}</td>
                  <td style={{ textAlign: "center" }}>
                    <input
                      type="checkbox"
                      checked={faltas[a.matriculaId] ?? false}
                      onChange={(e) => setFaltas(f => ({ ...f, [a.matriculaId]: e.target.checked }))}
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          <button className="btn-primary" style={{ marginTop: "16px" }} onClick={salvarChamada}>
            <i className="bx bx-save"></i> Salvar Chamada
          </button>
        </div>
      )}

      {sucesso && (
        <div className="chamada-sucesso">
          <p><i className="bx bx-check"></i> Chamada realizada com sucesso!</p>
          <button onClick={novaChamada}>Nova chamada</button>
        </div>
      )}

      {showBanner && (
        <div className="chamada-container">
          <div className="chamada-container-content">
            <p>{bannerMsg}</p>
            <div className="chamada-container-actions">
              <button className="btn-secondary" onClick={() => setShowBanner(false)}>Cancelar</button>
              <button className="btn-danger" onClick={substituirChamada}>Substituir chamada</button>
            </div>
          </div>
        </div>
      )}

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