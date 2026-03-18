import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import api from "../../services/api";
import { Modal } from "../../components/common/modal";

interface Curso { id: number; nome: string; }
interface Turma { id: number; nome: string; }
interface Materia { id: number; nome: string; }

export default function Conteudo() {
  const { user } = useAuth();
  const [cursos, setCursos] = useState<Curso[]>([]);
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [materias, setMaterias] = useState<Materia[]>([]);
  const [cursoId, setCursoId] = useState("");
  const [turmaId, setTurmaId] = useState("");
  const [materiaId, setMateriaId] = useState("");
  const [tipo, setTipo] = useState("");
  const [titulo, setTitulo] = useState("");
  const [url, setUrl] = useState("");
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [modal, setModal] = useState({ open: false, titulo: "", mensagem: "" });

  function abrirModal(titulo: string, mensagem: string) {
    setModal({ open: true, titulo, mensagem });
  }

  useEffect(() => {
    if (!user?.id) return;
    api.get(`/Curso/getAllCursosProf/${user.id}`).then(r => setCursos(r.data)).catch(console.error);
  }, [user]);

  async function handleCursoChange(id: string) {
    setCursoId(id); setTurmaId(""); setMateriaId(""); setTipo("");
    setTurmas([]); setMaterias([]);
    if (!id) return;
    const res = await api.get(`/Turma/getTurmasByCursoId/${id}`);
    setTurmas(res.data);
  }

  async function handleTurmaChange(id: string) {
    setTurmaId(id); setMateriaId(""); setTipo("");
    if (!id) return;
    const res = await api.get(`/Materia/getMateriasByCursoId/${cursoId}`);
    setMaterias(res.data);
  }

  async function publicar() {
    if (!titulo || !materiaId || !turmaId) {
      abrirModal("Atenção", "Preencha todos os campos obrigatórios.");
      return;
    }
    try {
      if (tipo === "link") {
        await api.post("/Conteudo/upload/link", {
          titulo, materiaId: Number(materiaId), url, turmaId: Number(turmaId),
        });
      } else if (tipo === "pdf") {
        if (!arquivo) { abrirModal("Atenção", "Selecione um PDF."); return; }
        const formData = new FormData();
        formData.append("Titulo", titulo);
        formData.append("MateriaId", String(materiaId));
        formData.append("TurmaId", String(turmaId));
        formData.append("Arquivo", arquivo);
        await api.post("/Conteudo/upload/pdf", formData, {
          headers: { "Content-Type": "multipart/form-data" },
        });
      }
      abrirModal("Publicado", "Conteúdo publicado com sucesso!");
      setTitulo(""); setUrl(""); setArquivo(null); setTipo("");
    } catch { abrirModal("Erro", "Erro ao publicar conteúdo."); }
  }

  return (
    <section className="prof-section">
      <h1><i className="bx bx-upload"></i> Publicar Conteúdo</h1>

      <div className="select-box">
        <label><strong>Curso</strong></label>
        <select value={cursoId} onChange={(e) => handleCursoChange(e.target.value)}>
          <option value="">-- Selecione o curso --</option>
          {cursos.map(c => <option key={c.id} value={c.id}>{c.nome}</option>)}
        </select>
      </div>

      {turmas.length > 0 && (
        <div className="select-box">
          <label><strong>Turma</strong></label>
          <select value={turmaId} onChange={(e) => handleTurmaChange(e.target.value)}>
            <option value="">-- Selecione a turma --</option>
            {turmas.map(t => <option key={t.id} value={t.id}>{t.nome}</option>)}
          </select>
        </div>
      )}

      {materias.length > 0 && (
        <div className="select-box">
          <label><strong>Matéria</strong></label>
          <select value={materiaId} onChange={(e) => setMateriaId(e.target.value)}>
            <option value="">-- Selecione a matéria --</option>
            {materias.map(m => <option key={m.id} value={m.id}>{m.nome}</option>)}
          </select>
        </div>
      )}

      {materiaId && (
        <div className="select-box">
          <label><strong>Tipo de conteúdo</strong></label>
          <select value={tipo} onChange={(e) => setTipo(e.target.value)}>
            <option value="">-- Selecione --</option>
            <option value="link">Link (vídeo / site)</option>
            <option value="pdf">PDF</option>
          </select>
        </div>
      )}

      {tipo && (
        <div className="conteudo-form">
          <h3 style={{ margin: "0 0 8px", fontSize: "16px", fontWeight: 700 }}>
            <i className="bx bx-edit-alt" style={{ color: "var(--accent)", marginRight: "8px" }}></i>
            Detalhes do Conteúdo
          </h3>

          <label>Título</label>
          <input
            type="text"
            placeholder="Título do conteúdo"
            value={titulo}
            onChange={(e) => setTitulo(e.target.value)}
          />

          {tipo === "link" && (
            <>
              <label>URL do conteúdo</label>
              <input
                type="text"
                placeholder="https://..."
                value={url}
                onChange={(e) => setUrl(e.target.value)}
              />
            </>
          )}

          {tipo === "pdf" && (
            <>
              <label>Arquivo PDF</label>
              <input
                type="file"
                accept="application/pdf"
                onChange={(e) => setArquivo(e.target.files?.[0] ?? null)}
              />
            </>
          )}

          <button
            className="btn-primary"
            onClick={publicar}
            style={{ marginTop: "8px", width: "100%", justifyContent: "center", padding: "12px" }}
          >
            <i className="bx bx-upload"></i> Publicar Conteúdo
          </button>
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