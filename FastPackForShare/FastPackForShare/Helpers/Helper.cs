using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Compression;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using FastPackForShare.Models;
using Microsoft.AspNetCore.Http;
using FastPackForShare.Enums;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using FastPackForShare.Extensions;

namespace FastPackForShare.Helpers;

public sealed class Helper
{
    private static IHostEnvironment _environment;
    private delegate decimal MyOperations(decimal number);

    public Helper()
    {

    }

    public Helper(IHostEnvironment environment)
    {
        _environment = environment;
    }

    #region Private Methods

    private XmlDocument RemoveXmlDeclaration(XmlDocument doc)
    {
        var declarations = doc.ChildNodes.OfType<XmlNode>().Where(x => x.NodeType == XmlNodeType.XmlDeclaration).ToList();
        declarations.ForEach(x => doc.RemoveChild(x));
        return doc;
    }

    private string GetFuseTimebyState(string state)
    {
        var dictionaryTimeZoneFromBrasil = new Dictionary<string, string>
    {
        { "AC", "America/Rio_Branco" }, // Acre
        { "AL", "America/Maceio" },    // Alagoas
        { "AP", "America/Belem" },     // Amapá
        { "AM", "America/Manaus" },    // Amazonas
        { "BA", "America/Bahia" },     // Bahia
        { "CE", "America/Fortaleza" }, // Ceará
        { "DF", "America/Sao_Paulo" }, // Distrito Federal
        { "ES", "America/Sao_Paulo" }, // Espírito Santo
        { "FN", "America/Noronha" },   // Fernando de Noronha
        { "GO", "America/Sao_Paulo" }, // Goiás
        { "MA", "America/Fortaleza" }, // Maranhão
        { "MT", "America/Cuiaba" },    // Mato Grosso
        { "MS", "America/Campo_Grande" }, // Mato Grosso do Sul
        { "MG", "America/Sao_Paulo" }, // Minas Gerais
        { "PA", "America/Belem" },     // Pará
        { "PB", "America/Fortaleza" }, // Paraíba
        { "PR", "America/Sao_Paulo" }, // Paraná
        { "PE", "America/Recife" },    // Pernambuco
        { "PI", "America/Fortaleza" }, // Piauí
        { "RJ", "America/Sao_Paulo" }, // Rio de Janeiro
        { "RN", "America/Fortaleza" }, // Rio Grande do Norte
        { "RS", "America/Sao_Paulo" }, // Rio Grande do Sul
        { "RO", "America/Porto_Velho" }, // Rondônia
        { "RR", "America/Boa_Vista" },   // Roraima
        { "SC", "America/Sao_Paulo" }, // Santa Catarina
        { "SP", "America/Sao_Paulo" }, // São Paulo
        { "SE", "America/Maceio" },    // Sergipe
        { "TO", "America/Araguaina" }  // Tocantins
    };

        if (dictionaryTimeZoneFromBrasil.TryGetValue(state, out var value))
            return value;

        return string.Empty;
    }

    #endregion

    public string RemoveMimeType(string base64)
    {
        var keyValue = "base64,";
        if (!base64.Contains(keyValue))
            return base64;

        var start = base64.LastIndexOf(keyValue);
        return base64.Substring(start).Replace(keyValue, "");
    }

    public string ConvertObjectParaJSon<T>(T obj)
    {
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
        MemoryStream ms = new MemoryStream();
        ser.WriteObject(ms, obj);
        string jsonString = Encoding.UTF8.GetString(ms.ToArray());
        ms.Close();
        return jsonString;
    }

    public T ConvertJSonParaObject<T>(string jsonString)
    {
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
        T obj = (T)serializer.ReadObject(ms);
        return obj;
    }

    public string RemoveHtmlTags(string value)
    {
        var tagList = Regex.Matches(value, @"(?<=</?)([^ >/]+)")
                           .Select(p => p.ToString())
                           .Distinct(StringComparer.OrdinalIgnoreCase)
                           .ToArray();

        var newText = value.ApplyTrim();

        foreach (var tag in tagList)
        {
            var start = newText.IndexOf($"<{tag}");
            var end = newText.IndexOf(">");

            if (start >= 0 && end >= 0 && end < newText.Length)
            {
                var lenght = end - start + 1;
                if (lenght > 0 && start + lenght < newText.Length)
                {
                    var removed = newText.ApplySubString(start, lenght);
                    newText = newText.ApplyReplace(removed, "");
                }
            }

            newText = newText.ApplyReplace($"</{tag}>", "");
        }

        return newText;
    }

