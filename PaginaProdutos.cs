using System.Text;

class PaginaProdutos : PaginaDinamica {
    public override byte[] Get(SortedList<string,string> param){
        string codigo = param.ContainsKey("id")?
            param["id"] : "";

        StringBuilder htmlGerado = new StringBuilder();
        foreach (var p in Produto.lista)
        {
            htmlGerado.Append("<tr>");
            if(!string.IsNullOrEmpty(codigo) && codigo == p.Codigo.ToString()){
                htmlGerado.Append($"<td><b>{p.Codigo:D4}</b></td>");
                htmlGerado.Append($"<td><b>{p.Nome}</b></td>");    
            }
            else{
                htmlGerado.Append($"<td>{p.Codigo:D4}</td>");
                htmlGerado.Append($"<td>{p.Nome}</td>");
            }
            htmlGerado.Append("</tr>");
        }
        string textoHtmlGerado = this.HtmlModelo.Replace("{{HtmlDinamico}}", htmlGerado.ToString());
        return Encoding.UTF8.GetBytes(textoHtmlGerado.ToString());
    }
}