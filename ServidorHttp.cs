using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Program{

    public class ServidorHttp{

        private TcpListener controller { get;set; }

        private int port { get;set; }
        private int qtdRequests { get;set; }
        private SortedList<string,string> TiposMime;
        public ServidorHttp(int port = 8080)
        {
            this.port = port;
            this.PopularTiposMime();
            try{
                this.controller = new TcpListener(IPAddress.Parse("127.0.0.1"), this.port);
                this.controller.Start();  
                Console.WriteLine($"Servidor iniciado na porta{this.port}");
                Console.WriteLine($"Para acessar, digite no navegar http://localhost:{this.port}");

                Task httpServerTask = Task.Run(() => WaitRequest());  
                httpServerTask.GetAwaiter().GetResult();
            }
            catch(Exception e){
                Console.WriteLine($"Não foi possível iniciar o servidor na porta {this.port}", e.Message);
            }
        }

        private async Task WaitRequest()
        {
            while(true)
            {
                Socket conection = await this.controller.AcceptSocketAsync();
                this.qtdRequests++;


                Task task = Task.Run(() => RequestProcess(conection, qtdRequests));
            }
        }

        private void RequestProcess(Socket conection, int requestNumber)
        {
            Console.WriteLine($"Request Process: {requestNumber}");
            if(conection.Connected)
            {
                byte[] requestBytes = new byte[1024];
                conection.Receive(requestBytes, requestBytes.Length, 0);
                string requestText = Encoding.UTF8.GetString(requestBytes)
                        .Replace((char)0, ' ').Trim();
                if(requestText.Length > 0)
                {
                    Console.WriteLine($"\n{requestText}\n");
                    string[] linhas = requestText.Split("\r\n");
                    int iPrimeiroEspaco = linhas[0].IndexOf(' ');
                    int iSegundoEspaco = linhas[0].LastIndexOf(' ');
                    string metodoHttp = linhas[0].Substring(0, iPrimeiroEspaco);
                    string recursoBuscado = linhas[0].Substring(iPrimeiroEspaco+1, 
                        iSegundoEspaco - iPrimeiroEspaco -1);
                    string versaoHttp = linhas[0].Substring(iSegundoEspaco+1);
                    
                    int iTerceiroEspaço = linhas[1].IndexOf(' ');
                    string hostName = linhas[1].Substring(iTerceiroEspaço+1);


                    byte[] contentBytes = null;

                    byte[] bytesHeader = null;

                    FileInfo fiArquivo = new FileInfo(ObterCaminhoArquivo(recursoBuscado));
                    if(fiArquivo.Exists)
                    {
                        if(TiposMime.ContainsKey(fiArquivo.Extension.ToLower()))
                        {
                            contentBytes = File.ReadAllBytes(fiArquivo.FullName);
                            string tipoMime = TiposMime[fiArquivo.Extension.ToLower()];
                            bytesHeader = CreateHeader(versaoHttp, tipoMime,
                            "200",contentBytes.Length);
                        }
                        else{
                            contentBytes = Encoding.UTF8.GetBytes(
                                "Erro! Tipo mime não suportado!");
                            bytesHeader = CreateHeader(versaoHttp, "text/html;charset=utf-8"
                                , "415", contentBytes.Length);
                        }
                    }
                    else{
                        contentBytes = Encoding.UTF8.GetBytes(
                            "<h1>Erro 404 - Arquivo não encontrado</h1>");
                        bytesHeader = CreateHeader(versaoHttp, "text/html;charset=utf-8",
                            "404", contentBytes.Length);
                    }
                    
                    int sentBytes = conection.Send(bytesHeader, bytesHeader.Length, 0);

                    sentBytes += conection.Send(contentBytes, contentBytes.Length, 0);
                    conection.Close();
                    Console.WriteLine($"\n{sentBytes} bytes enviados em resposta à requisição #{requestNumber}");
                }
            }
            Console.WriteLine($"Request {requestNumber} processed");
        }

        public byte[] CreateHeader(string versaoHttp, string tipoMime,
            string codigoHttp, int qtdBytes = 0)
        {
            StringBuilder text = new StringBuilder();
            text.Append($"{versaoHttp} {codigoHttp}{Environment.NewLine}");
            text.Append($"Server: SevidorHttpSimples 1.0{Environment.NewLine}");
            text.Append($"Content-type: {tipoMime}{Environment.NewLine}");
            text.Append($"Content-Length: {qtdBytes}{Environment.NewLine}{Environment.NewLine}");
            return Encoding.UTF8.GetBytes(text.ToString());
        }
        private string ObterCaminhoArquivo(string arquivo)
        {
            string caminhoArquivo = "U:\\GitHub\\Servidor-Http\\www" + arquivo.Replace("/", "\\");
            return caminhoArquivo;
        }

        private void PopularTiposMime()
        {
            TiposMime = new SortedList<string, string>();
            this.TiposMime.Add(".html", "text/html;charset=utf-8");
            this.TiposMime.Add(".htm", "text/html;charset=utf-8");
            this.TiposMime.Add(".css", "text/css");
            this.TiposMime.Add(".js", "text/javascript");
            this.TiposMime.Add(".png", "image/png");
            this.TiposMime.Add(".jpg", "image/jpg");
            this.TiposMime.Add(".gif", "image/gif");
            this.TiposMime.Add(".svg", "image/svg+xml");
            this.TiposMime.Add(".webp", "image/webp");
            this.TiposMime.Add(".ico", "image/ico");
            this.TiposMime.Add(".woff", "image/woff");
            this.TiposMime.Add(".woff2", "image/woff2");
        }
    }
}