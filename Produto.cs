class Produto
{
    public static List<Produto> lista { get; set; }
    public int Codigo { get; set; }
    public string Nome { get; set; }

    static Produto(){
        lista = new List<Produto>();
        Produto.lista.AddRange(new List<Produto>{
            new Produto{Codigo=1, Nome="Banana"},
            new Produto{Codigo=2, Nome="Ameixa"},
            new Produto{Codigo=3, Nome="Maçã"},
            new Produto{Codigo=4, Nome="Pera"},
            new Produto{Codigo=5, Nome="Abacaxi"},
        });
    }
}