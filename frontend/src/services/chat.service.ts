import api from "./api";

export interface ChatPergunta {
  conteudoId: number;
  pergunta: string;
}

export interface ChatResposta {
  resposta: string;
  dataResposta: string;
  sucesso: boolean;
  mensagem: string;
}

export interface ChatContexto {
  conteudoId: number;
  status: string;
  tamanhoTokens: number;
  dataArmazenamento: string;
  temErro: boolean;
  mensagemErro?: string;
}

export const fazerPerguntaAI = async (
  conteudoId: number,
  pergunta: string
): Promise<ChatResposta> => {
  if (!pergunta.trim()) {
    throw new Error("Pergunta não pode estar vazia");
  }

  try {
    const response = await api.post<ChatResposta>(
      `/chatconteudo/${conteudoId}/perguntar`,
      {
        conteudoId,
        pergunta,
      }
    );

    return response.data;
  } catch (error: any) {
    const mensagem = error.response?.data?.message || "Erro ao fazer pergunta";
    throw new Error(mensagem);
  }
};

export const obterContextoConteudo = async (
  conteudoId: number
): Promise<ChatContexto> => {
  try {
    const response = await api.get<ChatContexto>(
      `/chatconteudo/${conteudoId}/contexto`
    );

    return response.data;
  } catch (error: any) {
    const mensagem = error.response?.data?.message || "Erro ao obter contexto";
    throw new Error(mensagem);
  }
};
