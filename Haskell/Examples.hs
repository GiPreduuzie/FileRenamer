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


factorial 0 = 1
factorial 1 = 1
factorial x = x * factorial (x-1)

factorial' n = foldr (\x accum -> accum * x) 1 [1..n]


factorial'' :: Integer -> Maybe Integer
factorial'' x
              | x <  0    = Nothing
              | otherwise = Just (factorial' x)


f    = g square
f'   = g factorial
f''  = g factorial'
f''' = traverse id (g factorial'')

g f = (take 10) . (map f) $ [1..]

bind (Just x) f = Just (f x)
bind Nothing  _ = Nothing

bind'  = f''' >>= (\ x -> Just $ map (\y -> (show y) ++ "_?") x)

bind'' = do 
            x <- f'''
            return $ do
                        y <- x 
                        return $ show y ++ "_?"
            
            

