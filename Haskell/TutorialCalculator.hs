-- {-# OPTIONS_GHC -Wall #-}

import Data.Char
import qualified Data.Map as Map
import Control.Monad.State
import Control.Monad.Trans.Except

type SymTable = Map.Map String Double


data Operation = Plus | Minus | Times | Div
     deriving (Eq, Show)

data UnaryOperation = UnaryPlus | UnaryMinus
     deriving (Eq, Show)

     
data Token = TokenOperation Operation
           | TokenIdentifier String 
           | TokenNumber Integer
           | TokenAssign 
           | TokenLeftParen
           | TokenRightParen
           | TokenEnd
     deriving (Eq, Show)
     
data Tree = BinaryNode Operation Tree Tree
          | UnaryNode UnaryOperation Tree
          | AssignNode String Tree
          | NumNode Integer
          | VarNode String
          
    deriving (Show)
    
span' predicate value = 
     span'' ([], value)
      where
       span'' (good, []) = (good, [])
       span'' (good, (c:cs)) = 
                             if predicate c
                                    then span'' (good ++ [c], cs)
                                    else (good, c:cs)


tokenize :: String -> [Either String Token]
tokenize [] = []
tokenize (c:cs)
               | isAlpha c     = (Right $ let (good, tail) = span' isAlphaNum cs
                                         in TokenIdentifier (c:good))        : tokenize cs          
                                  
               | isDigit c     = (Right $ let (good, tail) = span' isDigit cs
                                         in TokenNumber (read (c:good)))     : tokenize cs
                                 
               | c == '='      = (Right $ TokenAssign                        ): tokenize cs
               | c == '('      = (Right $ TokenLeftParen                     ): tokenize cs
               | c == ')'      = (Right $ TokenRightParen                    ): tokenize cs
               | c == '+'      = (Right $ TokenOperation Plus                ): tokenize cs
               | c == '-'      = (Right $ TokenOperation Minus               ): tokenize cs
               | c == '*'      = (Right $ TokenOperation Times               ): tokenize cs
               | c == '/'      = (Right $ TokenOperation Div                 ): tokenize cs
               | isSpace c     = tokenize cs
               | otherwise     = [Left $ "Unknown tokens since: " ++ c:cs ]
                  
  
lookAhead [] = TokenEnd
lookAhead (c:_) = c

accept [] = error "Nothing to accept"
accept (t:ts) = ts

-- expression' tokens =
    -- let (termTree', tokens') = term tokens
    -- in case tokens' of
            -- []     ->  
            -- (x:xs) ->

expression tokens =
    let (termTree, tokens') = term tokens
    in case lookAhead tokens' of
           (TokenOperation operation) | elem operation [Plus, Minus] -> 
               let (termTree', tokens'') = expression (accept tokens')
               in (BinaryNode operation termTree termTree', tokens'')
           (TokenAssign) -> 
               case termTree of
                   VarNode variableName -> 
                           let (termTree', tokens'') = expression (accept tokens')
                           in (AssignNode variableName termTree', tokens'')
                   _ -> error "Only variables can be assigned to"
           _ -> (termTree, tokens')        
           
term tokens = 
    let (termTree, tokens') = factor tokens
    in case lookAhead tokens' of
        (TokenOperation operation) | elem operation [Times, Div] ->
                let (termTree', tokens'') = term (accept tokens')
                in (BinaryNode operation termTree termTree', tokens'')
        _ -> (termTree, tokens')

factor tokens = 
    case lookAhead tokens of
        (TokenNumber number)            -> (NumNode number, accept tokens)
        (TokenIdentifier variableName)  -> (VarNode variableName, accept tokens)
        (TokenOperation Plus)           -> let (factorTree, tokens') = factor (accept tokens)
                                           in (UnaryNode UnaryPlus factorTree, tokens')
        (TokenOperation Minus)          -> let (factorTree, tokens') = factor (accept tokens)
                                           in (UnaryNode UnaryMinus factorTree, tokens')
        TokenLeftParen                  -> 
            let (expressionTree, tokens') = expression (accept tokens)
            in if lookAhead tokens' /= TokenRightParen
                     then error "Missing right parenthesis"
                     else (expressionTree, accept tokens')
        _ -> error $ "Parse error on token: " ++ show tokens
        

factor' [] = Left "Parse error on token: end"
factor' (x:xs) =
    case x of
        (TokenNumber number)            -> Right (NumNode number, xs)
        (TokenIdentifier variableName)  -> Right (VarNode variableName, xs)
        (TokenOperation Plus)           -> do 
                                              (factorTree, tokens') <- factor' xs
                                              return (UnaryNode UnaryPlus factorTree, tokens')
                                              
        (TokenOperation Minus)          -> do 
                                              (factorTree, tokens') <- factor' xs
                                              return (UnaryNode UnaryMinus factorTree, tokens')
        -- TokenLeftParen                  -> 
            -- let (expressionTree, tokens') = expression xs
            -- in if lookAhead tokens' /= TokenRightParen
                     -- then error "Missing right parenthesis"
                     -- else (expressionTree, accept tokens')
        _ -> Left $ "Parse error on token: " ++ show xs
    
        
parse tokens = let (tree, tokens') = expression tokens
               in if null tokens' then tree
                                  else error $ "Still have some extra tokens: " ++ show tokens' 
                                  
takeUnaryOperation UnaryPlus  = id
takeUnaryOperation UnaryMinus = \x -> - x
                                  
takeBinaryOperation Plus  = (+)
takeBinaryOperation Minus = (-)
takeBinaryOperation Times = (*)
takeBinaryOperation Div   = (/)

evaluate :: Tree -> ExceptT String (State SymTable) Double                                  
evaluate (NumNode x) = return $ fromIntegral x

evaluate (UnaryNode operation tree) =
    do x <- evaluate tree
       return $ takeUnaryOperation operation x
      
evaluate (BinaryNode operation left right) =
    do left'  <- evaluate left
       right' <- evaluate right
       
       return $ takeBinaryOperation operation left' right'
        
evaluate (AssignNode variableName tree) =
    do result       <- evaluate tree
       symbolTable  <- get
       
       put (addSymbol variableName result symbolTable)
       return result 

evaluate (VarNode variableName) = 
    ExceptT $
    do 
       symbolTable <- get
       return $ lookUp variableName symbolTable
 
lookUp variableName symbolTable =
    case Map.lookup variableName symbolTable of
         Just value -> Right value
         Nothing    -> Left $ "Undefined variable " ++ variableName        
        
addSymbol variableName value symbolTable =
    Map.insert variableName value symbolTable
                                         
processInput value symbolTable = 
           do 
              tokens <- eitherTokens
              let expression = parse $ tokens
              return $ runState ( runExceptT (evaluate expression)) symbolTable
           where eitherTokens = traverse id $ tokenize value

process' input symbolTable = 
  do 
     putStrLn $ r'
     loop s'
  where eitherOutput = processInput input symbolTable
        (r', s') =
         case eitherOutput of
              Left x                       -> (x, symbolTable)
              Right (result, symbolTable') -> (show result, symbolTable')         
                                                    
loop symbolTable = do
           input <- getLine
           if input == [] then return ()
                          else process' input symbolTable
                                  
main = loop (Map.fromList [("pi", pi)])