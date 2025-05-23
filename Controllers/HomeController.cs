
using ProjectHttpClient.Dto.Login;
using ProjectHttpClient.Dto.Usuario;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ProjectHttpClient.Models;
using Newtonsoft.Json;
using System.Text;
using ProjectHttpClient.Service.Sessao;
using System.Net.Http.Headers;

namespace ProjectHttpClient.Controllers
{


    public class HomeController : Controller
    {
        Uri baseUrl = new Uri("https://localhost:7016/api");
        private readonly HttpClient _httpClient;
        private readonly ISessaoInterface _sessaoInterface;
        public HomeController(HttpClient httpClient, ISessaoInterface sessaoInterface)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = baseUrl;
            _sessaoInterface = sessaoInterface;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Logout()
        {
            _sessaoInterface.RemoverSessao();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> EditarUsuario(int id)
        {
            UsuarioModel usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                TempData["MensagemErro"] = "� necessario estar logado para acessar essa pagina";
                return RedirectToAction("Login");
            }
            ResponseModel<UsuarioModel> usuarioApi = new ResponseModel<UsuarioModel>();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, _httpClient.BaseAddress + "/Usuario/" + Convert.ToInt32(id)))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);

                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    usuarioApi = JsonConvert.DeserializeObject<ResponseModel<UsuarioModel>>(data);
                }

                var usuarioEdicaoDto = new UsuarioEdicaoDto
                {
                     Id = usuarioApi.Dados.Id,
                     Nome = usuarioApi.Dados.Nome,
                     Sobrenome = usuarioApi.Dados.Sobrenome,
                     Email = usuarioApi.Dados.Email,
                     Usuario = usuarioApi.Dados.Usuario,
                };

                return View(usuarioEdicaoDto);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ListarUsuarios()
        {
            UsuarioModel usuario = _sessaoInterface.BuscarSessao();
             
            if(usuario == null)
            {
                TempData["MensagemErro"] = "� necess�rio estar logado para acessar essa p�gina!";
                return RedirectToAction("Login");
            }
            ResponseModel<List<UsuarioModel>> usuarios = new ResponseModel<List<UsuarioModel>>();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, _httpClient.BaseAddress + "/Usuario"))
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", usuario.Token);

                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode) { 
                   var data = response.Content.ReadAsStringAsync().Result;
                   usuarios = JsonConvert.DeserializeObject<ResponseModel<List<UsuarioModel>>>(data);
                }

                return View(usuarios.Dados);
            }
        }

        [HttpGet]
        public async Task<IActionResult> RemoverUsuario(int id)
        {
            UsuarioModel usuario = _sessaoInterface.BuscarSessao();

            if (usuario == null)
            {
                TempData["MensagemErro"] = "� necess�rio estar logado para acessar essa p�gina!";
                return RedirectToAction("Login");
            }
            ResponseModel<UsuarioModel> usuarioApi = new ResponseModel<UsuarioModel>();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, _httpClient.BaseAddress + "/Usuario?id=" + Convert.ToInt32(id)))
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", usuario.Token);

                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    usuarioApi = JsonConvert.DeserializeObject<ResponseModel<UsuarioModel>>(data);
                }

                TempData["MensagemSucesso"] = usuarioApi.Mensagem;

                return RedirectToAction("ListarUsuarios");
            }
        }
        [HttpPost]

        public async Task<IActionResult> Login(UsuarioLoginDto usuarioLoginDto)
        {
            if (ModelState.IsValid)
            {
                ResponseModel<UsuarioModel> usuario = new ResponseModel<UsuarioModel>();

                var httpContent = new StringContent(JsonConvert.SerializeObject(usuarioLoginDto), Encoding.UTF8, "application/json"); //usuarioLoginDto em json
                
                HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Login/login", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    usuario =  JsonConvert.DeserializeObject<ResponseModel<UsuarioModel>>(data);
                 }
                if(usuario.Status == false)
                {
                     TempData["MensagemErro"] = "Credenciais inv�lidas!";
                     return View(usuarioLoginDto);
                }

                //criando sessao
                _sessaoInterface.CriarSessao(usuario.Dados);

                TempData["MensagemSucesso"] = "Usu�rio logado!";
                return RedirectToAction("ListarUsuarios");
            }
            else
            {
                return View(usuarioLoginDto);
            }
           
        }
        [HttpPost]
        public async Task<IActionResult> EditarUsuario(UsuarioEdicaoDto usuarioEdicaoDto)
        {
            UsuarioModel usuario = _sessaoInterface.BuscarSessao();

            if (usuario == null)
            {
                TempData["MensagemErro"] = "� necess�rio estar logado para acessar essa p�gina!";
                return RedirectToAction("Login");
            }

            ResponseModel<UsuarioModel> usuarioApi = new ResponseModel<UsuarioModel>();

            using(var requestMessage = new HttpRequestMessage(HttpMethod.Put, _httpClient.BaseAddress + "/Usuario"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);

                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(usuarioEdicaoDto), Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    usuarioApi = JsonConvert.DeserializeObject<ResponseModel<UsuarioModel>>(data);
                }
                TempData["MensagemSucesso"] = usuarioApi.Mensagem;
                return RedirectToAction("ListarUsuarios");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(UsuarioCriacaoDto usuarioCriacaoDto)
        {
           if(ModelState.IsValid)
            {
                ResponseModel<UsuarioModel> usuario = new ResponseModel<UsuarioModel>();

                var httpContent = new StringContent(JsonConvert.SerializeObject(usuarioCriacaoDto), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/login/register", httpContent);

                if (response.IsSuccessStatusCode)
                { 
                     var data = response.Content.ReadAsStringAsync().Result;
                     usuario = JsonConvert.DeserializeObject<ResponseModel<UsuarioModel>>(data);
                }
                if (usuario.Status == false) {
                    TempData["MensagemErro"] = "Ocorreu um erro ao realizar o processo!";
                    return View(usuarioCriacaoDto);
                }
                TempData["MensagemSucesso"] = usuario.Mensagem;
                return RedirectToAction("Login");
            }
            else
            {
                return View(usuarioCriacaoDto);
            }
        }




    }
}
