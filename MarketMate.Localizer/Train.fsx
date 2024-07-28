open System
open System.IO
open Microsoft.ML
open Microsoft.ML.Data
open Newtonsoft.Json

type InputData = {
    Text: string
    Корпус: string
    Линия: string
    Павильон: string
}

type ModelInput = {
    [<LoadColumn(0)>] Text: string
    [<LoadColumn(1)>] Label: string
}

type ModelOutput = {
    PredictedLabel: string
}

let loadData (filePath: string) =
    let json = File.ReadAllText(filePath)
    JsonConvert.DeserializeObject<InputData[]>(json)

let prepareData (inputData: InputData[]) =
    inputData |> Array.collect (fun item ->
        item.Text.Split(' ')
        |> Array.map (fun word ->
            { Text = word
              Label =
                  if item.Корпус.Contains(word) then "B-корпус"
                  elif item.Линия.Contains(word) then "B-линия"
                  elif item.Павильон.Contains(word) then "B-павильон"
                  else "O" }))

let context = MLContext()

let trainModel (dataView: IDataView) =
    let pipeline =
        EstimatorChain()
            .Append(context.Transforms.Conversion.MapValueToKey("Label"))
            .Append(context.Transforms.Text.FeaturizeText("Features", "Text"))
            .Append(context.Transforms.Concatenate("Features", "Features"))
            .Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"))

    let model = pipeline.Fit(dataView)
    model

let evaluateModel (model: ITransformer) (dataView: IDataView) =
    let predictions = model.Transform(dataView)
    let metrics = context.MulticlassClassification.Evaluate(predictions)
    printfn "Log-loss: %f" metrics.LogLoss

[<EntryPoint>]
let main argv =
    let inputData = loadData "data.json"
    let preparedData = prepareData inputData

    let dataView = context.Data.LoadFromEnumerable(preparedData)

    // Разделение данных на тренировочный и тестовый наборы
    let trainTestData = context.Data.TrainTestSplit(dataView, testFraction = 0.2)
    let trainData = trainTestData.TrainSet
    let testData = trainTestData.TestSet

    let model = trainModel trainData
    evaluateModel model testData

    context.Model.Save(model, trainData.Schema, "model.zip")
    0
