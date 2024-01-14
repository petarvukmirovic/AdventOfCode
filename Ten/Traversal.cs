namespace Ten
{
    public enum Direction
    {
        Left = 1,
        Right = -1,
        Up = -1,
        Down = 1,
    }

    public record Position(int i, int j)
    {
        public bool IsValidFor(char[][] matrix) => 
            i >= 0 && i < matrix.Length && 
            j >= 0 && j < matrix[0].Length;
    }

    public record TraversalNode(Position current, (int i, int j) nextStepMovement)
    {
        private static readonly (int i, int j) Left = (0, -1);
        private static readonly (int i, int j) Right = (0, 1);
        private static readonly (int i, int j) Up = (-1, 0);
        private static readonly (int i, int j) Down = (1, 0);

        public TraversalNode? Advance(char[][] pipeMatrix)
        {
            Position nextPosition = new Position(current.i + nextStepMovement.i, current.j + nextStepMovement.j);
            return nextPosition.IsValidFor(pipeMatrix) ? GetPossibleBffNode(nextPosition, pipeMatrix) : null;
        }

        private TraversalNode? GetPossibleBffNode(Position nextPosition, char[][] pipeMatrix)
        {
            var currentPipe = pipeMatrix[current.i][current.j];
            var nextPipe = pipeMatrix[nextPosition.i][nextPosition.j];
            return currentPipe switch
            {
                '-' => nextPipe switch
                {
                    'L' => MoveToPosition(nextPosition, movementDirection: Up, movementCondition: dir => dir.j == -1),
                    'J' => MoveToPosition(nextPosition, movementDirection: Up, movementCondition: dir => dir.j == 1),
                    '7' => MoveToPosition(nextPosition, movementDirection: Down, movementCondition: dir => dir.j == 1),
                    'F' => MoveToPosition(nextPosition, movementDirection: Down, movementCondition: dir => dir.j == -1),
                    _ => PipeOneOf(nextPipe, '-', 'S') ? MoveToPosition(nextPosition) : null
                },
                '|' => nextPipe switch
                {
                    'L' => MoveToPosition(nextPosition, movementDirection: Right, movementCondition: dir => dir.i == 1),
                    'J' => MoveToPosition(nextPosition, movementDirection: Left, movementCondition: dir => dir.i == 1),
                    '7' => MoveToPosition(nextPosition, movementDirection: Left, movementCondition: dir => dir.i == -1),
                    'F' => MoveToPosition(nextPosition, movementDirection: Right, movementCondition: dir => dir.i == -1),
                    _ => PipeOneOf(nextPipe, '|', 'S') ? MoveToPosition(nextPosition) : null
                },
                'L' => nextPipe switch
                {
                    '-' => MoveToPosition(nextPosition, movementCondition: dir => dir.j == 1),
                    '|' => MoveToPosition(nextPosition, movementCondition: dir => dir.i == -1),
                    'J' => MoveToPosition(nextPosition, movementDirection: Up, movementCondition: dir => dir.j == 1),
                    '7' => MoveToPosition(nextPosition, movementDirection: Down, movementCondition: dir => dir.j == 1) ??
                           MoveToPosition(nextPosition, movementDirection: Left, movementCondition: dir => dir.i == -1),
                    'F' => MoveToPosition(nextPosition, movementDirection: Right, movementCondition: dir => dir.i == -1),
                    _ => PipeOneOf(nextPipe, 'S') ? MoveToPosition(nextPosition) : null
                },
                'J' => nextPipe switch
                {
                    '-' => MoveToPosition(nextPosition, movementCondition: dir => dir.j == -1),
                    '|' => MoveToPosition(nextPosition, movementCondition: dir => dir.i == -1),
                    'L' => MoveToPosition(nextPosition, movementDirection: Up, movementCondition: dir => dir.j == -1),
                    'F' => MoveToPosition(nextPosition, movementDirection: Down, movementCondition: dir => dir.j == -1) ??
                           MoveToPosition(nextPosition, movementDirection: Right, movementCondition: dir => dir.i == -1),
                    '7' => MoveToPosition(nextPosition, movementDirection: Left, movementCondition: dir => dir.i == -1),
                    _ => PipeOneOf(nextPipe, 'S') ? MoveToPosition(nextPosition) : null
                },
                '7' => nextPipe switch
                {
                    '-' => MoveToPosition(nextPosition, movementCondition: dir => dir.j == -1),
                    '|' => MoveToPosition(nextPosition, movementCondition: dir => dir.i == 1),
                    'L' => MoveToPosition(nextPosition, movementDirection: Up, movementCondition: dir => dir.j == -1) ??
                           MoveToPosition(nextPosition, movementDirection: Right, movementCondition: dir => dir.i == 1),
                    'J' => MoveToPosition(nextPosition, movementDirection: Left, movementCondition: dir => dir.i == 1),
                    'F' => MoveToPosition(nextPosition, movementDirection: Down, movementCondition: dir => dir.j == -1),
                    _ => PipeOneOf(nextPipe, 'S') ? MoveToPosition(nextPosition) : null
                },
                'F' => nextPipe switch
                {
                    '-' => MoveToPosition(nextPosition, movementCondition: dir => dir.j == 1),
                    '|' => MoveToPosition(nextPosition, movementCondition: dir => dir.i == 1),
                    'L' => MoveToPosition(nextPosition, movementDirection: Right, movementCondition: dir => dir.i == 1),
                    'J' => MoveToPosition(nextPosition, movementDirection: Up, movementCondition: dir => dir.j == 1) ??
                           MoveToPosition(nextPosition, movementDirection: Left, movementCondition: dir => dir.i == 1),
                    '7' => MoveToPosition(nextPosition, movementDirection: Down, movementCondition: dir => dir.j == 1),
                    _ => PipeOneOf(nextPipe, 'S') ? MoveToPosition(nextPosition) : null
                },
                _ => null
            }; ;
        }

        public static bool MoveAlways((int i, int j) _) => true;
        private TraversalNode? MoveToPosition(Position nextPosition, (int i, int j)? movementDirection = null, Func<(int i, int j), bool>? movementCondition = null)
        {
            movementCondition ??= MoveAlways;
            return movementCondition(nextStepMovement) ? 
                new TraversalNode(nextPosition, movementDirection ?? nextStepMovement) : null;
        }
            

        private static bool PipeOneOf(char pipe, params char[] options) => options.Contains(pipe);
    }

    public static class Traversal
    {
        public static int FindMaxDistance(TraversalNode initialNode, char[][] pipeMatrix) => GetLoopLengthAndVerticalLines(initialNode, pipeMatrix).Item1;

        public static int FindNumberOfTilesInsideTheLoop(Position startPos, TraversalNode initialNode, char[][] pipeMatrix)
        {
            var verticalLines = GetLoopLengthAndVerticalLines(initialNode, pipeMatrix, startPos).Item2;
            var numberOfTilesInsideTheLoop = 0;
            for(int i=0; i < pipeMatrix.Length; i++)
            {
                if(verticalLines.TryGetValue(i, out var verticalLineIndices))
                {
                    for (int j = 0; j < verticalLineIndices.Count; j += 2)
                    {
                        var startIdx = verticalLineIndices[j];
                        var endIdx = verticalLineIndices[j + 1];
                        bool ignoreLine = (pipeMatrix[i][startIdx] == 'F' && pipeMatrix[i][endIdx] == '7') ||
                                          (pipeMatrix[i][startIdx] == 'L' && pipeMatrix[i][endIdx] == 'J') ||
                                          pipeMatrix[i][startIdx] == 'S' || pipeMatrix[i][endIdx] == 'S';
                        for (int colIdx = startIdx + 1; colIdx < endIdx; colIdx++)
                        {
                            if (!ignoreLine)
                            {
                                numberOfTilesInsideTheLoop++;
                            }
                        }
                    }
                }
            }
            return numberOfTilesInsideTheLoop;
        }


        private static (int, Dictionary<int, List<int>>) GetLoopLengthAndVerticalLines(TraversalNode initialNode, char[][] pipeMatrix, Position? startPos = null)
        {
            int steps = 1;
            Dictionary<int, List<int>> verticalLines = new();
            if(startPos != null)
            {
                verticalLines[startPos.i] = new() { startPos.j };
            }
            TraversalNode currentNode = initialNode;
            char currentPipe;
            while ((currentPipe = pipeMatrix[currentNode.current.i][currentNode.current.j]) != 'S')
            {
                if (currentPipe != '-')
                {
                    if (verticalLines.TryGetValue(currentNode.current.i, out var verticalIndicesList))
                    {
                        verticalIndicesList.Add(currentNode.current.j);
                    }
                    else
                    {
                        verticalLines[currentNode.current.i] = new() { currentNode.current.j };
                    }
                }
                currentNode = currentNode.Advance(pipeMatrix)!;
                steps++;
            }

            foreach((int row, List<int> verticalIdx) in verticalLines)
            {
                verticalIdx.Sort();
                List<int> withoutCurves = new();
                for(int i=0; i<verticalIdx.Count; i++)
                {
                    currentPipe = pipeMatrix[row][verticalIdx[i]];
                    int skipDoubleWallOffset = 0;
                    int idxToAdd = verticalIdx[i];
                    if(i<verticalIdx.Count-1)
                    {
                        char nextPipe = pipeMatrix[row][verticalIdx[i + 1]];
                        if((currentPipe == 'F' && nextPipe == 'J') ||
                           (currentPipe == 'L' && nextPipe == '7'))
                        {
                            var pipeToTake = i + (withoutCurves.Count % 2 == 1 ? 0 : 1);
                            idxToAdd =  verticalIdx[pipeToTake];
                            skipDoubleWallOffset = 1;
                        }
                    }
                    withoutCurves.Add(idxToAdd);
                    i += skipDoubleWallOffset;
                }
                verticalIdx.Clear();
                verticalIdx.AddRange(withoutCurves);
            }

            return (steps / 2, verticalLines);
        }

        public static TraversalNode[] GetInitialNodes(char[][] pipeMatrix, Position startPos)
        {
            var possibleMoves = new List<(int i, int j, (char, int)[] compatiblePipes)>()
                {
                    (0, 1, new [] {('-', 0), ('J', -1), ('7', 1)}),
                    (0, -1, new [] {('-',0), ('F', 1), ('L', -1)}),
                    (-1, 0, new [] {('|',0), ('7', -1), ('F', 1)}),
                    (1, 0, new [] {('|',0), ('J', -1), ('L', 1)}),
                };

            return possibleMoves.Select(move =>
            {
                var newPosition = new Position(startPos.i + move.i, startPos.j + move.j);
                TraversalNode? bfsNode = null;
                if (newPosition.IsValidFor(pipeMatrix))
                {
                    var pipe = pipeMatrix[newPosition.i][newPosition.j];
                    var newDirection = move.compatiblePipes.SingleOrDefault(pair => pair.Item1 == pipe);
                    if (newDirection != default)
                    {
                        var direction = (move.i, move.j);
                        if (newDirection.Item2 != 0)
                        {
                            direction = (move.i == 0 ? newDirection.Item2 : 0, move.j == 0 ? newDirection.Item2 : 0);
                        }
                        bfsNode = new TraversalNode(newPosition, direction);
                    }
                }
                return bfsNode;
            })
            .Where(node => node != null)
            .Select(node => node!).ToArray();
        }
    }

}
