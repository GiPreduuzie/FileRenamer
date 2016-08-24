
namespace FSharpCalendar

open System

module Calendar = 
    type Orientation = Hor | Ver
    let joinString sep x = x |> List.fold (fun state t -> state + sep + t) ""

    let me x = x


    let getColumns (block:'a[,]) = 
        let getColumn (block:'a[,]) j =
            let limit = block.GetLength(0) - 1
            [|0..limit|] |> Array.map (fun i -> block.[i, j])
        let limit = block.GetLength(1) - 1
        let getColumn' = getColumn block
        [|0..limit|] |> Array.map getColumn'

    let getRows (block:'a[,]) = 
        let getRow (block:'a[,]) i =
            let limit = block.GetLength(1) - 1
            [|0..limit|] |> Array.map (fun j -> block.[i, j])
        let limit = block.GetLength(0) - 1
        let getRow' = getRow block
        [|0..limit|] |> Array.map getRow'
        
            
    let adjust (block : 'a[,]) takeHeight takeWidth =
        let widthBlock = block |> Array2D.map takeHeight
        let maxByColumns = widthBlock |> getColumns |> Array.map Array.max

        let heightBlock = block |> Array2D.map takeWidth
        let maxByRows = heightBlock |> getRows |> Array.map (fun x -> x |> Array.max)

        block |> Array2D.mapi (fun i j x -> (x, maxByRows.[i], maxByColumns.[j]))

    let addInfiniteTail list =
        let tail = Seq.initInfinite (fun _ -> None)
        Seq.append list tail 

    let takeExactly amount from = 
        let length = from |> Seq.length
        let fromOption = from |> Seq.map (fun x -> Some(x))
        let (h, t) = if length >= amount 
                        then (fromOption |> Seq.take amount, from |> Seq.skip amount)
                        else (fromOption |> addInfiniteTail |> Seq.take amount, Seq.empty)
        (h |> List.ofSeq, t |> List.ofSeq)
        
    let slice knife x =
        let rec slice' knife result x =
            let (a, b) = knife x
            let newres = result @ [a]
            match b with
            | [] -> newres
            | b -> slice' knife newres b 
        slice' knife List.Empty x

    let createMonth year month =
        let daysInMonth = DateTime.DaysInMonth(year, month)
        let z = new DateTime(year, month, 1)

        let startWith = 
            let start' = 
                match int (z.DayOfWeek) with
                | 0 -> 7
                | x -> x
            start' - 1

        let prefix = Array.create startWith None
        let content = [|1..daysInMonth|] |> Array.map (fun x -> Some(x))
        let a = Array.concat [prefix; content] |> List.ofArray

        let width = 7
        let height = (a.Length / 7)  + (if a.Length % 7 > 0 then 1 else 0)

        let infA = a |> addInfiniteTail

        let getElement i j = (infA |> Seq.nth (i*width + j)) |> Option.map (fun x -> x.ToString())
        let month = Array2D.init height width getElement

        let getLength (x : string option) =
            match x with 
            | Some value -> value.Length
            | None -> 0

        let adjustedMonth = adjust month  getLength (fun _ -> 1)


        let resolve (x, h, w) =
            let element = x |> Option.fold (fun s t -> t) ""
            let padding = w
            element.PadLeft(padding)

        adjustedMonth 
        |> Array2D.map resolve 
        |> getRows 
        |> Array.map (fun x -> String.Join(" ", x)) 
        |> (fun x -> String.Join("\r\n", x))

        adjustedMonth 


//        let heightBlock = block |> Array2D.map takeHeight
//        1

    let takeMax (block : int list list) =
        let sum a b =
             let rec sum' (a: int list) b res =
                 match (a,b) with
                 | (ah::at, bh::bt) -> sum' at bt (res @ [Math.Max(ah, bh)])
                 | ([],     bh::bt) -> sum' [] bt (res @ [bh])
                 | (ah::at, [])     -> sum' at [] (res @ [ah])
                 | ([],     [])     -> res
             sum' a b List.Empty

        let hsum = block |> List.max
        let vsum = block |> List.fold (fun accum x -> sum accum x) List.Empty
        (hsum, vsum)
        
    let rec takeSome amount (h,t) =
        if amount > 0 then 
            match t with
            | head::tail -> takeSome (amount - 1) (h @ [head], tail) 
            | [] -> (h, t)
        else (h, t)

        
    let addInfiniteTail list =
        let tail = Seq.initInfinite (fun _ -> None)
        Seq.append list tail 

    
    let printMonth year month orientation padding =
        let daysInMonth = DateTime.DaysInMonth(year, month)
        let z = new DateTime(year, month, 1)

        let startWith = 
            let start' = 
                match int (z.DayOfWeek) with
                | 0 -> 7
                | x -> x
            start' - 1

        let startWithTrimmed = startWith % 7

        let prefix = [1..startWithTrimmed] |> List.map  (fun _ -> padding)

        let postfix =
            let a' = 7 - (startWithTrimmed + daysInMonth) % 7
            let a'' = a'%7
            [1..a''] |> List.map (fun _ -> padding)


        let month = [|1..daysInMonth|] |> List.ofArray |> List.map (fun x -> x.ToString())



        let takeWeek x = takeSome 7 (List.Empty,x)

        let rec toWeeks month result =
            let res = takeWeek month
            match res with
            | (a, []) -> result @ [a]
            | (a, b) -> toWeeks b (result @ [a])

        let res = toWeeks ( (prefix @ month @ postfix) |> List.map (fun x -> x.PadLeft(2, '_'))) List.Empty

        match orientation with
            | Hor -> res
            | Ver ->
                let lim = res.[0].Length - 1;
                [0..lim] |> List.map (fun i -> res |> List.fold (fun state t -> state @ [t.[i]]) List.Empty)


        



    let combineBlocks columnAmount defaultValue columnSeparator (blocks : 'a list list list) =
        let normalize padding lim x = 
            let (h,_) = takeExactly lim x
            h |> List.map (fun x -> match x with |Some s -> s |None -> padding)

        let rowMax = blocks |> List.map (fun x -> x.Length) |> List.max
        let columnMax = blocks |> List.map (fun x -> x |> List.map (fun y -> y.Length) |> List.max) |> List.max

        let defaultRow = [1..columnMax] |> List.map (fun _ -> defaultValue)
        let defaultBlock = [1..rowMax]  |> List.map (fun _ -> defaultRow )

        let normalizeRow = normalize defaultValue columnMax
        let normalizeCol = normalize defaultRow rowMax
        
        let blockDimensions (block : 'a list list) =
            let width = block |> List.map (fun x -> x.Length) |> List.max
            let height = block.Length
            (height, width)


        let knife = takeExactly columnAmount
        let resByRows = 
            blocks 
            |> slice knife
            |> List.map (fun x -> (fun y -> 
                                            let size = 
                                                match y with 
                                                | None -> (0, 0)
                                                | Some s -> blockDimensions(s)
                                            (y, size)))
            |> List.map (fux x)

            |> List.map (fun x -> x |> List.map normalizeRow) 
            |> List.map normalizeCol 
            |> slice knife
            |> List.map (fun x -> x |> List.map (fun y -> match y with |Some s -> s |None -> defaultBlock))
        
        let printRow height (res : 'a list list list) =
            let rowMax' = height - 1
            [0..rowMax'] |> List.map (fun i -> res |> List.fold (fun state t -> state @ columnSeparator @ t.[i]) List.Empty)

        let res'' = resByRows |> List.map (printRow rowMax) |> List.collect me
        res''


    let printYear year orientation monthInRow = 
        let padding = "  "
        let res = [1..12] |> List.map (fun x -> printMonth year x orientation padding) |> combineBlocks monthInRow padding ["|"]
        let printMonth' x = x |> List.map (joinString "_") |> joinString "\r\n"
        printMonth' res

    let printYear' year = printYear year Ver

    
type Class1() = 
    member this.PrintYear year orientation monthInRow = Calendar.printYear year orientation monthInRow
    member this.X = "F#"
