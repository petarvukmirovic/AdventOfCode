using Common;

namespace Fifteen
{
    internal class Program
    {
        static int GetHash(string value) =>
            value.Aggregate(0, (acc, c) => ((acc + c) * 17) & 0b11111111);

        static string[] ParseInput() =>
            Io.AllInputLines().Single().Split(',', Io.IgnoreEmptyElements).ToArray();

        public static void PartOne()
        {
            var initSequnce = ParseInput();
            var initSequenceHashSum = initSequnce.Select(GetHash).Sum();
            Console.WriteLine(initSequenceHashSum);
        }

        public static void PartTwo()
        {
            var initSequence = ParseInput();
            const int totalBoxes = 256;
            var hashMap = new List<(string label, int lens)>?[totalBoxes];
            foreach(var sequenceEl in initSequence)
            {
                if(sequenceEl.Contains('='))
                {
                    var split = sequenceEl.Split('=', Io.IgnoreEmptyElements);
                    AddNewLens(hashMap, split[0], int.Parse(split[1]));
                }
                else
                {
                    RemoveLens(hashMap, sequenceEl[..^1]);
                }
            }

            var focusingPower = ComputeFocusingPower(hashMap);
            Console.WriteLine(focusingPower);
        }

        private static long ComputeFocusingPower(List<(string label, int lens)>?[] boxes)
        {
            long totalFocusingPower = 0L;
            
            for(int boxIdx=0; boxIdx<boxes.Length; boxIdx++)
            {
                if (boxes[boxIdx] != null)
                {
                    for(int slotIdx=0; slotIdx < boxes[boxIdx]!.Count; slotIdx++)
                    {
                        totalFocusingPower += (boxIdx + 1) * (slotIdx + 1) * boxes[boxIdx]![slotIdx].lens;
                    }
                }
            }
            
            return totalFocusingPower;
        }

        private static void RemoveLens(List<(string label, int lens)>?[] hashMap, string label)
        {
            var labelHash = GetHash(label);
            var labelIdx = IndexOfLabel(hashMap[labelHash], label);
            if(labelIdx != null)
            {
                hashMap[labelHash]!.RemoveAt(labelIdx.Value);
            }
        }

        private static void AddNewLens(List<(string label, int lens)>?[] boxes, string label, int focalLength)
        {
            var labelHash = GetHash(label);
            var labelIdx = IndexOfLabel(boxes[labelHash], label);
            if (labelIdx != null)
            {
                boxes[labelHash]![labelIdx.Value] = (label, focalLength);
            }
            else
            {
                boxes[labelHash] ??= new();
                boxes[labelHash]!.Add((label, focalLength));
            }
        }

        private static int? IndexOfLabel(List<(string label, int lens)>? hashBox, string label)
        {
            if(hashBox == null)
            {
                return null;
            }

            for(int i=0; i<hashBox.Count; i++) 
            {
                if (hashBox[i].label == label)
                {
                    return i;
                }
            }
            return null;
        }

        static void Main(string[] args)
        {
            PartTwo();
        }
    }
}
