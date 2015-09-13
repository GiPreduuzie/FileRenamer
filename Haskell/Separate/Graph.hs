import Data.List

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

takeCost vertex edges = 
                 case ways of
                  [] -> Infinity
                  (x:xs) -> foldr min [] ways
                  where ways = [cost | (x, cost) <- edges, x == vertex]

getWayCosts edges vertecies = [(x, takeCost x edges) | x <- vertecies ]

buildVertex name mark mark' cost = Vertex name (min mark newMark) where newMark = addToMark mark' cost

newVertecies (Vertex name' mark') edgesWithCost = 
                 [buildVertex name mark mark' cost | ((Vertex name mark), cost) <- edgesWithCost]

getNewVertecies startVertex otherVertecies edges = 
                 newVertecies startVertex edgesWithCost
				   where
				    outgoingWays = getOutgoingWays startVertex edges
				    edgesWithCost = getWayCosts outgoingWays otherVertecies

finder startVertex otherVertecies edges = 
                 let vertecies' = getNewVertecies startVertex otherVertecies edges
			--	 in (foldl (:) [] (sort vertecies'))

				 in case (sort vertecies') of
				  []     -> []
				  (x:xs) -> x : (finder x xs edges)


s' = Vertex "1" (Value 0)
v' = map (\ x -> Vertex x Infinity) (map show [2..6])
e' = [(1, [(2,7),  (3,9),  (6,14)]),
      (2, [(3,10), (4,15)]),
	  (3, [(4,11), (6,2)]),
	  (4, [(5,6)]),
	  (5, [(6,9)])]

buildVertex' x = Vertex name Infinity where name = show x
selectMany f xs = foldl (\ x accum -> accum ++ (f x)) []

i  = [(buildVertex' x, edges) | x <- e']
ii = 

buildEdge vl vr cost = Edge (Vertex (show vl) Infinity) (Vertex (show vr) Infinity) cost


res = [ [ [buildEdge vl vr cost, buildEdge vr vl cost] | (vl, cost) <- es]  | (vr, es) <- e' ]

selectMany = foldl (++) []

edges' = selectMany (selectMany res)

result = s' : (finder s' v' edges')