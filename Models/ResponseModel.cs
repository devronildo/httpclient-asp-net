namespace ProjectHttpClient.Models
{
    public class ResponseModel<T> //T dado generico
    {
        public T? Dados { get; set; }

        public string Mensagem { get; set; } = string.Empty;

        public bool Status { get; set; } = true;
    }
}
