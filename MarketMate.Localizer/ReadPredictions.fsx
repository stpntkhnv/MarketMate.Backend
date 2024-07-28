#r "nuget: Newtonsoft.Json"

open System
open System.IO
open Newtonsoft.Json

type ModelOutput = {
    PredictedLabel: string
}

let readPredictions filePath =
    let json = File.ReadAllText(filePath)
    JsonConvert.DeserializeObject<ModelOutput[]>(json)

let predictions = readPredictions "predictions.json"
predictions |> Array.iter (fun p -> printfn "%A" p)
