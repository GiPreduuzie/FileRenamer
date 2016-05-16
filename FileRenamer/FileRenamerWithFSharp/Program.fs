// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System
open System.IO
open System.Text
open System.Linq
open DecompiledMp3Lib
open DecompiledMp3Lib.Mp3Lib

let split (separator : char) (value : string) = value.Split(separator)

let parseInt value = 
    match Int32.TryParse(value) with
    | (true, x) -> Some(x)
    | (false, _) -> None

let parseTrack =
    let first (array:'a[]) = array.[0]
    split '/' >> first >> parseInt

type Encoding = Win1251 | Win1252 | KOI8R
type ChangeEncoding = {From : Encoding; To : Encoding}
type Command = Nothing | ChangeEncoding of ChangeEncoding 

let decode (value : string) = Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(1252).GetBytes(value))
let encode (value : string) = Encoding.GetEncoding(1252).GetString(Encoding.GetEncoding(1251).GetBytes(value))

type TagModel = {Album : string; Artist : string; Song : string; Title : string; Track : int option}
type File = { Name : string; Path : string; TagModel : TagModel }
type Folder = { Name : string; Path : string; Children : Folder list; Files : File list }

let getFileName() =
    printfn "Enter root folder:" 
    let file = Console.ReadLine()
    file

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

type Album = 
    {Names  : string list
     Songs  : string list
     Artist : string list}

let addAlbums (a:Album) (b:Album) =
    {Names = a.Names @ b.Names;
     Songs = a.Songs @ b.Songs;
     Artist = a.Artist @ b.Artist}

    
    
let collectDirectories root = 
    let getDirectoryName path = (split Path.DirectorySeparatorChar path |> Array.rev).[0]
    let directories x = x |> Directory.GetDirectories |> List.ofArray

    let files x = Directory.GetFiles(x, "*.mp3")
    let rec makeFolder x = 
        let createFile x = {Name = Path.GetFileNameWithoutExtension(x); Path = x; TagModel = x |> getTagModel}
        let children = x |> directories |> List.map makeFolder
        let files = x 
                    |> files
                    |> List.ofArray
                    |> List.map createFile
                    //  |> codeTagModel decode}) }
        let folder =
            {
                Name = getDirectoryName x;
                Path = x;
                Children = children;
                Files = files
            }

        if children.Length = 0 && files.Length > 0 then printfn "%s" folder.Name

        folder
    makeFolder root

let rec makePlainList (rootFolder : Folder) =
    match rootFolder.Children with
    | [] -> [rootFolder]
    | children -> children |> List.map makePlainList |> List.reduce (@)

let reduceToOne value =
    match value with
    | [] -> None
    | list ->
         let distinctList = List.ofSeq(list.Distinct())
         match distinctList.Length with
              | 1 -> Some (distinctList.First())
              | _ -> None
             

let onlyParents (folders : Folder list) =
    let result = 
        folders
            |> List.filter (fun x-> x.Children.Length = 0 && x.Files.Length <> 0)
    result

type AnalisedWord = RussianWord of string | EnglishWord of string | Other

let getCoding (folders : Folder list) =
    let isCharAllowed (x : char) =
        Char.IsLetterOrDigit(x) || Char.IsPunctuation(x) || Char.IsSeparator(x)

    let isAllowed (symbols : char list) =
        symbols 
            |> List.map isCharAllowed
            |> List.fold (fun state x -> state && x) true

    let collectProhibitedSymbols (symbols : char list) =
        symbols 
            |> List.map (fun x -> (x, isCharAllowed x))
            |> List.filter (fun (x, allowed) -> allowed = false)
            |> List.map (fun (x, _ ) -> x)

    let inBetween a b x = a <= x && x <= b
    let isRussianLetter x = (inBetween 'а' 'я' x) || (inBetween 'А' 'Я' x)
    let isEnglishLetter x = (inBetween 'a' 'z' x) || (inBetween 'A' 'Z' x)
    let isDigit x = inBetween '0' '9' x
    let reduceBoolean x = x |> List.fold (fun state x -> state && x) true

    let analize (value:string option) =
        match value with
        | None -> []
        | Some x ->
            let words = x.Split(' ') |> List.ofArray
            let rec trimWord x = 
                match x with
                | [] -> []
                | head::tail -> if Char.IsPunctuation(head) then trimWord tail else x 
            let trim (x : string) = x.AsEnumerable() |> List.ofSeq |> List.rev |> trimWord |> List.rev
            
            let analizeWord f x = 
                x |> List.map f |> reduceBoolean
            let result =
                words 
                |> List.map trim 
                |> List.map (fun x ->  (x, analizeWord isEnglishLetter x, analizeWord isRussianLetter x, analizeWord isDigit x))
                |> List.filter (fun (_, a, b, c) -> a = false && b = false && c = false)
                |> List.map (fun (x, _, _, _) -> x)
            result |> List.map (fun x -> String.Join("", x))
        

    let stringToCharList (value : string option)=
        match value with
        | None -> []
        | Some string -> string.AsEnumerable() |> List.ofSeq
    folders
        |> List.map 
            (fun x-> (
                        x.Files 
                        |> List.map (fun y -> y.TagModel.Album)
                        |> List.filter (fun y -> String.IsNullOrWhiteSpace(y) = false)
                        |> reduceToOne, x.Name))
        |> List.map ( fun (album, name) -> (album, name, album |> analize))
        |> List.filter (fun ( album, _, allowed) -> match album with None -> false | Some _ -> true && match allowed with [] -> true | _ -> false)
        |> List.map (fun (x, y, _) -> ((match x with None -> None | Some value ->Some (decode value)), y))

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



