import { useState, useRef, useEffect } from "react";
import { fazerPerguntaAI } from "../../services/chat.service";
import "./ChatConteudo.css";
import taiIcon from "../../assets/TAI.png";

interface Mensagem {
  id: number;
  tipo: "pergunta" | "resposta" | "erro";
  conteudo: string;
  timestamp: Date;
}

interface ChatConteudoProps {
  conteudoId: number;
  tituloConteudo: string;
  isOpen: boolean;
  onClose: () => void;
}

export default function ChatConteudo({
  conteudoId,
  tituloConteudo,
  isOpen,
  onClose,
}: ChatConteudoProps) {
  const [mensagens, setMensagens] = useState<Mensagem[]>([]);
  const [pergunta, setPergunta] = useState("");
  const [carregando, setCarregando] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const [idContador, setIdContador] = useState(0);

  // Auto-scroll para última mensagem
  const scrollParaBaixo = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    scrollParaBaixo();
  }, [mensagens]);

  const adicionarMensagem = (tipo: "pergunta" | "resposta" | "erro", conteudo: string) => {
    const nova: Mensagem = {
      id: idContador,
      tipo,
      conteudo,
      timestamp: new Date(),
    };
    setMensagens((prev) => [...prev, nova]);
    setIdContador((prev) => prev + 1);
  };

  const handleEnviar = async () => {
    if (!pergunta.trim()) return;

    // Adicionar pergunta
    adicionarMensagem("pergunta", pergunta);
    const perguntaTemp = pergunta;
    setPergunta("");
    setCarregando(true);

    try {
      const resposta = await fazerPerguntaAI(conteudoId, perguntaTemp);

      if (resposta.sucesso) {
        adicionarMensagem("resposta", resposta.resposta);
      } else {
        adicionarMensagem("erro", resposta.mensagem || "Erro ao gerar resposta");
      }
    } catch (erro: any) {
      adicionarMensagem("erro", erro.message || "Erro ao comunicar com o servidor");
    } finally {
      setCarregando(false);
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter" && !e.shiftKey && !carregando) {
      e.preventDefault();
      handleEnviar();
    }
  };

  const limparChat = () => {
    setMensagens([]);
    setPergunta("");
  };

  if (!isOpen) return null;

  return (
    <div className="chat-container">
      <div className="chat-header">
        <div className="chat-header-content">
          <img
            src={taiIcon}
            alt="TAI"
            className="chat-tai-icon"
          />
          <div>
            <h3 className="chat-title">TAI - Assistente IA</h3>
            <p className="chat-subtitle">{tituloConteudo}</p>
          </div>
        </div>
        <div className="chat-header-actions">
          <button
            className="chat-btn-limpar"
            onClick={limparChat}
            title="Limpar histórico"
            disabled={carregando}
            style={{ display: "flex", alignItems: "center", justifyContent: "center", padding: "6px" }}
          >
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <path d="M3 6h18"/>
              <path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"/>
              <path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/>
              <line x1="10" x2="10" y1="11" y2="17"/>
              <line x1="14" x2="14" y1="11" y2="17"/>
            </svg>
          </button>
          <button
            className="chat-btn-fechar"
            onClick={onClose}
            disabled={carregando}
          >
            ✕
          </button>
        </div>
      </div>

      <div className="chat-messages">
        {mensagens.length === 0 ? (
          <div className="chat-vazio">
            <p>👋 Olá! Sou o TAI, seu assistente de IA.</p>
            <p>Faça perguntas sobre este conteúdo e responderei baseado no material.</p>
          </div>
        ) : (
          <>
            {mensagens.map((msg) => (
              <div
                key={msg.id}
                className={`chat-mensagem chat-${msg.tipo}`}
              >
                <div className="chat-mensagem-conteudo">
                  {msg.tipo === "pergunta" && <span className="chat-label">Você:</span>}
                  {msg.tipo === "resposta" && <span className="chat-label">TAI:</span>}
                  {msg.tipo === "erro" && <span className="chat-label">⚠️ Erro:</span>}
                  <p>{msg.conteudo}</p>
                </div>
                <span className="chat-timestamp">
                  {msg.timestamp.toLocaleTimeString("pt-BR", {
                    hour: "2-digit",
                    minute: "2-digit",
                  })}
                </span>
              </div>
            ))}
            <div ref={messagesEndRef} />
            {carregando && (
              <div className="chat-mensagem chat-resposta chat-carregando">
                <div className="chat-mensagem-conteudo">
                  <span className="chat-label">TAI:</span>
                  <div className="typing-animation">
                    <span></span>
                    <span></span>
                    <span></span>
                  </div>
                </div>
              </div>
            )}
          </>
        )}
      </div>

      <div className="chat-input-area">
        <textarea
          value={pergunta}
          onChange={(e) => setPergunta(e.target.value)}
          onKeyPress={handleKeyPress}
          placeholder="Digite sua pergunta... (Enter para enviar)"
          disabled={carregando}
          className="chat-input"
          rows={2}
        />
        <button
          onClick={handleEnviar}
          disabled={carregando || !pergunta.trim()}
          className="chat-btn-enviar"
        >
          {carregando ? "⏳ Enviando..." : "Enviar"}
        </button>
      </div>
    </div>
  );
}
