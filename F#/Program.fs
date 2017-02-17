namespace Game

open System
open Domain

module Main =

    [<EntryPoint>]
    let main argv = 
        let robotName = "test"
        let war = 
            newWar
            |> update (Command.AddGridSize (createGridSize 5 5))
            |> update (Command.AddRobot (Robot.Create robotName (Coordinates.Create 1 2) CompassPoint.North))
            |> update (Command.RotateLeft robotName)
            |> update (Command.Move robotName)
            |> update (Command.RotateLeft robotName)
            |> update (Command.Move robotName)
            |> update (Command.RotateLeft robotName)
            |> update (Command.Move robotName)
            |> update (Command.RotateLeft robotName)
            |> update (Command.Move robotName)
            |> update (Command.Move robotName)
        
        printfn "%s" (
            war 
            |> getRobot robotName
            |> getRobotPosition
        
        )
        
        0 // return an integer exit code