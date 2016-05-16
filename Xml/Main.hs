import System.IO
import Control.Arrow 
import Text.XML.HXT.Core
--import Text.XML.HXT.Parser.XmlParsec
import qualified Text.XML.HXT.Parser.HtmlParsec as H


-- div class="wall_post_text" (with br) \ span

-- endLine n = foldl (++) "" result where result = replicate n "\n<p></p>"
-- line x = "<p>" ++ x ++ "</p>"

line x = x
endLine n = foldl (++) "" (replicate n "\n")

process file = 
   
   foldl (\ (count, text) a -> (count-1, a ++ (endLine 4) ++ (show count)++ line " :=================" ++ (endLine 4) ++ text))
                                       (count, "") result 
   where 
    count = length result 
    result = runLA (
            deep $ hasAttrValue "class" (== "wall_post_text")
            >>> arr getVerses) ((H.parseHtmlDocument "" file) !! 0)
           
getVerses post = 
    line (foldl (\ a b -> a  ++ b) "" result)
    where
      result = runLA (
        (getChildren >>> ((isText >>> getText) 
                          <+> 
                        ((hasName "br") >>> arr (\ _ -> endLine 1))))
        <+>
        (getChildren >>> 
        hasName "span" >>> (getChildren >>> ((isText >>> getText) 
                             <+> 
                           ((hasName "br") >>> arr (\ _ -> endLine 1)))))
                        
                        
                        
                        )  post
           -- deep      ((isText >>> getText) 
                       -- <+> 
                     -- ((hasName "br") >>> arr (\ _ -> endLine 1))))  post
      -- result = runLA ( (deep isText >>> getText) <+> ((deep $ hasName "br") >>> arr (\ _ -> "\n")))  post

main = 
       do
        handle <- openFile "C:\\Temp\\in.txt" ReadMode
        contents <- hGetContents handle
        let (_, result) = process contents
        writeFile "C:\\Temp\\out.txt" result
        --print result
        hClose handle 
        