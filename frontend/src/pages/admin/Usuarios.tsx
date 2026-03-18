import { useState } from "react";
import { Modal } from "../../components/common/Modal";
import { buscarUsuarioPorCpf, buscarUsuariosPorNome, ativarUsuario, desativarUsuario } from "../../services/admin.service";
import type { UsuarioAdmin } from "../../types";

export default function Usuarios() {
  const [busca, setBusca] = useState("");
  const [usuarios, setUsuarios] = useState<UsuarioAdmin[]>([]);
  const [modal, setModal] = useState({ open: false, titulo: "", mensagem: "" });
  const [confirmModal, setConfirmModal] = useState<{ open: boolean; onConfirm: () => void; mensagem: string }>({ open: false, onConfirm: () => {}, mensagem: "" });

  async function buscar() {
    if (!busca.trim()) return;
    try {
      const isNumero = /^\d+$/.test(busca.trim());
      const dados = isNumero
        ? await buscarUsuarioPorCpf(busca.trim())
        : await buscarUsuariosPorNome(busca.trim());
      setUsuarios(dados);
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Usuário não encontrado." }); }
  }

  async function alterarStatus(usuario: UsuarioAdmin) {
    const isAtivo = usuario.status === "ATIVO";
    try {
      if (isAtivo) {
        await desativarUsuario(usuario.cpf, usuario.tipo);
      } else {
        await ativarUsuario(usuario.cpf, usuario.tipo);
      }
      setUsuarios(prev => prev.map(u =>
        u.cpf === usuario.cpf ? { ...u, status: isAtivo ? "INATIVO" : "ATIVO" } : u
      ));
      setModal({ open: true, titulo: "Sucesso", mensagem: `Usuário ${isAtivo ? "desativado" : "ativado"} com sucesso!` });
    } catch { setModal({ open: true, titulo: "Erro", mensagem: "Não foi possível alterar o status." }); }
  }

  return (
    <section className="prof-section">
      <h2><i className="bx bx-file"></i> Usuários</h2>

      <div className="boletim-container">
        <div className="search-box">
          <input
            type="text"
            placeholder="Pesquise pelo nome ou CPF"
            value={busca}
            onChange={e => setBusca(e.target.value)}
            onKeyDown={e => e.key === "Enter" && buscar()}
          />
          <button onClick={buscar}><i className="bx bx-search"></i> Buscar</button>
        </div>

        {usuarios.length > 0 && (
          <div className="resultado-box">
            <h3>Resultados da busca</h3>
            {usuarios.map((u, i) => (
              <div key={i} className="usuario-card">
                <p><strong>Nome:</strong> {u.nome}</p>
                <p><strong>Email:</strong> {u.email}</p>
                <p><strong>CPF:</strong> {u.cpf}</p>
                <p><strong>Tipo:</strong> {u.tipo}</p>
                <p><strong>Status:</strong> {u.status}</p>
                <i
                  className={`bx ${u.status === "ATIVO" ? "bx-user-x" : "bx-user-check"} btn-acao-usuario ${u.status === "ATIVO" ? "bx-user-x" : "bx-user-check"}`}
                  title={u.status === "ATIVO" ? "Desativar" : "Ativar"}
                  onClick={() => setConfirmModal({
                    open: true,
                    mensagem: u.status === "ATIVO" ? "Deseja desativar este usuário?" : "Deseja ativar este usuário?",
                    onConfirm: () => { alterarStatus(u); setConfirmModal(c => ({ ...c, open: false })); }
                  })}
                  style={{ fontSize: "22px", cursor: "pointer", color: u.status === "ATIVO" ? "#e74c3c" : "#27ae60" }}
                ></i>
                <hr />
              </div>
            ))}
          </div>
        )}
      </div>

      <Modal isOpen={modal.open} onClose={() => setModal({ open: false, titulo: "", mensagem: "" })} title={modal.titulo}>
        <p>{modal.mensagem}</p>
        <button className="btn-primary" style={{ width: "100%", marginTop: "12px" }} onClick={() => setModal({ open: false, titulo: "", mensagem: "" })}>OK</button>
      </Modal>

      <Modal isOpen={confirmModal.open} onClose={() => setConfirmModal(c => ({ ...c, open: false }))} title="Confirmação">
        <p>{confirmModal.mensagem}</p>
        <div style={{ display: "flex", gap: "10px", marginTop: "16px" }}>
          <button className="btn-primary" style={{ flex: 1 }} onClick={confirmModal.onConfirm}>Confirmar</button>
          <button className="btn-secondary" style={{ flex: 1 }} onClick={() => setConfirmModal(c => ({ ...c, open: false }))}>Cancelar</button>
        </div>
      </Modal>
    </section>
  );
}