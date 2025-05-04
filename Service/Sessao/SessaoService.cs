using Newtonsoft.Json;
using ProjectHttpClient.Models;

namespace ProjectHttpClient.Service.Sessao
{
    public class SessaoService : ISessaoInterface
    {
        private readonly IHttpContextAccessor _contextAcessor;
        public SessaoService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAcessor = httpContextAccessor;
        }

        public UsuarioModel BuscarSessao()
        {
          string sessaoUsuario = _contextAcessor.HttpContext.Session.GetString("SessaoUsuario");
            if (string.IsNullOrEmpty(sessaoUsuario))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);
          
        }

        public void CriarSessao(UsuarioModel usuarioModel)
        {
            string usuarioJson = JsonConvert.SerializeObject(usuarioModel);
            _contextAcessor.HttpContext.Session.SetString("SessaoUsuario", usuarioJson);   
        }

        public void RemoverSessao()
        {
            _contextAcessor.HttpContext.Session.Remove("SessaoUsuario");
        }
    }
}