// I. Ожидаемая модель
//  1. структура папок вида "Имя артиста"/"*номер*. Название альбома (комментарий)"/*номер*. название песни
//  2. если структура совпадает с указанной, ничего не далать
//  3. если нет, то подобрать варианты, показать пользователю, узнать его решение.
//  4. сохрать план обновления
//  5. загрузить и применить план по требованию

module SimpleAnalizer =

    let getDirectories x = x |> Directory.GetDirectories |> List.ofArray
    let getFiles x = Directory.GetFiles(x, "*.mp3") |> List.ofArray

    let rec collectDirectories wrap root =
        let directories = getDirectories root
        let result =
            match directories with
            | [] -> []
            | list -> list |> List.map (collectDirectories wrap) |> List.reduce (@)
        (root |> wrap result) :: result

    type Song = {Name : string}
    type SimpleFolder = { Path : string; Name : string; Songs : Song list; Embedded : SimpleFolder list}

    let wrapSong file = {Song.Name = file}
    let wrapDirectory childs root = {Path = root; Name = root; Songs = root |> getFiles |> List.map wrapSong; Embedded = childs}

    let rec ifGood folder = folder.Songs.Any() || folder.Embedded.Any(fun x -> ifGood x)
    let processF folders = 
        folders 
        |> List.filter ifGood
        |> List.collect (fun x -> x.Embedded |> List.map (fun y -> (x, y)))
        |> List.filter (fun (_, y) -> y.Songs.Any())
        

   

    let collect = collectDirectories wrapDirectory 



module Analizer =
    type DefaultArtist = string option
    type DefaultAlbum = 
        {Number : int option; Name : string option; Comment : string option} 

    type Brackets = 
        {
            OpenedCount : int;
            HeartSymbols : char list
        }

    let parseAlbum x =
        let unknown =  {Number = None; Name = None; Comment = None }
        let parseName (x:string) y = 
            let defaultVal = {Number = y; Name = Some(x); Comment = None }
            match split '[' x |> List.ofArray with
            | first::[] -> {Number = y; Name = Some(first.Trim()); Comment = None }
            | first::second::[] -> 
                  match second.AsEnumerable() |> List.ofSeq |> List.rev with
                  | ']'::list -> 
                            {Number = y;
                             Name = Some(first.Trim()); 
                             Comment = Some( list |> List.rev |> List.map (fun x -> x.ToString()) |> List.reduce (+))}

                  | _ -> {Number = y; Name = Some(x); Comment = None }
            | list -> defaultVal
        match x with
        | None -> unknown
        | Some value -> 
            match split '.' value |> List.ofArray with
            | first::[] -> parseName value None
            | first::tail ->  parseName (String.Join(".", tail)) (parseInt first)
            | [] -> unknown


    type DefaultContext = {Artist : DefaultArtist; Album : DefaultAlbum}

    let getDirectories x = x |> Directory.GetDirectories |> List.ofArray
    let getFiles x = Directory.GetFiles(x, "*.mp3") |> List.ofArray

    let getContext root =
        let folders = split Path.DirectorySeparatorChar root |> Array.filter (fun x -> x.EndsWith(":") |> not) |> List.ofArray
        match (folders |> List.rev) with
        | first::second::tail -> {Artist = Some(first); Album = second |> Some |> parseAlbum}
        | first::tail -> {Artist = Some(first); Album = first |> Some |> parseAlbum}
        | [] -> {Artist = None; Album = None |> parseAlbum }

    let wrapFile x = 
        {
            Name = Path.GetFileNameWithoutExtension(x);
            Path = x;
            TagModel = x |> getTagModel
        }

    let wrapDirectory childs root =
        let getDirectoryName path = (split Path.DirectorySeparatorChar path |> Array.rev).[0]
        {
            Name = getDirectoryName root;
            Path = root;
            Children = childs;
            Files = root |> getFiles |> List.map wrapFile
        }

    //let parsedName = parseAlbum (x.Name |> Some)

    let songTrackIsCorrect (x:TagModel) (y:DefaultAlbum) (z:DefaultContext) =
        let track = x.Track
        if track <> y.Number
            then Some ("Track is not correct: " + track.ToString()) 
            else None

    let songTitleIsCorrect (x:TagModel) (y:DefaultAlbum) (z:DefaultContext) =
        let title = x.Title
        match y.Name with
        | None -> Some ("Title is not correct.")
        | Some (value) ->
            if title <> value
                then Some ("Title is not correct: " + title.ToString()) 
                else None

    let songArtistIsCorrect (x:TagModel) (y:DefaultAlbum) (z:DefaultContext) =
        let artist = x.Artist
        match z.Artist with
        | None -> Some ("Artist is not correct.")
        | Some (value) ->
            if artist <> value
                then Some ("Artist is not correct: " + artist.ToString()) 
                else None


    let rec collectDirectories wrap root =
        let directories = getDirectories root
        let result =
            match directories with
            | [] -> []
            | list -> list |> List.map (collectDirectories wrap) |> List.reduce (@)
        (root |> wrap result) :: result

    let collect = collectDirectories wrapDirectory 

        

[<EntryPoint>]
let main argv = 

    
    //let root = getFileName()
    let root = "C:\Users\Иван\Music"
    let otherRoot = "C:\Users\Иван\YandexDisk\Музыка"

    //let a = collectDirectories >> printfn "%A"
    otherRoot |> Analizer.collect |> printfn "%A"
    0 // return an integer exit code
