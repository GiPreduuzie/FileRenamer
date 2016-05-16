import Data.List
import Control.Monad

--selectMany f xs = foldl (\ x accum -> accum ++ (f x)) []
--selectMany = foldl (++) []

data Mark = Infinity | Value Integer
instance Show Mark where
        show = showMark
instance Eq Mark where
        (==) a b = (compareMark a b) == EQ
instance Ord Mark where
	compare = compareMark

showMark Infinity  = "Infinity"
showMark (Value x) = show x

compareMark Infinity Infinity   = EQ
compareMark Infinity _          = GT
compareMark _        Infinity   = LT
compareMark (Value a) (Value b) = compare a b

addToMark Infinity  _          = Infinity
addToMark _         Infinity   = Infinity
addToMark (Value a) (Value b)  = Value (a + b)
     

mark1 = Value 10 
mark2 = Value 4
mark3 = Infinity

marks =  mark1 : mark2 : mark3: []
markPairs = [(a, b) | a <- marks, b <- marks]


data Vertex = Vertex String Mark
instance Show Vertex where
	show (Vertex name mark) = name ++ " " ++ (show mark)
instance Eq Vertex where
	(==) (Vertex name mark) (Vertex name' mark') = name == name'
instance Ord Vertex where
	compare (Vertex name mark) (Vertex name' mark') = compare mark mark'
 
data Edge = Edge Vertex Vertex Integer

otherVertecies = (Vertex "1" (Value 0)) : (map (\ x -> Vertex x Infinity) (map show [2..6]))

getOutgoingWays vertex edges = [ (b, Value cost) | (Edge a b cost) <- edges, a == vertex]

takeCost :: Eq a => a -> [(a, Mark)] -> Mark
takeCost vertex edges = 
                 case ways of
                  []     -> Infinity
                  (x:xs) -> foldr min x xs
                  where ways = [cost | (x, cost) <- edges, x == vertex]

                  
getWayCosts :: Eq a => [(a, Mark)] -> [a] -> [(a, Mark)]
getWayCosts edges vertecies = map  (\x -> (x, takeCost x edges)) vertecies

buildVertex:: String -> Mark -> Mark -> Mark -> Vertex
buildVertex name mark mark' cost = Vertex name (min mark newMark) where newMark = addToMark mark' cost

--newVertecies (Vertex _ mark') edgesWithCost = [buildVertex name mark mark' cost | ((Vertex name mark), cost) <- edgesWithCost]


newVertecies mark' ((Vertex name mark), cost) = buildVertex name mark mark' cost

-- newVertecies :: Vertex -> [(Vertex, Mark)] -> [Vertex]
-- newVertecies (Vertex _ mark') = map (\((Vertex name mark), cost) -> buildVertex name mark mark' cost)

getNewVertecies startVertex edges = 
                      (newVertecies . getMark $ startVertex) . (addCost)
				           where
                            outgoingWays = getOutgoingWays startVertex edges
                            addCost x = (x, takeCost x outgoingWays)
                            getMark (Vertex _ x) = x
                            
                            
doer [] = []
doer (x:xs) = x : (sort xs)

                            
finder' startVertex  getNewVertecies' otherVertecies = 
                 let vertecies' = getNewVertecies' startVertex otherVertecies
			     in case vertecies' of
				  []     -> []
				  (x:xs) ->  y : (finder' y  getNewVertecies' ys) where (y:ys) = sort (x:xs)
                  
finder :: Vertex -> [Vertex] -> [Edge] -> [Vertex]
finder startVertex otherVertecies edges =     
    finder' startVertex (\x -> map $ getNewVertecies x edges) $ otherVertecies
                  



s' = Vertex "1" (Value 0)
v' = map (\ x -> Vertex x Infinity) (map show [2..6])
e' = [(1, [(2,7),  (3,9),  (6,14)]),
      (2, [(3,10), (4,15)]),
	  (3, [(4,11), (6,2)]),
	  (4, [(5,6)]),
	  (5, [(6,9)])]

buildVertex' x = Vertex name Infinity where name = show x


--i  = [(buildVertex' x, edges) | x <- e']
--ii = 

buildEdge vl vr cost = Edge (Vertex (show vl) Infinity) (Vertex (show vr) Infinity) cost


res buildEdge = [ [ [buildEdge vl vr cost, buildEdge vr vl cost] | (vl, cost) <- es]  | (vr, es) <- e' ]

result = s' : (finder s' v' edges') where edges' = join . join $ res buildEdge



