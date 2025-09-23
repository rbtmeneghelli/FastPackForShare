namespace FastPackForShare.Extensions;

public static class StreamExtension<TModel>
{
    /// <summary>
    /// Aplicação do PipeReader para Json de grande volume de dados ou para uso de serviços streaming como mensageria ou webSockets
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<TModel> TransformStreamInModel(Stream stream)
    {
        stream.Position = 0;
        var pipeReader = PipeReader.Create(stream);
        using var asStream = pipeReader.AsStream();
        TModel model = await JsonSerializer.DeserializeAsync<TModel>(asStream);
        return model;
    }

    /// <summary>
    /// Aplicação do PipeReader para Json de grande volume de dados ou para uso de serviços streaming como mensageria ou webSockets
    /// A partir do NET 10 esse código abaixo e valido!
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    //public async Task<TModel> TransformStreamInModel(Stream stream)
    //{
    //    stream.Position = 0;
    //    var reader = PipeReader.Create(stream);
    //    return await JsonSerializer.DeserializeAsync<TModel>(reader);
    //}
}