    public static string ConvertImageToBase64(string pImage)
    {
        var imagePath = Path.GetFileName(pImage);
        var imageArray = File.ReadAllBytes(imagePath);
        return $"data:image/jpg;base64,{Convert.ToBase64String(imageArray)}";
    }

    public static int GetQuarter(DateTime date)
    {
        return (date.Month - 1) / 3 + 1;
    }

    public DateTime LastDayPreviousMonth(string currentMonth, string currentYear)
    {
        DateTime firstMonthDay = DateTime.Parse($"01/{currentMonth}/{currentYear}");
        DateTime lastMonthDay = firstMonthDay.AddDays(-1);
        return lastMonthDay;
    }

    public static DateTime LastDayCurrentMonth(string currentMonth, string currentYear)
    {
        DateTime firstMonthDay = DateTime.Parse($"01/{currentMonth}/{currentYear}");
        DateTime firstMonthNextDay = firstMonthDay.AddMonths(1);
        DateTime lastMonthDay = firstMonthNextDay.AddDays(-1);
        return lastMonthDay;
    }

    public DataTable ConvertDynamicListToDataTable(IEnumerable<dynamic> list)
    {
        var json = JsonSerializer.Serialize(list);
        DataTable dt = (DataTable)JsonSerializer.Deserialize(json, typeof(DataTable));
        return dt;
    }

    public static string GetUrlDetails(string url)
    {
        Uri uri = new Uri(url);
        StringBuilder result = new StringBuilder();
        result.Append($"AbsolutePath = {uri.AbsolutePath} || ");
        result.Append($"AbsoluteUri = {uri.AbsoluteUri} || ");
        result.Append($"Authority = {uri.Authority} || ");
        result.Append($"DnsSafeHost = {uri.DnsSafeHost} || ");
        result.Append($"Fragment = {uri.Fragment} || ");
        result.Append($"Host = {uri.Host} || ");
        result.Append($"HostNameType = {uri.HostNameType} || ");
        result.Append($"IsAbsoluteUri = {uri.IsAbsoluteUri} || ");
        result.Append($"IsDefaultPort = {uri.IsDefaultPort} || ");
        result.Append($"IsFile = {uri.IsFile} || ");
        result.Append($"IsLoopback = {uri.IsLoopback} || ");
        result.Append($"IsUnc = {uri.IsUnc} || ");
        result.Append($"LocalPath = {uri.LocalPath} || ");
        result.Append($"OriginalString = {uri.OriginalString} || ");
        result.Append($"PathAndQuery = {uri.PathAndQuery} || ");
        result.Append($"Port = {uri.Port} || ");
        result.Append($"Query = {uri.Query} || ");
        result.Append($"Scheme = {uri.Scheme} || ");
        result.Append($"UserEscaped = {uri.UserEscaped} || ");
        result.Append($"UserInfo = {uri.UserInfo} || ");
        result.Append(new string('-', 10));
        return result.ToString();
    }

    public static string BuildUrlWithUriBuilder(int option)
    {
        UriBuilder uriBuilder = new UriBuilder();
        string url = string.Empty;

        if (option == 1)
        {
            uriBuilder.Scheme = "http";
            uriBuilder.Host = "macoratti.net";
            uriBuilder.Path = "sistema";
            Uri uri = uriBuilder.Uri;
            url = uri.ToString();
        }
        else
        {
            uriBuilder.Scheme = "http";
            uriBuilder.Host = "macoratti.net";
            uriBuilder.Path = "sistema";
            uriBuilder.Port = 8089;
            uriBuilder.UserName = "macoratti";
            uriBuilder.Password = "numsey";
            uriBuilder.Query = "search=br";
            Uri uri2 = uriBuilder.Uri;
            url = uri2.ToString();
        }

        return url;
    }

    public static IPAddress GetIPAddress(string hostName)
    {
        // A partir de um hostname, o metodo ira retornar o endereço de IP
        Ping ping = new Ping();
        var replay = ping.Send(hostName);
        if (replay.Status == IPStatus.Success)
        {
            return replay.Address;
        }
        return null;
    }

