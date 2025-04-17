using System.Data;

namespace FastPackForShare.Extensions;

public class ListExtensionMethod
{
    public Queue<T> ConvertListToQueue<T>(IEnumerable<T> list) => new Queue<T>(list);

    /// <summary>
    /// Faz a combinação dos itens da list1 e list2, em uma unica lista
    /// Esse procedimento a partir do NET 12 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list1"></param>
    /// <param name="list2"></param>
    /// <returns></returns>
    public IEnumerable<T> CombineLists<T>(IEnumerable<T> list1, IEnumerable<T> list2) => [.. list1, .. list2];

    public T[] GetAllElements<T>(T[] list) => list[Range.All];

    public T[] GetAllElementsNFirsts<T>(T[] list, int numberOfElements) => list[..numberOfElements];

    public T[] GetAllElementsNLasts<T>(T[] list, int numberOfElements) => list[..^numberOfElements];

    public T[] GetAllElementsAfterNFirst<T>(T[] list, int numberOfElements) => list[numberOfElements..];

    public T[] GetAllElementsAfterNLast<T>(T[] list, int numberOfElements) => list[^numberOfElements..];

    public T[] GetReverseArray<T>(T[] array)
    {
        Array.Reverse(array);
        return array;
    }

    public IEnumerable<T> GetReverseList<T>(IEnumerable<T> list) => Enumerable.Reverse(list);

    public IEnumerable<T> ReturnListOrEmptyList<T>(IEnumerable<T> source) => source is not null ? source : Enumerable.Empty<T>();

    public IEnumerable<string> CloneList(List<string> list)
    {
        return list.GetRange(0, list.Count);
    }

    public IEnumerable<T> ConvertToList<T>(DataTable dt)
    {
        var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
        var properties = typeof(T).GetProperties();
        return dt.AsEnumerable().Select(row =>
        {
            var objT = Activator.CreateInstance<T>();
            foreach (var pro in properties)
            {
                if (columnNames.Contains(pro.Name.ToLower()))
                {
                    try
                    {
                        pro.SetValue(objT, row[pro.Name]);
                    }
                    catch (Exception ex)
                    { throw new Exception(ex.Message, ex.InnerException); }
                }
            }
            return objT;
        }).ToList();
    }

    public bool CompareLists(IEnumerable<int> listNumber, IEnumerable<int> listNumbers)
    {
        return listNumber.Except(listNumbers).Any() &&
        listNumbers.Except(listNumber).Any();
    }

    public bool CompareAllLinq(IEnumerable<int> numeros1, IEnumerable<int> numeros2)
    {
        bool isEqual = numeros1.All(numeros2.Contains)
                       && numeros2.All(numeros1.Contains);

        return isEqual;
    }

    /// <summary>
    /// Esse metodo funciona a partir do NET 9, tem performance melhor do que utilizar CONTAINS
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    //public bool ExistItemOnList(IEnumerable<string> list)
    //{
    //    SearchValues<string> searchValues = SearchValues.Create(
    //        ["Item1", "Item2", "Item3"],
    //        StringComparison.OrdinalIgnoreCase
    //    );

    //    return list.Where(p => searchValues.Contains(p)).Any();
    //}

    #region Trabalhando com a palavra chave params

    public int GetTotalElementsFromParams(params int[] numeros)
    {
        // So funciona com Arrays
        // GetTotalElementsFromParams(1, 2, 3, 4); 
        return numeros.Length;
    }

    //public int GetTotalElementsFromNET9(params IEnumerable numeros)
    //{
    //    // Funciona com qualquer tipo de coleção. (No caso desse metodo não é necessario efetuar a conversão explicita do parametro do metodo)
    //    // ContadorNumeros(new List<int> { 1, 2, 3 });
    //    // ou ContadorNumeros(new int[] { 1, 2, 3 });
    //    // ou ContadorNumeros(Enumerable.Range(1,3));
    //    return numeros.Length;
    //}

    #endregion

    #region Criando uma lista com valores no NET atual e NET 9

    public IEnumerable<int> GetListNET() => new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public IEnumerable<int> GetListNET9() => [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    #endregion

    #region Obter os valores finais da lista, a partir do Index

    public T GetItemFromListByIndex<T>(List<T> list, int index, bool firstItemsFromList) where T : class
    {
        if (firstItemsFromList)
            return list[index];
        else
            return list[index - 1];
    }

    public T GetItemFromListByIndexNET9<T>(List<T> list, int index, bool firstItemsFromList) where T : class
    {
        if (firstItemsFromList)
            return list[index];
        else
            return list[^index]; // Com o simbolo ^ conseguimos obter os ultimos valores pelo index da lista
    }

    #endregion
}
