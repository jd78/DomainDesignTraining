namespace Game

open System

module Domain =

    let maybeGetRobot robotName robotWar = 
        let robotExists = robotName |> robotWar.robots.ContainsKey 
        match robotExists with
        | true -> Some robotWar.robots.[robotName]
        | false -> None
    
    let getRobot robotName robotWar = 
        let r = robotWar |> maybeGetRobot robotName
        r.Value

    let ensureRobotExists f robotName robotWar =
        let maybeRobot = robotWar |> maybeGetRobot robotName
        match maybeRobot with
        | Some r -> f()
        | None -> failwith "Robot not found"

    module private Private =
        type Rotation = 
        |Left
        |Right

        let addGridSize gridSize robotWar =
            robotWar |> RobotWar.WithGridSize gridSize

        let addRobot r robotWar = 
            let maybeRobot = robotWar |> maybeGetRobot r.name
            match maybeRobot with
            | Some r -> failwith "a Robot with the same name already exists"
            | None -> robotWar |> RobotWar.AddRobot r

        let rotateRobot robotName rotation robotWar =
            let robot = robotWar |> getRobot robotName
            let compassPointInt = 
                match rotation with 
                | Right -> int robot.compassPoint + 1
                | Left -> int robot.compassPoint - 1
            
            let newCompassPoint = 
                match compassPointInt with
                | -1 -> CompassPoint.West
                | _ -> enum<CompassPoint>(compassPointInt % 4)
            
            robotWar 
            |> RobotWar.WithRobot (robot |> Robot.WithCompassPoint newCompassPoint)
        
        let moveRobot robotName robotWar = 
            let validateMove r = 
                match (r.coordinates.x, r.coordinates.y) with
                | (x, y) when x<0 || y<0 -> failwith "Robot outside of arena"
                | (x, y) when x>robotWar.gridSize.x || y> robotWar.gridSize.y -> failwith "Robot outside of arena"
                | _ -> r
            
            let moveRobot robot = 
                let x = robot.coordinates.x
                let y = robot.coordinates.y

                let newXCoordinate x coordinates:Coordinates = coordinates |> Coordinates.WithX x 
                let newYCoordinate y coordinates:Coordinates = coordinates |> Coordinates.WithY y
                
                match robot.compassPoint with
                    | CompassPoint.North -> robot |> Robot.WithCoordinates (robot.coordinates |> newYCoordinate (y + 1))  
                    | CompassPoint.South -> robot |> Robot.WithCoordinates (robot.coordinates |> newYCoordinate (y - 1))
                    | CompassPoint.West -> robot |> Robot.WithCoordinates (robot.coordinates |> newXCoordinate (x - 1))
                    | CompassPoint.East -> robot |> Robot.WithCoordinates (robot.coordinates |> newXCoordinate (x + 1))
                    | _ -> failwith "Compass point not matched"
            
            let op = moveRobot >> validateMove
            let robot = robotWar |> getRobot robotName
            let newRobotState = robot |> op

            robotWar |> RobotWar.WithRobot newRobotState

    let newWar = RobotWar.Create

    let createGridSize x y = GridSize.Create x y

    let createRobot coordinates compassPoint = Robot.Create coordinates compassPoint
    
    let getRobotPosition robot =
        sprintf "%d %d %s" robot.coordinates.x robot.coordinates.y (string robot.compassPoint)

    type RobotName = string
    type Command = 
        | AddGridSize of GridSize
        | AddRobot of Robot
        | RotateLeft of RobotName
        | RotateRight of RobotName
        | Move of RobotName
    
    let update command robotWar = 
        match command with
        | AddGridSize g -> robotWar |> Private.addGridSize g
        | AddRobot r -> robotWar |> Private.addRobot r
        | RotateLeft n -> robotWar |> ensureRobotExists (fun f ->  robotWar |> Private.rotateRobot n Private.Rotation.Left) n 
        | RotateRight n -> robotWar |> ensureRobotExists (fun f -> robotWar |> Private.rotateRobot n Private.Rotation.Right) n
        | Move n -> robotWar |> ensureRobotExists (fun f -> robotWar |> Private.moveRobot n) n