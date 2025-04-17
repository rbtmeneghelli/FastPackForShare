using FastPackForShare.Models;

namespace FastPackForShare.Extensions;

public sealed class LinqExtensionMethod
{
    #region Função de agregação do LINQ (Metodo útil para somar, concatenar ou calcular médias agrupadas.)

    public string AgregateStrings(IEnumerable<string> source)
    {
        return source.Aggregate((item, itemNext) => item + "," + itemNext);
    }

    public string JoinStrings(IEnumerable<string> source)
    {
        return string.Join(",", source);
    }

    public int AgregateSum(IEnumerable<int> source)
    {
        return source.Aggregate((item, itemNext) => item + itemNext);
    }

    public decimal AgregateAverage(IEnumerable<int> source)
    {
        return source.Aggregate(
            seed: 0,
            func: (result, item) => result + item,
            resultSelector: result => (decimal)(result / source.Count())
        );
    }

    public string AggregateByBeforeNET9()
    {
        StringBuilder sb = new StringBuilder();

        // Array de Tuplas
        (string nome, string departamento, int diasFerias)[] funcionarios =
        [
           ("João Duda", "IT", 12),
       ("Jane Soares", "Marketing", 18),
       ("Jose Silva", "IT", 28),
       ("Maria Fernandez", "RH", 17),
       ("Nivia Maria", "Marketing", 5),
       ("Maria Moreti", "RH", 9)
        ];

        var diasFeriasDepartamento = funcionarios
        .GroupBy(funci => funci.departamento)
        .ToDictionary(group => group.Key, group => group.Sum(funci => funci.diasFerias))
        .AsEnumerable();

        foreach (var entry in diasFeriasDepartamento)
            sb.AppendLine($"O Departamento {entry.Key} possui um total de {entry.Value} dias de férias a cumprir.");

        return sb.ToString();
    }

    //public string AggregateByNET9()
    //{
    //    StringBuilder sb = new StringBuilder();

    //    // Array de Tuplas
    //    (string nome, string departamento, int diasFerias)[] funcionarios =
    //    [
    //       ("João Duda", "IT", 12),
    //       ("Jane Soares", "Marketing", 18),
    //       ("Jose Silva", "IT", 28),
    //       ("Maria Fernandez", "RH", 17),
    //       ("Nivia Maria", "Marketing", 5),
    //       ("Maria Moreti", "RH", 9)
    //    ];

    //    //var diasFeriasDepartamento = funcionarios
    //    //.AggregateBy(emp => emp.departamento, 0, (acc, funci) => acc + funci.diasFerias);

    //    foreach (var entry in diasFeriasDepartamento)
    //        sb.AppendLine($"O Departamento {entry.Key} possui um total de {entry.Value} dias de férias a cumprir.");

    //    return sb.ToString();
    //}

    #endregion

    #region Funções de Quantificadores 

    public bool ValidateAllElements<T>(IEnumerable<T> source, Func<T, bool> predicate)
    {
        // Se todos os Elementos Atender a condição predicate, será retornado TRUE. Senão FALSE
        return source.All(predicate); // predicate => x => x % 2 == 0
    }

    public bool ExistAnyElements<T>(List<T> source, Predicate<T> predicate)
    {
        // Se existir um Elemento que Atenda a condição predicate, será retornado TRUE. Senão FALSE
        return source.Exists(predicate); // predicate => x => x % 2 == 0
    }

    #endregion

    #region Função para retornar quantidade de dados dentro de uma lista ou array

    public int GetQtdItensFromList<T>(IEnumerable<T> list, Func<T, bool> predicate) where T : class
    {
        if (GuardClauseExtension.IsNull(predicate))
            return list.Count();
        else
            return list.Count(predicate);
    }

    public long GetQtdItensFromBigList<T>(IEnumerable<T> list, Func<T, bool> predicate) where T : class
    {
        if (GuardClauseExtension.IsNull(predicate))
            return list.LongCount();
        else
            return list.LongCount(predicate);
    }

