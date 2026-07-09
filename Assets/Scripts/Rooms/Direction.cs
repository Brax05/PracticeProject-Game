namespace TravesiaACasa.Rooms
{
    /// <summary>
    /// Direcciones cardinales usadas para conectar celdas del grid
    /// y para saber por qué borde entra el jugador a una nueva room.
    /// </summary>
    public enum Direction
    {
        North,
        South,
        East,
        West
    }

    public static class DirectionUtils
    {
        /// <summary>
        /// Devuelve la dirección opuesta. Útil para saber en qué borde
        /// de la nueva room debe aparecer el jugador.
        /// Ej: si entraste por el borde North de la room nueva,
        /// significa que veniste desde el South de la anterior.
        /// </summary>
        public static Direction Opposite(Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.East: return Direction.West;
                case Direction.West: return Direction.East;
                default: return dir;
            }
        }
    }
}
