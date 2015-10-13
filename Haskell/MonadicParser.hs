{-# OPTIONS_GHC -fwarn-incomplete-patterns #-}
 
import Control.Monad
import Data.Char

newtype Parser a = Parser (String -> [(a, String)])

instance Functor Parser where
    fmap func p = Parser (\ cs -> [(func value, cs') | (value, cs') <- parser p cs])
    
instance Applicative Parser where
--  pure  :: a -> f a
--  (<*>) :: f (a -> b) -> f a -> f b

    pure x = Parser (\cs -> [(x, cs)])
    left <*> right = undefined

parser (Parser f) = f 

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

add (Parser left) (Parser right) = Parser (\cs -> (left cs) ++ (right cs))

 
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