    public string CountByBeforeNET9()
    {
        StringBuilder sb = new StringBuilder();

        // Array de Tuplas
        (string nome, string sobrenome)[] pessoas =
        [
           ("João", "Donato"),
       ("Janice", "Silva"),
       ("João", "Sanches"),
       ("Maria", "Silveira"),
       ("Pedro", "Sobrinho"),
       ("Janice", "Fernandez"),
       ("Maria", "Moretii")
        ];

        var contaNomes = pessoas
           .GroupBy(p => p.nome)
           .ToDictionary(group => group.Key, group => group.Count())
           .AsEnumerable();

        foreach (var entry in contaNomes)
        {
            sb.AppendLine($"O Nome {entry.Key} aparece {entry.Value} vezes");
        }

        return sb.ToString();
    }

    //public string CountByNET9()
    //{
    //    StringBuilder sb = new StringBuilder();

    //    // Array de Tuplas
    //    (string nome, string sobrenome)[] pessoas =
    //    [
    //       ("João", "Donato"),
    //       ("Janice", "Silva"),
    //       ("João", "Sanches"),
    //       ("Maria", "Silveira"),
    //       ("Pedro", "Sobrinho"),
    //       ("Janice", "Fernandez"),
    //       ("Maria", "Moretii")
    //    ];

    //    var contaNomes = pessoas.CountBy(p => p.nome);

    //    foreach (var entry in contaNomes)
    //    {
    //        sb.Append($"O Nome {entry.Key} aparece {entry.Value} vezes");
    //    }

    //    return sb.ToString();
    //}

    //public (string resultado, int total) GetCountBy(List<DropDownList> list)
    //{
    //    var result = list.CountBy(p => p.Description);
    //    return (result.Key, result.Value);
    //}

    #endregion;

    #region Função para obter o index dos valores de uma lista ou array

    public string GetIndexBeforeNET9()
    {
        StringBuilder sb = new StringBuilder();

        var alunos = new[]
        {
       "Jose Sanches",
       "Janice Pereira",
       "Carlos Nogueira",
       "João Silveira"
    };

        // antes
        foreach (var (index, aluno) in alunos.Select((m, i) => (i, m)))
            sb.AppendLine($"Aluno {index}: {aluno}");

        return sb.ToString();
    }

    //public string GetIndexNET9()
    //{
    //    StringBuilder sb = new StringBuilder();

    //    var alunos = new[]
    //    {
    //       "Jose Sanches",
    //       "Janice Pereira",
    //       "Carlos Nogueira",
    //       "João Silveira"
    //    };

    //    // antes
    //    foreach (var (index, aluno) in alunos.Index())
    //        Console.WriteLine($"Aluno {index}: {aluno}");

    //    return sb.ToString();
    //}

    //public (string resultado, int total) GetIndexFromList(List<DropDownList> list)
    //{
    //    var result = list.Index();
    //    return result;
    //}

    #endregion

    #region Função de ordenação do LINQ

    public IEnumerable<T> GetListOrderAsc<T>(IEnumerable<T> list)
    {
        return list.Order().ToList();
    }

    public IEnumerable<DropDownListModel> GetListOrderByAsc(IEnumerable<DropDownListModel> list)
    {
        return list.OrderBy(x => x.Id).ToList();
    }

    public IEnumerable<DropDownListModel> GetListOrderByDesc(IEnumerable<DropDownListModel> list)
    {
        return list.OrderByDescending(x => x.Id).ToList();
    }

    public IEnumerable<T> GetListReverse<T>(IEnumerable<T> list)
    {
        list.Reverse(); // Faz a inversão da ordem dos valores de uma lista
        return list;
    }

    public IEnumerable<T> GetListSortAsc<T>(List<T> list)
    {
        list.Sort(); // Faz a ordenação da lista em ordem crescente, seguindo o algoritmo de quicksort
        return list;
    }

    #endregion

    #region Remove os duplicados da lista source

    public IEnumerable<DropDownListModel> GetDistinctBy(IEnumerable<DropDownListModel> source)
    {
        return source.DistinctBy(p => p.Description, StringComparer.OrdinalIgnoreCase).ToList();
    }

    #endregion

    #region Retorna os elementos da lista source que não estão na lista itens

    public IEnumerable<DropDownListModel> GetExceptBy(IEnumerable<DropDownListModel> source, IEnumerable<string> itens)
    {
        return source.ExceptBy(itens, p => p.Description, StringComparer.OrdinalIgnoreCase).ToList();
    }

    #endregion

    #region Retorna os elementos que são comuns em ambas as listas

