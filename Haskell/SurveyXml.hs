import System.IO
import Control.Arrow
import Text.XML.HXT.Core
import Data.Tree.NTree.TypeDefs as Trees
import Data.Traversable
import XmlParsec
--import Text.XML.HXT.Parser.XmlParsec
-- deep (hasName tag) >>> 

safeHead []     = Nothing
safeHead (x:xs) = Just x

chain :: ArrowXml a => [String] -> a XmlTree XmlTree
chain [] = isElem
chain (start:xs) = foldr (\ x accum -> accum `concator` x) start' list
    where concator a b = a >>> getChildren >>> b
          start' = hasName start
          list =  map (\x -> hasName x ) (reverse xs)
          
orChain x = [ hasName name >>> arr f | (name, f) <- x]

data Node =    Open   String String
             | Single String String [String] 
             | Multi  String String [String] 
             | GenericNode [Node]
             
instance Show Node where
    show (Open _ _) = "Open"
    show (Single entityId name answers) = "Single : " ++ entityId ++ " : " ++ name ++ " : [" ++ (foldr (\x accum -> x ++ ":" ++ accum) "[]" answers) ++ "]"
    show (Multi _ _ _) = "Multi"
    show (GenericNode _ ) = "Not parsed"

    
childrenNamed x = getChildren >>> hasName x
embeddedText    = getChildren >>> getText


getEntityId  = getAttrValue "EntityId"
getMyName    = childrenNamed "Name" >>> embeddedText
getAnswers x = getChildren >>> hasName (x ++ "Answers") >>> childrenNamed "Answer" >>> getAttrValue "Precode"



constructSimpleQuestion questionConstructor blockName xmlTree =  
    do 
        entityId' <- entityId
        name'     <- name
        return (questionConstructor entityId' name')
    where
        entityId = safeHead $ runLA getEntityId xmlTree
        name     = safeHead $ runLA getMyName   xmlTree
           
constructQuestionWithAnswers questionConstructor blockName xmlTree =  
    do 
       simpleQuestion' <- simpleQuestion
       return (simpleQuestion' answers)
    where 
       simpleQuestion = constructSimpleQuestion questionConstructor blockName xmlTree
       answers = runLA (getAnswers blockName) xmlTree

       
constructOpen   xmlTree = constructSimpleQuestion      Open   "Open"   xmlTree
constructSingle xmlTree = constructQuestionWithAnswers Single "Single" xmlTree
constructMulti  xmlTree = constructQuestionWithAnswers Multi  "Multi"  xmlTree


process x = print . traverse id  $ runLA (xread >>> selector3 ) x
    where print (Just value) = show value
          print  Nothing     = "Sorry, some errors"


--selector3 :: LA (NTree XNode) XmlTree
selector3 = deep ( chain  ["Questionnaire", "Routing", "Nodes"] )
            >>> deep ( (hasName "Single" >>> arr constructSingle )
                         <+> 
                       (hasName "Multi"  >>> arr constructMulti  )
                         <+>
                       (hasName "Open"   >>> arr constructOpen   ))
                     -- >>> deep (
                                -- (hasName "Open" <+> hasName "Single")
                                -- getChildren >>> hasName "Name"
                                -- getChildren >>> getText ))

--chain "Questionnaire" ["Routing"] )
--selector1 = deep ( chain "Questionnaire" ["Routing", "Nodes"] )


selector2 :: LA (Trees.NTree XNode) String
selector2 = multi (hasAttrValue "EntityId" (\x -> True) ) >>> getAttrValue "EntityId"


main =
   do 
    handle <- openFile "C:\\Users\\IvanK\\Documents\\TempSurveys\\testsurvey.xml" ReadMode
    
    contents <- hGetContents handle
    
    putStr $ process contents
    putStrLn ""
    
    hClose handle
    