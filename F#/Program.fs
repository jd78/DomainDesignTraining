namespace Game

open System
open Domain

module Main =

    [<EntryPoint>]
    let main argv = 
        let war = 
            newWar
            |> update (Command.AddGridSize (createGridSize 5 5))
            |> update (Command.AddRobot (Robot.Create (Coordinates.Create 1 2) CompassPoint.North))
            |> update Command.RotateLeft
            |> update Command.Move
            |> update Command.RotateLeft
            |> update Command.Move
            |> update Command.RotateLeft
            |> update Command.Move
            |> update Command.RotateLeft
            |> update Command.Move
            |> update Command.Move
        
        printfn "%s" (war.robot |> getRobotPosition)

        0 // return an integer exit code