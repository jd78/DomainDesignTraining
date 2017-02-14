namespace Game

open System

module Domain =

    module private Private =
        type Rotation = 
        |Left
        |Right

        let addGridSize gridSize robotWar =
            robotWar |> RobotWar.WithGridSize gridSize

        let addRobot r robotWar = 
            robotWar |> RobotWar.WithRobot r

        let rotateRobot rotation robotWar =
            let robot = robotWar.robot 
            let compassPointInt = 
                match rotation with 
                | Right -> int robot.compassPoint + 1
                | Left -> int robot.compassPoint - 1
            
            let newCompassPoint = 
                match compassPointInt with
                | -1 -> CompassPoint.West
                | _ -> enum<CompassPoint>(compassPointInt % 4)
            
            robotWar 
            |> RobotWar.WithRobot (robotWar.robot |> Robot.WithCompassPoint newCompassPoint)
        
        let moveRobot robotWar = 
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

            let newRobotState = 
                robotWar.robot 
                |> op

            robotWar |> RobotWar.WithRobot newRobotState

    let newWar = RobotWar.Create

    let createGridSize x y = GridSize.Create x y

    let createRobot coordinates compassPoint = Robot.Create coordinates compassPoint
    
    let getRobotPosition robot =
        sprintf "%d %d %s" robot.coordinates.x robot.coordinates.y (string robot.compassPoint)

    type Command = 
        | AddGridSize of GridSize
        | AddRobot of Robot
        | RotateLeft
        | RotateRight
        | Move
    
    let update command robotWar = 
        match command with
        | AddGridSize g -> robotWar |> Private.addGridSize g
        | AddRobot r -> robotWar |> Private.addRobot r
        | RotateLeft -> robotWar |> Private.rotateRobot Private.Rotation.Left
        | RotateRight -> robotWar |> Private.rotateRobot Private.Rotation.Right
        | Move -> robotWar |> Private.moveRobot