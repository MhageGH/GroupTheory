Practice4_2_10.Execute();

class Practice4_2_2 // 群論入門 演習問題4.2.2
{
    public static void Execute()
    {
        var A = new AlternatingGroup(5);
        Console.WriteLine(string.Join("\r\n\r\n", A.conjugates.Select(x => string.Join("\r\n", x.Select(x => string.Join(", ", x.Select(x => x + 1)))))));
    }
}

class Practice4_2_10 // 群論入門 演習問題4.2.10
{
    public static void Execute()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        var groupChecker = new GroupChecker();
        var conjugatesDivider = new ConjugatesDivider();
        Console.WriteLine("Group");
        var G = new SymmetryGroup(4);
        Console.WriteLine(string.Join(" / ", G.elements.Select(x => string.Join(" ", x.Select(x => x + 1)))));
        Console.WriteLine();
        Console.WriteLine("SubGroupList");
        List<int> Ns = new();
        var N = G.elements.Count;
        for (int i = 1; i <= N; ++i) if (N % i == 0) Ns.Add(i);
        for (int i = 0; i < Ns.Count; i++)
        {
            Console.WriteLine("n = " + Ns[i]);
            var S = new SubGroupList(G, Ns[i]);
            var conjugates = conjugatesDivider.Divide(G.elements, S.elements);
            foreach (var c in conjugates)
            {
                foreach (var s in c) Console.WriteLine(string.Join(" / ", s.Select(x => string.Join(" ", x.Select(x => x + 1)))));
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        sw.Stop();
        Console.WriteLine("\r\ntime: " + sw.ToString());
    }
}

class SetUtility
{
    static bool IsEqual(int[] s1, int[] s2)
    {
        for (int i = 0; i < s1.Length; ++i) if (s1[i] != s2[i]) return false;
        return true;
    }

    public static bool IsEqual(List<int[]> subSet1, List<int[]> subSet2)
    {
        foreach (var s1 in subSet1) if (!IsIncluding(subSet2, s1)) return false;
        foreach (var s2 in subSet2) if (!IsIncluding(subSet1, s2)) return false;
        return true;
    }

    public static bool IsEqual(List<List<int[]>> subSets1, List<List<int[]>> subSets2)
    {
        foreach (var s1 in subSets1) if (!IsIncluding(subSets2, s1)) return false;
        foreach (var s2 in subSets2) if (!IsIncluding(subSets1, s2)) return false;
        return true;
    }

    public static bool IsIncluding(List<int[]> subset, int[] s)
    {
        foreach (var r in subset) if (IsEqual(s, r)) return true;
        return false;
    }

    public static bool IsIncluding(List<List<int[]>> subSets, List<int[]> subSet)
    {
        foreach (var s in subSets) if (IsEqual(s, subSet)) return true;
        return false;
    }

    public static bool IsIncluding(List<List<List<int[]>>> subSetss, List<List<int[]>> subSets)
    {
        foreach (var s in subSetss) if (IsEqual(s, subSets)) return true;
        return false;
    }
}

class ConjugatesDivider
{
    public List<List<List<int[]>>> Divide(List<int[]> group, List<List<int[]>> subGroupList)
    {
        var conjugates = new List<List<List<int[]>>>();
        foreach (var subGroup1 in subGroupList)
        {
            var s = new List<List<int[]>>() { subGroup1 };
            foreach (var subGroup2 in subGroupList) if (!SetUtility.IsEqual(subGroup1, subGroup2) && IsConjugate(group, subGroup1, subGroup2)) s.Add(subGroup2);
            if (!SetUtility.IsIncluding(conjugates, s)) conjugates.Add(s);
        }
        return conjugates;
    }

    bool IsConjugate(List<int[]> group, List<int[]> subGroup1, List<int[]> subGroup2)
    {
        foreach(var g in group)
        {
            var c = new List<int[]>();
            foreach (var s in subGroup1)
            {
                var h = new int[g.Length];
                for (int i = 0; i < g.Length; ++i) h[i] = g[s[Inverse(g)[i]]];
                c.Add(h);
            }
            if (SetUtility.IsEqual(c, subGroup2)) return true;
        }
        return false;
    }

