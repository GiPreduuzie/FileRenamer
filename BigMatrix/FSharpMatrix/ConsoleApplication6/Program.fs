// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System

let toString x = x.ToString()
let nextLine x = x + ""
let writeline message = Console.WriteLine(message |> toString)
let write message = Console.Write(message |> toString)
let readline = Console.ReadLine
let parseNumber value =  Int32.Parse value

let sum a b = a * b

//let getSum = 
//    let a = readline() |> parseNumber
//    let b = readline() |> parseNumber
//    sum a b

//let getNumber i =
//    Console.Write("Input {0}:", i.ToString())
//    Int32.Parse(Console.ReadLine()) 

let getNumber message =
    write message
    readline() |> parseNumber


let printArray result item =  result + ", " + item

//let arrayInput = 
//    let n = readline() |> parseNumber
//    let indexes = [|1..n|]
//    indexes 
//        |> Array.map getNumber
//        |> Array.rev
//        |> Array.map toString
//        |> Array.reduce printArray

let askElement i j = sprintf "[%s, %s]:" (i.ToString()) (j.ToString())

let rec newIter (i, maxi, result, accum) getcurrent =
    if i = maxi then
        result
    else
        let newi = i + 1
        let newresult = accum result (getcurrent i) 
        newIter (newi, maxi, newresult, accum) getcurrent

let newSimplePrint (a : 'a[,][])  = 
    let n = a.GetLength(0)
    let m = a.[0].GetLength(0)
    let k = a.[0].GetLength(1)
    let rowIndexer x y z = a.[x].[x, y].ToString()
    
    let accum   a b = a + b
    let accum'  a b = a + b
    let accum'' a b = a + b

    let start   = ""
    let start'  = fun i -> ""
    let start'' = fun i j -> ""

    let f  (a : string)           b =            accum    a b 
    let f' (a : int->string)      b = fun i   -> accum'  (a i) (b i)
    let f''(a : int->int->string) b = fun i j -> accum'' (a i j) (b i j)
    
    let compose   = newIter (0, n, start,   f)
    let compose'  = newIter (0, m, start',  f')
    let compose'' = newIter (0, k, start'', f'')

    let result = rowIndexer |> compose'' |> compose' |> compose

    //let result = compose (fun x -> compose' (fun y -> compose'' (rowIndexer x y)))
    result


let rec rowIter' i maxi result acummulator c x =
    if i = maxi then
        result
    else
        let newi = i + 1
        let newresult = acummulator c x i 
        result
        rowIter' newi maxi newresult acummulator c x 

let sumElements''' f fZero g gZero h hZero (a : 'a[,][])  = 
    let n = a.GetLength(0)
    let m = a.[0].GetLength(0)
    let k = a.[0].GetLength(1)
    let rowIndexer x y z = a.[x].[x, y]

    let accum f' c x i = f' i (c x i)
    let fAccum = accum f
    let gAccum = accum g
    let hAccum = accum h

    let compose'   = rowIter' 0 m fZero fAccum
    let compose''  = rowIter' 0 n gZero gAccum
    let compose''' = rowIter' 0 n hZero hAccum

    let result = compose''' compose'' compose' rowIndexer
    result |> toString


let sumElements'' f fZero g gZero (a : 'a[,])  = 
    let n = a.GetLength(0)
    let m = a.GetLength(1)
    let rowIndexer x y =  a.[x, y]

    let accum f' c x i = f' i (c x i)
    let fAccum = accum f
    let gAccum = accum g

    let compose'  = rowIter' 0 m fZero fAccum
    let compose'' = rowIter' 0 n gZero gAccum

    let result = compose'' compose' rowIndexer
    result |> toString

let rec rowIter i maxi result acummulator c x =
    if i = maxi then
        result
    else
        let newi = i + 1
        let newresult = acummulator c x i result
        rowIter newi maxi newresult acummulator c x 

let sumElements' f fZero g gZero (a : 'a[,])  = 
    let n = a.GetLength(0)
    let m = a.GetLength(1)
    let rowIndexer x y =  a.[x, y]

    let accum f' c x i = f' i (c x i)
    let fAccum = accum f
    let gAccum = accum g

    let compose'  = rowIter 0 m fZero fAccum
    let compose'' = rowIter 0 n gZero gAccum

    let result = compose'' compose' rowIndexer
    result |> toString


let sumElements = 
    let pattern map x i = 
        if i=0
            then (fun a b -> map a)
            else (fun a b -> x (map a) b)

    let f = pattern (fun x -> x) (fun a b -> a*b)
    let g = pattern (fun x -> x) (fun a b -> a+b)
    sumElements' f 1 g 0

let simplePrint betweenElm betweenRow =
    let map a = a.ToString()
    let pattern x i = 
        if i=0
            then (fun a b -> map a)
            else (fun a b -> x (map a) b)

    let pattern' x =
        pattern (fun a b -> a + x + b)
        
    let f = pattern' betweenElm
    let g = pattern' betweenRow
    sumElements' f "" g ""

let simplePrint'  = simplePrint "*" " + "
let simplePrint'' = simplePrint ", " "\r\n"


//    let a =
//        [(m, ", "); (n, "\r\n")] 
//        |> List.map (fun (x, y) -> compose x y)
//        |> List.reduce (|>)

    // (int -> int -> obj) -> string

    // (obj -> int -> obj) -> obj -> string
    // (obj -> int -> obj) -> obj -> string

    // ((int -> int > a) -> int -> string) -> (int -> int -> a) -> string
    // ((int -> int -> a) -> int -> string) -> (int -> int -> a) -> string
//    let compose'  = compose m ", "
//    let compose'' = compose n "\r\n"
    //let compose''' = compose n "\r\n"

//    let compose'  = compose m "[" "]" " "
//    let compose'' = compose n "{" "}" ":"

//
//    let a =  compose' >> compose''
//    let b =  compose'' >> compose'

//    let b c = ((compose' |> compose'') c) |> compose'''

    // let b = compose'' compose'


    //rowIndexer |> compose'' compose' |> toString


let inputMatrix() =
    let n = getNumber "rows: "
    let m = getNumber "columns: "
    Array2D.init n m askElement
    |> Array2D.map getNumber
    
let multiply (a : 'a[,]) (b : 'b[,]) =
    Array2D.init (a.GetLength(0)) (b.GetLength(1))
        (fun i j -> 
            let array =Array.init (a.GetLength(1)) (fun x -> a.[i,x]*b.[x,i])
            array |> Array.sum)

    
let printTwoMatrices() =
    let a = inputMatrix()
    let b = inputMatrix()
    (simplePrint' a) + "\r\n" +  (simplePrint' b) + "\r\n" + simplePrint' (multiply a b)

[<EntryPoint>]
let main argv = 
    let a = Array2D.init 2 4 (fun i j -> 3*i + j + 1)
    //let a = Array2D.init 2 3 (fun i j -> 3*i + j + 1)
    let b = Array2D.init 4 2 (fun i j -> 2*i + j + 1)
    let text = (simplePrint' a) + "\r\n" +  (simplePrint' b) + "\r\n" //+ simplePrint' (multiply a b)
    let text' = (simplePrint'' a) + "\r\n" +  (simplePrint'' b) + "\r\n" //+ simplePrint'' (multiply a b)
    let text'' = (sumElements a) + "\r\n" +  (sumElements b) + "\r\n" //+ sumElements (multiply a b)

    printfn "%s\r\n%s\r\n%s" text text' text''
    0 // return an integer exit code
