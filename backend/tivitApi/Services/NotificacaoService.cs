using Microsoft.EntityFrameworkCore;
using tivitApi.Data;
using tivitApi.DTOs;
using tivitApi.Models;
using tivitApi.Exceptions;

namespace tivitApi.Services
{
    public interface INotificacaoService
    {
        Task<Notificacao> CriarNotificacao(NotificacaoDTORequest notificacaoDTO);

    }

    public class NotificacaoService : INotificacaoService
    {
        private readonly AppDbContext _context;

        public NotificacaoService(AppDbContext context)
        {
            _context = context;

        }


        public async Task<Notificacao> CriarNotificacao(NotificacaoDTORequest notificacaoDTO)
        {
            // Verifica se todas as turmas informadas existem
            var turmas = await _context.Turmas
                .Where(t => notificacaoDTO.TurmasIds.Contains(t.Id))
                .ToListAsync();

            if (turmas.Count != notificacaoDTO.TurmasIds.Count)
                throw new Exception("Uma ou mais turmas informadas não foram encontradas.");

            var notificacao = new Notificacao(
                notificacaoDTO.Titulo,
                notificacaoDTO.Descricao
            );

            notificacao.NotificacaoTurmas = notificacaoDTO.TurmasIds.Select(turmaId => new NotificacaoTurma
            {
                TurmaId = turmaId
            }).ToList();

            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();

            return notificacao;
        }

    }
}