    int[] Inverse(int[] elements)
    {
        var inv = new int[elements.Length];
        for (int i = 0; i < elements.Length; ++i) inv[i] = Array.IndexOf(elements, i);
        return inv;
    }
}

class GroupChecker
{
    public bool IsGroup(List<int[]> subSet)
    {
        return (IsIncludingIdentityElement(subSet) && IsIncludingInverseElementForAll(subSet) && IsClosed(subSet)) ? true : false;
    }

    bool IsIncludingIdentityElement(List<int[]> subset)
    {
        foreach (var element in subset) if (IsIdentityElement(element)) return true;
        return false;
    }

    bool IsIdentityElement(int[] element)
    {
        for (int i = 0; i < element.Length; ++i) if (element[i] != i) return false;
        return true;
    }

    bool IsIncludingInverseElement(List<int[]> subset, int[] s)
    {
        foreach (var r in subset) if (IsIdentityElement(s.Select(x => r[x]).ToArray())) return true;
        return false;
    }

    bool IsIncludingInverseElementForAll(List<int[]> subset)
    {
        foreach (var s in subset) if (!IsIncludingInverseElement(subset, s)) return false;
        return true;
    }


    bool IsClosed(List<int[]> subset)
    {
        foreach (var s1 in subset) foreach (var s2 in subset) if (!SetUtility.IsIncluding(subset, s1.Select(x => s2[x]).ToArray())) return false;
        return true;
    }
}


class SubGroupList
{
    public List<List<int[]>> elements = new();

    public SubGroupList(SymmetryGroup G, int n)
    {
        var indexes = new int[n];
        var groupList = new List<List<int[]>>();
        elements = RecursiveCreate(G, 0, n, ref indexes, groupList);
    }

    List<List<int[]>> RecursiveCreate(SymmetryGroup G, int j, int n, ref int[] indexes, List<List<int[]>> groupList)
    {
        var groupChecker = new GroupChecker();
        for (indexes[j] = j == 0 ? 0 : indexes[j - 1] + 1; indexes[j] < G.elements.Count; ++indexes[j])
        {
            if (j == n - 1)
            {
                var subSet = new List<int[]>();
                for (int i = 0; i < n; ++i) subSet.Add(G.elements[indexes[i]]);
                if (SetUtility.IsIncluding(groupList, subSet)) continue;
                if (groupChecker.IsGroup(subSet)) groupList.Add(subSet);
            }
            else RecursiveCreate(G, j + 1, n, ref indexes, groupList);
        }
        return groupList;
    }
}

class SymmetryGroup
{
    public List<int[]> elements = new();

    public SymmetryGroup(int N)
    {
        var indexes = new int[N];
        RecursiveCreate(0, N, ref indexes);
    }

    void RecursiveCreate(int j, int N, ref int[] indexes)
    {
        for (indexes[j] = 0; indexes[j] < N; ++indexes[j])
        {
            var b = false;
            for (int i = 0; i < j; ++i) b |= indexes[j] == indexes[i];
            if (b) continue;
            if (j == N - 1)
            {
                var r = new int[N];
                for (int i = 0; i < N; ++i) r[i] = indexes[i];
                elements.Add(r);
            }
            else RecursiveCreate(j + 1, N, ref indexes);
        }
    }
}

class AlternatingGroup
{
    public List<List<int[]>> conjugates = new();
    public List<int[]> elements = new();
    public AlternatingGroup(int N)
    {
        var S = new SymmetryGroup(N);
        foreach (var s in S.elements)
        {
            int p = 1;
            for (int i = 0; i < s.Length; ++i) for (int j = 0; j < s.Length; ++j)
                    if (i > j) p *= s[i] > s[j] ? 1 : -1;
            if (p == 1) elements.Add(s);
        }
        foreach (var a in elements)
        {
            var isConjugate = false;
            for (int j = 0; j < conjugates.Count; ++j)
            {
                foreach (var e in conjugates[j])
                {
                    if (isConjugate) break;
                    foreach (var g in elements)
                    {
                        var b = true;
                        for (int i = 0; i < N; ++i) if (a[g[i]] != g[e[i]]) b = false;
                        if (b) // aがeと共役なら、ループを抜けてaをcに入れる
                        {
                            isConjugate = true;
                            break;
                        }
                    }
                }
                if (isConjugate)
                {
                    conjugates[j].Add(a);
                    break;
                }
            }
            if (!isConjugate) conjugates.Add(new List<int[]> { a });
        }
    }
}