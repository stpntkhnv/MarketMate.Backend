#r "nuget: Microsoft.ML"
#r "nuget: Newtonsoft.Json"

open System
open System.IO
open Microsoft.ML
open Microsoft.ML.Data
open Newtonsoft.Json

type ModelInput = {
    [<LoadColumn(0)>] Text: string
    [<LoadColumn(1)>] Label: string
}

type ModelOutput() = 
    [<DefaultValue>] val mutable PredictedLabel: string

let context = MLContext()

let modelPath = "model.zip"
let mutable inputSchema = Unchecked.defaultof<DataViewSchema>
let model = context.Model.Load(modelPath, &inputSchema)

let predict (text: string) =
    let predictionEngine = context.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model)
    let words = text.Split(' ')
    words
    |> Array.map (fun word -> { Text = word; Label = "" })
    |> Array.map (fun input -> predictionEngine.Predict(input))

let text = "ТК Садовод 2Д-03 корпус А"
let predictions = predict text

// Сохранение предсказаний в файл
let savePredictions predictions filePath =
    let json = JsonConvert.SerializeObject(predictions)
    File.WriteAllText(filePath, json)

savePredictions predictions "predictions.json"

// Вывод предсказаний на экран
predictions |> Array.iter (fun p -> printfn "%A" p)
