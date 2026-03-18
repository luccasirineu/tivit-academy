import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import api from '../../services/api';
import { login as apiLogin } from '../../services/auth.service';

export function LoginForm() {
  const [tipo, setTipo] = useState('');
  const [usuario, setUsuario] = useState('');
  const [senha, setSenha] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [showEsqueciSenha, setShowEsqueciSenha] = useState(false);
  const [cpfRecuperacao, setCpfRecuperacao] = useState('');
  const [recuperacaoMsg, setRecuperacaoMsg] = useState('');
  const [recuperacaoErro, setRecuperacaoErro] = useState('');
  const [isRecuperando, setIsRecuperando] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!tipo || !usuario || !senha) {
      setError('Por favor, preencha todos os campos.');
      return;
    }

    setIsSubmitting(true);
    try {
      await login({ Tipo: tipo, Cpf: usuario, Senha: senha });

      if (tipo === "professor") navigate("/professor", { replace: true });
      else if (tipo === "administrador") navigate("/admin/dashboard", { replace: true });
      else navigate("/aluno/dashboard", { replace: true });
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Erro desconhecido.';
      setError(message);
    } finally {
      setIsSubmitting(false);
    }
  };

  async function handleRecuperarSenha() {
    setRecuperacaoErro('');
    setRecuperacaoMsg('');

    if (!cpfRecuperacao.trim()) {
      setRecuperacaoErro('Por favor, informe o CPF.');
      return;
    }

    setIsRecuperando(true);
    try {
      await api.patch(`/Aluno/recuperarSenha/${cpfRecuperacao.replace(/\D/g, '')}`);
      setRecuperacaoMsg('As instruções de recuperação foram enviadas para o seu e-mail cadastrado.');
      setCpfRecuperacao('');
    } catch {
      setRecuperacaoErro('CPF não encontrado ou erro ao processar a solicitação.');
    } finally {
      setIsRecuperando(false);
    }
  }

  function fecharEsqueciSenha() {
    setShowEsqueciSenha(false);
    setCpfRecuperacao('');
    setRecuperacaoMsg('');
    setRecuperacaoErro('');
  }

  function formatarCpf(valor: string) {
  const numeros = valor.replace(/\D/g, '').slice(0, 11);
  return numeros
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
}

  return (
    <>
      <form id="loginForm" onSubmit={handleSubmit}>
        <label htmlFor="tipo">TIPO DE LOGIN</label>
        <select
          id="tipo"
          name="tipo"
          required
          value={tipo}
          onChange={(e) => setTipo(e.target.value)}
        >
          <option value="" disabled>Selecione...</option>
          <option value="aluno">Aluno</option>
          <option value="professor">Professor</option>
          <option value="administrador">Administrador</option>
        </select>

        <label htmlFor="usuario">USUÁRIO</label>
        <input
          type="text"
          id="usuario"
          name="usuario"
          required
          value={usuario}
          onChange={(e) => setUsuario(e.target.value)}
        />

        <label htmlFor="senha">Senha</label>
        <div className="senha-wrapper">
          <input
            type={showPassword ? 'text' : 'password'}
            id="senha"
            required
            value={senha}
            onChange={(e) => setSenha(e.target.value)}
          />
          <i
            className={`bx ${showPassword ? 'bx-show' : 'bx-hide'} toggle-senha`}
            onClick={() => setShowPassword(!showPassword)}
          ></i>
        </div>

        {error && <p style={{ color: '#ff3380', marginTop: '10px' }}>{error}</p>}

        <button type="submit" className="btn" disabled={isSubmitting}>
          {isSubmitting ? 'CONECTANDO...' : 'CONECTAR'}
        </button>
        <a
          href="#"
          className="forgot"
          onClick={(e) => { e.preventDefault(); setShowEsqueciSenha(true); }}>
        
          ESQUECI MINHA SENHA
        </a>
      </form>

      {showEsqueciSenha && (
        <div className="popup-overlay" onClick={fecharEsqueciSenha}>
          <div className="popup-content" onClick={(e) => e.stopPropagation()}>
            <h3><i className="bx bx-lock-open-alt"></i> Recuperar Senha</h3>

            {recuperacaoMsg ? (
              <>
                <p style={{ color: '#00c96e', fontSize: '14px', lineHeight: 1.6 }}>
                  <i className="bx bx-check-circle" style={{ marginRight: '6px' }}></i>
                  {recuperacaoMsg}
                </p>
                <div className="popup-actions">
                  <button
                    className="btn-accent"
                    style={{ width: '100%', justifyContent: 'center' }}
                    onClick={fecharEsqueciSenha}
                  >
                    OK
                  </button>
                </div>
              </>
            ) : (
              <>
                <label>CPF cadastrado</label>
                <input
                  type="text"
                  placeholder="000.000.000-00"
                  value={cpfRecuperacao}
                  onChange={(e) => setCpfRecuperacao(formatarCpf(e.target.value))}
                />

                {recuperacaoErro && (
                  <p style={{ color: '#ff3380', fontSize: '13px', margin: '4px 0' }}>
                    {recuperacaoErro}
                  </p>
                )}

                <div className="popup-actions">
                  <button type="button" className="btn-cancelar" onClick={fecharEsqueciSenha}>
                    Cancelar
                  </button>
                  <button
                    type="button"
                    className="btn-accent"
                    disabled={isRecuperando}
                    onClick={handleRecuperarSenha}
                  >
                    {isRecuperando ? 'Enviando...' : <><i className="bx bx-send"></i> Enviar</>}
                  </button>
                </div>
              </>
            )}
          </div>
        </div>
      )}
    </>
  );
}