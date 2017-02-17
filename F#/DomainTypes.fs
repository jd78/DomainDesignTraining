namespace Game

open System
open System.Collections.Generic

type RobotWarId = Guid
type RobotName = string

type GridSize = {
    x:int
    y:int
} with
    static member Create x y = {x=x;y=y}

type CompassPoint = 
    | North = 0
    | East = 1
    | South = 2
    | West = 3

type Coordinates = {
    x:int
    y:int
} with 
    static member ZeroCreate = {x=0; y=0}
    static member Create x y = {x=x; y=y}
    static member WithX x c = {c with x=x}
    static member WithY y c = {c with y=y}

type Robot = {
    name : RobotName
    coordinates:Coordinates
    compassPoint: CompassPoint
} with
    static member CreateDefault = {name=""; coordinates=Coordinates.ZeroCreate; compassPoint=CompassPoint.North; }
    static member Create name c cp = {name=name; coordinates=c; compassPoint=cp}
    static member WithCompassPoint c r = {r with compassPoint=c}
    static member WithCoordinates c r = {r with coordinates=c}

type RobotWar = {
    id:RobotWarId
    gridSize:GridSize
    robots:Dictionary<string,Robot>
} with 
    static member Create = { id=Guid.NewGuid(); gridSize={x=0; y=0}; robots=Dictionary<string,Robot>() }
    static member WithGridSize g w = {w with gridSize=g}
    static member AddRobot r w = 
        w.robots.Add(r.name, r)
        w
    static member WithRobot r w = 
        w.robots.[r.name] <- r
        w