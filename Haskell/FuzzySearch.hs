import Data.List

fromWord = "dagestan"
toWord = "arestant"

addCost    x = 1
deleteCost x = 1
-- addCost Nothing   = 1 

-- deleteCost (Just _)  = 1
-- deleteCost Nothing   = 1

--weightCell add delete substitute = min (min (add + addCost $ ) (delete + deleteCost)) substitute--


oneWeight xLetter yLetter b c a = min (min (a + (addCost xLetter)) (c + (deleteCost yLetter))) substitute
                                  where substitute = b + if xLetter == yLetter then 0 else 1

                                  
weightOneLine xLetter yLetter xLetters xBound xArray cross =
    let oneWeightLettered = oneWeight yLetter
        xPairs = [(b,c) | (b, c) <- zip (xBound : xArray) (xArray ++ [cross])] 
        xTriples = [(a, b, c) | (a, (b, c)) <- zip (xLetters ++ [xLetter]) xPairs] 
        toWeights = [oneWeightLettered a b c | (a, b, c) <- xTriples] 
    in scanl (\accum f -> f accum) (xBound + 1) toWeights
    

                                  
--weight from to =
block xLetter yLetter xLetters yLetters xBound yBound xArray yArray cross =
    let xLine  = weightOneLine xLetter yLetter xLetters xBound xArray cross
        yLine  = weightOneLine yLetter xLetter yLetters yBound yArray cross
        xCross = head . reverse $ xLine
        yCross = head . reverse $ xLine
        cross' = oneWeight xLetter yLetter cross yCross xCross
    in (head xLine, tail xLine, cross', head yLine, tail yLine)
    
    --in xTriples
        -- toWeights = [oneWeight xLetter b c | (xLetter, (b, c)) <- zip xPairs xLetters] 
    -- in scanl (\x accum -> x accum) (xLetter + 1) toWeights
    --zip xLetter xArray
    

longZip left right =
    let longZip' (x:xs) (y:ys) = (Just x,    Just y)  : longZip' xs ys
        longZip' []     (y:ys) = (Nothing,   Just y)  : longZip' [] ys
        longZip' (x:xs) []     = (Just x,    Nothing) : longZip' xs []
        longZip' []     []     = []
    in longZip' left right
    
do' left right =
    let zipped = longZip (left) (right)
        
        step (xLetter, yLetter) (xLetters, yLetters, xBound, xLine, cross, yBound, yLine) =
           let (xBound', xLine', cross', yBound', yLine') = block xLetter yLetter xLetters yLetters xBound yBound xLine yLine cross
           in (xLetters ++ [xLetter], yLetters ++ [yLetter], xBound', xLine', cross', yBound', yLine')
        
        do'' (pair:pairs) accum = do'' pairs accum' where accum' = step pair accum
        do'' [] accum = accum
        
        --result = do'' zipped ([Just ' '], [Just ' '], 1, [], 0, 1, [])
        (_, _, _, _, result, _, _) = do'' zipped ([Just ' '], [Just ' '], 1, [], 0, 1, [])
    in result
    
--test = block 'e' 'e' "ar" "dag" 3 3 [2,2] [3,3] 3
test1 = do' fromWord toWord
test3 = do' "" ""


test4 list =
 let list' = do
              x <- list
              y <- list
              
              return (x, y, (do' x y))
 in take 20 (filter (\ (_, _, x) -> x /= 0) (sortOn (\ (_, _, x) -> x) list' ))
 
 
 
exampleList = ["Rakovina",
               "Gorod",
               "Zima",
               "Lazeyka",
               "Yazyik",
               "Oduvanchik",
               "Na postsovetskom",
               "Peyzazh",
               "Pervyiy skazochnik",
               "Ptitsyi",
               "Kochevnik",
               "Stolnyiy gorod",
               "Tolko vpered",
               "Ryitsar i drakon",
               "Utoplennik",
               "O kotah i kryisah",
               "Molchalivyie teni",
               "Osvobozhdenie Osventsima",
               "Na rubezhe",
               "Ukroschenie konya",
               "Chayka",
               "Poslyi",
               "Poema mednogo kolossa",
               "Kartonnyiy nemets",
               "Vesna",
               "Prospekt",
               "U severnogo morya",
               "Trolleybus",
               "Pesenka zhYoludya",
               "Razmyishleniya za chaem",
               "Ryinok",
               "Angel",
               "XXI vek",
               "Ptichiy zamok",
               "Za naslednika prestola",
               "Rzhavyie latyi",
               "Dekadentskaya muza",
               "Les",
               "Step",
               "Letnee",
               "Pustyinya",
               "Mashinnyiy kod",
               "Bezbiletnik",
               "Opyat osen",
               "Iggdrasil"]
    