    public static DataTable ConvertToDataTable<T>(IEnumerable<T> list)
    {
        DataTable table = CreateTable<T>();
        Type entityType = typeof(T);
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
        foreach (T item in list)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
            {
                row[prop.Name] = prop.GetValue(item);
            }
            table.Rows.Add(row);
        }
        return table;
    }

    private static DataTable CreateTable<T>()
    {
        Type entityType = typeof(T);
        DataTable table = new DataTable(entityType.Name);
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
        foreach (PropertyDescriptor prop in properties)
        {
            table.Columns.Add(prop.Name, prop.PropertyType);
        }
        return table;
    }

    public static string ConvertTimeSpanToString(TimeSpan hour)
    {
        if (hour != null)
        {
            if (hour.Hours < 10 && hour.Minutes < 10)
                return $"0{hour.Hours}:0{hour.Minutes}";
            else if (hour.Hours >= 10 && hour.Minutes < 10)
                return $"{hour.Hours}:0{hour.Minutes}";
            else if (hour.Hours < 10 && hour.Minutes >= 10)
                return $"0{hour.Hours}:0{hour.Minutes}";
            else
                return $"{hour.Hours}:{hour.Minutes}";
        }

        return string.Empty;
    }

    public static string AddZeroToLeftOrRight(string text, int qty = 8, bool isLeft = true)
    {
        if (GuardClauseExtension.IsNullOrWhiteSpace(text) == false)
            return isLeft ? text.PadLeft(qty, '0') : text.PadRight(qty, '0');
        return text;
    }

    public string ConvertXmlToJson(string xml)
    {
        if (GuardClauseExtension.IsNullOrWhiteSpace(xml) == false)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            doc = RemoveXmlDeclaration(doc);
            var json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, true);
            return json.ToString();
        }

        return null;
    }

    public string CreateCpf()
    {
        int sum = 0, rest = 0;
        int[] mult1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        Random rnd = new Random();
        string result = rnd.Next(100000000, 999999999).ToString();

        for (int i = 0; i < 9; i++)
            sum += int.Parse(result[i].ToString()) * mult1[i];

        rest = sum % 11;
        if (rest < 2)
            rest = 0;
        else
            rest = 11 - rest;

        result = result + rest;
        sum = 0;

        for (int i = 0; i < 10; i++)
            sum += int.Parse(result[i].ToString()) * mult2[i];

        rest = sum % 11;

        if (rest < 2)
            rest = 0;
        else
            rest = 11 - rest;

        result = result + rest;
        return result;
    }

    public string GetCron(EnumPeriod typeExecution)
    {
        Dictionary<EnumPeriod, string> dictionary = new Dictionary<EnumPeriod, string>
    {
        { EnumPeriod.Never, "0 0 31 2 *" },
        { EnumPeriod.Daily, "0 0 * * *" },
        { EnumPeriod.Weekly, "0 0 * * 1" },
        { EnumPeriod.Monthly, "0 0 1 * *" },
        { EnumPeriod.Yearly, "0 0 1 1 *" }
    };
        return dictionary[typeExecution];
    }

    public DateTime GetFirstDayOfWeek(DateTime date)
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        var diff = date.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
        if (diff < 0)
            diff += 7;
        return date.AddDays(-diff).Date;
    }

    public static DateTime GetLastDayOfWeek(DateTime date)
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        var diff = date.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
        if (diff < 0)
            diff += 7;
        DateTime start = date.AddDays(-diff).Date;
        return start.AddDays(6).Date;
    }

    public static string[] RepeatValues(int times, string word)
    {
        times = times > 0 ? times : 1;

        if (!string.IsNullOrEmpty(word))
            return Enumerable.Range(1, times).Select(x => word).ToArray();

        return Enumerable.Range(1, times).Select(x => "gray").ToArray();
    }

    public void CreateJsonFile<T>(T model, string fileName = "arquivo.json")
    {
        File.WriteAllText(fileName, JsonSerializer.Serialize(model));
    }

    public void CreateXmlFile<T>(T model, string fileName = "arquivo.xml")
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
        xmlSerializer.Serialize(new FileStream(fileName, FileMode.OpenOrCreate), model);
    }

    public T ConvertXmlToObject<T>(string xmlFile)
    {
        StreamReader xmlStream = new StreamReader(xmlFile);
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        var result = (T)serializer.Deserialize(xmlStream);
        return result;
    }

    public Match EncontrarOcorrenciaDe(string fonte, string criterio, int ocorrencia)
    {
        Regex RE = new Regex(criterio, RegexOptions.Multiline);

        if (ocorrencia < 1)
            throw new ArgumentException("Não pode ser menor que 1", nameof(ocorrencia));

        MatchCollection correspondencias = RE.Matches(fonte);

        if (ocorrencia >= correspondencias.Count)
            return null;

        return correspondencias[ocorrencia];
    }

    public IEnumerable<Match> EncontrarCadaOcorrenciaDe(string fonte, string criterio, int ocorrencia)
    {
        ICollection<Match> ocorrencias = new List<Match>();
        Regex RE = new Regex(criterio, RegexOptions.Multiline);

        if (ocorrencia < 1)
            throw new ArgumentException("Não pode ser menor que 1", nameof(ocorrencia));

        MatchCollection correspondencias = RE.Matches(fonte);

        for (int index = ocorrencia - 1; index < correspondencias.Count; index += ocorrencia)
        {
            ocorrencias.Add(correspondencias[index]);
        }

        return ocorrencias;
    }

    /// <summary>
    /// Nesse metodo nós iremos receber um base64 em string
    /// </summary>
    /// <param name="file"></param>
    /// <param name="imgFile">Esse parametro tem que ser formado por um guid + "_" + nomearquivo</param>
    /// <returns></returns>
    public async Task<(bool, string)> UploadFile(string file, string imgFile)
    {

        if (GuardClauseExtension.IsNullOrWhiteSpace(file))
        {
            return (false, "Forneça uma imagem!");
        }

        var imageDataByteArray = Convert.FromBase64String(file);

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imgFile);

        if (File.Exists(filePath))
        {
            return (false, "Já existe um arquivo com este nome!");
        }

        await File.WriteAllBytesAsync(filePath, imageDataByteArray);

        return (true, "Upload efetuado com sucesso");
    }

    public string ConvertModelObjectToXml<T>(T modelObject)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

        using (StringWriter textWriter = new StringWriter())
        {
            xmlSerializer.Serialize(textWriter, modelObject);
            return textWriter.ToString();
        }
    }

    public string ConvertXmlToJson<T>(string xmlData)
    {
        T modelObject;

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

        using (StringReader textReader = new StringReader(xmlData))
        {
            modelObject = (T)xmlSerializer.Deserialize(textReader);
        }
        return modelObject.SerializeObject();
    }

    public string ConvertJsonToXml<T>(string jsonData)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        T modelObject = JsonSerializer.Deserialize<T>(jsonData, options);

        return ConvertModelObjectToXml(modelObject);
    }

    public string GetBytesFromBinaryString(string binary)
    {
        StringBuilder sb = new StringBuilder();

        if (GuardClauseExtension.IsNullOrWhiteSpace(binary) == false)
        {
            string[] binarySplited = binary.Split(' ');

            foreach (var item in binarySplited)
            {
                string binaryContent = item.ApplyTrim();

                if (GuardClauseExtension.IsBinaryString(binaryContent) && GuardClauseExtension.IsNullOrWhiteSpace(binaryContent) == false)
                    sb.Append((char)Convert.ToInt32(binaryContent, 2));

                else if (GuardClauseExtension.IsNullOrWhiteSpace(binaryContent) == false)
                    sb.Append(binaryContent);
            }
        }

        return sb.ToString();
    }

    public int GetFibonacciFromNumber(int number)
    {
        if (number == 0 || number == 1) return number;

        int a = 0;
        int b = 1;
        int c = 3;

        for (int i = 2; i <= number; i++)
        {
            c = a + b;
            a = b;
            b = c;
        }

        return c;
    }

    public int GetRecursiveFibonnaciFromNumber(int number)
    {
        if (number == 0 || number == 1) return number;

        return GetRecursiveFibonnaciFromNumber(number - 1) + GetRecursiveFibonnaciFromNumber(number - 2);
    }

    public int GetFatorialFromNumber(int number)
    {
        int fatorial = 1;

        if (number == 0 || number == 1) return fatorial;

        for (int i = number; i >= 1; i--)
        {
            fatorial = fatorial * i;
        }

        return fatorial;
    }

    public int GetRecursiveFatorialFromNumber(int number)
    {
        if (number == 0 || number == 1) return 1;

        return number * GetRecursiveFatorialFromNumber(number - 1);
    }

    public string ConvertNumberToRomanNumber(int number)
    {
        string result = string.Empty;

        string[] arrRoman = { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M" };
        int[] arrArabic = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };

        for (int count = 13; count >= 0; count--)
        {
            while (number >= arrArabic[count])
            {
                number -= arrArabic[count];
                result += arrRoman[count];
            }
        }

        return result;
    }

    public static IEnumerable<PropertyDescriptor> GetDataProperties<T>() where T : class
    {
        var listaPropriedades = TypeDescriptor.GetProperties(typeof(T)).Cast<PropertyDescriptor>().ToList();
        var listaPropriedadesNome = typeof(T).GetProperties().Select(x => x.GetCustomAttribute<DisplayNameAttribute>()).Where(x => x != null).Select(x => x.DisplayName);
        if (listaPropriedadesNome != null)
        {
            return listaPropriedades.Where(x => listaPropriedadesNome.Contains(x.DisplayName)).ToList();
        }

        return null;
    }

    public int CalculateAge(DateTime birthDate)
    {
        DateTime currentDate = DateOnlyExtension.GetDateTimeNowFromBrazil();
        int age = currentDate.Year - birthDate.Year;

        if (currentDate.DayOfYear < birthDate.DayOfYear)
        {
            age = age - 1;
        }

        return age;
    }

    public static ICollection<T> ConvertToList<T>(DataTable dt)
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
                    pro.SetValue(objT, row[pro.Name]);
                }
            }
            return objT;
        }).ToList();
    }

    public string GetModelAttributeErrors<T>(T obj)
    {
        var ctx = new ValidationContext(obj);
        var resultados = new List<ValidationResult>();
        if (!Validator.TryValidateObject(obj, ctx, resultados, true))
        {
            return string.Join(",", resultados);
        }

        return string.Empty;
    }

    public string StartDateToJob(EnumPeriod key)
    {
        DateTime currentDate = DateOnlyExtension.GetDateTimeNowFromBrazil();
        Dictionary<EnumPeriod, string> dictionary = new Dictionary<EnumPeriod, string>
        {
            { EnumPeriod.Daily, currentDate.ToString("yyyy-MM-dd") },
            { EnumPeriod.Weekly, GetFirstDayOfWeek(currentDate).ToString("yyyy-MM-dd") },
            { EnumPeriod.Monthly, new DateTime(currentDate.Year, currentDate.Month, 1).ToString("yyyy-MM-dd") },
            { EnumPeriod.Yearly, new DateTime(currentDate.Year, 1, 1).ToString("yyyy-MM-dd") },
            { EnumPeriod.Never, "" }
        };
        return dictionary[key];
    }

    public string EndDateToJob(EnumPeriod key)
    {
        DateTime currentDate = DateOnlyExtension.GetDateTimeNowFromBrazil();
        Dictionary<EnumPeriod, string> dictionary = new Dictionary<EnumPeriod, string>
        {
            { EnumPeriod.Daily, currentDate.ToString("yyyy-MM-dd") },
            { EnumPeriod.Weekly, GetLastDayOfWeek(currentDate).ToString("yyyy-MM-dd") },
            { EnumPeriod.Monthly, new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd") },
            { EnumPeriod.Yearly, new DateTime(currentDate.Year, 12, 31).ToString("yyyy-MM-dd") },
            { EnumPeriod.Never, "" }
        };
        return dictionary[key];
    }

    public static DateTime GetNextUtilDay(DateTime dateTime)
    {
        while (true)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Saturday)
                dateTime = dateTime.AddDays(2);
            else if (dateTime.DayOfWeek == DayOfWeek.Sunday)
                dateTime = dateTime.AddDays(1);

            // Caso tenha feriado nacional ou internacional, fazer uma consulta no BD pra isso...depois um IF para validar e somar 1 dia...

            return dateTime;
        }
    }

    public static JsonSerializerOptions ObterConfiguracaoJson() => new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    // Com esse metodo é possivel criar um objeto em tempo de execução, com base em um JSON
    // Nesse caso não é necessario criar classe ou record
    public dynamic GetDynamicObjectByJSON(string jsonData)
    {
        dynamic objDynamic = JsonSerializer.Serialize(jsonData);
        return objDynamic;
    }

    public DateTime CreateDateByFuseTimeState(string state)
    {
        string timezoneId = GetFuseTimebyState(state);

        if (string.IsNullOrWhiteSpace(timezoneId))
            return DateOnlyExtension.GetDateTimeNowFromBrazil();

        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        return TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);

    }

    public static IEnumerable<DropDownListModel> ConvertEnumToList<T>() where T : Enum
    {
        IEnumerable<DropDownListModel> list = Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(x => new DropDownListModel
            {
                Id = Convert.ToInt64(x),
                Description = x.ToString()
            });

        return list;
    }

    public static Dictionary<string, object> ObjectToDictionary(object source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        var dict = new Dictionary<string, object>();

        foreach (PropertyInfo prop in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            dict[prop.Name] = prop.GetValue(source);
        }

        return dict;
    }
}