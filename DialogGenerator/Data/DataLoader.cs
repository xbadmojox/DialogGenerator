using Microsoft.ML;

namespace DialogGenerator.Data;

public class DataLoader
{
    private readonly string _dataPath;
    private readonly MLContext _mlContext;

    public DataLoader(string dataPath, MLContext mlContext)
    {
        _dataPath = dataPath ?? throw new ArgumentNullException(nameof(dataPath));
        _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
    }

    public IDataView LoadData()
    {
        if (!File.Exists(_dataPath))
            throw new FileNotFoundException($"The file {_dataPath} does not exist.");

        // Загрузка данных из CSV файла
        var dataView = _mlContext.Data.LoadFromTextFile<Dialogue>(_dataPath, hasHeader: true, separatorChar: ',');

        // Разметка и подготовка данных
        var dataProcessPipeline = _mlContext.Transforms.Text.FeaturizeText("InputFeaturized", "Input")
            .Append(_mlContext.Transforms.Text.FeaturizeText("QuestFeaturized", "Quest"))
            .Append(_mlContext.Transforms.Text.FeaturizeText("EnvironmentFeaturized", "Environment"))
            .Append(_mlContext.Transforms.Concatenate("Features", "InputFeaturized", "QuestFeaturized", "EnvironmentFeaturized"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("Label", "Response"))
            .AppendCacheCheckpoint(_mlContext);

        // Преобразование данных с использованием TF-IDF
        var transformedData = dataProcessPipeline.Fit(dataView).Transform(dataView);

        return transformedData;
    }
}

public class Dialogue
{
    public string Input { get; set; }
    public string Quest { get; set; }
    public string Environment { get; set; }
    public string Response { get; set; }
}