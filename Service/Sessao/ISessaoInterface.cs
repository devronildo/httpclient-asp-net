using ProjectHttpClient.Models;

namespace ProjectHttpClient.Service.Sessao
{
    public interface ISessaoInterface 
    {
        UsuarioModel BuscarSessao();
        void CriarSessao(UsuarioModel usuarioModel);

        void RemoverSessao();
    }
}
