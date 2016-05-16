data Frac = Frac Integer Integer
instance Show Frac where
    show (Frac a b) 
        | b == 0    = "Division by zero"
        | b == 1    = show a
        | otherwise = show a ++ "/" ++ show b
instance Num Frac where
    (+) = addf
    (*) = mulf
    abs (Frac a b) = Frac (abs a) (abs b)
    signum (Frac a b) = fromInteger $ (signum a) * (signum b)
    fromInteger x = Frac x 1
    negate (Frac a b) = Frac (-a) bs
    
instance Fractional Frac where
    recip = inverse
    fromRational x = Frac (round x) 1
   

toFraction x = reduce $ Frac x 1

addf a b = arithmetic (+) a b 
subf a b = arithmetic (-) a b 

mulf (Frac leftA leftB) (Frac rightA rightB) = reduce $ Frac (leftA*rightA) (leftB*rightB)
divf a b = a `mulf` (inverse b)

inverse (Frac a b) = Frac b a

arithmetic f (Frac leftA leftB) (Frac rightA rightB) = reduce $ Frac a b where 
    b = leftB*rightB
    a = f (leftA*rightB) (rightA*leftB)
    

    
reduce (Frac a b) =  if factor == 0 then Frac 0 1 else Frac (a `div` factor) (b `div` factor) where factor = nod a b
    
nod a b = 
    if a == 0 || b == 0 then 0 else nod' (abs a) (abs b)
    where nod' a b 
           | a <  b = nod' a (b-a)
           | a == b = a
           | a >  b = nod' (a-b) a

matrixReduce' rows = matrixReduce rows' where rows' = map (\x -> map toFraction x) rows
       
matrixReduce [] = []
matrixReduce (row:rows) = 
    
    row : addNullColumn(matrixReduce (removeFirstColumn row rows))
    where 
        reduceRow ar br = [ b - a * factor | (a, b) <- zip ar br ] where factor = head br / head ar 
        reduceRows row rows = map (\ x -> reduceRow row x ) rows
        removeFirstColumn row rows = map tail (reduceRows row rows)
        addNullColumn rows = map (\x -> 0 : x) rows
        

    
    
result = Frac 1 2 `addf` Frac 3 4