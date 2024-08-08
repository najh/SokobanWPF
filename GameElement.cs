namespace Sokoban
{
    //Game elements exist in a separate "layer" than the map.
    //For this reason we store the position.
    struct GameElement
    {
        internal ElementType Type;
        internal int Position_X;
        internal int Position_Y;
    }
}
