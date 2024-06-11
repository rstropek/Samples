using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TrainStation.Tests")]

namespace TrainStation;

/// <summary>
/// Represents the two possible entries to a track.
/// </summary>
/// <remarks>
/// Most of our tracks can be entered from the east or the west.
/// Some tracks are one-way only, and can only be entered from one side.
/// </remarks>
[Flags]
enum TrackAccess
{
    East = 1,
    West = 2,
    Both = East | West
}

enum Direction
{
    East,
    West
}

/// <summary>
/// Represents a track in the train station.
/// </summary>
/// <remarks>
/// During switching operations, train wagons (see <see cref="Wagon"/>)
/// are added to or removed from a track. Internally, the track uses
/// a linked list to keep track of the wagons. Wagons can only be added
/// or removed according to the track's access restrictions (see <see cref="TrackAccess"/>).
/// </para>
/// </remarks>
class Track(TrackAccess access, int capacity)
{
    internal record WagonConnection(Wagon Wagon) { public WagonConnection? Next { get; set; } }

    /// <summary>
    /// The first wagon (from east).
    /// </summary>
    /// <remarks>
    /// If a wagon is added or removed from the east, this field is updated.
    /// If a wagon is added or removed from the west, this field is used to
    /// find the last wagon in the list and update it accordingly.
    /// </remarks>
    internal WagonConnection? First { get; set; }

    internal int NumberOfWagons { get; set; }

    public TrackAccess Access { get; } = access;

    /// <summary>
    /// Adds a wagon to the track.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Track cannot be accessed from the given direction.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The track is full (i.e. the number of wagons on the track is equal to the track's capacity).
    /// </exception>
    public void AddWagon(Wagon wagon, Direction from)
    {
        if (from == Direction.East)
        {
            if (!Access.HasFlag(TrackAccess.East))
            {
                throw new InvalidOperationException("Track can't be accessed from the east.");
            }

            if (NumberOfWagons == capacity)
            {
                throw new InvalidOperationException("Track is full.");
            }

            var newWagon = new WagonConnection(wagon) { Next = First };
            First = newWagon;
            NumberOfWagons++;
            return;
        }
        else
        {
            if (!Access.HasFlag(TrackAccess.West))
            {
                throw new InvalidOperationException("Track can't be accessed from the west.");
            }

            if (NumberOfWagons == capacity)
            {
                throw new InvalidOperationException("Track is full.");
            }

            if (First is null)
            {
                First = new WagonConnection(wagon);
                NumberOfWagons++;
                return;
            }

            var last = First;
            while (last.Next is not null)
            {
                last = last.Next;
            }

            last.Next = new WagonConnection(wagon);
            NumberOfWagons++;
            return;
        }
    }

    /// <summary>
    /// Remove a wagon from the track.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Track cannot be accessed from the given direction.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The track is empty.
    /// </exception>
    public void RemoveWagon(Direction from)
    {
        if (from == Direction.East)
        {
            if (!Access.HasFlag(TrackAccess.East))
            {
                throw new InvalidOperationException("Track can't be accessed from the east.");
            }

            if (First is null)
            {
                throw new InvalidOperationException("Track is empty.");
            }

            First = First.Next;
            NumberOfWagons--;
            return;
        }
        else
        {
            if (!Access.HasFlag(TrackAccess.West))
            {
                throw new InvalidOperationException("Track can't be accessed from the west.");
            }

            if (First is null)
            {
                throw new InvalidOperationException("Track is empty.");
            }

            if (First.Next is null)
            {
                First = null;
                NumberOfWagons--;
                return;
            }

            var current = First;
            while (current.Next!.Next is not null)
            {
                current = current.Next;
            }

            current.Next = null;
            NumberOfWagons--;
            return;
        }
        
    }

    /// <summary>
    /// The train leaves the track.
    /// </summary>
    /// <param name="to">Direction to which the train leaves</param>
    /// <exception cref="InvalidOperationException">
    /// Track cannot be accessed from the given direction.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Train does not contain at least one locomotive.
    /// Note that the locomotive can be anywhere in the train,
    /// not necessarily at the front or back.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Track is empty.
    /// </exception>
    /// <remarks>
    /// If the method is successful, the track is empty after
    /// the method returns.
    /// </remarks>
    public void Leave(Direction to)
    {
        if (to == Direction.East)
        {
            if (!Access.HasFlag(TrackAccess.East))
            {
                throw new InvalidOperationException("Track can't be accessed from the east.");
            }

            if (First is null)
            {
                throw new InvalidOperationException("Track is empty.");
            }

            var current = First;
            while (current is not null)
            {
                if (current.Wagon.Type == WagonType.Locomotive)
                {
                    First = null;
                    NumberOfWagons = 0;
                    return;
                }

                current = current.Next;
            }

            throw new InvalidOperationException("Train does not contain at least one locomotive.");
        }
        else
        {
            if (!Access.HasFlag(TrackAccess.West))
            {
                throw new InvalidOperationException("Track can't be accessed from the west.");
            }

            if (First is null)
            {
                throw new InvalidOperationException("Track is empty.");
            }

            var current = First;
            while (current.Next is not null)
            {
                current = current.Next;
            }

            while (current is not null)
            {
                if (current.Wagon.Type == WagonType.Locomotive)
                {
                    First = null;
                    NumberOfWagons = 0;
                    return;
                }

                current = current.Next;
            }

            throw new InvalidOperationException("Train does not contain at least one locomotive.");
        }
    }
}
