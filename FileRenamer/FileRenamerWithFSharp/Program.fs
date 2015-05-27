// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System
open System.IO
open System.Text
open DecompiledMp3Lib
open DecompiledMp3Lib.Mp3Lib
open System.Linq

let parseInt value = 
    match Int32.TryParse(value) with
    | (true, x) -> Some(x)
    | (false, _) -> None

let parseTrack =
    let split (x:string) = x.Split('/')
    let first (array:'a[]) = array.[0]
    split >> first >> parseInt


let decode (value : string) = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(1252).GetBytes(value))
let encode (value : string) = Encoding.GetEncoding(1252).GetString(Encoding.GetEncoding(1251).GetBytes(value))

type TagModel = {Album : string; Artist : string; Song : string; Title : string; Track : int option}
type File = { Name : string; Path : string; TagModel : TagModel }
type Folder = { Name : string; Path : string; Children : Folder list; Files : File list }

let getFileName() =
    printfn "Enter root folder:" 
    let file = Console.ReadLine()
    file

let split (separator : char) (value : string) = value.Split(separator)

let codeTagModel coding tagModel =
    {
        Album = tagModel.Album |> coding;
        Artist = tagModel.Artist |> coding;
        Song = tagModel.Song |> coding;
        Title = tagModel.Title |> coding;
        Track = tagModel.Track
    }

let getTagModel (file : string) = 
    let loadedFile = new Mp3File(file)
    {
        Album = loadedFile.TagHandler.Album;
        Artist = loadedFile.TagHandler.Artist; 
        Song = loadedFile.TagHandler.Song; 
        Title = loadedFile.TagHandler.Title; 
        Track = loadedFile.TagHandler.Track |> parseTrack
    }
    
let arrayToList array = List.ofArray array

let collectDirectories root = 
    let getDirectoryName path = (split Path.DirectorySeparatorChar path |> Array.rev).[0]
    let directories x = x |> Directory.GetDirectories |> arrayToList
    let files x = Directory.GetFiles(x, "*.mp3")
    let rec makeFolder x = {
        Name = getDirectoryName x;
        Path = x;
        Children = x |> directories |> List.map makeFolder;
        Files = x |> files |> arrayToList |> List.map (fun x -> {Name = Path.GetFileNameWithoutExtension(x); Path = x; TagModel = x |> getTagModel})}//  |> codeTagModel decode}) }
    makeFolder root

let rec makePlainList (rootFolder : Folder) =
    match rootFolder.Children with
    | [] -> [rootFolder]
    | children -> children |> List.map makePlainList |> List.reduce (fun x y -> x@y)

let reduceToOne value =
    match value with
    | [] -> None
    | list -> match List.ofSeq(list.Distinct()) with
              | [] | _::_::_ -> None
              | first::[] -> Some (first)
             

let onlyParents (folders : Folder list) =
    let result = 
        folders
            |> List.filter (fun x-> x.Children.Length = 0 && x.Files.Length <> 0)
    result

let getCoding (folders : Folder list) =
    let isAllowed (symbols : char list) =
        symbols 
            |> List.map (fun x -> Char.IsLetterOrDigit(x) || Char.IsPunctuation(x) || Char.IsSeparator(x))
            |> List.fold ( fun state x -> state && x) true
    let stringToCharList (value : string option)=
        match value with
        | None -> []
        | Some string -> string.AsEnumerable() |> List.ofSeq
    folders
        |> List.map (fun x-> (
            x.Files 
            |> List.map (fun y -> y.TagModel.Album)
            |> List.filter (fun y -> String.IsNullOrWhiteSpace(y) = false)
            |> reduceToOne, x.Name))
        |> List.map ( fun (album, name) -> (album, name, album |> stringToCharList |> isAllowed))
        |> List.filter (fun (_, _, allowed) -> allowed = false)
        //|> List.map (fun (_, name) -> name)

let takeGoodFolders (folders : Folder list) =
    let result = 
        folders
            |> onlyParents
            |> List.map (fun x-> (
                x.Files 
                |> List.map (fun y -> y.TagModel.Album)
                |> List.filter (fun y -> String.IsNullOrWhiteSpace(y) = false)
                |> reduceToOne, x))
            |> List.map 
                (fun (album, folder) -> 
                    let equals =
                        match album with
                        | None -> false
                        | Some name -> name = folder.Name
                    (album, folder, equals))
            |> List.filter (fun (x, y, z) -> z = false)
            |> List.map (fun (x, y, z) -> (x, y.Name))
    result
    

[<EntryPoint>]
let main argv = 

    
    //let root = getFileName()
    let root = "C:\Users\Иван\Music"

    //let a = collectDirectories >> printfn "%A"
    let a = collectDirectories >> makePlainList >> getCoding >> printfn "%A"
    a root
    0 // return an integer exit code
