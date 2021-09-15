using System.Collections.Generic;
using ASPNET.Business.Notifications;

namespace ASPNET.Business.Interfaces
{
    public interface INotificador
    {
        bool TemNotificacao();

        List<Notificacao> ObterNotificacoes();

        void Handle(Notificacao notificacao);
    }
}