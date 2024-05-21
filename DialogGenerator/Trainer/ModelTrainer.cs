using Microsoft.ML;

namespace DialogGenerator.Trainer;

public class ModelTrainer
{
    private readonly MLContext _mlContext;
    private ITransformer _trainedModel;
        
    public ModelTrainer(MLContext mlContext)
    {
        _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
    }

    public ITransformer TrainModel(IDataView dataView)
    {
        if (dataView == null) throw new ArgumentNullException(nameof(dataView));
        
        var trainer = _mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features")
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        _trainedModel = trainer.Fit(dataView);

        return _trainedModel;
    }

    public void SaveModel(ITransformer trainedModel, DataViewSchema schema, string modelPath)
    {
        if (trainedModel == null) throw new ArgumentNullException(nameof(trainedModel));
        if (schema == null) throw new ArgumentNullException(nameof(schema));
        if (string.IsNullOrEmpty(modelPath)) throw new ArgumentNullException(nameof(modelPath));

        _mlContext.Model.Save(trainedModel, schema, modelPath);
    }
}