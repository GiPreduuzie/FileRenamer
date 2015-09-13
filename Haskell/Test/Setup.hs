import System.IO
import Control.Monad

main = do
        let list = []
        handle <- openFile "test.txt" ReadMode
        contents <- hGetContents handle
        singlewords <- (words contents)
        list <- f singlewords
        print list
        hClose handle

f :: [String] -> [Int]
f = map read