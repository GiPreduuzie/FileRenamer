{-# OPTIONS_GHC -fwarn-incomplete-patterns #-}
 
import Control.Monad
import Data.Char

newtype Parser a = Parser (String -> [(a, String)])

instance Functor Parser where
    fmap func p = Parser (\ cs -> [(func value, cs') | (value, cs') <- parser p cs])
    
-- instance Applicative Parser where
-- --  pure  :: a -> f a
-- --  (<*>) :: f (a -> b) -> f a -> f b

    -- pure x = Parser (\cs -> [(x, cs)])
    -- left <*> right = undefined

parser (Parser f) = f 

-- class Monad m where
-- return :: a -> m a
-- (>>=) :: m a -> (a -> m b) -> m b

monadicFun a = a >>= (\ x1 -> return (x1 + 1) 
                   >>= (\ x2 -> return (show x2)
                     >>= (\ x3 -> return (x3 ++ "asdfasf" ++ (show x1)) )))
                     
monadicFun' a = do x1 <- a
                   let x2 = x1 + 1
                   let x3 = show x2
                   return (x3 ++ "asdfasf" ++ (show x1))
                   
monadList list1 list2 = 
 do 
    a <- list1
    b <- list2
    return (a,b)

                     


-- do 
 -- a <- List1
 -- b <- 

instance Monad Parser where
    return a = Parser (\cs -> [(a, cs)])
    p >>= f  = Parser (\cs -> concat [ parser (f value) cs' | (value, cs') <- parser p cs])
--instance 

item = Parser (\value -> 
                    case value of
                      []  ->  []
                      (c:cs) ->  [(c, cs)])
                     
probably predicate = Parser (\ value -> 
                               case value of 
                                 []     -> []
                                 (c:cs) -> if predicate c then [(c, cs)] else [])
                           
namely char = probably (char ==)
letter char = probably isLetter

string ""     = return ""
string (c:cs) = do 
                   x  <- namely c
                   xs <- string cs
                   return (x:xs)

many  p = many1 p `add` return [] 
many1 p = do
             result  <- p
             results <- many p
             return (result : results)

             
sepby  p sep = sepby1 p sep `add` return []
sepby1 p sep = do
                  result  <- p
                  results <- many (do {sep; p})
                  return (result:results)
                  
operator p f = do {p; return f}
                  
chainl p op a = chainl1 p op `add` return a

chainl1 p op = do
                  a <- p
                  rest' a
               where 
                   rest a = do 
                               f <- op 
                               b <- p
                               rest' (f a b)
                   rest' a = rest a `add` return a

add (Parser left) (Parser right) = Parser (\cs -> case (left cs) ++ (right cs) of 
                                                    [] -> []
                                                    (x:xs) -> [x])

-- expr   ::= expr addop term | term
-- term   ::= term mulop factor | factor
-- factor ::= digit | ( expr )
-- digit  ::= 0 | 1 | . . . | 9
-- addop  ::= + | -
-- mulop  ::= * | /



expr = chainl1 term   addop
term = chainl1 factor mulop
factor = digit `add` do {namely '('; x <- expr; namely ')'; return x}
digit =  do 
           letters <- many1 (probably isDigit)
           let digits = map (\x -> ord x - ord '0') letters
           return $ foldr (\ y accum -> accum*10 + y) 0 (reverse digits)
           
addop = operator (namely '+') (+) `add` operator (namely '-') (-) 
mulop = operator (namely '*') (*) `add` operator (namely '/') div 



--- Usage examples


main = 
    do
       x <- getLine
       if x == "" then return () else continue x
    where continue x = 
           do  
             let result = parser expr x
             putStrLn (show result)
             main
     

 
test1 value =
    let parser' =
         do
          first  <- item
          second <- namely 'o'
          _      <- namely 't'
          _      <- item
          third  <- item 
          return (first, second, third)
          
        parser'' =
         do
          _      <- item
          first  <- item
          second <- namely 'o'
          _      <- namely 't'
          _      <- item
          third  <- item 
          return (first, second, third)
          
          -- return first
    in (parser (parser' `add` parser'') ) value   --(parser parser')  value
