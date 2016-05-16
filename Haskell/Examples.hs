{-# OPTIONS_GHC -fwarn-incomplete-patterns #-}
-- леность
-- pattern matching
-- функции как основа: чистота, карирование, функции высших порядков, лямбда-выражения
-- неизменяемость значений
-- легкая расщепляемость
-- классы
-- алгебраические типы
-- выражения вместо утверждений

square x = x*x

abs x = if x >0 then x else -x

fun True  = 10
fun False = 20


       


fun'' a1 a2 a3 a4 = a1+a2 + r where r = a3+a4

fun''' x y = fun'' x y 1 2

a = 1


data Maybe' a = Just' a | Nothing'
-- data List a = [] | (:) List a

data List' a = EmptyList | Value (List' a, a)

customList = Value (Value (EmptyList, 1), 2)

showList' EmptyList = ""
showList' (Value (xs, x)) = show x ++ showList' xs




algebraType  x = Just' x
algebraType' x = Nothing'

processor Nothing'  = "Nothing"
processor (Just' a) = show a




factorial 0 = 1
factorial 1 = 1
factorial x = x * factorial (x-1)

factorial' n = foldr (\x accum -> accum * x) 1 [1..n]


-- factorial'' :: Integer -> Maybe Integer
factorial'' x
              | x <  0    = Nothing
              | otherwise = Just (factorial x)
              
              
              
do a <- List1
   b <- List2
   return (a,b)


-- f    = g square
-- f'   = g factorial
-- f''  = g factorial'
-- f''' = undefined -- traverse id (g factorial'')

-- g f = (take 10) . (map f) $ [1..]

-- bind (Just x) f = Just (f x)
-- bind Nothing  _ = Nothing

-- bind'  = f''' >>= (\ x -> Just $ map (\y -> (show y) ++ "_?") x)

-- bind'' = do 
            -- x <- f'''
            -- return $ do
                        -- y <- x 
                        -- return $ show y ++ "_?"
            
            

