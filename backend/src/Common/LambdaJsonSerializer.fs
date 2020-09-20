namespace Bank

open Amazon.Lambda.Core
open Amazon.Lambda.Serialization.Json

[<assembly:LambdaSerializer(typeof<JsonSerializer>)>]
do ()
