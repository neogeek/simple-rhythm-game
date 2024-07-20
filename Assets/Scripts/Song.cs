using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public struct Note
{

    public int Position;

    public int HandPosition;

    public int BPM;

}

public struct BeatBar
{

    public int Position;

    public int BPM;

    public int[] TimeSignature;

}

public enum Difficulty
{

    Easy,

    Medium,

    Hard,

    Expert

}

public class Song
{

    private const float SECONDS_PER_MINUTE = 60.0f;

    public int Resolution;

    public string MusicStream;

    public readonly Dictionary<Difficulty, Note[]> Difficulties = new();

    public readonly Dictionary<int, int> BPM = new();

    public readonly Dictionary<int, int[]> TimeSignatures = new();

    private Dictionary<int, int> _sortedBPM = new();

    public Dictionary<int, int> sortedBPM
    {
        get
        {
            if (_sortedBPM.Count != BPM.Count)
            {
                _sortedBPM = new Dictionary<int, int>(BPM.OrderBy(b => b.Key));
            }

            return _sortedBPM;
        }
    }

    private int _initialBPM;

    public int initialBPM
    {
        get
        {
            if (_initialBPM == 0)
            {
                _initialBPM = BPM.First().Value / 1000;
            }

            return _initialBPM;
        }
    }

    private List<BeatBar> _beatBars = new();

    public List<BeatBar> beatBars
    {
        get
        {
            if (_beatBars.Count == 0)
            {
                _beatBars = CalculateBeatBars(BPM);
            }

            return _beatBars;
        }
    }

    public static Song FromJSON(string input)
    {
        return JsonConvert.DeserializeObject<Song>(input);
    }

    public static float ConvertTickToPosition(float tick, int resolution)
    {
        return tick / resolution;
    }

    public static int ConvertSecondToTicks(float seconds, int resolution, Dictionary<int, int> bpmChanges)
    {
        var bpmChangesEnumerator = bpmChanges.GetEnumerator();

        bpmChangesEnumerator.MoveNext();

        var totalTicks = 0;
        var remainingSeconds = seconds;
        var previousTick = 0;
        var previousBPM = bpmChangesEnumerator.Current.Value / 1000;

        foreach (var (currentTick, value) in bpmChanges)
        {
            var timeForSegment = (currentTick - previousTick) / (resolution * previousBPM / SECONDS_PER_MINUTE);

            if (remainingSeconds <= timeForSegment)
            {
                totalTicks += (int)(remainingSeconds * previousBPM / SECONDS_PER_MINUTE * resolution);

                return totalTicks;
            }

            totalTicks += currentTick - previousTick;
            remainingSeconds -= timeForSegment;
            previousTick = currentTick;
            previousBPM = value / 1000;
        }

        totalTicks += (int)(remainingSeconds * previousBPM / SECONDS_PER_MINUTE * resolution);

        return totalTicks;
    }

    public static List<BeatBar> CalculateBeatBars(Dictionary<int, int> bpm, int resolution = 192, int ts = 4,
        bool includeHalfNotes = true)
    {
        var newBpm = new List<BeatBar>();

        var keyValuePairs = GenerateAdjacentKeyPairs(bpm);

        foreach (var keys in keyValuePairs)
        {
            var startTick = keys[0];
            var endTick = keys[1];

            for (var tick = startTick; tick <= endTick; tick += resolution)
            {
                newBpm.Add(new BeatBar { Position = tick, BPM = bpm[startTick], TimeSignature = new[] { ts } });

                if (includeHalfNotes && tick != endTick)
                {
                    newBpm.Add(new BeatBar
                    {
                        Position = tick + resolution / 2, BPM = bpm[keys[0]], TimeSignature = new[] { 4 }
                    });
                }
            }
        }

        return newBpm;
    }

    public static List<int[]> GenerateAdjacentKeyPairs<T>(Dictionary<int, T> dictionary)
    {
        var keys = dictionary.Keys.ToList();

        keys.Sort();

        var adjacentKeyPairs = new List<int[]>();

        for (var i = 0; i < keys.Count - 1; i += 1)
        {
            adjacentKeyPairs.Add(new[] { keys[i], keys[i + 1] });
        }

        return adjacentKeyPairs;
    }

    public static int RoundUpToTheNearestMultiplier(int value, int multiplier)
    {
        return (int)Math.Ceiling((float)value / multiplier) * multiplier;
    }

}