    public IEnumerable<DropDownListModel> GetIntersectBy(IEnumerable<DropDownListModel> source, IEnumerable<DropDownListModel> itens)
    {
        return source.IntersectBy(itens.Select(x => x.Description), p => p.Description, StringComparer.OrdinalIgnoreCase).ToList();
    }

    #endregion

    #region Faz a junção de ambos os conjuntos, sem geração de duplicidade

    public IEnumerable<DropDownListModel> GetUnionBy(IEnumerable<DropDownListModel> source, IEnumerable<DropDownListModel> itens)
    {
        return source.UnionBy(itens, p => p.Description, StringComparer.OrdinalIgnoreCase).ToList();
    }

    #endregion

    public DropDownListModel GetMinValueFromList(IEnumerable<DropDownListModel> list)
    {
        return list.MinBy(x => x.Id);
    }

    public DropDownListModel GetMaxValueFromList(IEnumerable<DropDownListModel> list)
    {
        return list.MaxBy(x => x.Id);
    }

    public Dictionary<long, string> ConvertListToDictionary(IEnumerable<DropDownListModel> list)
    {
        return list.ToDictionary(item => item.Id, item => item.Description);
    }

    public T GetFirstItemFromList<T>(IEnumerable<T> list, Func<T, bool> predicate) where T : class
    {
        if (GuardClauseExtension.IsNull(predicate))
            return list.FirstOrDefault();
        else
            return list.FirstOrDefault(predicate);
    }

    public T GetLastItemFromList<T>(IEnumerable<T> list, Func<T, bool> predicate) where T : class
    {
        if (GuardClauseExtension.IsNull(predicate))
            return list.LastOrDefault();
        else
            return list.LastOrDefault(predicate);
    }

    public decimal GetTotalItensFromList<T>(IEnumerable<T> list, Func<T, decimal> predicate) where T : class
    {
        return list.Sum(predicate);
    }

    public IEnumerable<T> GetFirstItensFromList<T>(IEnumerable<T> list, int qtyItens) where T : class
    {
        return list.Take(qtyItens);
    }

    public IEnumerable<T> GetLastItensFromList<T>(IEnumerable<T> list, int qtyItens) where T : class
    {
        return list.TakeLast(qtyItens);
    }

    public IEnumerable<T> RemoveItemFromList<T>(List<T> list, T item) where T : class
    {
        list.Remove(item);
        return list;
    }

    public IEnumerable<T> RemoveAtItemFromList<T>(List<T> list, Predicate<T> predicate) where T : class
    {
        list.RemoveAll(predicate);
        return list;
    }

    public T GetElementFromListByIndex<T>(IEnumerable<T> list, int index)
    {
        return list.ElementAtOrDefault(index);
    }

    public IEnumerable<T> AddItemOnFirstPlaceOfList<T>(IEnumerable<T> source, T item)
    {
        var newSource = source.Prepend<T>(item).ToList();
        return newSource;
    }

    public IEnumerable<T> AddItemOnLastPlaceOfList<T>(IEnumerable<T> source, T item)
    {
        var newSource = source.Append<T>(item).ToList();
        return newSource;
    }

    public List<string> ZipList(List<int> sourceId, List<string> sourceText)
    {
        // Se tiver a mesma quantidade de itens uma lista e a outra, vai combinar um resultado final numa nova lista
        var newSource = sourceId.Zip(sourceText, (Id, Text) => Id + " - " + Text).ToList();
        return newSource;
    }

    public bool ListItensIsAllOk(List<DropDownListModel> source, Func<DropDownListModel, bool> predicate)
    {
        return source.TrueForAll(x => x.Id > 0);
    }

    public IEnumerable<DropDownListModel> GetChunkList(IEnumerable<DropDownListModel> list, int pageIndex, int pageSize = 10)
    {
        return list.Chunk(pageSize).ElementAt(pageIndex).AsEnumerable();
    }

    #region Lista do tipo IEnumerable é para somente leitura, onde não é possivel alterar os valores originais

    public IEnumerable<T> ConvertArrInIEnumerable<T>(T[] array) => array.AsEnumerable();

    public IEnumerable<T> ConvertListInIEnumerable<T>(List<T> list) => list.AsEnumerable();

    #endregion
}
