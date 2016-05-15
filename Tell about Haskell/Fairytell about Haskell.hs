{-# LANGUAGE ParallelListComp #-}

-- Чистые функции
add a b = a + b

-- Разные возможности записи: where, guard
solveEquation a b c
    | disc < 0  = Nothing
    | otherwise = Just ((- b - disc_sqrt)/(2*a), (- b + disc_sqrt)/(2*a))
                        where disc_sqrt = sqrt disc
                              disc = b**2 - 4*a*c

-- Функции старших порядков
compose f g x = f (g x)
ex_1 = (compose show (+1)) 1
-- 2

-- 


-- Алегебраические типы

data MyBool = MyTrue | MyFalse
data MyMaybe a = MyNothing | MyJust a

-- Сопоставление с образцом

boolToString MyTrue  = "true"
boolToString MyFalse = "false"

applyFunction f MyNothing  = MyNothing
applyFunction f (MyJust x) = MyJust (f x)

maybeToString MyNothing  = "Nothing"
maybeToString (MyJust x) = "Just " ++ show x

ex_2 = map boolToString [MyTrue, MyFalse, MyTrue]
-- ["true","false","true"]

ex_3 = map (maybeToString . applyFunction (+1)) [MyNothing, MyJust 1, MyJust 2, MyNothing]
-- ["Nothing","Just 2","Just 3","Nothing"]

-- Виды типов

class MyMonad m where
    return :: a -> m a
    (>>=)  :: m a -> (a -> m b) -> m b

-- Отложенность исполнения
fib = 1:[a + b | a <- 0:fib | b <- fib]

ex_4 = take 10 fib
