// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System
open System.IO
open Mp3Lib;


//Console.WriteLine("Album: \t" + Decode(fileMp3.TagHandler.Album));
//            Console.WriteLine("Artist:\t" + Decode(fileMp3.TagHandler.Artist));
//            Console.WriteLine("Song: \t" + Decode(fileMp3.TagHandler.Song));
//            Console.WriteLine("Title: \t" + Decode(fileMp3.TagHandler.Title));
//            Console.WriteLine("Track: \t" + Decode(fileMp3.TagHandler.Track));

type TagModel = {Album : string; Artist : string; Song : string; Title : string; Track : int}
type File = { Name : string; Path : string; TagModel : TagModel }
type Folder = { Name : string; Path : string; Children : Folder[]; Files : File[] }

let getFileName() =
    printfn "Enter root folder:" 
    let file = Console.ReadLine()
    file

// return absoluteLocation.Split(Path.DirectorySeparatorChar).Reverse().Take(1).Single();
//Directory.GetFiles(rootPath, "*.mp3")

let split (separator : char) (value : string) = value.Split(separator)

let getTagModel (file : string) = 
    let loadedFile = new Mp3File(file)
    {
        Album = loadedFile.TagHandler.Album;
        Artist = loadedFile.TagHandler.Artist; 
        Song = loadedFile.TagHandler.Song; 
        Title = loadedFile.TagHandler.Title; 
        Track = loadedFile.TagHandler.Track |> Int32.Parse
    }
    

let rec collectDirectories root = 
    let directories = Directory.GetDirectories(root)
    let getDirectoryName path = (split Path.DirectorySeparatorChar path |> Array.rev).[0]
    let makeFolder x = {
        Name = getDirectoryName x;
        Path = x;
        Children = collectDirectories x;
        Files =  Directory.GetFiles(root, "*.mp3") |> Array.map (fun x -> {Name = Path.GetFileNameWithoutExtension(x); Path = x; TagModel = getTagModel x}) }
    let folders = Array.map makeFolder directories
    folders
    

[<EntryPoint>]
let main argv = 

    let a = [|1..4|] |> Array.rev
    //let root = getFileName()
    let root = "C:\Users\Иван\Music"
    printfn "%A" (collectDirectories root)
    0 // return an integer exit code
