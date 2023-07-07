using System.Text;

class PaginaCadastroProduto: PaginaDinamica{
    public byte[] Post(SortedList<string,string> param){
        Produto produto = new Produto();

        produto.Codigo = param.ContainsKey("codigo") ? 
            Convert.ToInt32(param["codigo"]) : produto.Codigo = 0;
        produto.Nome = param.ContainsKey("nome") ? 
            param["nome"] : "";
        if(produto.Codigo>0){
            Produto.lista.Add(produto);
        }
        string html = "<script>window.location.replace(\"produtos.dhtml\")</script>";
        return Encoding.UTF8.GetBytes(html.ToString());
    }

}