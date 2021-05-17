namespace Bank

open Amazon.Lambda.Core
open Amazon.Lambda.Serialization.SystemTextJson

[<assembly:LambdaSerializer(typeof<DefaultLambdaJsonSerializer>)>]
do ()
