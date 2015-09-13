data SumMonoid a = SumMonoid a
    deriving Show

data Troi a = TLeft | TMiddle a | TRight deriving Show
instance Functor Troi where
    fmap func TLeft       = TLeft
    fmap func (TMiddle x) = TMiddle (func x)
    fmap func TRight      = TRight

instance Applicative Troi where
--  pure  :: a -> f a
--  (<*>) :: f (a -> b) -> f a -> f b

    pure x = TMiddle x
    
    (TMiddle func) <*> (TMiddle x) = TMiddle (func x)
    TLeft          <*> _           = TLeft
    _              <*> TLeft       = TLeft
    _              <*> _           = TRight
    

instance Num a => Monoid (SumMonoid a) where
    mempty = SumMonoid 0
    mappend (SumMonoid left) (SumMonoid right) = SumMonoid $ left + right
    
data MyList a = MyList [a]
    deriving Show

instance Functor MyList where
--  fmap        :: (a -> b) -> f a -> f b
    fmap func (MyList [])     = MyList []
    fmap func (MyList (x:xs)) = MyList $ (func x) : tailing
                                where (MyList tailing) = fmap func ( MyList ( xs ))
                                
instance Foldable MyList where
--  foldMap :: Monoid m => (a -> m) -> t a -> m
    --foldMap toMonoid (MyList [])     = ?
    foldMap toMonoid (MyList list) = mconcat (toMonoid `map` list)
    
    
-- instance Traversable MyList where
-- --   traverse  :: Applicative f => (a -> f b) -> t a -> f (t b)
     -- traverse _ (MyList [])     = pure []
     -- traverse toApplicative (MyList (x:xs))
         -- = (fmap (:) (f x)) <*> traverse f (MyList xs)
     
myList = MyList [-5..10]
-- test = traverse (\x -> if x < 0 then Just x else Nothing) myList
     
 